using Logger;
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
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.FeeCollection
{
    public class FeeArrearsController : Controller
    {
        //
        // GET: /FeeArrears/
        IFeePlanRepository feePlanRepo;
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        private IFinanceAccountRepository accountRepo;
        static int errorCode = 0;
        public FeeArrearsController()
        {
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2()); 
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2()); 
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());; 
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_ARREARS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewData["Error"] = errorCode;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["nameList"] = SessionHelper.StudentNameList(branchId);
                errorCode = 0;
                if (Session[ConstHelper.FEE_ARREARS_SEARCH_FLAG] != null && (bool)Session[ConstHelper.FEE_ARREARS_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.FEE_ARREARS_SEARCH_FLAG] = false;

                    if (Session[ConstHelper.QUICK_ADMISSION_NO] != null)
                    {
                        string admissionNo = (string)Session[ConstHelper.QUICK_ADMISSION_NO];
                        Session[ConstHelper.QUICK_ADMISSION_NO] = null;
                        return View(feePlanRepo.SearchFeeArrearsByAdmissionNo(admissionNo));
                    }
                    else
                        return View(SearchFeeArrears());
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchFeeArrears(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_ARREARS) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                SearchFeeArrearsParmas(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic);
                Session[ConstHelper.FEE_ARREARS_SEARCH_FLAG] = true;
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
        [ValidateAntiForgeryToken]
        public ActionResult SearchByAdmissionNo(string admissionNo)
        {
            Session[ConstHelper.FEE_ARREARS_SEARCH_FLAG] = true;
            Session[ConstHelper.QUICK_ADMISSION_NO] = admissionNo;

            return RedirectToAction("Index");

        }

        private List<IssuedChallanViewModel> SearchFeeArrears()
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string rollNumber = (string)Session[ConstHelper.FEE_ARREARS_ROLL_NO];
                string name = (string)Session[ConstHelper.FEE_ARREARS_NAME];
                string fatherName = (string)Session[ConstHelper.FEE_ARREARS_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.FEE_ARREARS_FATHER_CNIC];
                int classSectionId = (int)Session[ConstHelper.FEE_ARREARS_CLASS_SCETION_ID];

                var arrears = feePlanRepo.SearchFeeArrears(classSectionId, rollNumber, name, fatherName, fatherCnic);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return arrears;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }

            return null;
        }

        public void SearchFeeArrearsParmas(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic)
        {
            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(SectionId);
            Session[ConstHelper.FEE_ARREARS_SEARCH_FLAG] = true;
            Session[ConstHelper.FEE_ARREARS_ROLL_NO] = RollNo;
            Session[ConstHelper.FEE_ARREARS_NAME] = Name;
            Session[ConstHelper.FEE_ARREARS_FATHER_NAME] = FatherName;
            Session[ConstHelper.FEE_ARREARS_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.FEE_ARREARS_CLASS_SCETION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetArrearDetails(int[] studentIds)
        {
            return RedirectToAction("ArrearDetail", new { studentId = studentIds[0] });
        }

        public ActionResult ArrearDetail(int studentId)
        {
            ViewData["feeHeads"] = feePlanRepo.GetStudentArrearDetail(studentId);
            ViewData["Error"] = errorCode;
            errorCode = 0;
            return View(studentRepo.GetStudentById(studentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveArrearDetails(int studentId, int[] feeHeadIds, int[] arrearAmounts, int[] studentDisounts, int[] addAmounts)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_ARREARS) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Saving arrears for the student : " + studentId);
                LogWriter.WriteLog("Fee heads count : " + (feeHeadIds == null ? 0 : feeHeadIds.Count()));

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                FeeBalance balance = feePlanRepo.GetFeeBalanceByStudentId(studentId);

                int feeBalanceId = 0;
                if (balance == null)
                {
                    LogWriter.WriteLog("Adding new fee balnace for the student ");
                    FeeBalance feeBalance = new FeeBalance();
                    feeBalance.StudentId = studentId;
                    feeBalance.Balance = feeBalance.Advance = 0;
                    feePlanRepo.AddFeeBalance(feeBalance);
                    balance = feeBalance;
                }
               
                feeBalanceId = balance.Id;
                int total = 0;
                LogWriter.WriteLog("Adding arrears detail ");

                for (int i = 0; i < feeHeadIds.Length; i++)
                {
                    FeeArrearsDetail detail = new FeeArrearsDetail();
                    detail.FeeBalanceId = feeBalanceId;
                    detail.FeeHeadId = feeHeadIds[i];
                    detail.HeadAmount = arrearAmounts[i] - studentDisounts[i] + addAmounts[i];
                    total += (int)detail.HeadAmount;
                    var obj = feePlanRepo.GetFeeArrearDetail((int)detail.FeeBalanceId, (int)detail.FeeHeadId);
                    LogWriter.WriteLog("Adding detail for the fee head  : " + feeHeadIds[i]);
                    if (obj == null)
                    {
                        AddArrearHistroy(detail, studentDisounts[i], addAmounts[i], arrearAmounts[i]);
                        feePlanRepo.SaveFeeArrearDetail(detail);
                    }
                    else
                    {
                        detail.ID = obj.ID;
                        AddArrearHistroy(detail, studentDisounts[i], addAmounts[i], arrearAmounts[i]);
                        feePlanRepo.UpdateFeeArrearDetail(detail);
                    }
                    if (studentDisounts[i] > 0)
                    {
                        feePlanRepo.setArrearDiscount(feeHeadIds[i], studentDisounts[i], studentId);
                        CreateJournalEntry(studentId, studentDisounts[i], branchId, feeHeadIds[i]);
                    }
                }

                balance.Balance = total;
                feePlanRepo.UpdateFeeBalance(balance);
                errorCode = 10;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }


            return RedirectToAction("ArrearDetail", new { studentId = studentId });
        }

        private int CreateJournalEntry(int studentId, int discount, int branchId, int feeHeadId)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the journal entry for the (student : " + studentId + ", doscount : " + discount +", fee head : " + feeHeadId+ ")");
                var feeHead = feePlanRepo.GetFeeHeadById(feeHeadId);
                var student = studentRepo.GetStudentById(studentId);
                string studenInfo = " (" + student.RollNumber + ", " + student.Name + ", " + student.FatherName + ")";
                JournalEntry je = new JournalEntry();
                je.CreditAmount = discount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Fee discount is provided to Student : " + studenInfo + " From Fee Fee Head : ("+ feeHead.Name +")";
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = "";
                je.EntryType = "DISC";
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Entry is saved");
             
                var tempAccount1 = accountRepo.GetFinanceFifthLvlAccountByName("Fee Discounts", branchId);
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                var tempAccount = accountRepo.GetFinanceFifthLvlAccount(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, receivableAccountid);

                LogWriter.WriteLog("Adding entry debit detail");

                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount.Id;
                entryDetail1.Amount = discount;
                entryDetail1.Description = je.CreditDescription;

                accountRepo.AddJurnalEntryDebitDetail(entryDetail1);


                LogWriter.WriteLog("Adding entry credit detail");
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount1.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = je.CreditDescription;
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

        private void AddArrearHistroy(FeeArrearsDetail detail, int discount, int amount, int arrearAmount)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Adding History (discount :" + discount + ", amount : "+amount+", arrearAmount : "+arrearAmount+")");
                if (discount > 0 || amount > 0)
                {
                    ArreartHistory history = new ArreartHistory();
                    history.FeeBalanceId = detail.FeeBalanceId;
                    history.FeeHeadId = detail.FeeHeadId;
                    history.CreatedOn = DateTime.Now;
                    if (arrearAmount == 0)
                    {
                        history.Description = "New Arrear is added to student with Amount : " + amount.ToString();
                    }
                    else
                    {
                        if (amount > 0)
                        {
                            history.PayAmount = detail.HeadAmount;
                            history.Discount = 0;
                            history.Description = "Amount : " + amount + " is added to student arrear";
                        }
                        if (discount > 0)
                        {
                            history.PayAmount = 0;
                            history.Discount = discount;
                            history.Description = "Discount : " + discount + " is given to student arrear";
                        }
                    }

                    LogWriter.WriteLog(history.Description);
                    history.IsAddedInChallan = false;
                    feePlanRepo.AddArrearHistory(history);
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
