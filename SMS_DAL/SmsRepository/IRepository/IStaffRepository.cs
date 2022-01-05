using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IStaffRepository : IDisposable
    {

        int AddSession(Session session);
        Session GetSessionById(int sessionId);
        Session GetCurrentSession();
        Session GetSessionByName(string sessionName);
        List<Staff> GetAllStaff();
        Session GetSessionByNameAndId(string sessionName, int sessionId);
        List<Session> GetAllSessions();
        void UpdateSession(Session session);
        void DeleteSession(Session session);
        Session GetSessionInDates(Session session, int branchId);
        Session GetEditSessionInDates(Session session);

        int AddDesignationCategory(DesignationCatagory category);
        DesignationCatagory GetDesignationCategoryById(int categoryId);
        DesignationCatagory GetDesignationCategoryByName(string categoryName, int branchId);
        DesignationCatagory GetDesignationCategoryByNameAndId(string categoryName, int categoryId, int branchId);
        List<DesignationCatagory> GetAllDesignationCategories();
        void UpdateDesignationCategory(DesignationCatagory category);
        void DeleteDesignationCategory(DesignationCatagory category);

        int AddDesignation(Designation designation);
        Designation GetDesignationById(int designationId);
        Designation GetDesignationByName(string designationName, int branchId);
        Designation GetDesignationByNameAndId(string designationName, int designationId, int branchId);
        List<Designation> GetAllDesignations();
        void UpdateDesignation(Designation designation);
        void DeleteDesignation(Designation designation);
        List<Designation> GetAllDesignationsByCategoryId(int categoryId);

        int AddAllownce(Allownce allownce);
        Allownce GetAllownceById(int allownceId);
        Allownce GetAllowncenByName(string allownceName, int branchId);
        Allownce GetAllownceByNameAndId(string allownceName, int allownceId, int branchId);
        List<Allownce> GetAllAllownces();
        void UpdateAllownce(Allownce allownce);
        void DeleteAllownce(Allownce allownce);

        int AddStaffAttandancePolicy(StaffAttandancePolicy policy);
        StaffAttandancePolicy GetStaffAttandancePolicyById(int policyId);
        StaffAttandancePolicy GetStaffAttandancePolicyByDesignationId(int designationId, int branchId);
        StaffAttandancePolicy GetStaffAttandancePolicyByDesignationIdAndId(int designationId, int policyId, int branchId);
        List<StaffAttandancePolicy> GetAllStaffAttandancePolicies();
        List<StaffAttandancePolicyModel> GetAllStaffAttandancePoliciesModel();
        void UpdateStaffAttandancePolicy(StaffAttandancePolicy policy);
        void DeleteStaffAttandancePolicy(StaffAttandancePolicy policy);

        Staff GetStaffById(int staffId);
        StaffModel GetStaffModelById(int staffId);
        void AddStaffAdvance(StaffAdvance advance);
        void UpdateStaffAdvance(StaffAdvance advance);
        StaffAdvance GetStaffAdvancesById(int id);
        List<StaffAdvance> GetSTaffAdvancesByStaffId(int staffId);
        void AddStaffMiscWithdraw(StaffMiscWithdraw withdraw);
        void UpdateStaffMiscWithdraw(StaffMiscWithdraw withdraw);
        StaffMiscWithdraw GetStaffMiscWithdrawsById(int id);
        List<StaffMiscWithdraw> GetStaffMiscWithdrawByStaffId(int staffId);
        int GetStaffMiscWithdraws(string forMonth, int staffId);
        int AddStaff(Staff staff);
        Staff GetStaffByNameAndFatherName(string name, string fatherName);
        Staff GetStaffByNameAndFatherNameAndId(string name, string fatherName, int staffId);
        List<Staff> SearchStaff(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId);
        List<StaffModel> SearchStaffModel(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId);
        List<Staff> SearchAllStaff(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId);
        List<StaffModel> SearchAllStaffModel(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId);
        void UpdateStaff(Staff staff);
        void DeleteStaff(Staff staff);
        List<StaffAdvanceModel> SearchAdvance(int categoryId, int designationId, int staffId, DateTime fromDate, DateTime toDate, int branchId);
        List<StaffMiscWithdrawModel> SearchMiscWithdraws(int categoryId, int designationId, int staffId, DateTime fromDate, DateTime toDate, int branchId);

        StaffAllownce GetStaffAllownceById(int Id);
        List<StaffAllownce> GetStaffAllownceByStaffId(int staffId);
        StaffAllownce GetStaffAllownceByStaffId(int staffId, int allownceId);
        int AddStaffAllownce(StaffAllownce staffAllownce);
        void UpdateStaffAllownce(StaffAllownce staffAllownce);
        void DeleteStaffAllownce(StaffAllownce staffAllownce);
        StaffSalaryIncrementHistory GetStaffSalaryIncrementHistory(int incrementId);
        List<StaffSalaryIncrementHistory> GetStaffSalaryIncrementHistoryByAllownceId(int allownceId);
        void DeleteStaffSalaryIncrementHistory(StaffSalaryIncrementHistory staffSalaryIncrementHistory);

        List<StaffDegree> GetStaffDegreeByStaffId(int staffId);
        int AddStaffDegree(StaffDegree staffDegree);
        void DeleteStaffDegree(StaffDegree staffDegree);

        List<AllowLeave> GetAllAllowLeaves();
        List<LateInCount> GetAllLateInCOunts();
        List<PaymentMethod> GetAllPaymentMethods();
        List<MeritalStatu> GetAllMeritalStatuses();

        StaffSubjectsStatsViewModel GetStaffSubjectStats(int branchId);
        List<SessionSubject> GetAllSessionSubjects();
        List<SessionSubjectModel> GetAllSessionSubjectsModel();
        SessionSubject GetSessionSubjectById(int Id);
        void DeleteSessionSubject(SessionSubject subject);
        int AddSessionSubject(SessionSubject sessionSubject);
        SessionSubject GetSessionSubjectInDates(SessionSubject subject);
        SessionSubject GetTeacherSessionSubjectInDates(SessionSubject subject);
        
        List<Staff> GetStaffByDesignation(string designatioName);

        List<StaffSalariesViewModel> GetSalaryStatsByMonth(int branchId, DateTime from, DateTime to);
        List<StaffSalary> SearchSalaries(int searchCatagoryId, int searchDesginationId, int searchStaffId, string forMonth, int branchId, bool deleteFlag = false);
        List<StaffSalary> GetLastSixMonthSalaries(int staffId, int branchId);
        List<QueryResult> SearchAttendanceDetail(string spQuery);
        List<StaffSalaryHistoryViewModel> SearchStaffSalaryDetail(string spQuery);
        int AddStaffSalaryIncrementHostory(StaffSalaryIncrementHistory staffSalaryIncrementHistory);
        int AddStaffSalary(StaffSalary staffSalary);
        StaffSalary SearchStaffSalaries(int staffId, string forMonth);
        void DeleteStaffSalary(int salaryId);
        void UpdateStaffSalary(StaffSalary salary);

        StaffAttandance GetStaffAttendanceById(int staffAttendanceId);
        void DeleteStaffAttendance(StaffAttandance attendance);
        List<AttendanceStats> GetAttendanceStats(int branchId, DateTime date);
        List<AttendanceStats> GetAttendanceStatsByMonth(int branchId, DateTime from, DateTime to);
        DashboardDataViewModels GetAttendanceStatsByDate(int branchId, DateTime from, DateTime to, string view = "day");
        List<StaffAttandance> SearchAttendnace(DateTime markDate, int searchCatagoryId, int searchDesignationId, int branchId, int statusId = 0);
        List<StaffAttandanceModel> SearchStaffAttendnaceModel(DateTime markDate, int searchCatagoryId, int searchDesignationId, int branchId, int statusId = 0);
        List<StaffAttandance> SearchAttendnace(DateTime fromDate, DateTime toDate, int searchCatagoryId, int searchDesignationId, int staffId, int branchId, int statusId = 0);
        List<StaffAttandanceModel> SearchStaffAttendnaceModel(DateTime fromDate, DateTime toDate, int searchCatagoryId, int searchDesignationId, int staffId, int branchId, int statusId = 0);
        int AddStaffAttendnace(StaffAttandance staffAttandance);
        int AddStaffAttendnaceLogs(StaffAttendanceLog log);
        int GetAttendanceLogId(int staffId, string dateTimeStr);
        List<StaffAttandance> SearchAttendnaceForTheDay(DateTime fromDate);
        StaffAttandance SearchStaffDailyAttendance(DateTime attedanceDate, int staffId);
        StaffAttandance GetStaffDailyAttendance(DateTime attedanceDate, int staffId);
        void UpdateStaffAttendance(StaffAttandance staffAttendance);
        StaffAttendanceDetail GetTopStaffAttendanceDetailByAttId(int attendanceId);
        StaffAttendanceDetail GetFirstStaffAttendanceDetailByAttId(int attendanceId);
        List<StaffAttendanceDetail> GetStaffAttendanceDetailByAttId(int attendanceId);
        StaffAttendanceDetail GetStaffAttendanceDetailById(int attendanceId);
        int AddStaffAttendnaceDetail(StaffAttendanceDetail staffAttandanceDetail);
        void UpdateStaffAttendanceDetail(StaffAttendanceDetail staffAttandanceDetail);
        void DeleteStaffAttendanceDetail(StaffAttendanceDetail staffAttandanceDetail);
        List<StaffAttendanceDetailModel> GetStaffAttendnaceDetailByStaffId(DateTime attendnaceDate, int staffId);
        List<StaffType> GetAllStaffTypes();

        string AddStaffBioMatric(string staffId, string bioHash);
        void AddStaffPaymentApprovalHistory(StaffPaymentApproval approval);
        List<string> GetAPIAllStaff();
        string GetAPIStaffById(string staffId);
        int GetDesignationCount(int CategoryId);
        int GetStaffCount(int DesignationId);
        int GetStudentCount(int SessionId);

        void AddStaffAttendanceRequest(StaffAttendanceRequest request);
        void UpdateStaffAttendanceRequest(StaffAttendanceRequest request);
        StaffAttendanceRequest GetStaffAttendanceRequest(int RequestId);
        void AddStaffAttendanceRequestDetail(StaffAttendanceRequestDetail requestDetail);
        void UpdateStaffAttendanceRequestDetail(StaffAttendanceRequestDetail requestDetail);
        StaffAttendanceRequestDetail GetStaffAttendanceRequestDetail(int RequestDetailId);
        List<StaffAttendanceRequestModel> GetStaffAttendanceRequests(DateTime FromDate, DateTime ToDate, int StaffId);
        List<StaffAttendanceRequestModel> GetStaffAttendanceRequests(int StaffAttendanceRequestId);
        List<StaffAttandance> SearchStaffAttendnace(DateTime FromDate, DateTime ToDate, int staffId);
        List<StaffAttandanceModel> SearchStaffAttendnaceModel(DateTime FromDate, DateTime ToDate, int staffId);
        List<StaffAdvanceModel> SearchStaffAdvance(int staffId, DateTime fromDate, DateTime toDate);
        List<StaffMiscWithdrawModel> SearchStaffMiscWithdraw(int staffId, DateTime fromDate, DateTime toDate);
        List<StaffSalaryModel> SearchStaffSalary(int staffId, DateTime fromDate, DateTime toDate);
        void UpdateBioMatrixLogCount(int count);
        int GetBioMatrixLogCount();
        void AddStaffHolidays(StaffHoliday holiday);
        StaffHoliday GetStaffHolidayById(int holidayId);
        StaffHoliday GetStaffHolidayByDate(DateTime date, int branchId);
        void DeleteStaffHoliday(StaffHoliday holiday);
        List<StaffHoliday> GetAllStaffHoliday();
    }
}

