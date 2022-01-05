using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StaffHandling
{
    public class SessionController : Controller
    {
        private static int errorCode = 0;
        //
        // GET: /Session/
        
        IStaffRepository staffRepo;

        public SessionController()
        {
            
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_SESSION) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Session session = new Session();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["sessions"] = SessionHelper.SessionList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                session = staffRepo.GetSessionById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(session);
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Exclude = "Id")]Session session)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (CheckSessionDates(session, branchId) == false)
                {
                    LogWriter.WriteLog("Model state is valid, creating the session");
                    errorCode = 59;
                    ViewData["Error"] = errorCode;
                    ViewData["sessions"] = SessionHelper.SessionList(Session.SessionID);
                    errorCode = 0;
                    return View(session);
                }
                var sessionObj = staffRepo.GetSessionByName(session.Name);
                if (ModelState.IsValid && sessionObj == null)
                {
                    session.BranchId = branchId;
                    session.Branch = null;

                    staffRepo.AddSession(session);
                    errorCode = 2;
                    SessionHelper.InvalidateSessionCache = false;
                }
                else if (sessionObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
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

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Session session)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (CheckSessionDatesEdit(session) == false)
                {
                    errorCode = 59;
                    ViewData["Error"] = errorCode;
                    ViewData["sessions"] = SessionHelper.SessionList(Session.SessionID);
                    ViewData["Operation"] = session.Id;
                    errorCode = 0;
                    return View(session);
                }
                var sessionObj = staffRepo.GetSessionByNameAndId(session.Name, session.Id);
                if (ModelState.IsValid && sessionObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, updating the session");
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    session.BranchId = branchId;
                    session.Branch = null;

                    staffRepo.UpdateSession(session);
                    errorCode = 2;
                    SessionHelper.InvalidateSessionCache = false;
                }
                else if (sessionObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        private bool CheckSessionDates(Session session, int branchId)
        {
            var sessionObj = staffRepo.GetSessionInDates(session, branchId);
            if (sessionObj != null)
                return false;
            return true;
        }

        private bool CheckSessionDatesEdit(Session session)
        {
            var sessionObj = staffRepo.GetEditSessionInDates(session);
            if (sessionObj != null)
                return false;
            return true;
        }

        //
        // GET: /Session/Edit/5

        
        //
        // POST: /Session/Edit/5

       
        //
        // GET: /Session/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session session = staffRepo.GetSessionById(id);
                if (session == null)
                {
                    return HttpNotFound();
                }

                int studentCount = staffRepo.GetStudentCount(id);
                if (studentCount == 0)
                {
                    staffRepo.DeleteSession(session);
                    errorCode = 4;
                    SessionHelper.InvalidateSessionCache = false;
                }
                else
                    errorCode = 40;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

    }
}