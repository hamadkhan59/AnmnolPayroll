using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class DashboardDataViewModels
    {
        public List<AttendanceStats> StudentAttendanceStats { get; set; }
        public List<AttendanceStats> StaffAttendanceStats { get; set; }
        public List<FeeByDate> FeeByDate { get; set; }

        public List<string> Months { get; set; }
        public List<DateTime> Dates { get; set; }
    }

    public class AttendanceStats
    {
        public int StatusId { get; set; }
        public string StatusCode { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int Count { get; set; }
        public List<int> Data { get; set; }
    }

    public class FeeSubmissionDetails
    {
        public double PaidFee { get; set; }
        public double UnpaidFee { get; set; }
        public double PendingFee { get; set; }
        public double Receivable { get; set; }
        public double PaidToday { get; set; }
        public List<FeeByDate> FeeByDate { get; set; }
    }

    public class FeeByDate
    {
        public string Month { get; set; }
        public DateTime Date { get; set; }
        public double DepositedFeeAmount { get; set; }
        public double PendingFeeAmount { get; set; }
        public double TotalFeeAmount { get; set; }
    }

    public class FeeByClassSection
    {
        public int? Key { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int Status { get; set; }
        public decimal? Fee { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
