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
    
    public partial class AttendanceRequestDetail
    {
        public int Id { get; set; }
        public int AttendanceRequesId { get; set; }
        public Nullable<int> AttendanceId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string Comments { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> AttendanceStatusId { get; set; }
    
        public virtual AttendanceRequest AttendanceRequest { get; set; }
    }
}
