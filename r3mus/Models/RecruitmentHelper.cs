using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EveAI.Live;

namespace r3mus.Models
{
    public partial class Application
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public enum ApplicationStatus
        //{
        //    Applied,
        //    InScreening,
        //    OnHold,
        //    Rejected,
        //    DirectorReview,
        //    AwaitingInterview,
        //    Accepted
        //}

        public int Id { get; set; }

        public int ApplicantId { get; set; }

        public string Status { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        
        public virtual ApplicationUser Reviewer { get; set; }

        public DateTime DateTimeCreated { get; set; }

    }

    [MetadataType(typeof(ApplicationMetaData))]
    public partial class Application
    { 
        public string Screener
        {
            get
            {
                if (this.Reviewer == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.Reviewer.UserName;
                }
            }
        }
    }

    public partial class ApplicationMetaData
    {
        [Display(Name = "Status Update")]
        public DateTime DateTimeCreated { get; set; }

    }

    [MetadataType(typeof(ApplicantListMetaData))]
    public partial class ApplicantList
    { }

    public partial class ApplicantListMetaData
    {
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Api Key")]
        public int ApiKey { get; set; }

        [Display(Name = "Verification Code")]
        public int VerificationCode { get; set; }

        [Display(Name = "Last Update")]
        [UIHint("_DateTime")]
        public DateTime LastStatusUpdate { get; set; }

        [Display(Name = "Status Update")]
        [UIHint("_DateTime")]
        public DateTime DateTimeCreated { get; set; }

        [Display(Name = "Screener")]
        public DateTime UserName { get; set; }

        [DataType(DataType.MultilineText)]
        public DateTime Notes { get; set; }
    }
}