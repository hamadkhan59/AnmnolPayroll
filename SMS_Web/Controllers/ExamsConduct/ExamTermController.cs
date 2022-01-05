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

namespace SMS_Web.Controllers.ExamsConduct
{
    public class ExamTermController : Controller
    {
        private static int errorCode = 0;

        
        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        //
        // GET: /ExamTerm/

        public ExamTermController()
        {

            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_TERMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            ExamTerm term = new ExamTerm();
            try
            {
                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                term = examRepo.GetExamTermById(Id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(term);
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]ExamTerm examterm)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            bool errorFlag = false;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Model state is valid, creating exam term");
                    int termPerecentageTotal = 0;
                    if (examRepo.GetExamTermByYear((int)examterm.Year, branchId).Count > 0)
                        termPerecentageTotal = examRepo.GetExamTermPercentageByYear((int)examterm.Year, branchId);
                    if (termPerecentageTotal + examterm.Percentage <= 100)
                    {
                        examterm.BranchId = branchId;
                        examterm.Branch = null;

                        examRepo.AddExamTerm(examterm);
                        errorCode = 2;
                        SessionHelper.InvalidateExamTermCache = false;
                    }
                    else
                    {
                        errorCode = 100;
                        errorFlag = true;
                    }
                }
                else
                {
                    errorFlag = true;
                }
                if (errorFlag)
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Error"] = errorCode;
                    ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                    ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                    return View(examterm);
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
        public ActionResult Edit(ExamTerm examterm)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            bool errorFlag = false;
            try
            {
                //int termPerecentage = (int)db.ExamTerms.Find(examterm.Id).Percentage;
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Model state is valid, updating exam term");
                    int termPerecentageTotal = 0;
                    if (examRepo.GetExamTermByYearAndId((int)examterm.Year, (int)examterm.Id, branchId).ToList().Count > 0)
                        termPerecentageTotal = examRepo.GetExamTermPercentageByYearAndId((int)examterm.Year, (int)examterm.Id, branchId);
                    if (termPerecentageTotal + examterm.Percentage <= 100)
                    {
                        examterm.BranchId = branchId;
                        examterm.Branch = null;

                        examRepo.UpdateExamTerm(examterm);
                        errorCode = 2;
                        SessionHelper.InvalidateExamTermCache = false;
                    }
                    else
                    {
                        errorCode = 100;
                        errorFlag = true;
                    }
                }
                else
                {
                    errorFlag = true;
                }
                if (errorFlag)
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Error"] = errorCode;
                    ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                    ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                    ViewData["Operation"] = examterm.Id;
                    return View(examterm);
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
                ExamTerm examterm = examRepo.GetExamTermById(id);
                if (examterm == null)
                {
                    return HttpNotFound();
                }
                int examTypeCount = examRepo.GetExamTypeCount(id);

                if (examTypeCount == 0)
                {
                    examRepo.DeleteExamTerm(examterm);
                    errorCode = 4;
                    SessionHelper.InvalidateExamTermCache = false;
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
            return RedirectToAction("Index", new { id = -59 });
        }
    }
}