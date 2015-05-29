using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace r3mus.Models
{
    public class Applicant
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        [Display(Name = "Api Key")]
        public int ApiKey { get; set; }

        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; }

        public string Information { get; set; }
        public string Age { get; set; }

        [Display(Name = "Played for")]
        public string ToonAge { get; set; }

        public string Source { get; set; }
    }
}