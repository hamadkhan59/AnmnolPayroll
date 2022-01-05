using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using System.Collections;
using SMS_DAL.ViewModel;
using SMS_Web.Helpers.PdfHelper;
using PdfSharp.Pdf;
using System.IO;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository;
using SMS_Web.Filters;
using SMS_Web.Controllers.SecurityAssurance;
using CrystalDecisions.CrystalReports.Engine;
using System.Globalization;
using Logger;
using System.Reflection;
using SMS_DAL.Reports;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffSalaryController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        private static int errorCode = 0, sheetErrorCode = 0, salaryDetailErrorCode = 0, staffSalaryErrorCode = 0;
        static List<StaffAllownce> staffAllownces = null;
        IStaffRepository staffRepo;

        private IFinanceAccountRepository accountRepo;
        private ISecurityRepository secRepo;
        IFeePlanRepository feePlanRepo;
        public StaffSalaryController()
        {

            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); ;
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
        }

        //
        // GET: /StaffSalary/

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_PAID_SALARIES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                ViewBag.PaymentMethod = new SelectList(staffRepo.GetAllPaymentMethods(), "Id", "Method");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                var staffNames = SessionHelper.GetStaffNames(Session.SessionID);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;
                errorCode = 0;
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SALARY_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SALARY_SEARCH_FLAG] = false;
                    var slaryList = SearchStaffSalary(branchId);
                    return View(slaryList);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View("");
        }

        public ActionResult SingleStaffSalary()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_PAY_SALARIES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.PaymentMethod = new SelectList(staffRepo.GetAllPaymentMethods(), "Id", "Method");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
                ViewBag.StaffNames = SessionHelper.GetStaffNames(Session.SessionID);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["branchId"] = branchId;
                ViewData["Error"] = errorCode;
                errorCode = 0;

                if (Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_FLAG] != null && (bool)Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_FLAG] = false;
                    var staffSalary = SearchSignleStaffSalary(branchId);
                    ViewData["history"] = staffRepo.GetLastSixMonthSalaries((int)staffSalary.StaffId, branchId);
                    return View(staffSalary);
                }
               
            }
            catch (Exception ex)
            {
                ViewData["Error"] = errorCode;
                errorCode = 0;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View("");
        }

        public JsonResult GetSalaryStatsByMonth(string from = null, string to = null)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(staffRepo.GetSalaryStatsByMonth(branchId, fromDate, toDate), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStaffSalary(int? CatagoryId, int? DesignationId, int? StaffId, string YearId, string MonthId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID] = Session[ConstHelper.STAFF_SALARY_SEARCH_DESIGNATION_ID] = Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_ID] = 0;
                if (CatagoryId != null)
                    Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                if (DesignationId != null)
                    Session[ConstHelper.STAFF_SALARY_SEARCH_DESIGNATION_ID] = (int)DesignationId;
                if (StaffId != null)
                    Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_ID] = (int)StaffId;
                Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_YEAR] = YearId;
                Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_MONTH] = MonthId;
                Session[ConstHelper.STAFF_SALARY_SEARCH_FLAG] = true;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSingleStaffSalary(string StaffId, string YearId, string MonthId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                int staffId = int.Parse(StaffId.Split('|')[0]);
                Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_STAFF_ID] = staffId;
                Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_STAFF_YEAR] = YearId;
                Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_STAFF_MONTH] = MonthId;
                Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_FLAG] = true;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SingleStaffSalary");
        }

        public List<StaffSalary> SearchStaffSalary(int branchId)
        {
            List<StaffSalary> salaryList = new List<StaffSalary>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];
                int searchDesginationId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_DESIGNATION_ID];
                int searchStaffId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_ID];
                string searchYear = (string)Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_YEAR];
                string searchMonth = (string)Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_MONTH];
                int policyCount = 0;
                string forMonth = SessionHelper.GetMonthName(int.Parse(searchMonth)) + "-" + (2016 + int.Parse(searchYear) - 1);
                int monthDays = DateTime.DaysInMonth(2016 + int.Parse(searchYear) - 1, int.Parse(searchMonth));
                DateTime fromDate = new DateTime(2016 + int.Parse(searchYear) - 1, int.Parse(searchMonth), 1, 0, 0, 0);
                DateTime toDate = new DateTime(2016 + int.Parse(searchYear) - 1, int.Parse(searchMonth), monthDays, 0, 0, 0);
                Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE] = fromDate;

                salaryList = staffRepo.SearchSalaries(searchCatagoryId, searchDesginationId, searchStaffId, forMonth, branchId);
                //if (salaryList == null || salaryList.Count == 0)
                //{
                //    //IList attendanceDetailList = GetStaffAttandanceDetail(searchStaffId, searchDesginationId, searchCatagoryId, fromDate, toDate);
                //    IList attendanceDetailList = GetStaffAttandanceDetail(0, 0, 0, fromDate, toDate, branchId);
                //    if (attendanceDetailList != null && attendanceDetailList.Count > 0)
                //    {
                //        int index = 0;
                //        foreach (QueryResult resul in attendanceDetailList)
                //        {

                //            int lateIns = Convert.ToInt32(resul.Latein);
                //            int earlyOut = Convert.ToInt32(resul.EarlyOut);
                //            int halfDays = Convert.ToInt32(resul.Halfdays);
                //            int absents = Convert.ToInt32(resul.Absents);
                //            int presents = Convert.ToInt32(resul.Presents);
                //            int salary = Convert.ToInt32(resul.Salary) + Convert.ToInt32(resul.allownces);
                //            int designationId = Convert.ToInt32(resul.DesignationId);
                //            StaffAttandancePolicy policy = staffRepo.GetStaffAttandancePolicyByDesignationId(designationId, branchId);
                //            if (policy != null)
                //            {
                //                int deduction = GetSalaryDeduction(0, (int)policy.LeaveInMonth, (int)policy.LateInCount, lateIns + earlyOut, halfDays, absents, salary, (int)policy.LeaveInYear, Convert.ToInt32(resul.StaffId));
                //                int grossSalary = salary - deduction;
                //                StaffSalary salaryObj = new StaffSalary();
                //                salaryObj.ForMonth = forMonth;
                //                salaryObj.StaffId = Convert.ToInt32(resul.StaffId);
                //                salaryObj.SalaryAmount = grossSalary;
                //                salaryObj.LateIns = lateIns;
                //                salaryObj.EarlyOuts = earlyOut;
                //                salaryObj.HalfDays = halfDays;
                //                salaryObj.Presents = presents;
                //                salaryObj.Absents = absents;
                //                salaryObj.AttendanceDeduction = deduction;
                //                staffRepo.AddStaffSalary(salaryObj);
                //                SmsInfoProxy.sendSmsStaffSalaryEvent(salaryObj);
                //                index++;
                //                policyCount++;
                //            }
                //        }

                //        if (policyCount == 0)
                //        {
                //            errorCode = 310;
                //        }
                //    }
                //    else
                //    {
                //        errorCode = 1340;
                //    }
                //    //List<StaffAttandance>
                //    salaryList = staffRepo.SearchSalaries(searchCatagoryId, searchDesginationId, searchStaffId, forMonth, branchId);
                //}
                LogWriter.WriteLog("Search Staff Salary Count : " + (salaryList == null ? 0 : salaryList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return salaryList;
        }

        public StaffSalary SearchSignleStaffSalary(int branchId)
        {
            StaffSalary staffSalary = new StaffSalary();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = 0;
                int searchDesginationId = 0;
                int searchStaffId = (int)Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_STAFF_ID];
                string searchYear = (string)Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_STAFF_YEAR];
                string searchMonth = (string)Session[ConstHelper.SINGLE_STAFF_SALARY_SEARCH_STAFF_MONTH];
                int policyCount = 0;
                string forMonth = SessionHelper.GetMonthName(int.Parse(searchMonth)) + "-" + (2016 + int.Parse(searchYear) - 1);
                int monthDays = DateTime.DaysInMonth(2016 + int.Parse(searchYear) - 1, int.Parse(searchMonth));
                DateTime fromDate = new DateTime(2016 + int.Parse(searchYear) - 1, int.Parse(searchMonth), 1, 0, 0, 0);
                DateTime toDate = new DateTime(2016 + int.Parse(searchYear) - 1, int.Parse(searchMonth), monthDays, 0, 0, 0);
                Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE] = fromDate;

                var salaryList = staffRepo.SearchSalaries(searchCatagoryId, searchDesginationId, searchStaffId, forMonth, branchId, true);
                if (salaryList == null || salaryList.Count == 0)
                {
                    //IList attendanceDetailList = GetStaffAttandanceDetail(searchStaffId, searchDesginationId, searchCatagoryId, fromDate, toDate);
                    IList attendanceDetailList = GetStaffAttandanceDetail(searchStaffId, 0, 0, fromDate, toDate, branchId);
                    if (attendanceDetailList != null && attendanceDetailList.Count > 0)
                    {
                        int index = 0;
                        foreach (QueryResult resul in attendanceDetailList)
                        {

                            int lateIns = Convert.ToInt32(resul.Latein);
                            int earlyOut = Convert.ToInt32(resul.EarlyOut);
                            int halfDays = Convert.ToInt32(resul.Halfdays);
                            int absents = Convert.ToInt32(resul.Absents);
                            int presents = Convert.ToInt32(resul.Presents);
                            int salary = Convert.ToInt32(resul.Salary) + Convert.ToInt32(resul.allownces);
                            int designationId = Convert.ToInt32(resul.DesignationId);
                            Staff staff = staffRepo.GetStaffById(searchStaffId);
                            staff.Advance = staff.Advance == null ? 0 : staff.Advance;
                            staff.AdvanceInstallment = staff.AdvanceInstallment == null ? 0 : staff.AdvanceInstallment;
                            int advanceAmount =  staff.Advance > staff.AdvanceInstallment ? (int) staff.AdvanceInstallment : (int) staff.Advance ;
                            StaffAttandancePolicy policy = staffRepo.GetStaffAttandancePolicyByDesignationId(designationId, branchId);
                            if (policy != null)
                            {
                                DataSet ds = (new DAL_Staff_Reports()).GetAttendanceDetail(searchStaffId, fromDate, toDate);
                                double totalHrs = 0;
                                double workingHrs = 0;
                                double shortHrs = 0;
                                int bonusdaysFourHours = 0;
                                int bonusdaysSixHours = 0;
                                int bonusFourHours = 0;
                                int bonusSixHours = 0;
                                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 
                                    && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    totalHrs = double.Parse(ds.Tables[0].Rows[0]["TotalHours"].ToString());
                                    workingHrs = double.Parse(ds.Tables[0].Rows[0]["WorkingHours"].ToString());
                                    shortHrs = double.Parse(ds.Tables[0].Rows[0]["ShortHours"].ToString());
                                    bonusdaysFourHours = int.Parse(ds.Tables[0].Rows[0]["BonusDaysFourHrs"].ToString());
                                    bonusdaysSixHours = int.Parse(ds.Tables[0].Rows[0]["BonusDaysSixHrs"].ToString());
                                    bonusFourHours = int.Parse(ds.Tables[0].Rows[0]["BonusFourHours"].ToString());
                                    bonusSixHours = int.Parse(ds.Tables[0].Rows[0]["BonusSixHours"].ToString());
                                }
                                int foodBonuse = (bonusdaysFourHours * bonusFourHours) + (bonusdaysSixHours * bonusSixHours);
                                //int deduction = GetSalaryDeductionOnHourlyBasis(totalHrs, shortHrs, salary);
                                int deduction = 0;
                                int grossSalary = (int) ((salary / totalHrs) * workingHrs);
                                StaffSalary salaryObj = new StaffSalary();
                                salaryObj.ForMonth = forMonth;
                                salaryObj.StaffId = Convert.ToInt32(resul.StaffId);
                                salaryObj.SalaryAmount = grossSalary;
                                salaryObj.LateIns = lateIns;
                                salaryObj.EarlyOuts = earlyOut;
                                salaryObj.HalfDays = halfDays;
                                salaryObj.Presents = presents;
                                salaryObj.Absents = absents;
                                salaryObj.AttendanceDeduction = deduction;
                                salaryObj.ClubbedSundays = GetClubbedSundays(fromDate, toDate, searchStaffId, branchId);
                                salaryObj.SundaysDeduction = 0;
                                salaryObj.Deduction = deduction;
                                salaryObj.AdvanceAdjustment = advanceAmount;
                                salaryObj.Bonus = foodBonuse;
                                salaryObj.TotalHours = (decimal)totalHrs;
                                salaryObj.WorkingHours = (decimal)workingHrs;
                                salaryObj.ShortHours = (decimal)shortHrs;
                                salaryObj.BonusDays = bonusdaysFourHours + bonusdaysSixHours;
                                salaryObj.MiscWithdraw = staffRepo.GetStaffMiscWithdraws(forMonth, (int)salaryObj.StaffId);
                                staffRepo.AddStaffSalary(salaryObj);
                                SmsInfoProxy.sendSmsStaffSalaryEvent(salaryObj);
                                index++;
                                policyCount++;
                            }
                        }

                        if (policyCount == 0)
                        {
                            errorCode = 310;
                        }
                    }
                    else
                    {
                        errorCode = 1340;
                    }
                   
                    //List<StaffAttandance>
                    salaryList = staffRepo.SearchSalaries(searchCatagoryId, searchDesginationId, searchStaffId, forMonth, branchId);
                }
                staffSalary = salaryList[0];
                LogWriter.WriteLog("Search Staff Salary Count : " + (salaryList == null ? 0 : salaryList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 1420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return staffSalary;
        }

        private int GetClubbedSundays(DateTime fromDate, DateTime toDate, int staffId, int branchId)
        {
            int sundayCount = 0;
            var attendanceList = staffRepo.SearchStaffAttendnaceModel(fromDate, toDate, staffId);
            attendanceList = attendanceList.OrderBy(x => x.Date).ToList();
            LogWriter.WriteLog("Attendance Count : " + (attendanceList == null ? 0 : attendanceList.Count));
            for (int i = 0; i < attendanceList.Count; i++)
            {
                LogWriter.WriteLog("Attendance of the day : " + i);
                if (attendanceList[i].Date.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (i - 2 >= 0 && i - 1 >= 0 &&
                        attendanceList[i - 2].Date.Value.DayOfWeek == DayOfWeek.Friday
                        && attendanceList[i - 1].Date.Value.DayOfWeek == DayOfWeek.Saturday
                        && attendanceList[i - 2].Status == 2 && attendanceList[i - 1].Status == 2
                        && attendanceList[i].Status == 2)
                    {
                        var fridayHoliday = staffRepo.GetStaffHolidayByDate((DateTime)attendanceList[i - 2].Date, branchId);
                        var saturdayHoliday = staffRepo.GetStaffHolidayByDate((DateTime)attendanceList[i - 1].Date, branchId);
                        if(fridayHoliday == null && saturdayHoliday == null)
                            sundayCount++;
                    }
                    else if (i - 1 >= 0 && i + 1 < attendanceList.Count &&
                        attendanceList[i - 1].Date.Value.DayOfWeek == DayOfWeek.Saturday
                        && attendanceList[i + 1].Date.Value.DayOfWeek == DayOfWeek.Monday
                        && attendanceList[i - 1].Status == 2 && attendanceList[i].Status == 2
                        && attendanceList[i + 1].Status == 2)
                    {
                        var mondayHoliday = staffRepo.GetStaffHolidayByDate((DateTime)attendanceList[i + 1].Date, branchId);
                        var saturdayHoliday = staffRepo.GetStaffHolidayByDate((DateTime)attendanceList[i - 1].Date, branchId);
                        if (mondayHoliday == null && saturdayHoliday == null)
                            sundayCount++;
                    }
                    else if (i + 1 < attendanceList.Count && i + 2 < attendanceList.Count &&
                        attendanceList[i + 2].Date.Value.DayOfWeek == DayOfWeek.Tuesday
                        && attendanceList[i + 1].Date.Value.DayOfWeek == DayOfWeek.Monday
                        && attendanceList[i].Status == 2 && attendanceList[i + 1].Status == 2
                        && attendanceList[i + 2].Status == 2)
                    {
                        var mondayHoliday = staffRepo.GetStaffHolidayByDate((DateTime)attendanceList[i + 1].Date, branchId);
                        var tuesdayHoliday = staffRepo.GetStaffHolidayByDate((DateTime)attendanceList[i + 2].Date, branchId);
                        if (mondayHoliday == null && tuesdayHoliday == null)
                            sundayCount++;
                    }
                }
            }

            return sundayCount;
        }

        private int GetSalaryDeduction(int deduction, int allowleaveCount, int lateInSattlement, int lateIns, int halfDays, int absents, int salary, int yearlyLeave, int staffId)
        {
            int salaryDeduction = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                double leaveCount = 0;
                leaveCount += lateIns / lateInSattlement;
                leaveCount += ((double)halfDays) / 2;
                leaveCount += absents;

                deduction = salary / 30;

                if (yearlyLeave == 0)
                {
                    leaveCount -= allowleaveCount;
                    if (leaveCount > 0)
                    {
                        salaryDeduction = (int)(deduction * leaveCount);
                    }
                }
                else
                {
                    Staff staff = staffRepo.GetStaffById(staffId);
                    if (staff.YearlyLeaves == null)
                    {
                        staff.YearlyLeaves = yearlyLeave;
                    }
                    int leaveDeduction = 0;

                    if (leaveCount >= 1)
                    {
                        if (staff.YearlyLeaves > 0)
                        {
                            staff.YearlyLeaves -= (int)leaveCount;
                            if (staff.YearlyLeaves < 0)
                                leaveDeduction = (int)staff.YearlyLeaves;
                        }
                        else
                        {
                            staff.YearlyLeaves -= (int)leaveCount;
                            leaveDeduction = (int)leaveCount;
                        }
                    }

                    salaryDeduction = (int)(deduction * leaveDeduction);
                    staffRepo.UpdateStaff(staff);

                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return salaryDeduction;
        }

        private int GetSalaryDeductionOnHourlyBasis(double totalHrs, double ShortHrs, int salary)
        {
            double salaryDeduction = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                double perHrSalary = salary / totalHrs;
                salaryDeduction = ShortHrs * perHrSalary;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return (int)salaryDeduction;
        }

        public IList GetStaffAttandanceDetail(int staffId, int designationId, int catagoryId, DateTime fromDate, DateTime toDate, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var sql = @"select st.StaffId, st.Name, st.FatherName,
                                count(case when sta.time is null then null when sta.Time > stp.LateInTime  
                                and sta.Time <= stp.HalfDayTime then 1 else null end) as Latein,
                                count (case when sta.time is null then null when sta.Time  >= stp.HalfDayTime  then 1 else null end) as Halfdays,
								count(case when sta.OutTime is null then null when sta.OutTime < stp.EarlyOutTime then 1 else null end) as EarlyOut,
                                count (case when sta.Status = 1 then 1 else null end ) as Presents,
                                count (case when sta.Status = 2 then 1 else null end ) as Absents,  isNUll(st.Salary, 0) as Salary,
                                isnull((select sum(Amount) from StaffAllownce where StaffId = st.StaffId),0) as allownces, st.DesignationId , desgn.Name as Designation
                                from staff st Left outer join StaffAttandancePolicy as stp on 
                                st.DesignationId = stp.DesignationId 
                                Left outer join StaffAttandance as sta on st.StaffId = sta.StaffId 
								Left outer join Designation desgn on desgn.Id = st.DesignationId
                     where ( 0 = {2} or  st.DesignationId = {2}) and ( 0 = {3} or desgn.CatagoryId = {3})  
					  and ( 0 = {4} or  st.StaffId = {4}) and sta.Date >= '{0}' and sta.Date <= '{1}' 
                        and st.BranchId = {5}
                             group by desgn.Name, st.Name, st.StaffId, st.FatherName, st.Salary, allownces, st.DesignationId   order by desgn.Name, st.StaffId Asc ";
                sql = string.Format(sql, GetDate(fromDate.Date), GetDate(toDate.Date), designationId, catagoryId, staffId, branchId);
                var results = staffRepo.SearchAttendanceDetail(sql);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return (IList)results;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return null;
        }

        public string GetDate(DateTime date)
        {
            return date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveStaffSalary(int[] UnpaidIds, int[] StaffId, int[] PaidAmount, int[] Bonus, int[] AdvanceAdjustment, int[] Deduction, DateTime PaidDate, string PaymentMethodId, string ChequeNO, int[] Presents, int[] Absents, int[] HalfDays, int[] LateIN, int[] EarlyOut, int[] Indexes, int[] SalaryAmount, int IsPrint, int[] AttendanceDeduction, int IsUnpaid = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Save Staff Salary Count : " + (StaffId == null ? 0 : StaffId.Count()));
                if (IsPrint == 1)
                {
                    SalarySlipPdf pdf = new SalarySlipPdf();
                    PdfDocument document = new PdfDocument();
                    DateTime searchFromDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE];

                    LogWriter.WriteLog("Printing the staff Salary Sheet");
                    document = pdf.CreateSheetPdf(StaffId, searchFromDate.Date.Month.ToString(), searchFromDate.Date.Year.ToString(), Indexes, LateIN, HalfDays, Absents, PaidAmount, Bonus, AttendanceDeduction, AdvanceAdjustment, Deduction, SalaryAmount, EarlyOut);

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf");
                }
                else
                {
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    string searchYear = (string)Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_YEAR];
                    string searchMonth = (string)Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_MONTH];
                    string forMonth = (2016 + int.Parse(searchYear) - 1).ToString() + "-" + searchMonth;
                    if (IsUnpaid == 1)
                    {
                        LogWriter.WriteLog("Unpaying the staff salary");
                        for (int i = 0; i < UnpaidIds.Count(); i++)
                        {
                            int index = UnpaidIds[i];
                            if (PaidAmount[index] > 0)
                            {
                                int staffIdTemp = StaffId[index];
                                var salaryObj = staffRepo.SearchStaffSalaries(staffIdTemp, forMonth);
                                CreateUnpaidJournalEntry(salaryObj, PaymentMethodId, PaidAmount[index], branchId, ChequeNO);
                                salaryObj.PaidAmount = 0;
                                salaryObj.AdvanceAdjustment = 0;
                                salaryObj.Deduction = 0;
                                salaryObj.Bonus = 0;
                                salaryObj.PaidDate = null;
                                salaryObj.FinanceAccountId = int.Parse(PaymentMethodId);
                                staffRepo.UpdateStaffSalary(salaryObj);
                                SmsInfoProxy.sendSmsStaffSalaryEvent(salaryObj);
                            }
                        }
                        errorCode = 3;
                    }
                    else
                    {
                        LogWriter.WriteLog("Paying the staff salary");
                        for (int i = 0; i < UnpaidIds.Count(); i++)
                        {
                            int index = UnpaidIds[i];
                            if (PaidAmount[index] > 0)
                            {
                                int staffIdTemp = StaffId[index];
                                var salaryObj = staffRepo.SearchStaffSalaries(staffIdTemp, forMonth);
                                salaryObj.PaidAmount = PaidAmount[index];
                                salaryObj.AdvanceAdjustment = AdvanceAdjustment[index];
                                salaryObj.Deduction = Deduction[index];
                                salaryObj.Bonus = Bonus[index];
                                salaryObj.PaidDate = PaidDate;
                                salaryObj.FinanceAccountId = int.Parse(PaymentMethodId);
                                salaryObj.FinanceFifthLvlAccount = null;
                                staffRepo.UpdateStaffSalary(salaryObj);
                                SmsInfoProxy.sendSmsStaffSalaryEvent(salaryObj);
                                CreateJournalEntry(salaryObj, PaymentMethodId, branchId, ChequeNO);
                            }
                        }
                        errorCode = 2;
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
            //return null;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSingleStaffSalary(int StaffId, string ForMonth, int SundaysDeduction, int Deduction, 
            int AdvanceAdjustment, int Bonus, int PaidAmount, int IsUnpaid, int FinanceAccountId, DateTime PaidDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Saving Staff Salary for StaffId : " + StaffId);
                
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (IsUnpaid == 1)
                {
                    LogWriter.WriteLog("Unpaying the staff salary");
                    
                    var salaryObj = staffRepo.SearchStaffSalaries(StaffId, ForMonth);
                    CreateUnpaidJournalEntrySingleStaff(salaryObj, FinanceAccountId.ToString(), PaidAmount, branchId, "");
                    staffRepo.DeleteStaffSalary(salaryObj.SalaryId);
                    //SmsInfoProxy.sendSmsStaffSalaryEvent(salaryObj);
                    errorCode = 3;
                }
                else
                {
                    LogWriter.WriteLog("Paying the staff salary");
                   
                    var salaryObj = staffRepo.SearchStaffSalaries(StaffId, ForMonth);
                    salaryObj.PaidAmount = PaidAmount;
                    salaryObj.AdvanceAdjustment = AdvanceAdjustment;
                    salaryObj.Deduction = Deduction;
                    salaryObj.Bonus = Bonus;
                    salaryObj.PaidDate = PaidDate;
                    salaryObj.FinanceAccountId = FinanceAccountId;
                    salaryObj.SundaysDeduction = SundaysDeduction;
                    salaryObj.FinanceFifthLvlAccount = null;
                    staffRepo.UpdateStaffSalary(salaryObj);
                    //SmsInfoProxy.sendSmsStaffSalaryEvent(salaryObj);
                    CreateJournalEntrySingleStaff(salaryObj, FinanceAccountId.ToString(), branchId, "");

                    Staff staff = staffRepo.GetStaffById(StaffId);
                    staff.Advance -= AdvanceAdjustment;
                    staffRepo.UpdateStaff(staff);
                    (new DAL_Staff_Reports()).ResetStaffApprovalData(StaffId);
                    errorCode = 2;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            //return null;
            return RedirectToAction("SingleStaffSalary");
        }

        public ActionResult SalarySheet()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_SALARY_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<SalarySheetViewModel> model = new List<SalarySheetViewModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] != null && (bool)Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] = false;
                    model = SearchSalarySheet(branchId);
                }

                ViewData["Error"] = sheetErrorCode;
                sheetErrorCode = 0;
                Session[ConstHelper.SALARY_SHEET_LIST] = model;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(model);
        }


        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSalarySheet(int? Id, int? DesignationId, int? StaffId, DateTime FromDate, DateTime ToDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Session[ConstHelper.SALARY_SHEET_SEARCH_CATEGORY_ID] = Session[ConstHelper.SALARY_SHEET_SEARCH_DESIGNATION_ID] = Session[ConstHelper.SALARY_SHEET_SEARCH_STAFF_ID] = 0;
                if (Id != null)
                    Session[ConstHelper.SALARY_SHEET_SEARCH_CATEGORY_ID] = (int)Id;
                if (DesignationId != null)
                    Session[ConstHelper.SALARY_SHEET_SEARCH_DESIGNATION_ID] = (int)DesignationId;
                if (StaffId != null)
                    Session[ConstHelper.SALARY_SHEET_SEARCH_STAFF_ID] = (int)StaffId;
                Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE] = FromDate;
                Session[ConstHelper.SALARY_SHEET_SEARCH_TO_DATE] = ToDate;
                Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] = true;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SalarySheet");
        }

        public List<SalarySheetViewModel> SearchSalarySheet(int branchId)
        {
            List<SalarySheetViewModel> salarySheetList = new List<SalarySheetViewModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchSheetCatagoryId = 0;
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID] != null)
                    searchSheetCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];
                int searchSheetDesginationId = 0;
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_DESIGNATION_ID] != null)
                    searchSheetCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];
                int searchSheetStaffId = 0;
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_ID] != null)
                    searchSheetCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];

                DateTime searchFromDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE];
                DateTime searchToDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_TO_DATE];

                IList attendanceDetailList = GetStaffAttandanceDetail(searchSheetStaffId, searchSheetDesginationId,
                    searchSheetCatagoryId, searchFromDate, searchToDate, branchId);
                int passCount = 0;
                if (attendanceDetailList != null && attendanceDetailList.Count > 0)
                {
                    foreach (QueryResult resul in attendanceDetailList)
                    {

                        int lateIns = Convert.ToInt32(resul.Latein);
                        int earlyOut = Convert.ToInt32(resul.EarlyOut);
                        int halfDays = Convert.ToInt32(resul.Halfdays);
                        int absents = Convert.ToInt32(resul.Absents);
                        int presents = Convert.ToInt32(resul.Presents);
                        int salary = Convert.ToInt32(resul.Salary) + Convert.ToInt32(resul.allownces);
                        int designationId = Convert.ToInt32(resul.DesignationId);
                        StaffAttandancePolicy policy = staffRepo.GetStaffAttandancePolicyByDesignationId(designationId, branchId);
                        int deduction = 0, grossSalary = 0;
                        if (policy != null)
                        {
                            deduction = GetSalaryDeduction(0, (int)policy.LeaveInMonth, (int)policy.LateInCount, lateIns + earlyOut, halfDays, absents, salary, (int)policy.LeaveInYear, Convert.ToInt32(resul.StaffId));
                            grossSalary = salary - deduction;
                            passCount++;
                        }
                        SalarySheetViewModel ssvm = new SalarySheetViewModel();
                        ssvm.StaffId = resul.StaffId;
                        ssvm.Name = resul.Name;
                        ssvm.FatherName = resul.FatherName;
                        ssvm.LateIN = lateIns;
                        ssvm.EarlyOut = earlyOut;
                        ssvm.HalfDays = halfDays;
                        ssvm.Presents = presents;
                        ssvm.Designation = resul.Designation;
                        ssvm.BasicSalary = Convert.ToInt32(resul.Salary);
                        ssvm.Absents = absents;
                        ssvm.Allownces = Convert.ToInt32(resul.allownces);
                        ssvm.GrossSalary = grossSalary;
                        ssvm.Deduction = deduction;
                        salarySheetList.Add(ssvm);
                    }

                    if (passCount == 0)
                    {
                        sheetErrorCode = 310;
                        return new List<SalarySheetViewModel>();
                    }
                }
                else
                {
                    sheetErrorCode = 1340;
                }
                LogWriter.WriteLog("Search Salary Sheet List Count : " + (salarySheetList == null ? 0 : salarySheetList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return salarySheetList;
        }

        private ReportDocument GetReport(List<SalarySheetViewModel> staffList, DateTime fromDate, DateTime toDate, int branchId)
        {
            ReportDocument rd = new ReportDocument();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DataSet ds = GetStaffSalarySheetDataSet(staffList, branchId);

                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffSalarySheet.rpt"));
                rd.SetDataSource(ds.Tables[0]);

                rd.SetParameterValue("FromDate", fromDate.ToString());
                rd.SetParameterValue("ToDate", toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return rd;
        }

        private DataSet GetStaffSalarySheetDataSet(List<SalarySheetViewModel> staffList,  int branchId)
        {
            DataSet ds = new DataSet();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Staff Salary Sheet Data Set Count : " + (staffList == null ? 0 : staffList.Count));
                DataTable table = new DataTable();
                table.Columns.Add("StaffId");
                table.Columns.Add("Name");
                table.Columns.Add("FatherName");
                table.Columns.Add("Designation");
                table.Columns.Add("LateIns");
                table.Columns.Add("EarlyOut");
                table.Columns.Add("HalfDays");
                table.Columns.Add("Absents");
                table.Columns.Add("BasicSalary");
                table.Columns.Add("Allownces");
                table.Columns.Add("GrossSalary");
                table.Columns.Add("Deduction");
                table.Columns.Add("Presents");

                int i = 0;
                foreach (var staff in staffList)
                {
                    table.Rows.Add();
                    table.Rows[i]["StaffId"] = staff.StaffId;
                    table.Rows[i]["Name"] = staff.Name;
                    table.Rows[i]["FatherName"] = staff.FatherName;
                    table.Rows[i]["Designation"] = staff.Designation;
                    table.Rows[i]["LateIns"] = staff.LateIN;
                    table.Rows[i]["EarlyOut"] = staff.EarlyOut;
                    table.Rows[i]["HalfDays"] = staff.HalfDays;
                    table.Rows[i]["Absents"] = staff.Absents;
                    table.Rows[i]["BasicSalary"] = staff.BasicSalary;
                    table.Rows[i]["Allownces"] = staff.Allownces;
                    table.Rows[i]["GrossSalary"] = staff.GrossSalary;
                    table.Rows[i]["Deduction"] = staff.Deduction;
                    table.Rows[i]["Presents"] = staff.Presents;
                    i++;
                }

                ds.Tables.Add(table);
                AddImage(ds, branchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return ds;
        }

        private void AddImage(DataSet ds, int branchId)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                ds.Tables[0].Rows.Add();
            }

            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
            DataColumn colByteArray = new DataColumn("IMAGE");
            colByteArray.DataType = System.Type.GetType("System.Byte[]");
            ds.Tables[0].Columns.Add(colByteArray);
            ds.Tables[0].Rows[0]["IMAGE"] = config.SchoolLogo;
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSalarySlip()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<SalarySheetViewModel> model = new List<SalarySheetViewModel>();
                model = (List<SalarySheetViewModel>)Session[ConstHelper.SALARY_SHEET_LIST];
                DateTime fromDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE];
                DateTime toDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_TO_DATE];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ReportDocument rd = GetReport(model, fromDate, toDate, branchId);

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                var contentLength = stream.Length;
                Response.AppendHeader("Content-Length", contentLength.ToString());
                Response.AppendHeader("Content-Disposition", "inline; filename=StaffSalarySheet.pdf");

                stream.Seek(0, SeekOrigin.Begin);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return File(stream, "application/pdf");
                //return File(stream, "application/pdf", model.reportName + ".pdf");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }

        }

        public ActionResult CreateSalarySlip1(int[] StaffIds, int[] Indexes, int[] LateIN, int[] HalfDays, int[] Absents, int[] BasicSalary, int[] Allownces, int[] Deduction, int[] GrossSalary, int[] AdvancsAdjustment, int[] SecDeductions)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Create Salary Slip Count : " + (StaffIds == null ? 0 : StaffIds.Count()));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                SalarySlipPdf pdf = new SalarySlipPdf();
                PdfDocument document = new PdfDocument();
                DateTime searchFromDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE];

                document = pdf.CreatePdf(StaffIds, searchFromDate.Date.Month.ToString(), searchFromDate.Date.Year.ToString(), Indexes, LateIN, HalfDays, Absents, BasicSalary, Allownces, Deduction, GrossSalary, AdvancsAdjustment, SecDeductions, branchId);

                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                stream.Seek(0, SeekOrigin.Begin);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Staff Salary Sheets : "+ DateTime.Now.ToString() + ".pdf" };
            //}  
        }

        public ActionResult SalaryIncrement()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_SALARY_INCREMENT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                ViewData["Error"] = sheetErrorCode;
                sheetErrorCode = 0;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_FLAG] = false;
                    return View(SearchSalaryDetail(branchId));
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<SalarySheetViewModel>());
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult SalaryDetails(int? CatagoryId, int? DesignationId, int? StaffId, string Name, string FatherName)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (CatagoryId == null)
                    CatagoryId = 0;
                if (DesignationId == null)
                    DesignationId = 0;
                if (StaffId == null)
                    StaffId = 0;

                if (Name == null)
                    Name = "";
                if (FatherName == null)
                    FatherName = "";

                Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_DESIGNATION_ID] = (int)DesignationId;
                Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_STAFF_ID] = (int)StaffId;
                Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_NAME] = (string)Name;
                Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_FATHER_NAME] = (string)FatherName;

                Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_FLAG] = true;
                errorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SalaryIncrement");
        }

        public List<SalarySheetViewModel> SearchSalaryDetail(int branchId)
        {
            List<SalarySheetViewModel> salarySheetList = new List<SalarySheetViewModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int salaryDetailCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_CATEGORY_ID];
                int salaryDetailDesignationId = (int)Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_DESIGNATION_ID];
                int salaryDetailStaffId = (int)Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_STAFF_ID];
                string salaryDetailStaffName = (string)Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_NAME];
                string salaryDetailStaffFatherName = (string)Session[ConstHelper.STAFF_SALARY_INCREMENT_SEARCH_FATHER_NAME];

                IList salaryDetailList = GetStaffSalaryDetail(salaryDetailStaffId, salaryDetailDesignationId, salaryDetailCatagoryId, salaryDetailStaffName, salaryDetailStaffFatherName, branchId);
                if (salaryDetailList != null && salaryDetailList.Count > 0)
                {
                    foreach (QueryResult resul in salaryDetailList)
                    {
                        SalarySheetViewModel ssvm = new SalarySheetViewModel();
                        ssvm.StaffId = resul.StaffId;
                        ssvm.Name = resul.Name;
                        ssvm.FatherName = resul.FatherName;
                        ssvm.BasicSalary = Convert.ToInt32(resul.Salary);
                        ssvm.Allownces = Convert.ToInt32(resul.allownces);
                        salarySheetList.Add(ssvm);
                    }
                }
                LogWriter.WriteLog("Search Salary Detail Count : " + (salarySheetList == null ? 0 : salarySheetList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return salarySheetList;
        }

        public IList GetStaffSalaryDetail(int staffId, int designationId, int catagoryId, string staffName, string staffFatherName, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var sql = @"select st.StaffId, st.Name, st.FatherName, isNUll(st.Salary, 0) as Salary,
                                isnull((select sum(Amount) from StaffAllownce where StaffId = st.StaffId),0) as allownces, st.DesignationId 
                                from staff st Left outer join StaffAttandancePolicy as stp on 
                                st.DesignationId = stp.DesignationId 
								Left outer join Designation desgn on desgn.Id = st.DesignationId
								where ( 0 = {2} or  st.DesignationId = {2}) and ( 0 = {3} or desgn.CatagoryId = {3})  
								and ( 0 = {4} or  st.StaffId = {4}) and st.Name like '%{0}%' and st.FatherName like '%{1}%' 
                                and st.BranchId = {5}
								group by st.Name, st.StaffId, st.FatherName, st.Salary, allownces, st.DesignationId   order by st.StaffId Asc";
                sql = string.Format(sql, staffName, staffFatherName, designationId, catagoryId, staffId, branchId);
                var results = staffRepo.SearchAttendanceDetail(sql);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return (IList)results;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return null;
        }

        [HttpGet]

        public void setIncrementHistory()
        {
            Session[ConstHelper.STAFF_SALARY_INCREMENT_FLAG] = true;
        }

        [HttpGet]
        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetStaffPaymentApproval(int staffId)
        {
            var result = (new DAL_Staff_Reports()).GetStaffApprovalData(staffId);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult GotoStaffSalaryIncrement(int[] StaffIds)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (Session[ConstHelper.STAFF_SALARY_INCREMENT_FLAG] != null && (bool)Session[ConstHelper.STAFF_SALARY_INCREMENT_FLAG] == true)
                return RedirectToAction("StaffSalaryIncrementHistory", new { id = StaffIds[0] });
            else
                return RedirectToAction("StaffSalaryIncrement", new { id = StaffIds[0] });
        }

        public ActionResult StaffSalaryIncrementHistory(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            StaffModel staff = new StaffModel();
            try
            {
                Session[ConstHelper.STAFF_SALARY_INCREMENT_FLAG] = false;
                ViewData["IncrementHistory"] = GetStaffSalaryDetail(Id);
                if (Session[ConstHelper.STAFF_SALARY_INCREMENT_DETAIL_FLAG] != null && (bool)Session[ConstHelper.STAFF_SALARY_INCREMENT_DETAIL_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SALARY_INCREMENT_DETAIL_FLAG] = false;
                    ViewData["staffIncrementDetail"] = GetStaffIncrementDetail(Id);
                }
                ViewData["Error"] = salaryDetailErrorCode;
                salaryDetailErrorCode = 0;
                staff = staffRepo.GetStaffModelById(Id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(staff);
        }

        public IList GetStaffIncrementDetail(int staffId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime salaryIncrementDate = (DateTime)Session[ConstHelper.STAFF_SALARY_INCREMENT_DATE];
                var sql = @"Select ssih.IncrementId, IncrementDate, case when IncrementType = 1 then 'Basic Salary Increment'  
                            when IncrementType = 2 then case when AllownceType = 1 then 'Newly Added Allownce' 
                            else  'Increment in Allownce' end end as IncrementName, 
                            (select isnull(Name,'') from Allownce where Id = (select AllownceId from StaffAllownce where Id = ssih.AllownceId)) as AllownceName,
                            Increment, IsApplied  from StaffSalaryIncrementHistory ssih
                            where convert (date, IncrementDate) = convert (date,'{1}') and StaffId = {0}
                            order by IncrementName";
                sql = string.Format(sql, staffId, GetDate(salaryIncrementDate));
                var results = staffRepo.SearchStaffSalaryDetail(sql);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return (IList)results;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                salaryDetailErrorCode = 2420;
            }
            return null;
        }


        public ActionResult getStaffSalaryIncrementDetail(int StaffId, DateTime IncrementDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Session[ConstHelper.STAFF_SALARY_INCREMENT_DATE] = IncrementDate;
                Session[ConstHelper.STAFF_SALARY_INCREMENT_DETAIL_FLAG] = true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("StaffSalaryIncrementHistory", new { id = StaffId });

        }

        public IList GetStaffSalaryDetail(int staffId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var sql = @"select staffId, convert (date, IncrementDate) as IncrementDate, 
                        (select isnull(sum(Increment),0) from StaffSalaryIncrementHistory where IncrementType = 1
                         and convert (date, IncrementDate) = convert (date, ssih.IncrementDate) and staffId = {0}) as BasicSalaryIncrement, 
                        (select isnull(sum(Increment),0) from StaffSalaryIncrementHistory where IncrementType = 2 
                         and convert (date, IncrementDate) = convert (date, ssih.IncrementDate) and staffId = {0}) as AllownceIncrement 
                        from StaffSalaryIncrementHistory ssih
                        where staffId = {0}
                        Group BY convert (date, ssih.IncrementDate), StaffId
                        order by convert (date, ssih.IncrementDate) desc";
                sql = string.Format(sql, staffId);
                var results = staffRepo.SearchStaffSalaryDetail(sql);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return (IList)results;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                salaryDetailErrorCode = 1420;
            }
            return null;
        }

        public ActionResult StaffSalaryIncrement(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            StaffModel staff = new StaffModel();
            try
            {
                ViewBag.StaffAllownces = new SelectList(SessionHelper.AllownceList(Session.SessionID), "Id", "Name");
                ViewData["Error"] = staffSalaryErrorCode;
                staffSalaryErrorCode = 0;
                staff = staffRepo.GetStaffModelById(Id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(staff);
        }

        [HttpPost]
        public void SaveStaffAllownces(List<StaffAllownce> staffAllowncesList)
        {
            staffAllownces = staffAllowncesList;
        }

        [HttpPost]
        [ActionName("StaffSalaryIncrement")]
        [OnAction(ButtonName = "SaveSalaryIncrement")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSalaryIncrement(Staff staff, string salaryIncrement)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Staff StaffObj = staffRepo.GetStaffById(staff.StaffId);
                if (StaffObj != null && salaryIncrement.Length > 0)
                {
                    StaffObj.Salary = StaffObj.Salary + int.Parse(salaryIncrement);
                    staffRepo.UpdateStaff(StaffObj);

                    StaffSalaryIncrementHistory history = new StaffSalaryIncrementHistory();
                    history.StaffId = staff.StaffId;
                    history.Increment = int.Parse(salaryIncrement);
                    history.IncrementType = 1;
                    history.IsApplied = true;
                    history.IncrementDate = DateTime.Now;
                    staffRepo.AddStaffSalaryIncrementHostory(history);
                }

                if (staffAllownces != null)
                {
                    foreach (StaffAllownce allownce in staffAllownces)
                    {
                        bool isNewAllownce = false;
                        StaffAllownce allownceObj = staffRepo.GetStaffAllownceByStaffId((int)staff.StaffId, (int)allownce.AllownceId);
                        int incrementAmount = 0;
                        if (allownce.Increment != null)
                            incrementAmount = (int)allownce.Increment;
                        if (allownceObj != null)
                        {
                            allownceObj.Amount = allownce.Amount + incrementAmount;
                            staffRepo.UpdateStaffAllownce(allownceObj);

                        }
                        else
                        {
                            isNewAllownce = true;
                            allownce.StaffId = staff.StaffId;
                            allownce.Amount = allownce.Amount + incrementAmount;
                            incrementAmount = (int)allownce.Amount;
                            staffRepo.AddStaffAllownce(allownce);
                        }

                        if (incrementAmount > 0 || isNewAllownce)
                        {
                            StaffSalaryIncrementHistory history = new StaffSalaryIncrementHistory();
                            history.StaffId = staff.StaffId;
                            history.Increment = incrementAmount;
                            history.IncrementType = 2;
                            if (isNewAllownce)
                                history.AllownceType = 1;
                            else
                                history.AllownceType = 2;
                            history.AllownceId = allownce.Id;
                            if (isNewAllownce == false)
                                history.AllownceId = allownceObj.Id;

                            history.IncrementDate = DateTime.Now;
                            history.IsApplied = true;
                            staffRepo.AddStaffSalaryIncrementHostory(history);
                        }

                    }
                }

                staffSalaryErrorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                staffSalaryErrorCode = 420;
            }
            return RedirectToAction("StaffSalaryIncrement", new { id = staff.StaffId });
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSalaryIncrements(DateTime[] IncrementsDate, string[] IncrementName, int[] increment, string[] AllownceName, int[] IncrementIndexes, int[] IncrementIds)
        {
            int staffId = 0;
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int deleteIndex = IncrementIndexes[0];
                int IncrementId = IncrementIds[deleteIndex];
                var staffSalaryHistory = staffRepo.GetStaffSalaryIncrementHistory(IncrementId);
                staffId = (int)staffSalaryHistory.StaffId;
                if (staffSalaryHistory.IncrementType == 1)
                {
                    Staff staff = staffRepo.GetStaffById((int)staffSalaryHistory.StaffId);
                    staff.Salary = staff.Salary - staffSalaryHistory.Increment;
                    staffRepo.UpdateStaff(staff);
                    staffRepo.DeleteStaffSalaryIncrementHistory(staffSalaryHistory);
                }
                else if (staffSalaryHistory.IncrementType == 2)
                {
                    if (staffSalaryHistory.AllownceType == 2)//updated
                    {
                        StaffAllownce staffAllownce = staffRepo.GetStaffAllownceById((int)staffSalaryHistory.AllownceId);
                        staffAllownce.Amount = staffAllownce.Amount - staffSalaryHistory.Increment;
                        staffRepo.UpdateStaffAllownce(staffAllownce);
                        staffRepo.DeleteStaffSalaryIncrementHistory(staffSalaryHistory);
                    }
                    else if (staffSalaryHistory.AllownceType == 1)//newly added
                    {
                        StaffAllownce staffAllownce = staffRepo.GetStaffAllownceById((int)staffSalaryHistory.AllownceId);
                        staffRepo.DeleteStaffAllownce(staffAllownce);
                        var historyList = staffRepo.GetStaffSalaryIncrementHistoryByAllownceId((int)staffSalaryHistory.AllownceId);
                        foreach (var history in historyList)
                        {
                            staffRepo.DeleteStaffSalaryIncrementHistory(history);
                        }
                    }
                }
                //if (staffSalaryHistory.AllownceType != 1)
                //    staffRepo.DeleteStaffSalaryIncrementHistory(staffSalaryHistory);
                salaryDetailErrorCode = 10;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                salaryDetailErrorCode = 420;
            }
            return RedirectToAction("StaffSalaryIncrementHistory", new { id = staffId });
        }

        bool isApplicable(int index, int[] IncrementIndexes)
        {
            for (int i = 0; i < IncrementIndexes.Length; i++)
            {
                if (index == IncrementIndexes[i])
                    return true;
            }
            return false;
        }

        private void CreateJournalEntry(StaffSalary salary, string finanaceAccount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff salary Journal Entry for staff Id : " + salary.StaffId);

                var staff = staffRepo.GetStaffById((int)salary.StaffId);
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountById((int)salary.FinanceAccountId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = salary.PaidAmount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff monthly salary is paid for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = ChequeNO;
                je.EntryType = "JE";
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Staff Salary Journal Entry is saved succesfully");

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Credit Detail");
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = "Staff monthly salary is paid for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Credit Detail Advance Adjustment");
                if (salary.AdvanceAdjustment > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail3 = new JournalEntryCreditDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.AdvanceAdjustment;
                    entryDetail3.Description = "Advance is adjusted for : " + staff.Name + ", From Advance Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail3);
                    FinanceHelper.UpdateCreditAccountBalance((int)tempAccount3.FourthLvlAccountId, (int)entryDetail3.Amount, je.EntryId);
                    FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail3.FifthLvlAccountId, (int)entryDetail3.Amount);
                }

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail");
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name, branchId);
                //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = salary.SalaryAmount;
                entryDetail1.Description = "Staff monthly salary is paid for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail Deduction");
                if (salary.Deduction > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail2 = new JournalEntryCreditDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.Deduction;
                    entryDetail2.Description = "Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail2);
                    FinanceHelper.UpdateDebitAccountBalance((int)tempAccount2.FourthLvlAccountId, (int)entryDetail2.Amount);
                    FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail2.FifthLvlAccountId, (int)entryDetail2.Amount);
                }

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail Bonus");
                if (salary.Bonus > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail2 = new JournalEntryDebitDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.Bonus;
                    entryDetail2.Description = "Bonus is paid for : " + staff.Name + ", against Bonus Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail2);
                    FinanceHelper.UpdateDebitAccountBalance((int)tempAccount2.FourthLvlAccountId, (int)entryDetail2.Amount);
                    FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail2.FifthLvlAccountId, (int)entryDetail2.Amount);
                }

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void CreateUnpaidJournalEntry(StaffSalary salary, string finanaceAccount, int paidAmount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff salary unpaid Journal Entry for staff Id : " + salary.StaffId);
                var staff = staffRepo.GetStaffById((int)salary.StaffId);
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById((int)salary.FinanceAccountId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = paidAmount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff monthly salary is unpaid for : " + staff.Name + ", to Finanace Account : " + tempAccount1.AccountName;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = ChequeNO;
                je.EntryType = "JE";
                je.BranchId = branchId;
                je.DebitDescription = je.CreditDescription;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Staff Salary unpaid Journal Entry is saved succesfully");

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Credit Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name, branchId);
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = salary.SalaryAmount;
                entryDetail.Description = "Staff monthly salary is unpaid for : " + staff.Name + ", to Finanace Account : " + tempAccount1.AccountName;
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail");
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                entryDetail1.Description = "Staff monthly salary is unpaid for : " + staff.Name + ", to Finanace Account : " + tempAccount1.AccountName;
                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail Deduction");
                if (salary.Deduction > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail3 = new JournalEntryDebitDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.Deduction;
                    entryDetail3.Description = "Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail3);
                    FinanceHelper.UpdateCreditAccountBalance((int)tempAccount3.FourthLvlAccountId, (int)entryDetail3.Amount, je.EntryId);
                    FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail3.FifthLvlAccountId, (int)entryDetail3.Amount);
                }

                

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Credit Detail Bonus");
                if (salary.Bonus > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail3 = new JournalEntryCreditDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.Bonus;
                    entryDetail3.Description = "Bonus is added for : " + staff.Name + ", against Bonus Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail3);
                    FinanceHelper.UpdateCreditAccountBalance((int)tempAccount3.FourthLvlAccountId, (int)entryDetail3.Amount, je.EntryId);
                    FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail3.FifthLvlAccountId, (int)entryDetail3.Amount);
                }

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail Advance Adjustment");
                if (salary.AdvanceAdjustment > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail2 = new JournalEntryDebitDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.AdvanceAdjustment;
                    entryDetail2.Description = "Advance is adjusted for : " + staff.Name + ", From Advance Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail2);
                    FinanceHelper.UpdateDebitAccountBalance((int)tempAccount2.FourthLvlAccountId, (int)entryDetail2.Amount);
                    FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail2.FifthLvlAccountId, (int)entryDetail2.Amount);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void CreateJournalEntrySingleStaff(StaffSalary salary, string finanaceAccount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff salary Journal Entry for staff Id : " + salary.StaffId);

                var staff = staffRepo.GetStaffById((int)salary.StaffId);
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountById((int)salary.FinanceAccountId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = salary.PaidAmount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff monthly salary is paid for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = ChequeNO;
                je.EntryType = "JE";
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Staff Salary Journal Entry is saved succesfully");

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Credit Detail");
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = "Staff monthly salary is paid for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Credit Detail Advance Adjustment");
                if (salary.AdvanceAdjustment > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail3 = new JournalEntryCreditDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.AdvanceAdjustment;
                    entryDetail3.Description = "Advance is adjusted for : " + staff.Name + ", From Advance Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail3);
                    FinanceHelper.UpdateCreditAccountBalance((int)tempAccount3.FourthLvlAccountId, (int)entryDetail3.Amount, je.EntryId);
                    FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail3.FifthLvlAccountId, (int)entryDetail3.Amount);
                }

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail");
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name, branchId);
                //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = salary.SalaryAmount;
                entryDetail1.Description = "Staff monthly salary is paid for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail Deduction");
                if (salary.Deduction > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail2 = new JournalEntryCreditDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.Deduction;
                    entryDetail2.Description = "Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail2);

                }

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail Sundays Deduction");
                if (salary.SundaysDeduction > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail2 = new JournalEntryCreditDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.SundaysDeduction;
                    entryDetail2.Description = "Sundays Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail2);

                }

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail Misc Withdraw Deduction");
                if (salary.MiscWithdraw > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail2 = new JournalEntryCreditDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.MiscWithdraw;
                    entryDetail2.Description = "Misc Withdraw Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail2);

                }

                LogWriter.WriteLog("Adding Staff Salary Journal Entry Debit Detail Bonus");
                if (salary.Bonus > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail2 = new JournalEntryDebitDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.Bonus;
                    entryDetail2.Description = "Bonus is paid for : " + staff.Name + ", against Bonus Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail2);
                }

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void CreateUnpaidJournalEntrySingleStaff(StaffSalary salary, string finanaceAccount, int paidAmount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff salary unpaid Journal Entry for staff Id : " + salary.StaffId);
                var staff = staffRepo.GetStaffById((int)salary.StaffId);
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById((int)salary.FinanceAccountId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = paidAmount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff monthly salary is unpaid for : " + staff.Name + ", to Finanace Account : " + tempAccount1.AccountName;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = ChequeNO;
                je.EntryType = "JE";
                je.BranchId = branchId;
                je.DebitDescription = je.CreditDescription;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Staff Salary unpaid Journal Entry is saved succesfully");

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Credit Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name, branchId);
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = salary.SalaryAmount;
                entryDetail.Description = "Staff monthly salary is unpaid for : " + staff.Name + ", to Finanace Account : " + tempAccount1.AccountName;
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail");
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                entryDetail1.Description = "Staff monthly salary is unpaid for : " + staff.Name + ", to Finanace Account : " + tempAccount1.AccountName;
                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail Deduction");
                if (salary.Deduction > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail3 = new JournalEntryDebitDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.Deduction;
                    entryDetail3.Description = "Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail3);
                }

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail Sundays Deduction");
                if (salary.SundaysDeduction > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail3 = new JournalEntryDebitDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.SundaysDeduction;
                    entryDetail3.Description = "Sundays Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail3);
                }

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Credit Detail Bonus");
                if (salary.Bonus > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryCreditDetail entryDetail3 = new JournalEntryCreditDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.Bonus;
                    entryDetail3.Description = "Bonus is added for : " + staff.Name + ", against Bonus Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryCreditDetail(entryDetail3);
                }

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail Advance Adjustment");
                if (salary.AdvanceAdjustment > 0)
                {
                    var tempAccount2 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail2 = new JournalEntryDebitDetail();
                    entryDetail2.EntryId = je.EntryId;
                    entryDetail2.FifthLvlAccountId = tempAccount2.Id;
                    entryDetail2.Amount = salary.AdvanceAdjustment;
                    entryDetail2.Description = "Advance is adjusted for : " + staff.Name + ", From Advance Account : " + tempAccount2.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail2);
                }

                LogWriter.WriteLog("Adding Staff Salary unpaid Journal Entry Debit Detail Misc Withdraw Deduction");
                if (salary.MiscWithdraw > 0)
                {
                    var tempAccount3 = accountRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account", branchId);
                    //var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                    JournalEntryDebitDetail entryDetail3 = new JournalEntryDebitDetail();
                    entryDetail3.EntryId = je.EntryId;
                    entryDetail3.FifthLvlAccountId = tempAccount3.Id;
                    entryDetail3.Amount = salary.MiscWithdraw;
                    entryDetail3.Description = "Misc Withdraw Deduction is added for : " + staff.Name + ", against Deduction Account : " + tempAccount3.AccountName;
                    accountRepo.AddJurnalEntryDebitDetail(entryDetail3);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        [HttpPost]
        public JsonResult GetStaffAttendanceDetail(int attendanceId)
        {
            LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
            List<string[]> detailResponse = new List<string[]>();

            try
            {
                var detailList = staffRepo.GetStaffAttendanceDetailByAttId(attendanceId);
                if (detailList != null && detailList.Count > 0)
                {
                    foreach (var detail in detailList)
                    {
                        string[] tempObj = new string[2];
                        tempObj[0] = detail.TimeIn;
                        tempObj[1] = detail.TimeOut == null ? "--:--" : detail.TimeOut;

                        detailResponse.Add(tempObj);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return Json(detailResponse);
        }

        
    }

}