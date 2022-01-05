using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers
{
    public class HomeController : Controller
    {
        IAttendanceRepository attendanceRepo;
        IStaffRepository staffRepo;
        IFeePlanRepository feePlanRepo;
        IStudentRepository studentRepo;
        public HomeController()
        {
            attendanceRepo = new AttendanceRepositoryImp(new SC_WEBEntities2());
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public JsonResult GetStaffPresence()
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            return Json(staffRepo.GetAttendanceStats(branchId, DateTime.UtcNow), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentPresence()
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            return Json(attendanceRepo.GetAttendanceStats(branchId, DateTime.UtcNow), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentCurrentYearAdmissionData()
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            DateTime dateLastYear = DateTime.Now.AddYears(-1);
            return Json(studentRepo.GetStudentAdmissionData(dateLastYear, DateTime.Now, branchId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentTodayAdmissionData()
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime toDate = fromDate.AddDays(1);
            return Json(studentRepo.GetStudentAdmissionData(fromDate, toDate, branchId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFeeDetails(string from = null, string to = null, string view = "month")
        {
            DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
            if (from != null && from.Length > 24)
                fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            DateTime toDate = DateTime.Now;
            if (to != null && to.Length > 24)
            {
                toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            return Json(feePlanRepo.GetMonthlyFeeStats(branchId, fromDate, toDate, view), JsonRequestBehavior.AllowGet);
        }
    }
}
