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
    public class StudentController : ApiController
    {
        IStudentRepository stdRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        [HttpGet]
        public string getStudent(string mobileNo)
        {
            string json = "";
            StudentResponse response = new StudentResponse();
            try
            {
                response.StudentDetail = stdRepo.GetAllStudentByParentCnic(mobileNo);
                response.StatusCode = ((response.StudentDetail != null && response.StudentDetail.Count > 0)
                                       ? AppConstHelper.SUCCESS : AppConstHelper.STUDENTS_NO_RECORD_ERROR);
                json = JsonConvert.SerializeObject(response);
            }
            catch(Exception e)
            {
                response.StatusCode = AppConstHelper.STUDENTS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }
    }
}
