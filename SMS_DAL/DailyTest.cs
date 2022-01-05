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
    
    public partial class DailyTest
    {
        public DailyTest()
        {
            this.DailyTestsDetails = new HashSet<DailyTestsDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ClassSectionId { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> TotalMarks { get; set; }
        public Nullable<int> PassPercentage { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual ClassSection ClassSection { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<DailyTestsDetail> DailyTestsDetails { get; set; }
    }
}
