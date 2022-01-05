using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FinanceOperations
{
    public class DriverStopController : Controller
    {
        private ITransportRepository accountRepo;

        private static int errorCode = 0;
        //
        // GET: /Class/

        public DriverStopController()
        {

            accountRepo = new TransportRepositoryImp(new SC_WEBEntities2()); ;
        }

        //
        // GET: /FinanceSeccondLvl/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER_STOP) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.DriverId = new SelectList(SessionHelper.transportDriverList, "Id", "DriverName");
                ViewBag.SearchDriverId = new SelectList(SessionHelper.transportDriverList, "Id", "DriverName");
                ViewData["TransportStops"] = SessionHelper.transportStopList;
                ViewData["DriverStops"] = accountRepo.GetAllTransportDriverStopModel();
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(int DriverId, string StopName)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER_STOP) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var stop = accountRepo.GetTransportStopByName(StopName);
                if (stop == null)
                {
                    errorCode = 330;
                }
                else
                {
                    var tempObj = accountRepo.GetDriverStopByIdAndStopId(DriverId, stop.Id);
                    if (tempObj == null)
                    {
                        TransportDriverStop driverStop = new TransportDriverStop();
                        driverStop.StopId = stop.Id;
                        driverStop.DriverId = DriverId;
                        accountRepo.AddDriverStop(driverStop);
                        errorCode = 2;
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
            return RedirectToAction("Index");

        }


        
        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER_STOP) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                TransportDriverStop stop = accountRepo.GetDriverStopById(id);
                if (stop == null)
                {
                    return HttpNotFound();
                }
                accountRepo.DeleteTransportDriverStop(stop);
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