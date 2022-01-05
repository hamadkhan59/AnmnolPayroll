using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ClassSectionModel
    {
        public int ClassSectionId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public bool IsFinanceAccountOpen { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public class ClassModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
    }

    public class SectionModel
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
    }
}
