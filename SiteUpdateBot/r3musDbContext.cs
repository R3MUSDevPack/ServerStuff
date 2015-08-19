using Microsoft.AspNet.Identity.EntityFramework;
using r3mus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteUpdateBot
{
    public class r3musDbContext : IdentityDbContext<ApplicationUser>
    {
        public System.Data.Entity.DbSet<r3mus.Models.RecruitmentMailee> RecruitmentMailees { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.ApplicantList> Applicants { get; set; }

        public System.Data.Entity.DbSet<r3mus.Models.Title> Titles { get; set; }

        public r3musDbContext()
            : base("DefaultConnection")
        {
        }
    }
}
