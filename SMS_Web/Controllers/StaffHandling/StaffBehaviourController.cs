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

namespace SMS_Web.Controllers.StaffAdministration
{
    public class StaffBehaviourController : Controller
    {
        private IStaffBehaviourRepository behaviourRepo;
        private static int errorCode = -1;
        private static string invalidParameterRatings = "";
        //
        // GET: /Class/

        public StaffBehaviourController()
        {
            behaviourRepo = new StaffBehaviourRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index( int staffId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {

                if (staffId > 0)
                {
                    var session = UserPermissionController.GetSessionModel(Session.SessionID);
                    var evalStaffId = session.STAFF_ID.GetValueOrDefault();
                    var viewModels = behaviourRepo.GetStaffBehaviour(staffId, evalStaffId, session.BRANCH_ID);
                    if (evalStaffId < 1)
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
            return RedirectToAction("Search", "Staff");
        }

        public ActionResult StaffProfile(int staffId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                if (staffId > 0)
                {
                    var session = UserPermissionController.GetSessionModel(Session.SessionID);
                    var viewModels = behaviourRepo.GetOverallStaffBehaviour(staffId, session.BRANCH_ID);
                    return View("Profile", viewModels);
                }

                ViewData["Error"] = errorCode;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Search", "Staff");
        }

        public ActionResult ProfileDetails(int staffId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (staffId > 0)
                {
                    var session = UserPermissionController.GetSessionModel(Session.SessionID);
                    //var viewModels = behaviourRepo.GetOverallStaffBehaviour(staffId, session.BRANCH_ID);
                    //return View("Profile", viewModels);
                }
                ViewData["StaffId"] = staffId;
                ViewData["Error"] = errorCode;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("Details");
        }

        public JsonResult GetPerformanceChartData(int staffId, int categoryId = 0, int parameterId = 0)
        {
            return Json(behaviourRepo.GetPerformance(staffId, categoryId, parameterId), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [OnAction(ButtonName = "CreateOrUpdate")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(SMS_DAL.ViewModel.StaffBehaviourViewModel viewModel)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
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
                    return RedirectToAction("Index", new { staffId = viewModel.StaffID });
                }
                var stdBehaviours = new List<StaffBehaviour>();
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
                            var stdBehaviour = new StaffBehaviour();
                            stdBehaviour.ID = detail.ID;
                            stdBehaviour.StaffId = viewModel.StaffID;
                            stdBehaviour.EvaluatingStaffId = viewModel.EvaluatingStaffID;
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
                    LogWriter.WriteLog("There is some error occure in the operation");
                    return RedirectToAction("Index", new {staffId = viewModel.StaffID});
                }
                else
                {
                    LogWriter.WriteLog("Creating or updating the staff behaviour");
                    var result = behaviourRepo.CreateOrUpdateStaffBehaviour(stdBehaviours);
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
            return RedirectToAction("Index", new { staffId = viewModel.StaffID });
        }

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
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
