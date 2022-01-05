using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS_DAL
{
    public class AppConstHelper
    {
        public const string SUCCESS = "SUCCESS";
        public const string ATTENDANCE_GENERIC_ERROR = "Unable to get attendance detail, Please try again again later.";
        public const string ATTENDANCE_NO_RECORD_ERROR = "Unable to find any data with your search criteria, Please search with some other criteria.";

        public const string TERMS_GENERIC_ERROR = "Unable to get terms, Please try again again later.";
        public const string TERMS_NO_RECORD_ERROR = "No terms are defied, Please contact School.";

        public const string YEAR_GENERIC_ERROR = "Unable to get years, Please try again again later.";
        public const string YEAR_NO_RECORD_ERROR = "No years are defied, Please contact School.";

        public const string SUBJECT_GENERIC_ERROR = "Unable to get subject, Please try again again later.";
        public const string SUBJECT_NO_RECORD_ERROR = "No subjects are defied, Please contact School.";

        public const string TYPE_GENERIC_ERROR = "Unable to get exam types, Please try again again later.";
        public const string TYPE_NO_RECORD_ERROR = "No exam types are defied, Please contact School.";

        public const string RESULTS_GENERIC_ERROR = "Unable to get exam results, Please try again again later.";
        public const string RESULTS_NO_RECORD_ERROR = "No exam results are uploaded yet for this exam, Please contact School.";

        public const string FEE_GENERIC_ERROR = "Unable to get fee detail, Please try again again later.";
        public const string FEE_NO_RECORD_ERROR = "No fee detail is found for this criteria, Please contact School.";

        public const string STUDENTS_GENERIC_ERROR = "Unable to get student detail, Please try again again later.";
        public const string STUDENTS_NO_RECORD_ERROR = "No student detail is found, Please contact School.";

        public const string PARENTS_GENERIC_ERROR = "Unable to login, Please try again again later.";
        public const string PARENTS_INVALID_LOGIN = "Invalid Username or Password, please try again.";
        public const string PARENTS_ALREADY_EXIST = "Parent account already exist with this contact number";
        public const string PARENTS_NO_STUDENT = "There is no student registered with this contact number in school, for further details please contact school";
        public const string PARENTS_REGISTER_ERROR = "Unable to register now, please try again later";
        public const string STAFF_ALREADY_EXIST = "Staff account already exist with this contact number";
        public const string STAFF_NO_STUDENT = "There is no staff registered with this contact number in school, for further details please contact school";

        public const string DATESHEET_GENERIC_ERROR = "Unable to get date sheet, Please try again again later.";
        public const string DATESHEET_NO_RECORD_ERROR = "No date sheet is found for this exam, Please contact School.";

        public const string TIMETABLE_GENERIC_ERROR = "Unable to get time table, Please try again again later.";
        public const string TIMETABLE_NO_RECORD_ERROR = "No time table is found for this class, Please contact School.";

        public const string DAILYTEST_GENERIC_ERROR = "Unable to get daily tests, Please try again again later.";
        public const string DAILYTEST_NO_RECORD_ERROR = "No daily test is found in this search criteria, Please search with another criteria or contact School.";
        public const string DAILYTEST_EMPTY_MONTH = "No daily test is found in this months, Please contact School.";

        public const string ACTIVITYMARKS_GENERIC_ERROR = "Unable to get activties marks, Please try again again later.";
        public const string ACTIVITYMARKS_NO_RECORD_ERROR = "No activity marks is found in this search criteria, Please search with another criteria or contact School.";

        public const string EXAMPAPERS_GENERIC_ERROR = "Unable to get exam papers, Please try again again later.";
        public const string EXAMPAPERS_NO_RECORD_ERROR = "No exam paper is found in this search criteria, Please search with another criteria or contact School.";

        public const string ACTIVITYMARKS_EMPTY_MONTH = "No activity marks is found in this months, Please contact School.";
        public const string LOGIN_SUCCESS = "0";
        public const string LOGIN_ALREADY_EXIST = "1";
        public const string LOGIN_NO_DATA = "2";

        public const string STAFF_ATTENDANCE_GENERIC_ERROR = "Unable to get staff attendnace detail, Please try again again later.";
        public const string STAFF_ATTENDANCE_NO_RECORD_ERROR = "No staff attendnace detail is found, Please contact School.";

        public const string STAFF_ADVANCE_GENERIC_ERROR = "Unable to get staff advance detail, Please try again again later.";
        public const string STAFF_ADVANCE_NO_RECORD_ERROR = "No staff advance detail is found, Please contact School.";

        public const string STAFF_SALARY_GENERIC_ERROR = "Unable to get staff salary detail, Please try again again later.";
        public const string STAFF_SALARY_NO_RECORD_ERROR = "No staff salary detail is found, Please contact School.";

        public const string STAFF_STD_ATTENDANCE_GENERIC_ERROR = "Unable to get staff salary detail, Please try again again later.";
        public const string STAFF_STD_ATTENDANCE_NO_RECORD_ERROR = "No staff salary detail is found, Please contact School.";

        public const string STAFF_STD_ATTENDANCE_SAVE_ERROR = "Unable to save attendance, Please try again again later.";
        public const string STAFF_STD_ATTENDANCE_SUCCESS = "Attendance is saved succesfully";
    }
}