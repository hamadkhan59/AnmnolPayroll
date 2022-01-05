using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class StudentModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ChallanToStdId { get; set; }
        public int LeavingStatusCode { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNumber { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string FatherCnic { get; set; }
        public string CurrentAddress { get; set; }
        public string LeavingRemarks { get; set; }
        public string LeavingReason { get; set; }
        public string LeavingStatus { get; set; }
        public string Contact_1 { get; set; }
        public string SrNo { get; set; }
        public bool IsPromoted { get; set; }
        public bool ClearDues { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime LeavingDate { get; set; }
        public byte[] StdImage { get; set; }

    }

    public class StudentAdmissionModel
    {
        public int Admitted { get; set; }
        public int Left { get; set; }
        public int Inquiries { get; set; }

    }

    public class ClassStudentAdmissionViewModel
    {
        public List<string> Class { get; set; }
        public List<StudentAdmissionModel> StudentAdmissionDetail { get; set; }
        public ClassStudentAdmissionViewModel()
        {
            Class = new List<string>();
            StudentAdmissionDetail = new List<StudentAdmissionModel>();
        }
    }

    public class StudentAdmissionViewModel
    {
        public List<string> Months { get; set; }
        public List<DateTime> Days { get; set; }
        public List<int> Admitted { get; set; }
        public List<int> Left { get; set; }
        public List<int> Inquiries { get; set; }

        public StudentAdmissionViewModel()
        {
            Months = new List<string>();
            Days = new List<DateTime>();
            Admitted = new List<int>();
            Left = new List<int>();
            Inquiries = new List<int>();
        }
    }
    public class StudentInquiryModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string InquiryNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string RollNumber { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string FatherCnic { get; set; }
        public string FatherContact { get; set; }
        public DateTime InquiryDate { get; set; }
        public byte[] StdImage { get; set; }


    }


}
