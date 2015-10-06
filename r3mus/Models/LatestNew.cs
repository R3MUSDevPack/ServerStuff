namespace r3mus.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LatestNew
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(450)]
        public string Category { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(450)]
        public string Topic { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 3)]
        public string Post { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(150)]
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }
}
