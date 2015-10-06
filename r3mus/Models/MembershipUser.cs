namespace r3mus.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MembershipUser")]
    public partial class MembershipUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MembershipUser()
        {
            MembershipUsersInRoles = new HashSet<MembershipUsersInRole>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        [StringLength(128)]
        public string PasswordSalt { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(256)]
        public string PasswordQuestion { get; set; }

        [StringLength(128)]
        public string PasswordAnswer { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public DateTime LastLockoutDate { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public int FailedPasswordAnswerAttempt { get; set; }

        [Required]
        [StringLength(150)]
        public string Slug { get; set; }

        public string Comment { get; set; }

        [StringLength(1000)]
        public string Signature { get; set; }

        public int? Age { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(60)]
        public string Twitter { get; set; }

        [StringLength(60)]
        public string Facebook { get; set; }

        [StringLength(500)]
        public string Avatar { get; set; }

        [StringLength(300)]
        public string FacebookAccessToken { get; set; }

        public long? FacebookId { get; set; }

        [StringLength(300)]
        public string TwitterAccessToken { get; set; }

        [StringLength(150)]
        public string TwitterId { get; set; }

        [StringLength(300)]
        public string GoogleAccessToken { get; set; }

        [StringLength(150)]
        public string GoogleId { get; set; }

        public bool? IsExternalAccount { get; set; }

        public bool? TwitterShowFeed { get; set; }

        public bool? DisableEmailNotifications { get; set; }

        public bool? DisablePosting { get; set; }

        public bool? DisablePrivateMessages { get; set; }

        public bool? DisableFileUploads { get; set; }

        public DateTime? LoginIdExpires { get; set; }

        [StringLength(250)]
        public string MiscAccessToken { get; set; }

        [StringLength(40)]
        public string Latitude { get; set; }

        [StringLength(40)]
        public string Longitude { get; set; }

        public DateTime? LastActivityDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MembershipUsersInRole> MembershipUsersInRoles { get; set; }
    }
}
