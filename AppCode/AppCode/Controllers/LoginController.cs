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
    public class LoginController : ApiController
    {
        ISecurityRepository loginRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        static string parentCnic = null;
        [HttpGet]
        public string getLogin(string mobileNo, string password)
        {
            string json = "";
            ParentResponse response = new ParentResponse();
            try
            {
                AppUser parent = loginRepo.AuthenticateParent(mobileNo, password);
                if (parent != null)
                {
                    response.Parent = parent;
                    loginRepo.SetSession("Active", parent.Id);
                    response.StatusCode = AppConstHelper.SUCCESS;
                }
                else
                {
                    response.StatusCode = AppConstHelper.PARENTS_INVALID_LOGIN;
                }
                json = JsonConvert.SerializeObject(response);
            }
            catch(Exception e)
            {
                response.StatusCode = AppConstHelper.PARENTS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpPost]
        public string getRegister(AppUser parent)
        {
            string json = "";
            ParentResponse response = new ParentResponse();
            try
            {
                parent.Session = "inActive";
                string code = loginRepo.AddParent(parent);
                if (code == AppConstHelper.LOGIN_ALREADY_EXIST)
                    response.StatusCode = AppConstHelper.PARENTS_ALREADY_EXIST;
                else if (code == AppConstHelper.LOGIN_NO_DATA)
                    response.StatusCode = AppConstHelper.PARENTS_NO_STUDENT;
                else
                    response.StatusCode = AppConstHelper.SUCCESS;
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception exc)
            {
                response.StatusCode = AppConstHelper.PARENTS_REGISTER_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

        public string getParentCnic()
        {
            return parentCnic;
        }

        [HttpGet]
        public string getLoginStaff(string mobileNo, string password)
        {
            string json = "";
            ParentResponse response = new ParentResponse();
            try
            {
                AppUser parent = loginRepo.AuthenticateStaff(mobileNo, password);
                if (parent != null)
                {
                    response.Parent = parent;
                    loginRepo.SetSession("Active", parent.Id);
                    response.StatusCode = AppConstHelper.SUCCESS;
                }
                else
                {
                    response.StatusCode = AppConstHelper.PARENTS_INVALID_LOGIN;
                }
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.StatusCode = AppConstHelper.PARENTS_GENERIC_ERROR;
                json = JsonConvert.SerializeObject(response);
            }
            return json;
        }

        [HttpPost]
        public string getRegisterStaff(AppUser parent)
        {
            string json = "";
            ParentResponse response = new ParentResponse();
            try
            {
                parent.Session = "inActive";
                string code = loginRepo.RegisterStaff(parent);
                if (code == AppConstHelper.LOGIN_ALREADY_EXIST)
                    response.StatusCode = AppConstHelper.STAFF_ALREADY_EXIST;
                else if (code == AppConstHelper.LOGIN_NO_DATA)
                    response.StatusCode = AppConstHelper.STAFF_NO_STUDENT;
                else
                    response.StatusCode = AppConstHelper.SUCCESS;
                json = JsonConvert.SerializeObject(response);
            }
            catch (Exception exc)
            {
                response.StatusCode = AppConstHelper.PARENTS_REGISTER_ERROR;
                json = JsonConvert.SerializeObject(response);
            }

            return json;
        }

    }
}
