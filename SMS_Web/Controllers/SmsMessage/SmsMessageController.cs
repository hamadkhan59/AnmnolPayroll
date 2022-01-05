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
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class SmsMessageController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;


        //private ISecurityRepository secRepo;
        private IFeePlanRepository smsRepo;
        private static int errorCode = 0;
        static int m_selectedEventId = -1;
        static int m_selectedEventMessageId = -1;
        //
        // GET: /Class/

        public SmsMessageController()
        {
            smsRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); 
        }

        //
        // GET: /SMS Message/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            SmsMessage newMessage = new SmsMessage();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["SMS Message"] = SessionHelper.SmsMessageList;
                ViewData["SMSParams"] = SessionHelper.SmsParamsList.Where(x => x.ParamType == 1 || x.ParamType == 2).ToList();
                ViewData["Error"] = errorCode;
                errorCode = 0;
                newMessage = smsRepo.GetMessageById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View(newMessage);
        }

        //
        // POST: /SMS Message/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(SmsMessage branch)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (branch.Name != null && branch.Message != null)
                {
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    branch.BranchId = branchId;
                    int returnCode = smsRepo.AddSmsMessage(branch);
                    List<SmsMessage> abc = SessionHelper.RefreshSmsMessageList;
                    SessionHelper.InvalidateBranchCache = false;
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                }
                else if (branch.Name == null)
                {
                    errorCode = 521;
                }
                else
                    errorCode = 522;
                

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

        public ActionResult ViewSample(SmsMessage branch)
        {
            ViewData["ShowPopup"] = "";
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (branch.Name != null && branch.Message != null)
                {
                    errorCode = 523;
                    ViewData["ShowPopup"] = "ShowingPopup";
                }
                else if (branch.Name == null)
                {
                    errorCode = 521;
                }
                else
                    errorCode = 522;

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
        public ActionResult Edit(SmsMessage branch)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                smsRepo.UpdateSmsMessage(branch);
                List<SmsMessage> abc = SessionHelper.RefreshSmsMessageList;
                errorCode = 2;
                SessionHelper.InvalidateBranchCache = false;
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

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SmsMessage branch = smsRepo.GetMessageById(id);
                if (branch == null)
                {
                    return HttpNotFound();
                }
                smsRepo.DeleteSmsMessage(branch);
                errorCode = 4;
                SessionHelper.InvalidateBranchCache = false;
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

        public ActionResult SmsEvents()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_MESSAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<SmsModel> smsList = new List<SmsModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                smsList = smsRepo.GetSMSEvents(branchId);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(smsList);
        }

        public ActionResult SmsEventMessages()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_EVENT_MESSAGES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            List<SmsModel> smsList = new List<SmsModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                smsList = smsRepo.GetSMSEvents(branchId);

                ViewBag.EventId = new SelectList(SessionHelper.SmsEventList, "Id", "EventName");
                ViewBag.MessageId = new SelectList(SessionHelper.SmsMessageList, "Id", "Name");

                ViewData["Error"] = errorCode;
                errorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(smsList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SaveSmsEventMessages(int[] eventIds)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (m_selectedEventId != -1 && m_selectedEventMessageId != -1)
                {
                    SmsEvent selectedEvent = smsRepo.GetSmsEventById(m_selectedEventId);
                    SmsMessage selectedMsg = smsRepo.GetMessageById(m_selectedEventMessageId);
                    if (selectedEvent != null && selectedMsg != null)
                    {
                        if (SmsInfoProxy.isValidateSmsMsg(selectedEvent.Id, selectedMsg.Message))
                        {
                            selectedEvent.SmsTemplateId = m_selectedEventMessageId;
                            smsRepo.UpdateSmsEvent(selectedEvent);
                            List<SmsEvent> abc = SessionHelper.RefreshSmsEventList;
                            errorCode = 2;
                        }
                        else
                        {
                            errorCode = 425;
                        }
                        
                    }
                    else
                        errorCode = 421;
                }
                else
                    errorCode = 421;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }

            return RedirectToAction("SmsEventMessages");
        }

        public JsonResult getSearchSmsEventIdInfo(int SmsMessageEventId)
        {
            m_selectedEventId = SmsMessageEventId;
            return Json(SmsMessageEventId);         // Not need to use this Response.
        }

        public JsonResult getSearchSmsMessageIdInfo(int SmsEventMessageId)
        {
            m_selectedEventMessageId = SmsEventMessageId;
            return Json(SmsEventMessageId);         // Not need to use this Response.
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "SaveSmsEventMessages")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSmsEventMessages(SmsEvent smsModel)
        {
            return RedirectToAction("SmsEventMessages");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSmsEvents(int[] eventIds, int IsDontSend)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SMS_EVENT) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (eventIds != null && eventIds.Count() > 0)
                {
                    bool smsFlag = IsDontSend == 1 ? false : true;
                    if (smsFlag && !IsAllEventsSmsAssigned(eventIds))
                    {
                        errorCode = 20;
                    }
                    else
                    { 
                        foreach (int id in eventIds)
                        {
                            var smsEvent = smsRepo.GetSmsEventById(id);
                            smsEvent.SmsFlag = smsFlag;
                            smsRepo.UpdateSmsEvent(smsEvent);
                            List<SmsEvent> abc = SessionHelper.RefreshSmsEventList;
                        }
                        errorCode = 2;
                    }
                }
                    else
                        errorCode = 421;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SmsEvents");
        }

        private bool IsAllEventsSmsAssigned(int [] eventIds)
        {
            bool flag = true;
            if (eventIds != null && eventIds.Count() > 0)
            {
                foreach (int id in eventIds)
                {
                    var smsEvent = smsRepo.GetSmsEventById(id);
                    if (smsEvent.SmsTemplateId == null || smsEvent.SmsTemplateId == 0)
                    {
                        flag = false;
                        break;
                    }
                }

                errorCode = 2;
            }

            return flag;
        }



        [HttpPost]
        public JsonResult GetViewMessageDetail(int messageId)
        {

            List<string[]> allownceList = new List<string[]>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SmsMessage respondMsg = smsRepo.GetMessageById(messageId);
                respondMsg.Message = sampleMessage(respondMsg.Message);


                string[] tempObj12 = new string[5];
                tempObj12[0] = respondMsg.Name;// +" : ";
                tempObj12[1] = respondMsg.Message;
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

        public string sampleMessage(string userMessage)
        {
            string sampleMsg = userMessage;

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (sampleMsg.Contains("{StudentName}"))
                {
                    sampleMsg = sampleMsg.Replace("{StudentName}", ConstHelper.SAMPLE_MSG_STD_NAME);
                }
                if (sampleMsg.Contains("{FatherName}"))
                {
                    sampleMsg = sampleMsg.Replace("{FatherName}", ConstHelper.SAMPLE_MSG_FATHER_NAME);
                }
                if (sampleMsg.Contains("{RollNo}"))
                {
                    sampleMsg = sampleMsg.Replace("{RollNo}", ConstHelper.SAMPLE_MSG_ROLL_NO);
                }
                if (sampleMsg.Contains("{AdmissionNo}"))
                {
                    sampleMsg = sampleMsg.Replace("{AdmissionNo}", ConstHelper.SAMPLE_MSG_ADMISSION_NO);
                }
                if (sampleMsg.Contains("{ClassName}"))
                {
                    sampleMsg = sampleMsg.Replace("{ClassName}", ConstHelper.SAMPLE_MSG_CLASS);
                }
                if (sampleMsg.Contains("{SectionName}"))
                {
                    sampleMsg = sampleMsg.Replace("{SectionName}", ConstHelper.SAMPLE_MSG_SECTION);
                }
                if (sampleMsg.Contains("{FeeMonth}"))
                {
                    sampleMsg = sampleMsg.Replace("{FeeMonth}", ConstHelper.SAMPLE_MSG_FEE_MONTH);
                }
                if (sampleMsg.Contains("{TotalFee}"))
                {
                    sampleMsg = sampleMsg.Replace("{TotalFee}", ConstHelper.SAMPLE_MSG_TOTAL_FEE);
                }
                if (sampleMsg.Contains("{PaidFee}"))
                {
                    sampleMsg = sampleMsg.Replace("{PaidFee}", ConstHelper.SAMPLE_MSG_PAID_FEE);
                }
                if (sampleMsg.Contains("{PendingFee}"))
                {
                    sampleMsg = sampleMsg.Replace("{PendingFee}", ConstHelper.SAMPLE_MSG_PENDING_FEE);
                }
                if (sampleMsg.Contains("{ExamName}"))
                {
                    sampleMsg = sampleMsg.Replace("{ExamName}", ConstHelper.SAMPLE_MSG_EXAM_NAME);
                }
                if (sampleMsg.Contains("{SubjectName}"))
                {
                    sampleMsg = sampleMsg.Replace("{SubjectName}", ConstHelper.SAMPLE_MSG_SUBJECT_NAME);
                }
                if (sampleMsg.Contains("{ObtMarks}"))
                {
                    sampleMsg = sampleMsg.Replace("{ObtMarks}", ConstHelper.SAMPLE_MSG_OBT_MARKS);
                }
                if (sampleMsg.Contains("{TotalMarks}"))
                {
                    sampleMsg = sampleMsg.Replace("{TotalMarks}", ConstHelper.SAMPLE_MSG_TOTAL_MARKS);
                }
                if (sampleMsg.Contains("{Grade}"))
                {
                    sampleMsg = sampleMsg.Replace("{Grade}", ConstHelper.SAMPLE_MSG_GRADE);
                }
                if (sampleMsg.Contains("{TotalObtMarks}"))
                {
                    sampleMsg = sampleMsg.Replace("{TotalObtMarks}", ConstHelper.SAMPLE_MSG_TOTAL_OBT_MARKS);
                }
                if (sampleMsg.Contains("{GrandTotalMarks}"))
                {
                    sampleMsg = sampleMsg.Replace("{GrandTotalMarks}", ConstHelper.SAMPLE_MSG_GRAND_TOTAL_MARKS);
                }
                if (sampleMsg.Contains("{GrandGrade}"))
                {
                    sampleMsg = sampleMsg.Replace("{GrandGrade}", ConstHelper.SAMPLE_MSG_GRAND_GRADE);
                }
                if (sampleMsg.Contains("{StaffName}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffName}", ConstHelper.SAMPLE_MSG_STAFF_NAME);
                }
                if (sampleMsg.Contains("{StaffFatherName}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffFatherName}", ConstHelper.SAMPLE_MSG_STAFF_FATHER_NAME);
                }
                if (sampleMsg.Contains("{StaffId}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffId}", ConstHelper.SAMPLE_MSG_STAFF_ID);
                }
                if (sampleMsg.Contains("{StaffMobileNo}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffMobileNo}", ConstHelper.SAMPLE_MSG_STAFF_MOBILE_NO);
                }
                if (sampleMsg.Contains("{StaffDesignation}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffDesignation}", ConstHelper.SAMPLE_MSG_STAFF_DESIGNATION);
                }
                if (sampleMsg.Contains("{SalaryMonth}"))
                {
                    sampleMsg = sampleMsg.Replace("{SalaryMonth}", ConstHelper.SAMPLE_MSG_SALARY_MONTH);
                }
                if (sampleMsg.Contains("{TotalSalary}"))
                {
                    sampleMsg = sampleMsg.Replace("{TotalSalary}", ConstHelper.SAMPLE_MSG_TOTAL_SALARY);
                }
                if (sampleMsg.Contains("{SalaryDeduction}"))
                {
                    sampleMsg = sampleMsg.Replace("{SalaryDeduction}", ConstHelper.SAMPLE_MSG_SALARY_DEDUCTION);
                }
                if (sampleMsg.Contains("{AdvanceAdjustment}"))
                {
                    sampleMsg = sampleMsg.Replace("{AdvanceAdjustment}", ConstHelper.SAMPLE_MSG_ADVANCE_ADJUSTMENT);
                }
                if (sampleMsg.Contains("{GrossSalary}"))
                {
                    sampleMsg = sampleMsg.Replace("{GrossSalary}", ConstHelper.SAMPLE_MSG_GROSS_SALARY);
                }
                if (sampleMsg.Contains("{SalaryDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{SalaryDate}", ConstHelper.SAMPLE_MSG_SALARY_DATE);
                }
                if (sampleMsg.Contains("{LeavingDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{LeavingDate}", ConstHelper.SAMPLE_MSG_LEAVING_DATE);
                }
                if (sampleMsg.Contains("{FeeDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{FeeDate}", ConstHelper.SAMPLE_MSG_FEE_DATE);
                }
                if (sampleMsg.Contains("{FeeDueDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{FeeDueDate}", ConstHelper.SAMPLE_MSG_FEE_DUE_DATE);
                }
                if (sampleMsg.Contains("{AttendanceStatus}"))
                {
                    sampleMsg = sampleMsg.Replace("{AttendanceStatus}", ConstHelper.SAMPLE_MSG_STD_ATTENDANCE_STATUS);
                }
                if (sampleMsg.Contains("{AttendanceDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{AttendanceDate}", ConstHelper.SAMPLE_MSG_STD_ATTENDANCE_DATE);
                }
                if (sampleMsg.Contains("{StaffAttendanceStatus}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffAttendanceStatus}", ConstHelper.SAMPLE_MSG_STAFF_ATTENDANCE_STATUS);
                }
                if (sampleMsg.Contains("{StaffAttendanceDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffAttendanceDate}", ConstHelper.SAMPLE_MSG_STAFF_ATTENDANCE_DATE);
                }
                if (sampleMsg.Contains("{StaffAttendanceTimeIn}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffAttendanceTimeIn}", ConstHelper.SAMPLE_MSG_STAFF_ATTENDANCE_TIME_IN);
                }
                if (sampleMsg.Contains("{StaffAttendanceTimeOut}"))
                {
                    sampleMsg = sampleMsg.Replace("{StaffAttendanceTimeOut}", ConstHelper.SAMPLE_MSG_STAFF_ATTENDANCE_TIME_OUT);
                }
                if (sampleMsg.Contains("{ExamPercentage}"))
                {
                    sampleMsg = sampleMsg.Replace("{ExamPercentage}", ConstHelper.SAMPLE_MSG_EXAM_PERCENTAGE);
                }
                if (sampleMsg.Contains("{ExamRemarks}"))
                {
                    sampleMsg = sampleMsg.Replace("{ExamRemarks}", ConstHelper.SAMPLE_MSG_EXAM_REMARKS);
                }
                if (sampleMsg.Contains("{ExamIssueDate}"))
                {
                    sampleMsg = sampleMsg.Replace("{ExamIssueDate}", ConstHelper.SAMPLE_MSG_EXAM_ISSUED_DATE);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return sampleMsg;
        }


    }
}
