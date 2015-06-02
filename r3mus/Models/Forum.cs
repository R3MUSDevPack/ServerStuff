using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace r3mus.Models
{
    [Table("Forums")]
    public partial class Forum
    {
        [Display(Name = "Forum Id")]
        public int Id { get; set; }
        [Display(Name = "Forum Title")]
        public string Title { get; set; }
        public virtual ICollection<Thread> Threads { get; set; }

        [Display(Name = "Minimum Role")]
        public string MinimumRole { get; set; }

        public string CreatorId { get; set; }
        [UIHint("_DateTime")]
        public DateTime Created { get; set; }

        public bool Deleted { get; set; }
                
        public Forum() {
            Threads = new List<Thread>();
        }
    }
}