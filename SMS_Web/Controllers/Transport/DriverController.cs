using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class DriverController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;


        //private ISecurityRepository secRepo;
        private ITransportRepository staffRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public DriverController()
        {
            staffRepo = new TransportRepositoryImp(new SC_WEBEntities2()); ;
        }

        //
        // GET: /TransportStop/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            if (id > 0)
                errorCode = 0;
            ViewData["Operation"] = id;
            ViewData["TransportDrivers"] = SessionHelper.transportDriverList;// TransportStops
            ViewData["Error"] = errorCode;
            errorCode = 0;
            TransportDriver newMessage = staffRepo.GetTransportDriverById(id);
            ViewResult v = View(newMessage);
            return v;
        }

        //
        // POST: /TransportStops/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(TransportDriver branch)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                branch.CreatedOn = System.DateTime.Now;
                int returnCode = staffRepo.AddTransportDriver(branch);
                SessionHelper.InvalidateTransportDriverCache = false;
                if (returnCode == -1)
                    errorCode = 420;
                else
                    errorCode = 2;

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


        //
        // POST: /Branch/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransportDriver branch)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                TransportDriver updateTransportStop = staffRepo.GetTransportDriverById(branch.Id);
                updateTransportStop.DriverName = branch.DriverName;
                updateTransportStop.VanNo = branch.VanNo;
                updateTransportStop.PhoneNo = branch.PhoneNo;
                staffRepo.UpdateTransportDriver(updateTransportStop);
                errorCode = 2;
                SessionHelper.InvalidateTransportDriverCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        //
        // GET: /Branch/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.TR_DRIVER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                TransportDriver branch = staffRepo.GetTransportDriverById(id);
                if (branch == null)
                {
                    return HttpNotFound();
                }
                staffRepo.DeleteTransportDriver(branch);
                errorCode = 4;
                SessionHelper.InvalidateTransportDriverCache = false;
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
