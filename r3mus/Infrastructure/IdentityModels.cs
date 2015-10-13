using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

using r3mus.Infrastructure;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using EveAI.Live;
using System.IO;
//using eZet.EveLib.EveWhoModule.Models;
using eZet.EveLib.EveXmlModule;

namespace r3mus.Models
{
    public class ApplicationUser : IdentityUser, IApplicationUser
    { 
        private ApplicationDbContext db = new ApplicationDbContext();

        public IList<ApiInfo> ApiKeys { get; set; }

        public bool Errored { get; set; }
        public string ErrorMessage { get; set; }

        public IList<Title> Titles { get; set; }

        public string MemberType { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Member Since")]
        public DateTime? MemberSince { get; set; }

        public string Avatar { get; set; }

        public enum IDType
        {
            Corporation,
            Alliance,
            Plus10,
            [Display(Name="Honoured Guest")]
            Guest
        }

        public ApplicationUser()
        {
            ApiKeys = new List<ApiInfo>();
            Titles = new List<Title>();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void AddApiInfo(string api, string vcode)
        {
            ApiKeys.Add(new ApiInfo() { User = this, ApiKey = Convert.ToInt32(api), VerificationCode = vcode });
            //ApiKeys.Add(new ApiInfo() { User_Id = this.Id, ApiKey = Convert.ToInt32(api), VerificationCode = vcode });
        }

        public bool IsValid()
        {
            //return true;
            bool result = false;
            bool unloadApis = false;

            try
            {
                        
                long hostCorpID = GetCorpOrAllianceId(IDType.Corporation, Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);
                long hostAllianceID = GetCorpOrAllianceId(IDType.Alliance, Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);

                if(this.ApiKeys.Count == 0)
                {
                    unloadApis = this.LoadApiKeys();
                }

                foreach (ApiInfo apiInfo in this.ApiKeys)
                {
                    long clientCorpId = GetCorpOrAllianceId(IDType.Corporation, apiInfo.ApiKey, apiInfo.VerificationCode);
                    long clientAllianceId = GetCorpOrAllianceId(IDType.Alliance, apiInfo.ApiKey, apiInfo.VerificationCode);

                    if (clientCorpId == hostCorpID)
                    {
                        MemberType = IDType.Corporation.ToString();
                        result = apiInfo.ValidateAccessMask(IDType.Corporation);
                    }
                    else if (clientAllianceId == hostAllianceID)
                    {
                        MemberType = IDType.Alliance.ToString();
                        result = apiInfo.ValidateAccessMask(IDType.Alliance);
                    }
                    else
                    {
                        //  Get standings
                        result = ValidateStandings(UserName, clientCorpId, clientAllianceId, Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);
                    }
                    if(result)
                    {
                        break;
                    }
                }
                if(unloadApis)
                {
                    this.UnloadApiKeys();
                }
                Errored = false;

            }
            catch(Exception ex)
            {
                Errored = true;
                ErrorMessage = ex.Message;
            }

            return result;
        }

        public long GetToonId(IDType type, int apiKey, string vcode)
        {
            long result = -1;

            try
            {
                var newkey = EveXml.CreateApiKey(apiKey, vcode).Init();
                if ((newkey.IsValidKey()) && newkey.KeyType != (ApiKeyType.Corporation))
                {
                    var cKey = (CharacterKey)newkey.GetActualKey();
                    Character toon = null;

                    try
                    {
                        toon = cKey.Characters.Single(c => c.CharacterName == this.UserName);
                    }
                    catch (Exception ex) { }

                    if (toon != null)
                    {
                        if (type == IDType.Corporation)
                        {
                            result = toon.CharacterId;
                        }
                    }
                }
                else if ((newkey.IsValidKey()) && newkey.KeyType == (ApiKeyType.Corporation))
                {
                    var cKey = (CorporationKey)newkey.GetActualKey();
                    Corporation corp = cKey.Corporation;
                    if (corp != null)
                    {
                        if (type == IDType.Corporation)
                        {
                            result = corp.GetCorporationSheet().Result.CeoId;
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return result;
        }

        public long GetCorpOrAllianceId(IDType type, int apiKey, string vcode)
        {
            long result = -1;

            try
            {
                var newkey = EveXml.CreateApiKey(apiKey, vcode).Init();
                if ((newkey.IsValidKey()) && newkey.KeyType != (ApiKeyType.Corporation))
                {
                    var cKey = (CharacterKey)newkey.GetActualKey();
                    Character toon = null;

                    try
                    {
                        //var things = cKey.Characters;
                        toon = cKey.Characters.Single(c => c.CharacterName.ToUpper() == this.UserName.ToUpper());
                    }
                    catch (Exception ex) { }

                    if (toon != null)
                    {
                        if (type == IDType.Corporation)
                        {
                            result = toon.CorporationId;
                        }
                        else if (type == IDType.Alliance)
                        {
                            result = toon.AllianceId;
                        }
                    }
                }
                else if ((newkey.IsValidKey()) && newkey.KeyType == (ApiKeyType.Corporation))
                {
                    var cKey = (CorporationKey)newkey.GetActualKey();
                    Corporation corp = cKey.Corporation;
                    if (corp != null)
                    {
                        if (type == IDType.Corporation)
                        {
                            result = corp.CorporationId;
                        }
                        else if (type == IDType.Alliance)
                        {
                            result = corp.AllianceId;
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return result;
        }

        public bool ValidateStandings(string checkName, long checkCorpId, long checkAllianceId, int apiKey, string vcode)
        {
            bool result = false;
            List<double> standings = new List<double>();
            
            try
            {
                var newkey = EveXml.CreateApiKey(apiKey, vcode).Init();
                var cKey = (CorporationKey)newkey.GetActualKey();

                List<long> list = new List<long>();
                list.Add(checkCorpId);
                if(checkAllianceId > 0)
                {
                    list.Add(checkAllianceId);
                }

                var api = new EveApi("R3MUS Recruitment", Convert.ToInt64(apiKey), vcode);

                Dictionary<long, string> dict = api.ConvertIDsToNames(list);

                string checkCorpName = dict[checkCorpId];
                string checkAllianceName = string.Empty;
                if(dict.Count == 2)
                {
                    checkAllianceName = dict[checkAllianceId];
                }

                var cList = cKey.Corporation.GetContactList().Result;
                //var corpList = cList.CorporationContacts;
                //var allList = cList.AllianceContacts;

                var contactList = cList.CorporationContacts.Concat(cList.AllianceContacts);

                var filteredContactList = contactList.Where
                    (c =>
                        (c.ContactName == checkName) || (c.ContactName == checkCorpName) || (c.ContactName == checkAllianceName)
                    );

                foreach(var contact in filteredContactList)
                {
                    standings.Add(contact.Standing);
                    result = (contact.Standing == 10);
                    if(result)
                    {
                        break;
                    }
                }

                //eZet.EveLib.Modules.Models.Corporation.ContactList.Contact contact = corpList.Where(c => c.ContactName == checkName).FirstOrDefault();
                //eZet.EveLib.Modules.Models.Corporation.ContactList.Contact contactCorp = corpList.Where(c => c.ContactName == checkName).FirstOrDefault();
                //eZet.EveLib.Modules.Models.Corporation.ContactList.Contact contactAlliance = corpList.Where(c => c.ContactName == checkName).FirstOrDefault();

                //if (contact != null)
                //{
                //    result = (contact.Standing == 10);
                //}
                //else if (contactCorp != null)
                //{
                //    result = (contactAlliance.Standing == 10);
                //}
                //else if (contactAlliance != null)
                //{
                //    result = (contactCorp.Standing == 10);
                //}
                //else
                //{
                //    contact = allList.Where(c => c.ContactName == checkName).FirstOrDefault();
                //    contactCorp = allList.Where(c => c.ContactName == checkName).FirstOrDefault();
                //    contactAlliance = allList.Where(c => c.ContactName == checkName).FirstOrDefault();
                //}

                //if (contact != null)
                //{
                //    result = (contact.Standing == 10);
                //}
                //else if (contactCorp != null)
                //{
                //    result = (contactAlliance.Standing == 10);
                //}
                //else if (contactAlliance != null)
                //{
                //    result = (contactCorp.Standing == 10);
                //}
            }
            catch(Exception ex)
            { }

            if(result)
            {
                MemberType = IDType.Plus10.ToString();
            }
            else
            {
                if((standings.Count == 0) || (standings.Min() >= 0.0))
                {
                    MemberType = IDType.Guest.ToString();
                }
            }

            return result;
        }

        public void GetDetails(bool JustDoIt = false)
        {
            bool unloadApis = false;

            if (ApiKeys.Count == 0)
            {
                unloadApis = LoadApiKeys();
            }
            ApiInfo liveAPI = new ApiInfo();

            if((MemberSince == null) || (Avatar == null) || (JustDoIt))
            {
                foreach (ApiInfo info in ApiKeys)
                {
                    try
                    {
                        var newkey = EveXml.CreateApiKey(info.ApiKey, info.VerificationCode).Init();
                        if ((newkey.IsValidKey()) && newkey.KeyType != (ApiKeyType.Corporation))
                        {
                            var cKey = (CharacterKey)newkey.GetActualKey();
                            Character toon = null;

                            try
                            {
                                toon = cKey.Characters.Single(c => c.CharacterName == UserName);
                            }
                            catch (Exception ex) { }

                            if (toon != null)
                            {
                                liveAPI = info;

                                MemberSince = toon.GetCharacterInfo().Result.CorporationDate;

                                //var tempAvatar = ImageServer.DownloadCharacterImage(toon.CharacterId, ImageServer.ImageSize.Size128px);
                                //using (var stream = new MemoryStream())
                                //{
                                //    tempAvatar.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                //    Avatar = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(stream.ToArray()));
                                //}
                                //tempAvatar.Dispose();

                                Avatar = GetAvatar(toon.CharacterId, ImageServer.ImageSize.Size128px);

                                break;
                            }
                        }
                    }
                    catch(Exception ex)
                    { }
                }
                if(MemberType == IDType.Corporation.ToString())
                {
                    GetTitles(liveAPI);
                }
            }
            if (unloadApis)
            {
                UnloadApiKeys();
            }
        }

        public static string GetAvatar(long charId, ImageServer.ImageSize size)
        {
            string avatar;

            var tempAvatar = ImageServer.DownloadCharacterImage(charId, size);
            using (var stream = new MemoryStream())
            {
                tempAvatar.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                avatar = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(stream.ToArray()));
            }
            tempAvatar.Dispose();
            return avatar;
        }

        public void GetTitles(ApiInfo liveAPI)
        {
            var corp = ((CorporationKey)EveXml.CreateApiKey(Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode).Init().GetActualKey()).Corporation;
            var titles = corp.GetMemberTracking().Result.Members.Where(member => member.CharacterName == UserName).FirstOrDefault().Title.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            var titleList = new List<Title>();
            foreach (var title in titles)
            {
                titleList.Add(new Title() { UserId = Id, TitleName = title });
            }
            Titles = titleList;
            if ((GetToonId(IDType.Corporation, liveAPI.ApiKey, liveAPI.VerificationCode) == GetToonId(IDType.Corporation, Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode))
                    &&
                    (!this.Titles.Any(title => title.TitleName == "CEO"))
                )
            {
                Titles.Add(new Title() { UserId = this.Id, TitleName = "CEO" });
            }
        }

        public bool LoadApiKeys()
        {
            if(ApiKeys.Count == 0)
            { 
                ApiKeys = db.ApiInfoes.Where(user => user.User.Id == Id).Distinct<ApiInfo>().ToList<ApiInfo>();
            }
            return true;
        }

        public void UnloadApiKeys()
        {
            ApiKeys = new List<ApiInfo>();
        }
    }

    //public partial class IdentityUser
    //{

    //}

    public class ApiInfo
    {
        public int Id { get; set; }

        [Display(Name = "Api Key")]
        public int ApiKey { get; set; }

        [Display(Name = "Verification Code")]
        [DataType(DataType.MultilineText)]
        [UIHint("DisplayVCode")]
        public string VerificationCode { get; set; }

        [NotMapped]
        [Display(Name = "Access Mask")]
        [UIHint("AccessMaskHighlight")]
        public long AccessMask { get { try { return EveXml.CreateApiKey(Convert.ToInt32(ApiKey), VerificationCode).Init().AccessMask; } catch (Exception ex) { return -1; } } }

        public virtual ApplicationUser User { get; set; }

        public List<Character> GetDetails()
        {
            CharacterKey key = EveXml.CreateCharacterKey(ApiKey, VerificationCode);
            var chars = key.Characters.ToList();
            //var charList = key.GetCharacterList();
            return chars;
        }
        
        public bool ValidateAccessMask(ApplicationUser.IDType type)
        {
            bool result = false;
            long allianceAccessMask = 8388608;
            long corpOldAccessMask = 268435455;
            long corpNewAccessMask = Properties.Settings.Default.FullAPIAccessMask;

            try
            {
                var mask = AccessMask;

                if (type == ApplicationUser.IDType.Corporation)
                {
                    result = ((mask == corpOldAccessMask) || (mask == corpNewAccessMask));
                }
                else if (type == ApplicationUser.IDType.Alliance)
                {
                    result = (mask == allianceAccessMask);
                }
            }
            catch (Exception ex) { }

            return result;
        }
    }

    public class Title
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Title")]
        public string TitleName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //public System.Data.Entity.DbSet<r3mus.Models.ApplicationUser> Users { get; set; }

        public virtual DbSet<LatestNew> LatestNewsItem { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.Forum> Forums { get; set; }
        public System.Data.Entity.DbSet<r3mus.Models.Post> Posts { get; set; }
        public System.Data.Entity.DbSet<r3mus.Models.Thread> Threads { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.ApiInfo> ApiInfoes { get; set; }
        public System.Data.Entity.DbSet<r3mus.Models.RecruitmentMailee> RecruitmentMailees { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.Applicant> Applicants { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.Application> Applications { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.Title> Titles { get;set; }
    }
}