using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class ClassSubjectRepositoryImp : IClassSubjectRepository
    {
        private SC_WEBEntities2 dbContext1;

        public ClassSubjectRepositoryImp(SC_WEBEntities2 context)   
        {  
            dbContext1 = context;  
        }

        SC_WEBEntities2 dbContext
        {
            get
            {
                if (dbContext1 == null || this.disposed == true)
                    dbContext1 = new SC_WEBEntities2();
                this.disposed = false;
                return dbContext1;
            }
        }


        public int AddRegisterCourse(RegisterCourse registerCourse)
        {
            int result = -1;
            if (registerCourse != null)
            {
                dbContext.RegisterCourses.Add(registerCourse);
                dbContext.SaveChanges();
                result = registerCourse.RegisterCourseId;
            }

            return result;
        }

        public RegisterCourse GetRegisterCourseById(int RegisterCourseId)
        {
            return dbContext.RegisterCourses.Where(x => x.RegisterCourseId == RegisterCourseId).FirstOrDefault();
        }


        public RegisterCourse GetRegisterCourseByClassId(int classId)
        {
            return dbContext.RegisterCourses.Where(x => x.ClassSection.ClassId == classId).FirstOrDefault();
        }

        public RegisterCourse GetRegisterCourseListByClassId(int classId)
        {
            return dbContext.RegisterCourses.Where(x => x.ClassSection.ClassId == classId).FirstOrDefault();
        }

        public List<RegisterCourse> GetRegisterCourseByClassSectionId(int classSectionId)
        {
            return dbContext.RegisterCourses.Where(x => x.ClassSectionId == classSectionId).ToList();
        }

        public RegisterCourse GetRegisterCourseByClassSectionAndSUbjectId(int classSectionId, int subjectId)
        {
            return dbContext.RegisterCourses.Where(x => x.ClassSectionId == classSectionId && x.SubjectId == subjectId).FirstOrDefault();
        }

        //public RegisterCourse GetRegisterCourseByNameAndId(string RegisterCourseName, int RegisterCourseId)
        //{
        //    return dbContext.RegisterCoursees.Where(x => x.Name == RegisterCourseName && x.Id != RegisterCourseId).FirstOrDefault();
        //}

        public List<RegisterCourse> GetAllRegisterCourses()
        {
            return dbContext.RegisterCourses.ToList();    
        }

        public List<RegisterCourseModel> GetAllRegisterCoursesModel()
        {
            var query = from course in dbContext.RegisterCourses
                        join subj in dbContext.Subjects on course.SubjectId equals subj.Id
                        join clsec in dbContext.ClassSections on course.ClassSectionId equals clsec.ClassSectionId
                        join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        select new RegisterCourseModel
                        {
                            BranchId = course.BranchId,
                            ClassId = cls.Id,
                            ClassName = cls.Name,
                            ClassSectionId = clsec.ClassSectionId,
                            RegisterCourseId = course.RegisterCourseId,
                            ResultOrder = course.ResultOrder,
                            SectionId = sec.Id,
                            SectionName = sec.Name,
                            SubjectId = subj.Id,
                            SubjectName = subj.Name
                        };

            return query.Distinct().ToList();
                 
            //return dbContext.RegisterCourses.ToList();
        }

        public void UpdateRegisterCourse(RegisterCourse registerCourse)
        {
            if (registerCourse != null)
            {
                RegisterCourse sessionObject = dbContext.RegisterCourses.Find(registerCourse.RegisterCourseId);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(registerCourse);
                //dbContext.Entry(classSection).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteRegisterCourse(RegisterCourse registerCourse)
        {
            if (registerCourse != null)
            {
                dbContext.RegisterCourses.Remove(registerCourse);
                dbContext.SaveChanges();
            }
        }

        public List<SubjectModel> GetSubjectListByClass(int classId)
        {
            var query = from course in dbContext.RegisterCourses
                        join clsec in dbContext.ClassSections on course.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join subject in dbContext.Subjects on course.SubjectId equals subject.Id
                        where clas.Id == classId
                        select new SubjectModel
                        {
                            ClassId = clas.Id,
                            SubjectId = subject.Id,
                            SubjectName = subject.Name
                        };
            return query.Distinct().ToList();

        }

        public List<int> GetSubjectListByClassAndSectionName(string className, string sectionName)
        {
            var query = from course in dbContext.RegisterCourses
                        join clsec in dbContext.ClassSections on course.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id 
                        join subject in dbContext.Subjects on course.SubjectId equals subject.Id
                        where clas.Name == className && sec.Name == sectionName
                        select subject.Id;
            return query.Distinct().ToList();

        }

        public int GetSubjectCount(int subjectId)
        {
            return dbContext.RegisterCourses.Where(x => x.SubjectId == subjectId).Count();
        }

        public int GetTimeTableCount(int ClassSubjectId)
        {
            return dbContext.TimeTables.Where(x => x.ClassSubjectId == ClassSubjectId).Count();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }  
    }
}
