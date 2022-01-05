using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class BranchController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;

        
        private ISecurityRepository secRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public BranchController()
        {

            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());; 
        }

        //
        // GET: /Branch/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Branch) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Branch branch = new Branch();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["branch"] = SessionHelper.BranchList;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                branch = secRepo.GetBranchById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(branch);

        }

        //
        // POST: /Branch/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(Branch branch)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Branch) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var classObj = secRepo.GetBranchByName(branch.BRANCH_NAME);
                if (classObj == null)
                {
                    classObj = secRepo.GetBranchByCode(branch.BRANCH_CODE);
                    if (classObj == null)
                    {
                        classObj = secRepo.GetBranchByLoginId(branch.LOGIN_ID);

                        if (ModelState.IsValid && classObj == null)
                        {
                            int returnCode = secRepo.AddBracnh(branch);
                            SessionHelper.InvalidateBranchCache = false;
                            if (returnCode == -1)
                                errorCode = 420;
                            else
                                errorCode = 2;
                        }
                        else if (classObj != null)
                        {
                            errorCode = 11;
                            ViewData["Error"] = errorCode;
                            ViewData["branch"] = SessionHelper.BranchList;
                            return View(branch);
                        }
                        else
                            errorCode = 1;
                    }
                    else
                    {
                        errorCode = 422;
                        ViewData["Error"] = errorCode;
                        ViewData["branch"] = SessionHelper.BranchList;
                        return View(branch);
                    }
                }
                else
                {
                    errorCode = 421;
                    ViewData["Error"] = errorCode;
                    ViewData["branch"] = SessionHelper.BranchList;
                    return View(branch);
                }
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
        // POST: /Branch/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Branch branch)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Branch) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var classObj = secRepo.GetBranchByNameAndId(branch.BRANCH_NAME, branch.ID);
                if (classObj == null)
                {
                    classObj = secRepo.GetBranchByCodeAndId(branch.BRANCH_CODE, branch.ID);
                    if (classObj == null)
                    {
                        classObj = secRepo.GetBranchByLoginIdAndId(branch.LOGIN_ID, branch.ID);
                        if (ModelState.IsValid && classObj == null)
                        {
                            secRepo.UpdateBranch(branch);
                            errorCode = 2;
                            SessionHelper.InvalidateBranchCache = false;
                        }
                        else if (classObj != null)
                        {
                            errorCode = 111;
                            ViewData["Operation"] = branch.ID;
                            ViewData["Error"] = errorCode;
                            ViewData["branch"] = SessionHelper.BranchList;
                            return View(branch);
                        }
                        else
                            errorCode = 1;
                    }
                    else
                    {
                        errorCode = 1422;
                        ViewData["Operation"] = branch.ID;
                        ViewData["Error"] = errorCode;
                        ViewData["branch"] = SessionHelper.BranchList;
                        return View(branch);
                    }
                }
                else
                {
                    errorCode = 1421;
                    ViewData["Operation"] = branch.ID;
                    ViewData["Error"] = errorCode;
                    ViewData["branch"] = SessionHelper.BranchList;
                    return View(branch);
                }
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
        // GET: /Branch/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Branch) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Branch branch = secRepo.GetBranchById(id);
                if (branch == null)
                {
                    return HttpNotFound();
                }
                secRepo.DeleteBranch(branch);
                errorCode = 4;
                SessionHelper.InvalidateBranchCache = false;
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