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
    
    public partial class StaffAttandancePolicy
    {
        public int Id { get; set; }
        public string LateInTime { get; set; }
        public string HalfDayTime { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int> LeaveInMonth { get; set; }
        public Nullable<int> LateInCount { get; set; }
        public Nullable<int> SalaryDeduction { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> LeaveInYear { get; set; }
        public string EarlyOutTime { get; set; }
        public bool IsSundayClubed { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Designation Designation { get; set; }
    }
}
