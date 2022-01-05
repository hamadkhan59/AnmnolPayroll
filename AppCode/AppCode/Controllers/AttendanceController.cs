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
using System.Web.Http.Results;
using System.Web.Script.Serialization;

namespace SMSApi.Controllers
{
    public class AttendanceController : ApiController
    {
        IAttendanceRepository attendanceRepo = new AttendanceRepositoryImp(new SC_WEBEntities2());
        static string cnic = null;

        public string getAttendanceStatus(DateTime fromDate, DateTime toDate, int studentId)
        {
            string json = "";
            AttendanceResponse response = new AttendanceResponse();
            try
            {
                response.Attendance =  attendanceRepo.GetAttendanceStatus(studentId, fromDate, toDate);
                response.StatusCode = ((response.Attendance != null && response.Attendance.Count > 0) 
                                        ? AppConstHelper.SUCCESS : AppConstHelper.ATTENDANCE_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch(Exception exc)
            {
                response.StatusCode = AppConstHelper.ATTENDANCE_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }
    }
}
