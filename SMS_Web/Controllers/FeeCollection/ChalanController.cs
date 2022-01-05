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

namespace SMS_Web.Controllers.FeeCollection
{
    public class ChalanController : Controller
    {
        private static int errorCode = 0;
        
        IFeePlanRepository feePlanRepo;
        IClassRepository classRepo;
        //
        // GET: /FeeHead/

        public ChalanController()
        {

            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());; 
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_CHALLAN) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Challan head = new Challan();
            try
            {
                if (id > 0)
                    errorCode = 0;
                head = feePlanRepo.GetChallanById(id);
                int classid = 0;
                if (id > 0)
                    classid = (int)head.ClassId;
                if (Session["ClassId"] != null)
                {
                    classid = (int)Session["ClassId"];
                }
                ViewData["Operation"] = id;
                ViewData["challan"] = SessionHelper.ChallanList(Session.SessionID);

                ViewBag.Classes = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", classid);
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(head);
        }

        public ActionResult SetClassValue(int id = 0)
        {
            Session["ClassId"] = id;
            return RedirectToAction("Index", new { id = 0 });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]Challan challan)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = feePlanRepo.GetChallanByName(challan.Name, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, creating challan");
                    challan.CreatedOn = DateTime.Now;
                    challan.BranchId = branchId;
                    feePlanRepo.AddChallan(challan);
                    errorCode = 2;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["challan"] = SessionHelper.ChallanList(Session.SessionID);
                    ViewBag.Classes = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    return View(challan);
                }
                SessionHelper.InvalidateChallanCache = false;
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

        

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Challan challan)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = feePlanRepo.GetChallanByNameAndId(challan.Name, challan.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, updating challan");
                    challan.BranchId = branchId;
                    feePlanRepo.UpdateChallan(challan);
                    errorCode = 2;
                    SessionHelper.InvalidateChallanCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Operation"] = challan.Id;
                    ViewData["challan"] = SessionHelper.ChallanList(Session.SessionID);
                    ViewBag.Classes = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    return View(challan);
                }

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
        // GET: /FeeHead/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Challan challan = feePlanRepo.GetChallanById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (challan == null)
                {
                    return HttpNotFound();
                }

                int chalanCount = feePlanRepo.GetChallanDetailCount(id);
                if (chalanCount == 0)
                {
                    feePlanRepo.DeleteChallan(challan);
                    errorCode = 4;
                    SessionHelper.InvalidateChallanCache = false;
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