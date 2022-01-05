using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SMSApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CommonController : ApiController
    {
        IStaffRepository staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        //public string[] Get()
        //{
        //    return new string[]
        //    {
        //        "Hello",
        //        "World"
        //    };
        //}

        public string AuthenticateUser(string userName, string password)
        {
            string errorCode = "000";
            try
            {
                User user = secRepo.AuthenticateUser(userName, password);
                if (user == null)
                    errorCode = "200";
            }
            catch
            {
                errorCode = "420";
            }
            return errorCode;
        }

        public string AddStaffBioMatric(string staffId, string bioHash)
        {
            return staffRepo.AddStaffBioMatric(staffId, bioHash);
        }

        public string MarkAttendance(string bioHash)
        {
            return "000";
        }

        public string SearchStaffId(string staffId)
        {
            return staffRepo.GetAPIStaffById(staffId);
        }

        public List<string> GetAllStaff()
        {
            return staffRepo.GetAPIAllStaff();
        }
    }
}
