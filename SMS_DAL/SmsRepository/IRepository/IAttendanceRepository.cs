using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IAttendanceRepository : IDisposable
    {
        List<AttendanceModel1> GetAttendanceStatus(int id, DateTime fromDate, DateTime curDate);
        List<AttendanceModel> GetAttendanceByDate(int clasSectionId, DateTime markDate);
        List<AttendanceStats> GetAttendanceStats(int branchId, DateTime date);
        List<AttendanceStats> GetAttendanceStatsByMonth(int branchId, DateTime fromDate, DateTime toDate, int? statusId = null, int? classId = null);
        DashboardDataViewModels GetAttendanceStatsByDate(int branchId, DateTime fromDate, DateTime toDate, string view = "day");
        List<AttendanceModel> GetAttendanceSheetByDate(int classSectionId, DateTime fromDate, DateTime toDate, int statusId = 0);
        int AddAttendance(Attandance attendance);
        void UpdateAttendance(Attandance attendance);

        int GetStudentAbsents(int studentId, DateTime fromDate, DateTime toDate);
        List<AttendanceRequestModel> GetAttendanceRequests(DateTime FromDate, DateTime ToDate, int StudentId);
        List<AttendanceRequestModel> GetAttendanceRequests(int AttendanceRequestId);
        void AddAttendanceRequest(AttendanceRequest request);
        void UpdateAttendanceRequest(AttendanceRequest request);
        AttendanceRequest GetAttendanceRequest(int RequestId);
        void AddAttendanceRequestDetail(AttendanceRequestDetail requestDetail);
        void UpdateAttendanceRequestDetail(AttendanceRequestDetail requestDetail);
        AttendanceRequestDetail GetAttendanceRequestDetail(int RequestDetailId);
        List<AttendanceModel> GetStudentAttendanceByDate(int studentId, DateTime FromDate, DateTime ToDate);
        Attandance GetAttandanceById(int Id);
        void DeleteAttendance(Attandance attendance);
      
    }
}
