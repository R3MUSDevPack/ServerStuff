namespace r3mus.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MembershipUsersInRole
    {
        public Guid Id { get; set; }

        public Guid RoleIdentifier { get; set; }

        public Guid UserIdentifier { get; set; }

        public virtual MembershipRole MembershipRole { get; set; }

        public virtual MembershipUser MembershipUser { get; set; }
    }
}
