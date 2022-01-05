using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class SubjectModel
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

    }

    public partial class SessionSubjectModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Nullable<int> TeacherId { get; set; }
        public Nullable<int> ClassSectionId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<System.DateTime> From_Date { get; set; }
        public Nullable<System.DateTime> To_Date { get; set; }
        public string SessionYear { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
        public string StaffName { get; set; }
    }
}
