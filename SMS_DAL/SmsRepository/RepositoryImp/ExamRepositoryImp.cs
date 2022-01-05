using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Objects;
using SMS_DAL.ViewModel;
using System.Data.Objects.SqlClient;
using Logger;
using System.Reflection;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class ExamRepositoryImp : IExamRepository
    {
        private SC_WEBEntities2 dbContext1;

        IStudentRepository studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        public ExamRepositoryImp(SC_WEBEntities2 context)
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

        #region EXAM_TERM_REPO

		public List<Year> getAllYears()
        {
            return dbContext.Years.ToList();
        }

        public List<YearModel> getAllYearsModel()
        {
            var query = from year in dbContext.Years
                        select new YearModel
                        {
                            Id = year.Id,
                            Year1 = year.Year1
                        };

            return query.ToList();
        }

        public List<StudentSubjectModel> getStudentSubject(int studentId)
        {
            var student = dbContext.Students.Find(studentId);
            var query = from course in dbContext.RegisterCourses
                        join subj in dbContext.Subjects on course.SubjectId equals subj.Id
                        where course.ClassSectionId == student.ClassSectionId
                        select new StudentSubjectModel
                        {
                            Id = subj.Id,
                            Name = subj.Name
                        };

            return query.ToList();
        }

        public int AddExamTerm(ExamTerm examTerm)
        {
            int result = -1;
            if (examTerm != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ExamTerms.Add(examTerm);
                dbContext.SaveChanges();
                result = examTerm.Id;
            }

            return result;
        }

        public List<ExamTerm> GetExamTermByYear(int year, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTerms.Where(x => x.Year == year && x.BranchId == branchId).ToList();
        }

        public List<ExamTermModel> GetExamTermModelByYear(int year, int branchId)
        {
            var query = from term in dbContext.ExamTerms
                        where term.Year == year && term.BranchId == branchId
                        select new ExamTermModel
                        {
                            Id = term.Id,
                            TermName = term.TermName
                        };

            return query.ToList();
        }

        public int GetExamTermPercentageByYear(int year, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return (int)dbContext.ExamTerms.Where(x => x.Year == year && x.BranchId == branchId).Sum(x => x.Percentage);
        }

        public List<ExamTerm> GetAllExamTerm()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTerms.ToList();
        }

        public List<ExamTerm> GetExamTermByYearAndId(int year, int id, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTerms.Where(x => x.Year == year && x.Id != id && x.BranchId == branchId).ToList();
        }

        public List<ExamTerm> GetExamTermByYear(int year)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTerms.Where(x => x.Year == year).ToList();
        }

        public int GetExamTermPercentageByYearAndId(int year, int id, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return (int)dbContext.ExamTerms.Where(x => x.Year == year && x.Id != id && x.BranchId == branchId).Sum(x => x.Percentage);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Entry(examTerm).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public ExamTerm GetExamTermById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTerms.Find(id);
        }

        public void DeleteExamTerm(ExamTerm examTerm)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.ExamTerms.Remove(examTerm);
            dbContext.SaveChanges();
        }

        public int AddGrade(GradesConfig config)
        {
            int result = -1;
            if (config != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.GradesConfigs.Add(config);
                dbContext.SaveChanges();
                result = config.Id;
            }

            return result;
        }

        public void UpdateGrade(GradesConfig config)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Entry(config).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public GradesConfig GetGradeById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.GradesConfigs.Find(id);
        }

        public GradesConfig GetGradeByName(string grade)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.GradesConfigs.Where(x => x.Grade == grade).FirstOrDefault();
        }

        public GradesConfig GetGradeByNameAndId(string grade, int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.GradesConfigs.Where(x => x.Grade == grade && x.Id != id).FirstOrDefault();
        }

        public void DeleteGrade(GradesConfig config)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.GradesConfigs.Remove(config);
            dbContext.SaveChanges();
        }

        public List<GradesConfig> GetAllGrade()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.GradesConfigs.ToList();
        }


        public int AddRemarks(RemarksConfig config)
        {
            int result = -1;
            if (config != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.RemarksConfigs.Add(config);
                dbContext.SaveChanges();
                result = config.Id;
            }

            return result;
        }

        public void UpdateRemarks(RemarksConfig config)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Entry(config).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public RemarksConfig GetRemarksById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.RemarksConfigs.Find(id);
        }

        public RemarksConfig GetRemarksByName(string grade)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.RemarksConfigs.Where(x => x.Remarks == grade).FirstOrDefault();
        }

        public RemarksConfig GetRemarksByNameAndId(string grade, int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.RemarksConfigs.Where(x => x.Remarks == grade && x.Id != id).FirstOrDefault();
        }

        public void DeleteRemarks(RemarksConfig config)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.RemarksConfigs.Remove(config);
            dbContext.SaveChanges();
        }

        public List<RemarksConfig> GetAllRemarks()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.RemarksConfigs.ToList();
        }
        #endregion


        #region EXAM_TYPE_REPO

        public int AddExamType(ExamType examType)
        {
            int result = -1;
            if (examType != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ExamTypes.Add(examType);
                dbContext.SaveChanges();
                result = examType.Id;
            }

            return result;
        }

		public List<ExamType> GetExamTypeByExamTermsName(string termName)
        {
            string name = termName.ToString();
            dbContext.Configuration.LazyLoadingEnabled = false;
            ExamTerm examTerm = dbContext.ExamTerms.Where(x => x.TermName == name).FirstOrDefault();
            return dbContext.ExamTypes.Where(x => x.TermId == examTerm.Id).ToList();
        }
        public List<ExamType> GetExamTypeByExamTerms(int termId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTypes.Where(x => x.TermId == termId && x.BranchId == branchId).ToList();
        }

        public List<ExamTypeModel> GetExamTypeModel(string termName, int branchId)
        {
            var query = from etype in dbContext.ExamTypes
                        join term in dbContext.ExamTerms on etype.TermId equals term.Id
                        where term.TermName == termName && etype.BranchId == branchId
                        select new ExamTypeModel
                        {
                            Id = etype.Id,
                            Name = etype.Name
                        };
            return query.ToList();
        }

        public int GetExamTypePercentageByTerm(int termId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return (int)dbContext.ExamTypes.Where(x => x.TermId == termId && x.BranchId == branchId).Sum(x => x.Percent_Of_Total);
        }

        public List<ExamType> GetAllExamTypes()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTypes.ToList();
        }

        public List<ExamType> GetExamTypeByTermAndId(int termId, int id, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTypes.Where(x => x.TermId == termId && x.Id != id && x.BranchId == branchId).ToList();
        }

        public int GetExamTypePercentageByTermAndId(int termId, int id, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return (int)dbContext.ExamTypes.Where(x => x.TermId == termId && x.Id != id && x.BranchId == branchId).Sum(x => x.Percent_Of_Total);
        }

        public void UpdateExamType(ExamType examType)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Entry(examType).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public ExamType GetExamTypeById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ExamTypes.Find(id);
        }

        public void DeleteExamType(ExamType examType)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.ExamTypes.Remove(examType);
            dbContext.SaveChanges();
        }
        #endregion

        #region DATE_SHEET_REGION

        public List<DateSheet> GetDateSheetByClassAndExam(int classId, int sectionId, int examId)
        {
            return dbContext.DateSheets.Where(x => x.ClassId == classId && x.SectionId == sectionId && x.ExamId == examId).Include(x => x.Subject).ToList();
        }

        public List<DateSheetModel> GetDateSheetByClassAndExamModel(int classId, int sectionId, int examId)
        {
            var query = from sheet in dbContext.DateSheets
                        join exam in dbContext.ExamTypes on sheet.ExamId equals exam.Id
                        join subject in dbContext.Subjects on sheet.SubjectId equals subject.Id
                        where sheet.ClassId == classId && sheet.SectionId == sectionId
                        && exam.Id == examId
                        select new DateSheetModel
                        {
                            BranchId = exam.BranchId,
                            Center = sheet.Center,
                            ClassId = sheet.ClassId,
                            ExamDate = sheet.ExamDate, 
                            ExamId = sheet.ExamId, 
                            ExamName = exam.Name, 
                            ExamTime = sheet.ExamTime,
                            Id = sheet.Id,
                            SectionId = sheet.SectionId, 
                            SubjectId = sheet.SubjectId,
                            SubjectName = subject.Name
                        };

            return query.ToList();
        }


        public DateSheet GetDateSheetByClassAndExam(int classId, int sectionId, int examId, int subjectId)
        {
            return dbContext.DateSheets.Where(x => x.ClassId == classId && x.SectionId == sectionId && x.ExamId == examId && x.SubjectId == subjectId).FirstOrDefault();
        }

        public List<DateSheet> GetClassDateSheetByExam(int classId, int examId)
        {
            return dbContext.DateSheets.Where(x => x.ClassId == classId && x.SectionId == 0 && x.ExamId == examId).Include(x => x.Subject).ToList();
        }

        public List<DateSheetModel> GetClassDateSheetByExamModel(int classId, int examId)
        {
            var query = from sheet in dbContext.DateSheets
                        join exam in dbContext.ExamTypes on sheet.ExamId equals exam.Id
                        join subject in dbContext.Subjects on sheet.SubjectId equals subject.Id
                        where sheet.ClassId == classId 
                        && exam.Id == examId
                        select new DateSheetModel
                        {
                            BranchId = exam.BranchId,
                            Center = sheet.Center,
                            ClassId = sheet.ClassId,
                            ExamDate = sheet.ExamDate,
                            ExamId = sheet.ExamId,
                            ExamName = exam.Name,
                            ExamTime = sheet.ExamTime,
                            Id = sheet.Id,
                            SectionId = sheet.SectionId,
                            SubjectId = sheet.SubjectId,
                            SubjectName = subject.Name
                        };

            return query.ToList();
        }

        public List<DateSheet> GetClassDateSheetBySubjectAndExam(int classId, int examId, int subjectId)
        {
            return dbContext.DateSheets.Where(x => x.ClassId == classId && x.ExamId == examId && x.SubjectId == subjectId).ToList();
        }

        public DateSheet GetSingleDateSheetBySubjectAndExam(int classId, int examId, int subjectId)
        {
            return dbContext.DateSheets.Where(x => x.ClassId == classId && x.SectionId == 0 && x.ExamId == examId && x.SubjectId == subjectId).FirstOrDefault();
        }

        public int AddDateSheet(DateSheet dateSheet)
        {
            int result = -1;
            if (dateSheet != null)
            {
                dbContext.DateSheets.Add(dateSheet);
                dbContext.SaveChanges();
                result = dateSheet.Id;
            }

            return result;
        }

        public void UpdateDateSheet(DateSheet dateSheet)
        {
            dbContext.Entry(dateSheet).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void DeleteDateSheet(DateSheet dateSheet)
        {
            dbContext.DateSheets.Remove(dateSheet);
            dbContext.SaveChanges();
        }

        #endregion

        #region EXAM_RESULT

        public Exam GetExamByExamType(int examId, int classSectionId, int subjectId)
        {
            return dbContext.Exams.Where(x => x.ExamTypeId == examId && x.ClassSectionId == classSectionId && x.CourseId == subjectId).OrderBy(x => x.Subject.Name).FirstOrDefault();
        }

        public List<Exam> GetExamByExamType(int examId, int classSectionId)
        {
            return dbContext.Exams.Where(x => x.ExamTypeId == examId && x.ClassSectionId == classSectionId).ToList();
        }

        public List<Exam> GetExamByExamTypeId(int examId, int clasSectionId)
        {
            return dbContext.Exams.Where(x => x.ExamTypeId == examId && x.ClassSectionId == clasSectionId).ToList();
        }

        public List<ExamResult> GetExamResultByExamId(int examId)
        {
            return dbContext.ExamResults.Where(x => x.ExamId == examId).ToList();
        }

        public List<ExamResultViewModel> GetExamResultModelByExamId(int examId)
        {
            var query = from result in dbContext.ExamResults
                        join exam in dbContext.Exams on result.ExamId equals exam.Id
                        join subject in dbContext.Subjects on exam.CourseId equals subject.Id
                        join student in dbContext.Students on result.StudentId equals student.id
                        where result.ExamId == examId
                        select new ExamResultViewModel
                        {
                            AdmissionNo = student.AdmissionNo,
                            CourseId = (int)exam.CourseId,
                            CourseName = subject.Name, 
                            ExamTypeId = (int)exam.ExamTypeId,
                            FatherName = student.FatherName,
                            Id = (int)result.id,
                            Name = student.Name,
                            PassPercentage = (int)exam.PassPercentage,
                            RollNumber = student.RollNumber,
                            StudentId = student.id, 
                            Total = (int)exam.TotalMarks,
                            LeavingStatus = (int)student.LeavingStatus,
                            Obtained = Math.Round((decimal)result.ObtainedMarks, 2)
                        };

            var examList = query.ToList();
            if (examList != null && examList.Count > 0)
            {
                foreach (var exam in examList)
                {
                    exam.ObtMarks = exam.Obtained.ToString();
                    exam.totalMarks = exam.Total.ToString();
                    exam.ObtMarks = Math.Round((decimal)exam.Obtained, 2).ToString();
                    exam.ActualMarks = Math.Round((decimal)exam.Total, 2);
                    exam.totalMarks = exam.ActualMarks.ToString();
                }
            }

            return examList;
        }

        //public List<ExamResultViewModel> GetExamResultModelByClassSectionId(int classSectionId)
        //{
        //    var query = from result in dbContext.ExamResults
        //                join exam in dbContext.Exams on result.ExamId equals exam.Id
        //                join subject in dbContext.Subjects on exam.CourseId equals subject.Id
        //                join student in dbContext.Students on result.StudentId equals student.id
        //                where student.ClassSectionId == classSectionId
        //                select new ExamResultViewModel
        //                {
        //                    AdmissionNo = student.AdmissionNo,
        //                    CourseId = (int)exam.CourseId,
        //                    CourseName = subject.Name,
        //                    ExamTypeId = (int)exam.ExamTypeId,
        //                    FatherName = student.FatherName,
        //                    Id = (int)result.id,
        //                    Name = student.Name,
        //                    ObtainedMarks = (int)result.ObtainedMarks,
        //                    PassPercentage = (int)exam.PassPercentage,
        //                    RollNumber = student.RollNumber,
        //                    StudentId = student.id,
        //                    Total = (int)exam.TotalMarks,
        //                    LeavingStatus = (int)student.LeavingStatus
        //                };

        //    var examList = query.ToList();
        //    if (examList != null && examList.Count > 0)
        //    {
        //        foreach (var exam in examList)
        //        {
        //            exam.ObtMarks = exam.ObtainedMarks.ToString();
        //            exam.totalMarks = exam.Total.ToString();
        //        }
        //    }

        //    return examList;
        //}

        public int DeleteExamResult(int examId)
        {
            int errorCode = 200;
            try
            {
                var examReult = dbContext.ExamResults.Where(x => x.ExamId == examId).ToList();

                foreach (var result in examReult)
                {
                    dbContext.ExamResults.Remove(result);
                }

                var exam = dbContext.Exams.Find(examId);
                dbContext.Exams.Remove(exam);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 520;
            }

            return errorCode;
        }

        public void GetNewStudentForExam(int examId)
        {
            List<ExamResult> examList = GetExamResultByExamId(examId).OrderByDescending(x => x.StudentId).ToList();
            int maxId = (int)examList[0].StudentId;
            var student = dbContext.Students.Find(maxId);
            List<Student> studentList = dbContext.Students.Where(x => x.ClassSectionId == student.ClassSectionId && x.id > maxId && x.LeavingStatus == 1).ToList();

            if (studentList != null && studentList.Count > 0)
            {
                foreach (var std in studentList)
                {
                    ExamResult er = new ExamResult();
                    er.StudentId = std.id;
                    er.ObtainedMarks = 0;
                    er.ExamId = examList[0].ExamId;
                    er.CreatedOn = DateTime.Now;
                    dbContext.ExamResults.Add(er);
                }
                dbContext.SaveChanges();
            }

        }
        public ExamResult GetExamResultByExamAndStudentId(int examId, int studentId)
        {
            return dbContext.ExamResults.Where(x => x.ExamId == examId && x.StudentId == studentId).Include(x => x.Student).FirstOrDefault();
        }

        public ExamResultViewModel GetExamResultModelByExamAndStudentId(int examId, int studentId)
        {
            var query = from result in dbContext.ExamResults
                        join exam in dbContext.Exams on result.ExamId equals exam.Id
                        join subject in dbContext.Subjects on exam.CourseId equals subject.Id
                        join student in dbContext.Students on result.StudentId equals student.id
                        where result.ExamId == examId && student.id == studentId
                        select new ExamResultViewModel
                        {
                            AdmissionNo = student.AdmissionNo,
                            CourseId = (int)exam.CourseId,
                            CourseName = subject.Name,
                            ExamTypeId = (int)exam.ExamTypeId,
                            FatherName = student.FatherName,
                            Id = (int)result.id,
                            Name = student.Name,
                            PassPercentage = (int)exam.PassPercentage,
                            RollNumber = student.RollNumber,
                            StudentId = student.id,
                            Total = (int)exam.TotalMarks,
                            LeavingStatus = (int)student.LeavingStatus,
                            Obtained = Math.Round((decimal)result.ObtainedMarks, 2)
                        };

            var examList = query.ToList();
            if (examList != null && examList.Count > 0)
            {
                foreach (var exam in examList)
                {
                    exam.ObtMarks = exam.Obtained.ToString();
                    exam.totalMarks = exam.Total.ToString();
                    exam.ObtMarks = Math.Round((decimal)exam.Obtained, 2).ToString();
                    exam.ActualMarks = Math.Round((decimal)exam.Total, 2);
                    exam.totalMarks = exam.ActualMarks.ToString();
                }
            }

            return (examList == null || examList.Count == 0) ? null :  examList[0];
        }

        public int AddExam(Exam exam)
        {
            int result = -1;
            if (exam != null)
            {
                dbContext.Exams.Add(exam);
                dbContext.SaveChanges();
                result = exam.Id;
            }

            return result;
        }

        public void UpdateExam(Exam exam)
        {
            dbContext.Entry(exam).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int AddExamResult(ExamResult examResult)
        {
            int result = -1;
            if (examResult != null)
            {
                dbContext.ExamResults.Add(examResult);
                dbContext.SaveChanges();
                result = (int)examResult.id;
            }

            return result;
        }

        public void UpdateExamResult(ExamResult examResult)
        {
            dbContext.Entry(examResult).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public ExamResult GetExamResultById(int id)
        {
            return dbContext.ExamResults.Find(id);
        }

        public DataSet BuildStudentGrandList(string spQuery)
        {
            var resultds = new DataSet();
            var cmd = dbContext.Database.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = spQuery;

            dbContext.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            do
            {
                var tb = new DataTable();
                tb.Load(reader);
                resultds.Tables.Add(tb);
            } while (reader.IsClosed == false);

            dbContext.Database.Connection.Close();

            return resultds;
        }

        public List<Activity> GetAllActivity()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Activities.ToList();
        }

        public Activity GetActivityById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Activities.Find(id);
        }

        public void UpdateActivity(Activity activity)
        {
            dbContext.Entry(activity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int Addactivity(Activity activity)
        {
            int result = -1;
            if (activity != null)
            {
                dbContext.Activities.Add(activity);
                dbContext.SaveChanges();
                result = (int)activity.Id;
            }

            return result;
        }

        public Activity GetActivityByName(string name)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Activities.Where(x => x.ActivityName == name).FirstOrDefault();
        }

        public Activity GetActivityByNameAndId(string name, int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Activities.Where(x => x.ActivityName == name && x.Id != id).FirstOrDefault();
        }

        public void DeleteActivity(Activity activity)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Activities.Remove(activity);
            dbContext.SaveChanges();
        }


        public void AddActivityMarks(ActivityMark marks)
        {
            if (marks != null)
            {
                dbContext.ActivityMarks.Add(marks);
                dbContext.SaveChanges();
            }
        }

        public void UpdateActivityMarks(ActivityMark marks)
        {
            if (marks != null)
            {
                dbContext.Entry(marks).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteActivityMarks(ActivityMark marks)
        {
            if (marks != null)
            {
                dbContext.ActivityMarks.Remove(marks);
                dbContext.SaveChanges();
            }
        }

        public ActivityMark GetActivityMarksById(int id)
        {
            return dbContext.ActivityMarks.Find(id);
        }

        public List<ActivityMark> GetActivityMarksByActivityId(int activityId)
        {
            return dbContext.ActivityMarks.Where( x => x.ActivityId == activityId).ToList();
        }

        public List<ActivityMark> GetActivityMarksByClassSectionId(int classSectionId)
        {
            return dbContext.ActivityMarks.Where(x => x.ClassSectionId == classSectionId).ToList();
        }

        public ActivityMark GetActivityMarksByClassSectionAndActivtyId(int classSectionId, int activityId)
        {
            return dbContext.ActivityMarks.Where(x => x.ClassSectionId == classSectionId && x.ActivityId == activityId).FirstOrDefault();
        }

        public void AddActivityMarksDetail(ActivityMarksDetail marksDetail)
        {
            if (marksDetail != null)
            {
                dbContext.ActivityMarksDetails.Add(marksDetail);
                dbContext.SaveChanges();
            }
        }

        public void UpdateActivityMarksDetail(ActivityMarksDetail marksDetail)
        {
            if (marksDetail != null)
            {
                dbContext.Entry(marksDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteActivityMarksDetail(ActivityMarksDetail marksDetail)
        {
            if (marksDetail != null)
            {
                dbContext.ActivityMarksDetails.Remove(marksDetail);
                dbContext.SaveChanges();
            }
        }

        public ActivityMarksDetail GetActivityMarksDetailById(int id)
        {
            return dbContext.ActivityMarksDetails.Find(id);
        }

        public List<ActivityMarksDetail> GetActivityMarksDetailByActivityMarksId(int activityMarksId)
        {
            return dbContext.ActivityMarksDetails.Where(x => x.ActivityMarksId == activityMarksId).ToList();
        }

        public List<ActivityMarksViewModel> GetActivityMarksModelByActivityMarksId(int activityMarksId)
        {
            var query = from detail in dbContext.ActivityMarksDetails
                        join marks in dbContext.ActivityMarks on detail.ActivityMarksId equals marks.Id
                        join activity in dbContext.Activities on marks.ActivityId equals activity.Id
                        join student in dbContext.Students on detail.StudentId equals student.id
                        where detail.ActivityMarksId == activityMarksId
                        select new ActivityMarksViewModel
                        {
                            Id = detail.Id,
                            AdmissionNo = student.AdmissionNo,
                            Contact_1 = student.Contact_1,
                            FatherName = student.FatherName,
                            Name = student.Name,
                            RollNumber = student.RollNumber,
                            StudentId = student.id,
                            ObtMarks = Math.Round((decimal)(detail.ObtMarks == null ? 0 : detail.ObtMarks), 2),
                            TotalMarks = Math.Round((decimal)(marks.TotalMarks == null ? 0 : marks.TotalMarks), 2),
                            ActitivtyName = activity.ActivityName
                        };

            return query.ToList();
        }

        public List<ActivityMarksViewModel> GetActivityMarksModelByStudentId(int studentId)
        {
            var query = from detail in dbContext.ActivityMarksDetails
                        join marks in dbContext.ActivityMarks on detail.ActivityMarksId equals marks.Id
                        join activity in dbContext.Activities on marks.ActivityId equals activity.Id
                        join student in dbContext.Students on detail.StudentId equals student.id
                        where detail.StudentId == studentId
                        select new ActivityMarksViewModel
                        {
                            Id = detail.Id,
                            AdmissionNo = student.AdmissionNo,
                            Contact_1 = student.Contact_1,
                            FatherName = student.FatherName,
                            Name = student.Name,
                            RollNumber = student.RollNumber,
                            StudentId = student.id,
                            ObtMarks = (int)detail.ObtMarks,
                            TotalMarks = (int)marks.TotalMarks,
                            ActitivtyName = activity.ActivityName
                        };

            return query.ToList();
        }


        public List<ActivityMarksDetail> GetActivityMarksDetailByStudentId(int studentId)
        {
            return dbContext.ActivityMarksDetails.Where(x => x.StudentId == studentId).ToList();
        }


        public void AddDailyTest(DailyTest test)
        {
            if (test != null)
            {
                dbContext.DailyTests.Add(test);
                dbContext.SaveChanges();
            }
        }

        public void UpdateDailyTest(DailyTest test)
        {
            if (test != null)
            {
                dbContext.Entry(test).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteDailyTest(DailyTest test)
        {
            if (test != null)
            {
                dbContext.DailyTests.Remove(test);
                dbContext.SaveChanges();
            }
        }

        public DailyTest GetDailyTestById(int id)
        {
            return dbContext.DailyTests.Find(id);
        }

        public List<DailyTest> GetDailyTestBySubjectId(int subjectId)
        {
            return dbContext.DailyTests.Where(x => x.SubjectId == subjectId).ToList();
        }

        public List<DailyTest> GetDailyTestByClassSectionId(int classSectionId)
        {
            return dbContext.DailyTests.Where(x => x.ClassSectionId == classSectionId).ToList();
        }

        public DailyTest GetDailyTestByClassSectionAndSubjectId(int classSectionId, int subjectId)
        {
            return dbContext.DailyTests.Where(x => x.ClassSectionId == classSectionId && x.SubjectId == subjectId).FirstOrDefault();
        }

        public DailyTest GetDailyTestByClassSectionAndSubjectId(int classSectionId, int subjectId, DateTime testDate)
        {
            return dbContext.DailyTests.Where(x => x.ClassSectionId == classSectionId && x.SubjectId == subjectId
                && EntityFunctions.TruncateTime(x.TestDate) == testDate).FirstOrDefault();
        }

        public void AddDailyTestDetail(DailyTestsDetail testDetail)
        {
            if (testDetail != null)
            {
                dbContext.DailyTestsDetails.Add(testDetail);
                dbContext.SaveChanges();
            }
        }

        public void UpdateDailyTestDetail(DailyTestsDetail testDetail)
        {
            if (testDetail != null)
            {
                dbContext.Entry(testDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteDailyTestsDetail(DailyTestsDetail testDetail)
        {
            if (testDetail != null)
            {
                dbContext.DailyTestsDetails.Remove(testDetail);
                dbContext.SaveChanges();
            }
        }

        public DailyTestsDetail GetDailyTestsDetailById(int id)
        {
            return dbContext.DailyTestsDetails.Find(id);
        }

        public List<DailyTestsDetail> GetDailyTestsDetailByDailyTestId(int dailyTestId)
        {
            return dbContext.DailyTestsDetails.Where(x => x.DailyTestId == dailyTestId).ToList();
        }

        public List<DailyTestViewModel> GetDailyTestsModelByDailyTestId(int dailyTestId)
        {
            var query = from detail in dbContext.DailyTestsDetails
                        join test in dbContext.DailyTests on detail.DailyTestId equals test.Id
                        join student in dbContext.Students on detail.StudentId equals student.id
                        where detail.DailyTestId == dailyTestId
                        select new DailyTestViewModel
                        {
                            Id = detail.Id,
                            AdmissionNo = student.AdmissionNo,
                            Contact_1 = student.Contact_1,
                            FatherName = student.FatherName,
                            Name = student.Name,
                            RollNumber = student.RollNumber,
                            StudentId = student.id,
                            ObtMarks = Math.Round((decimal)(detail.ObtMarks == null ? 0 : detail.ObtMarks), 2),
                            TotalMarks = Math.Round((decimal)(test.TotalMarks == null ? 0 : test.TotalMarks), 2)
                        };

            return query.ToList();
        }


        public List<DailyTestViewModel> SearchDailyTest(int classId, int sectionId, int subjectId, DateTime fromDate, DateTime toDate)
        {
            var query = from test in dbContext.DailyTests
                        join clsec in dbContext.ClassSections on test.ClassSectionId equals clsec.ClassSectionId
                        join subject in dbContext.Subjects on test.SubjectId equals subject.Id
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where (classId == 0 || classId == clas.Id)
                        && (sectionId == 0 || sectionId == sec.Id)
                        && (subjectId == 0 || subjectId == subject.Id)
                        && ( EntityFunctions.TruncateTime(test.TestDate) >= fromDate &&
                        EntityFunctions.TruncateTime(test.TestDate) <= toDate)
                        select new DailyTestViewModel
                        {
                            TestDate = (DateTime) test.TestDate,
                            DailyTestId = test.Id,
                            Class = clas.Name,
                            Section = sec.Name,
                            Subject = subject.Name,
                            TotalMarks = (int)test.TotalMarks
                        };

            return query.ToList();
        }



        public List<DailyTestsDetail> GetDailyTestsDetailByStudentId(int studentId)
        {
            return dbContext.DailyTestsDetails.Where(x => x.StudentId == studentId).ToList();
        }


        #endregion


        public void AddDateSheetConfig(DateSheetConfig config)
        {
            dbContext.DateSheetConfigs.Add(config);
            dbContext.SaveChanges();
        }

        public void UpdateDateSheetConfig(DateSheetConfig config)
        {
            dbContext.Entry(config).State = EntityState.Modified;
            dbContext.SaveChanges();
        }


        public int GetExamTypeCount(int ExamTermId)
        {
            return dbContext.ExamTypes.Where(x => x.TermId == ExamTermId).Count();
        }

        public int GetDateSheetCount(int ExamTypeId)
        {
            int count =  dbContext.DateSheets.Where(x => x.ExamId == ExamTypeId).Count();
            if (count == 0)
            {
                count = dbContext.Exams.Where(x => x.ExamTypeId == ExamTypeId).Count();
            }
            return count;
        }

        public int GetActivityCount(int ActivityId)
        {
            return dbContext.ActivityMarks.Where(x => x.ActivityId == ActivityId).Count();
        }

        public DateSheetConfig GetDateSheetConfigByBranchId(int branchId)
        {
            return dbContext.DateSheetConfigs.Where(x => x.BranchId == branchId).FirstOrDefault();
        }

        public List<ExamResultViewModel> GetExamResult(int studentId, string examType)
        {
            var query = from exam in dbContext.Exams
                        join result in dbContext.ExamResults on exam.Id equals result.ExamId
                        join student in dbContext.Students on result.StudentId equals student.id
                        join subject in dbContext.Subjects on exam.CourseId equals subject.Id
                        join etype in dbContext.ExamTypes on exam.ExamTypeId equals etype.Id
                        where etype.Name == examType && student.id == studentId
                        select new ExamResultViewModel
                        {
                            Id = (int)result.id,
                            ExamTypeId = (int)exam.ExamTypeId,
                            CourseId = (int)exam.CourseId,
                            StudentId = (int)student.id,
                            RollNumber = student.RollNumber,
                            Name = student.Name,
                            FatherName = student.FatherName,
                            ObtMarks = SqlFunctions.StringConvert((double)result.ObtainedMarks),
                            totalMarks = SqlFunctions.StringConvert((double)exam.TotalMarks),
                            CourseName = subject.Name,
                            PassPercentage = (int)exam.PassPercentage
                            //Grade = GetGrade((int)result.ObtainedMarks, (int)exam.TotalMarks, (int)exam.PassPercentage)
                        };

            List<ExamResultViewModel> list = query.ToList();
            list.ToList().ForEach(x => { x.Grade = GetGrade(int.Parse(x.ObtMarks), int.Parse(x.totalMarks), x.PassPercentage); });
            return list;
        }

        public string GetGrade(int obtained, int totalMarks, int passPercentage)
        {
            var gradeList = dbContext.GradesConfigs.ToList();
            int percentage = 0;

            if(totalMarks > 0)
                percentage = (obtained * 100) / totalMarks;
            string obtGrade = "";

            if (gradeList != null && gradeList.Count() > 0)
            {
                foreach (var grade in gradeList)
                {
                    if (percentage >= grade.MinRange && percentage <= grade.MaxRange)
                    {
                        obtGrade = grade.Grade;
                    }
                }
            }

            return obtGrade;
        }

        public List<ExamTypeModel> GetAllCurrentYearExams()
        {
            int yearId = dbContext.Years.OrderByDescending(x => x.Id).FirstOrDefault().Id;

            var query = from etype in dbContext.ExamTypes
                        join term in dbContext.ExamTerms on etype.TermId equals term.Id
                        join year in dbContext.Years on term.Year equals year.Id
                        where year.Id == yearId
                        select new ExamTypeModel
                        {
                            Id = etype.Id,
                            Name = etype.Name,
                            TermName = term.TermName,
                            Year = year.Year1
                        };

            return query.ToList();
        }

        public List<ExamTypeModel> GetYearExams(int yearId, string termName)
        {
            var query = from etype in dbContext.ExamTypes
                        join term in dbContext.ExamTerms on etype.TermId equals term.Id
                        join year in dbContext.Years on term.Year equals year.Id
                        where (year.Id == yearId || yearId == 0)
                        && (termName == null || termName == "" || term.TermName == termName)
                        select new ExamTypeModel
                        {
                            Id = etype.Id,
                            Name = etype.Name,
                            TermName = term.TermName,
                            Year = year.Year1
                        };

            return query.Distinct().ToList();
        }

        public List<DateSheetViewModel> GetStudentDateSheet(int studentId, string examName)
        {
            var query = from sheet in dbContext.DateSheets
                        join etype in dbContext.ExamTypes on sheet.ExamId equals etype.Id
                        join subject in dbContext.Subjects on sheet.SubjectId equals subject.Id
                        join clas in dbContext.Classes on sheet.ClassId equals clas.Id
                        join sec in dbContext.Sections on sheet.SectionId equals sec.Id
                        join classec in dbContext.ClassSections on clas.Id equals classec.ClassId 
                        join student in dbContext.Students on classec.ClassSectionId equals student.ClassSectionId
                        where clas.Id == classec.ClassId && sec.Id == classec.SectionId
                        && student.id == studentId && etype.Name == examName
                        select new DateSheetViewModel
                        {
                            Id = sheet.Id,
                            ExamName = etype.Name, 
                            SubjectName = subject.Name,
                            ExamPlace = sheet.Center,
                            ExamTime = sheet.ExamTime,
                            ExamDate = (DateTime) sheet.ExamDate
                        };

            return query.Distinct().ToList();
        }

        public List<TimeTableViewModel> GetStudentTimeTable(int studentId)
        {
            var query = from table in dbContext.TimeTables
                        join staff in dbContext.Staffs on table.TeacherId equals staff.StaffId
                        join classsubj in dbContext.RegisterCourses on table.ClassSubjectId equals classsubj.RegisterCourseId
                        join subject in dbContext.Subjects on classsubj.SubjectId equals subject.Id
                        join classec in dbContext.ClassSections on table.ClassSectionId equals classec.ClassSectionId
                        join student in dbContext.Students on classec.ClassSectionId equals student.ClassSectionId
                        where student.id == studentId
                        select new TimeTableViewModel
                        {
                            Id = table.ID,
                            TeacherName = staff.Name,
                            SubjectName = subject.Name,
                            Slot = table.Slot
                        };

            var list = query.Distinct().ToList();
            foreach (var obj in list)
            {
                obj.Time = obj.Slot + " Slot";
            }

            return list;
        }

        public List<DailyTestViewModel> GetCurrentMonthDailyTest(int studentId)
        {
            DateTime currentDate = DateTime.Now;
            DateTime FromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime ToDate = GetToDate(currentDate);

            var query = from test in dbContext.DailyTests
                        join subject in dbContext.Subjects on test.SubjectId equals subject.Id
                        join detail in dbContext.DailyTestsDetails on test.Id equals detail.DailyTestId
                        join classec in dbContext.ClassSections on test.ClassSectionId equals classec.ClassSectionId
                        join student in dbContext.Students on classec.ClassSectionId equals student.ClassSectionId
                        where student.id == studentId && test.CreatedOn >= FromDate && test.CreatedOn <= ToDate
                        select new DailyTestViewModel
                        {
                            Id = test.Id,
                            TestDate = (DateTime) test.TestDate,
                            Subject = subject.Name,
                            TotalMarks = (decimal) test.TotalMarks,
                            ObtMarks = (decimal) detail.ObtMarks
                        };

            List<DailyTestViewModel> list = query.Distinct().ToList();
            list.ToList().ForEach(x => { x.Grade = GetGrade((int)x.ObtMarks, (int)x.TotalMarks, 0); });
            return list;
        }

        private DateTime GetToDate(DateTime currentDate)
        {
            DateTime ToDate = DateTime.Now;
            if (currentDate.Month == 12)
            {
                ToDate = new DateTime(currentDate.Year + 1, 1, 1);
                ToDate = ToDate.AddDays(-1);
            }
            else
            {
                ToDate = new DateTime(currentDate.Year, currentDate.Month + 1, 1);
                ToDate = ToDate.AddDays(-1);
            }

            return ToDate;
        }

        public List<DailyTestViewModel> GetDailyTest(int studentId, DateTime fromDate, DateTime toDate, int SubjectId)
        {
            var query = from test in dbContext.DailyTests
                        join subject in dbContext.Subjects on test.SubjectId equals subject.Id
                        join detail in dbContext.DailyTestsDetails on test.Id equals detail.DailyTestId
                        join classec in dbContext.ClassSections on test.ClassSectionId equals classec.ClassSectionId
                        join student in dbContext.Students on classec.ClassSectionId equals student.ClassSectionId
                        where student.id == studentId && test.CreatedOn >= fromDate && test.CreatedOn <= toDate
                        && (SubjectId == 0 || test.SubjectId == SubjectId)
                        select new DailyTestViewModel
                        {
                            Id = test.Id,
                            TestDate = (DateTime)test.TestDate,
                            Subject = subject.Name,
                            TotalMarks = (decimal)test.TotalMarks,
                            ObtMarks = (decimal)detail.ObtMarks
                        };

            List<DailyTestViewModel> list = query.Distinct().ToList();
            list.ToList().ForEach(x => { x.Grade = GetGrade((int)x.ObtMarks, (int)x.TotalMarks, 0); });
            return list;
        }

        public List<ActivityMarksViewModel> GetCurrentMonthActivityMarks(int studentId)
        {
            DateTime currentDate = DateTime.Now;
            DateTime FromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime ToDate = GetToDate(currentDate);

            var query = from marks in dbContext.ActivityMarks
                        join activity in dbContext.Activities on marks.ActivityId equals activity.Id
                        join detail in dbContext.ActivityMarksDetails on marks.Id equals detail.ActivityMarksId
                        join student in dbContext.Students on detail.StudentId equals student.id
                        where student.id == studentId && marks.CreatedOn >= FromDate && marks.CreatedOn <= ToDate
                        select new ActivityMarksViewModel
                        {
                            Id = marks.Id,
                            ActitivtyName = activity.ActivityName,
                            ActivityDate = (DateTime) marks.CreatedOn,
                            TotalMarks = (int)marks.TotalMarks,
                            ObtMarks = (int)detail.ObtMarks
                        };

            List<ActivityMarksViewModel> list = query.Distinct().ToList();
            list.ToList().ForEach(x => { x.Grade = GetGrade((int)x.ObtMarks, (int)x.TotalMarks, 0); });
            return list;
        }

        public List<ActivityMarksViewModel> GetActivityMarks(int studentId, DateTime fromDate, DateTime toDate, int activityId)
        {
            var query = from marks in dbContext.ActivityMarks
                        join activity in dbContext.Activities on marks.ActivityId equals activity.Id
                        join detail in dbContext.ActivityMarksDetails on marks.Id equals detail.ActivityMarksId
                        join student in dbContext.Students on detail.StudentId equals student.id
                        where student.id == studentId && marks.CreatedOn >= fromDate && marks.CreatedOn <= toDate
                        && (activity.Id == activityId || activityId == 0)
                        select new ActivityMarksViewModel
                        {
                            Id = marks.Id,
                            ActitivtyName = activity.ActivityName,
                            ActivityDate = (DateTime)marks.CreatedOn,
                            TotalMarks = (int)marks.TotalMarks,
                            ObtMarks = (int)detail.ObtMarks
                        };

            List<ActivityMarksViewModel> list = query.Distinct().ToList();
            list.ToList().ForEach(x => { x.Grade = GetGrade((int)x.ObtMarks, (int)x.TotalMarks, 0); });
            return list;
        }

        public int AddExamPaper(ExamPaper paper)
        {
            int result = -1;
            if (paper != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.ExamPapers.Add(paper);
                dbContext.SaveChanges();
                result = paper.Id;
            }

            return result;
        }

        public void UpdateExamPaper(ExamPaper paper)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Entry(paper).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public ExamPaperModel GetExamPaperModelById(int id)
        {
            var query = from paper in dbContext.ExamPapers
                        join type in dbContext.ExamTypes on paper.ExamTypeId equals type.Id
                        join subject in dbContext.Subjects on paper.CourseId equals subject.Id
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where paper.Id == id
                        select new ExamPaperModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CourseId = paper.CourseId,
                            CreatedOn = paper.CreatedOn,
                            ExamTypeId = paper.ExamTypeId,
                            ExamTypeName = type.Name,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            SubjectName = subject.Name,
                            UploadedPaperPath = paper.UploadedPaperPath,
                            UploadedFile = paper.UplodedFile
                        };
            return query.FirstOrDefault();
        }

        public List<ExamPaperModel> SearchExamPapers(int classId, int sectionId, int subjectId, int examTypeId)
        {
            var query = from paper in dbContext.ExamPapers
                        join type in dbContext.ExamTypes on paper.ExamTypeId equals type.Id
                        join subject in dbContext.Subjects on paper.CourseId equals subject.Id
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where (classId == 0 || clsec.ClassId == classId)
                        && (sectionId == 0 || clsec.SectionId == sectionId)
                        && (subjectId == 0 || subject.Id == subjectId)
                        && (examTypeId == 0 || paper.ExamTypeId == examTypeId)
                        select new ExamPaperModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CourseId = paper.CourseId,
                            CreatedOn = paper.CreatedOn,
                            ExamTypeId = paper.ExamTypeId,
                            ExamTypeName = type.Name,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            SubjectName = subject.Name,
                            UploadedPaperPath = paper.UploadedPaperPath
                        };
            return query.ToList();
        }

        public List<ExamPaperModel> SearchStudentExamPapers(int studentId, int examTypeId)
        {
            var query = from paper in dbContext.ExamPapers
                        join type in dbContext.ExamTypes on paper.ExamTypeId equals type.Id
                        join subject in dbContext.Subjects on paper.CourseId equals subject.Id
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        join std in dbContext.Students on clsec.ClassSectionId equals std.ClassSectionId
                        where std.id == studentId
                        && (examTypeId == 0 || paper.ExamTypeId == examTypeId)
                        select new ExamPaperModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CourseId = paper.CourseId,
                            CreatedOn = paper.CreatedOn,
                            ExamTypeId = paper.ExamTypeId,
                            ExamTypeName = type.Name,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            SubjectName = subject.Name,
                            UploadedPaperPath = paper.UploadedPaperPath
                        };
            return query.ToList();
        }


        public int AddDailyDairy(DailyDairy dairy)
        {
            int result = -1;
            if (dairy != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.DailyDairies.Add(dairy);
                dbContext.SaveChanges();
                result = dairy.Id;
            }

            return result;
        }

        public void UpdateDailyDairy(DailyDairy dairy)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Entry(dairy).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public DailyDairyModel GetDailyDairyModelById(int id)
        {
            var query = from paper in dbContext.DailyDairies
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where paper.Id == id
                        select new DailyDairyModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CreatedOn = paper.CreatedOn,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            UploadedPaperPath = paper.UploadedPaperPath,
                            UploadedFile = paper.UplodedFile
                        };
            return query.FirstOrDefault();
        }

        public List<DailyDairyModel> SearchDailyDairy(int classId, int sectionId, DateTime FileDate)
        {
            var query = from paper in dbContext.DailyDairies
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where (classId == 0 || clsec.ClassId == classId)
                        && (sectionId == 0 || clsec.SectionId == sectionId)
                        && EntityFunctions.TruncateTime(paper.CreatedOn) >= FileDate.Date
                        select new DailyDairyModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CreatedOn = paper.CreatedOn,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            UploadedPaperPath = paper.UploadedPaperPath
                        };
            return query.ToList();
        }

        public List<DailyDairyModel> SearchDailyDairy(int classId, int sectionId, DateTime fromDate, DateTime toDate)
        {
            var query = from paper in dbContext.DailyDairies
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where (classId == 0 || clsec.ClassId == classId)
                        && (sectionId == 0 || clsec.SectionId == sectionId)
                        && paper.CreatedOn >= fromDate && paper.CreatedOn <= toDate
                        select new DailyDairyModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CreatedOn = paper.CreatedOn,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            UploadedPaperPath = paper.UploadedPaperPath
                        };
            return query.ToList();
        }

        public List<DailyDairyModel> GetAllStudentDailyDairies(int studentId)
        {
            var query = from paper in dbContext.ExamPapers
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        join std in dbContext.Students on clsec.ClassSectionId equals std.ClassSectionId
                        where std.id == studentId
                        select new DailyDairyModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CreatedOn = paper.CreatedOn,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            UploadedPaperPath = paper.UploadedPaperPath
                        };
            return query.ToList();
        }

        public List<DailyDairyModel> SearchStudentDailyDairies(int studentId, DateTime fromDate, DateTime toDate)
        {
            var query = from paper in dbContext.ExamPapers
                        join clsec in dbContext.ClassSections on paper.ClassSectionId equals clsec.ClassSectionId
                        join clas in dbContext.Classes on clsec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        join std in dbContext.Students on clsec.ClassSectionId equals std.ClassSectionId
                        where std.id == studentId
                        && paper.CreatedOn >= fromDate && paper.CreatedOn <= toDate
                        select new DailyDairyModel
                        {
                            ClassId = clas.Id,
                            ClassName = clas.Name,
                            Id = paper.Id,
                            ClassSectionId = paper.ClassSectionId,
                            CreatedOn = paper.CreatedOn,
                            SectionId = sec.Id,
                            SectionNaem = sec.Name,
                            UploadedPaperPath = paper.UploadedPaperPath
                        };
            return query.ToList();
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
