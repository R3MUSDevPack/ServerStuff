namespace r3mus.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LatestNews : DbContext
    {
        public LatestNews()
            : base("name=LatestNewsConnection")
        {
        }

        public virtual DbSet<LatestNew> LatestNewsItem { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
