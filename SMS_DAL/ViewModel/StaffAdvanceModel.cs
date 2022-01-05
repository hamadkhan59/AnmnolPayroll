using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class StaffAdvanceModel
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string Remarks { get; set; }
    }

    public class StaffMiscWithdrawModel
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string ForMonth { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string Remarks { get; set; }
    }

    public partial class StaffModel
    {
        public int StaffId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int> MeritalStatusId { get; set; }
        public Nullable<int> ReligionId { get; set; }
        public Nullable<int> GenederId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string DesignationName { get; set; }
        public string CategoryName { get; set; }
        public string MartialStatus { get; set; }
        public string Religion { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<int> Salary { get; set; }
        public string ImageLocation { get; set; }
        public string SrNo { get; set; }
        public string CNIC { get; set; }
        public Nullable<System.DateTime> LeavingDate { get; set; }
        public string Education { get; set; }
        public string Year { get; set; }
        public string Marks_Or_Cgpa { get; set; }
        public string Total_Marks_Or_Cgpa { get; set; }
        public string Roll_Number { get; set; }
        public string Institute { get; set; }
        public Nullable<int> Allownces { get; set; }
        public byte[] StaffImage { get; set; }
        public string Nationality { get; set; }
        public string FatherPhoneNo { get; set; }
        public string FatherPhoneNo1 { get; set; }
        public string FatherOccupation { get; set; }
        public string FatherCNIC { get; set; }
        public string FatherEmail { get; set; }
        public string CurrentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string Email { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> YearlyLeaves { get; set; }
        public string BioMatricHash { get; set; }
        public Nullable<int> Deduction { get; set; }
        public Nullable<int> Advance { get; set; }
        public bool IsLeft { get; set; }
    }

    public partial class StaffAttandanceModel
    {
        public int Id { get; set; }
        public Nullable<int> StaffId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Time { get; set; }
        public string CodeName { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string OutTime { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
    }

    public partial class StaffAttandancePolicyModel
    {
        public int Id { get; set; }
        public string DesignationName { get; set; }
        public string LateInTime { get; set; }
        public string HalfDayTime { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int> LeaveInMonth { get; set; }
        public Nullable<int> LateInCount { get; set; }
        public Nullable<int> SalaryDeduction { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> LeaveInYear { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string EarlyOutTime { get; set; }
        public bool IsSalaryClubbed { get; set; }

    }

    public class StaffAttendanceDetailModel
    {
        public int Id { get; set; }
        public int AttendnaceId { get; set; }
        public int StaffId { get; set; }
        public DateTime AttendnaceDate { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }

    }

}
