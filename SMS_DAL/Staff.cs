//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS_DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Staff
    {
        public Staff()
        {
            this.SessionSubjects = new HashSet<SessionSubject>();
            this.StaffAdvances = new HashSet<StaffAdvance>();
            this.StaffAllownces = new HashSet<StaffAllownce>();
            this.StaffAttandances = new HashSet<StaffAttandance>();
            this.StaffBehaviours = new HashSet<StaffBehaviour>();
            this.StaffBehaviours1 = new HashSet<StaffBehaviour>();
            this.StaffDegrees = new HashSet<StaffDegree>();
            this.StaffMiscWithdraws = new HashSet<StaffMiscWithdraw>();
            this.StaffSalaries = new HashSet<StaffSalary>();
            this.StaffSalaryIncrementHistories = new HashSet<StaffSalaryIncrementHistory>();
            this.StudentBehaviours = new HashSet<StudentBehaviour>();
            this.StudentInquiries = new HashSet<StudentInquiry>();
            this.StudentInquiries1 = new HashSet<StudentInquiry>();
            this.TimeTables = new HashSet<TimeTable>();
            this.Users = new HashSet<User>();
        }
    
        public int StaffId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<int> MeritalStatusId { get; set; }
        public Nullable<int> ReligionId { get; set; }
        public Nullable<int> GenederId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber1 { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
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
        public Nullable<int> AdvanceInstallment { get; set; }
        public Nullable<int> DutyHours { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Designation Designation { get; set; }
        public virtual Gender Gender1 { get; set; }
        public virtual MeritalStatu MeritalStatu { get; set; }
        public virtual Relegion Relegion { get; set; }
        public virtual ICollection<SessionSubject> SessionSubjects { get; set; }
        public virtual StaffType StaffType { get; set; }
        public virtual ICollection<StaffAdvance> StaffAdvances { get; set; }
        public virtual ICollection<StaffAllownce> StaffAllownces { get; set; }
        public virtual ICollection<StaffAttandance> StaffAttandances { get; set; }
        public virtual ICollection<StaffBehaviour> StaffBehaviours { get; set; }
        public virtual ICollection<StaffBehaviour> StaffBehaviours1 { get; set; }
        public virtual ICollection<StaffDegree> StaffDegrees { get; set; }
        public virtual ICollection<StaffMiscWithdraw> StaffMiscWithdraws { get; set; }
        public virtual ICollection<StaffSalary> StaffSalaries { get; set; }
        public virtual ICollection<StaffSalaryIncrementHistory> StaffSalaryIncrementHistories { get; set; }
        public virtual ICollection<StudentBehaviour> StudentBehaviours { get; set; }
        public virtual ICollection<StudentInquiry> StudentInquiries { get; set; }
        public virtual ICollection<StudentInquiry> StudentInquiries1 { get; set; }
        public virtual ICollection<TimeTable> TimeTables { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
