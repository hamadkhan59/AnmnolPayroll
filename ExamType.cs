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
    
    public partial class ExamType
    {
        public ExamType()
        {
            this.Exams = new HashSet<Exam>();
        }
    
        public int Id { get; set; }
        public Nullable<int> TermId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IsDeleted { get; set; }
        public Nullable<int> Percent_Of_Total { get; set; }
    
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ExamTerm ExamTerm { get; set; }
        public virtual DateSheet DateSheet { get; set; }
    }
}