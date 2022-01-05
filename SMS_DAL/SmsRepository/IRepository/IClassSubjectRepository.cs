using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IClassSubjectRepository : IDisposable
    {
        int AddRegisterCourse(RegisterCourse registerCourse);
        //RegisterCourse GetRegisterCourseByName(string RegisterCourseName);
        //RegisterCourse GetRegisterCourseByNameAndId(string RegisterCourseName, int RegisterCourseId);
        void UpdateRegisterCourse(RegisterCourse registerCourse);
        RegisterCourse GetRegisterCourseById(int RegisterCourseId);
        RegisterCourse GetRegisterCourseByClassId(int classId);
        List<RegisterCourse> GetRegisterCourseByClassSectionId(int classSectionId);
        void DeleteRegisterCourse(RegisterCourse registerCourse);
        List<RegisterCourse> GetAllRegisterCourses();
        List<RegisterCourseModel> GetAllRegisterCoursesModel();
        RegisterCourse GetRegisterCourseByClassSectionAndSUbjectId(int classSectionId, int subjectId);
        List<SubjectModel> GetSubjectListByClass(int classId);
        List<int> GetSubjectListByClassAndSectionName(string className, string sectionName);
        int GetSubjectCount(int subjectId);
        int GetTimeTableCount(int ClassSubjectId);
    }
}
