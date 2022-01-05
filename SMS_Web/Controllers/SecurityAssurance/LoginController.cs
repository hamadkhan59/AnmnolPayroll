using SMS_Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using System.Web.Providers.Entities;
using System.Text;
using System.Reflection;
using System.IO;
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using Logger;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class LoginController : Controller
    {
        static int errorCode = 0;
        //
        // GET: /Login/
        public LoginController()
        {
            try
            {
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);

                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                string[] err = new string[1];
                err[0] = errorMessage;
                //System.IO.File.WriteAllLines(@"E:\log.txt", err);
            }
        }
        public ActionResult Index(int sessionId = 0)
        {
            try
            {
                //if (UserPermissionController.GetLoginBranchId(Session.SessionID) > 0)
                //{
                if (sessionId == -59)
                {
                    LogoutUser();
                    return View(new SMS_DAL.User());
                }
                else if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                    return View(new SMS_DAL.User());
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                string[] err = new string[1];
                err[0] = errorMessage;
                //System.IO.File.WriteAllLines(@"E:\log.txt", err);
            }
            return View();
        }

        public ActionResult BranchLogin()
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (UserPermissionController.isMoscoUser(Session.SessionID) == false)
                {
                    UserPermissionController.LogoutUser(Session.SessionID);
                    return RedirectToAction("Index", "Login");
                }

                ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
                var branch = secRepo.GetAllBranches();

                return View(branch);
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                string[] err = new string[1];
                err[0] = errorMessage;
                //System.IO.File.WriteAllLines(@"E:\log.txt", err);
            }
            return View();
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

        public ActionResult AuthenticateUser(SMS_DAL.User user)
        {
            try
            {
                errorCode = 0;
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int loginStatus = UserPermissionController.AuthenticateUser(user.LoginId, user.Password, Session.SessionID);
                LogWriter.WriteLog("User Login Status : " + loginStatus);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (loginStatus == 0)
                {
                    errorCode = 420;
                    return RedirectToAction("Index");
                }
                else if (loginStatus == 2)
                {
                    errorCode = 421;
                    return RedirectToAction("Index");
                }
                else
                {
                    if (!user.LoginId.Equals("mosco"))
                    {
                        SessionHelper.BuildCache();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("BranchLogin");
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult AuthenticateBranch(int[] BranchId)
        {
            int branchId = BranchId[0];
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                UserPermissionController.SetLoginBranchId(Session.SessionID, branchId);
                SessionHelper.BuildCache();
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
