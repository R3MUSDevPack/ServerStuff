﻿using Hipchat_Plugin;
using JKON.Slack;
using r3mus.Models;
using r3mus.ViewModels;
using Slack_Plugin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteUpdateBot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DateTime checkDT = DateTime.Now;
                DateTime lastFullRunTime = GetLastRunTime();
                bool doFullRun = ((checkDT - lastFullRunTime).TotalHours > 23);

                Console.WriteLine(string.Format("Last Run Time {0}", lastFullRunTime));

                long totalCount, runCount = 0;

                MakeAnnoucements();

                using (var r3musDB = new r3musDbContext())
                {
                    ResetMailees(true);

                    if (doFullRun)
                    {
                        UpdateRunTime(checkDT);
                    }
                    totalCount = r3musDB.RecruitmentMailees.Where(mailee => (mailee.CorpId_AtLastCheck == 0) && (mailee.Mailed == null)).Count();

                    while (UpdateMailees() && (runCount < 10000))
                    {
                        runCount = (totalCount - r3musDB.RecruitmentMailees.Where(mailee => (mailee.CorpId_AtLastCheck == 0) && (mailee.Mailed == null)).Count());
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                Console.WriteLine("Complete!");

                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (Environment.UserInteractive)
                {
                    Console.ReadLine();
                }
            }
        }

        public static void ResetMailees(bool reset)
        {
            if (reset)
            {
                var r3musDB = new r3musDbContext();

                r3musDB.RecruitmentMailees.Where(mailee =>
                    (mailee.MailerId == null)
                    && (!mailee.Name.Contains("Citizen"))
                    && (!mailee.Name.Contains("Trader"))
                    && (!mailee.Name.Contains("Miner"))
                    && (DbFunctions.DiffHours(mailee.Submitted, DateTime.Now) >= 24)
                    && (DbFunctions.DiffHours(mailee.LastUpdated, DateTime.Now) >= 24))
                    .Take(500).ToList().ForEach(mailee => mailee.CorpId_AtLastCheck = 0);

                var count = r3musDB.SaveChanges();
                Console.WriteLine(string.Format("Reset {0} mailees corpId's to 0",
                    count.ToString()));
            }
        }

        private static string GenerateSaltedHash(string plainText, string salt)
        {
            // http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            // Combine the two lists
            var plainTextWithSaltBytes = new List<byte>(plainTextBytes.Length + saltBytes.Length);
            plainTextWithSaltBytes.AddRange(plainTextBytes);
            plainTextWithSaltBytes.AddRange(saltBytes);

            // Produce 256-bit hashed value i.e. 32 bytes
            HashAlgorithm algorithm = new SHA256Managed();
            var byteHash = algorithm.ComputeHash(plainTextWithSaltBytes.ToArray());
            return Convert.ToBase64String(byteHash);
        }

        public static void SyncUsers()
        {
            var r3musDB = new r3musDbContext();
            var r3musForum = new r3mus_ForumDBEntities();

            try
            {
                var siteUsers = r3musDB.Users.ToList();
                var forumUsers = r3musForum.MembershipUsers.ToList();

                var forumRole = r3musForum.MembershipRoles.Where(role => role.RoleName == "Standard Members").FirstOrDefault();

                siteUsers.ForEach(siteUser =>
                {
                    var forumUser = forumUsers.Where(user => user.UserName == siteUser.UserName).FirstOrDefault();
                    if (forumUser == null)
                    {
                        forumUser = new MembershipUser()
                        {
                            Id = new Guid(siteUser.Id),
                            UserName = siteUser.UserName,
                            Password = string.Concat("R3MUSUser_", siteUser.UserName.Substring(0, siteUser.UserName.IndexOf(" "))),
                            Email = siteUser.EmailAddress,
                            PasswordSalt = string.Concat("R3MUS_", siteUser.UserName.Substring(0, siteUser.UserName.IndexOf(" "))),
                            IsApproved = true,
                            IsLockedOut = false,
                            CreateDate = DateTime.Now,
                            LastLoginDate = DateTime.Now,
                            LastPasswordChangedDate = DateTime.Now,
                            LastLockoutDate = DateTime.Now,
                            FailedPasswordAttemptCount = 3,
                            FailedPasswordAnswerAttempt = 3,
                            Slug = string.Empty,
                            DisableEmailNotifications = true,
                            IsExternalAccount = true
                        };
                        forumUser.Password = GenerateSaltedHash(forumUser.Password, forumUser.PasswordSalt);
                        r3musForum.MembershipUsers.Add(forumUser);

                        var role = new MembershipUsersInRole()
                        {
                            Id = Guid.NewGuid(),
                            UserIdentifier = forumUser.Id,
                            RoleIdentifier = forumRole.Id,
                            MembershipUser = forumUser,
                            MembershipRole = forumRole
                        };

                        r3musForum.MembershipUsersInRoles.Add(role);
                    }
                }
                    );
                r3musForum.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void GetWebsiteUserDetails(bool FullUpdate)
        {
            try
            {
                var r3musDB = new r3musDbContext();
                var users = r3musDB.Users;

                if (FullUpdate)
                {
                    Console.WriteLine("Updating all {0} users", users.Count());
                    var counter = 0;
                    foreach (var user in users)
                    {
                        try
                        {
                            var cUsrTitles = user.Titles;

                            counter++;
                            Console.WriteLine(string.Format("Updating #{0} user {1}", counter.ToString(), user.UserName));
                            user.GetDetails(true);

                            r3musDB.Titles.Where(dbTitle => dbTitle.UserId == user.Id).ToList().Where(dbTitle => cUsrTitles.Any(cUsrTitle => dbTitle.TitleName == cUsrTitle.TitleName)).ToList().ForEach(dbTitle => r3musDB.Entry(dbTitle).State = EntityState.Unchanged);
                            r3musDB.Titles.Where(dbTitle => dbTitle.UserId == user.Id).ToList().Where(dbTitle => !cUsrTitles.Any(cUsrTitle => dbTitle.TitleName == cUsrTitle.TitleName)).ToList().ForEach(dbTitle => r3musDB.Entry(dbTitle).State = EntityState.Deleted);
                        }
                        catch (Exception ex) { Console.WriteLine(string.Format("User: {0}, Error {1}", user.UserName, ex.Message)); }
                    }
                    //users.Where(user => !user.IsValid()).ToList().ForEach(dbTitle => r3musDB.Entry(dbTitle).State = EntityState.Deleted);
                    r3musDB.Users = users;
                }
                else
                {
                    var userList = users.Where(user => user.Avatar == null);
                    Console.WriteLine("Updating {0} users", userList.Count());
                    foreach (var user in userList)
                    {
                        try
                        {
                            Console.WriteLine(string.Format("Updating user {0}", user.UserName));
                            user.GetDetails(true);
                        }
                        catch (Exception ex) { Console.WriteLine(string.Format("User: {0}, Error {1}", user.UserName, ex.Message)); }
                    }
                }
                r3musDB.SaveChanges();
            }
            catch (Exception ex) { }
        }

        private static DateTime GetLastRunTime()
        {
            try
            {
                return Convert.ToDateTime(ConfigurationSettings.AppSettings["LastCheckedAt"]);
            }
            catch (Exception ex)
            {
                return new DateTime();
            }
        }

        private static DateTime GetLastAnnouncementTime()
        {
            try
            {
                return Convert.ToDateTime(ConfigurationSettings.AppSettings["LastAnnounceDT"]);
            }
            catch (Exception ex)
            {
                return new DateTime();
            }
        }

        private static void UpdateRunTime(DateTime writeThis)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(string.Format("Updating Last Run Time in {0}", config.FilePath));
            config.AppSettings.Settings.Remove("LastCheckedAt");
            config.AppSettings.Settings.Add("LastCheckedAt", writeThis.ToString("yyyy-MM-dd HH:mm:ss"));
            config.Save();
        }

        private static void UpdateAnnouncementTime(DateTime writeThis)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(string.Format("Updating Last Announcement Time in {0}", config.FilePath));
            config.AppSettings.Settings.Remove("LastAnnounceDT");
            config.AppSettings.Settings.Add("LastAnnounceDT", writeThis.ToString("yyyy-MM-dd HH:mm:ss"));
            config.Save();
        }

        private static bool UpdateMailees()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int i = 0;

            using (var r3musDB = new r3musDbContext())
            {
                r3musDB.RecruitmentMailees.Where(mailee => mailee.CorpId_AtLastCheck == 0).Take(30).ToList().ForEach(mailee =>
                {
                    try
                    {
                        i++;
                        Console.WriteLine("# {0}: Mailee {1}", i.ToString(), mailee.Name);
                        mailee.GetToonDetails();
                        mailee.LastUpdated = DateTime.Now;
                    }
                    catch (Exception ex) { Console.WriteLine(string.Format("Mailee: {0}, Error {1}", mailee.Name, ex.Message)); }
                });
                r3musDB.SaveChanges();
                sw.Stop();
                Console.WriteLine("{0} mailees processed in {1}", i.ToString(), sw.Elapsed.ToString());

                GC.Collect();
                GC.WaitForPendingFinalizers();
                return (r3musDB.RecruitmentMailees.Where(mailee => mailee.CorpId_AtLastCheck == 0).Count() > 0);
            }
        }

        private static void NotifyApplicationChanges(DateTime lastRunTime)
        {
            var r3musDB = new ApplicantEntities();
            //var lastCheck = DateTime.Now.AddMinutes(-1);
            var applications = r3musDB.ApplicantLists.Where(applicant => applicant.LastStatusUpdate >= lastRunTime).ToList();

            Console.WriteLine(string.Format("Running applications check from {0}.", lastRunTime.ToString("yyyy-MM-dd HH:mm:ss")));

            foreach (var application in applications)
            {
                if (application.Status == ApplicationReviewViewModel.ApplicationStatus.Applied.ToString())
                {
                    SendMessage(string.Format(Properties.Settings.Default.NewApp_MessageFormatLine1, application.Name, application.DateTimeCreated.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                else
                {
                    SendMessage(string.Format(Properties.Settings.Default.AppUpdate_MessageFormatLine2, application.Name, application.Status, application.UserName, application.DateTimeCreated.ToString("yyyy-MM-dd HH:mm:ss")));
                }
            }
            if (Environment.UserInteractive)
            {
                SendMessage("I am a robot. Beep.");
            }
        }

        private static void SendMessage(string message)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                Hipchat.SendToRoom(message, Properties.Settings.Default.RecruitmentRoomName, Properties.Settings.Default.HipchatToken);
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                Slack.SendToRoom(message, Properties.Settings.Default.RecruitmentRoomName, Properties.Settings.Default.SlackWebhook);
            }
        }

        private static void SendPM(string message)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                Hipchat.SendPM(message, "Clyde en Marland", Properties.Settings.Default.HipchatToken);
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                Slack.SendPM(message, "Clyde en Marland", Properties.Settings.Default.SlackWebhook);
            }
        }

        private static void MakeAnnoucements()
        {
            var lastAnnouncementTime = GetLastAnnouncementTime();
            var announcements = new r3musDbContext().Announcements.Where(ann => ann.Date > lastAnnouncementTime).ToList();

            announcements.ForEach(ann =>
            {
                ann.Post = ann.Post.Replace("\r", "");
                ann.Post = Regex.Replace(ann.Post, @"<[^>]+>|&nbsp;", "").Trim();
                ann.Post = Regex.Replace(ann.Post, @"\s{2,}", " ");
                SendMessage(HyperFormatKillMessage(ann), Properties.Settings.Default.NewsRoomName);
            });
            if (announcements.Count() > 0)
            { 
                UpdateAnnouncementTime(announcements.LastOrDefault().Date);
            }
        }
        private static void SendPM(MessagePayload message)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                //Hipchat.SendPM(message, Properties.Settings.Default.HipchatToken, "Clyde en Marland");
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                Slack.SendPM(message, "ClydeenMarland", Properties.Settings.Default.SlackWebhook);
            }
        }

        private static void SendMessage(MessagePayload message, string room)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                //Hipchat.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.HipchatToken);
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                //message = Linkify(message);
                Slack.SendToRoom(message, room, Properties.Settings.Default.SlackWebhook);
            }
        }

        private static MessagePayload HyperFormatKillMessage(Announcement ann)
        {
            MessagePayload message = new MessagePayload();
            message.Attachments = new List<MessagePayloadAttachment>();
            message.Text = "CODE PINK";

            message.Attachments.Add(new MessagePayloadAttachment()
            {
                Text = ann.Post,
                TitleLink = string.Format("http://forums.r3mus.org/chat/{0}", ann.Topic.ToLower().Replace(" ", "-")),
                Title = ann.Topic,
                ThumbUrl = "http://www.r3mus.org/Images/logo.png",
                AuthorName = ann.UserName,
                AuthorIcon = string.Format("http://image.eveonline.com/Character/{0}_32.jpg", JKON.EveWho.Api.GetCharacterID(ann.UserName)),
                Colour = "#FF3399"
            });
            
            return message;
        }
    }
}
