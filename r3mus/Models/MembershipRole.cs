namespace r3mus.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MembershipRole")]
    public partial class MembershipRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MembershipRole()
        {
            MembershipUsersInRoles = new HashSet<MembershipUsersInRole>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(256)]
        public string RoleName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MembershipUsersInRole> MembershipUsersInRoles { get; set; }
    }
}
