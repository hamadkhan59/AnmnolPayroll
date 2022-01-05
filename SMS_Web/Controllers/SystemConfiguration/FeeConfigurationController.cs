using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.FeeCollection
{
    public class FeeConfigurationController : Controller
    {
        private static int errorCode = 0;

        IFeePlanRepository feePlanRepo;

        public FeeConfigurationController()
        {
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SC_FEE_CONFIG) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            IssueChallanConfig FineObj = new IssueChallanConfig();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                FineObj = feePlanRepo.GetFine(branchId);
                ViewBag.StudentPerChallan = new SelectList(SessionHelper.StudentPerChallanList, "Id", "Description", FineObj.StudentPerChallan);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                //FeeHead head = feePlanRepo.GetFeeHeadById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(FineObj);
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]IssueChallanConfig issueChallanFine)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SC_FEE_CONFIG) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = feePlanRepo.GetFine(branchId);
                if (classObj == null)
                {
                    classObj = new IssueChallanConfig();
                    classObj.Fine = issueChallanFine.Fine;
                    classObj.AttendanceDays = issueChallanFine.AttendanceDays;
                    classObj.FinePerDay = issueChallanFine.FinePerDay;
                    classObj.BranchId = branchId;
                    classObj.Text1 = issueChallanFine.Text1;
                    classObj.Text2 = issueChallanFine.Text2;
                    classObj.Text3 = issueChallanFine.Text3;
                    classObj.Text4 = issueChallanFine.Text4;
                    feePlanRepo.AddFineValue(classObj);
                }
                else
                {
                    classObj.Fine = issueChallanFine.Fine;
                    classObj.AttendanceDays = issueChallanFine.AttendanceDays;
                    classObj.FinePerDay = issueChallanFine.FinePerDay;
                    classObj.BranchId = branchId;
                    classObj.StudentPerChallan = issueChallanFine.StudentPerChallan;
                    classObj.Text1 = issueChallanFine.Text1;
                    classObj.Text2 = issueChallanFine.Text2;
                    classObj.Text3 = issueChallanFine.Text3;
                    classObj.Text4 = issueChallanFine.Text4;
                    feePlanRepo.UpdateFineValue(classObj);
                }
                //return View(issueChallanFine);
                errorCode = 2;
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
        public ActionResult Edit(FeeHead feehead)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SC_FEE_CONFIG) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = feePlanRepo.GetFeeHeadByNameAndId(feehead.Name, feehead.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    feehead.BranchId = branchId;
                    feehead.Branch = null;

                    feePlanRepo.UpdateFeeHead(feehead);
                    errorCode = 2;
                    SessionHelper.InvalidateFeeHeadCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    ViewData["feeHeads"] = SessionHelper.FeeHeadList(Session.SessionID);
                    return View(feehead);
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















    }
}
