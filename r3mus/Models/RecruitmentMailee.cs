using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [UIHint("_DateTime")]
        public DateTime Submitted { get; set; }
        [UIHint("_DateTime")]
        public DateTime? Mailed { get; set; }
        public string SubmitterId { get; set; }
        public string MailerId { get; set; }
        [UIHint("_DateTime")]
        public DateTime? DateOfBirth { get; set; }

        public long CorpId_AtLastCheck { get; set; }

        public DateTime LastUpdated { get; set; }

        [NotMapped]
        public bool InNPCCorp { get { return IsInNPCCorp(); } }
        [NotMapped]
        public bool DateOfBirthInRange { get { return IsDateOfBirthInRange(); } }

        public RecruitmentMailee()
        {
            try
            {
                SubmitterId = HttpContext.Current.User.Identity.GetUserId();
            }
            catch (Exception ex) { }
        }

        public void GetToonDetails()
        {
            try
            {
                var toon = JKON.EveWho.Api.GetCharacter(Name, Convert.ToInt64(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode).result;
                CorpId_AtLastCheck = toon.corporationID;
                DateOfBirth = Convert.ToDateTime(toon.employmentHistory.employmentRecords.LastOrDefault().StartDate);
            }
            catch (Exception ex)
            {
                try
                {
                    var toon = JKON.EveWho.EveWho.GetCharacter(Name, Convert.ToInt64(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);
                    CorpId_AtLastCheck = toon.info.corporation_id;
                    DateOfBirth = toon.history.LastOrDefault().start_date;
                }
                catch (Exception ex1)
                {
                    CorpId_AtLastCheck = -1;
                    DateOfBirth = new DateTime(1900, 1, 1);
                }
                if (CorpId_AtLastCheck == 0)
                {
                    CorpId_AtLastCheck = -1;
                }
            }
        }

        private bool IsInNPCCorp()
        {
            return ((CorpId_AtLastCheck >= 1000000) && (CorpId_AtLastCheck <= 1000200));
        }

        private bool IsDateOfBirthInRange()
        {
            if (DateOfBirth.HasValue)
            {
                return ((DateTime.Now - DateOfBirth.Value).Days < Properties.Settings.Default.MaxDayAgeForMailees);
            }
            else
            {
                return false;
            }
        }
    }

    public class RecruitmentMailDump
    {
        [Key]
        public int Id { get; set; }
        public string Names { get; set; }
    }
}