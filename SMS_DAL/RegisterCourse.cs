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
    
    public partial class RegisterCourse
    {
        public RegisterCourse()
        {
            this.SubjectChapters = new HashSet<SubjectChapter>();
            this.TimeTables = new HashSet<TimeTable>();
        }
    
        public int RegisterCourseId { get; set; }
        public int ClassSectionId { get; set; }
        public int SubjectId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ResultOrder { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ClassSection ClassSection { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<SubjectChapter> SubjectChapters { get; set; }
        public virtual ICollection<TimeTable> TimeTables { get; set; }
    }
}