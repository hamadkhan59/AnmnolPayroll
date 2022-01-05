using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface ISubjectRepository : IDisposable
    {
        int AddSubject(Subject subject);
        Subject GetSubjectByName(string SubjectName, int branchId);
        Subject GetSubjectByNameAndId(string SubjectName, int SubjectId, int branchId);
        void UpdateSubject(Subject subject);
        Subject GetSubjectById(int SubjectId);
        void DeleteSubject(Subject subject);
        List<Subject> GetAllSubjectes();
    }
}
