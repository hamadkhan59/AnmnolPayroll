using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SMS_Web.Helpers
{
    public static class SmsInfoProxy
    {
        public static void sendSmsStudentAdmissionEvent(Student stdObj)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_STD_ADMISSION).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stdObj != null &&  stdObj.id != -1)
                {
                    //Student stdObj = SessionHelper.studentRepo.GetStudentById(stdId);
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsStudentLeavingEvent(Student stdObj)
        {
            try
            { 
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_STD_LEAVING).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stdObj != null &&  stdObj.id != -1)
                {
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsStudentAttendanceEvent(int stdId, int attendanceStat, DateTime attendanceDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                string stdAttendaceStatus = "";
                string attendanceEventName = "";
                if (attendanceStat == 1)
                {
                    stdAttendaceStatus = "Present";
                    attendanceEventName = ConstHelper.SMS_EVENT_NAME_STD_ATTENDANCE_PRESENT;
                }
                else
                {
                    stdAttendaceStatus = "Absent";
                    attendanceEventName = ConstHelper.SMS_EVENT_NAME_STD_ATTENDANCE_ABSENT;
                }

                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == attendanceEventName).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stdId != -1)
                {
                    Student stdObj = SessionHelper.studentRepo.GetStudentById(stdId);
                    List<SmsHistory> smsHisList = SessionHelper.feePlanRepo.GetSmsHistoryForAttendace(-1, (int)stdObj.id, attendanceDate, attendanceDate.AddDays(1), attendanceStat);
                    if (smsHisList == null || smsHisList.Count == 0)
                    {
                        string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                        string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                        actualMsgText = getStudentAttendanceSmsMsg(actualMsgText, stdAttendaceStatus, attendanceDate.ToString());
                        sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, attendanceStat);
                    }
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsStudentFeeCollectionEvent(Student stdObj, string feeMonth, string toalFee, string paidFee, string pendingFee, string payDate, string dueDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_STD_FEE_COLLECTION).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stdObj != null &&  stdObj.id != -1)
                {
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    actualMsgText = getStudentFeeSmsMsg(actualMsgText, feeMonth, toalFee, paidFee, pendingFee, payDate, dueDate);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsStudentFeeDefaulterEvent(Student stdObj, string pendingFee)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_STD_FEE_DEFAULTER).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stdObj != null &&  stdObj.id != -1)
                {
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    actualMsgText = getStudentFeeSmsMsg(actualMsgText, "", "", "", pendingFee, "", "");
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
        public static void sendSmsGeneralMessageEvent(Student stdObj)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_GENERAL_MESSAGE_EVENT).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stdObj != null &&  stdObj.id != -1)
                {
                    //Student stdObj = SessionHelper.studentRepo.GetStudentById(stdId);
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }


        public static void sendSmsStaffAdmissionEvent(Staff staffObj)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_STAFF_ADMISSION).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && staffObj != null && staffObj.StaffId != -1)
                {
                    string actualContact = ConvertUniversalNumber(staffObj.PhoneNumber);
                    string actualMsgText = getStaffInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, staffObj);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)staffObj.BranchId, staffObj.StaffId, 2, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsStaffSalaryEvent(StaffSalary stfSalary)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == ConstHelper.SMS_EVENT_NAME_STAFF_SALARY_PAID).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stfSalary.StaffId != null && stfSalary.StaffId != -1 && stfSalary != null)
                {
                    Staff staffObj = SessionHelper.staffRepo.GetStaffById((int)stfSalary.StaffId);
                    string actualContact = ConvertUniversalNumber(staffObj.PhoneNumber);
                    string actualMsgText = getStaffInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, staffObj);
                    actualMsgText = getStaffSalaryInfoSmsMsg(actualMsgText, stfSalary);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)staffObj.BranchId, staffObj.StaffId, 2, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsStaffAttendanceEvent(StaffAttandance stfAttend)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                string staffAttendaceStatus = "";
                string attendanceEventName = "";
                if (stfAttend.Status == 1)
                {
                    staffAttendaceStatus = "Present";
                    attendanceEventName = ConstHelper.SMS_EVENT_NAME_STAFF_ATTENDANCE_PRESENT;
                }
                else
                {
                    staffAttendaceStatus = "Absent";
                    attendanceEventName = ConstHelper.SMS_EVENT_NAME_STAFF_ATTENDANCE_ABSENT;
                }

                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == attendanceEventName).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && stfAttend.StaffId != null && stfAttend.StaffId != -1 && stfAttend.OutTime != null)
                {
                    Staff staffObj = SessionHelper.staffRepo.GetStaffById((int)stfAttend.StaffId);
                    List<SmsHistory> smsHisList = SessionHelper.feePlanRepo.GetSmsHistoryForAttendace((int)staffObj.StaffId, -1, (DateTime)stfAttend.Date, ((DateTime)stfAttend.Date).AddDays(1), (int)stfAttend.Status);
                    if (smsHisList == null || smsHisList.Count == 0)
                    {
                        string actualContact = ConvertUniversalNumber(staffObj.PhoneNumber);
                        string actualMsgText = getStaffInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, staffObj);
                        actualMsgText = getStaffAttendanceInfoSmsMsg(actualMsgText, stfAttend, staffAttendaceStatus);
                        sendSmsEventMsg(actualContact, actualMsgText, (int)staffObj.BranchId, staffObj.StaffId, 2, (int)eventInfo.Id, stfAttend.Status);
                    }
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendTermResultToAllStudentEvent(List<ExamResultViewModel> examdetail, int examTypeId, DateTime IssuedDate, string eventNamee)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == eventNamee).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && examdetail != null && examdetail.Count > 0 && examTypeId != -1)
                {
                    Student stdObj = SessionHelper.dbContext.Students.Find((int)examdetail[0].StudentId);
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    actualMsgText = getStudentTermExamSmsMsg(actualMsgText, examdetail, examTypeId, IssuedDate);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSessionResultToAllStudentEvent(StudentModel st, List<DataSet> dsList, int AutoRemarks, string teacherRemmarks, DateTime IssuedDat, string eventNamee)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsEvent eventInfo = SessionHelper.SmsEventList.Where(x => x.EventName == eventNamee).FirstOrDefault();
                if (eventInfo != null && eventInfo.SmsFlag && dsList != null && dsList.Count > 0)
                {
                    Student stdObj = SessionHelper.dbContext.Students.Find(st.Id);
                    string actualContact = ConvertUniversalNumber(stdObj.Contact_1);
                    string actualMsgText = getStudentInfoSmsMsg(SessionHelper.SmsMessageList.Where(x => x.Id == eventInfo.SmsTemplateId).FirstOrDefault().Message, stdObj);
                    actualMsgText = getStudentSessionExamSmsMsg(actualMsgText, dsList, AutoRemarks, teacherRemmarks, IssuedDat);
                    sendSmsEventMsg(actualContact, actualMsgText, (int)stdObj.BranchId, stdObj.id, 1, (int)eventInfo.Id, -1);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public static void sendSmsEventMsg(string actualContact, string actualMsgText, int branchId, int objectId, int objType, int eventId, int attendanceStauts)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                if (isValidMsgContact(actualContact, actualMsgText))
                {
                    string response = SmsService.sendSmsService(branchId, "", actualMsgText);
                    if (response != null && response != "" && response.Contains("Success"))
                        response = "Success!";
                    else
                        response = "Failure!";

                    createSmsHistory(objType, objectId, actualMsgText, response, eventId, attendanceStauts);
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
        public static void createSmsHistory(int objType, int objId, string msg, string Response, int eventId, int attendanceStauts)
        {
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                SmsHistory smsHis = new SmsHistory();
                if (objType == 1)
                {
                    smsHis.StdId = objId;
                    smsHis.StaffId = -1;
                }
                else
                {
                    smsHis.StaffId = objId;
                    smsHis.StdId = -1;
                }

                smsHis.Message = msg;
                smsHis.SentDate = System.DateTime.Now;
                smsHis.Response = Response;
                smsHis.UserId = 123;
                smsHis.EventId = eventId;
                smsHis.AttendanceStatus = attendanceStauts;
                SessionHelper.feePlanRepo.AddSmsHistory(smsHis);
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private static string ConvertUniversalNumber(string mobileNo)
        {
            string convertedMobileNo = "";

            if (!string.IsNullOrEmpty(mobileNo))
            {
                convertedMobileNo = "92";
                if (mobileNo.StartsWith("0"))
                {
                    convertedMobileNo = convertedMobileNo + mobileNo.Substring(1, mobileNo.Length - 1);
                }
                else
                    convertedMobileNo = mobileNo;
            }

            return convertedMobileNo;
        }

        public static bool isValidMsgContact(string actualContact, string actualMsgText)
        {
            bool isValid = false;
            if (actualContact != null && actualContact.Length == 12 && actualMsgText != null && actualMsgText != "")
            {
                isValid = true;
            }
            return isValid;
        }

        public static bool isValidateSmsMsg(int eventId, string userMessage)
        {
            bool isValid = true;
            string[] smsParamArray = userMessage.Split('{');
            int skipFirstIndex = 0;
            foreach (string smsParm in smsParamArray)
            {
                if (skipFirstIndex != 0)
                {
                    string smsParameter = smsParm.ToString().Split('}')[0];
                    smsParameter = "{" + smsParameter + "}";
                    SmsParam smsParamObj = SessionHelper.SmsParamsList.Where(x => x.ParamName == smsParameter).FirstOrDefault();
                    SmsEventParam smsEventParamObj = SessionHelper.SmsEventParamList.Where(x => x.SmsEventId == eventId && x.SmsParamId == smsParamObj.Id).FirstOrDefault();
                    if (smsEventParamObj == null || smsEventParamObj.Id == 0)
                    {
                        isValid = false;
                        break;
                    }
                }
                else
                    skipFirstIndex++;
            }

            return isValid;
        }

        //public static bool isValidateSmsMsg(int eventId, string userMessage)
        //{
        //    bool isValid = true;
        //    string[] smsParamArray = userMessage.Split('{');
        //    foreach (string smsParm in smsParamArray)
        //    {
        //        string smsParameter = smsParm.ToString().Split('}')[0];
        //        SmsParam smsParamObj = SessionHelper.SmsParamsList.Where(x => x.ParamName == smsParameter).FirstOrDefault();
        //        SmsEventParam smsEventParamObj = SessionHelper.SmsEventParamList.Where(x => x.SmsEventId == eventId && x.SmsParamId == smsParamObj.Id).FirstOrDefault();
        //        if (smsEventParamObj == null || smsEventParamObj.Id == 0)
        //        {
        //            isValid = false;
        //            break;
        //        }
        //    }

        //    return isValid;
        //}
        public static string getStudentFeeSmsMsg(string userMessage, string feeMonth, string toalFee, string paidFee, string pendingFee, string payDate, string dueDate)
        {
            string actualMsg = userMessage;
            if (actualMsg.Contains("{FeeMonth}"))
            {
                actualMsg = actualMsg.Replace("{FeeMonth}", feeMonth);
            }
            if (actualMsg.Contains("{TotalFee}"))
            {
                actualMsg = actualMsg.Replace("{TotalFee}", toalFee);
            }
            if (actualMsg.Contains("{PaidFee}"))
            {
                actualMsg = actualMsg.Replace("{PaidFee}", paidFee);
            }
            if (actualMsg.Contains("{PendingFee}"))
            {
                actualMsg = actualMsg.Replace("{PendingFee}", pendingFee);
            }
            if (actualMsg.Contains("{FeePaidDate}"))
            {
                actualMsg = actualMsg.Replace("{FeePaidDate}", payDate);
            }
            if (actualMsg.Contains("{FeeDueDate}"))
            {
                actualMsg = actualMsg.Replace("{FeeDueDate}", dueDate);
            }

            return actualMsg;
        }

        public static string getStudentAttendanceSmsMsg(string userMessage, string stdAttendaceStatus, string attendanceDate)
        {
            string actualMsg = userMessage;
            if (actualMsg.Contains("{AttendanceStatus}"))
            {
                actualMsg = actualMsg.Replace("{AttendanceStatus}", stdAttendaceStatus);
            }
            if (actualMsg.Contains("{AttendanceDate}"))
            {
                actualMsg = actualMsg.Replace("{AttendanceDate}", attendanceDate);
            }

            return actualMsg;
        }

        public static string getStudentInfoSmsMsg(string userMessage, Student std)
        {
            string actualMsg = userMessage;
            if (actualMsg.Contains("{StudentName}"))
            {
                actualMsg = actualMsg.Replace("{StudentName}", std.Name);
            }
            if (actualMsg.Contains("{FatherName}"))
            {
                actualMsg = actualMsg.Replace("{FatherName}", std.FatherName);
            }
            if (actualMsg.Contains("{RollNo}"))
            {
                actualMsg = actualMsg.Replace("{RollNo}", std.RollNumber);
            }
            if (actualMsg.Contains("{AdmissionNo}"))
            {
                actualMsg = actualMsg.Replace("{AdmissionNo}", std.AdmissionNo);
            }
            if (actualMsg.Contains("{ClassName}"))
            {
                actualMsg = actualMsg.Replace("{ClassName}", SessionHelper.classSecRepo.GetClassSectionById((int)std.ClassSectionId).Class.Name);
            }
            if (actualMsg.Contains("{SectionName}"))
            {
                actualMsg = actualMsg.Replace("{SectionName}", SessionHelper.classSecRepo.GetClassSectionById((int)std.ClassSectionId).Section.Name);
            }
            if (actualMsg.Contains("{LeavingDate}"))
            {
                actualMsg = actualMsg.Replace("{LeavingDate}", std.LeavingDate.ToString());
            }

            return actualMsg;
        }

        public static string getStaffInfoSmsMsg(string userMessage, Staff staff)
        {
            string actualMsg = userMessage;
            if (actualMsg.Contains("{StaffName}"))
            {
                actualMsg = actualMsg.Replace("{StaffName}", staff.Name);
            }
            if (actualMsg.Contains("{StaffFatherName}"))
            {
                actualMsg = actualMsg.Replace("{StaffFatherName}", staff.FatherName);
            }
            if (actualMsg.Contains("{StaffId}"))
            {
                actualMsg = actualMsg.Replace("{StaffId}", staff.StaffId.ToString());
            }
            if (actualMsg.Contains("{StaffMobileNo}"))
            {
                actualMsg = actualMsg.Replace("{StaffMobileNo}", staff.PhoneNumber);
            }
            if (actualMsg.Contains("{StaffDesignation}"))
            {
                actualMsg = actualMsg.Replace("{StaffDesignation}", SessionHelper.staffRepo.GetDesignationCategoryById((int)staff.DesignationId).CatagoryName);
            }
            
            return actualMsg;
        }

        public static string getStaffSalaryInfoSmsMsg(string userMessage, StaffSalary staffSalry)
        {
            string actualMsg = userMessage;
            if (actualMsg.Contains("{SalaryMonth}"))
            {
                actualMsg = actualMsg.Replace("{SalaryMonth}", staffSalry.ForMonth);
            }
            if (actualMsg.Contains("{SalaryDeduction}"))
            {
                actualMsg = actualMsg.Replace("{SalaryDeduction}", staffSalry.Deduction.ToString());
            }
            if (actualMsg.Contains("{AdvanceAdjustment}"))
            {
                actualMsg = actualMsg.Replace("{AdvanceAdjustment}", staffSalry.AdvanceAdjustment.ToString());
            }
            if (actualMsg.Contains("{TotalSalary}"))
            {
                actualMsg = actualMsg.Replace("{TotalSalary}", staffSalry.PaidAmount.ToString());
            }
            if (actualMsg.Contains("{GrossSalary}"))
            {
                actualMsg = actualMsg.Replace("{GrossSalary}", staffSalry.SalaryAmount.ToString());
            }
            if (actualMsg.Contains("{SalaryDate}"))
            {
                actualMsg = actualMsg.Replace("{SalaryDate}", staffSalry.PaidDate.ToString());
            }

            return actualMsg;
        }

        public static string getStaffAttendanceInfoSmsMsg(string userMessage, StaffAttandance staffAtt, string attendanceStatus)
        {
            string actualMsg = userMessage;
            if (actualMsg.Contains("{StaffAttendanceStatus}"))
            {
                actualMsg = actualMsg.Replace("{StaffAttendanceStatus}", attendanceStatus);
            }
            if (actualMsg.Contains("{StaffAttendanceDate}"))
            {
                actualMsg = actualMsg.Replace("{StaffAttendanceDate}", staffAtt.Date.ToString());
            }
            if (actualMsg.Contains("{StaffAttendanceTimeIn}"))
            {
                actualMsg = actualMsg.Replace("{StaffAttendanceTimeIn}", staffAtt.Time.ToString());
            }
            if (actualMsg.Contains("{StaffAttendanceTimeOut}"))
            {
                actualMsg = actualMsg.Replace("{StaffAttendanceTimeOut}", staffAtt.OutTime.ToString());
            }

            return actualMsg;
        }

        public static string getStudentSessionExamSmsMsg(string userMessage, List<DataSet> dsList, int AutoRemarks, string teacherRemmarks,  DateTime IssuedDate)
        {
            string actualMsg = userMessage;
            string examTermResulDetail = " ";
            int obtIndx = 0, totalIndx = 0;
            double sessionObtMarks = 0, sessionTotalMarks = 0;
            if (dsList.Count > 0)
            {
                for (int m = 0; m < dsList.Count; m++)
                {
                    DataSet ds = dsList[m];
                    for (int indx = 0; indx < ds.Tables.Count; indx++)
                    {
                        DataTable dt = ds.Tables[indx];

                        string examName = "";

                        if (dt.Columns.Contains("ExamTypeName"))
                        {
                            examName = "EXAM : " + dt.Rows[0]["ExamTypeName"].ToString().ToUpper();
                            obtIndx = 8;
                            totalIndx = 3;
                        }
                        else if (dt.Columns.Contains("TermName"))
                        {
                            examName = "TERM : " + dt.Rows[0]["TermName"].ToString().ToUpper();
                            obtIndx = 7;
                            totalIndx = 2;
                        }
                        else
                        {
                            examName = "SESSION RESULT";
                            obtIndx = 8;
                            totalIndx = 3;
                        }

                        double m_totalObtMarks = 0, m_totalSubMarks = 0;
                        string teacherRemarks = "", examGrade = "";
                        if (examName != "SESSION RESULT")
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                DataRow row = dt.Rows[i];
                                m_totalObtMarks += double.Parse(row[obtIndx].ToString());
                                m_totalSubMarks += double.Parse(row[totalIndx].ToString());
                                if(examName.Contains("EXAM : "))
                                {
                                    sessionObtMarks += double.Parse(row[obtIndx].ToString());
                                    sessionTotalMarks += double.Parse(row[totalIndx].ToString());
                                }
                            }
                        }
                        else
                        {
                            m_totalObtMarks = sessionObtMarks;
                            m_totalSubMarks = sessionTotalMarks;
                        }



                        if (AutoRemarks == 1)
                            teacherRemarks = SessionHelper.GetRemarks(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);
                        else
                            teacherRemarks = teacherRemmarks;


                        examGrade = SessionHelper.GetGrade(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);

                        examTermResulDetail = examTermResulDetail + " " + examName + " Obtained Marks: " + m_totalObtMarks + " Total Marks: " + m_totalSubMarks;

                        double overallPercenatge = Math.Round((double.Parse(m_totalObtMarks.ToString()) / double.Parse(m_totalSubMarks.ToString())) * 100, 2);

                        if (actualMsg.Contains("{ExamPercentage}"))
                        {
                            examTermResulDetail = examTermResulDetail + " Total Percentage: " + overallPercenatge.ToString();
                        }
                        if (actualMsg.Contains("{ExamGrade}"))
                        {
                            examTermResulDetail = examTermResulDetail + " Final Grade: " + examGrade;
                        }
                        if (actualMsg.Contains("{ExamRemarks}"))
                        {
                            examTermResulDetail = examTermResulDetail + " Final Remarks: " + teacherRemarks;
                        }
                    }
                }
            }

            if (actualMsg.Contains("{ExamName}"))
            {
                actualMsg = actualMsg.Replace("{ExamName}", "");
            }
            if (actualMsg.Contains("{ExamIssueDate}"))
            {
                actualMsg = actualMsg.Replace("{ExamIssueDate}", IssuedDate.ToString());
            }
            if (actualMsg.Contains("{ExamGrade}"))
            {
                actualMsg = actualMsg.Replace("{ExamGrade}", "");
            }
            if (actualMsg.Contains("{ExamPercentage}"))
            {
                actualMsg = actualMsg.Replace("{ExamPercentage}", "");
            }
            if (actualMsg.Contains("{ExamRemarks}"))
            {
                actualMsg = actualMsg.Replace("{ExamRemarks}", "");
            }
            
            actualMsg = actualMsg + examTermResulDetail;

            return actualMsg;
        }

        public static string getStudentTermExamSmsMsg(string userMessage, List<ExamResultViewModel> examdetail, int examTypeId, DateTime IssuedDate)
        {
            string actualMsg = userMessage;

            int totalMarks = 0;
            int obtainedMarks = 0;
            string examTermName = SessionHelper.dbContext.ExamTypes.Find(examTypeId).Name;
            string examTermResulDetail = " ";

            foreach (var exam in examdetail)
            {
                if (exam != null)
                {
                    examTermResulDetail = examTermResulDetail + exam.CourseName + ": " + exam.ObtMarks + "/" + exam.totalMarks;
                    if (actualMsg.Contains("{ExamPercentage}"))
                    {
                        examTermResulDetail = examTermResulDetail + " Percentage: " + Math.Round((double.Parse(exam.ObtMarks) / double.Parse(exam.totalMarks))*100, 2);
                    }
                    if (actualMsg.Contains("{ExamGrade}"))
                    {
                        examTermResulDetail = examTermResulDetail + " Grade: " + exam.Grade;
                    }
                    obtainedMarks += int.Parse(exam.ObtMarks);
                    totalMarks += int.Parse(exam.totalMarks);
                }
            }

            double overallPercenatge = Math.Round((double.Parse(obtainedMarks.ToString()) / double.Parse(totalMarks.ToString())) * 100, 2);
            string overallGrade = SessionHelper.GetGrade(obtainedMarks, totalMarks, 50);
            string remarks = SessionHelper.GetRemarks(obtainedMarks, totalMarks);

            examTermResulDetail = examTermResulDetail + " Total Marks: " + obtainedMarks + "/" + totalMarks;
            
            if (actualMsg.Contains("{ExamPercentage}"))
            {
                examTermResulDetail = examTermResulDetail + " Total Percentage: " + overallPercenatge.ToString();
            }
            if (actualMsg.Contains("{ExamGrade}"))
            {
                examTermResulDetail = examTermResulDetail + " Final Grade: " + overallGrade;
            }
            if (actualMsg.Contains("{ExamRemarks}"))
            {
                examTermResulDetail = examTermResulDetail + " Final Remarks: " + remarks;
            }



            
            if (actualMsg.Contains("{ExamName}"))
            {
                actualMsg = actualMsg.Replace("{ExamName}", examTermName);
            }
            if (actualMsg.Contains("{ExamIssueDate}"))
            {
                actualMsg = actualMsg.Replace("{ExamIssueDate}", IssuedDate.ToString());
            }
            if (actualMsg.Contains("{ExamGrade}"))
            {
                actualMsg = actualMsg.Replace("{ExamGrade}", "");
            }
            if (actualMsg.Contains("{ExamPercentage}"))
            {
                actualMsg = actualMsg.Replace("{ExamPercentage}", "");
            }
            if (actualMsg.Contains("{ExamRemarks}"))
            {
                actualMsg = actualMsg.Replace("{ExamRemarks}", "");
            }

            actualMsg = actualMsg + examTermResulDetail;

            return actualMsg;
        }

    }
}