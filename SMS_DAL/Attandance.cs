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
    
    public partial class Attandance
    {
        public int id { get; set; }
        public Nullable<System.DateTime> AttandanceDate { get; set; }
        public Nullable<int> StudentID { get; set; }
        public int StatusId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual StudentAttendanceStatu StudentAttendanceStatu { get; set; }
        public virtual Student Student { get; set; }
    }
}
