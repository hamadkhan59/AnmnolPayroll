//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS_DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttendanceRequest
    {
        public AttendanceRequest()
        {
            this.AttendanceRequestDetails = new HashSet<AttendanceRequestDetail>();
        }
    
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Remarks { get; set; }
    
        public virtual ICollection<AttendanceRequestDetail> AttendanceRequestDetails { get; set; }
    }
}
