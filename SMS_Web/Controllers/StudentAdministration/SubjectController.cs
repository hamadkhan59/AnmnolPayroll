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

namespace SMS_Web.Controllers.StudentAdministration
{
    public class SubjectController : Controller
    {
        private static int errorCode = 0;
        
        private ISubjectRepository subjectRepo;
        private IClassSubjectRepository classSubjRepo;

        //
        // GET: /Subject/

        public SubjectController()
        {
            classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());;
            subjectRepo = new SubjectRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_SUBJECTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Subject subject = new Subject();
            try
            {
                ViewData["Operation"] = id;
                ViewData["subject"] = SessionHelper.SubjectList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                subject = subjectRepo.GetSubjectById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(subject);
        }

        //
        // POST: /Subject/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]Subject subject)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try 
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var subjObj = subjectRepo.GetSubjectByName(subject.Name, branchId);

                if (ModelState.IsValid && subjObj == null)
                {
                    subject.BranchId = branchId;
                    subject.Branch = null;

                    subjectRepo.AddSubject(subject);
                    errorCode = 2;
                    SessionHelper.InvalidateSubjectCache = false;
                }
                else if (subjObj != null)
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


        //
        // POST: /Subject/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Subject subject)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var subjObj = subjectRepo.GetSubjectByNameAndId(subject.Name, subject.Id, branchId);

                if (ModelState.IsValid && subjObj == null)
                {
                    subject.BranchId = branchId;
                    subject.Branch = null;

                    subjectRepo.UpdateSubject(subject);
                    errorCode = 2;
                    SessionHelper.InvalidateSubjectCache = false;
                }
                else if (subjObj != null)
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
            return RedirectToAction("Index", new { id = 0 });
        }

        //
        // GET: /Subject/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Subject subject = subjectRepo.GetSubjectById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (subject == null)
                {
                    return HttpNotFound();
                }
                int subjectCount = classSubjRepo.GetSubjectCount(id);
                if (subjectCount == 0)
                {
                    subjectRepo.DeleteSubject(subject);
                    errorCode = 4;
                    SessionHelper.InvalidateSubjectCache = false;
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