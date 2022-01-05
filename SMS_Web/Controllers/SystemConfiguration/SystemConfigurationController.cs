using Logger;
using SMS_DAL;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.SystemConfiguration
{
    public class SystemConfigurationController : Controller
    {
        //
        // GET: /SystemConfiguration/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult ExamConfig()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SC_EXAM_CONFIG) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.YesNoDD = new SelectList(SysConfig.GetYesNoDD(), "Id", "Value");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveExamConfig(List<SystemConfig> SystemConfigList)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                SaveSystemConfig(SystemConfigList);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("ExamConfig");
        }


        private void SaveSystemConfig(List<SystemConfig> SystemConfigList)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                foreach (var config in SystemConfigList)
                {
                    var temp = SysConfig.GetSystemConfigById(config.Id);
                    temp.ParamValue = config.ParamValue;
                    SysConfig.UpdateSystemParam(temp);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
    }
}
