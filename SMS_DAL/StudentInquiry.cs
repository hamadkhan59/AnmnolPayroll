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
    
    public partial class StudentInquiry
    {
        public int ID { get; set; }
        public string InquiryNumber { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<System.DateTime> AdmissionDate { get; set; }
        public string Contact_1 { get; set; }
        public string CurrentAddress { get; set; }
        public Nullable<int> ReligionCode { get; set; }
        public Nullable<int> GenderCode { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string ImageLocation { get; set; }
        public string FatherOccupation { get; set; }
        public string Nationality { get; set; }
        public string Cast { get; set; }
        public string FatherCNIC { get; set; }
        public string Contact_2 { get; set; }
        public string PermanentAddress { get; set; }
        public Nullable<int> SessionId { get; set; }
        public string MotherName { get; set; }
        public string MotherContact1 { get; set; }
        public string MotherContact2 { get; set; }
        public string Email { get; set; }
        public byte[] StdImage { get; set; }
        public string BFormNo { get; set; }
        public string MotherCnic { get; set; }
        public Nullable<int> RegisterationFee { get; set; }
        public Nullable<int> ProspectusFee { get; set; }
        public string MotherOccupation { get; set; }
        public string GuardianName { get; set; }
        public string GuardinCnic { get; set; }
        public string GuardinContact { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public Nullable<int> TakenBy { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<int> PassPercentage { get; set; }
        public Nullable<int> TotalMarks { get; set; }
        public Nullable<int> ObtainedMarks { get; set; }
        public Nullable<int> TestStatus { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Class Class { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Relegion Relegion { get; set; }
        public virtual Session Session { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Staff Staff1 { get; set; }
        public virtual TestStatu TestStatu { get; set; }
    }
}
