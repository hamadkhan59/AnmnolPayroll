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
    
    public partial class IssueChallanConfig
    {
        public int Id { get; set; }
        public Nullable<int> Fine { get; set; }
        public string Type { get; set; }
        public Nullable<int> AttendanceDays { get; set; }
        public Nullable<int> FinePerDay { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> StudentPerChallan { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
    
        public virtual StudentPerChallan StudentPerChallan1 { get; set; }
    }
}
