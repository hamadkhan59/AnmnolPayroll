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
    
    public partial class StaffSalary
    {
        public int SalaryId { get; set; }
        public Nullable<int> StaffId { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<int> FinanceAccountId { get; set; }
        public string ForMonth { get; set; }
        public Nullable<int> PaidAmount { get; set; }
        public Nullable<int> SalaryAmount { get; set; }
        public Nullable<int> AdvanceAdjustment { get; set; }
        public Nullable<int> Deduction { get; set; }
        public Nullable<int> Bonus { get; set; }
        public Nullable<int> LateIns { get; set; }
        public Nullable<int> EarlyOuts { get; set; }
        public Nullable<int> HalfDays { get; set; }
        public Nullable<int> Absents { get; set; }
        public Nullable<int> Presents { get; set; }
        public Nullable<int> AttendanceDeduction { get; set; }
        public Nullable<int> MiscWithdraw { get; set; }
        public Nullable<int> ClubbedSundays { get; set; }
        public Nullable<int> SundaysDeduction { get; set; }
        public Nullable<decimal> TotalHours { get; set; }
        public Nullable<decimal> WorkingHours { get; set; }
        public Nullable<decimal> ShortHours { get; set; }
        public Nullable<int> BonusDays { get; set; }
    
        public virtual FinanceFifthLvlAccount FinanceFifthLvlAccount { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
