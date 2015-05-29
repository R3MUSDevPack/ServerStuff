using eZet.EveLib.Modules;
using r3mus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using r3mus.Filters;
using System.Collections.ObjectModel;
using System.Web.Mvc;

namespace r3mus.ViewModels
{
    public class ApplicantViewModel
    {
        [Required]
        [Display(Name = "User name (Your character name from Eve)")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        [Required]
        [Display(Name = "API Key")]
        [ApiKey]
        public string ApiKey { get; set; }

        [Required]
        [Display(Name = "Verification Code")]
        [MinLength(64)]
        [MaxLength(64)]
        public string VerificationCode { get; set; }

        [Required]
        [Display(Name = "How old are you?")]
        public string Age { get; set; }

        [Required]
        [Display(Name = "How long have you been playing Eve?")]
        public string ToonAge { get; set; }

        [Required]
        [Display(Name = "Where did you hear about us?")]
        public string Source { get; set; }

        [Required]
        [Display(Name = "Give us a little information about yourself, such as previous gaming experience & interests in Eve (industry, PvP, missions etc.)")]
        [DataType(DataType.MultilineText)]
        public string Information { get; set; }

        public IEnumerable<SelectListItem> TimeZones
        {
            get
            {
                var list = TimeZoneInfo.GetSystemTimeZones().Select(tz => tz.DisplayName).ToList<string>();
                list.Insert(0, "Select a Time Zone");
                return new SelectList(list);
            }
        }
    }
}