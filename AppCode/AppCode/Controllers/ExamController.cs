using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Web;
using SMS_DAL.ViewModel;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace SMSApi.Controllers
{
    public class ExamController : ApiController
    {
        IExamRepository examRepo = new ExamRepositoryImp(new SC_WEBEntities2());

        [HttpGet]
        public string getExamTerms(int yearId, int branchId)
        {
            string json = "";
            ExamTermResponse response = new ExamTermResponse();
            try
            {
                response.Terms = examRepo.GetExamTermModelByYear(yearId, branchId);
                response.StatusCode = ((response.Terms != null && response.Terms.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.TERMS_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.TERMS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }


        [HttpGet]
        public string getYears()
        {
            string json = "";
            YearResponse response = new YearResponse();
            try
            {
                response.Years = examRepo.getAllYearsModel();
                response.StatusCode = ((response.Years != null && response.Years.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.YEAR_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.YEAR_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string getStudentSubject(int studentId)
        {
            string json = "";
            StudentSubjecResponse response = new StudentSubjecResponse();
            try
            {
                response.Subjects = examRepo.getStudentSubject(studentId);
                response.StatusCode = ((response.Subjects != null && response.Subjects.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.SUBJECT_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.SUBJECT_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string getExamTypes(string termName, int branchId)
        {
            string json = "";
            ExamTypeResponse response = new ExamTypeResponse();
            try
            {
                response.Types = examRepo.GetExamTypeModel(termName, branchId);
                response.StatusCode = ((response.Types != null && response.Types.Count > 0)
                                       ? AppConstHelper.SUCCESS : AppConstHelper.TYPE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.TYPE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string examResult(string examType, int studentId)
        {
            string json = "";
            ExamResultResponse response = new ExamResultResponse();
            try
            {
                response.ExamResults = examRepo.GetExamResult(studentId, examType);
                response.StatusCode = ((response.ExamResults != null && response.ExamResults.Count > 0)
                                      ? AppConstHelper.SUCCESS : AppConstHelper.RESULTS_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.RESULTS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string getAllCurrentYearExams()
        {
            string json = "";
            ExamTypeResponse response = new ExamTypeResponse();
            try
            {
                response.Types = examRepo.GetAllCurrentYearExams();
                response.StatusCode = ((response.Types != null && response.Types.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.TYPE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.TYPE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getYearExams(int yearId, string termName)
        {
            string json = "";
            ExamTypeResponse response = new ExamTypeResponse();
            try
            {
                response.Types = examRepo.GetYearExams(yearId, termName);
                response.StatusCode = ((response.Types != null && response.Types.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.TYPE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.TYPE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getStudentDateSheet(int studentId, string examName)
        {
            string json = "";
            DateSheetResponse response = new DateSheetResponse();
            try
            {
                response.DateSheet = examRepo.GetStudentDateSheet(studentId, examName);
                response.StatusCode = ((response.DateSheet != null && response.DateSheet.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.DATESHEET_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.DATESHEET_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getStudentTimeTable(int studentId)
        {
            string json = "";
            TimeTableResponse response = new TimeTableResponse();
            try
            {
                response.TimeTable = examRepo.GetStudentTimeTable(studentId);
                response.StatusCode = ((response.TimeTable != null && response.TimeTable.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.TIMETABLE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.TIMETABLE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getCurrentMonthDailyTest(int studentId)
        {
            string json = "";
            DailyTestResponse response = new DailyTestResponse();
            try
            {
                response.DailyTest = examRepo.GetCurrentMonthDailyTest(studentId);
                response.StatusCode = ((response.DailyTest != null && response.DailyTest.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.DAILYTEST_EMPTY_MONTH);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.DAILYTEST_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getDailyTest(int studentId, DateTime fromDate, DateTime toDate, int subjectId)
        {
            string json = "";
            DailyTestResponse response = new DailyTestResponse();
            try
            {
                response.DailyTest = examRepo.GetDailyTest(studentId, fromDate, toDate, subjectId);
                response.StatusCode = ((response.DailyTest != null && response.DailyTest.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.DAILYTEST_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.DAILYTEST_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getCurrentMonthActivities(int studentId)
        {
            string json = "";
            ActivityMarksResponse response = new ActivityMarksResponse();
            try
            {
                response.ActivityMarks = examRepo.GetCurrentMonthActivityMarks(studentId);
                response.StatusCode = ((response.ActivityMarks != null && response.ActivityMarks.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.ACTIVITYMARKS_EMPTY_MONTH);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.ACTIVITYMARKS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getActivityMarks(int studentId, DateTime fromDate, DateTime toDate, int activityId)
        {
            string json = "";
            ActivityMarksResponse response = new ActivityMarksResponse();
            try
            {
                response.ActivityMarks = examRepo.GetActivityMarks(studentId, fromDate, toDate, activityId);
                response.StatusCode = ((response.ActivityMarks != null && response.ActivityMarks.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.ACTIVITYMARKS_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.ACTIVITYMARKS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        [HttpGet]
        public string getStudentExamPapers(int studentId, int examTypeId)
        {
            string json = "";
            ExamPaperResponse response = new ExamPaperResponse();
            try
            {
                response.ExamPapers = examRepo.SearchStudentExamPapers(studentId, examTypeId);
                response.StatusCode = ((response.ExamPapers != null && response.ExamPapers.Count > 0)
                                        ? AppConstHelper.SUCCESS : AppConstHelper.EXAMPAPERS_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.EXAMPAPERS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

    }
}
