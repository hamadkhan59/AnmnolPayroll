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
using Newtonsoft.Json;
using System.Web.Script.Services;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class BehaviourCategoryController : Controller
    {
        private IBehaviourRepository behaviourRepo;
        private static int errorCode = -1;
        //
        // GET: /Class/

        public BehaviourCategoryController()
        {
            behaviourRepo = new BehaviourRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index( int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_BEHAVIOUR_CATEGORIES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            BehaviourCategory clas = new BehaviourCategory();
            try
            {
                ViewData["Operation"] = id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["categories"] = behaviourRepo.GetCategoriesByBranch(branchId);
                ViewData["Error"] = errorCode;
                errorCode = -1;

                if (id > 0)
                {
                    clas = behaviourRepo.GetCategory(id);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(clas);
        }

        [HttpGet]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult GetCategoriesByBranch(int branchId = 0)
        {
            if (branchId == 0)
            {
                branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            }

            return Json(behaviourRepo.GetCategoryViewsByBranch(branchId), JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "CreateOrUpdate")]
        [ValidateAntiForgeryToken]

        public ActionResult CreateOrUpdate(BehaviourCategory category)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_BEHAVIOUR_CATEGORIES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                category.BranchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                
                if (ModelState.IsValid && category != null)
                {
                    var obj = behaviourRepo.CreateOrUpdateCategory(category);
                    if (obj == null)
                        errorCode = 420;
                    else
                        errorCode = 200;
                    //SessionHelper.InvalidateClassCache = false;
                }
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

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_BEHAVIOUR_CATEGORIES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var returnVal = behaviourRepo.DeleteCategory(id);
                if (returnVal == false)
                {
                    return HttpNotFound();
                }
                //SessionHelper.InvalidateClassCache = false;
                errorCode = 201;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 421;
            }
            return RedirectToAction("Index", new { id = 0 });
        }
    }
}
