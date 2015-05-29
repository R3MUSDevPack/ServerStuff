using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace r3mus.Models
{
    public class RecruitmentMailee
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        protected UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Submitted { get; set; }
        public DateTime? Mailed { get; set; }
        public string SubmitterId { get; set; }
        public string MailerId { get; set; }

        public long CorpId_AtLastCheck { get; set; }

        public RecruitmentMailee()
        {
            try
            {
                SubmitterId = HttpContext.Current.User.Identity.GetUserId();
            }
            catch (Exception ex) { }
        }

        public bool IsInNPCCorp()
        {
            //long corpId = JKON.EveWho.EveWho.GetCharacter(Name, Convert.ToInt64(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode).info.corporation_id;

            long corpId = JKON.EveWho.Api.GetCharacter(Name, Convert.ToInt64(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode).result.corporationID;

            CorpId_AtLastCheck = corpId;
            return ((corpId >= 1000000) && (corpId <= 1000200));
        }
    }

    public class RecruitmentMailDump
    {
        [Key]
        public int Id { get; set; }
        public string Names { get; set; }
    }
}