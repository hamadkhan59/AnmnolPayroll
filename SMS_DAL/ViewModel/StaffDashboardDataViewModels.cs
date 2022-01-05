using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class StaffDashboardDataViewModels
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class StaffSalariesViewModel
    {
        public DateTime? PaidDate { get; set; }
        public string Month { get; set; }
        public decimal? Salary { get; set; }
        public decimal? PayableSalary { get; set; }
        public decimal? SalaryDeductions { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? AdvanceAdjustment { get; set; }
    }

    public class StaffSalaryModel
    {
        public DateTime? PaidDate { get; set; }
        public string Month { get; set; }
        public decimal? Salary { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? SalaryDeductions { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? AdvanceAdjustment { get; set; }
        public string Name { get; set; }
        public int StaffId { get; set; }
        public int Id { get; set; }

    }

    public class StaffSubjectsStatsViewModel
    {
        public List<string> Staff { get; set; }
        public List<StaffClassSubjectCount> StaffClassSubjectCount { get; set; }
        public StaffSubjectsStatsViewModel()
        {
            Staff = new List<string>();
            StaffClassSubjectCount = new List<StaffClassSubjectCount>();
        }
    }

    public class StaffClassSubjectCount
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<int> SubjectCount { get; set; }
        public StaffClassSubjectCount()
        {
            SubjectCount = new List<int>();
        }
    }

    public class StaffAttendanceResponse
    {
        public string StatusCode { get; set; }
        public List<StaffAttandanceModel> StaffAttendanceDetail { get; set; }
    }

    public class StaffAdvanceResponse
    {
        public string StatusCode { get; set; }
        public List<StaffAdvanceModel> StaffAdvanceDetail { get; set; }
    }

    public class StaffSalaryResponse
    {
        public string StatusCode { get; set; }
        public List<StaffSalaryModel> StaffSalaryDetail { get; set; }
    }

    public class StaffStudentAttendanceResponse
    {
        public string StatusCode { get; set; }
        public List<AttendanceModel> StaffAttendanceDetail { get; set; }
    }
}
