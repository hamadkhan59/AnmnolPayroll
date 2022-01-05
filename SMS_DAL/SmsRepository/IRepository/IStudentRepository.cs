using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IStudentRepository : IDisposable
    {
        int AddStudent(Student student);
        string GetMaxSrNo();
        string GetMaxAdmissionNo();

        void UpdateStudent(Student student);
        Student GetStudentById(int studentId);
        StudentModel GetStudentModelByStudentId(int studentId);
        Student GetStudentByAdmissionNo(string admissionNo, int branchId);
        Student GetStudentBySrNo(string srNo);
        void DeleteStudent(Student student);
        SchoolConfig GetSchoolConfigById(int id);
		
		List<Student> GetStudentByParentCnic(string contactNo);
        List<StudentModel> GetAllStudentByParentCnic(string fatherCnic);

        List<StudentModel> SearchDischargedStudents(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo, string contactNo);
        List<StudentModel> SearchDischargedClassStudents(string rollNo, string studentName, string fatherName, int classId, string fatherCnic, int branchId, string admissionNo, string contactNo);
        List<StudentModel> SearchDischargedStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo, string contactNo);
      
        List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo, string contactNo);
        List<StudentModel> SearchClassStudents(string rollNo, string studentName, string fatherName, int classId, string fatherCnic, int branchId, string admissionNo, string contactNo);
        List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo, string contactNo);
        StudentModel SearchStudentsModel(string rollNo, string studentName, string fatherName, string fatherCnic, string admissionNo, int branchId);
        List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo);
        List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo);
        List<StudentModel> GetStudentByClassSectionId(int classSectionId);
        List<StudentModel> GetStudentByClassSectionIdAndExamYear(int classSectionId, int year);
        List<StudentModel> GetStudentByClassId(int classId);
        Student GetStudentByRollNoAndClassSectionId(string rollNO, int classSectionId);
        List<StudentModel> GetStudentByAdmissionDate(DateTime fromDate, DateTime toDate, int classId, int sectionId, int branchId);
        int GetStudentId(string rollNo, string studentName, string fatherName);
        void MakeClassSequential(int classSectionId);
        string GetMaxRollNo(int classId, int sectionId);
        int GetVanStrength(int driverId);
        int AddStudentInquiry(StudentInquiry studentInquiry);
        void UpdateStudentInquiry(StudentInquiry studentInquiry);
        StudentInquiry GetStudentInquiryById(int studentInquiryId);
        void DeleteStudentInquiry(StudentInquiry studentInquiry);
        string GetInquiryNumber(int branchId);
        List<StudentInquiryModel> SearchInquiryByInquiryNo(string inquiryNumber, int branchId);
        List<StudentInquiry> SearchStudentInquiryByInquiryNo(string inquiryNumber, int branchId);
        List<StudentInquiryModel> SearchStudentInquiry(int classId, string name, string fatherName, string fatherCnic, DateTime fromDate, DateTime toDate, int branchId);

        List<Relegion> GetAllReligion();
        List<Gender> GetAllGenders();
        List<StudentAttendanceStatu> GetAllAttendanceStatus();
        List<Session> GetAllSessions();
        List<TestStatu> GetAllTestStatus();
        List<AdmissionType> GetAllAdmissionTypes();
        void DeleteStudentDocs(int studentId);
        void AddStudentDocs(StudentDocument studentDoc);
        byte[] GetStudentImageDoc(int studentId, int imageId);

        List<string> GetFatherNameList();
        List<string> GetStudentNameList();

        List<StudentModel> SearchStudentsForSms(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo);
        List<StudentModel> SearchStudentsForSms(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo);
        StudentAdmissionModel GetStudentAdmissionData(DateTime fromDate, DateTime toDate, int branchId);
        StudentAdmissionViewModel GetAdmissionLineStats(int branchId, DateTime fromDate, DateTime toDate, string view = "month");
        ClassStudentAdmissionViewModel GetClassAdmissionStats(int branchId, DateTime fromDate, DateTime toDate);
    }
}
