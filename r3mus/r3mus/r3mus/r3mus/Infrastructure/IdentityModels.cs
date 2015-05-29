using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

using eZet.EveLib.Modules;
using eZet.EveLib.Modules.Models.Account;
using eZet.EveLib.Modules.Models;
using r3mus.Infrastructure;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace r3mus.Models
{
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
        public IList<ApiInfo> ApiKeys { get; set; }
        public string MemberType { get; set; }
        public string EmailAddress { get; set; }

        public enum IDType
        {
            Corporation,
            Alliance
        }

        public ApplicationUser()
        {
            ApiKeys = new List<ApiInfo>();
        }

        public void AddApiInfo(string api, string vcode)
        {
            ApiKeys.Add(new ApiInfo() { User = this,  ApiKey = Convert.ToInt32(api), VerificationCode = vcode });
        }

        public bool IsValid()
        {
            bool result = false;

            long hostCorpID = GetCorpOrAllianceId(IDType.Corporation, Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);
            long hostAllianceID = GetCorpOrAllianceId(IDType.Alliance, Convert.ToInt32(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);

            foreach (ApiInfo apiInfo in this.ApiKeys)
            {
                if (GetCorpOrAllianceId(IDType.Corporation, apiInfo.ApiKey, apiInfo.VerificationCode) == hostCorpID)
                {
                    MemberType = "Corporation";
                    result = true;
                }
                else if (GetCorpOrAllianceId(IDType.Alliance, apiInfo.ApiKey, apiInfo.VerificationCode) == hostAllianceID)
                {
                    MemberType = "Alliance";
                    result = true;
                }
            }

            return result;
        }

        public long GetCorpOrAllianceId(IDType type, int apiKey, string vcode)
        {
            long result = -1;

            var newkey = EveOnlineApi.CreateApiKey(apiKey, vcode).Init();
            if ((newkey.IsValidKey()) && newkey.KeyType != (ApiKeyType.Corporation))
            {
                var cKey = (CharacterKey)newkey.GetActualKey();
                Character toon = cKey.Characters.Single(c => c.CharacterName == this.UserName);
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

            return result;
        }
    }

    public class ApiInfo
    {
        public int Id { get; set; }
        public int ApiKey { get; set; }
        public string VerificationCode { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}