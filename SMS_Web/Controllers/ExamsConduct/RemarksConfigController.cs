﻿using System;
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

namespace SMS_Web.Controllers.ExamsConduct
{
    public class RemarksConfigController : Controller
    {
        private static int errorCode = 0;


        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        //
        // GET: /ExamTerm/

        public RemarksConfigController()
        {

            examRepo = new ExamRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_GRADE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            RemarksConfig config = new RemarksConfig();
            try
            {
                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewData["remarks"] = SessionHelper.RemarksConfigList();

                config = examRepo.GetRemarksById(Id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(config);
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]RemarksConfig remarksconfig)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool overlap = SessionHelper.IsRemarksOverlapped(remarksconfig);
                if (overlap)
                    errorCode = 110;
                else
                {
                    var temp = examRepo.GetRemarksByName(remarksconfig.Remarks);
                    if (temp == null)
                    {
                        remarksconfig.CreatedOn = DateTime.Now;
                        examRepo.AddRemarks(remarksconfig);
                        errorCode = 2;
                        SessionHelper.InvalidateRemarksCache = false;
                    }
                    else
                    {
                        errorCode = 11;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = -59 });
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RemarksConfig remarksconfig)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool overlap = SessionHelper.IsRemarksOverlapped(remarksconfig);
                if (overlap)
                    errorCode = 110;
                else
                {
                    var temp = examRepo.GetRemarksByNameAndId(remarksconfig.Remarks, remarksconfig.Id);
                    if (temp == null)
                    {
                        remarksconfig.CreatedOn = DateTime.Now;
                        examRepo.UpdateRemarks(remarksconfig);
                        errorCode = 2;
                        SessionHelper.InvalidateRemarksCache = false;
                    }
                    else
                    {
                        errorCode = 11;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        //
        // GET: /ExamTerm/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                RemarksConfig config = examRepo.GetRemarksById(id);
                if (config == null)
                {
                    return HttpNotFound();
                }
                examRepo.DeleteRemarks(config);
                errorCode = 4;
                SessionHelper.InvalidateRemarksCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = -59 });
        }


    }
}
