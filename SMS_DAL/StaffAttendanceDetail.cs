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
    
    public partial class StaffAttendanceDetail
    {
        public int Id { get; set; }
        public Nullable<int> AttendanceId { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string TimeInString { get; set; }
        public string TimeOutString { get; set; }
    
        public virtual StaffAttandance StaffAttandance { get; set; }
    }
}
