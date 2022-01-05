using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using PdfSharp.Pdf;
using SMS.Modules.BuildPdf;
using System.IO;
using System.IO.Compression;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using System.Net;
using System.Globalization;
using System.Text;
using Logger;
using System.Reflection;
//using System.Collections;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class SmsSendGeneralController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        static int errorCode = 0, classErrorCode = 0, paidErrorCode = 0;
        static int examTotalMarks = 0, examPassPercentage = 0;
        static string m_smsMsg = null;
        static string m_smsMsgName = null;
        List<SmsHistory> m_smsHistoryList = null;
        //
        // GET: /ExamResult/

        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        IClassSubjectRepository clasSubjRepo;
        ISecurityRepository securityRepo;
        ISubjectRepository subjRepo;
        public SmsSendGeneralController()
        {
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
            clasSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2()); ;
            subjRepo = new SubjectRepositoryImp(new SC_WEBEntities2()); ;
            securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_GEN_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.ExamId = new SelectList(SessionHelper.ExamTypeList(Session.SessionID), "Id", "Name");
                ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
                ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);
                ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");

                if (Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] != null && (bool)Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] = false;
                    //ViewData["examSheet"] = SearchMarksSheet();
                }
                ViewData["Error"] = errorCode;
                if (id == 0)
                {
                    examTotalMarks = examPassPercentage = 0;
                }
                ViewData["totalMarks"] = examTotalMarks;
                ViewData["passPerecentage"] = examPassPercentage;
                examTotalMarks = examPassPercentage = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        public JsonResult GetSmsMessageTextInfo(int SmsMessageId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<SmsMessage> myList = SessionHelper.SmsMessageList;
                m_smsMsg = myList.Where(x => x.Id == SmsMessageId).FirstOrDefault().Message;
                m_smsMsgName = myList.Where(x => x.Id == SmsMessageId).FirstOrDefault().Name;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return Json(m_smsMsg);
        }

        public ActionResult SendGeneralSms()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_GEN_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StudentModel> list = new List<StudentModel>();
            try
            {
                ViewData["SMSMessage"] = SessionHelper.SmsMessageList;
                //ViewBag.SMSMessage = SessionHelper.SmsMessageList;
                ViewBag.SMSMessage = new SelectList(SessionHelper.SmsMessageList, "Id", "Name");

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["Error"] = classErrorCode;
                classErrorCode = 0;
                if (Session[ConstHelper.SEND_SMS_STUDENTSEARCH] != null)
                    list = (List<StudentModel>)Session[ConstHelper.SEND_SMS_STUDENTSEARCH];

                Session[ConstHelper.SEND_SMS_STUDENTSEARCH] = null;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudentForSms(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ClassId = string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId;
                SectionId = string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId;

                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);

                int classSectionId = 0;
                if (classId > 0 && sectionId > 0)
                    classSectionId = classSecRepo.GetClassSectionId(classId, sectionId);

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                List<StudentModel> modelList = new List<StudentModel>();
                if (classSectionId > 0)
                    modelList = studentRepo.SearchStudentsForSms(RollNo, Name, FatherName, classSectionId, FatherCnic, branchId, AdmissionNo);
                else
                    modelList = studentRepo.SearchStudentsForSms(RollNo, Name, FatherName, FatherCnic, branchId, AdmissionNo);
                Session[ConstHelper.SEND_SMS_STUDENTSEARCH] = modelList;

                LogWriter.WriteLog("Search Student Count : " + (modelList == null ? 0 : modelList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);

            }
            return RedirectToAction("SendGeneralSms");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SendGeneralMsg(int[] studentIds, string MessageText)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_GEN_MESSAGE) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (studentIds != null && studentIds.Count() > 0)
                {
                    m_smsHistoryList = new List<SmsHistory>();
                    foreach (int id in studentIds)
                    {
                        Student student = studentRepo.GetStudentById(id);
                        if(student != null)
                        {
                            SmsInfoProxy.sendSmsGeneralMessageEvent(student);
                        }
                    }
                }

                classErrorCode = 100;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }
            return RedirectToAction("SendGeneralSms");
        }

        public void sendSmsToStudents()
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (m_smsMsg != null && m_smsMsg != "" && m_smsHistoryList != null && m_smsHistoryList.Count > 0)
                {
                    //string contacts = "";
                    int unSendStudentCount = 0, sendStudentCount = 0;
                    int i = 0;

                    foreach (SmsHistory hisObj in m_smsHistoryList)
                    {
                        if (i < m_smsHistoryList.Count - 1)
                        {
                            //contacts = contacts + hisObj.Contact + ",";
                        }
                        else
                        {
                            //contacts = contacts + hisObj.Contact;
                        }
                        i++;
                    }

                    //string finalResult = readHtmlPage("http://www.outreach.pk/api/sendsms.php/sendsms/url", m_smsMsg, contacts);
                    string finalResult = "Success"; // Work
                    if (finalResult.Contains("Success"))
                    {
                        foreach (SmsHistory hisObj in m_smsHistoryList)
                        {
                            feePlanRepo.AddSmsHistory(hisObj);
                            sendStudentCount++;
                        }
                    }
                    else
                    {
                        unSendStudentCount++;
                    }

                    if (sendStudentCount != 0)
                    {
                        //MessageBox.Show("SMS has been sent to " + sendStudentCount + " Students successfully.");
                    }
                    m_smsHistoryList = null;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        // For UnPaid Students
        public ActionResult SendUnPaidSms()// SendGeneralMsg // PaidChallan
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_UNP_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                //var issuedchallans = db.IssuedChallans.Include(i => i.ChallanStudentDetail);
                //return View(issuedchallans.ToList());
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.ChallanId = new SelectList(SessionHelper.ChallanList(Session.SessionID), "Id", "Name");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                //ViewBag.BankId = new SelectList(SessionHelper.BankAccountList(Session.SessionID), "Id", "BankDetail");
                //ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
                //ViewBag.FinanceAccountId = new SelectList(SessionHelper.FeeAccountDetailList(Session.SessionID), "Id", "AccountName");
                //ViewBag.FinanceAccountId = new SelectList(SessionHelper.FeeAccountDetailList(Session.SessionID), "Id", "AccountName");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                //ViewData["financeAccounts"] = SessionHelper.FeeFinanceAccountList(Session.SessionID);
                ViewData["Error"] = paidErrorCode;
                paidErrorCode = 0;
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
                string challanNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CHALLAN_NO];
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];

                //List<IssuedChallanViewModel> studentChalanList = new List<IssuedChallanViewModel>();

                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);
                Session[ConstHelper.PAID_MONTH] = currentMonth;

                if (challanNo.Length > 0)
                {
                    int chalanNo = int.Parse(challanNo);
                    issuedChalaList = feePlanRepo.GetIssueChallanByChallanNo(chalanNo);
                }
                else
                {
                    issuedChalaList = feePlanRepo.SearchIssueChallanForSms(classSectionId, rollNumber, name, fatherName, currentMonth, fatherCnic, branchId, admissionInfo);
                }
                LogWriter.WriteLog("Search Issued Challan Count : " + (issuedChalaList == null ? 0 : issuedChalaList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return issuedChalaList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPaidChalan(string MonthId, string YearId, string ClassId, string SectionId, string ChalanNo, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                SearchPaidChalanParams(MonthId, YearId, ClassId, SectionId, RollNo, Name, FatherName, ChalanNo, FatherCnic, AdmissionNo);
                paidErrorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("SendUnPaidSms", new { id = -59 });

        }

        public void SearchPaidChalanParams(string MonthId, string YearId, string ClassId, string SectionId, string RollNo, string Name, string FatherName, string chalanNo, string FatherCnic, string AdmissionNo)
        {
            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.SEARCH_PAID_CHALLAN_FLAG] = true;
            Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID] = int.Parse(MonthId);
            Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID] = int.Parse(YearId);
            Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO] = RollNo;
            Session[ConstHelper.SEARCH_CHALLAN_NAME] = Name;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME] = FatherName;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.SEARCH_CHALLAN_CHALLAN_NO] = chalanNo;
            Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO] = AdmissionNo;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
            else
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = 0;
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
            var fine = feePlanRepo.GetFine(branchId);
            if (fine != null)
            {
                fine.Fine = Fine;
                feePlanRepo.UpdateFineValue(fine);
            }
        }

        [HttpPost]
        public void SaveIssueChallanDetail(List<IssueChalanDetail> detailList)
        {
            Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST] = detailList;
        }

        public ActionResult SendMsgToUnpaid(int[] ChalanIds, int[] Indexes, string[] PaidAmount, string[] Fine, string[] ChalanAmount, string[] Balance, string[] Advance, string FinanceAccountId, string AccountTypeId, string FullChallanPayment)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_UNP_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
                string currentMonth = SessionHelper.GetMonthName(monthId) + "-" + (2016 + yearId - 1);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                if (ChalanIds != null && ChalanIds.Count() > 0)
                {
                    for (int i = 0; i < ChalanIds.Count(); i++)
                    {
                        int id = ChalanIds[i];

                        //IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanByIdAndMonth(id, currentMonth); 
                        IssuedChallan issuedChalan = feePlanRepo.GetIssueChallanById(id); 

                        //int std1 = studentChalan.StdId;
                        int stdId = issuedChalan.ChallanStudentDetail.StdId;
                        if (stdId > 0)
                        {
                            FeeBalance feebalance = feePlanRepo.GetFeeBalanceByStudentId(issuedChalan.ChallanStudentDetail.StdId);
                            Student student = studentRepo.GetStudentById(stdId);

                            if (student != null && feebalance.Balance != null && feebalance.Balance > 0)
                            {
                                SmsInfoProxy.sendSmsStudentFeeDefaulterEvent(student, feebalance.Balance.ToString());
                            }
                        }
                    }
                    paidErrorCode = 4;
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
                return RedirectToAction("SendUnPaidSms", new { id = -59 });
            else
                return RedirectToAction("SendUnPaidSms", new { id = -59 });
        }

    }
}