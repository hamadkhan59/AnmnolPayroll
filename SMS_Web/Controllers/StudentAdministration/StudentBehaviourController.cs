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
    public class StudentBehaviourController : Controller
    {
        private IBehaviourRepository behaviourRepo;
        private static int errorCode = -1;
        private static string invalidParameterRatings = "";
        //
        // GET: /Class/

        public StudentBehaviourController()
        {
            behaviourRepo = new BehaviourRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index( int studentId = 0)
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
                if (studentId > 0)
                {
                    var session = UserPermissionController.GetSessionModel(Session.SessionID);
                    var staffId = session.STAFF_ID.GetValueOrDefault();
                    var viewModels = behaviourRepo.GetStudentBehaviour(studentId, staffId, session.BRANCH_ID);

                    if (staffId < 1)
                    {
                        errorCode = 400;
                    }
                    ViewData["Error"] = errorCode;
                    ViewData["invalidParameterRatings"] = invalidParameterRatings;
                    return View(viewModels);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Search", "Student");
        }

        public ActionResult StudentProfile(int studentId = 0)
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
                if (studentId > 0)
                {
                    var session = UserPermissionController.GetSessionModel(Session.SessionID);
                    var viewModels = behaviourRepo.GetOverallStudentBehaviour(studentId, session.BRANCH_ID);
                    return View("Profile", viewModels);
                }

                ViewData["Error"] = errorCode;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Search", "Student");
        }

        public ActionResult ProfileDetails(int studentId = 0)
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
                if (studentId > 0)
                {
                    var session = UserPermissionController.GetSessionModel(Session.SessionID);
                    //var viewModels = behaviourRepo.GetOverallStudentBehaviour(studentId, session.BRANCH_ID);
                    //return View("Profile", viewModels);
                }
                ViewData["StudentId"] = studentId;
                ViewData["Error"] = errorCode;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("Details");
        }

        public JsonResult GetPerformanceChartData(int studentId, int categoryId = 0, int parameterId = 0)
        {
            return Json(behaviourRepo.GetStudentPerformance(studentId, categoryId, parameterId), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [OnAction(ButtonName = "CreateOrUpdate")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(SMS_DAL.ViewModel.StudentBehaviourViewModel viewModel)
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
                errorCode = 0;
                invalidParameterRatings = "";

                if (viewModel.StaffID < 1)
                {
                    errorCode = 400;
                    return RedirectToAction("Index", new { studentId = viewModel.StudentID });
                }
                var stdBehaviours = new List<StudentBehaviour>();
                foreach (var cat in viewModel.Categories)
                {
                    foreach (var parameter in cat.Parameters)
                    {
                        foreach (var detail in parameter.BehaviourDetails.Where(n => n.StaffRating > -1))
                        {
                            if (detail.StaffRating > parameter.ParameterRating)
                            {
                                invalidParameterRatings += parameter.ParameterName + " (" + parameter.ParameterRating + "), ";
                                errorCode = 300;
                            }
                            var stdBehaviour = new StudentBehaviour();
                            stdBehaviour.ID = detail.ID;
                            stdBehaviour.StudentId = viewModel.StudentID;
                            stdBehaviour.StaffId = viewModel.StaffID;
                            stdBehaviour.ParameterId = parameter.ParameterID;
                            stdBehaviour.ParameterRating = parameter.ParameterRating;

                            stdBehaviour.Rating = detail.StaffRating;
                            stdBehaviour.RatingWeightage = ((double)detail.StaffRating / parameter.ParameterRating) * 100.0d;
                            stdBehaviour.Comment = detail.StaffComment;
                            stdBehaviour.CreatedOn = detail.Date;

                            stdBehaviours.Add(stdBehaviour);
                        }
                        
                    }                    
                }

                if (errorCode > 0)
                {
                    return RedirectToAction("Index", new {studentId = viewModel.StudentID});
                }
                else
                {
                    var result = behaviourRepo.CreateOrUpdateStudentBehaviour(stdBehaviours);
                    if (result == false)
                    {
                        errorCode = 420;
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
            return RedirectToAction("Index", new { studentId = viewModel.StudentID });
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
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var returnVal = behaviourRepo.DeleteParameter(id);
                if (returnVal == false)
                {
                    return HttpNotFound();
                }
                //SessionHelper.InvalidateClassCache = false;
                errorCode = 4;
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
