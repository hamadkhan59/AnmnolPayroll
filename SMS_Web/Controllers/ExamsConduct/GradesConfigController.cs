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
    public class GradesConfigController : Controller
    {
        private static int errorCode = 0;


        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        //
        // GET: /ExamTerm/

        public GradesConfigController()
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
            GradesConfig grade = new GradesConfig();

            try
            {
                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewData["grades"] = SessionHelper.GradesConfigList();

                grade = examRepo.GetGradeById(Id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(grade);
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]GradesConfig gradesconfig)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool overlap = SessionHelper.IsGradesOverlapped(gradesconfig);
                if (overlap)
                    errorCode = 110;
                else
                {
                    var temp = examRepo.GetGradeByName(gradesconfig.Grade);
                    if (temp == null)
                    {
                        gradesconfig.CreatedOn = DateTime.Now;
                        examRepo.AddGrade(gradesconfig);
                        errorCode = 2;
                        SessionHelper.InvalidateGradeCache = false;
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
        public ActionResult Edit(GradesConfig gradesconfig)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool overlap = SessionHelper.IsGradesOverlapped(gradesconfig);
                if (overlap)
                    errorCode = 110;
                else
                {
                    var temp = examRepo.GetGradeByNameAndId(gradesconfig.Grade, gradesconfig.Id);
                    if (temp == null)
                    {
                        gradesconfig.CreatedOn = DateTime.Now;
                        examRepo.UpdateGrade(gradesconfig);
                        errorCode = 2;
                        SessionHelper.InvalidateGradeCache = false;
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
        public JsonResult GetGradeList()
        {
            var gradeList = SessionHelper.GradesConfigList();
            List<string[]> gradeDetail = gradeList.Select(x => new string[] { x.Grade.ToString(), x.MinRange.ToString(), x.MaxRange.ToString() }).ToList();
            return Json(gradeDetail);
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
                GradesConfig grade = examRepo.GetGradeById(id);
                if (grade == null)
                {
                    return HttpNotFound();
                }
                examRepo.DeleteGrade(grade);
                errorCode = 4;
                SessionHelper.InvalidateGradeCache = false;
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
