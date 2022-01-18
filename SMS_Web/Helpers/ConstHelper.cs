using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS_Web.Helpers
{
    public class ConstHelper
    {
        #region Session_Const

        public const string SESSION_EDUCATION_HISTORY = "SESSION_EDUCATION_HISTORY";

        #endregion

        #region Student_Search_Const

        public const string GLOBAL_MIN_FEE = "GLOBAL_MIN_FEE";
        public const string GLOBAL_MAX_FEE = "GLOBAL_MAX_FEE";
        public const string GLOBAL_CLASS_ID = "GLOBAL_CLASS_ID";
        public const string GLOBAL_SECTION_ID = "GLOBAL_SECTION_ID";
        public const string GLOBAL_MONTH_ID = "GLOBAL_MONTH_ID";
        public const string GLOBAL_YEAR_ID = "GLOBAL_YEAR_ID";
        public const string GLOBAL_SUBJECT_ID = "GLOBAL_SUBJECT_ID";
        public const string GLOBAL_TERM_ID = "GLOBAL_TERM_ID";
        public const string GLOBAL_RESULT_TYPE_ID = "GLOBAL_RESULT_TYPE_ID";
        public const string GLOBAL_EXAM_TYPE_ID = "GLOBAL_EXAM_TYPE_ID";
        public const string GLOBAL_FROM_DATE = "GLOBAL_FROM_DATE";
        public const string GLOBAL_TO_DATE = "GLOBAL_TO_DATE";
        public const string GLOBAL_MARKS_ID = "GLOBAL_MARKS_ID";
        public const string GLOBAL_TEST_DATE = "GLOBAL_TEST_DATE";
        public const string GLOBAL_TEST_FROM_DATE = "GLOBAL_TEST_FROM_DATE";
        public const string GLOBAL_TEST_TO_DATE = "GLOBAL_TEST_TO_DATE";
        public const string STUDENT_ROLL_NO = "STUDENT_ROLL_NO";
        public const string STUDENT_NAME = "STUDENT_NAME";
        public const string STUDENT_FATHER_NAME = "STUDENT_FATHER_NAME";
        public const string STUDENT_CLASS_SECTION_ID = "STUDENT_CLASS_SECTION_ID";
        public const string STUDENT_FATHER_CNIC = "STUDENT_FATHER_CNIC";
        public const string STUDENT_ADMISSION_NO = "STUDENT_ADMISSION_NO";
        public const string STUDENT_CONTACT_NO = "STUDENT_CONTACT_NO";
        public const string CLASS_ID = "CLASS_ID";
        public const string CLASS_SECTION_ID = "CLASS_SECTION_ID";
        public const string CLASS_SUBJECT_ID = "CLASS_SUBJECT_ID";
        //public const string CERTIFICATES_CLASS_SECTION_ID = "CERTIFICATES_CLASS_SECTION_ID";
        //public const string CERTIFICATES_ROLL_NO = "CERTIFICATES_ROLL_NO";
        //public const string CERTIFICATES_NAME = "CERTIFICATES_NAME";
        //public const string CERTIFICATES_FATHER_NAME = "CERTIFICATES_FATHER_NAME";
        public const string CERTIFICATES_SEARCH_FLAG = "CERTIFICATES_SEARCH_FLAG";
        public const string CARDS_SEARCH_FLAG = "CARDS_SEARCH_FLAG";
        public const string RELEIVED_SEARCH_FLAG = "RELEIVED_SEARCH_FLAG";
        public const string STUDENT_IMAGE = "STUDENT_IMAGE";
        public const string SA_BEHAVIOUR_CATEGORIES = "Behaviour Categories";
        public const string SA_BEHAVIOUR_PARAMS = "Behaviour Parameters";
        public const string SA_BEHAVIOUR = "Student Behaviour";
        public const string CAT_FEE_RECEIVABLE = "Fee Receivables";
        public const string CAT_TEACHING_STAFF = "Teaching Staff";
        public const string CAT_NON_TEACHING_STAFF = "Non Teaching Staff";
        public const string CAT_ACCEDAMIC_STAFF = "Academic Staff Salaries";
        public const string CAT_NON_ACCEDAMIC_STAFF = "Non Academic Staff Salaries";
        public const int CAT_STAFF_SALARIES = 33;
        public const int CAT_STAFF_ADVANCES = 10;
        public const int CAT_RECEIVABLES = 1023;
        public const int CAT_BANK_FIRST_LVL = 7;
        public const int CAT_BANK_SECOND_LVL = 13;
        public const int CAT_BANK_THIRD_LVL = 13;

        public const int CAT_CASH_FIRST_LVL = 7;
        public const int CAT_CASH_SECOND_LVL = 13;
        public const int CAT_CASH_THIRD_LVL = 14;


        public const string BANK_FIRST_LVL = "BANK_FIRST_LVL";
        public const string BANK_SECOND_LVL = "BANK_SECOND_LVL";
        public const string BANK_THIRD_LVL = "BANK_THIRD_LVL";

        public const string CASH_FIRST_LVL = "CASH_FIRST_LVL";
        public const string CASH_SECOND_LVL = "CASH_SECOND_LVL";
        public const string CASH_THIRD_LVL = "CASH_THIRD_LVL";

        #endregion

        #region Attendance_Search_Const

        public const string ATTENDANCE_CLASS_ID = "ATTENDANCE_CLASS_ID";
        public const string ATTENDANCE_SECTION_ID = "ATTENDANCE_SECTION_ID";
        public const string ATTENDANCE_CLASS_SECTION_ID = "ATTENDANCE_CLASS_SECTION_ID";
        public const string ATTENDANCE_MARK_DATE = "ATTENDANCE_MARK_DATE";
        public const string ATTENDANCE_SEARCH_FLAG = "ATTENDANCE_SEARCH_FLAG";

        public const string ATTENDANCE_SHEET_CLASS_SECTION_ID = "ATTENDANCE_SHEET_CLASS_SECTION_ID";
        public const string ATTENDANCE_SHEET_FROM_DATE = "ATTENDANCE_SHEET_FROM_DATE";
        public const string ATTENDANCE_SHEET_TO_DATE = "ATTENDANCE_SHEET_TO_DATE";
        public const string ATTENDANCE_SHEET_SEARCH_FLAG = "ATTENDANCE_SHEET_SEARCH_FLAG";

        #endregion

        #region StudentInquiry_Search_Const

        public const string STUDENT_INQUIRY_CLASS_ID = "STUDENT_INQUIRY_CLASS_ID";
        public const string STUDENT_INQUIRY_NAME = "STUDENT_INQUIRY_NAME";
        public const string STUDENT_INQUIRY_FATHER_NAME = "STUDENT_INQUIRY_FATHER_NAME";
        public const string STUDENT_INQUIRY_FATHER_CNIC = "STUDENT_INQUIRY_FATHER_CNIC";
        public const string STUDENT_INQUIRY_INQUIRY_NO = "STUDENT_INQUIRY_INQUIRY_NO";
        public const string STUDENT_INQUIRY_FROM_DATE = "STUDENT_INQUIRY_FROM_DATE";
        public const string STUDENT_INQUIRY_TO_DATE = "STUDENT_INQUIRY_TO_DATE";
        public const string STUDENT_INQUIRY_SEARCH_FLAG = "STUDENT_INQUIRY_SEARCH_FLAG";

        #endregion

        #region Fee_Const
        public const string SEARCH_CHALLAN_ID = "SEARCH_CHALLAN_ID";
        public const string SEARCH_CHALLAN_FLAG = "SEARCH_CHALLAN_FLAG";
        public const string SEARCH_CHALLAN_CLASS_ID = "SEARCH_CHALLAN_CLASS_ID";
        public const string SEARCH_CHALLAN_SECTION_ID = "SEARCH_CHALLAN_SECTION_ID";
        public const string SEARCH_CHALLAN_MONTH_ID = "SEARCH_CHALLAN_MONTH_ID";
        public const string SEARCH_CHALLAN_YEAR_ID = "SEARCH_CHALLAN_YEAR_ID";
        public const string SEARCH_CHALLAN_ROLL_NO = "SEARCH_CHALLAN_ROLL_NO";
        public const string SEARCH_CHALLAN_NAME = "SEARCH_CHALLAN_NAME";
        public const string SEARCH_CHALLAN_FATHER_NAME = "SEARCH_CHALLAN_FATHER_NAME";
        public const string SEARCH_CHALLAN_FATHER_CNIC = "SEARCH_CHALLAN_FATHER_CNIC";
        public const string SEARCH_CHALLAN_CHALLAN_NO = "SEARCH_CHALLAN_CHALLAN_NO";
        public const string SEARCH_CHALLAN_CLASS_SCETION_ID = "SEARCH_CHALLAN_CLASS_SCETION_ID";
        public const string SEARCH_CHALLAN_ADMISSION_NO = "SEARCH_CHALLAN_ADMISSION_NO";
        public const string SEARCH_CHALLAN_CONTACT_NO = "SEARCH_CHALLAN_CONTACT_NO";
        public const string SEARCH_ISSUE_CHALLAN_FLAG = "SEARCH_ISSUE_CHALLAN_FLAG";
        public const string QUICK_ADMISSION_NO = "QUICK_ADMISSION_NO";
        public const string SEARCH_PAID_CHALLAN_FLAG = "SEARCH_PAID_CHALLAN_FLAG";
        public const string ISSUED_CHALLAN_DETAIL_LIST = "ISSUED_CHALLAN_DETAIL_LIST";
        public const string PAID_MONTH = "PAID_MONTH";
        public const string SEARCH_FAST_PAY = "SEARCH_FAST_PAY";
        public const string SIX_MONTH_DETAIL = "SIX_MONTH_DETAIL";
        public const string SEND_SMS_STUDENTSEARCH = "SEND_SMS_STUDENTSEARCH";
        public const string WAVE_OFF_MODEL = "WAVE_OFF_MODEL";
        public const string ADMISSION_NO_CHALLAN_MONTH = "ADMISSION_NO_CHALLAN_MONTH";

        public const string FEE_ARREARS_SEARCH_FLAG = "FEE_ARREARS_SEARCH_FLAG";
        public const string FEE_ARREARS_CLASS_ID = "FEE_ARREARS_CLASS_ID";
        public const string FEE_ARREARS_SECTION_ID = "FEE_ARREARS_SECTION_ID";
        public const string FEE_ARREARS_CLASS_SCETION_ID = "SEARCH_CHALLAN_CLASS_SCETION_ID";
        public const string FEE_ARREARS_ROLL_NO = "FEE_ARREARS_ROLL_NO";
        public const string FEE_ARREARS_NAME = "FEE_ARREARS_NAME";
        public const string FEE_ARREARS_FATHER_NAME = "FEE_ARREARS_FATHER_NAME";
        public const string FEE_ARREARS_FATHER_CNIC = "FEE_ARREARS_FATHER_CNIC";
        public const string STUDENT_EXTRA_CHARGES_SEARCH_FLAG = "STUDENT_EXTRA_CHARGES_SEARCH_FLAG";
        public const string STUDENT_EXTRA_CHARGES_FOR_MONTH = "STUDENT_EXTRA_CHARGES_FOR_MONTH";
        public const string STUDENT_EXTRA_CHARGES_CLASS_ID = "STUDENT_EXTRA_CHARGES_CLASS_ID";
        public const string STUDENT_EXTRA_CHARGES_SECTION_ID = "STUDENT_EXTRA_CHARGES_SECTION_ID";
        public const string STUDENT_EXTRA_CHARGES_ROLL_NO = "STUDENT_EXTRA_CHARGES_ROLL_NO";
        public const string STUDENT_EXTRA_CHARGES_AMOUNT = "STUDENT_EXTRA_CHARGES_AMOUNT";
        public const string STUDENT_EXTRA_CHARGES_FEE_HEAD_ID = "STUDENT_EXTRA_CHARGES_FEE_HEAD_ID";

        public const string SEARCH_ADMISSION_CHARGES_FROM_DATE = "SEARCH_ADMISSION_CHARGES_FROM_DATE";
        public const string SEARCH_ADMISSION_CHARGES_TO_DATE = "SEARCH_ADMISSION_CHARGES_TO_DATE";
        public const string SEARCH_ADMISSION_CHARGES_CLASS_ID = "SEARCH_ADMISSION_CHARGES_CLASS_ID";
        public const string SEARCH_ADMISSION_CHARGES_SECTION_ID = "SEARCH_ADMISSION_CHARGES_SECTION_ID";
        public const string SEARCH_ADMISSION_CHARGES_FLAG = "SEARCH_ADMISSION_CHARGES_FLAG";

        #endregion

        #region EXAM_CONST

        public const string SEARCH_DATE_SHEET_CLASS_ID = "SEARCH_DATE_SHEET_CLASS_ID";
        public const string SEARCH_DATE_SHEET_SECTION_ID = "SEARCH_DATE_SHEET_SECTION_ID";
        public const string SEARCH_DATE_SHEET_EXAM_ID = "SEARCH_DATE_SHEET_EXAM_ID";
        public const string SEARCH_DATE_SHEET_FLAG = "SEARCH_DATE_SHEET_FLAG";

        public const string GLOBAL_DATE_SHEET_CLASS_ID = "GLOBAL_DATE_SHEET_CLASS_ID";
        public const string GLOBAL_DATE_SHEET_SECTION_ID = "GLOBAL_DATE_SHEET_SECTION_ID";
        public const string GLOBAL_DATE_SHEET_YEAR_ID = "GLOBAL_DATE_SHEET_YEAR_ID";
        public const string GLOBAL_DATE_SHEET_TERM_ID = "GLOBAL_DATE_SHEET_TERM_ID";
        public const string GLOBAL_DATE_SHEET_EXAM_ID = "GLOBAL_DATE_SHEET_EXAM_ID";


        public const string SEARCH_ATTENDANCE_SHEET_CLASS_ID = "SEARCH_ATTENDANCE_SHEET_CLASS_ID";
        public const string SEARCH_ATTENDANCE_SHEET_SECTION_ID = "SEARCH_ATTENDANCE_SHEET_SECTION_ID";
        public const string SEARCH_ATTENDANCE_SHEET_EXAM_ID = "SEARCH_ATTENDANCE_SHEET_EXAM_ID";
        public const string SEARCH_ATTENDANCE_SHEET_SUBJECT_ID = "SEARCH_ATTENDANCE_SHEET_SUBJECT_ID";
        public const string SEARCH_ATTENDANCE_SHEET_FLAG = "SEARCH_ATTENDANCE_SHEET_FLAG";

        public const string SEARCH_EXAM_RESULT_CLASS_ID = "SEARCH_EXAM_RESULT_CLASS_ID";
        public const string SEARCH_EXAM_RESULT_CLASS_SECTION_ID = "SEARCH_EXAM_RESULT_CLASS_SECTION_ID";
        public const string SEARCH_EXAM_RESULT_SECTION_ID = "SEARCH_EXAM_RESULT_SECTION_ID";
        public const string SEARCH_EXAM_RESULT_EXAM_ID = "SEARCH_EXAM_RESULT_EXAM_ID";
        public const string SEARCH_EXAM_RESULT_SUBJECT_ID = "SEARCH_EXAM_RESULT_SUBJECT_ID";
        public const string SEARCH_EXAM_RESULT_FLAG = "SEARCH_EXAM_RESULT_FLAG";

        public const string SEARCH_CLASS_SHEET_CLASS_ID = "SEARCH_CLASS_SHEET_CLASS_SECTION_ID";
        public const string SEARCH_CLASS_SHEET_CLASS_SECTION_ID = "SEARCH_EXAM_RESULT_CLASS_SECTION_ID";
        public const string SEARCH_CLASS_SHEET_SECTION_ID = "SEARCH_CLASS_SHEET_SECTION_ID";
        public const string SEARCH_CLASS_SHEET_EXAM_ID = "SEARCH_CLASS_SHEET_EXAM_ID";
        public const string SEARCH_CLASS_SHEET_SUBJECT_ID = "SEARCH_CLASS_SHEET_SUBJECT_ID";
        public const string SEARCH_CLASS_SHEET_FLAG = "SEARCH_CLASS_SHEET_FLAG";

        public const string SEARCH_GRAND_SHEET_CLASS_ID = "SEARCH_GRAND_SHEET_CLASS_ID";
        public const string SEARCH_GRAND_SHEET_CLASS_SECTION_ID = "SEARCH_GRAND_SHEET_CLASS_SECTION_ID";
        public const string SEARCH_GRAND_SHEET_SECTION_ID = "SEARCH_GRAND_SHEET_SECTION_ID";
        public const string SEARCH_GRAND_SHEET_EXAM_ID = "SEARCH_GRAND_SHEET_EXAM_ID";
        public const string SEARCH_GRAND_SHEET_TERM_ID = "SEARCH_GRAND_SHEET_TERM_ID";
        public const string SEARCH_GRAND_SHEET_YEAR = "SEARCH_GRAND_SHEET_YEAR";
        public const string SEARCH_GRAND_RESULT_TYPE = "SEARCH_GRAND_RESULT_TYPE";
        public const string SEARCH_GRAND_SHEET_SUBJECT_ID = "SEARCH_GRAND_SHEET_SUBJECT_ID";
        public const string SEARCH_GRAND_SHEET_FLAG = "SEARCH_GRAND_SHEET_FLAG";
        public const string SEARCH_GRAND_STUDENT_ID = "SEARCH_GRAND_STUDENT_ID";

        public const string STUDENT_RESULT_CLASS_ID = "STUDENT_RESULT_CLASS_ID";
        public const string STUDENT_RESULT_CLASS_SECTION_ID = "STUDENT_RESULT_CLASS_SECTION_ID";
        public const string STUDENT_RESULT_SECTION_ID = "STUDENT_RESULT_SECTION_ID";
        public const string STUDENT_RESULT_EXAM_ID = "STUDENT_RESULT_EXAM_ID";
        public const string STUDENT_RESULT_TERM_ID = "STUDENT_RESULT_TERM_ID";
        public const string STUDENT_RESULT_YEAR = "STUDENT_RESULT_YEAR";
        public const string STUDENT_RESULT_SUBJECT_ID = "STUDENT_RESULT_SUBJECT_ID";
        public const string STUDENT_RESULT_STUDENT_ID = "STUDENT_RESULT_STUDENT_ID";
        public const string STUDENT_RESULT_FLAG = "STUDENT_RESULT_FLAG";
        public const string EXAM_PROMOTE_STUDENTSEARCH = "EXAM_PROMOTE_STUDENTSEARCH";

        #endregion

        #region SMS_MESSAGE

        public const string SMS_MESSAGE = "Message";
        public const string SMS_EVENT = "Sms Events";
        public const string SMS_EVENT_MESSAGES = "Sms Event Messages";
        public const string SMS_GEN_MESSAGE = "Send General Message";
        public const string SMS_UNP_MESSAGE = "Send Unpaid Message";

        //public const string SMS_EVENT_MSG_STD_ADMISSION = "Dear Parents your child has been Enrolled in school";
        //public const string SMS_EVENT_MSG_STD_LEAVING = "Dear Parents your child has been withdraw from school";
        //public const string SMS_EVENT_MSG_STD_ATTENDANCE_PRESENT = "Dear Parents your child is Present Today";
        //public const string SMS_EVENT_MSG_STD_ATTENDANCE_ABSENT = "Dear Parents your child is Absent Today";

        public const string SMS_VENDER_OUTREACH = "Outreach";
        public const string SMS_VENDER_SMSBIZ = "SmsBiz";

        public const string SMS_EVENT_NAME_STD_ADMISSION = "Student Admission";
        public const string SMS_EVENT_NAME_STD_LEAVING = "Student Leaving";
        public const string SMS_EVENT_NAME_STD_ATTENDANCE_PRESENT = "Student Attendance (Present)";
        public const string SMS_EVENT_NAME_STD_ATTENDANCE_ABSENT = "Student Attendance (Absent)";
        public const string SMS_EVENT_NAME_STD_FEE_COLLECTION = "Fee Collection";
        public const string SMS_EVENT_NAME_STD_FEE_DEFAULTER = "Fee Defaulter";
        public const string SMS_EVENT_NAME_STAFF_ATTENDANCE_PRESENT = "Staff Attendance (Present)";
        public const string SMS_EVENT_NAME_STAFF_ATTENDANCE_ABSENT = "Staff Attendance (Absent)";
        public const string SMS_EVENT_NAME_STAFF_SALARY_PAID = "Staff Salary Paid";
        public const string SMS_EVENT_NAME_EXAM_STUDENT_TERM_RESULT = "Exam Term Result For One Student";
        public const string SMS_EVENT_NAME_EXAM_STUDENT_SESSION_RESULT = "Exam Session Result For One Student";
        public const string SMS_EVENT_NAME_EXAM_ALL_STUDENT_TERM_RESULT = "Exam Term Result For All Student";
        public const string SMS_EVENT_NAME_EXAM_ALL_STUDENT_SESSION_RESULT = "Exam Session Result For All Student";
        public const string SMS_EVENT_NAME_STAFF_ADMISSION = "Staff Admission";
        public const string SMS_EVENT_NAME_GENERAL_MESSAGE_EVENT = "General Message Event";
        

        public const string SAMPLE_MSG_STD_NAME = "FAISAL HAMID";
        public const string SAMPLE_MSG_FATHER_NAME = "HAMID ALI";
        public const string SAMPLE_MSG_ROLL_NO = "63";
        public const string SAMPLE_MSG_ADMISSION_NO = "145";
        public const string SAMPLE_MSG_CLASS = "TENTH";
        public const string SAMPLE_MSG_SECTION = "BLUE";
        public const string SAMPLE_MSG_FEE_MONTH = "May-2020";
        public const string SAMPLE_MSG_TOTAL_FEE = "5500";
        public const string SAMPLE_MSG_PAID_FEE = "3500";
        public const string SAMPLE_MSG_PENDING_FEE = "2000";
        public const string SAMPLE_MSG_EXAM_NAME = "A_B_C";
        public const string SAMPLE_MSG_SUBJECT_NAME = "Math";
        public const string SAMPLE_MSG_TOTAL_MARKS = "75";
        public const string SAMPLE_MSG_OBT_MARKS = "72";
        public const string SAMPLE_MSG_GRADE = "+A";
        public const string SAMPLE_MSG_TOTAL_OBT_MARKS = "1050";
        public const string SAMPLE_MSG_GRAND_TOTAL_MARKS = "879";
        public const string SAMPLE_MSG_GRAND_GRADE = "+A";
        public const string SAMPLE_MSG_STAFF_NAME = "ALI RAZA";
        public const string SAMPLE_MSG_STAFF_FATHER_NAME = "RAZA ALI";
        public const string SAMPLE_MSG_STAFF_ID = "23";
        public const string SAMPLE_MSG_STAFF_MOBILE_NO = "923228059877";
        public const string SAMPLE_MSG_STAFF_DESIGNATION = "TEACHER";
        public const string SAMPLE_MSG_SALARY_MONTH = "MAY-2020";
        public const string SAMPLE_MSG_TOTAL_SALARY = "24000";
        public const string SAMPLE_MSG_SALARY_DEDUCTION = "1000";
        public const string SAMPLE_MSG_ADVANCE_ADJUSTMENT = "1000";
        public const string SAMPLE_MSG_GROSS_SALARY = "22000";
        public const string SAMPLE_MSG_SALARY_DATE = "20/05/2020";
        public const string SAMPLE_MSG_LEAVING_DATE = "10/05/2020";
        public const string SAMPLE_MSG_FEE_DATE = "12/05/2020";
        public const string SAMPLE_MSG_FEE_DUE_DATE = "18/05/2020";
        public const string SAMPLE_MSG_STD_ATTENDANCE_STATUS = "Present";
        public const string SAMPLE_MSG_STD_ATTENDANCE_DATE = "18/05/2020";
        public const string SAMPLE_MSG_STAFF_ATTENDANCE_STATUS = "Present";
        public const string SAMPLE_MSG_STAFF_ATTENDANCE_DATE = "18/05/2020";
        public const string SAMPLE_MSG_STAFF_ATTENDANCE_TIME_IN = "07:30:10";
        public const string SAMPLE_MSG_STAFF_ATTENDANCE_TIME_OUT = "15:30:10";
        public const string SAMPLE_MSG_EXAM_PERCENTAGE = "67.95";
        public const string SAMPLE_MSG_EXAM_REMARKS = "Satisfactory";
        public const string SAMPLE_MSG_EXAM_ISSUED_DATE = "18/05/2020";

        #endregion

        #region STAFF_CONSTS

        public const string GLOBAL_CATEGORY_ID = "GLOBAL_CATEGORY_ID";
        public const string GLOBAL_DESIGNATION_ID = "GLOBAL_DESIGNATION_ID";
        public const string STAFF_SEARCH_CATEGORY_ID = "STAFF_SEARCH_CATEGORY_ID";
        public const string STAFF_SEARCH_DESIGNATION_ID = "STAFF_SEARCH_DESIGNATION_ID";
        public const string STAFF_SEARCH_STAFF_ID = "STAFF_SEARCH_STAFF_ID";
        public const string STAFF_SEARCH_STAFF_NAME = "STAFF_SEARCH_STAFF_NAME";
        public const string STAFF_SEARCH_STAFF_FATHER_NAME = "STAFF_SEARCH_STAFF_FATHER_NAME";
        public const string STAFF_SEARCH_FLAG = "STAFF_SEARCH_FLAG";

        public const string STAFF_SALARY_SEARCH_CATEGORY_ID = "STAFF_SALARY_SEARCH_CATEGORY_ID";
        public const string STAFF_SALARY_SEARCH_DESIGNATION_ID = "STAFF_SALARY_SEARCH_DESIGNATION_ID";
        public const string STAFF_SALARY_SEARCH_STAFF_ID = "STAFF_SALARY_SEARCH_STAFF_ID";
        public const string SINGLE_STAFF_SALARY_SEARCH_STAFF_ID = "SINGLE_STAFF_SALARY_SEARCH_STAFF_ID";
        public const string STAFF_SALARY_SEARCH_STAFF_MONTH = "STAFF_SALARY_SEARCH_STAFF_MONTH";
        public const string SINGLE_STAFF_SALARY_SEARCH_STAFF_MONTH = "SINGLE_STAFF_SALARY_SEARCH_STAFF_MONTH";
        public const string STAFF_SALARY_SEARCH_STAFF_YEAR = "STAFF_SALARY_SEARCH_STAFF_YEAR";
        public const string SINGLE_STAFF_SALARY_SEARCH_STAFF_YEAR = "SINGLE_STAFF_SALARY_SEARCH_STAFF_YEAR";
        public const string STAFF_SALARY_SEARCH_FLAG = "STAFF_SALARY_SEARCH_FLAG";
        public const string SINGLE_STAFF_SALARY_SEARCH_FLAG = "SINGLE_STAFF_SALARY_SEARCH_FLAG";

        public const string SALARY_SHEET_SEARCH_CATEGORY_ID = "SALARY_SHEET_SEARCH_CATEGORY_ID";
        public const string SALARY_SHEET_SEARCH_DESIGNATION_ID = "SALARY_SHEET_SEARCH_DESIGNATION_ID";
        public const string SALARY_SHEET_SEARCH_STAFF_ID = "SALARY_SHEET_SEARCH_STAFF_ID";
        public const string SALARY_SHEET_SEARCH_FROM_DATE = "SALARY_SHEET_SEARCH_FROM_DATE";
        public const string SALARY_SHEET_SEARCH_TO_DATE = "SALARY_SHEET_SEARCH_TO_DATE";
        public const string SALARY_SHEET_SEARCH_FLAG = "SALARY_SHEET_SEARCH_FLAG";
        public const string SALARY_SHEET_LIST = "SALARY_SHEET_LIST";

        public const string SALARY_ATTENDANCE_SEARCH_CATEGORY_ID = "SALARY_ATTENDANCE_SEARCH_CATEGORY_ID";
        public const string SALARY_ATTENDANCE_SEARCH_DESIGNATION_ID = "SALARY_ATTENDANCE_SEARCH_DESIGNATION_ID";
        public const string SALARY_ATTENDANCE_SEARCH_STAFF_ID = "SALARY_ATTENDANCE_SEARCH_STAFF_ID";
        public const string SALARY_ATTENDANCE_SEARCH_FROM_DATE = "SALARY_ATTENDANCE_SEARCH_FROM_DATE";
        public const string SALARY_ATTENDANCE_SEARCH_TO_DATE = "SALARY_ATTENDANCE_SEARCH_TO_DATE";
        public const string SALARY_ATTENDANCE_MARK_DATE = "SALARY_ATTENDANCE_MARK_DATE";
        public const string SALARY_ATTENDANCE_SEARCH_FLAG = "SALARY_ATTENDANCE_SEARCH_FLAG";

        public const string STAFF_ATTENDANCE_SHEET_SEARCH_CATEGORY_ID = "STAFF_ATTENDANCE_SHEET_SEARCH_CATEGORY_ID";
        public const string STAFF_ATTENDANCE_SHEET_SEARCH_DESIGNATION_ID = "STAFF_ATTENDANCE_SHEET_SEARCH_DESIGNATION_ID";
        public const string STAFF_ATTENDANCE_SHEET_SEARCH_STAFF_ID = "STAFF_ATTENDANCE_SHEET_SEARCH_STAFF_ID";
        public const string STAFF_ATTENDANCE_SHEET_SEARCH_FROM_DATE = "STAFF_ATTENDANCE_SHEET_SEARCH_FROM_DATE";
        public const string STAFF_ATTENDANCE_SHEET_SEARCH_TO_DATE = "STAFF_ATTENDANCE_SHEET_SEARCH_TO_DATE";
        public const string STAFF_ATTENDANCE_SHEET_MARK_DATE = "STAFF_ATTENDANCE_SHEET_MARK_DATE";
        public const string STAFF_ATTENDANCE_SHEET_SEARCH_FLAG = "STAFF_ATTENDANCE_SHEET_SEARCH_FLAG";
        public const string STAFF_ATTENDANCE_REQUEST_LIST = "STAFF_ATTENDANCE_REQUEST_LIST";
        public const string STAFF_ATTENDANCE_APPROVE_REQUEST_LIST = "STAFF_ATTENDANCE_APPROVE_REQUEST_LIST";
        public const string STAFF_ATTENDANCE_REQUEST = "STAFF_ATTENDANCE_REQUEST";
        public const string STAFF_ATTENDANCE_DETAIL = "STAFF_ATTENDANCE_DETAIL";
        public const string ATTENDANCE_REQUEST_LIST = "ATTENDANCE_REQUEST_LIST";
        public const string ATTENDANCE_APPROVE_REQUEST_LIST = "ATTENDANCE_APPROVE_REQUEST_LIST";
        public const string ATTENDANCE_REQUEST = "ATTENDANCE_REQUEST";
        public const string ATTENDANCE_DETAIL = "ATTENDANCE_DETAIL";
        public const string ATTENDANCE_DETAIL_LIST = "ATTENDANCE_DETAIL_LIST";
        public const string ATTENDANCE_ID = "ATTENDANCE_ID";

        public const string STAFF_SALARY_INCREMENT_SEARCH_CATEGORY_ID = "STAFF_SALARY_INCREMENT_SEARCH_CATEGORY_ID";
        public const string STAFF_SALARY_INCREMENT_SEARCH_DESIGNATION_ID = "STAFF_SALARY_INCREMENT_SEARCH_DESIGNATION_ID";
        public const string STAFF_SALARY_INCREMENT_SEARCH_STAFF_ID = "STAFF_SALARY_INCREMENT_SEARCH_STAFF_ID";
        public const string STAFF_SALARY_INCREMENT_SEARCH_NAME = "STAFF_SALARY_INCREMENT_SEARCH_NAME";
        public const string STAFF_SALARY_INCREMENT_SEARCH_FATHER_NAME = "STAFF_SALARY_INCREMENT_SEARCH_FATHER_NAME";
        public const string STAFF_SALARY_INCREMENT_SEARCH_FLAG = "STAFF_SALARY_INCREMENT_SEARCH_FLAG";
        public const string STAFF_SALARY_INCREMENT_FLAG = "STAFF_SALARY_INCREMENT_FLAG";
        public const string STAFF_SALARY_INCREMENT_DETAIL_FLAG = "STAFF_SALARY_INCREMENT_DETAIL_FLAG";
        public const string STAFF_SALARY_INCREMENT_DATE = "STAFF_SALARY_INCREMENT_DATE";

        #endregion

        #region USER_CONSTS

        public const string SESSION_USER = "SESSION_USER";
        public const string USER_MODULE_PERMISSIONS = "USER_MODULE_PERMISSIONS";
        public const string USER_SUB_MODULE_PERMISSIONS = "USER_SUB_MODULE_PERMISSIONS";
        public const string USER_NAME = "USER_NAME";
        public const string BRANCH_ID = "BRANCH_ID";

        #endregion

        #region PERMISSIONS_CONST

        public const string SA_ADMISSION = "Student Admission";
        public const string SA_ATTENDANCE = "Attendance";
        public const string SA_ATTENDANCE_SHEET = "Attendance Sheet";
        public const string SA_CLASSES = "Classes";
        public const string SA_SECTIONS = "Sections";
        public const string SA_CLASS_SECTIONS = "Class Sections";
        public const string SA_SUBJECTS = "Subjects";
        public const string SA_CLASS_SUBJECTS = "Class Subjects";
        public const string SA_CERTIFICATES = "Certificates";
        public const string SA_CARDS = "Cards";
        public const string SA_STUDENT_LEAVING = "Student Leaving";
        public const string SA_STUDENT_INQUIRY = "Student Inquiry";
        public const string SA_REPORTS = "Reports";
        public const string SA_DISCHARGED_STUDENTS = "Discharged Students";
        public const string SA_PROCEED_ATTENDANCE = "Proceed Attendance";

        public const string FO_ISSUE_CHALLAN = "Issue Chalan";
        public const string FO_PAID_CHALLAN = "Paid Chalan";
        public const string FO_ADMISSION_CHARGES = "Admission Charges";
        public const string FO_STUDENT_CHALLAN = "Student Chalan";
        public const string FO_FEE_HEAD = "Fee Head";
        public const string FO_CHALLAN = "Chalan";
        public const string FO_CHALLAN_DETAIL = "Chalan Detail";
        public const string FO_FEE_INCREMENT = "Fee Increment";
        public const string FO_BANK_ACCOUNT = "Bank Account";
        public const string FO_FEE_REPORTS = "Fee Reports";
        public const string FO_PROCEED_MONTH = "Proceed Month";
        public const string FO_FAST_PAY = "Fast Pay";
        public const string FO_FEE_ARREARS = "Fee Arrears";
        public const string FO_STD_EXT_CHARGES = "Student Extra Charges";
        public const string FO_FINE_SETTINGS = "Fine Settings";
        public const string FO_FEE_EDIT = "Fee Edit";
        public const string FO_SINGLE_STUDENT_CHALLAN = "Single Student Chalan";
        public const string FO_CASH_ACCOUNT = "Cash Account";
        public const string FO_MONTHLY_WAVE_OFF = "Monthly Wave Off";


        public const string EC_SECTION_WISE = "Section Wise";
        public const string EC_CLASS_WISE = "Class Wise"; 
        public const string EC_DATE_SHEET_CONFIG = "Date Sheet Config"; 
        public const string EC_MARKS_SHEET = "Marks Sheet";
        public const string EC_CLASS_RESULT = "Class Result";
        public const string EC_GRAND_RESULT = "Grand Result";
        public const string EC_EXAM_ATTENDANCE_SHEET = "Exam Attendance Sheet";
        public const string EC_EXAM_AWARD_SHEET = "Award Sheet";
        public const string EC_ACTIVITY = "Activity";
        public const string EC_ACTIVITY_MARKS = "Activity Marks";
        public const string EC_DAILY_TESTS = "Daily Tests";
        public const string EC_DAILY_TESTS_SEARCH = "Daily Tests Search";
        public const string EC_TERMS = "Terms";
        public const string EC_GRADE = "Grade";
        public const string EC_EXAMS = "Exams";
        public const string EC_PROMOTE_STUDENT = "Promote Student";
        public const string EC_PROMOTED_STUDENT = "Promoted Student";
        public const string EC_UPLOAD_PAPERS = "Upload Papers";
        public const string EC_SEARCH_PAPERS = "Search Papers";

        public const string SH_NEW_STAFF = "New Staff";
        public const string SH_TEACHER_SUBJECTS = "Teacher Subjects";
        public const string SH_PAY_SALARIES = "Pay Salaries";
        public const string SH_PAID_SALARIES = "Paid Salaries";
        public const string SH_SALARY_SHEET = "Salary Sheet";
        public const string SH_SALARY_INCREMENT = "Salary Inrement";
        public const string SH_STAFF_ATTENDANCE = "|Staff Attendance|";
        public const string SH_STAFF_ATTENDANCE_SHEET = "Staff Attendance Sheet";
        public const string SH_STAFF_ATTENDANCE_POLICY = "|Staff Attendance Policy|";
        public const string SH_STAFF_CERTIFICATE = "Staff Certificate";
        public const string SH_SESSION = "Session";
        public const string SH_DESIGNATION_CATEGORY = "Designation Catagory";
        public const string SH_DESIGNATION = "|Designation|";
        public const string SH_ALLOWNCES = "Allownces";
        public const string SH_STAFF_REPORTS = "Staff Reports";
        public const string SH_ADVANCE = "New Advance";
        public const string SH_SEARCH_ADVANCE = "Search Advance";
        public const string SH_BEH_CAT = "Staff Behaviour Categories";
        public const string SH_BEH_PARAMS = "Staff Behaviour Parameters";
        public const string SH_ST_BEH = "Staff Behaviour";
        public const string SH_STAFF_VIEW_EDIT = "Salary View OR Edit";
        public const string SH_TIME_TABLE = "Time Table";
        public const string SH_STAFF_LEAVING = "Staff Leaving";
        public const string SH_STAFF_HOLIDAYS = "Staff Holidays";
        public const string SH_STAFF_NEW_WITHDRAW = "New Withdraw";
        public const string SH_STAFF_SEARCH_WITHDRAW = "Search Withdraw";
        public const string SH_STAFF_INSTANT_WITHDRAW = "Instant Withdraws";

        public const string SECA_USER_GROUP = "User Group";
        public const string SECA_Users = "Users";
        public const string SECA_Permissions = "Permissions";
        public const string SECA_Branch = "Branch";

        public const string TR_DRIVER = "Driver";
        public const string TR_TRANS_STOP = "Transport Stop";
        public const string TR_DRIVER_STOP = "Driver Stop";

        public const string AD_ATTENDANCE_DASHBOARD = "Attendance Dashboard";
        public const string AD_FEE_DASHBOARD = "Fee Dashboard";
        public const string AD_STAFF_DASHBOARD = "Staff Dasboard";
        public const string AD_STAFF_ATTENDANCE_DASHBOARD = "Staff Attendance Dashboard";
        public const string AD_FINANCE_DASHBAORD = "Finance Dashboard";
        public const string AD_ADMISSION_DASHBOARD = "Admission Dashboard";
        public const string AD_COURSE_COVERAGE = "Course Coverage";
        public const string AD_STAFF_COURSE_COVERAGE = "Staff Course Coverage";

        public const string AP_STUDENT_ATTENDANCE_REQ = "Student Attendance Req";
        public const string AP_STAFF_ATTENDANCE_REQ = "Staff Attendance Req";

        public const string SC_EXAM_CONFIG = "Exam Configuration";
        public const string SC_FEE_CONFIG = "Fee Configuration";

        #endregion

        #region FINANCE_CONSTS

        public const string ENTRY_DEBIT_DETAIL = "ENTRY_DEBIT_DETAIL";
        public const string FA_FINANCE_CATEGORY = "Finance Catagory";
        public const string FA_FINANCE_MAIN_ACCOUNT = "Finance Main Account";
        public const string FA_FINANCE_SUB_ACCOUNT = "Finance Sub Account";
        public const string FA_FINANCE_SUB_SUB_ACCOUNT = "Finance Sub Sub Account";
        public const string FA_FINANCE_DETAIL_ACCOUNT = "Finance Detail Account";
        public const string FA_FINANCE_JOURNAL_ENTRY = "Journal Entry";
        public const string FA_FINANCE_JOURNAL_INQUIRY = "Journal Inquiry";
        public const string FA_ADJUST_PAYABLE_VOUCHER = "Adjust Payable Voucher";
        public const string FA_BANK_PAYMENT_VOUCHER = "Bank Payment Voucher";
        public const string FA_CASH_PAYMENT_VOUCHER = "Cash Payment Voucher";
        public const string FA_BANK_RECEIPT_VOUCHER = "Bank Receipt Voucher";
        public const string FA_CASH_RECEIPT_VOUCHER = "Cash Receipt Voucher";
        public const string FA_FINANCE_REPORTS = "Finance Reports";
        public const string FA_CAPITAL_ACCOUNTS = "Capital Account";

        public const string PC_PETTY_ACCOUNTS = "Petty Cash Accounts";
        public const string PC_PETTY_PAYMENTS = "Petty Cash Inquiry";
        public const string PC_PETTY_RECEIPTS = "Petty Cash Payments";
        public const string PC_PETTY_INQUIRY = "Petty Cash Receipts";
        public const string PC_PETTY_REPORTS = "Petty Cash Reports";

        #endregion

        public const int LOGIN_BRNACH_ID = 1;
        public const int CAPITAL_ACCOUNT_ID = 23;
        public const int CAPITAL_ACCOUNT_TYPE = 1;
        public const int ATTENDANCE_REQUEST_APPROVED = 1;
        public const int ATTENDANCE_REQUEST_REJECTED = 2;
        public const int ATTENDANCE_REQUEST_PENDING = 3;

        public const int FINANCE_CATEGORY = 1;
        public const int FINANCE_MAIN_ACCOUNT = 2;
        public const int FINANCE_SUB_ACCOUNT = 3;
        public const int FINANCE_SUB_SUB_ACCOUNT = 4;
        public const int FINANCE_DETAIL_ACCOUNT = 5;
        public const int FINANCE_REVENUE_ACCOUNT = 5;
        public const int FINANCE_EXPENSE_ACCOUNT = 5;

        public const string FORMATE_PDF = ".pdf";
        public const string FORMATE_WORD = ".docx";
        public const string FORMATE_PNG = ".png";
        public const string FORMATE_JPG = ".jpg";
        public const string FORMATE_JPEG = ".jpeg";

        public const string ET_JE = "JE";
        public const string ET_BPV = "BPV";
        public const string ET_CPV = "CPV";
        public const string ET_BRV = "BRV";
        public const string ET_CRV = "CRV";

        public const string STM_ITEM_PURCHASE_LIST = "ITEM_PURCHASE_LIST";

    }
}
