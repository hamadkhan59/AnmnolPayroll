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
    
    public partial class Year
    {
        public Year()
        {
            this.Activities = new HashSet<Activity>();
            this.ExamTerms = new HashSet<ExamTerm>();
        }
    
        public int Id { get; set; }
        public string Year1 { get; set; }
    
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<ExamTerm> ExamTerms { get; set; }
        public virtual Year Years1 { get; set; }
        public virtual Year Year2 { get; set; }
    }
}
