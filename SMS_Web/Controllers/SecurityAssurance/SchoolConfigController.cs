using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class SchoolConfigController : Controller
    {
        //
        // GET: /SchoolConfig/
        private ISecurityRepository secRepo;
        static int errorCode = 0;

        public SchoolConfigController()
        {
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckAdminLogin(Session.SessionID) == 0)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            SchoolConfig config = new SchoolConfig();

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                config = secRepo.GetSchoolConfigByBranchId(branchId);

                ViewData["Error"] = errorCode;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(config);
        }


        [HttpPost]
        public void UploadStudentDocs()
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (System.Web.HttpContext.Current.Request.Files["UploadedImage"] != null)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage"];
                    using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                    {
                        byte[] doc = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                        Session["SchoolLogo"] = doc;
                    }
                }

                if (System.Web.HttpContext.Current.Request.Files["UploadedImage1"] != null)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage1"];
                    using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                    {
                        byte[] doc = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                        Session["CardImage"] = doc;
                    }
                }

                if (System.Web.HttpContext.Current.Request.Files["UploadedImage2"] != null)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage2"];
                    using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                    {
                        byte[] doc = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                        Session["PrincipalSignature"] = doc;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SchoolConfig schoolConfig)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);

                if (config == null)
                    config = new SchoolConfig();
                config.SchoolName = schoolConfig.SchoolName;
                config.CampusName = schoolConfig.CampusName;
                if (Session["SchoolLogo"] != null) 
                {
                    LogWriter.WriteLog("Adding School Logo");
                    config.SchoolLogo = (byte[])Session["SchoolLogo"];
                }

                if (Session["CardImage"] != null)
                {
                    LogWriter.WriteLog("Adding Card Image");
                    config.CardImage = (byte[])Session["CardImage"];
                }

                if (Session["PrincipalSignature"] != null)
                {
                    LogWriter.WriteLog("Adding Pricnipal Signature");
                    config.PrincipalSignature = (byte[])Session["PrincipalSignature"];
                }

                config.BranchId = branchId;

                if (config.SchoolId > 0)
                {
                    LogWriter.WriteLog("Updating School Config");
                    secRepo.UpdateSchoolConfig(config);
                }
                else
                {
                    LogWriter.WriteLog("Adding School Config");
                    secRepo.AddSchoolConfig(config);
                }
                
                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index");
        }
    }
}
