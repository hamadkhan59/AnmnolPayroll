using Newtonsoft.Json;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace SMSApi.Controllers
{
    public class StaffController : ApiController
    {
        IStaffRepository staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        IClassSectionRepository clsecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        IAttendanceRepository attendRepo = new AttendanceRepositoryImp(new SC_WEBEntities2());
        IStudentRepository stdRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        [HttpGet]
        public string getStaffAttendance(int staffId, DateTime fromDate, DateTime toDate)
        {
            string json = "";
            StaffAttendanceResponse response = new StaffAttendanceResponse();
            try
            {
                response.StaffAttendanceDetail = staffRepo.SearchStaffAttendnaceModel(fromDate, toDate, staffId);
                response.StatusCode = ((response.StaffAttendanceDetail != null && response.StaffAttendanceDetail.Count > 0)
                                       ? AppConstHelper.SUCCESS : AppConstHelper.STAFF_ATTENDANCE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch(Exception e)
            {
                response.StatusCode = AppConstHelper.STAFF_ATTENDANCE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string getStaffAdvance(int staffId, DateTime fromDate, DateTime toDate)
        {
            string json = "";
            StaffAdvanceResponse response = new StaffAdvanceResponse();
            try
            {
                response.StaffAdvanceDetail = staffRepo.SearchStaffAdvance(staffId, fromDate, toDate);
                response.StatusCode = ((response.StaffAdvanceDetail != null && response.StaffAdvanceDetail.Count > 0)
                                       ? AppConstHelper.SUCCESS : AppConstHelper.STAFF_ADVANCE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.STAFF_ADVANCE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string getStaffSalary(int staffId, DateTime fromDate, DateTime toDate)
        {
            string json = "";
            StaffSalaryResponse response = new StaffSalaryResponse();
            try
            {
                response.StaffSalaryDetail = staffRepo.SearchStaffSalary(staffId, fromDate, toDate);
                response.StatusCode = ((response.StaffSalaryDetail != null && response.StaffSalaryDetail.Count > 0)
                                       ? AppConstHelper.SUCCESS : AppConstHelper.STAFF_SALARY_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.STAFF_SALARY_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string getStaffStudentAttendance(int ClassId, int SectionId, DateTime markDate)
        {
            string json = "";
            StaffStudentAttendanceResponse response = new StaffStudentAttendanceResponse();
            try
            {
                var clsec = clsecRepo.GetClassSectionByClassAndSectionId(ClassId, SectionId);
                response.StaffAttendanceDetail = attendRepo.GetAttendanceByDate(clsec.ClassSectionId, markDate);

                List<StudentModel> studentList = stdRepo.GetStudentByClassSectionId(clsec.ClassSectionId);
                foreach (StudentModel std in studentList)
                {
                    if (response.StaffAttendanceDetail.Where(x => x.StudentId == std.Id).FirstOrDefault() == null)
                    {
                        Attandance tempObj = new Attandance();
                        tempObj.StudentID = std.Id;
                        tempObj.AttandanceDate = markDate;
                        tempObj.StatusId = 1;
                        attendRepo.AddAttendance(tempObj);
                    }
                }

                response.StaffAttendanceDetail = attendRepo.GetAttendanceByDate(clsec.ClassSectionId, markDate);
                response.StatusCode = ((response.StaffAttendanceDetail != null && response.StaffAttendanceDetail.Count > 0)
                                       ? AppConstHelper.SUCCESS : AppConstHelper.STAFF_STD_ATTENDANCE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.STAFF_STD_ATTENDANCE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpGet]
        public string SaveStaffStudentAttendance(List<AttendanceModel> attendanceList)
        {
            string json = "";
            StaffStudentAttendanceResponse response = new StaffStudentAttendanceResponse();
            try
            {
                foreach (var obj in attendanceList)
                {
                    Attandance attendance = attendRepo.GetAttandanceById(obj.Id);
                    attendance.StatusId = obj.CurrentStatus == true ? 1 : 2;
                    attendance.CreatedOn = DateTime.Now;
                    attendRepo.UpdateAttendance(attendance);
                }

                response.StatusCode = AppConstHelper.STAFF_STD_ATTENDANCE_SUCCESS;
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.STAFF_STD_ATTENDANCE_SAVE_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }
    }
}
