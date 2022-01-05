using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class DailyTestViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string RollNumber { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Grade { get; set; }
        public string AdmissionNo { get; set; }
        public string Contact_1 { get; set; }
        public decimal ObtMarks { get; set; }
        public decimal TotalMarks { get; set; }
        public DateTime TestDate { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string Subject { get; set; }
        public int DailyTestId { get; set; }

    }

    public class DateSheetViewModel
    {
        public int Id { get; set; }
        public string ExamName { get; set; }
        public string SubjectName { get; set; }
        public string ExamPlace { get; set; }
        public string ExamTime { get; set; }
        public DateTime ExamDate { get; set; }
    }

    public partial class DateSheetModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<System.DateTime> ExamDate { get; set; }
        public string ExamTime { get; set; }
        public string Center { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string ExamName { get; set; }
        public string SubjectName { get; set; }
    }

    public class DateSheetResponse
    {
        public string StatusCode { get; set; }
        public List<DateSheetViewModel> DateSheet { get; set; }
    }

    public class TimeTableViewModel
    {
        public int Id { get; set; }
        public int Slot { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public string Time { get; set; }
    }

    public class TimeTableResponse
    {
        public string StatusCode { get; set; }
        public List<TimeTableViewModel> TimeTable { get; set; }
    }
    public class DailyTestResponse
    {
        public string StatusCode { get; set; }
        public List<DailyTestViewModel> DailyTest { get; set; }
    }

}
