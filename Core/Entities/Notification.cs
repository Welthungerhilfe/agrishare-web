//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Agrishare.Core.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Nullable<int> BookingId { get; set; }
        public NotificationType TypeId { get; set; }
        public NotificationStatus StatusId { get; set; }
        public NotificationGroup GroupId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool Deleted { get; set; }
    
        public virtual Booking Booking { get; set; }
        public virtual User User { get; set; }
    }
}
