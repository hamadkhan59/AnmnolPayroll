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
using System.Web.Script.Services;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class BehaviourParametersController : Controller
    {
        private IBehaviourRepository behaviourRepo;
        private static int errorCode = -1;
        //
        // GET: /Class/

        public BehaviourParametersController()
        {
            behaviourRepo = new BehaviourRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_BEHAVIOUR_PARAMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            BehaviourParameter param = new BehaviourParameter();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewData["Operation"] = id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.Categories = new SelectList(behaviourRepo.GetCategoriesByBranch(branchId), "ID", "Name", param.CategoryId);
                ViewData["parameters"] = behaviourRepo.GetParametersByBranch(branchId);
                ViewData["Error"] = errorCode;
                errorCode = -1;

                if (id > 0)
                {
                    param = behaviourRepo.GetParameter(id);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(param);
        }

        [HttpGet]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public JsonResult GetByCategory(int categoryId = 0)
        {
            return Json(behaviourRepo.GetParameterViewByCategory(categoryId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "CreateOrUpdate")]
        [ValidateAntiForgeryToken]

        public ActionResult CreateOrUpdate(BehaviourParameter parameter)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_BEHAVIOUR_PARAMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (ModelState.IsValid && parameter != null)
                {
                    var obj = behaviourRepo.CreateOrUpdateParameter(parameter);
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

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_BEHAVIOUR_PARAMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                var returnVal = behaviourRepo.DeleteParameter(id);
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
