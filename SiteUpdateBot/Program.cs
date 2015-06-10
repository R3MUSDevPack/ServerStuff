using Hipchat_Plugin;
using r3mus.Models;
using r3mus.ViewModels;
using Slack_Plugin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SiteUpdateBot
{
    class Program
    {      
        static void Main(string[] args)
        {

            DateTime checkDT = DateTime.Now;
            //UpdateRunTime(checkDT);
            //return;
            DateTime lastFullRunTime = GetLastRunTime();
            bool doFullRun = ((checkDT - lastFullRunTime).TotalHours > 23);

            Console.WriteLine(string.Format("Last Run Time {0}", lastFullRunTime));
            Console.WriteLine(string.Format("Full Run {0}", doFullRun.ToString()));

            //GetWebsiteUserDetails(doFullRun);
            //UpdateMailees();

            NotifyApplicationChanges(lastFullRunTime);

            ResetMailees(doFullRun);
            while (UpdateMailees())
            {
                System.Threading.Thread.Sleep(2000);
            }

            if(doFullRun)
            {
                UpdateRunTime(checkDT);
            }

            Console.WriteLine("Complete!");

            System.Threading.Thread.Sleep(1000);
        }

        public static void ResetMailees(bool reset)
        {
            if (reset)
            {
                Console.WriteLine("Resetting mailees corpId's to 0");
                var r3musDB = new r3musDbContext();
                r3musDB.RecruitmentMailees.ToList().ForEach(mailee => mailee.CorpId_AtLastCheck = 0);
                r3musDB.SaveChanges();
            }
        }

        //public static void GetWebsiteUserDetails(bool FullUpdate)
        //{
        //    try
        //    {
        //        var r3musDB = new r3musDbContext();
        //        var users = r3musDB.Users;

        //        if (FullUpdate)
        //        {
        //            Console.WriteLine("Updating all {0} users", users.Count());
        //            var counter = 0;
        //            foreach (var user in users)
        //            {
        //                try
        //                {
        //                    var cUsrTitles = user.Titles;

        //                    counter++;
        //                    Console.WriteLine(string.Format("Updating #{0} user {1}", counter.ToString(), user.UserName));
        //                    user.GetDetails(true);

        //                    r3musDB.Titles.Where(dbTitle => dbTitle.UserId == user.Id).ToList().Where(dbTitle => cUsrTitles.Any(cUsrTitle => dbTitle.TitleName == cUsrTitle.TitleName)).ToList().ForEach(dbTitle => r3musDB.Entry(dbTitle).State = EntityState.Unchanged);
        //                    r3musDB.Titles.Where(dbTitle => dbTitle.UserId == user.Id).ToList().Where(dbTitle => !cUsrTitles.Any(cUsrTitle => dbTitle.TitleName == cUsrTitle.TitleName)).ToList().ForEach(dbTitle => r3musDB.Entry(dbTitle).State = EntityState.Deleted);

        //                }
        //                catch (Exception ex) { Console.WriteLine(string.Format("User: {0}, Error {1}", user.UserName, ex.Message)); }
        //            }
        //            r3musDB.Users = users;
        //        }
        //        else
        //        {
        //            var userList = users.Where(user => user.Avatar == null);
        //            Console.WriteLine("Updating {0} users", userList.Count());
        //            foreach (var user in userList)
        //            {
        //                try
        //                {
        //                    Console.WriteLine(string.Format("Updating user {0}", user.UserName));
        //                    user.GetDetails(true);
        //                }
        //                catch (Exception ex) { Console.WriteLine(string.Format("User: {0}, Error {1}", user.UserName, ex.Message)); }
        //            }
        //        }
        //        r3musDB.SaveChanges();
        //    }
        //    catch (Exception ex) { }
        //}

        private static DateTime GetLastRunTime()
        {
            try
            {
                return Convert.ToDateTime(ConfigurationSettings.AppSettings["LastCheckedAt"]);
            }
            catch(Exception ex)
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

        private static bool UpdateMailees()
        {
            var r3musDB = new r3musDbContext();
            var mailees = r3musDB.RecruitmentMailees.Where(mailee => mailee.CorpId_AtLastCheck == 0).Take(30).ToList();
            int i = 0;
            Stopwatch sw = new Stopwatch();

            Console.WriteLine(string.Join(", ", mailees.ToList().Select(mailee => mailee.Name).ToList()));

            sw.Start();

            foreach (var mailee in mailees)
            {
                try
                {
                    i++;
                    Console.WriteLine("# {0}: Mailee {1}", i.ToString(), mailee.Name);
                    mailee.IsInNPCCorp();
                    //System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex) { Console.WriteLine(string.Format("Mailee: {0}, Error {1}", mailee.Name, ex.Message)); }
                //if ((i % 20) == 0)
                //{
                //    r3musDB.SaveChanges();
                //    Console.WriteLine("Pausing...");
                //    System.Threading.Thread.Sleep(15000);
                //}
            }

            sw.Stop();
            Console.WriteLine("{0} mailees processed in {1}", mailees.Count().ToString(), sw.Elapsed.ToString());
            
            r3musDB.SaveChanges();

            return (r3musDB.RecruitmentMailees.Where(mailee => mailee.CorpId_AtLastCheck == 0).Count() > 0);
        }

        private static void NotifyApplicationChanges(DateTime lastRunTime)
        {
            var r3musDB = new ApplicantEntities();
            var lastCheck = DateTime.Now.AddMinutes(-1);
            var applications = r3musDB.ApplicantLists.Where(applicant => applicant.LastStatusUpdate >= lastCheck).ToList();

            Console.WriteLine(string.Format("Running applications check from {0}.", lastCheck.ToString("yyyy-MM-dd HH:mm:ss")));

            foreach(var application in applications)
            {
                if(application.Status == ApplicationReviewViewModel.ApplicationStatus.Applied.ToString())
                {
                    SendMessage(string.Format(Properties.Settings.Default.NewApp_MessageFormatLine1, application.Name, application.DateTimeCreated.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                else
                {
                    SendMessage(string.Format(Properties.Settings.Default.AppUpdate_MessageFormatLine2, application.Name, application.Status, application.UserName, application.DateTimeCreated.ToString("yyyy-MM-dd HH:mm:ss")));
                }
            }
            //if(applications.Count == 0)
            //{
            //    SendMessage("I am a robot. Beep.");
            //}
        }
        
        private static void SendMessage(string message)
        {
            if (Properties.Settings.Default.Plugin.ToUpper() == "HIPCHAT")
            {
                Hipchat.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.HipchatToken);
            }
            else if (Properties.Settings.Default.Plugin.ToUpper() == "SLACK")
            {
                Slack.SendToRoom(message, Properties.Settings.Default.RoomName, Properties.Settings.Default.SlackWebhook);
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
    }
}
