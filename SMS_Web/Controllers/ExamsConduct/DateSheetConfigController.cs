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
    public class DateSheetConfigController : Controller
    {
        //
        // GET: /SchoolConfig/
        IExamRepository examRepo;
        static int errorCode = 0;

        public DateSheetConfigController()
        {
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckAdminLogin(Session.SessionID) == 0)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_DATE_SHEET_CONFIG) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            DateSheetConfig config = new DateSheetConfig();
            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                config = examRepo.GetDateSheetConfigByBranchId(branchId);

                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(config);
        }



        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateSheetConfig dateSheetConfig)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                DateSheetConfig config = examRepo.GetDateSheetConfigByBranchId(branchId);

                if (config == null)
                    config = new DateSheetConfig();
                config.Notes = dateSheetConfig.Notes;
                config.ContactNo = dateSheetConfig.ContactNo;
                config.BranchId = branchId;

                if (config.Id > 0)
                {
                    examRepo.UpdateDateSheetConfig(config);
                }
                else
                {
                    examRepo.AddDateSheetConfig(config);
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
