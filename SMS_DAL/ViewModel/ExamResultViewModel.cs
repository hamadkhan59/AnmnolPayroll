using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SMS_DAL.ViewModel
{
    public class ExamResultViewModel
    {
        public int Id { get; set; }
        public int ExamTypeId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int LeavingStatus { get; set; }
        public string RollNumber { get; set; }
        public string AdmissionNo { get; set; }
        public int PassPercentage { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string ObtMarks { get; set; }
        public int Total { get; set; }
        public string totalMarks { get; set; }
        public string Grade { get; set; }
        public string CourseName { get; set; }
        public string actualPercentage { get; set; }
        public int Position { get; set; }
        public decimal Obtained { get; set; }
        public decimal ActualMarks { get; set; }
        public decimal TotalObtained { get; set; }
        public decimal Maximum { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ExamTermModel
    {
        public int Id { get; set; }
        public string TermName { get; set; }
    }

    public class ExamTermResponse
    {
        public string StatusCode { get; set; }
        public List<ExamTermModel> Terms { get; set; }
    }

    public class YearModel
    {
        public int Id { get; set; }
        public string Year1 { get; set; }
    }

    public class YearResponse
    {
        public string StatusCode { get; set; }
        public List<YearModel> Years { get; set; }
    }

    public class StudentSubjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StudentSubjecResponse
    {
        public string StatusCode { get; set; }
        public List<StudentSubjectModel> Subjects { get; set; }
    }


    public class ExamTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TermName { get; set; }
        public string Year { get; set; }
    }

    public class ExamTypeResponse
    {
        public string StatusCode { get; set; }
        public List<ExamTypeModel> Types { get; set; }
    }

    public class ExamResultResponse
    {
        public string StatusCode { get; set; }
        public List<ExamResultViewModel> ExamResults { get; set; }
    }

    public partial class RegisterCourseModel
    {
        public int RegisterCourseId { get; set; }
        public int ClassSectionId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int SubjectId { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
        public Nullable<int> ResultOrder { get; set; }
        public Nullable<int> BranchId { get; set; }
    }

    public partial class TermChapterModel
    {
        public int Id { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> ChapterId { get; set; }
        public Nullable<int> RegisterCourseId { get; set; }
        public Nullable<int> TermId { get; set; }
        public string TermName { get; set; }
        public int ExamTermYear { get; set; }
        public string SubjectChapterName { get; set; }

    }

    public class ExamPaperResponse
    {
        public string StatusCode { get; set; }
        public List<ExamPaperModel> ExamPapers { get; set; }
    }

    public partial class ExamPaperModel
    {
        public int Id { get; set; }
        public Nullable<int> ExamTypeId { get; set; }
        public Nullable<int> ClassSectionId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> CourseId { get; set; }
        public string UploadedPaperPath { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string SubjectName { get; set; }
        public string ClassName { get; set; }
        public string SectionNaem { get; set; }
        public string ExamTypeName { get; set; }
        public byte [] UploadedFile { get; set; }
    }


    public partial class DailyDairyModel
    {
        public int Id { get; set; }
        public Nullable<int> ClassSectionId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public string UploadedPaperPath { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ClassName { get; set; }
        public string SectionNaem { get; set; }
        public byte[] UploadedFile { get; set; }
    }

    public class FileModel
    {
        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public HttpPostedFileBase File { get; set; }
    }

}
