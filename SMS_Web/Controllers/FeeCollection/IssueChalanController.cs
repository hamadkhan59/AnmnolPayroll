using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using System.Globalization;
using System.Collections;
using SMS_Web.Helpers.PdfHelper;
using System.IO;
using PdfSharp.Pdf;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using CrystalDecisions.CrystalReports.Engine;
using SMS_DAL.Reports;
using Logger;
using System.Reflection;
using System.Threading;

namespace SMS_Web.Controllers.FeeCollection
{
    public class IssueChalanController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;

        IFeePlanRepository feePlanRepo;
        IFinanceAccountRepository financeRepo;
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IAttendanceRepository attRepo;
        private IFinanceAccountRepository accountRepo;
        IStudentRepository studentRepo;
        ISecurityRepository securityRepo;

        static int classSectionId = 0;
        static string rollNumber = "";
        static string name = "";
        static string fatherName = "";
        static int errorCode = 0, paidErrorCode = 0;
        static bool IsAnnualCharges = false, IsUnpaid = false;

        public IssueChalanController()
        {

            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            attRepo = new AttendanceRepositoryImp(new SC_WEBEntities2()); ;
            securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;
        }

        //
        // GET: /IssueChalan/

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_ISSUE_CHALLAN) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;
                errorCode = 0;
                ViewData["nameList"] = SessionHelper.StudentNameList(branchId);
                voidSetSearchVeriables();
                if ((Session[ConstHelper.SEARCH_ISSUE_CHALLAN_FLAG] != null && (bool)Session[ConstHelper.SEARCH_ISSUE_CHALLAN_FLAG] == true) || errorCode == 5 || errorCode == 6 || errorCode == 7 || errorCode == 8)
                {
                    Session[ConstHelper.SEARCH_ISSUE_CHALLAN_FLAG] = false;
                    if (Session[ConstHelper.QUICK_ADMISSION_NO] != null)
                    {
                        string admissionNo = (string)Session[ConstHelper.QUICK_ADMISSION_NO];
                        Session[ConstHelper.QUICK_ADMISSION_NO] = null;
                        ViewData["SingleChallan"] = true;
                        var issueChallan = feePlanRepo.SearchIssueChallanByAdmissionNo(admissionNo);
                        Session[ConstHelper.ADMISSION_NO_CHALLAN_MONTH] = issueChallan[0].ForMonth;
                        return View(issueChallan);
                    }
                    else
                        return View(SearchIssueChalan(branchId));
                }
                else
                {
                    return View("");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchByAdmissionNo(string admissionNo)
        {
            Session[ConstHelper.SEARCH_ISSUE_CHALLAN_FLAG] = true;
            Session[ConstHelper.QUICK_ADMISSION_NO] = admissionNo;

            return RedirectToAction("Index");

        }

        public ActionResult PaidChallan()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_PAID_CHALLAN) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }


            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
                ViewData["Error"] = paidErrorCode;
                ViewData["branchId"] = branchId;
                paidErrorCode = 0;
                voidSetSearchVeriables();
                if ((Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] != null && (bool)Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] == true) || paidErrorCode == 6 || paidErrorCode == 61 || paidErrorCode == 81)
                {
                    Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] = false;
                    return View(SearchPaidChalan(branchId));
                }
                else
                {
                    return View("");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View("");
            }
        }

        public ActionResult FastPay()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FAST_PAY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewData["Error"] = paidErrorCode;
            IssuedChallanViewModel model = new IssuedChallanViewModel();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
                ViewData["nameList"] = SessionHelper.StudentNameList(branchId);
                paidErrorCode = 0;
                model = (IssuedChallanViewModel)Session[ConstHelper.SEARCH_FAST_PAY];
                Session[ConstHelper.SEARCH_FAST_PAY] = null;
                ViewData["history"] = Session[ConstHelper.SIX_MONTH_DETAIL];
                ViewData["branchId"] = branchId;
                Session[ConstHelper.SIX_MONTH_DETAIL] = null;

                if (model != null && (model.IsPaid != null || model.IsPaid.ToString() != "Yes"))
                {
                    int monthId = SessionHelper.GetMonthID(model.ForMonth.ToString().Split('-')[0]);
                    int yearId = SessionHelper.GetYearID(model.ForMonth.ToString().Split('-')[1]);
                    int calculatedFine = GetStudentTotalFine(int.Parse(model.studentId.ToString()), monthId, (2016 + yearId - 1), branchId, DateTime.Parse(model.DueDate.ToString()));

                    if (model.Fine.ToString().Length > 0)
                    {
                        int savedFine = model.Fine;
                        if (calculatedFine >= savedFine)
                        {
                            model.Fine = calculatedFine - savedFine;
                        }
                        else
                        {
                            model.Fine = 0;
                        }
                    }
                    else
                    {
                        model.Fine = calculatedFine;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(model);
        }

        public ActionResult MonthlyWaveOff()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FAST_PAY) == false) //FO_MONTHLY_WAVE_OFF
            {
                return RedirectToAction("Index", "NoPermission");
            }

            MonthlyWaveOffModel model = new MonthlyWaveOffModel();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewData["Error"] = paidErrorCode;

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["nameList"] = SessionHelper.StudentNameList(branchId);
                if (Session[ConstHelper.WAVE_OFF_MODEL] != null)
                {
                    model = (MonthlyWaveOffModel)Session[ConstHelper.WAVE_OFF_MODEL];
                }
                Session[ConstHelper.WAVE_OFF_MODEL] = null;
                ViewData["Error"] = paidErrorCode;
                paidErrorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(model);
        }

        public ActionResult PaidPartialChallan()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_PAID_CHALLAN) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                //var issuedchallans = db.IssuedChallans.Include(i => i.ChallanStudentDetail);
                //return View(issuedchallans.ToList());
                ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.ChallanId = new SelectList(SessionHelper.ChallanList(Session.SessionID), "Id", "Name");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.BankId = new SelectList(SessionHelper.BankAccountList(Session.SessionID), "Id", "BankDetail");
                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
                ViewBag.FinanceAccountId = new SelectList(SessionHelper.FeeAccountDetailList(Session.SessionID), "Id", "AccountName");
                //ViewBag.FinanceAccountId = new SelectList(SessionHelper.FeeAccountDetailList(Session.SessionID), "Id", "AccountName");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["financeAccounts"] = SessionHelper.FeeFinanceAccountList(Session.SessionID);
                ViewData["Error"] = paidErrorCode;
                paidErrorCode = 0;
                voidSetSearchVeriables();
                if ((Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] != null && (bool)Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] == true) || paidErrorCode == 6 || paidErrorCode == 61 || paidErrorCode == 81)
                {
                    Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] = false;
                    return View(SearchPaidChalan(branchId));
                }
                else
                {
                    return View("");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View("");

            }
        }

        //
        // GET: /IssueChalan/Details/5

        //public ActionResult Details(decimal id = 0)
        //{
        //    IssuedChallan issuedchallan = db.IssuedChallans.Find(id);
        //    if (issuedchallan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(issuedchallan);
        //}

        //
        // GET: /IssueChalan/Create

        //public ActionResult Create()
        //{
        //    ViewBag.ChallanToStdId = new SelectList(db.ChallanStudentDetails, "Id", "Id");
        //    return View();
        //}

        //
        // POST: /IssueChalan/Create

        private List<IssuedChallanViewModel> SearchIssueChalan(int branchId)
        {
            List<StudentModel> studentList = null;
            List<StudentModel> newStudentList = null;
            //List<IssuedChallan> issuedChalaList = null;
            List<IssuedChallanViewModel> studentChalanList = new List<IssuedChallanViewModel>();

            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
                string rollNumber = (string)Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO];
                string name = (string)Session[ConstHelper.SEARCH_CHALLAN_NAME];
                string fatherName = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC];
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];
                string admissionInfo = (string)Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO];
                string contactNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO];
                string challanNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CHALLAN_NO];
                int classId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID];

                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);

                LogWriter.WriteLog("Searching the issues challan with the following paramaters");
                LogWriter.WriteLog(string.Format("Month : {0}, ClassSectionId : {1}", currentMonth, classSectionId));
                LogWriter.WriteLog(string.Format("Name : {0}, FatherName : {1}, Father Cnic : {2}", name, fatherName, fatherCnic));
                LogWriter.WriteLog(string.Format("ClassId : {0}, Challan NO : {1}", classId, challanNo));
                LogWriter.WriteLog(string.Format("AdmissionNo : {0}, ContactNo : {1}", admissionInfo, contactNo));

                if (challanNo.Length > 0)
                {
                    LogWriter.WriteLog("Searching the issues challan with Challan No");
                    int chalanNo = int.Parse(challanNo);
                    studentChalanList = feePlanRepo.GetIssueChallanByChallanNo(chalanNo);
                }
                else
                {
                    if (classSectionId > 0)
                        studentChalanList = feePlanRepo.SearchIssueChallan(classSectionId, rollNumber, name, fatherName, currentMonth, fatherCnic, branchId, admissionInfo, contactNo);
                    else
                        studentChalanList = feePlanRepo.SearchClassIssueChallan(classId, rollNumber, name, fatherName, currentMonth, fatherCnic, branchId, admissionInfo, contactNo);
                }

                //get the new students in db
                LogWriter.WriteLog("Getting the newly admitted students");
                if (studentChalanList != null && studentChalanList.Count > 0)
                {
                    if (classSectionId > 0)
                        newStudentList = feePlanRepo.SearchNewStudentForChallan(classSectionId, rollNumber, name, fatherName, fatherCnic, currentMonth);
                    else
                        newStudentList = feePlanRepo.SearchClassNewStudentForChallan(classId, rollNumber, name, fatherName, fatherCnic, currentMonth);
                }

                LogWriter.WriteLog("Sarched Challan Count : " + (studentChalanList == null ? 0 : studentChalanList.Count));
                if (studentChalanList == null || studentChalanList.Count == 0)
                {
                    LogWriter.WriteLog("Challan is not created for the students, getting students to create issue challans");
                    if (classSectionId > 0)
                        studentList = feePlanRepo.SearchStudentForChallan(classSectionId, rollNumber, name, fatherName, fatherCnic);
                    else
                        studentList = feePlanRepo.SearchClassStudentForChallan(classId, rollNumber, name, fatherName, fatherCnic);

                    studentList = studentList.Where(x => x.LeavingStatusCode == 1).ToList();

                    LogWriter.WriteLog("Sarched Student Count : " + (studentList == null ? 0 : studentList.Count));
                    foreach (var student in studentList)
                    {
                        IssuedChallanViewModel icvm = new IssuedChallanViewModel();
                        ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailByStudentId(student.Id);

                        if (detail != null)
                        {
                            IssuedChallan issueChalan = new IssuedChallan();
                            issueChalan.ChallanToStdId = detail.Id;
                            issueChalan.ChalanAmount = feePlanRepo.GetChallanAmountByChallanId(detail.ChallanId);
                            //issueChalan.ChalanAmount += feePlanRepo.GetStudentArrearDetail(student.Id).Sum(x => x.ArrearAmount);
                            issueChalan.ChalanAmount += feePlanRepo.GetStudentExtraChargesByStudent(student.Id, currentMonth).Sum(x => x.HeadAmount);
                            issueChalan.PayedTo = 0;
                            issueChalan.PaidFlag = 0;
                            issueChalan.IssuedFlag = 0;
                            issueChalan.ForMonth = currentMonth;
                            issueChalan.DueDate = issueChalan.PaidDate = issueChalan.IssueDate = DateTime.Now;
                            issueChalan.BranchId = branchId;
                            issueChalan.Fine = GetStudentTotalFine(student.Id, monthId, (2016 + yearId - 1), branchId, DateTime.Parse(issueChalan.DueDate.ToString()));
                            feePlanRepo.AddIssueChallan(issueChalan);

                            icvm.Id = (int)issueChalan.Id;
                            icvm.Name = student.Name;
                            icvm.RollNumber = student.RollNumber;
                            icvm.Contact_1 = student.Contact_1;
                            icvm.Chalan = detail.Challan.Name;
                            icvm.Amount = (int)issueChalan.ChalanAmount;
                            icvm.Fine = (int)issueChalan.Fine;

                            FeeBalance balance = feePlanRepo.GetFeeBalanceByStudentId(student.Id);
                            if (balance != null)
                            {
                                icvm.Balance = (int)balance.Balance;
                                icvm.Advance = (int)(balance.Advance == null ? 0 : balance.Advance);
                            }
                            else
                            {
                                icvm.Balance = 0;
                                icvm.Advance = 0;
                            }
                            icvm.DueDate = DateTime.Now;
                            icvm.IsPaid = "No";
                            studentChalanList.Add(icvm);
                        }
                    }
                    LogWriter.WriteLog("Issue Challans are created successfully");
                }

                //add the new students in challan Lists
                LogWriter.WriteLog("Newly admitted Student Count : " + (newStudentList == null ? 0 : newStudentList.Count));
                if (newStudentList != null && newStudentList.Count > 0)
                {
                    LogWriter.WriteLog("Creating issue challans for the newly admitted students");
                    studentList = newStudentList;
                    studentList = studentList.Where(x => x.LeavingStatusCode == 1).ToList();

                    foreach (var student in studentList)
                    {
                        IssuedChallanViewModel icvm = new IssuedChallanViewModel();
                        ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailByStudentId(student.Id);

                        if (detail != null)
                        {
                            IssuedChallan issueChalan = new IssuedChallan();
                            issueChalan.ChallanToStdId = detail.Id;
                            issueChalan.ChalanAmount = feePlanRepo.GetChallanAmountByChallanId(detail.ChallanId);
                            //issueChalan.ChalanAmount += feePlanRepo.GetStudentArrearDetail(student.Id).Sum(x => x.ArrearAmount);
                            issueChalan.ChalanAmount += feePlanRepo.GetStudentExtraChargesByStudent(student.Id, currentMonth).Sum(x => x.HeadAmount);
                            issueChalan.PayedTo = 0;
                            issueChalan.DueDate = issueChalan.PaidDate = issueChalan.IssueDate = DateTime.Now;
                            issueChalan.Fine = GetStudentTotalFine(student.Id, monthId, (2016 + yearId - 1), branchId, DateTime.Parse(issueChalan.DueDate.ToString()));
                            issueChalan.PaidFlag = 0;
                            issueChalan.IssuedFlag = 0;
                            issueChalan.ForMonth = currentMonth;
                            issueChalan.BranchId = branchId;
                            feePlanRepo.AddIssueChallan(issueChalan);

                            icvm.Id = (int)issueChalan.Id;
                            icvm.Name = student.Name;
                            icvm.RollNumber = student.RollNumber;
                            icvm.Chalan = detail.Challan.Name;
                            icvm.Amount = (int)issueChalan.ChalanAmount;
                            icvm.Fine = (int)issueChalan.Fine;

                            FeeBalance balance = feePlanRepo.GetFeeBalanceByStudentId(student.Id);
                            if (balance != null)
                            {
                                icvm.Balance = (int)balance.Balance;
                                icvm.Advance = (int)(balance.Advance == null ? 0 : balance.Advance);
                            }
                            else
                            {
                                icvm.Balance = 0;
                                icvm.Advance = 0;
                            }
                            icvm.DueDate = DateTime.Now;
                            icvm.IsPaid = "No";
                            studentChalanList.Add(icvm);
                        }
                    }
                    LogWriter.WriteLog("Issue Challans are created successfully");
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return studentChalanList;
        }

        private List<IssuedChallanViewModel> SearchPaidChalan(int branchId)
        {

            List<IssuedChallanViewModel> issuedChalaList = new List<IssuedChallanViewModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
                string rollNumber = (string)Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO];
                string name = (string)Session[ConstHelper.SEARCH_CHALLAN_NAME];
                string fatherName = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC];
                string admissionInfo = (string)Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO];
                string contactNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO];
                string challanNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CHALLAN_NO];
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];
                int classId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID];

                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);
                Session[ConstHelper.PAID_MONTH] = currentMonth;

                if (challanNo.Length > 0)
                {
                    int chalanNo = int.Parse(challanNo);
                    issuedChalaList = feePlanRepo.GetPaidChallanByChallanNo(chalanNo);
                }
                else
                {
                    if (classSectionId > 0)
                        issuedChalaList = feePlanRepo.SearchPaidChallan(classSectionId, rollNumber, name, fatherName, currentMonth, fatherCnic, branchId, admissionInfo, contactNo);
                    else
                        issuedChalaList = feePlanRepo.SearchClassPaidChallan(classId, rollNumber, name, fatherName, currentMonth, fatherCnic, branchId, admissionInfo, contactNo);
                }
                LogWriter.WriteLog("Search Paid Challans Count : " + (issuedChalaList == null ? 0 : issuedChalaList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return issuedChalaList;
        }

        [HttpGet]

        public int GetFine()
        {
            var fine = feePlanRepo.GetDefinedFine();
            int fineValue = (int)fine;

            return fineValue;
        }

        [HttpGet]

        public void SetFine(int Fine)
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            var fine = feePlanRepo.GetFine(Fine);
            if (fine != null)
            {
                fine.Fine = Fine;
                feePlanRepo.UpdateFineValue(fine);
                //feePlanRepo.UpdateFine(fine);
            }

        }

        [HttpPost]
        public JsonResult GetPaidChallanDetail(int challanId)
        {
            List<string[]> allownceList = new List<string[]>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Getting Paid Challan Detail for the Challan Id : " + challanId);
                var studentChallan = feePlanRepo.GetStudentChallanDetailById(challanId);
                string month = (string)Session[ConstHelper.PAID_MONTH];

                IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByIdAndMonth(challanId, month);
                IssuedChallan prevIssueChallan = feePlanRepo.GetPreviousIssuedChallan((int)issuedChalan.ChallanToStdId, (int)issuedChalan.Id);
                int discount = 0;
                int total = 0, payTotal = 0, arrearCount = 0;
                LogWriter.WriteLog("Issue Challan Paid Flag : " + issuedChalan.PaidFlag);
                if (issuedChalan.PaidFlag == 0)
                {
                    int chId = studentChallan.ChallanId;
                    var challanDetail = feePlanRepo.GetChallDetailByChallanId(chId);
                    var arrearDetail = feePlanRepo.GetStudentArrearDetail(studentChallan.StdId);
                    var lastMonthFee = feePlanRepo.GetLastMonthsUnPaidFee(studentChallan.Id, challanId);
                    var extraChargesDetail = feePlanRepo.GetStudentExtraChargesByStudent(studentChallan.StdId, month);


                    LogWriter.WriteLog("Getting Challan Detail With Count : " + (challanDetail == null ? 0 : challanDetail.Count));
                    foreach (ChallanDetailViewModel detail in challanDetail)
                    {
                        if (detail.Amount > 0)
                        {
                            string[] tempObj = new string[5];
                            tempObj[0] = detail.Name;
                            tempObj[1] = detail.Amount.ToString();
                            tempObj[2] = "1";
                            tempObj[3] = detail.HeadId.ToString();
                            tempObj[4] = "";
                            allownceList.Add(tempObj);
                            total += (int)detail.Amount;
                        }
                    }

                    LogWriter.WriteLog("Getting Arrear Detail With Count : " + (arrearDetail == null ? 0 : arrearDetail.Count));
                    arrearCount = arrearDetail.Where(x => x.ArrearAmount > 0).Count();
                    if (arrearCount > 0 || (lastMonthFee != null && lastMonthFee.Count > 0))
                    {
                        string[] tempObj4 = new string[5];
                        tempObj4[0] = "Arrears";
                        tempObj4[1] = "";
                        tempObj4[2] = "";
                        tempObj4[3] = "";
                        tempObj4[4] = "";
                        allownceList.Add(tempObj4);
                    }

                    LogWriter.WriteLog("Previous Month Issue Challan Status : " + (prevIssueChallan == null ? 0 : 1));
                    if (prevIssueChallan == null)
                    {
                        foreach (FeeArrearViewModel detail in arrearDetail)
                        {
                            if (detail.ArrearAmount > 0)
                            {
                                string[] tempObj = new string[5];
                                tempObj[0] = detail.HeadName;
                                tempObj[1] = detail.ArrearAmount.ToString();
                                tempObj[2] = "2";
                                tempObj[3] = detail.FeeHeadId.ToString();
                                tempObj[4] = "";
                                allownceList.Add(tempObj);
                                total += (int)detail.ArrearAmount;
                            }
                        }
                    }


                    LogWriter.WriteLog("Getting Last Month Arrear Detail With Count : " + (lastMonthFee == null ? 0 : lastMonthFee.Count));
                    foreach (FeeArrearViewModel detail in lastMonthFee)
                    {
                        if (detail.ArrearAmount > 0)
                        {
                            string[] tempObj = new string[5];
                            tempObj[0] = detail.HeadName;
                            tempObj[1] = detail.ArrearAmount.ToString();
                            tempObj[2] = "2";
                            tempObj[3] = detail.FeeHeadId.ToString();
                            tempObj[4] = "";
                            allownceList.Add(tempObj);
                            total += (int)detail.ArrearAmount;
                        }
                    }

                    LogWriter.WriteLog("Getting Extra Charges Detail With Count : " + (extraChargesDetail == null ? 0 : extraChargesDetail.Count));
                    if (extraChargesDetail.Count > 0)
                    {
                        string[] tempObj7 = new string[5];
                        tempObj7[0] = "Extra Charges";
                        tempObj7[1] = "";
                        tempObj7[2] = "";
                        tempObj7[3] = "";
                        tempObj7[4] = "";
                        allownceList.Add(tempObj7);
                    }

                    foreach (StudentExtraChargesDetail detail in extraChargesDetail)
                    {
                        if (detail.HeadAmount > 0)
                        {
                            string[] tempObj = new string[5];
                            tempObj[0] = detail.FeeHead.Name;
                            tempObj[1] = detail.HeadAmount.ToString();
                            tempObj[2] = "3";
                            tempObj[3] = detail.HeadId.ToString();
                            tempObj[4] = "";
                            allownceList.Add(tempObj);
                            total += (int)detail.HeadAmount;
                        }
                    }
                }
                else
                {
                    List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail((int)issuedChalan.Id);
                    LogWriter.WriteLog("Getting Paid Challan Detail With Count : " + (detailList == null ? 0 : detailList.Count));
                    bool arrearsType = false, chargesType = false;
                    foreach (IssueChalanDetail detail in detailList)
                    {
                        if (detail.Type == 2 && arrearsType == false)
                        {
                            arrearsType = true;

                            string[] tempObj4 = new string[5];
                            tempObj4[0] = "Arrears";
                            tempObj4[1] = "";
                            tempObj4[2] = "";
                            tempObj4[3] = "";
                            tempObj4[4] = "";
                            allownceList.Add(tempObj4);
                        }

                        if (detail.Type == 3 && chargesType == false)
                        {
                            chargesType = true;
                            string[] tempObj4 = new string[5];
                            tempObj4[0] = "Extra Charges";
                            tempObj4[1] = "";
                            tempObj4[2] = "";
                            tempObj4[3] = "";
                            tempObj4[4] = "";
                            allownceList.Add(tempObj4);
                        }

                        detail.PayAmount = (detail.PayAmount == null ? 0 : detail.PayAmount);
                        string[] tempObj2 = new string[5];
                        tempObj2[0] = detail.FeeHead.Name;
                        tempObj2[1] = (detail.TotalAmount - (detail.Discount == null ? 0 : detail.Discount)).ToString();
                        tempObj2[2] = detail.Type.ToString();
                        tempObj2[3] = detail.FeeHeadId.ToString();
                        tempObj2[4] = detail.PayAmount.ToString();
                        allownceList.Add(tempObj2);

                        total += (int)detail.TotalAmount;
                        payTotal += (int)detail.PayAmount;
                        discount += (int)(detail.Discount == null ? 0 : detail.Discount);
                    }
                }

                string[] tempObj12 = new string[5];
                tempObj12[0] = "Total";
                tempObj12[1] = (total - discount).ToString();
                tempObj12[2] = "";
                tempObj12[3] = "";
                tempObj12[4] = payTotal.ToString();
                allownceList.Add(tempObj12);
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
        public JsonResult GetPaidChallanDetailHelper(int challanId)
        {
            List<string[]> allownceList = new List<string[]>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Getting Paid Challan Helper Detail for the Challan Id : " + challanId);
                IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByChalanId(challanId);
                int total = 0, payTotal = 0; ;
                var arrearDetail = feePlanRepo.GetPendingChallanDetailModel((int)issuedChalan.ChallanToStdId, issuedChalan.ForMonth);

                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail((int)issuedChalan.Id).OrderBy(x => x.Type).ToList();
                bool arrearsType = false, chargesType = false;
                LogWriter.WriteLog("Getting Paid Challan Detail With Count : " + (detailList == null ? 0 : detailList.Count));
                foreach (IssueChalanDetail detail in detailList)
                {

                    if (detail.Type == 3 && chargesType == false)
                    {
                        chargesType = true;
                        string[] tempObj4 = new string[6];
                        tempObj4[0] = "Extra Charges";
                        tempObj4[1] = "";
                        tempObj4[2] = "";
                        tempObj4[3] = "";
                        tempObj4[4] = "";
                        tempObj4[5] = "";
                        allownceList.Add(tempObj4);
                    }

                    if (detail.Type == 2 && arrearsType == false)
                    {
                        arrearsType = true;
                        string[] tempObj4 = new string[6];
                        tempObj4[0] = "Arrears (Current Month)";
                        tempObj4[1] = "";
                        tempObj4[2] = "";
                        tempObj4[3] = "";
                        tempObj4[4] = "";
                        tempObj4[5] = "";
                        allownceList.Add(tempObj4);
                    }

                    detail.PayAmount = (detail.PayAmount == null ? 0 : detail.PayAmount);
                    string[] tempObj2 = new string[6];
                    tempObj2[0] = detail.FeeHead.Name;
                    if ((detail.Discount == null ? 0 : detail.Discount) > 0)
                    {
                        tempObj2[0] += " (Discount :: " + (detail.Discount == null ? 0 : detail.Discount) + ")";
                    }
                    tempObj2[1] = detail.TotalAmount.ToString();
                    tempObj2[2] = detail.Type.ToString();
                    tempObj2[3] = detail.FeeHeadId.ToString();
                    tempObj2[4] = detail.PayAmount.ToString();
                    tempObj2[5] = detail.ID.ToString();
                    allownceList.Add(tempObj2);

                    total += (int)detail.TotalAmount;
                    payTotal += (int)detail.PayAmount;
                }

                LogWriter.WriteLog("Getting Arrears Detail With Count : " + (arrearDetail == null ? 0 : arrearDetail.Count));
                if (arrearDetail.Count > 0)
                {
                    string[] tempObj4 = new string[6];
                    tempObj4[0] = "Arrears";
                    tempObj4[1] = "";
                    tempObj4[2] = "";
                    tempObj4[3] = "";
                    tempObj4[4] = "";
                    tempObj4[5] = "";
                    allownceList.Add(tempObj4);
                }

                foreach (IssuedChallanDetailModel detail in arrearDetail)
                {
                    if (detail.PendingAmount > 0)
                    {
                        string[] tempObj = new string[6];
                        tempObj[0] = detail.HeadDetail;
                        if ((detail.Discount == null ? 0 : detail.Discount) > 0)
                        {
                            tempObj[0] += " (Discount :: " + (detail.Discount == null ? 0 : detail.Discount) + ")";
                        }
                        tempObj[1] = detail.PendingAmount.ToString();
                        tempObj[2] = "2";
                        tempObj[3] = detail.FeeHeadId.ToString();
                        tempObj[4] = "";
                        tempObj[5] = detail.IssueChallanDetailId.ToString();
                        allownceList.Add(tempObj);
                        total += (int)detail.PendingAmount;
                    }
                }

                string[] tempObj12 = new string[6];
                tempObj12[0] = "Total";
                tempObj12[1] = total.ToString();
                tempObj12[2] = "";
                tempObj12[3] = "";
                tempObj12[4] = issuedChalan.PaidFlag == 0 ? "" : payTotal.ToString();
                tempObj12[5] = "";
                allownceList.Add(tempObj12);

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
        public void SaveIssueChallanDetail(List<IssueChalanDetail> detailList)
        {
            Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST] = detailList;
        }

        [HttpPost]
        public JsonResult GetChallanDetail(int challanId)
        {
            List<string[]> allownceList = new List<string[]>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Getting Issue Challan Detail for the Challan Id : " + challanId);
                int monthId = 0, yearId = 0;
                string currentMonth = "";
                if (Session[ConstHelper.ADMISSION_NO_CHALLAN_MONTH] != null)
                {
                    currentMonth = (string)Session[ConstHelper.ADMISSION_NO_CHALLAN_MONTH];
                }

                monthId = (Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID] == null ? 0 : (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID]);
                yearId = (Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID] == null ? 0 : (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID]);
                if (currentMonth.Length > 0)
                    currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);

                var studentChallan = feePlanRepo.GetStudentChallanDetailById(challanId);
                int chId = studentChallan.ChallanId;
                var challanDetail = feePlanRepo.GetChallDetailByChallanId(chId);
                var arrearDetail = feePlanRepo.GetStudentArrearDetail(studentChallan.StdId);
                var lastMonthFee = feePlanRepo.GetLastMonthsUnPaidFee(studentChallan.Id, challanId);
                var extraChargesDetail = feePlanRepo.GetStudentExtraChargesByStudent(studentChallan.StdId, currentMonth);
                IssuedChallan prevIssueChallan = feePlanRepo.GetPreviousIssuedChallan((int)studentChallan.Id, (int)challanId);

                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail(challanId).OrderBy(x => x.Type).ToList();
                List<IssueChalanDetail> CurrentExtras = detailList.Where(x => x.Type == 3).ToList();

                int total = 0;
                LogWriter.WriteLog("Getting Challan Detail With Count : " + (challanDetail == null ? 0 : challanDetail.Count));
                foreach (ChallanDetailViewModel detail in challanDetail)
                {
                    if (detail.Amount > 0)
                    {
                        string[] tempObj = new string[3];
                        tempObj[0] = detail.Name;
                        tempObj[1] = detail.Amount.ToString();
                        tempObj[2] = "1";
                        allownceList.Add(tempObj);
                        total += (int)detail.Amount;
                    }
                }

                int arrearCount = arrearDetail.Where(x => x.ArrearAmount > 0).Count();
                int extraCount = CurrentExtras.Where(x => (int)x.TotalAmount - (int)(x.PayAmount == null ? 0 : x.PayAmount) - (int)(x.Discount == null ? 0 : x.Discount) > 0).Count();

                if (arrearCount > 0 || (lastMonthFee != null && lastMonthFee.Count > 0))
                {
                    string[] tempObj4 = new string[3];
                    tempObj4[0] = "Arrears";
                    tempObj4[1] = "";
                    tempObj4[2] = "";
                    allownceList.Add(tempObj4);
                }

                LogWriter.WriteLog("Previous Month Issue Challan Status : " + (prevIssueChallan == null ? 0 : 1));
                if (prevIssueChallan == null)
                {
                    LogWriter.WriteLog("Getting Arrears Detail With Count : " + (arrearDetail == null ? 0 : arrearDetail.Count));
                    foreach (FeeArrearViewModel detail in arrearDetail)
                    {
                        if (detail.ArrearAmount > 0)
                        {
                            string[] tempObj = new string[3];
                            tempObj[0] = detail.HeadName;
                            tempObj[1] = detail.ArrearAmount.ToString();
                            tempObj[2] = "2";
                            allownceList.Add(tempObj);
                            total += (int)detail.ArrearAmount;
                        }
                    }
                }

                LogWriter.WriteLog("Getting Last Month Arrears Detail With Count : " + (lastMonthFee == null ? 0 : lastMonthFee.Count));
                foreach (FeeArrearViewModel detail in lastMonthFee)
                {
                    if (detail.ArrearAmount > 0)
                    {
                        string[] tempObj = new string[3];
                        tempObj[0] = detail.HeadName;
                        tempObj[1] = detail.ArrearAmount.ToString();
                        tempObj[2] = "2";
                        allownceList.Add(tempObj);
                        total += (int)detail.ArrearAmount;
                    }
                }


                if (extraChargesDetail.Count > 0 && prevIssueChallan == null || extraCount > 0)
                {
                    //string[] tempObj6 = new string[3];
                    //tempObj6[0] = "";
                    //tempObj6[1] = "";
                    //tempObj6[2] = "";
                    //allownceList.Add(tempObj6);

                    string[] tempObj7 = new string[3];
                    tempObj7[0] = "Extra Charges";
                    tempObj7[1] = "";
                    tempObj7[2] = "";
                    allownceList.Add(tempObj7);

                    //string[] tempObj8 = new string[3];
                    //tempObj8[0] = "";
                    //tempObj8[1] = "";
                    //tempObj8[2] = "";
                    //allownceList.Add(tempObj8); 
                }

                if (prevIssueChallan == null)
                {
                    LogWriter.WriteLog("Getting Extra Chrages Detail With Count : " + (extraChargesDetail == null ? 0 : extraChargesDetail.Count));
                    foreach (StudentExtraChargesDetail detail in extraChargesDetail)
                    {
                        if (detail.HeadAmount > 0)
                        {
                            string[] tempObj = new string[3];
                            tempObj[0] = detail.FeeHead.Name;
                            tempObj[1] = detail.HeadAmount.ToString();
                            tempObj[2] = "3";
                            allownceList.Add(tempObj);
                            total += (int)detail.HeadAmount;
                        }
                    }
                }
                else
                {
                    LogWriter.WriteLog("Getting Current ExtrasDetail With Count : " + (CurrentExtras == null ? 0 : CurrentExtras.Count));
                    foreach (var detail in CurrentExtras)
                    {
                        int pendingAmount = (int)detail.TotalAmount - (int)(detail.PayAmount == null ? 0 : detail.PayAmount) - (int)(detail.Discount == null ? 0 : detail.Discount);
                        if (pendingAmount > 0)
                        {
                            string[] tempObj = new string[3];
                            tempObj[0] = detail.FeeHead.Name;
                            tempObj[1] = pendingAmount.ToString();
                            tempObj[2] = "3";
                            allownceList.Add(tempObj);
                            total += (int)pendingAmount;
                        }
                    }
                }


                //string[] tempObj1 = new string[3];
                //tempObj1[0] = "";
                //tempObj1[1] = "";
                //tempObj1[2] = "";
                //allownceList.Add(tempObj1);

                string[] tempObj2 = new string[3];
                tempObj2[0] = "Total";
                tempObj2[1] = total.ToString();
                tempObj2[2] = "";
                allownceList.Add(tempObj2);


                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return Json(allownceList);
        }

        [HttpGet]

        public void ResetAnnualCharges()
        {
            IsAnnualCharges = true;
        }

        [HttpGet]

        public void setUnpaid()
        {
            IsUnpaid = true;
        }


        private FileResult PrintSingleIssueChalan(int[] ChalanIds, string AccountTypeId, string FinanceAccountId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Printing Single Issue Challan With Count : " + (ChalanIds == null ? 0 : ChalanIds.Count()));
                string className = "";
                string secName = "";
                string currentMonth = "";
                string financeAccountType = AccountTypeId;
                string financeAccountId = FinanceAccountId;

                IList SrNoList = new List<string>();
                IList rollNoList = new List<string>();
                IList studentNameList = new List<string>();
                IList chalanIdList = new List<int>();
                IList studentIdList = new List<int>();
                IList studentChallanIdList = new List<int>();
                IList issueChallanIdList = new List<int>();
                IList actualFineList = new List<int>();
                IList newFineList = new List<int>();
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                IssueChallanConfig iConfig = feePlanRepo.GetFine(branchId);
                SchoolConfig schoolConfig = securityRepo.GetSchoolConfigByBranchId(branchId);
                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];

                LogWriter.WriteLog("Gathering Printing Information for Single Issue Challan");
                foreach (int id in ChalanIds)
                {
                    IssuedChallan issueChalan = feePlanRepo.GetIssueChallanByChalanId(id);
                    ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailById((int)issueChalan.Id);
                    var student = studentRepo.GetStudentById(detail.StdId);

                    SrNoList.Add(student.AdmissionNo);
                    rollNoList.Add(student.RollNumber);
                    if (schoolConfig.SchoolName.ToLower().Contains("nusrat"))
                        studentNameList.Add(student.Name);
                    else
                        studentNameList.Add(student.Name + ", " + student.FatherName);

                    studentIdList.Add(student.id);
                    chalanIdList.Add(detail.ChallanId);
                    studentChallanIdList.Add(detail.Id);
                    issueChallanIdList.Add(id);
                    //actualFineList.Add(GetStudentTotalFine(iConfig, issueChalan));
                    actualFineList.Add(GetStudentTotalFine(student.id, monthId, (2016 + yearId - 1), branchId, DateTime.Parse(issueChalan.DueDate.ToString())));
                    var classSection = classSecRepo.GetClassSectionsModelById((int)student.ClassSectionId);
                    className = classSection.ClassName;
                    secName = classSection.SectionName;
                    currentMonth = issueChalan.ForMonth;
                    //financeAccountType = ((int) issueChalan.PayedToType).ToString();
                    //financeAccountId = ((int) issueChalan.PayedTo).ToString();
                }


                LogWriter.WriteLog("Printing the Issue Challan for Single Issue Challan");
                LogWriter.WriteLog(string.Format("Single Issue Challan Config Type : {0}, Student Per Challan : {1}", iConfig.Type, iConfig.StudentPerChallan));
                PdfDocument document = new PdfDocument();
                if (schoolConfig.SchoolName.ToLower().Contains("nusrat"))
                {
                    AlNusratChallanPdf pdf = new AlNusratChallanPdf();
                    document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth);
                }
                else
                {
                    if (iConfig.Type != null && iConfig.Type == "3")
                    {
                        BuildThreeFeeChallan pdf = new BuildThreeFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth);
                    }
                    else if (iConfig.Type != null && iConfig.Type == "4")
                    {
                        BuildFourFeeChallan pdf = new BuildFourFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth, branchId);
                    }
                    else if (iConfig.StudentPerChallan == 1)
                    {
                        BuildFeeChallan pdf = new BuildFeeChallan();
                        //document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth, iConfig.Text1, iConfig.Text2, iConfig.Text3, iConfig.Text4);
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth);
                    }
                    else
                    {
                        BuildTwoFeeChallan pdf = new BuildTwoFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth, branchId);
                    }
                }

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
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = className + "-" + secName + "-" + currentMonth + "-" + DateTime.Now.ToString() + ".pdf" };
            //}

        }

        private FileResult PrintIssueChalan(int[] ChalanIds, string financeAccountType, string financeAccountId, DateTime DueDate, string Fine, string currentMonth)
        {

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Printing Issue Challan With Count : " + (ChalanIds == null ? 0 : ChalanIds.Count()));
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];

                var classSection = classSecRepo.GetClassSectionsModelById(classSectionId);
                string className = classSection.ClassName;
                string secName = classSection.SectionName;
                int tempChalanId = ChalanIds[0];
                IssuedChallan chalan = feePlanRepo.GetIssueChallanById(tempChalanId);
                //BankAccount account = db.BankAccounts.Where(x => x.Id == chalan.PayedTo).FirstOrDefault();
                //string accountNo = account.AccountNo, bankName = account.BankName;

                IList SrNoList = new List<string>();
                IList rollNoList = new List<string>();
                IList studentNameList = new List<string>();
                IList chalanIdList = new List<int>();
                IList studentIdList = new List<int>();
                IList studentChallanIdList = new List<int>();
                IList issueChallanIdList = new List<int>();
                IList actualFineList = new List<int>();

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                IssueChallanConfig issueChalanConfig = feePlanRepo.GetFine(branchId);

                SchoolConfig schoolConfig = securityRepo.GetSchoolConfigByBranchId(branchId);
                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];

                LogWriter.WriteLog("Gathering Printing Information");
                foreach (int id in ChalanIds)
                {
                    IssuedChallan issueChalan = feePlanRepo.GetIssueChallanByChalanId(id);
                    ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailById((int)issueChalan.Id);
                    var student = studentRepo.GetStudentById(detail.StdId);

                    SrNoList.Add(student.AdmissionNo);
                    rollNoList.Add(student.RollNumber);
                    if (schoolConfig.SchoolName.ToLower().Contains("nusrat"))
                        studentNameList.Add(student.Name);
                    else
                        studentNameList.Add(student.Name + ", " + student.FatherName);

                    studentIdList.Add(student.id);
                    chalanIdList.Add(detail.ChallanId);
                    studentChallanIdList.Add(detail.Id);
                    issueChallanIdList.Add(id);

                    IssuedChallan issueChalanForFine = feePlanRepo.GetIssueChallanByChalanId(int.Parse(issueChalan.Id.ToString()));
                    int calculatedFine = GetStudentTotalFine(student.id, monthId, (2016 + yearId - 1), branchId, DateTime.Parse(issueChalanForFine.DueDate.ToString()));
                    if (issueChalanForFine != null && issueChalanForFine.Fine != null && issueChalanForFine.Fine.ToString().Length > 0)
                    {
                        int savedFine = int.Parse(issueChalanForFine.Fine.ToString());
                        calculatedFine = GetStudentTotalFine(student.id, monthId, (2016 + yearId - 1), branchId, DateTime.Parse(issueChalanForFine.DueDate.ToString()));
                        if (calculatedFine >= savedFine)
                        {
                            calculatedFine = calculatedFine - savedFine;
                        }
                        else
                        {
                            calculatedFine = 0;
                        }
                    }

                    actualFineList.Add(calculatedFine);
                }

                LogWriter.WriteLog("Printing the Issue Challan");
                LogWriter.WriteLog(string.Format("Challan Config Type : {0}, Student Per Challan : {1}", issueChalanConfig.Type, issueChalanConfig.StudentPerChallan));
                PdfDocument document = new PdfDocument();
                if (schoolConfig.SchoolName.ToLower().Contains("nusrat"))
                {
                    AlNusratChallanPdf pdf = new AlNusratChallanPdf();
                    document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth);
                }
                else
                {
                    if (issueChalanConfig.Type != null && issueChalanConfig.Type == "3")
                    {
                        BuildThreeFeeChallan pdf = new BuildThreeFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth);
                    }
                    else if (issueChalanConfig.Type != null && issueChalanConfig.Type == "4")
                    {
                        BuildFourFeeChallan pdf = new BuildFourFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth, branchId);
                    }
                    else if (issueChalanConfig.StudentPerChallan == 1)
                    {
                        BuildFeeChallan pdf = new BuildFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth);
                        //document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth, issueChalanConfig.Text1, issueChalanConfig.Text2, issueChalanConfig.Text3, issueChalanConfig.Text4);
                    }
                    else
                    {
                        BuildTwoFeeChallan pdf = new BuildTwoFeeChallan();
                        document = pdf.CreatePdf(className, secName, SrNoList, rollNoList, studentNameList, chalanIdList, issueChallanIdList, studentIdList, financeAccountType, financeAccountId, actualFineList, currentMonth, branchId);
                    }
                }

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
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = className + "-" + secName + "-" + currentMonth + "-" + DateTime.Now.ToString() + ".pdf" };
            //}

        }

        private bool IsPrintSave(int[] ChalanIds, string currentMonth)
        {
            foreach (int id in ChalanIds)
            {
                IssuedChallan isseChalan = feePlanRepo.GetIssueChallanByChalanId(id);
                if (isseChalan.IssuedFlag == 0)
                    return false;
            }
            return true;
        }

        private bool isUnpaidActionSaved(int[] ChalanIds, string currentMonth)
        {
            foreach (int id in ChalanIds)
            {
                IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                if (issuedChalan.PaidFlag == 0)
                    return false;
            }
            return true;
        }
        private void SaveUnPaidChalan(int[] ChalanIds, int[] Indexes, DateTime PaidDate, string[] PaidAmount, string[] Fine, string[] ChalanAmount, string[] Balance, string[] Advance)
        {
            int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
            int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("UnPaid Challan With Count : " + (ChalanIds == null ? 0 : ChalanIds.Count()));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);
                if (!CheckPaidStatus(ChalanIds, currentMonth))
                {
                    paidErrorCode = 109;
                }
                else
                {
                    if (IsNoSelected(ChalanIds) == false)
                    {
                        if (isUnpaidActionSaved(ChalanIds, currentMonth) == false)
                        {
                            paidErrorCode = 72;
                            return;
                        }
                        for (int i = 0; i < ChalanIds.Count(); i++)
                        {
                            int id = ChalanIds[i];
                            int paidAmount = 0;
                            int chalanAmount = int.Parse(ChalanAmount[Indexes[i]]);
                            paidAmount = int.Parse(PaidAmount[Indexes[i]]);
                            int balance = int.Parse(Balance[Indexes[i]]);
                            int advance = int.Parse(Advance[Indexes[i]]);

                            IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                            DateTime paidDate = (DateTime)issuedChalan.PaidDate;
                            //makeFeeAdjustment(issuedChalan.ChallanStudentDetail.Student.id, 0 - (int)issuedChalan.Amount, chalanAmount, id, balance, advance, 1);

                            issuedChalan.Amount = issuedChalan.ChalanAmount;
                            issuedChalan.PaidDate = issuedChalan.DueDate;
                            issuedChalan.PaidFlag = 0;
                            issuedChalan.Fine = 0;
                            feePlanRepo.UpdateIssueChallan(issuedChalan);
                            LogWriter.WriteLog("UnPaid Challan Saved Succesfully Id : " + issuedChalan.Id);
                            FeeBalance feebalance = feePlanRepo.GetFeeBalanceByStudentId(issuedChalan.ChallanStudentDetail.StdId);

                            ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailById((int)issuedChalan.Id);
                            var student = studentRepo.GetStudentById(detail.StdId);

                            LogWriter.WriteLog("Creating Journal Entry for the Unpaid Challan");
                            CreateUnPaidJournalEntry(issuedChalan.Id.ToString(), DateTime.Now, paidAmount, "", "JE", student, currentMonth, issuedChalan.PayedTo.ToString(), branchId);
                            LogWriter.WriteLog("Saving Unpaid Challan Detail");
                            SaveUnpaidChallanDetail((int)issuedChalan.Id, (int)issuedChalan.PayedTo, (int) issuedChalan.PayedToType, paidDate, issuedChalan.ForMonth, feebalance.Id);
                        }
                        paidErrorCode = 41;
                    }
                    else
                    {
                        if (ChalanIds == null || ChalanIds.Length == 0)
                            paidErrorCode = 71;
                        else
                            paidErrorCode = 61;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                paidErrorCode = 421;
            }
        }

        private void SaveUnpaidChallanDetail(int IssuedChallanId, int PayedTo, int payToType, DateTime paidDate, string month, int FeeBalanceId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var issueChallanDetail = feePlanRepo.GetIssueChallanDetail(IssuedChallanId);

                foreach (var detail in issueChallanDetail)
                {
                    PaymentHistory history = new PaymentHistory();
                    history.PaymentType = detail.Type;
                    history.IssueChallanId = IssuedChallanId;
                    history.FeeHeadId = detail.FeeHeadId;
                    history.PayAmount = -1 * (detail.PayAmount == null ? 0 : detail.PayAmount);
                    history.CreatedOn = DateTime.Now;
                    history.FeeBalanceId = FeeBalanceId;
                    history.Description = "Amount : " + history.PayAmount + " is unpaid paid for Student Challan for month (" + month + ") in month (" + month + ")";
                    history.PaidDate = paidDate;
                    history.PayedTo = PayedTo;
                    history.PayedToType = payToType;
                    history.ForMonth = month;
                    if (history.PayAmount != 0)
                        feePlanRepo.AddPaymentHistory(history);

                    detail.PayAmount = 0;
                    feePlanRepo.UpdateIssueChalanDetail(detail);

                    int arrearAmount = (int)issueChallanDetail.Where(x => x.FeeHeadId == detail.FeeHeadId && x.Type == 2).Sum(x => (x.TotalAmount - x.PayAmount - (x.Discount == null ? 0 : x.Discount)));
                    if (arrearAmount > 0)
                    {
                        FeeArrearsDetail arrear = feePlanRepo.GetFeeArrearDetail(FeeBalanceId, (int)detail.FeeHeadId);
                        if (arrear == null)
                        {
                            arrear = new FeeArrearsDetail();
                            arrear.HeadAmount = arrearAmount;
                            arrear.FeeBalanceId = FeeBalanceId;
                            arrear.FeeHeadId = detail.FeeHeadId;
                            arrear.CreatedOn = DateTime.Now;
                            arrear.UpdatedOn = DateTime.Now;
                            feePlanRepo.SaveFeeArrearDetail(arrear);
                        }
                        else
                        {
                            arrear.HeadAmount = arrear.HeadAmount + arrearAmount;
                            feePlanRepo.UpdateFeeArrearDetail(arrear);
                        }
                    }
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
        [ValidateAntiForgeryToken]
        public ActionResult SearchFastPay(string ChalanNo = "", string RollNo = "", string Name = "", string FatherName = "", string FatherCnic = "", string AdmissionNo = "")
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FAST_PAY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int challanNo = 0;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                LogWriter.WriteLog("Searching the fast pay with the following paramaters");
                LogWriter.WriteLog(string.Format("RollNo : {0}, Name : {1}", RollNo, Name));
                LogWriter.WriteLog(string.Format("FatherName : {0}, FatherCnic : {1}, branchId : {2}", FatherName, FatherCnic, branchId));
                LogWriter.WriteLog(string.Format("AdmissionNo : {0}, Challan NO : {1}", AdmissionNo, ChalanNo));

                if (ChalanNo != null && ChalanNo.Trim().Length > 0)
                {
                    challanNo = int.Parse(ChalanNo.Trim());
                }
                //string.IsNullOrEmpty(ChalanNo) == true ? 0 : int.Parse(ChalanNo);

                var challanObject = feePlanRepo.SearchFastPaidChallan(RollNo, Name, FatherName, FatherCnic, branchId, AdmissionNo, challanNo);
                Session[ConstHelper.SEARCH_FAST_PAY] = challanObject;

                List<IssuedChallanViewModel> sixMonth = new List<IssuedChallanViewModel>();
                if (challanObject != null)
                {
                    sixMonth = feePlanRepo.GetStudentSixMonthPaymentDetail(challanObject.StudentChallanId, challanObject.Id);
                }
                Session[ConstHelper.SIX_MONTH_DETAIL] = sixMonth;
                LogWriter.WriteLog("Last Six Month History Count : " + (sixMonth == null ? 0 : sixMonth.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("FastPay");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchMonthlyWaveOff(string RollNo = null, string Name = null, string FatherName = null, string FatherCnic = null, string AdmissionNo = null)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FAST_PAY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteLog("Searching the fast pay with the following paramaters");
                LogWriter.WriteLog(string.Format("RollNo : {0}, Name : {1}, AdmissionNo : {2}", RollNo, Name, AdmissionNo));
                LogWriter.WriteLog(string.Format("FatherName : {0}, FatherCnic : {1}, branchId : {2}", FatherName, FatherCnic, branchId));

                var studentModel = studentRepo.SearchStudentsModel(RollNo, Name, FatherName, FatherCnic, AdmissionNo, branchId);
                var feeDetail = feePlanRepo.GetMonthlyWaveOffFeeDetail(studentModel.Id);
                MonthlyWaveOffModel model = new MonthlyWaveOffModel();
                model.Student = studentModel;
                model.ChallanDetail = feeDetail;
                Session[ConstHelper.WAVE_OFF_MODEL] = model;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("MonthlyWaveOff");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePaidChalan(int[] ChalanIds, int[] Indexes, DateTime PaidDate, string[] PaidAmount, string[] Fine, string[] ChalanAmount, string[] Balance, string[] Advance, string FinanceAccountId, string AccountTypeId, string FullChallanPayment)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
           
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Check the IsUnpaid Flag : " + IsUnpaid);

                if (IsUnpaid == true)
                {
                    IsUnpaid = false;
                    SaveUnPaidChalan(ChalanIds, Indexes, PaidDate, PaidAmount, Fine, ChalanAmount, Balance, Advance);
                    return RedirectToAction("PaidChallan", new { id = -59 });
                }

                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                if (!CheckPaidStatus(ChalanIds, currentMonth))
                {
                    paidErrorCode = 109;
                }
                else
                {
                    LogWriter.WriteLog("Check if the Challans are safe to save");
                    if (IsNoSelected(ChalanIds) == false && IsNoAmountEntered(Indexes, PaidAmount) == true)
                    {
                        LogWriter.WriteLog("Paid Challan With Count : " + (ChalanIds == null ? 0 : ChalanIds.Count()));
                        for (int i = 0; i < ChalanIds.Count(); i++)
                        {
                            int id = ChalanIds[i];
                            int paidAmount = int.Parse(PaidAmount[Indexes[i]]);
                            int chalanAmount = int.Parse(ChalanAmount[Indexes[i]]);
                            int balance = int.Parse(Balance[Indexes[i]]);
                            int advance = int.Parse(Advance[Indexes[i]]);
                            int fine = 0;
                            if (Fine[Indexes[i]] != null && Fine[Indexes[i]].Length > 0)
                                fine = int.Parse(Fine[Indexes[i]]);

                            IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                            //if (issuedChalan.PaidFlag == 1)
                            //    makeUpdateFeeAdjustment(issuedChalan.ChallanStudentDetail.Student.id, paidAmount, chalanAmount, (int)issuedChalan.Amount, id, balance, advance);
                            //else
                            //    makeFeeAdjustment(issuedChalan.ChallanStudentDetail.Student.id, paidAmount, chalanAmount, id, balance, advance);
                            int paidFlag = 0;
                            paidFlag = (int)issuedChalan.PaidFlag;
                            int alreadyPaidAmount = (int)(issuedChalan.Amount == null ? 0 : issuedChalan.Amount);
                            DateTime previousDate = (DateTime) issuedChalan.PaidDate;
                            issuedChalan.Amount = paidAmount;
                            issuedChalan.PaidDate = PaidDate;
                            issuedChalan.PaidFlag = 1;
                            issuedChalan.Fine = fine;
                            issuedChalan.BranchId = branchId;
                            issuedChalan.PayedToType = int.Parse(AccountTypeId);
                            issuedChalan.PayedTo = int.Parse(FinanceAccountId);
                            feePlanRepo.UpdateIssueChallan(issuedChalan);
                            int balanceId = 0;
                            FeeBalance feebalance = feePlanRepo.GetFeeBalanceByStudentId(issuedChalan.ChallanStudentDetail.StdId);
                            if (feebalance != null)
                                balanceId = feebalance.Id;
                            //SaveIssueChallanDetail((int)issuedChalan.Id, balanceId);
                            LogWriter.WriteLog("Challan is saved, Saving the Challan Detail, Id : " + issuedChalan.Id);
                            SaveIssueChallanDetail((int)issuedChalan.Id, PaidDate, previousDate, balanceId, int.Parse(AccountTypeId), int.Parse(FinanceAccountId),  0, 0, currentMonth, int.Parse(FullChallanPayment), true);
                            ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailById((int)issuedChalan.Id);
                            var student = studentRepo.GetStudentById(detail.StdId);

                            LogWriter.WriteLog("Creating Finance Entry against the challan");
                            if (paidFlag == 0)
                            {
                                CreatePaidJournalEntry(issuedChalan.Id.ToString(), PaidDate, paidAmount, "", "JE", student, currentMonth, FinanceAccountId, branchId);
                            }
                            else
                            {
                                CreatePaidJournalEntry(issuedChalan.Id.ToString(), PaidDate, paidAmount - alreadyPaidAmount, "", "JE", student, currentMonth, FinanceAccountId, branchId, true);
                            }

                            DateTime toDayDate = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                            int totalPaymentHist = feePlanRepo.getPaymentHistorySum(feebalance.Id, toDayDate, System.DateTime.Now);

                            Student stdObj = studentRepo.GetStudentById((int)feebalance.StudentId);
                            int toatalBalance = (int)feebalance.Balance;
                            int totalPayableAmount = toatalBalance + totalPaymentHist;


                            SmsInfoProxy.sendSmsStudentFeeCollectionEvent(stdObj, issuedChalan.Amount.ToString(), totalPayableAmount.ToString(), totalPaymentHist.ToString(), toatalBalance.ToString(), System.DateTime.Now.ToString(), issuedChalan.DueDate.ToString());

                        }
                        paidErrorCode = 4;

                    }
                    else
                    {
                        if (IsNoAmountEntered(Indexes, PaidAmount) == false)
                            paidErrorCode = 81;
                        else
                        {
                            if (ChalanIds == null || ChalanIds.Length == 0)
                                paidErrorCode = 7;
                            else
                                paidErrorCode = 6;
                        }
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                paidErrorCode = 420;
            }
            if (FullChallanPayment == "1")
                return RedirectToAction("PaidChallan", new { id = -59 });
            else
                return RedirectToAction("PaidPartialChallan", new { id = -59 });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFastPaidChalan(int ChalanId, string PaidAmount, string Fine, string ChalanAmount, string Balance, string Advance, string FinanceAccountId, string AccountTypeId, int isPrint, DateTime PaidDate, bool IsLcm = false)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FAST_PAY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Saving Fast Paid Callan with Id : " + ChalanId);
                LogWriter.WriteLog("Check printing issue Challan Status : " + isPrint);
                if (isPrint == 1)
                {
                    return showReportAsPdf(ChalanId);
                }

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);


                int id = ChalanId;
                int paidAmount = int.Parse(PaidAmount);
                int chalanAmount = int.Parse(ChalanAmount);
                int tempPaidAmount = paidAmount;
                int balance = int.Parse(Balance);
                int advance = int.Parse(Advance);
                if (IsLcm)
                    tempPaidAmount = chalanAmount;
                int fine = 0;
                if (Fine != null && Fine.Length > 0)
                    fine = int.Parse(Fine);

                IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByChalanId(id);
                if (IsLcm && issuedChalan.IsLcm == false)
                {
                    balance = balance - ((int)issuedChalan.ChalanAmount - (int)issuedChalan.Amount);
                }

                string currentMonth = issuedChalan.ForMonth;

                ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailById((int)issuedChalan.Id);
                var student = studentRepo.GetStudentById(detail.StdId);

                if (issuedChalan.PaidFlag == 1)
                    makeUpdateFeeAdjustment(student.id, tempPaidAmount, chalanAmount, (int)(issuedChalan.Amount == null ? 0 : issuedChalan.Amount), id, balance, advance);
                else
                    makeFeeAdjustment(student.id, tempPaidAmount, chalanAmount, id, balance, advance);
                int paidFlag = 0;
                paidFlag = (int)issuedChalan.PaidFlag;
                int alreadyPaidAmount = (int)(issuedChalan.Amount == null ? 0 : issuedChalan.Amount);
                DateTime previouDate = (DateTime) issuedChalan.PaidDate;
                issuedChalan.Amount = paidAmount;
                issuedChalan.PaidDate = PaidDate;
                issuedChalan.PaidFlag = 1;
                if (issuedChalan.Fine != null && issuedChalan.Fine.ToString().Length > 0)
                {
                    issuedChalan.Fine += fine;
                }
                else
                {
                    issuedChalan.Fine = fine;
                }
                issuedChalan.BranchId = branchId;
                issuedChalan.PayedToType = int.Parse(AccountTypeId);
                issuedChalan.PayedTo = int.Parse(FinanceAccountId);
                issuedChalan.IsLcm = IsLcm;
                feePlanRepo.UpdateIssueChallan(issuedChalan);
                LogWriter.WriteLog("Saved Fast Paid Challan successfully, Id : " + issuedChalan.Id);
                int balanceId = 0;

                FeeBalance feebalance = feePlanRepo.GetFeeBalanceByStudentId(student.id);
                if (feebalance != null)
                    balanceId = feebalance.Id;
                //SaveIssueChallanDetail((int)issuedChalan.Id, balanceId);
                LogWriter.WriteLog("Now saving the fast paid challan detail");
                SaveFastPaidIssueChallanDetail((int)issuedChalan.Id, PaidDate, previouDate, balanceId, int.Parse(AccountTypeId), int.Parse(FinanceAccountId), 0, 0, currentMonth, 0, true, IsLcm);
               

                LogWriter.WriteLog("Creating Journal Entry for the Fast Paid Challan");
                if (paidFlag == 0)
                {
                    CreatePaidJournalEntry(issuedChalan.Id.ToString(), PaidDate, paidAmount, "", "JE", student, currentMonth, FinanceAccountId, branchId);
                }
                else
                {
                    CreatePaidJournalEntry(issuedChalan.Id.ToString(), PaidDate, paidAmount - alreadyPaidAmount, "", "JE", student, currentMonth, FinanceAccountId, branchId, true);
                }
                paidErrorCode = 4;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                paidErrorCode = 420;
            }

            return RedirectToAction("FastPay", new { id = -59 });

        }

        private void makeUpdateFeeAdjustment(int stdId, int paidAmount, int chalanAmount, int previousPaidAmount, int studentChallanId, int balance, int advance)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Make Update Fee Adjustment for the Student : " + stdId);

                FeeBalance tempFeeBalance = feePlanRepo.GetFeeBalanceByStudentId(stdId);
                balance = (chalanAmount - paidAmount < 0 ? 0 : chalanAmount - paidAmount);
                advance = (paidAmount - chalanAmount < 0 ? 0 : paidAmount - chalanAmount);
                if (tempFeeBalance != null)
                {
                    tempFeeBalance.Balance = balance;
                    tempFeeBalance.Advance = advance;
                    feePlanRepo.UpdateFeeBalance(tempFeeBalance);
                }
                else
                {
                    FeeBalance feeBalance = new FeeBalance();
                    feeBalance.Balance = balance;
                    feeBalance.Advance = advance;
                    feeBalance.StudentId = stdId;
                    feePlanRepo.AddFeeBalance(feeBalance);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }


        private void makeFeeAdjustment(int stdId, int paidAmount, int chalanAmount, int studentChallanId, int balance, int advance, int reset = 0)
        {
            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Make Fee Adjustment for the Student : " + stdId);
                FeeBalance tempFeeBalance = feePlanRepo.GetFeeBalanceByStudentId(stdId);

                balance = chalanAmount - paidAmount < 0 ? 0 : chalanAmount - paidAmount;
                advance = paidAmount - chalanAmount < 0 ? 0 : paidAmount - chalanAmount;
                if (tempFeeBalance != null)
                {
                    tempFeeBalance.Balance = balance;
                    tempFeeBalance.Advance = advance;
                    feePlanRepo.UpdateFeeBalance(tempFeeBalance);
                }
                else
                {
                    FeeBalance feeBalance = new FeeBalance();
                    feeBalance.Balance = balance;
                    feeBalance.Advance = advance;
                    feeBalance.StudentId = stdId;
                    feePlanRepo.AddFeeBalance(feeBalance);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public void SaveFastPaidIssueChallanDetail(int issuedChallanId, DateTime paidDate, DateTime previousDate, int FeeBalanceId, int payToType, int PayedTo, int challanId = 0, int studentChallanId = 0, string month = "", int fullPayment = 0, bool isPaid = false, bool isLcm = false)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Saving Fast Paid Challan Detail with ChallanId : " + issuedChallanId);
                List<IssueChalanDetail> detailList = (List<IssueChalanDetail>)Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST];
                List<int> updateIssueChallanIds = new List<int>();
                int historyCount = 0;

                LogWriter.WriteLog("Fast Paid Challan Detail with Count : " + (detailList == null ? 0 : detailList.Count));

                foreach (IssueChalanDetail detail in detailList)
                {
                    detail.PayAmount = detail.PayAmount == null ? 0 : detail.PayAmount;
                }

                LogWriter.WriteLog("Saving Fast Paid Issue Challan Detail");
                foreach (IssueChalanDetail detail in detailList)
                {
                    IssueChalanDetail temp = null;
                    if (detail.Type == 2)
                        temp = feePlanRepo.GetIssuedChallanDetailById(detail.ID);
                    else
                        temp = feePlanRepo.GetIssuedChallanDetail(issuedChallanId, (int)detail.FeeHeadId, (int)detail.Type);
                    PaymentHistory history = new PaymentHistory();
                    string forMonth = "";
                    history.PaymentType = detail.Type;
                    history.FeeType = temp.Type;
                    history.ForMonth = month;
                    history.IssueChallanId = issuedChallanId;
                    history.FeeHeadId = detail.FeeHeadId;
                    history.CreatedOn = DateTime.Now;
                    history.FeeBalanceId = FeeBalanceId;
                    if (temp == null)
                    {
                        detail.IssueChallanId = issuedChallanId;
                        detail.CreatedOn = DateTime.Now;
                        detail.UpdateOn = paidDate;
                        feePlanRepo.saveIssuedChallanDetail(detail);
                        forMonth = temp.IssuedChallan.ForMonth;
                        history.Description = "Amount : " + detail.PayAmount + " is paid for Student Challan for month (" + forMonth + ") in month (" + month + ")";
                    }
                    else
                    {
                        var challanObj = feePlanRepo.GetIssueChallanByChalanId((int)temp.IssueChallanId);
                        forMonth = challanObj.ForMonth;
                        if (temp.PayAmount != detail.PayAmount || forMonth != month)
                        {
                            int historyAmount = 0;
                            //if (temp.Type == 1)
                            //    historyAmount = (int)((detail.PayAmount == null ? 0 : detail.PayAmount) - (temp.PayAmount == null ? 0 : temp.PayAmount));
                            //else
                            //{
                            if (detail.TotalAmount == temp.TotalAmount)
                            {
                                historyAmount = (int)((detail.PayAmount == null ? 0 : detail.PayAmount) - (temp.PayAmount == null ? 0 : temp.PayAmount));
                            }
                            else
                            {
                                historyAmount = (int)(detail.PayAmount == null ? 0 : detail.PayAmount);
                            }
                            //}
                            if (detail.PayAmount > 0)
                                temp.PayAmount = temp.TotalAmount - detail.TotalAmount + detail.PayAmount - (temp.Discount == null ? 0 : temp.Discount);

                            temp.UpdateOn = paidDate;
                            feePlanRepo.UpdateIssueChalanDetail(temp);
                            if (updateIssueChallanIds.Count == 0 || updateIssueChallanIds.Contains((int)temp.IssueChallanId) == false)
                                updateIssueChallanIds.Add((int)temp.IssueChallanId);

                            if (historyAmount > 0)
                                history.Description = "Amount : " + historyAmount + " is paid for Student Challan for month (" + forMonth + ") in month (" + month + ")";
                            else
                                history.Description = "Amount : " + historyAmount + " is reversed for Student Challan for month (" + forMonth + ") in month (" + month + ")";
                            history.PayAmount = historyAmount;

                        }
                    }

                    if (forMonth != month)
                        history.ForMonth = forMonth;

                    if (isPaid)
                    {
                        int arrearAmount = 0;
                        if (!isLcm)
                            arrearAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => (x.TotalAmount - x.PayAmount));
                        int discount = feePlanRepo.GetChallanDiscount(issuedChallanId, (int)detail.FeeHeadId);
                        arrearAmount = arrearAmount - discount;

                        //int payAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => x.PayAmount);
                        //if (arrearAmount > 0)
                        //{
                        FeeArrearsDetail arrear = feePlanRepo.GetFeeArrearDetail(FeeBalanceId, (int)detail.FeeHeadId);
                        if (arrear == null)
                        {
                            arrear = new FeeArrearsDetail();
                            arrear.HeadAmount = arrearAmount;
                            arrear.FeeBalanceId = FeeBalanceId;
                            arrear.FeeHeadId = detail.FeeHeadId;
                            arrear.CreatedOn = DateTime.Now;
                            arrear.UpdatedOn = DateTime.Now;
                            feePlanRepo.SaveFeeArrearDetail(arrear);
                        }
                        else
                        {
                            arrear.HeadAmount = arrearAmount;
                            feePlanRepo.UpdateFeeArrearDetail(arrear);
                        }
                        //}
                    }

                    history.PaidDate = paidDate;
                    history.PayedTo = PayedTo;
                    history.PayedToType = payToType;
                    if (detail.PayAmount != null && detail.PayAmount != 0 && history.PayAmount != 0 && history.Description != null)
                    {
                        feePlanRepo.AddPaymentHistory(history);
                        historyCount++;
                    }
                }
                DateTime toDayDate = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                int totalPaymentHist = feePlanRepo.getPaymentHistorySum(FeeBalanceId, toDayDate, System.DateTime.Now);

                LogWriter.WriteLog("Adding Payment History for the Fast Paid Challan");
                FeeBalance feeBal = feePlanRepo.GetFeeBalanceById(FeeBalanceId);
                Student stdObj = studentRepo.GetStudentById((int)feeBal.StudentId);
                int toatalBalance = (int)feeBal.Balance;
                int totalPayableAmount = toatalBalance + totalPaymentHist;
                IssuedChallan ischObj = feePlanRepo.GetIssueChallanByIdAndMonth(issuedChallanId, month);

                if (totalPaymentHist > 0)
                    SmsInfoProxy.sendSmsStudentFeeCollectionEvent(stdObj, month, totalPayableAmount.ToString(), totalPaymentHist.ToString(), toatalBalance.ToString(), System.DateTime.Now.ToString(), ischObj.DueDate.ToString());
                if (historyCount == 0)
                {
                    var paymentHistory = feePlanRepo.SearchPaymentHistory(0, issuedChallanId, 0, 0);
                    foreach (var history in paymentHistory)
                    {
                        if (history.PaidDate.Value.Date == previousDate.Date)
                        {
                            history.PaidDate = paidDate;
                            feePlanRepo.UpdatePaymentHistory(history);
                        }
                    }
                }
                LogWriter.WriteLog("Updating amount for the Fast Paid Challan");
                foreach (int id in updateIssueChallanIds)
                {
                    feePlanRepo.UpdateIssueChalanPaidAmount(id, payToType, PayedTo);
                }
                feePlanRepo.UpdateIssueChalanPaidAmount(issuedChallanId, payToType, PayedTo);
                Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST] = null;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public void SaveIssueChallanDetail(int issuedChallanId, DateTime paidDate, DateTime previousDate, int FeeBalanceId, int payToType, int PayedTo, int challanId = 0, int studentChallanId = 0, string month = "", int fullPayment = 0, bool isPaid = false)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Saving Issue Challan Detail with ChallanId : " + issuedChallanId);
                List<IssueChalanDetail> detailList = (List<IssueChalanDetail>)Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST];

                LogWriter.WriteLog("Full Payment Status : " + fullPayment);
                if (fullPayment == 1)
                {
                    int historyCount = 0;
                    List<IssueChalanDetail> paidList = feePlanRepo.GetIssueChallanDetail(issuedChallanId);
                    LogWriter.WriteLog("Full Payment Paid List Count : " + (paidList == null ? 0 : paidList.Count));
                    LogWriter.WriteLog("Full Payment Adding Issue Challan Detail");
                    foreach (IssueChalanDetail detail in paidList)
                    {
                        PaymentHistory history = new PaymentHistory();
                        history.PaymentType = detail.Type;
                        history.IssueChallanId = issuedChallanId;
                        history.FeeHeadId = detail.FeeHeadId;
                        history.PayAmount = (detail.TotalAmount == null ? 0 : detail.TotalAmount) - (detail.PayAmount == null ? 0 : detail.PayAmount) - (detail.Discount == null ? 0 : detail.Discount);
                        history.CreatedOn = DateTime.Now;
                        history.FeeBalanceId = FeeBalanceId;
                        history.Description = "Amount : " + detail.TotalAmount + " is paid for Student Challan for month (" + month + ") in month (" + month + ")";
                        history.PaidDate = paidDate;
                        history.PayedTo = PayedTo;
                        history.PayedToType = payToType;
                        history.ForMonth = month;
                        history.FeeBalanceId = FeeBalanceId;
                        if (history.PayAmount != 0)
                        {
                            feePlanRepo.AddPaymentHistory(history);
                            historyCount++;
                        }

                        detail.PayAmount = (detail.TotalAmount == null ? 0 : detail.TotalAmount) - (detail.Discount == null ? 0 : detail.Discount);
                        detail.UpdateOn = DateTime.Now;

                        feePlanRepo.UpdateIssueChalanDetail(detail);

                        int arrearAmount = (int)paidList.Where(x => x.FeeHeadId == detail.FeeHeadId && x.Type == 2).Sum(x => (x.TotalAmount - (x.PayAmount == null ? 0 : x.PayAmount) - (x.Discount == null ? 0 : x.Discount)));
                        //int payAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => x.PayAmount);

                        if (arrearAmount > 0)
                        {
                            FeeArrearsDetail arrear = feePlanRepo.GetFeeArrearDetail(FeeBalanceId, (int)detail.FeeHeadId);
                            if (arrear == null)
                            {
                                arrear = new FeeArrearsDetail();
                                arrear.HeadAmount = arrearAmount;
                                arrear.FeeBalanceId = FeeBalanceId;
                                arrear.FeeHeadId = detail.FeeHeadId;
                                arrear.CreatedOn = DateTime.Now;
                                arrear.UpdatedOn = DateTime.Now;
                                feePlanRepo.SaveFeeArrearDetail(arrear);
                            }
                            else
                            {
                                arrear.HeadAmount = arrear.HeadAmount - arrearAmount;
                                feePlanRepo.UpdateFeeArrearDetail(arrear);
                            }
                        }
                    }

                    LogWriter.WriteLog("Full Payment Updating Issue Challan Amount");
                    feePlanRepo.UpdateIssueChalanPaidAmount(issuedChallanId, payToType, PayedTo);

                    if (historyCount == 0)
                    {
                        var paymentHistory = feePlanRepo.SearchPaymentHistory(0, issuedChallanId, 0, 0);
                        foreach (var history in paymentHistory)
                        {
                            if (history.PaidDate.Value.Date == previousDate.Date)
                            {
                                history.PaidDate = paidDate;
                                feePlanRepo.UpdatePaymentHistory(history);
                            }
                        }
                    }

                }
                else
                {

                    LogWriter.WriteLog("Issue Challan Detail List Count : " + (detailList == null ? 0 : detailList.Count));
                    LogWriter.WriteLog("Issue Challan Adding Issue Challan Detail");
                    if (detailList == null)
                    {
                        var challanDetail = feePlanRepo.GetChallDetailByChallanId(challanId);
                        foreach (ChallanDetailViewModel detail in challanDetail)
                        {
                            if (detail.Amount > 0)
                            {
                                IssueChalanDetail chDetail = new IssueChalanDetail();
                                chDetail.PayAmount = 0;
                                chDetail.IssueChallanId = issuedChallanId;
                                chDetail.FeeHeadId = detail.HeadId;
                                chDetail.TotalAmount = detail.Amount;
                                chDetail.CreatedOn = DateTime.Now;
                                chDetail.UpdateOn = paidDate;
                                chDetail.Discount = 0;
                                chDetail.Type = 1;
                                feePlanRepo.saveIssuedChallanDetail(chDetail);
                            }
                        }

                        LogWriter.WriteLog("Issue Challan Adding Student Arrears");
                        var studentChallan = feePlanRepo.GetStudentChallanDetailById(issuedChallanId);
                        var tempChallan = feePlanRepo.GetPaidChallanByChalanStudentId(studentChallan.Id);
                        if (tempChallan == null)
                        {
                            var arrearDetail = feePlanRepo.GetStudentArrearDetail(studentChallanId).Where(x => x.ArrearAmount > 0).ToList();
                            if (arrearDetail != null && arrearDetail.Count > 0)
                            {
                                foreach (FeeArrearViewModel detail in arrearDetail)
                                {
                                    IssueChalanDetail chDetail = new IssueChalanDetail();
                                    chDetail.PayAmount = 0;
                                    chDetail.IssueChallanId = issuedChallanId;
                                    chDetail.FeeHeadId = detail.FeeHeadId;
                                    chDetail.TotalAmount = detail.ArrearAmount;
                                    chDetail.CreatedOn = DateTime.Now;
                                    chDetail.UpdateOn = paidDate;
                                    chDetail.Type = 2;
                                    feePlanRepo.saveIssuedChallanDetail(chDetail);
                                }
                            }
                        }
                        else
                        {
                            var lastmonthPaidDetail = feePlanRepo.GetLastMonthUnpaidDetail(issuedChallanId);
                            if (lastmonthPaidDetail != null && lastmonthPaidDetail.Count > 0)
                            {
                            }
                            else
                            {
                                List<int> addedList = new List<int>();
                                var arrearHistory = feePlanRepo.GetLastMonthArrears(issuedChallanId, FeeBalanceId);
                                if (arrearHistory != null && arrearHistory.Count > 0)
                                {
                                    foreach (ArreartHistory history in arrearHistory)
                                    {
                                        if (addedList.Count == 0 || addedList.Contains((int)history.FeeHeadId) == false)
                                        {
                                            int payAmount = (int)arrearHistory.Where(x => x.FeeHeadId == history.FeeHeadId).Sum(x => x.PayAmount);
                                            int discount = (int)arrearHistory.Where(x => x.FeeHeadId == history.FeeHeadId).Sum(x => x.Discount == null ? 0 : x.Discount);
                                            int arrearAmount = payAmount - discount;
                                            if (arrearAmount > 0)
                                            {
                                                IssueChalanDetail chDetail = new IssueChalanDetail();
                                                chDetail.PayAmount = 0;
                                                chDetail.IssueChallanId = issuedChallanId;
                                                chDetail.FeeHeadId = history.FeeHeadId;
                                                chDetail.TotalAmount = arrearAmount;
                                                chDetail.CreatedOn = DateTime.Now;
                                                chDetail.UpdateOn = paidDate;
                                                chDetail.Type = 2;
                                                feePlanRepo.saveIssuedChallanDetail(chDetail);
                                                addedList.Add((int)history.FeeHeadId);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        LogWriter.WriteLog("Issue Challan Adding Extra Charges");
                        var extraChargesDetail = feePlanRepo.GetStudentExtraChargesByStudent(studentChallanId, month).Where(x => x.HeadAmount > 0).ToList();
                        if (extraChargesDetail != null && extraChargesDetail.Count > 0)
                        {
                            foreach (StudentExtraChargesDetail detail in extraChargesDetail)
                            {
                                IssueChalanDetail chDetail = new IssueChalanDetail();
                                chDetail.PayAmount = 0;
                                chDetail.IssueChallanId = issuedChallanId;
                                chDetail.FeeHeadId = detail.HeadId;
                                chDetail.TotalAmount = detail.HeadAmount;
                                chDetail.CreatedOn = DateTime.Now;
                                chDetail.UpdateOn = paidDate;
                                chDetail.Type = 3;
                                feePlanRepo.saveIssuedChallanDetail(chDetail);
                            }
                        }
                    }
                    else if (detailList != null && detailList.Count > 0)
                    {
                        LogWriter.WriteLog("Issue Challan Adding Payment History");
                        foreach (IssueChalanDetail detail in detailList)
                        {
                            var temp = feePlanRepo.GetIssuedChallanDetail(issuedChallanId, (int)detail.FeeHeadId, (int)detail.Type);
                            PaymentHistory history = new PaymentHistory();
                            history.PaymentType = detail.Type;
                            history.IssueChallanId = issuedChallanId;
                            history.FeeHeadId = detail.FeeHeadId;
                            history.PayAmount = (detail.PayAmount == null ? 0 : detail.PayAmount);
                            history.CreatedOn = DateTime.Now;
                            history.FeeBalanceId = FeeBalanceId;
                            history.Description = "Amount : " + detail.PayAmount + " is paid for Student Challan for month (" + month + ") in month (" + month + ")";

                            if (temp == null)
                            {
                                detail.IssueChallanId = issuedChallanId;
                                feePlanRepo.saveIssuedChallanDetail(detail);
                            }
                            else
                            {
                                if (temp.PayAmount != detail.PayAmount)
                                {
                                    temp.PayAmount = (detail.PayAmount == null ? 0 : detail.PayAmount);
                                    temp.UpdateOn = paidDate;
                                    feePlanRepo.UpdateIssueChalanDetail(temp);
                                }
                            }

                            if (isPaid)
                            {
                                int arrearAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => (x.TotalAmount - x.PayAmount - (x.Discount == null ? 0 : x.Discount)));
                                //int payAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => x.PayAmount);
                                if (arrearAmount > 0)
                                {
                                    FeeArrearsDetail arrear = feePlanRepo.GetFeeArrearDetail(FeeBalanceId, (int)detail.FeeHeadId);
                                    if (arrear == null)
                                    {
                                        arrear = new FeeArrearsDetail();
                                        arrear.HeadAmount = arrearAmount;
                                        arrear.FeeBalanceId = FeeBalanceId;
                                        arrear.FeeHeadId = detail.FeeHeadId;
                                        arrear.CreatedOn = DateTime.Now;
                                        arrear.UpdatedOn = DateTime.Now;
                                        feePlanRepo.SaveFeeArrearDetail(arrear);
                                    }
                                    else
                                    {
                                        arrear.HeadAmount = arrearAmount;
                                        feePlanRepo.UpdateFeeArrearDetail(arrear);
                                    }
                                }
                            }

                            history.PaidDate = paidDate;
                            history.PayedTo = PayedTo;
                            history.PayedToType = payToType;
                            history.ForMonth = month;
                            history.FeeBalanceId = FeeBalanceId;

                            if (detail.PayAmount > 0 && history.Description != null)
                                feePlanRepo.AddPaymentHistory(history);
                        }
                    }

                    LogWriter.WriteLog("Issue Challan Updating Issue Chalan Amount");
                    IssuedChallan tempIssueChallan = feePlanRepo.GetIssueChallanByChalanId(issuedChallanId);
                    tempIssueChallan.Amount = tempIssueChallan.IssueChalanDetails.Sum(x => x.TotalAmount);
                    tempIssueChallan.ChalanAmount = (int)tempIssueChallan.Amount;
                    feePlanRepo.UpdateIssueChallan(tempIssueChallan);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        //public ActionResult SavePaidChalan(List<IssuedChallanViewModel> model)
        //{
        //    return RedirectToAction("Index", new { id = -59 });
        //}

        private bool isCorrectIssueMonth(int issueChallanId, int year, int monthId)
        {
            bool flag = false;
            monthId--;
            monthId = monthId == 0 ? 12 : monthId;

            string prevMonth = SessionHelper.GetMonthName(monthId);
            prevMonth = prevMonth + "-" + year;
            string prevIssuedMonth = feePlanRepo.GetPreviousIssuedMonth(issueChallanId);

            if (prevIssuedMonth == prevMonth || prevIssuedMonth.Length == 0)
                flag = true;
            return flag;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMonthlyWaveOff(int[] DetailId, int[] Discount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                feePlanRepo.SaveFeeDiscount(Discount, DetailId);
                CreateDiscountJournalEntry(DetailId, Discount);
                paidErrorCode = 100;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                paidErrorCode = 420;
            }
            return RedirectToAction("MonthlyWaveOff", new { id = -59 });
        }

        private int CreateDiscountJournalEntry(int [] DetailId, int [] Discount)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int discountAmount = Discount.Sum();
                var challanDetail = feePlanRepo.GetIssuedChallanDetailById(DetailId[0]);
                var challan = feePlanRepo.GetIssueChallanByChalanId((int)challanDetail.IssueChallanId);
                var studentDetail = feePlanRepo.GetStudentChallanDetailById((int)challan.Id);
                var student = studentRepo.GetStudentById(studentDetail.StdId);
                int branchId = (int)student.BranchId;
                string studenInfo = " (" + student.RollNumber + ", " + student.Name + ", " + student.FatherName + ")";
                LogWriter.WriteLog("Adding Discount Journal Entry for the Student Id : " + student.id);
                LogWriter.WriteLog("Student Discount Count for the entry : " + (DetailId == null ? 0 : DetailId.Count()));
                JournalEntry je = new JournalEntry();
                je.CreditAmount = discountAmount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Fee discount is provided to Student : " + studenInfo;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = "";
                je.EntryType = "DISC";
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Saved Discount Journal Entry");


                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountByName("Fee Discounts", branchId);
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                var tempAccount = accountRepo.GetFinanceFifthLvlAccount(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, receivableAccountid);

                LogWriter.WriteLog("Adding Discount Journal Entry Debit Detail");
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount.Id;
                entryDetail1.Amount = discountAmount;
                entryDetail1.Description = je.CreditDescription;

                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);

                LogWriter.WriteLog("Adding Discount Journal Entry Credit Detail");
                int i = 0;
                foreach (var detailId in DetailId)
                {
                    if (Discount[i] > 0)
                    {
                        var detail = feePlanRepo.GetIssuedChallanDetailById(detailId);
                        var feeHead = feePlanRepo.GetFeeHeadById((int)detail.FeeHeadId);
                        var challanObj = feePlanRepo.GetIssueChallanByChalanId((int)detail.IssueChallanId);
                        JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                        entryDetail.EntryId = je.EntryId;
                        entryDetail.FifthLvlAccountId = tempAccount1.Id;
                        entryDetail.Amount = Discount[i];
                        entryDetail.Description = "Fee discount is provided to Student : " + studenInfo + " From Fee Fee Head : (" + feeHead.Name + ") For Month : " + challanObj.ForMonth;
                        accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                        FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, Discount[i], je.EntryId);
                        FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, Discount[i]);
                    }
                    i++;
                }

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return errorCode;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveIssueChalan(int[] ChalanIds, string FinanceAccountId, string AccountTypeId, DateTime DueDate, string AnnualFunds, string Fine = "0", string Print = "0", int PrintSingleChallan = 0)
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Save Issue Challan Count : " + (ChalanIds == null ? 0 : ChalanIds.Count()));
                if (Print == "1" && (ChalanIds == null || ChalanIds.Length == 0))
                {
                    errorCode = 10;
                    return RedirectToAction("Index", new { id = -59 });
                }
                else if (ChalanIds == null || ChalanIds.Length == 0)
                {
                    errorCode = 11;
                    return RedirectToAction("Index", new { id = -59 });
                }

                LogWriter.WriteLog("Save Issue Challan Printing Single Challan Status : " + PrintSingleChallan);
                if (PrintSingleChallan == 1)
                {
                    return PrintSingleIssueChalan(ChalanIds, AccountTypeId, FinanceAccountId);
                }

                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
                string rollNumber = (string)Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO];
                string name = (string)Session[ConstHelper.SEARCH_CHALLAN_NAME];
                string fatherName = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC];
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];
                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);

                if (Print != "1" && Print != "2")
                {
                    bool correctMonth = isCorrectIssueMonth(ChalanIds[0], 2016 + yearId - 1, monthId);
                    LogWriter.WriteLog("Save Issue Challan Correct Month Status : " + correctMonth);
                    if (correctMonth == false)
                    {
                        errorCode = 12;
                        return RedirectToAction("Index", new { id = -59 });
                    }
                }

                LogWriter.WriteLog("Save Issue Challan Is Annual Charges Status : " + IsAnnualCharges);
                if (IsAnnualCharges)
                {
                    IsAnnualCharges = false;
                    foreach (var id in ChalanIds)
                    {
                        var studentChalan = feePlanRepo.GetStudentChallanDetailById(id);
                        var charges = feePlanRepo.GetAnnualChargesByStudentIdAndMonth(studentChalan.StdId, currentMonth);
                        if (charges != null && charges.Count > 0)
                        {
                            feePlanRepo.DeleteAnnualCharges(charges[0]);
                        }

                        IssuedChallan issueChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                        //CreateResetJournalEntry(issueChalan.Id.ToString(), DateTime.Now, (int)issueChalan.ChalanAmount, "", "JE", studentChalan.Student, currentMonth);
                    }
                    errorCode = 20;
                }
                else if (Print == "1")
                {
                    if (IsNoSelected(ChalanIds) == false)
                    {
                        LogWriter.WriteLog("Save Issue Challan Checking if Printing is safe");
                        if (IsPrintSave(ChalanIds, currentMonth))
                        {
                            LogWriter.WriteLog("Save Issue Challan Printing Issue Challan");
                            return PrintIssueChalan(ChalanIds, AccountTypeId, FinanceAccountId, DueDate, Fine, currentMonth);
                        }
                        else
                            errorCode = 5;
                    }
                    else
                    {
                        if (ChalanIds == null || ChalanIds.Length == 0)
                            errorCode = 10;
                        else
                            errorCode = 7;
                    }
                }
                else if (Print == "2")
                {
                    LogWriter.WriteLog("Save Issue Challan Reset Issue Challan");
                    ResetIssueChallan(ChalanIds, currentMonth, branchId);
                }
                else
                {
                    LogWriter.WriteLog("Save Issue Challan Checking if Saving is safe");
                    if (IsNoSelected(ChalanIds) == false)
                    {
                        if (DueDate < DateTime.Now)
                        {
                            errorCode = 8;
                            return RedirectToAction("Index", new { id = -59 });
                        }
                        foreach (var id in ChalanIds)
                        {
                            var studentChalan = feePlanRepo.GetStudentChallanDetailById(id);
                            int issuedFlag = 0;
                            IssuedChallan issueChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                            issuedFlag = (int)issueChalan.IssuedFlag;
                            //issueChalan.ChallanToStdId = id;
                            issueChalan.ChalanAmount = feePlanRepo.GetChallanAmountByChallanId(studentChalan.ChallanId);
                            //issueChalan.ChalanAmount += feePlanRepo.GetStudentArrearDetail(studentChalan.StdId).Sum(x => x.ArrearAmount);
                            issueChalan.ChalanAmount += feePlanRepo.GetStudentExtraChargesByStudent(studentChalan.StdId, currentMonth).Sum(x => x.HeadAmount);
                            issueChalan.PayedToType = int.Parse(AccountTypeId);
                            issueChalan.PayedTo = int.Parse(FinanceAccountId);
                            //issueChalan.Fine = GetStudentAttendanceFine(studentChalan.StdId, monthId, (2016 + yearId - 1), branchId);
                            issueChalan.DueDate = DueDate;
                            issueChalan.Fine = GetStudentTotalFine(studentChalan.StdId, monthId, (2016 + yearId - 1), branchId, DateTime.Parse(issueChalan.DueDate.ToString()));
                            issueChalan.PaidFlag = 0;
                            issueChalan.IssuedFlag = 1;
                            issueChalan.ForMonth = currentMonth;
                            issueChalan.PaidDate = issueChalan.IssueDate = DateTime.Now;
                            issueChalan.BranchId = branchId;
                            feePlanRepo.UpdateIssueChallan(issueChalan);
                            LogWriter.WriteLog("Saved Issue Challan successfully, Id : " + issueChalan.Id);
                            var student = studentRepo.GetStudentById(studentChalan.StdId);
                            if (issuedFlag == 0)
                            {
                                int balanceId = 0;
                                FeeBalance balance = feePlanRepo.GetFeeBalanceByStudentId(student.id);
                                if (balance != null)
                                    balanceId = balance.Id;
                                LogWriter.WriteLog("Save Issue Challan saving Issue Challan Detail");
                                //Thread detailThread = new Thread(() => SaveIssueChallanDetail((int)issueChalan.Id, DateTime.Now, DateTime.Now, balanceId, 0, 0, studentChalan.ChallanId, studentChalan.StdId, currentMonth));
                                //detailThread.Start();

                                SaveIssueChallanDetail((int)issueChalan.Id, DateTime.Now, DateTime.Now, balanceId, 0, 0, studentChalan.ChallanId, studentChalan.StdId, currentMonth);
                                LogWriter.WriteLog("Save Issue Challan Create Journal Entry");
                                CreateJournalEntry(issueChalan.Id.ToString(), DateTime.Now, (int)issueChalan.ChalanAmount, "", "JE", student, currentMonth, branchId);
                                //Thread entryThread = new Thread(() => CreateJournalEntry(issueChalan.Id.ToString(), DateTime.Now, (int)issueChalan.ChalanAmount, "", "JE", student, currentMonth, branchId));
                                //entryThread.Start();
                            }

                            int annualFunds = 0;
                            if (AnnualFunds != null && AnnualFunds.Length > 0)
                                annualFunds = int.Parse(AnnualFunds);
                            if (annualFunds > 0)
                            {
                                var annualCharges = feePlanRepo.GetAnnualChargesByStudentIdAndMonth(studentChalan.StdId, currentMonth);
                                if (annualCharges != null && annualCharges.Count > 0)
                                {
                                    var charges = annualCharges[0];
                                    charges.Charges = annualFunds;
                                    feePlanRepo.UpdateAnnualCharges(charges);
                                }
                                else
                                {
                                    AnnualCharge charges = new AnnualCharge();
                                    charges.Charges = annualFunds;
                                    charges.ForMonth = currentMonth;
                                    charges.StudentId = studentChalan.StdId;
                                    feePlanRepo.AddAnnualCharges(charges);
                                }
                            }
                        }
                        errorCode = 4;
                    }
                    else
                    {
                        if (ChalanIds == null || ChalanIds.Length == 0)
                            errorCode = 11;
                        else
                            errorCode = 6;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        private void ResetIssueChallan(int[] ChalanIds, string currentMonth, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Reset Issue Challan Count : " + (ChalanIds == null ? 0 : ChalanIds.Count()));
                LogWriter.WriteLog("Reset Issue Challan Checking if Saving is safe");
                if (IsNoSelected(ChalanIds) == false)
                {
                    if (CheckIfChallansPaid(ChalanIds, currentMonth) == false)
                    {
                        foreach (var id in ChalanIds)
                        {
                            var studentChalan = feePlanRepo.GetStudentChallanDetailById(id);
                            IssuedChallan issueChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                            var student = studentRepo.GetStudentById(studentChalan.StdId);
                            if (issueChalan.IssuedFlag == 1)
                            {
                                var detailList = feePlanRepo.GetIssueChallanDetail((int)issueChalan.Id);
                                LogWriter.WriteLog("Reset Issue Challan Create Journal Entry");
                                CreateResetIssueJournalEntry(issueChalan.Id.ToString(), DateTime.Now, (int)issueChalan.ChalanAmount, "", "JE", student, currentMonth, branchId, detailList);
                                foreach (var detail in detailList)
                                {
                                    feePlanRepo.DeleteIssueChallanDetail(detail);
                                }
                            }
                            feePlanRepo.DeleteIssueChallan(issueChalan);
                        }
                        errorCode = 40;
                    }
                    else
                    {
                        errorCode = 120;
                    }
                }
                else
                {
                    errorCode = 60;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 4420;
            }
            
        }

        private bool CheckIfChallansPaid(int[] ChalanIds, string currentMonth)
        {
            foreach (var id in ChalanIds)
            {
                var studentChalan = feePlanRepo.GetStudentChallanDetailById(id);
                IssuedChallan issueChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);
                if (issueChalan.PaidFlag == 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsFineConfigDefined(int branchId)
        {
            IssueChallanConfig config = feePlanRepo.GetFine(branchId);
            if (config == null)
                return false;
            return true;
        }

        private int GetStudentTotalFine(int id, int month, int year, int branchId, DateTime challanDueDate)
        {
            int actualFine = 0;

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Calculating the Student Fine, studentId : " + id);
                IssueChallanConfig isueChalConfg = feePlanRepo.GetFine(branchId);
                //DateTime challanDueDate = DateTime.Parse(issueChalan.DueDate.ToString());
                DateTime currentDay = DateTime.Now;
                String diff2 = (currentDay - challanDueDate).TotalDays.ToString();
                //float daysDiffr = float.Parse(diff2, System.Globalization.CultureInfo.InvariantCulture);
                //int FineDays = int.Parse(daysDiffr, System.Globalization.CultureInfo.InvariantCulture);
                string dayExceed = diff2.ToString().Split('.')[0];
                int FineDays = int.Parse(dayExceed);



                if (FineDays != null && FineDays > 0)
                {
                    if (isueChalConfg.Fine != null && isueChalConfg.Fine > 0)
                    {
                        actualFine = int.Parse(isueChalConfg.Fine.ToString());
                    }
                    else if (isueChalConfg.FinePerDay != null && isueChalConfg.FinePerDay > 0)
                    {
                        actualFine = int.Parse(isueChalConfg.FinePerDay.ToString()) * FineDays;
                    }
                }

                DateTime firstDay = new DateTime(year, month, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                int nOfAbsents = attRepo.GetStudentAbsents(id, firstDay, lastDay);
                if (nOfAbsents != null && nOfAbsents > 0 && isueChalConfg.AttendanceDays != null && isueChalConfg.AttendanceDays > 0)
                {
                    actualFine = actualFine + (nOfAbsents * int.Parse(isueChalConfg.AttendanceDays.ToString()));
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return actualFine;
        }

        private int GetStudentAttendanceFineee(int id, int month, int year, int branchId)
        {
            IssueChallanConfig config = feePlanRepo.GetFine(branchId);
            int fine = 0;

            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            int absents = attRepo.GetStudentAbsents(id, firstDay, lastDay);
            if (absents > config.AttendanceDays)
                absents = (int)config.AttendanceDays;

            fine = absents * (int)config.FinePerDay;
            return fine;
        }

        private int CreateJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, Student student, string forMonth, int branchId)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating Journal Entry for the IssueChallanNo : " + ChequeNo);

                string studenInfo = " (" + student.RollNumber + ", " + student.Name +", " + student.FatherName + ")";
                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail(int.Parse(ChequeNo));
                JournalEntry je = new JournalEntry();
                je.CreditAmount = detailList.Sum(x => x.TotalAmount);
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Fee Amount is added to Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Journal Entry is Saved Succesfully");

                LogWriter.WriteLog("Journal Entry Adding Debit Entry Detail");
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                foreach (IssueChalanDetail detail in detailList)
                {
                    if (detail.TotalAmount > 0)
                    {
                        var tempAccount1 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, receivableAccountid);
                        JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                        entryDetail1.EntryId = je.EntryId;
                        entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                        entryDetail1.Amount = detail.TotalAmount;
                        if (detail.Type == 1)
                            entryDetail1.Description = "Fee Challan Issued, For Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        else if (detail.Type == 2)
                            entryDetail1.Description = "Fee Challan Issued, For Arrear Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        else
                            entryDetail1.Description = "Fee Challan Issued, For Extra Charges Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;

                        accountRepo.AddJurnalEntryDebitDetail(entryDetail1);
                        FinanceHelper.UpdateDebitAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount);
                        FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);
                    }
                }


                LogWriter.WriteLog("Journal Entry Adding Credit Entry Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccount(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, receivableAccountid);
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = "Fee Amount is added to Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return errorCode;
        }

        private int CreateResetIssueJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, Student student, string forMonth, int branchId, List<IssueChalanDetail> detailList)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating Reset Journal Entry for the IssueChallanNo : " + ChequeNo);
                string studenInfo = " (" + student.RollNumber + ", " + student.Name +", " + student.FatherName + ")";
                JournalEntry je = new JournalEntry();
                je.CreditAmount = EntryAmount;
                je.DebitAmount = EntryAmount;
                je.CreditDescription = "Fee Amount is reset for Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Reset Journal Entry is Saved Succesfully");

                LogWriter.WriteLog("Reset Journal Entry Adding Credit Entry Detail");
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                foreach (IssueChalanDetail detail in detailList)
                {
                    if (detail.TotalAmount > 0)
                    {
                        var tempAccount1 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, receivableAccountid);
                        JournalEntryCreditDetail entryDetail1 = new JournalEntryCreditDetail();
                        entryDetail1.EntryId = je.EntryId;
                        entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                        entryDetail1.Amount = detail.TotalAmount;
                        if (detail.Type == 1)
                            entryDetail1.Description = "Fee Challan Reset, For Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        else if (detail.Type == 2)
                            entryDetail1.Description = "Fee Challan Reset, For Arrear Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        else
                            entryDetail1.Description = "Fee Challan Reset, For Extra Charges Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;

                        accountRepo.AddJurnalEntryCreditDetail(entryDetail1);
                        FinanceHelper.UpdateCreditAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount, je.EntryId);
                        FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);
                        
                    }
                }


                LogWriter.WriteLog("Reset Journal Entry Adding Debit Entry Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccount(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, receivableAccountid);
                JournalEntryDebitDetail entryDetail = new JournalEntryDebitDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = EntryAmount;
                entryDetail.Description = "Fee Amount is Reset for Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                accountRepo.AddJurnalEntryDebitDetail(entryDetail);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return errorCode;
        }

        private int CreatePaidJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, Student student, string forMonth, string FinanceAccountId, int branchId, bool updateFlag = false)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating Paid Journal Entry for the IssueChallanNo : " + ChequeNo);
                string studenInfo = " (" + student.RollNumber + ", " + student.Name +", " + student.FatherName + ")";
                JournalEntry je = new JournalEntry();
                je.CreditAmount = EntryAmount;
                je.DebitAmount = EntryAmount;
                if (!updateFlag)
                {
                    je.CreditDescription = "Fee Challan Paid, Challan No : " + ChequeNo + ", For Student : " + student.id.ToString().PadLeft(6, '0') + "-" + student.Name + ", For Month : " + forMonth + studenInfo;
                }
                else
                {
                    je.CreditDescription = "Fee Challan Updated, Challan No : " + ChequeNo + ", For Student : " + student.id.ToString().PadLeft(6, '0') + "-" + student.Name + ", For Month : " + forMonth + studenInfo;
                }
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Paid Journal Entry is Saved Succesfully");

                LogWriter.WriteLog("Paid Journal Entry Adding Debit Entry Detail");
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(FinanceAccountId));
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = EntryAmount;
                if (!updateFlag)
                {
                    entryDetail1.Description = "Fee Amount is paid from Student " + tempAccount1.AccountName + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                else
                {
                    entryDetail1.Description = "Fee Amount is Updated from Student " + tempAccount1.AccountName + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);

                LogWriter.WriteLog("Paid Journal Entry Adding Credit Entry Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountByName(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, branchId);
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = -1 * EntryAmount;
                if (!updateFlag)
                {
                    entryDetail.Description = "Fee Amount is reversed from Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                else
                {
                    entryDetail.Description = "Fee Amount is Updated from Student " + tempAccount.AccountName + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                List<IssueChalanDetail> detailList = (List<IssueChalanDetail>)Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST];

                LogWriter.WriteLog("Paid Journal Entry Updating Receivables");
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                if (detailList != null && detailList.Count > 0)
                {
                    foreach (IssueChalanDetail detail in detailList)
                    {
                        var temp = feePlanRepo.GetIssuedChallanDetail(int.Parse(ChequeNo), (int)detail.FeeHeadId, (int)detail.Type);
                        if (temp.PayAmount != detail.PayAmount)
                        {
                            var tempAccount12 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, receivableAccountid);
                            string fourthLvlAccountName = student.ClassSection.Class.Name + ", Section : " + student.ClassSection.Section.Name;
                            var fourthLvl = accountRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccountName, branchId);
                            var tempAccount123 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, fourthLvl.Id);

                            if (tempAccount12 != null && tempAccount123 != null)
                            {
                                int adjustmentAmount = (int)(detail.PayAmount - temp.PayAmount);
                                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount12.FourthLvlAccountId, adjustmentAmount, je.EntryId);
                                FinanceHelper.UpdateCreditFifthAccountBalance((int)tempAccount12.Id, adjustmentAmount);

                                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount123.FourthLvlAccountId, adjustmentAmount);
                                FinanceHelper.UpdateDebitFifthAccountBalance((int)tempAccount123.Id, adjustmentAmount);
                            }
                        }

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

            return errorCode;
        }

        private int CreateUnPaidJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, Student student, string forMonth, string FinanceAccountId, int branchId, bool updateFlag = false)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating UnPaid Journal Entry for the IssueChallanNo : " + ChequeNo);
                string studenInfo = " (" + student.RollNumber + ", " + student.Name +", " + student.FatherName + ")";
                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail(int.Parse(ChequeNo));
                JournalEntry je = new JournalEntry();
                je.CreditAmount = detailList.Sum(x => x.PayAmount);
                je.DebitAmount = je.CreditAmount;
                if (updateFlag)
                {
                    je.CreditDescription = "Fee Challan Unpaid, Challan No : " + ChequeNo + ", For Student : " + student.id.ToString().PadLeft(6, '0') + "-" + student.Name + ", For Month : " + forMonth + studenInfo;
                }
                else
                {
                    je.CreditDescription = "Fee Challan Unpaid, Challan No : " + ChequeNo + ", For Student : " + student.id.ToString().PadLeft(6, '0') + "-" + student.Name + ", For Month : " + forMonth + studenInfo;
                }
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("UnPaid Journal Entry is Saved Succesfully");

                LogWriter.WriteLog("UnPaid Journal Entry Adding Credit Entry Detail");
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountById(int.Parse(FinanceAccountId));
                JournalEntryCreditDetail entryDetail1 = new JournalEntryCreditDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                if (updateFlag)
                {
                    entryDetail1.Description = "Fee Amount is Unpaid from Student " + tempAccount1.AccountName + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                else
                {
                    entryDetail1.Description = "Fee Amount is Unpaid from Student " + tempAccount1.AccountName + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                accountRepo.AddJurnalEntryCreditDetail(entryDetail1);
                //FinanceHelper.UpdateCreditAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount, je.EntryId);
                //FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);

                LogWriter.WriteLog("UnPaid Journal Entry Adding Debit Entry Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountByName(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, branchId);
                JournalEntryDebitDetail entryDetail = new JournalEntryDebitDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = -1 * je.CreditAmount;
                if (updateFlag)
                {
                    entryDetail.Description = "Fee Amount is reversed to Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                else
                {
                    entryDetail.Description = "Fee Amount is reversed from Student " + tempAccount1.AccountName + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                }
                //FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                //FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                accountRepo.AddJurnalEntryDebitDetail(entryDetail);
                LogWriter.WriteLog("UnPaid Journal Entry Updating Receivables");
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                if (detailList != null && detailList.Count > 0)
                {
                    foreach (IssueChalanDetail detail in detailList)
                    {
                        var temp = feePlanRepo.GetIssuedChallanDetail(int.Parse(ChequeNo), (int)detail.FeeHeadId, (int)detail.Type);
                        if (temp.PayAmount != detail.PayAmount)
                        {
                            var tempAccount12 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, receivableAccountid);
                            string fourthLvlAccountName = student.ClassSection.Class.Name + ", Section : " + student.ClassSection.Section.Name;
                            var fourthLvl = accountRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccountName, branchId);
                            var tempAccount123 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, fourthLvl.Id);

                            if (tempAccount12 != null && tempAccount123 != null)
                            {
                                int adjustmentAmount = (int)(detail.PayAmount - temp.PayAmount);
                                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount12.FourthLvlAccountId, adjustmentAmount);
                                FinanceHelper.UpdateDebitFifthAccountBalance((int)tempAccount12.Id, adjustmentAmount);

                                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount123.FourthLvlAccountId, adjustmentAmount, je.EntryId);
                                FinanceHelper.UpdateCreditFifthAccountBalance((int)tempAccount123.Id, adjustmentAmount);
                            }
                        }

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

            return errorCode;
        }

        private void CreateResetJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, Student student, string forMonth, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating Reset Journal Entry for the IssueChallanNo : " + ChequeNo);
                string studenInfo = " (" + student.RollNumber + ", " + student.Name + ", " + student.FatherName + ")";
                JournalEntry je = new JournalEntry();
                je.CreditAmount = EntryAmount;
                je.DebitAmount = EntryAmount;
                je.CreditDescription = CreditDescription;
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Reset Journal Entry is Saved Succesfully");

                LogWriter.WriteLog("Reset Journal Entry Adding Credit Entry Detail");
                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail(int.Parse(ChequeNo));

                foreach (IssueChalanDetail detail in detailList)
                {
                    if (detail.PayAmount > 0)
                    {
                        var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountByName(detail.FeeHead.Name, branchId);
                        JournalEntryCreditDetail entryDetail1 = new JournalEntryCreditDetail();
                        entryDetail1.EntryId = je.EntryId;
                        entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                        entryDetail1.Amount = detail.PayAmount;
                        entryDetail1.Description = "Fee Challan Reset, For Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        accountRepo.AddJurnalEntryCreditDetail(entryDetail1);
                        FinanceHelper.UpdateCreditAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount, je.EntryId);
                        FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);
                    }
                }


                LogWriter.WriteLog("Reset Journal Entry Adding Debit Entry Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccountByName(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, branchId);
                JournalEntryDebitDetail entryDetail = new JournalEntryDebitDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = EntryAmount;
                entryDetail.Description = "Fee Amount is Reset from Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                accountRepo.AddJurnalEntryDebitDetail(entryDetail);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private bool CheckPaidStatus(int[] chalanIds, string currentMonth)
        {
            if (chalanIds != null && chalanIds.Length > 0)
            {
                foreach (int id in chalanIds)
                {
                    if (id > 0)
                    {
                        IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth);

                        if (issuedChalan.IssuedFlag == 0)
                            return false;
                    }
                }
            }

            return true;
        }

        private bool IsNoSelected(int[] chalanIds)
        {
            if (chalanIds != null && chalanIds.Length > 0)
            {
                foreach (int id in chalanIds)
                {
                    if (id > 0)
                        return false;
                }
            }

            return true;
        }

        private bool IsNoAmountEntered(int[] indexes, string[] PaidAmount)
        {
            foreach (int id in indexes)
            {
                if (PaidAmount[id].Length == 0)
                    return false;
            }


            return true;
        }
        public void SearchStudentParams(string ClassId, string SectionId, string RollNo, string Name, string FatherName)
        {
            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(SectionId);
            rollNumber = RollNo;
            name = Name;
            fatherName = FatherName;
            classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchIssueChalan(string MonthId, string YearId, string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string ChalanNo, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (IsFineConfigDefined(branchId) == true)
                {
                    SearchStudentParams(MonthId, YearId, ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, ChalanNo, AdmissionNo, FatherContact);
                    Session[ConstHelper.SEARCH_ISSUE_CHALLAN_FLAG] = true;
                    errorCode = 0;
                }
                else
                {
                    errorCode = 515;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = -59 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPaidChalan(string MonthId, string YearId, string ClassId, string SectionId, string ChalanNo, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                SearchPaidChalanParams(MonthId, YearId, ClassId, SectionId, RollNo, Name, FatherName, ChalanNo, FatherCnic, AdmissionNo, FatherContact);
                paidErrorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("PaidChallan", new { id = -59 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPartialPaidChalan(string MonthId, string YearId, string ClassId, string SectionId, string ChalanNo, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                SearchPaidChalanParams(MonthId, YearId, ClassId, SectionId, RollNo, Name, FatherName, ChalanNo, FatherCnic, AdmissionNo, FatherContact);
                paidErrorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("PaidPartialChallan", new { id = -59 });

        }

        public void SearchPaidChalanParams(string MonthId, string YearId, string ClassId, string SectionId, string RollNo, string Name, string FatherName, string chalanNo, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
            Session[ConstHelper.GLOBAL_MONTH_ID] = int.Parse(MonthId);
            Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(YearId);

            Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] = true;
            Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID] = int.Parse(MonthId);
            Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID] = int.Parse(YearId);
            Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO] = RollNo;
            Session[ConstHelper.SEARCH_CHALLAN_NAME] = Name;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME] = FatherName;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.SEARCH_CHALLAN_CHALLAN_NO] = chalanNo;
            Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO] = AdmissionNo;
            Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO] = FatherContact;
            Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID] = classId;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
            else
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = 0;
        }

        public void SearchStudentParams(string MonthId, string YearId, string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string ChalanNo, string AdmissionNo, string FatherContact)
        {
            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
            Session[ConstHelper.GLOBAL_MONTH_ID] = int.Parse(MonthId);
            Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(YearId);

            Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID] = int.Parse(MonthId);
            Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID] = int.Parse(YearId);
            Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO] = RollNo;
            Session[ConstHelper.SEARCH_CHALLAN_NAME] = Name;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME] = FatherName;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.SEARCH_CHALLAN_CHALLAN_NO] = ChalanNo;
            Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO] = AdmissionNo;
            Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO] = FatherContact;
            Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID] = classId;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
            else
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = 0;
        }


        private void voidSetSearchVeriables()
        {
            if (Session[ConstHelper.GLOBAL_CLASS_ID] != null)
            {
                ViewData["GlobalClassId"] = (int)Session[ConstHelper.GLOBAL_CLASS_ID];
                Session[ConstHelper.GLOBAL_CLASS_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SECTION_ID] != null)
            {
                ViewData["GlobalSectionId"] = (int)Session[ConstHelper.GLOBAL_SECTION_ID];
                Session[ConstHelper.GLOBAL_SECTION_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_MONTH_ID] != null)
            {
                ViewData["GlobalMonthId"] = (int)Session[ConstHelper.GLOBAL_MONTH_ID];
                Session[ConstHelper.GLOBAL_MONTH_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_YEAR_ID] != null)
            {
                ViewData["GlobalYearId"] = (int)Session[ConstHelper.GLOBAL_YEAR_ID];
                Session[ConstHelper.GLOBAL_YEAR_ID] = null;
            }
        }


        public FileStreamResult showReportAsPdf(int issueChallan)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DAL_Fee_Reports reports = new DAL_Fee_Reports();
                DataSet ds = reports.GetFeePaymentSlip(issueChallan);
                AddImage(ds);
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "FeePaymentSlipReport.rpt"));
                rd.SetDataSource(ds.Tables[0]);
                SchoolConfig config = studentRepo.GetSchoolConfigById(1);
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                var contentLength = stream.Length;
                Response.AppendHeader("Content-Length", contentLength.ToString());
                Response.AppendHeader("Content-Disposition", "inline; filename=FeePaymentSlip.pdf");

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
        }

        private void AddImage(DataSet ds)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                ds.Tables[0].Rows.Add();
            }

            SchoolConfig config = studentRepo.GetSchoolConfigById(1);
            DataColumn colByteArray = new DataColumn("IMAGE");
            colByteArray.DataType = System.Type.GetType("System.Byte[]");
            ds.Tables[0].Columns.Add(colByteArray);
            ds.Tables[0].Rows[0]["IMAGE"] = config.SchoolLogo;
        }

    }
}
