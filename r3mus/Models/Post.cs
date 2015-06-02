using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace r3mus.Models
{
    [Table("Posts")]
    public partial class Post
    {
        [Display(Name = "Post Id")]
        public int Id { get; set; }

        [Display(Name = "Post Title")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Post Body")]
        public string Body { get; set; }

        [Column(TypeName = "smalldatetime")]
        [UIHint("_DateTime")]
        public DateTime PostedAt { get; set; }

        public int ThreadId { get; set; }
        public string UserId { get; set; }
        
        public virtual Thread Thread { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}