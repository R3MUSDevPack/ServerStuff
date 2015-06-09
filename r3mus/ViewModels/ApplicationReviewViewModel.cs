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
            [Display(Name = "On Hold")]
            OnHold,
            Rejected,
            [Display(Name = "Director Review")]
            DirectorReview,
            [Display(Name = "Awaiting Interview")]
            AwaitingInterview,
            [Display(Name = "Needs Invitation Sending")]
            AwaitingInvitation,
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