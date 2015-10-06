namespace r3mus.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class r3musForumDBContext : DbContext
    {
        public r3musForumDBContext()
            : base("name=r3musForumDBContext")
        {
        }

        public virtual DbSet<MembershipRole> MembershipRoles { get; set; }
        public virtual DbSet<MembershipUser> MembershipUsers { get; set; }
        public virtual DbSet<MembershipUsersInRole> MembershipUsersInRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MembershipRole>()
                .HasMany(e => e.MembershipUsersInRoles)
                .WithRequired(e => e.MembershipRole)
                .HasForeignKey(e => e.RoleIdentifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MembershipUser>()
                .HasMany(e => e.MembershipUsersInRoles)
                .WithRequired(e => e.MembershipUser)
                .HasForeignKey(e => e.UserIdentifier)
                .WillCascadeOnDelete(false);
        }
    }
}
