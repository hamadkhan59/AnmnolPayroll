using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IExamRepository : IDisposable
    {
		List<Year> getAllYears();
        List<YearModel> getAllYearsModel();
        List<StudentSubjectModel> getStudentSubject(int studentId);
        int AddExamTerm(ExamTerm examTerm);
        List<ExamTerm> GetExamTermByYear(int year, int branchId);
        List<ExamTermModel> GetExamTermModelByYear(int year, int branchId);
        int GetExamTermPercentageByYear(int year, int branchId);
        List<ExamTerm> GetAllExamTerm();
        List<ExamTerm> GetExamTermByYearAndId(int year, int id, int branchId);
        int GetExamTermPercentageByYearAndId(int year, int id, int branchId);
        void UpdateExamTerm(ExamTerm examTerm);
        ExamTerm GetExamTermById(int id);
        void DeleteExamTerm(ExamTerm examTerm);

        int AddGrade(GradesConfig config);
        void UpdateGrade(GradesConfig config);
        GradesConfig GetGradeById(int id);
        void DeleteGrade(GradesConfig config);
        List<GradesConfig> GetAllGrade();
        GradesConfig GetGradeByName(string grade);
        GradesConfig GetGradeByNameAndId(string grade, int id);

        int AddRemarks(RemarksConfig config);
        void UpdateRemarks(RemarksConfig config);
        RemarksConfig GetRemarksById(int id);
        RemarksConfig GetRemarksByName(string grade);
        RemarksConfig GetRemarksByNameAndId(string grade, int id);
        void DeleteRemarks(RemarksConfig config);
        List<RemarksConfig> GetAllRemarks();
        int AddExamType(ExamType examType);
		List<ExamType> GetExamTypeByExamTermsName(string termName);
        List<ExamTypeModel> GetExamTypeModel(string termName, int branchId);
        List<ExamType> GetExamTypeByExamTerms(int termId, int branchId);
        int GetExamTypePercentageByTerm(int termId, int branchId);
        List<ExamType> GetAllExamTypes();
        List<ExamType> GetExamTypeByTermAndId(int termId, int id, int branchId);
        int GetExamTypePercentageByTermAndId(int termId, int id, int branchId);
        void UpdateExamType(ExamType examType);
        ExamType GetExamTypeById(int id);
        DataSet BuildStudentGrandList(string spQuery);
        void DeleteExamType(ExamType examType);

        List<DateSheet> GetDateSheetByClassAndExam(int classId, int sectionId, int examId);
        List<DateSheetModel> GetDateSheetByClassAndExamModel(int classId, int sectionId, int examId);
        List<DateSheet> GetClassDateSheetByExam(int classId, int examId);
        List<DateSheetModel> GetClassDateSheetByExamModel(int classId, int examId);
        List<DateSheet> GetClassDateSheetBySubjectAndExam(int classId, int examId, int subjectId);
        DateSheet GetSingleDateSheetBySubjectAndExam(int classId, int examId, int subjectId);
        int AddDateSheet(DateSheet dateSheet);
        DateSheet GetDateSheetByClassAndExam(int classId, int sectionId, int examId, int subjectId);
        void UpdateDateSheet(DateSheet dateSheet);
        void DeleteDateSheet(DateSheet dateSheet);

        Exam GetExamByExamType(int examId, int classSectionId, int subjectId);
        List<Exam> GetExamByExamType(int examId, int classSectionId);
        List<Exam> GetExamByExamTypeId(int examId, int clasSectionId);
        List<ExamResult> GetExamResultByExamId(int examId);
        List<ExamResultViewModel> GetExamResultModelByExamId(int examId);

        int DeleteExamResult(int examId);
        void GetNewStudentForExam(int examId);
        ExamResult GetExamResultByExamAndStudentId(int examId, int studentId);
        ExamResultViewModel GetExamResultModelByExamAndStudentId(int examId, int studentId);
        int AddExam(Exam exam);
        void UpdateExam(Exam exam);
        int AddExamResult(ExamResult examResult);
        void UpdateExamResult(ExamResult examResult);
        ExamResult GetExamResultById(int id);

        List<Activity> GetAllActivity();
        Activity GetActivityById(int id);
        void UpdateActivity(Activity activity);
        int Addactivity(Activity activity);
        Activity GetActivityByName(string name);
        Activity GetActivityByNameAndId(string name, int id);
        void DeleteActivity(Activity activity);
        void AddActivityMarks(ActivityMark marks);
        void UpdateActivityMarks(ActivityMark marks);
        void DeleteActivityMarks(ActivityMark marks);
        ActivityMark GetActivityMarksById(int id);
        List<ActivityMark> GetActivityMarksByActivityId(int activityId);
        List<ActivityMark> GetActivityMarksByClassSectionId(int classSectionId);
        ActivityMark GetActivityMarksByClassSectionAndActivtyId(int classSectionId, int activityId);
        void AddActivityMarksDetail(ActivityMarksDetail marksDetail);
        void UpdateActivityMarksDetail(ActivityMarksDetail marksDetail);
        void DeleteActivityMarksDetail(ActivityMarksDetail marksDetail);
        ActivityMarksDetail GetActivityMarksDetailById(int id);
        List<ActivityMarksDetail> GetActivityMarksDetailByActivityMarksId(int activityMarksId);
        List<ActivityMarksViewModel> GetActivityMarksModelByActivityMarksId(int activityMarksId);
        List<ActivityMarksViewModel> GetActivityMarksModelByStudentId(int studentId);

        void AddDailyTest(DailyTest test);
        void UpdateDailyTest(DailyTest test);
        void DeleteDailyTest(DailyTest test);
        DailyTest GetDailyTestById(int id);
        List<DailyTest> GetDailyTestBySubjectId(int subjectId);
        List<DailyTest> GetDailyTestByClassSectionId(int classSectionId);
        DailyTest GetDailyTestByClassSectionAndSubjectId(int classSectionId, int subjectId);
        DailyTest GetDailyTestByClassSectionAndSubjectId(int classSectionId, int subjectId, DateTime testDate);
        void AddDailyTestDetail(DailyTestsDetail testDetail);
        void UpdateDailyTestDetail(DailyTestsDetail testDetail);
        void DeleteDailyTestsDetail(DailyTestsDetail testDetail);
        DailyTestsDetail GetDailyTestsDetailById(int id);
        List<DailyTestsDetail> GetDailyTestsDetailByDailyTestId(int dailyTestId);
        List<DailyTestViewModel> GetDailyTestsModelByDailyTestId(int dailyTestId);
        List<DailyTestsDetail> GetDailyTestsDetailByStudentId(int studentId);
		List<ExamResultViewModel> GetExamResult(int studentId, string examType);
        List<DailyTestViewModel> SearchDailyTest(int classId, int sectionId, int subjectId, DateTime fromDate, DateTime toDate);

        void AddDateSheetConfig(DateSheetConfig config);
        void UpdateDateSheetConfig(DateSheetConfig config);
        DateSheetConfig GetDateSheetConfigByBranchId(int branchId);
        int GetExamTypeCount(int ExamTermId);
        int GetDateSheetCount(int ExamTypeId);
        int GetActivityCount(int ActivityId);
        List<ExamTerm> GetExamTermByYear(int year);

        List<ExamTypeModel> GetAllCurrentYearExams();
        List<ExamTypeModel> GetYearExams(int yearId, string termName);
        List<DateSheetViewModel> GetStudentDateSheet(int studentId, string examName);
        List<TimeTableViewModel> GetStudentTimeTable(int studentId);
        List<DailyTestViewModel> GetCurrentMonthDailyTest(int studentId);
        List<DailyTestViewModel> GetDailyTest(int studentId, DateTime fromDate, DateTime toDate, int SubjectId);
        List<ActivityMarksViewModel> GetCurrentMonthActivityMarks(int studentId);
        List<ActivityMarksViewModel> GetActivityMarks(int studentId, DateTime fromDate, DateTime toDate, int activityId);
        int AddExamPaper(ExamPaper paper);
        void UpdateExamPaper(ExamPaper paper);
        ExamPaperModel GetExamPaperModelById(int id);
        List<ExamPaperModel> SearchExamPapers(int classId, int sectionId, int subjectId, int examTypeId);
        List<ExamPaperModel> SearchStudentExamPapers(int studentId, int examTypeId);

        int AddDailyDairy(DailyDairy dairy);
        void UpdateDailyDairy(DailyDairy dairy);
        DailyDairyModel GetDailyDairyModelById(int id);
        List<DailyDairyModel> SearchDailyDairy(int classId, int sectionId, DateTime FileDate);
        List<DailyDairyModel> SearchDailyDairy(int classId, int sectionId, DateTime fromDate, DateTime toDate);
        List<DailyDairyModel> GetAllStudentDailyDairies(int studentId);
        List<DailyDairyModel> SearchStudentDailyDairies(int studentId, DateTime fromDate, DateTime toDate);
    }
}
