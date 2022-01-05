using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class AttendanceModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int StudentId { get; set; }
        public int StatusId { get; set; }
        public string RollNumber { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string FatherName { get; set; }
        public string AdmissionNo { get; set; }
        public string Contact_1 { get; set; }
        public string Status { get; set; }
        public bool CurrentStatus { get; set; }
        public DateTime AttendanceDate { get; set; }

    }

    public class AttendanceRequestModel
    {
        public int Id { get; set; }
        public int DetailId { get; set; }
        public int AttendanceId { get; set; }
        public int AttendanceStatusId { get; set; }
        public int StudentId { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string StatusRequested { get; set; }
        public string RollNumber { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string AdmissionNo { get; set; }
        public string Contact_1 { get; set; }
        public string Comments { get; set; }
        public string Remarks { get; set; }
        public string RequestStatus { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApprovedBy { get; set; }

    }

    public class StaffAttendanceRequestModel
    {
        public int Id { get; set; }
        public int DetailId { get; set; }
        public int AttendanceId { get; set; }
        public int StaffId { get; set; }
        public int StatusId { get; set; }
        public int AttendanceStatusId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Designation { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        public string StatusRequested { get; set; }
        public string TimeIn { get; set; }
        public string AttendanceTimeIn { get; set; }
        public string TimeOut { get; set; }
        public string AttendanceTimeOut { get; set; }
        public string Comments { get; set; }
        public string Remarks { get; set; }
        public string RequestStatus { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public class AttendanceModel1
    {
        public string status { get; set; }
        public DateTime? date { get; set; }
        public string name { get; set; }

    }

    public class FeeModel
    {
        public string month;
        public decimal? amount;
        public DateTime? date;
        public string name;
    }

    public class FeeResponse
    {
        public string StatusCode { get; set; }
        public List<FeeModel> FeeDetail { get; set; }

    }

    public class StudentResponse
    {
        public string StatusCode { get; set; }
        public List<StudentModel> StudentDetail { get; set; }

    }

    public class AttendanceResponse
    {
        public string StatusCode { get; set; }
        public List<AttendanceModel1> Attendance { get; set; }

    }

    public class ParentResponse
    {
        public string StatusCode { get; set; }
        public AppUser Parent { get; set; }

    }
}
