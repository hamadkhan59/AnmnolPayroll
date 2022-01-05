using Logger;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class ClassSectionRepositoryImp : IClassSectionRepository
    {
        private SC_WEBEntities2 dbContext1;

        public ClassSectionRepositoryImp(SC_WEBEntities2 context)
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

        public int GetClassSectionId(int classId, int sectionId)
        {
            return dbContext.ClassSections.Where(x => x.ClassId == classId && x.SectionId == sectionId).FirstOrDefault().ClassSectionId;
        }

        public int AddClassSection(ClassSection classSection)
        {
            int result = -1;
            if (classSection != null)
            {
                dbContext.ClassSections.Add(classSection);
                dbContext.SaveChanges();
                result = classSection.ClassSectionId;
            }

            return result;
        }

        public ClassSection GetClassSectionById(int classSectionId)
        {
            return dbContext.ClassSections.Where(x => x.ClassSectionId == classSectionId).FirstOrDefault();
        }

        public List<Section> GetSectionsByClassId(int classId)
        {
            return (List<Section>)dbContext.ClassSections.Where(x => x.ClassId == classId).Select(r => r.Section).ToList();
        }

        public List<ClassSection> GetClassSectionsByClassId(int classId)
        {
            return (List<ClassSection>)dbContext.ClassSections.Where(x => x.ClassId == classId).ToList();
        }

        public ClassSection GetClassSectionByClassAndSectionId(int classId, int sectionId)
        {
            return dbContext.ClassSections.Where(x => x.ClassId == classId && x.SectionId == sectionId).FirstOrDefault();
        }


        public void UpdateClassSection(ClassSection classSection)
        {
            if (classSection != null)
            {
                ClassSection sessionObject = dbContext.ClassSections.Find(classSection.ClassSectionId);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(classSection);
                //dbContext.Entry(classSection).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteClassSection(ClassSection classSection)
        {
            if (classSection != null)
            {
                dbContext.ClassSections.Remove(classSection);
                dbContext.SaveChanges();
            }
        }

        public List<ClassSection> GetAllClassSections()
        {
            return dbContext.ClassSections.ToList();
        }

        public List<ClassSectionModel> GetAllClassSectionsModel()
        {
            var query = from clsec in dbContext.ClassSections
                        join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        select new ClassSectionModel
                        {
                            ClassSectionId = clsec.ClassSectionId,
                            ClassId = (int)clsec.ClassId,
                            SectionId = (int)clsec.SectionId,
                            BranchId = (int)cls.BranchId,
                            ClassName = cls.Name,
                            SectionName = sec.Name,
                            IsFinanceAccountOpen = clsec.IsFinanceAccountOpen
                        };

            return query.ToList();
        }

        public ClassSectionModel GetClassSectionsModelById(int id)
        {
            var query = from clsec in dbContext.ClassSections
                        join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where clsec.ClassSectionId == id
                        select new ClassSectionModel
                        {
                            ClassSectionId = clsec.ClassSectionId,
                            ClassId = (int)clsec.ClassId,
                            SectionId = (int)clsec.SectionId,
                            BranchId = (int)cls.BranchId,
                            ClassName = cls.Name,
                            SectionName = sec.Name,
                            IsFinanceAccountOpen = clsec.IsFinanceAccountOpen
                        };

            return query.FirstOrDefault();
        }

        public Relegion GetReligionById(int id)
        {
            return dbContext.Relegions.Find(id);
        }

        public Gender GetGenderById(int id)
        {
            return dbContext.Genders.Find(id);
        }

        public int GetClassCount(int ClassId)
        {
            return dbContext.ClassSections.Where(x => x.ClassId == ClassId).Count();
        }

        public int GetStudentCount(int ClassSectionId)
        {
            return dbContext.Students.Where(x => x.ClassSectionId == ClassSectionId).Count();
        }

        public int GetSectionCount(int SectionId)
        {
            return dbContext.ClassSections.Where(x => x.SectionId == SectionId).Count();
        }


        #region Subject Chapters
        public SubjectChapter GetChapterById(int chapterId)
        {
            return dbContext.SubjectChapters.Find(chapterId);
        }

        public List<SubjectChapter> GetAllChapters(int branchId)
        {
            return dbContext.SubjectChapters.Where(n => n.RegisterCourse.BranchId == branchId).Include(n => n.TermChapters).Include(n => n.RegisterCourse).ToList();
        }
        public ChapterTopicViewModel GetChapters(int registeredCourseId)
        {
            var response = new ChapterTopicViewModel();
            response.RegisterCourseId = registeredCourseId;
            response.SubjectChapters = dbContext.SubjectChapters.Where(n => n.RegisterCourseId == registeredCourseId).ToList();

            var first = response.SubjectChapters.FirstOrDefault();
            if (first != null)
            {
                var course = dbContext.RegisterCourses.Where(x => x.RegisterCourseId == first.RegisterCourseId).FirstOrDefault();
                var classSectionModel = GetClassSectionsModelById(course.ClassSectionId);
                response.ClassSectionId = classSectionModel.ClassSectionId;
                response.ClassId = classSectionModel.ClassId;
                response.SectionId = classSectionModel.SectionId;
            }

            return response;
        }

        public int AddMoreChapters(int registerCourseId, int increaseBy)
        {
            var existingChaptersCount = dbContext.SubjectChapters.Count(n => n.RegisterCourseId == registerCourseId);
            List<SubjectChapter> subjectChapters = new List<SubjectChapter>();
            increaseBy += existingChaptersCount;
            for (int counter = existingChaptersCount+1; counter <= increaseBy; counter++)
            {
                subjectChapters.Add(new SubjectChapter { Name = "Chapter-" + counter, RegisterCourseId = registerCourseId });
            }

            return CreateOrUpdateChapters(subjectChapters);
        }
        public int CreateOrUpdateChapters(List<SubjectChapter> subjectChapters)
        {
            try
            {
                foreach (var obj in subjectChapters)
                {
                    if (obj.Id > 0)
                    {
                        dbContext.Entry(obj).State = EntityState.Modified;
                        if (obj.IsCovered)
                        {
                            var chapterTopics = dbContext.ChapterTopics.Where(n => n.ChapterId == obj.Id);
                            foreach (var topic in chapterTopics)
                            {
                                topic.IsCovered = true;
                                dbContext.Entry(topic).State = EntityState.Modified;
                            }
                        }
                    }
                    else
                    {
                        obj.ChapterTopics = new List<ChapterTopic> { new ChapterTopic { Name = "Topic-1" } };
                        dbContext.SubjectChapters.Add(obj);
                    }
                }

                dbContext.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return 400;
            }
            
        }

        public void DeleteSubjectChater(SubjectChapter subjectChapter)
        {
            if (subjectChapter != null)
            {
                dbContext.SubjectChapters.Remove(subjectChapter);
                foreach(var topic in subjectChapter.ChapterTopics)
                {
                    dbContext.ChapterTopics.Remove(topic);
                }
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region Chapter Topic
        public ChapterTopic GetTopicById(int topicId)
        {
            return dbContext.ChapterTopics.Find(topicId);
        }
        public ChapterTopicViewModel GetTopics(int chapterId)
        {
            var response = new ChapterTopicViewModel();
            response.ChapterId = chapterId;
            response.ChapterTopics = dbContext.ChapterTopics.Where(n => n.ChapterId == chapterId).ToList();
            var first = response.ChapterTopics.FirstOrDefault();
            if (first != null)
            {
                response.RegisterCourseId = first.SubjectChapter.RegisterCourseId;
                var course = dbContext.RegisterCourses.Where(x => x.RegisterCourseId == response.RegisterCourseId).FirstOrDefault();
                var classSectionModel = GetClassSectionsModelById(course.ClassSectionId);
                response.ClassSectionId = classSectionModel.ClassSectionId;
                response.ClassId = classSectionModel.ClassId;
                response.SectionId = classSectionModel.SectionId;
            }

            return response;
        }

        public List<ChapterTopic> GetAllTopics(int branchId)
        {
            return dbContext.ChapterTopics.Where(n => n.SubjectChapter.RegisterCourse.BranchId == branchId).ToList();
        }

        public int AddMoreTopics(int chapterId, int increaseBy)
        {
            var existingTopicsCount = dbContext.ChapterTopics.Count(n => n.ChapterId == chapterId);
            List<ChapterTopic> topics = new List<ChapterTopic>();
            increaseBy += existingTopicsCount;
            for (int counter = existingTopicsCount + 1; counter <= increaseBy; counter++)
            {
                topics.Add(new ChapterTopic { Name = "Topic-" + counter, ChapterId = chapterId });
            }

            return CreateOrUpdateTopics(topics);
        }
        public int CreateOrUpdateTopics(List<ChapterTopic> topics)
        {
            try
            {
                foreach (var obj in topics)
                {
                    if (obj.Id > 0)
                    {
                        dbContext.Entry(obj).State = EntityState.Modified;
                    }
                    else
                    {
                        dbContext.ChapterTopics.Add(obj);
                    }
                }

                if(topics.Any())
                {
                    var chapterID = topics.FirstOrDefault().ChapterId;
                    var subjectChapter = dbContext.SubjectChapters.FirstOrDefault(n => n.Id == chapterID);
                    if (topics.All(n => n.IsCovered))
                    {
                        subjectChapter.IsCovered = true;
                    }
                    else
                    {
                        subjectChapter.IsCovered = false;
                    }
                    
                    dbContext.Entry(subjectChapter).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return 400;
            }

        }

        public void DeleteTopic(ChapterTopic topic)
        {
            if (topic != null)
            {
                dbContext.ChapterTopics.Remove(topic);
                var subjectChapter = dbContext.SubjectChapters.Find(topic.ChapterId);
                if (subjectChapter.ChapterTopics.All(n => n.IsCovered))
                {
                    subjectChapter.IsCovered = true;
                    dbContext.Entry(subjectChapter).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
            }
        }
        #endregion

        #region Term Chapters

        public List<TermChapter> GetTermChapters(int branchId, int yearId = 0, int termId = 0, int chapterId = 0, int registerCourseId = 0, int classId = 0, int sectionId = 0)
        {
            return dbContext.TermChapters.Where(n => n.ExamTerm.BranchId == branchId &&
            (termId > 0 ? n.TermId == termId : (yearId > 0 ? n.ExamTerm.Year == yearId : true))
            && (chapterId > 0 ? n.ChapterId == chapterId : (
                registerCourseId > 0 ? n.SubjectChapter.RegisterCourseId == registerCourseId : (
                    classId > 0 ? n.SubjectChapter.RegisterCourse.ClassSection.ClassId == classId : (
                        sectionId > 0 ? n.SubjectChapter.RegisterCourse.ClassSection.SectionId == sectionId : true))))
            ).ToList();
        }

        public List<TermChapterModel> GetTermChaptersModel(int branchId, int yearId = 0, int termId = 0, int chapterId = 0, int registerCourseId = 0, int classId = 0, int sectionId = 0)
        {
            var query = from chapter in dbContext.TermChapters
                        join subjChap in dbContext.SubjectChapters on chapter.SubjectChapter.Id equals subjChap.Id
                        join term in dbContext.ExamTerms on chapter.TermId equals term.Id
                        join course in dbContext.RegisterCourses on subjChap.RegisterCourseId equals course.RegisterCourseId
                        join clsec in dbContext.ClassSections on course.ClassSectionId equals clsec.ClassSectionId
                        where term.BranchId == branchId &&
                        (termId > 0 ? chapter.TermId == termId : (yearId > 0 ? term.Year == yearId : true))
                        && (chapterId > 0 ? chapter.ChapterId == chapterId : (
                        registerCourseId > 0 ? course.RegisterCourseId == registerCourseId : (
                        classId > 0 ? clsec.ClassId == classId : (
                        sectionId > 0 ? clsec.SectionId == sectionId : true))))
                        select new TermChapterModel
                        {
                            ChapterId = subjChap.Id,
                            ExamTermYear = (int) term.Year,
                            Id = chapter.Id,
                            RegisterCourseId = course.RegisterCourseId,
                            SubjectChapterName = subjChap.Name,
                            TermId = term.Id,
                            TermName = term.TermName,
                            ClassId = clsec.ClassId,
                            SectionId = clsec.SectionId
                        };
            return query.ToList();
        }

        public int SaveTermChapters(List<TermChapter> objs)
        {
            try
            {
                foreach (var obj in objs)
                {
                    if (obj.Id > 0)
                    {
                        dbContext.Entry(obj).State = EntityState.Modified;
                    }
                    else
                    {
                        dbContext.TermChapters.Add(obj);
                    }
                }
                dbContext.SaveChanges();
                return 1;
            }
            catch(Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return 202;
            }
        }

        public int DeleteTermChapter(int id)
        {
            var obj = dbContext.TermChapters.Find(id);
            if (obj != null)
            {
                dbContext.TermChapters.Remove(obj);
                dbContext.SaveChanges();
                return 200;
            }
            else
            {
                return 404;
            }
        }
        #endregion

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
