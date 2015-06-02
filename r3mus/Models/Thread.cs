using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace r3mus.Models
{
    [Table("Threads")]
    public partial class Thread
    {
        public virtual ICollection<Post> Posts { get; set; }
        public virtual Forum Forum { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int ForumID { get; set; }

        public string CreatorId { get; set; }
        public DateTime Created { get; set; }

        [Display(Name = "Thread Id")]
        public int Id { get; set; }

        [Display(Name = "Thread Title")]
        public string Title { get; set; }

        public Thread()
        {
            Posts = new List<Post>();
        }
    }
}