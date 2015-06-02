using r3mus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace r3mus.ViewModels
{
    public class ForumViewModel
    {
        public Forum Forum { get; set; }
        
        public SelectList RoleList { get; set; }

        [DisplayName("Creator")]
        public string CreatorName { get; set; }
    }
}