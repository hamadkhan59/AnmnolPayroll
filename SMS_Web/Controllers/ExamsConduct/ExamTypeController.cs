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
    public class ExamTypeController : Controller
    {
        private static int errorCode = 0;

        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;

        public ExamTypeController()
        {

            examRepo = new ExamRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
        }
        //
        // GET: /ExamType/

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_EXAMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ExamType term = new ExamType();

            try
            {
                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
                ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
                ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                term = examRepo.GetExamTypeById(Id);
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
        public ActionResult Create([Bind(Exclude = "Id")]ExamType examtype)
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
                    LogWriter.WriteLog("Model state is valid, creating exam type");
                    int termPerecentageTotal = 0;
                    if (examRepo.GetExamTypeByExamTerms((int)examtype.TermId, branchId).ToList().Count > 0)
                        termPerecentageTotal = examRepo.GetExamTypePercentageByTerm((int)examtype.TermId, branchId);
                    if (termPerecentageTotal + examtype.Percent_Of_Total <= 100)
                    {
                        examtype.BranchId = branchId;
                        //examtype.Branch = null;

                        examRepo.AddExamType(examtype);
                        errorCode = 2;
                        SessionHelper.InvalidateExamTypeCache = false;
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
                    ViewBag.TermId = new SelectList(examRepo.GetAllExamTerm(), "Id", "TermName", examtype.TermId);
                    ViewData["examTerm"] = examRepo.GetAllExamTerm();
                    ViewData["examType"] = examRepo.GetAllExamTypes();
                    return View(examtype);
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
        public ActionResult Edit(ExamType examtype)
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
                    LogWriter.WriteLog("Model state is valid, updating exam type");
                    int termPerecentageTotal = 0;
                    if (examRepo.GetExamTypeByTermAndId((int)examtype.TermId, (int)examtype.Id, branchId).ToList().Count > 0)
                        termPerecentageTotal = examRepo.GetExamTypePercentageByTermAndId((int)examtype.TermId, (int)examtype.Id, branchId);
                    if (termPerecentageTotal + examtype.Percent_Of_Total <= 100)
                    {
                        examtype.BranchId = branchId;
                        //examtype.Branch = null;

                        examRepo.UpdateExamType(examtype);
                        errorCode = 2;
                        SessionHelper.InvalidateExamTypeCache = false;
                    }
                    else
                    {
                        errorFlag = true;
                        errorCode = 100;
                    }
                }
                else
                {
                    errorFlag = true;
                }
                if (errorFlag)
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Operation"] = examtype.Id;
                    ViewData["Error"] = errorCode;
                    ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                    ViewBag.TermId = new SelectList(examRepo.GetAllExamTerm(), "Id", "TermName", examtype.TermId);
                    ViewData["examTerm"] = examRepo.GetAllExamTerm();
                    ViewData["examType"] = examRepo.GetAllExamTypes();
                    return View(examtype);
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
        // GET: /ExamType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ExamType examtype = examRepo.GetExamTypeById(id);
                if (examtype == null)
                {
                    return HttpNotFound();
                }
                int dateSheetCount = examRepo.GetDateSheetCount(id);

                if (dateSheetCount == 0)
                {
                    examRepo.DeleteExamType(examtype);
                    errorCode = 4;
                    SessionHelper.InvalidateExamTypeCache = false;
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