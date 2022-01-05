using SMS_Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using System.Web.Providers.Entities;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class BranchLoginController : Controller
    {
        static int errorCode = 0;
        //
        // GET: /Login/

        public BranchLoginController()
        {
            ViewData["Error"] = errorCode;
        }
        public ActionResult Index(int sessionId = 0)
        {
            try
            {
                if (sessionId == -59)
                {
                    LogoutUser();
                    return View(new SMS_DAL.Branch());
                }
                else if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                    return View(new SMS_DAL.Branch());
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch(Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("Index", "Login");
            }
        }

        private void LogoutUser()
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                UserPermissionController.LogoutUser(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "Authenticate")]
        [ValidateAntiForgeryToken]

        public ActionResult AuthenticateBranch(SMS_DAL.Branch branch)
        {
            try
            {
                errorCode = 0;
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int loginStatus = UserPermissionController.AuthenticateBranch(branch.LOGIN_ID, branch.PASSWORD);
                LogWriter.WriteLog("Branch Login Status : " + loginStatus);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (loginStatus == 0)
                {
                    errorCode = 420;
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("Index", "Login");
            }
        }
    }
}
