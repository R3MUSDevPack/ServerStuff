//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SiteUpdateBot
{
    using System;
    using System.Collections.Generic;
    
    public partial class Topic
    {
        public Topic()
        {
            this.Posts = new HashSet<Post>();
            this.Topic_Tag = new HashSet<Topic_Tag>();
            this.TopicNotifications = new HashSet<TopicNotification>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.Guid MembershipUser_Id { get; set; }
        public bool Solved { get; set; }
        public System.Guid Category_Id { get; set; }
        public Nullable<System.Guid> Post_Id { get; set; }
        public string Slug { get; set; }
        public Nullable<int> Views { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public Nullable<System.Guid> Poll_Id { get; set; }
        public Nullable<bool> Pending { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual MembershipUser MembershipUser { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Topic_Tag> Topic_Tag { get; set; }
        public virtual ICollection<TopicNotification> TopicNotifications { get; set; }
    }
}