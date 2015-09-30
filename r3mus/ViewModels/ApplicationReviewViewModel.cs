using r3mus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace r3mus.ViewModels
{
    public class ApplicationReviewViewModel
    {
        public enum ApplicationStatus
        {
            Applied,
            [Display(Name = "In Screening")]
            InScreening,
            [Display(Name = "Needs API Mail")]
            NeedsAPIMail,
            [Display(Name = "API Mail Sent")]
            APIMailSent,
            [Display(Name = "On Hold")]
            OnHold,
            [Display(Name = "Director Review")]
            DirectorReview,
            [Display(Name = "Awaiting Interview")]
            AwaitingInterview,
            [Display(Name = "Awaiting Invitation")]
            AwaitingInvitation,
            Rejected,
            Accepted
        }

        public IEnumerable<Application> ApplicationInfo { get; set; }
        public Applicant Applicant { get; set; }
        public Application NewReviewItem { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ApplicationStatus NewReviewItemStatus { get; set; }
    }
}