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
    public class SectionController : Controller
    {
        //private cs db = new cs();
        private static int errorCode = 0;
        
        private ISectionRepository secRepo;
        private IClassSectionRepository classSecRepo;
        //
        // GET: /Section/

        public SectionController()
        {

            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_SECTIONS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Section section = new Section();
            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["section"] = SessionHelper.SectionList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                section = secRepo.GetSectionById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(section);
        }

        //
        // POST: /Section/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]Section section)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try 
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var secObj = secRepo.GetSectionByName(section.Name, branchId);
                if (ModelState.IsValid && secObj == null)
                {
                    section.BranchId = branchId;
                    section.Branch = null;

                    int returnStatus = secRepo.AddSection(section);
                    if (returnStatus == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateSectionCache = false;
                    SessionHelper.InvalidateClassSectionCache = false;
                }
                else if (secObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Section/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Section section)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var secObj = secRepo.GetSectionByNameAndId(section.Name, section.Id, branchId);

                if (ModelState.IsValid && secObj == null)
                {
                    section.BranchId = branchId;
                    section.Branch = null;

                    secRepo.UpdateSection(section);
                    errorCode = 2;
                    SessionHelper.InvalidateSectionCache = false;
                    SessionHelper.InvalidateClassSectionCache = false;
                }
                else if (secObj != null)
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

        //
        // GET: /Section/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Section section = secRepo.GetSectionById(id);
                if (section == null)
                {
                    return HttpNotFound();
                }
                int sectionCount = classSecRepo.GetSectionCount(id);
                if (sectionCount == 0)
                {
                    secRepo.DeleteSection(section);
                    errorCode = 4;
                    SessionHelper.InvalidateSectionCache = false;
                    SessionHelper.InvalidateClassSectionCache = false;
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