using r3mus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace r3mus.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Website Roles")]
        public string WebsiteRoles { get; set; }

        [Display(Name = "Website Member Type")]
        public string MemberType { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Joined current corp")]
        public DateTime MemberSince { get; set; }
        
        public string Avatar { get; set; }

        public string Titles { get; set; }

        public List<ApiInfo> ApiKeys { get; set; }

        [Display(Name = "Available Roles")]
        public List<string> AvailableRoles { get; set; }

        [Display(Name = "Assigned Roles")]
        public List<string> UserRoles { get; set; }
    }
}