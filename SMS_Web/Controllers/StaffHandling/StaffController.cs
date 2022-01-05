using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using System.IO;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using SMS_Web.Helpers.PdfHelper;
using PdfSharp.Pdf;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using System.Collections;
using SMS.Modules.BuildPdf.StaffSheets;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffController : Controller
    {
        static List<StaffAllownce> staffAllownces = null;
        static byte[] staffImage;
        static List<StaffDegree> staffDegrees = null;
        //

        private static int errorCode = 0;
        // GET: /Staff/

        IStaffRepository staffRepo;
        private IFinanceAccountRepository financeRepo;
        public StaffController()
        {

            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); ;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_NEW_STAFF) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            //CreateAccount();

            try
            {
                if (Id > 0)
                {
                    errorCode = 0;
                }
                else
                {
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                    ViewBag.Relegions = new SelectList(SessionHelper.RelegionList, "Id", "Name");
                    ViewBag.Genders = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
                    ViewBag.StaffAllownces = new SelectList(SessionHelper.AllownceList(Session.SessionID), "Id", "Name");
                    ViewBag.MeritalStatus = new SelectList(SessionHelper.MeritalStatusList, "Id", "Merital_Status");
                    ViewBag.TypeId = new SelectList(SessionHelper.StaffTypeList, "ID", "TypeName");
                }

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.STAFF_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SEARCH_FLAG] = false;
                    ViewData["staff"] = SearchStaff(branchId);
                }
                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                if (Id > 0)
                {
                    var staff = staffRepo.GetStaffById(Id);
                    var designation = staffRepo.GetDesignationById((int)staff.DesignationId);
                    ViewBag.Designations = new SelectList(SessionHelper.DesignationList(Session.SessionID), "Id", "Name", staff.DesignationId);
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName", designation.CatagoryId);
                    ViewBag.Relegions = new SelectList(SessionHelper.RelegionList, "Id", "Name", staff.ReligionId);
                    ViewBag.Genders = new SelectList(SessionHelper.GenderList, "Id", "Gender1", staff.GenederId);
                    ViewBag.StaffAllownces = new SelectList(SessionHelper.AllownceList(Session.SessionID), "Id", "Name");
                    ViewBag.MeritalStatus = new SelectList(SessionHelper.MeritalStatusList, "Id", "Merital_Status", staff.MeritalStatusId);
                    ViewBag.TypeId = new SelectList(SessionHelper.StaffTypeList, "ID", "TypeName");
                    Session["OldName"] = staff.Name;
                    return View(staff);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            //OpenStaffAccounts();
            return View("");
        }

        public ActionResult Dashboard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            return View();
        }

        public JsonResult GetStaffSubjectStats()
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            return Json(staffRepo.GetStaffSubjectStats(branchId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_NEW_STAFF) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                Session["Operation"] = Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.STAFF_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SEARCH_FLAG] = false;
                    ViewData["staff"] = SearchStaff(branchId);
                }
                
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

        public ActionResult StaffCertificates(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_CERTIFICATE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (Id > 0)
                {
                    errorCode = 0;
                }
                else
                {
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                }

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.STAFF_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SEARCH_FLAG] = false;
                    ViewData["staff"] = SearchAllStaff(branchId).Where(x => x.IsLeft == true).ToList();
                }
                ViewData["Operation"] = Id;
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

        public ActionResult StaffLeaving(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_LEAVING) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (Id > 0)
                {
                    errorCode = 0;
                }
                else
                {
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                }

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.STAFF_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SEARCH_FLAG] = false;
                    ViewData["staff"] = SearchAllStaff(branchId).Where(x => x.IsLeft == false).ToList();
                }
                ViewData["Operation"] = Id;
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
        public JsonResult GetStaffAllownces(int staffId)
        {
            List<string[]> allownceList = new List<string[]>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var allownces = staffRepo.GetStaffAllownceByStaffId(staffId);
                foreach (StaffAllownce allownce in allownces)
                {
                    string[] tempObj = new string[3];
                    tempObj[0] = allownce.Allownce.Name;
                    tempObj[1] = allownce.Allownce.Id.ToString();
                    tempObj[2] = allownce.Amount.ToString();
                    allownceList.Add(tempObj);
                }
                LogWriter.WriteLog("Staff Allownces Count : " + (allownceList == null ? 0 : allownceList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return Json(allownceList);
        }

        [HttpPost]
        public JsonResult GetStaffDegrees(int staffId)
        {
            List<string[]> degreeList = new List<string[]>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var staffDegrees = staffRepo.GetStaffDegreeByStaffId(staffId);
                foreach (StaffDegree degree in staffDegrees)
                {
                    string[] tempObj = new string[6];
                    tempObj[0] = degree.DegreeName;
                    tempObj[1] = degree.Year;
                    tempObj[2] = degree.ObtMarks.ToString();
                    tempObj[3] = degree.TotalMarks.ToString();
                    tempObj[4] = degree.RollNo;
                    tempObj[5] = degree.Institute;
                    degreeList.Add(tempObj);
                }
                LogWriter.WriteLog("Staff Degrees Count : " + (degreeList == null ? 0 : degreeList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return Json(degreeList);
        }

        //
        // POST: /Staff/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "StaffId")]Staff staff)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ModelState.Remove("Designation.Name");
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Model state is valid, creating the staff");

                    var classObj = staffRepo.GetStaffByNameAndFatherName(staff.Name, staff.FatherName);
                    if (classObj == null)
                    {
                        staff.StaffImage = staffImage;
                        staff.DesignationId = staff.Designation.Id;
                        staff.Designation = null;
                        int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                        staff.BranchId = branchId;
                        staff.Branch = null;

                        staffRepo.AddStaff(staff);
                        LogWriter.WriteLog("Staff is addedd succesfully");

                        SmsInfoProxy.sendSmsStaffAdmissionEvent(staff);
                        LogWriter.WriteLog("Adding staff Degrees");
                        if (staffDegrees != null && staffDegrees.Count > 0)
                        {
                            foreach (StaffDegree degree in staffDegrees)
                            {
                                degree.StaffId = staff.StaffId;
                                staffRepo.AddStaffDegree(degree);
                            }
                        }

                        LogWriter.WriteLog("Adding staff Allownces");
                        if (staffAllownces != null && staffAllownces.Count > 0)
                        {
                            foreach (StaffAllownce allownce in staffAllownces)
                            {
                                allownce.StaffId = staff.StaffId;
                                staffRepo.AddStaffAllownce(allownce);
                            }
                        }
                        staffDegrees = null;
                        staffAllownces = null;
                        errorCode = 2;

                        SessionHelper.InvalidateStaffCache = false;
                        createFinanceAccount(staff);
                    }
                    else if (classObj != null)
                        errorCode = 11;
                }
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["designation"] = SessionHelper.DesignationList(Session.SessionID);
                    ViewBag.Designations = new SelectList(SessionHelper.DesignationList(Session.SessionID), "Id", "Name");
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName");
                    ViewBag.Relegions = new SelectList(SessionHelper.RelegionList, "Id", "Name");
                    ViewBag.Genders = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
                    ViewBag.StaffAllownces = new SelectList(SessionHelper.AllownceList(Session.SessionID), "Id", "Name");
                    ViewBag.MeritalStatus = new SelectList(SessionHelper.MeritalStatusList, "Id", "Merital_Status");
                    ViewBag.TypeId = new SelectList(SessionHelper.StaffTypeList, "ID", "TypeName");
                    return View(staff);
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

        private void OpenStaffAccounts()
        {
            var staffList = staffRepo.GetAllStaff();

            foreach (var staff in staffList)
            {
                //UpdateFinanceAccount(staff, staff.Name, 1);
                FinanceFifthLvlAccount accounts = new FinanceFifthLvlAccount();
                accounts.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name;
                int salaryAccountAccedamic = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_ACCEDAMIC_STAFF, ConstHelper.CAT_STAFF_SALARIES);
                int salaryAccountNonAccedamic = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_NON_ACCEDAMIC_STAFF, ConstHelper.CAT_STAFF_SALARIES);

                FinanceFifthLvlAccount miscWithdrawAccount = new FinanceFifthLvlAccount();

                miscWithdrawAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Misc Withdraw Account";
                miscWithdrawAccount.AccountDescription = "Bonus Account For : " + accounts.AccountName;
                miscWithdrawAccount.CreatedOn = DateTime.Now;
                miscWithdrawAccount.Value = 0;
                miscWithdrawAccount.Count = 0;
                miscWithdrawAccount.BranchId = staff.BranchId;
                miscWithdrawAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    miscWithdrawAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(miscWithdrawAccount);
            }
        }

        private void createFinanceAccount(Staff staff)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Opening staff accounts with the Sraff Id : " + staff.StaffId);
                int advanceAccountTeaching = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_TEACHING_STAFF, ConstHelper.CAT_STAFF_ADVANCES);
                int advanceAccountNonTeaching = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_NON_TEACHING_STAFF, ConstHelper.CAT_STAFF_ADVANCES);
                int salaryAccountAccedamic = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_ACCEDAMIC_STAFF, ConstHelper.CAT_STAFF_SALARIES);
                int salaryAccountNonAccedamic = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_NON_ACCEDAMIC_STAFF, ConstHelper.CAT_STAFF_SALARIES);

                LogWriter.WriteLog("Opening staff salary account");
                FinanceFifthLvlAccount accounts = new FinanceFifthLvlAccount();

                accounts.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name;
                accounts.AccountDescription = "Finanace Salary Account For : " + accounts.AccountName;
                accounts.CreatedOn = DateTime.Now;
                accounts.Value = 0;
                accounts.Count = 0;
                accounts.BranchId = staff.BranchId;
                accounts.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    accounts.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(accounts);

                LogWriter.WriteLog("Opening staff advance account");
                FinanceFifthLvlAccount advanceAccount = new FinanceFifthLvlAccount();

                advanceAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account";
                advanceAccount.AccountDescription = "Advance Account For : " + accounts.AccountName;
                advanceAccount.CreatedOn = DateTime.Now;
                advanceAccount.Value = 0;
                advanceAccount.Count = 0;
                advanceAccount.BranchId = staff.BranchId;
                advanceAccount.FourthLvlAccountId = advanceAccountTeaching;
                if (staff.TypeId == 2)
                    advanceAccount.FourthLvlAccountId = advanceAccountNonTeaching;

                financeRepo.AddFinanceFifthLvlAccount(advanceAccount);

                LogWriter.WriteLog("Opening staff deduction account");
                FinanceFifthLvlAccount deductionAccount = new FinanceFifthLvlAccount();

                deductionAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account";
                deductionAccount.AccountDescription = "Deduction Account For : " + accounts.AccountName;
                deductionAccount.CreatedOn = DateTime.Now;
                deductionAccount.Value = 0;
                deductionAccount.Count = 0;
                deductionAccount.BranchId = staff.BranchId;
                deductionAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    deductionAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(deductionAccount);

                LogWriter.WriteLog("Opening staff bonus account");
                FinanceFifthLvlAccount bonusAccount = new FinanceFifthLvlAccount();

                bonusAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account";
                bonusAccount.AccountDescription = "Bonus Account For : " + accounts.AccountName;
                bonusAccount.CreatedOn = DateTime.Now;
                bonusAccount.Value = 0;
                bonusAccount.Count = 0;
                bonusAccount.BranchId = staff.BranchId;
                bonusAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    bonusAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(bonusAccount);

                FinanceFifthLvlAccount miscWithdrawAccount = new FinanceFifthLvlAccount();

                miscWithdrawAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Misc Withdraw Account";
                miscWithdrawAccount.AccountDescription = "Bonus Account For : " + accounts.AccountName;
                miscWithdrawAccount.CreatedOn = DateTime.Now;
                miscWithdrawAccount.Value = 0;
                miscWithdrawAccount.Count = 0;
                miscWithdrawAccount.BranchId = staff.BranchId;
                miscWithdrawAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    miscWithdrawAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(miscWithdrawAccount);

                SessionHelper.InvalidateFinanceFifthLvlCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
        
        private void UpdateFinanceAccount(Staff staff, string oldName, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Updating staff accounts with the Sraff Id : " + staff.StaffId);
                int advanceAccountTeaching = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_TEACHING_STAFF, ConstHelper.CAT_STAFF_ADVANCES);
                int advanceAccountNonTeaching = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_NON_TEACHING_STAFF, ConstHelper.CAT_STAFF_ADVANCES);
                int salaryAccountAccedamic = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_ACCEDAMIC_STAFF, ConstHelper.CAT_STAFF_SALARIES);
                int salaryAccountNonAccedamic = SessionHelper.GetFourthLvlConfigurationAccount((int)staff.BranchId, ConstHelper.CAT_NON_ACCEDAMIC_STAFF, ConstHelper.CAT_STAFF_SALARIES);

                LogWriter.WriteLog("Updating staff salary account");
                string oldAccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + oldName;
                FinanceFifthLvlAccount accounts = financeRepo.GetFinanceFifthLvlAccountByName(oldAccountName, branchId);
                if (accounts != null)
                {
                    accounts.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name;
                    accounts.AccountDescription = "Finanace Salary Account For : " + accounts.AccountName;
                    financeRepo.UpdateFinanceFifthLvlAccount(accounts);
                }
                else
                {
                    accounts = new FinanceFifthLvlAccount();

                    accounts.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name;
                    accounts.AccountDescription = "Finanace Salary Account For : " + accounts.AccountName;
                    accounts.CreatedOn = DateTime.Now;
                    accounts.Value = 0;
                    accounts.Count = 0;
                    accounts.BranchId = staff.BranchId;
                    accounts.FourthLvlAccountId = salaryAccountAccedamic;
                    if (staff.TypeId == 2)
                        accounts.FourthLvlAccountId = salaryAccountNonAccedamic;

                    financeRepo.AddFinanceFifthLvlAccount(accounts);
                }


                LogWriter.WriteLog("Updating staff Advance account");
                string oldAdvanceAccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + oldName + " Advance Account";
                FinanceFifthLvlAccount advanceAccount = financeRepo.GetFinanceFifthLvlAccountByName(oldAdvanceAccountName, branchId);
                if (advanceAccount != null)
                {
                    advanceAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account";
                    advanceAccount.AccountDescription = "Advance Account For : " + advanceAccount.AccountName;
                    financeRepo.UpdateFinanceFifthLvlAccount(advanceAccount);
                }
                else
                {
                    advanceAccount = new FinanceFifthLvlAccount();

                    advanceAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account";
                    advanceAccount.AccountDescription = "Advance Account For : " + accounts.AccountName;
                    advanceAccount.CreatedOn = DateTime.Now;
                    advanceAccount.Value = 0;
                    advanceAccount.Count = 0;
                    advanceAccount.BranchId = staff.BranchId;
                    advanceAccount.FourthLvlAccountId = advanceAccountTeaching;
                    if (staff.TypeId == 2)
                        advanceAccount.FourthLvlAccountId = advanceAccountNonTeaching;

                    financeRepo.AddFinanceFifthLvlAccount(advanceAccount);
                }

                LogWriter.WriteLog("Updating staff deduction account");
                string oldDeductionAccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + oldName + " Deduction Account";
                FinanceFifthLvlAccount deductionAccount = financeRepo.GetFinanceFifthLvlAccountByName(oldDeductionAccountName, branchId);
                if (deductionAccount != null)
                {
                    deductionAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account";
                    deductionAccount.AccountDescription = "Deduction Account For : " + deductionAccount.AccountName;
                    financeRepo.UpdateFinanceFifthLvlAccount(deductionAccount);
                }
                else
                {
                    deductionAccount = new FinanceFifthLvlAccount();

                    deductionAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account";
                    deductionAccount.AccountDescription = "Deduction Account For : " + accounts.AccountName;
                    deductionAccount.CreatedOn = DateTime.Now;
                    deductionAccount.Value = 0;
                    deductionAccount.Count = 0;
                    deductionAccount.BranchId = staff.BranchId;
                    deductionAccount.FourthLvlAccountId = salaryAccountAccedamic;
                    if (staff.TypeId == 2)
                        deductionAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                    financeRepo.AddFinanceFifthLvlAccount(deductionAccount);
                }

                LogWriter.WriteLog("Updating staff bonus account");
                string oldBonusAccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + oldName + " Bonus Account";
                FinanceFifthLvlAccount bonusAccount = financeRepo.GetFinanceFifthLvlAccountByName(oldBonusAccountName, branchId);
                if (bonusAccount != null)
                {
                    bonusAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account";
                    bonusAccount.AccountDescription = "Bonus Account For : " + bonusAccount.AccountName;
                    financeRepo.UpdateFinanceFifthLvlAccount(bonusAccount);
                }
                else
                {
                    bonusAccount = new FinanceFifthLvlAccount();

                    bonusAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account";
                    bonusAccount.AccountDescription = "Bonus Account For : " + accounts.AccountName;
                    bonusAccount.CreatedOn = DateTime.Now;
                    bonusAccount.Value = 0;
                    bonusAccount.Count = 0;
                    bonusAccount.BranchId = staff.BranchId;
                    bonusAccount.FourthLvlAccountId = salaryAccountAccedamic;
                    if (staff.TypeId == 2)
                        bonusAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                    financeRepo.AddFinanceFifthLvlAccount(bonusAccount);
                }

                SessionHelper.InvalidateFinanceFifthLvlCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Staff staff)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ModelState.Remove("Designation.Name");
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Model state is valid, updating the staff");
                    var classObj = staffRepo.GetStaffByNameAndFatherNameAndId(staff.Name, staff.FatherName, staff.StaffId);
                    if (classObj == null)
                    {
                        staff.StaffImage = staffImage;
                        staff.DesignationId = staff.Designation.Id;
                        staff.Designation = null;
                        int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                        staff.BranchId = branchId;
                        staff.Branch = null;

                        staffRepo.UpdateStaff(staff);
                        LogWriter.WriteLog("Staff is saved successfully");

                        var staffDegreeListDelete = staffRepo.GetStaffDegreeByStaffId(staff.StaffId);
                        var staffAllowncesListDelete = staffRepo.GetStaffAllownceByStaffId(staff.StaffId);
                        if (staffDegreeListDelete != null)
                        {
                            foreach (StaffDegree degree in staffDegreeListDelete)
                            {
                                staffRepo.DeleteStaffDegree(degree);
                            }
                        }
                        if (staffAllowncesListDelete != null)
                        {
                            foreach (StaffAllownce allownce in staffAllowncesListDelete)
                            {
                                staffRepo.DeleteStaffAllownce(allownce);
                            }
                        }
                        LogWriter.WriteLog("Updating the staff degrees");
                        if (staffDegrees != null)
                        {
                            foreach (StaffDegree degree in staffDegrees)
                            {
                                degree.StaffId = staff.StaffId;
                                staffRepo.AddStaffDegree(degree);
                            }
                        }
                        LogWriter.WriteLog("Updating the staff aloownces");
                        if (staffAllownces != null)
                        {
                            foreach (StaffAllownce allownce in staffAllownces)
                            {
                                allownce.StaffId = staff.StaffId;
                                staffRepo.AddStaffAllownce(allownce);
                            }
                        }
                        staffDegrees = null;
                        staffAllownces = null;
                        errorCode = 2;

                        string oldName = (string)Session["OldName"];
                        Session["OldName"] = null;
                        if (staff.Name != oldName)
                        {
                            UpdateFinanceAccount(staff, oldName, branchId);
                        }
                        SessionHelper.InvalidateStaffCache = false;
                    }
                    else if (classObj != null)
                        errorCode = 11;
                }
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Operation"] = staff.StaffId;
                    ViewData["designation"] = SessionHelper.DesignationList(Session.SessionID);
                    ViewBag.Designations = new SelectList(SessionHelper.DesignationList(Session.SessionID), "Id", "Name");
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName");
                    ViewBag.Relegions = new SelectList(SessionHelper.RelegionList, "Id", "Name");
                    ViewBag.Genders = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
                    ViewBag.StaffAllownces = new SelectList(SessionHelper.AllownceList(Session.SessionID), "Id", "Name");
                    ViewBag.MeritalStatus = new SelectList(SessionHelper.MeritalStatusList, "Id", "Merital_Status");
                    ViewBag.TypeId = new SelectList(SessionHelper.StaffTypeList, "ID", "TypeName");
                    return View(staff);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = 0 });

        }
        [HttpPost]
        public void UploadImage()
        {
            var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage"];
            using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
            {
                staffImage = binaryReader.ReadBytes(httpPostedFile.ContentLength);
            }
        }

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Staff staff = staffRepo.GetStaffById(id);
                if (staff != null)
                {
                    staffRepo.DeleteStaff(staff);
                    errorCode = 4;
                }
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

        //
        // GET: /Staff/Delete/5
        [HttpPost]
        public void SaveStaffAllownces(List<StaffAllownce> staffAllowncesList)
        {
            staffAllownces = staffAllowncesList;
        }

        [HttpPost]
        public void SaveStaffDegree(List<StaffDegree> staffDegreeList)
        {
            staffDegrees = staffDegreeList;
        }


        [HttpPost]
        [ActionName("Search")]
        [OnAction(ButtonName = "Search")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStaff(int? CatagoryId = 0, int? DesignationId = 0, string StaffId = "", string Name = "", string FatherName = "")
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_FLAG] = true;

                if (CatagoryId != null)
                    Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                //if (staff.DesignationId != null)
                Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = DesignationId;
                if (StaffId.Length > 0)
                    Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = int.Parse(StaffId);
                //if (staff.Name != null)
                Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = Name;
                //if (staff.FatherName != null)
                Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = FatherName;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Search", new { id = Session["Operation"] });
        }

        [HttpPost]
        [ActionName("StaffCertificates")]
        [OnAction(ButtonName = "SearchStaffCertificates")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchSearchStaffCertificates(Staff staff, int? CatagoryId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_FLAG] = true;

                if (CatagoryId != null)
                    Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                if (staff.DesignationId != null)
                    Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = (int)staff.DesignationId;
                Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = (int)staff.StaffId;
                if (staff.Name != null)
                    Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = staff.Name;
                if (staff.FatherName != null)
                    Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = staff.FatherName;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("StaffCertificates", new { id = 0 });
        }

        [HttpPost]
        [ActionName("StaffLeaving")]
        [OnAction(ButtonName = "SearchStaffLeaving")]
        [ValidateAntiForgeryToken]
        public ActionResult StaffLeaving(Staff staff, int? CatagoryId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_FLAG] = true;

                if (CatagoryId != null)
                    Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                if (staff.DesignationId != null)
                    Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = (int)staff.DesignationId;
                Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = (int)staff.StaffId;
                if (staff.Name != null)
                    Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = staff.Name;
                if (staff.FatherName != null)
                    Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = staff.FatherName;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("StaffLeaving", new { id = 0 });
        }

        [HttpPost]
        [ActionName("StaffCertificates")]
        [OnAction(ButtonName = "CreateCertificate")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCertificate(int[] StaffIds)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total staff for certificates : " + (StaffIds == null ? 0 : StaffIds.Count()) );
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                IList sname = new List<string>();
                IList fname = new List<string>();
                IList desgn = new List<string>();
                IList fDate = new List<DateTime>();
                IList toDate = new List<DateTime>();
                IList staffIds = new List<int>();

                LogWriter.WriteLog("Compiling the staff certificates data");
                foreach (int id in StaffIds)
                {
                    StaffModel staff = staffRepo.GetStaffModelById(id);
                    sname.Add(staff.Name);
                    fname.Add(staff.FatherName);
                    desgn.Add(staff.DesignationName);
                    fDate.Add(staff.JoinDate);
                    toDate.Add(staff.LeavingDate);
                    staffIds.Add(staff.StaffId);
                }
                LogWriter.WriteLog("creating the staff certificates");
                StaffCertificate pdf = new StaffCertificate();
                PdfDocument document = pdf.CreatePdf(sname, fname, desgn, fDate, toDate, staffIds, branchId);

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
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "StaffCertificates-" + DateTime.Now.ToString() + ".pdf" };
            //}

            //return RedirectToAction("StaffCertificates", new { id = 0 });
        }

        [HttpPost]
        [ActionName("StaffLeaving")]
        [OnAction(ButtonName = "SaveLeavingDate")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveLeavingDate(int[] StaffIds, DateTime LeavingDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total staff for leaving : " + (StaffIds == null ? 0 : StaffIds.Count()) );
                foreach (int id in StaffIds)
                {
                    Staff staff = staffRepo.GetStaffById(id);
                    staff.LeavingDate = LeavingDate;
                    staff.IsLeft = true;
                    staffRepo.UpdateStaff(staff);
                }
                errorCode = 10;

                SessionHelper.InvalidateStaffCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }

            return RedirectToAction("StaffLeaving", new { id = 0 });
        }

        [HttpPost]
        [ActionName("StaffCertificates")]
        [OnAction(ButtonName = "ResetLeavingDate")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetLeavingDate(int[] StaffIds, DateTime LeavingDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total staff for reset leaving : " + (StaffIds == null ? 0 : StaffIds.Count()) );
                foreach (int id in StaffIds)
                {
                    Staff staff = staffRepo.GetStaffById(id);
                    staff.LeavingDate = LeavingDate;
                    staff.IsLeft = false;
                    staffRepo.UpdateStaff(staff);
                }

                errorCode = 10;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }


            return RedirectToAction("StaffCertificates", new { id = 0 });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Print")]
        [ValidateAntiForgeryToken]
        public FileResult Print(Staff staff)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Printing the staff information with the staff Id : " + staff.StaffId );
                staff = staffRepo.GetStaffById(staff.StaffId);
                StaffForm form = new StaffForm();
                PdfDocument document = form.CreatePdf(staff);

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
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = staff.Name + "-" + staff.StaffId + "-" + DateTime.Now.ToString() + ".pdf" };
            //}
            //}
            //else
            //    return null;
        }

        private List<StaffModel> SearchStaff(int branchId)
        {
            

            List<StaffModel> staffList = new List<StaffModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = (int)Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID];
                int searchDesignationId = (int)Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID];
                int searchStaffId = (int)Session[ConstHelper.STAFF_SEARCH_STAFF_ID];
                string searchStaffName = (string)Session[ConstHelper.STAFF_SEARCH_STAFF_NAME];
                string searchStaffFatherName = (string)Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME];
                staffList = staffRepo.SearchStaffModel(searchCatagoryId, searchDesignationId, searchStaffId, searchStaffName, searchStaffFatherName, branchId);
                LogWriter.WriteLog("Search staff list count : " + (staffList == null ? 0 : staffList.Count()) );
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return staffList;
        }

        private List<StaffModel> SearchAllStaff(int branchId)
        {
           

            List<StaffModel> staffList = new List<StaffModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = (int)Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID];
                int searchDesignationId = (int)Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID];
                int searchStaffId = (int)Session[ConstHelper.STAFF_SEARCH_STAFF_ID];
                string searchStaffName = (string)Session[ConstHelper.STAFF_SEARCH_STAFF_NAME];
                string searchStaffFatherName = (string)Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME];

                staffList = staffRepo.SearchAllStaffModel(searchCatagoryId, searchDesignationId, searchStaffId, searchStaffName, searchStaffFatherName, branchId);
                LogWriter.WriteLog("Search all staff list count : " + (staffList == null ? 0 : staffList.Count()) );
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return staffList;
        }

    }
}