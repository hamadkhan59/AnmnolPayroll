using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IClassSectionRepository : IDisposable 
    {
        int GetClassSectionId(int classId, int sectionId);
        int AddClassSection(ClassSection classSection);
        ClassSection GetClassSectionById(int classSectionId);
        ClassSection GetClassSectionByClassAndSectionId(int classId, int sectionId);
        void UpdateClassSection(ClassSection classSection);
        void DeleteClassSection(ClassSection classSection);
        List<ClassSection> GetAllClassSections();
        List<ClassSectionModel> GetAllClassSectionsModel();
        ClassSectionModel GetClassSectionsModelById(int id);
        List<Section> GetSectionsByClassId(int classId);
        List<ClassSection> GetClassSectionsByClassId(int classId);
        Relegion GetReligionById(int id);
        Gender GetGenderById(int id);
        int GetClassCount(int ClassId);
        int GetSectionCount(int SectionId);
        int GetStudentCount(int ClassSectionId);

        #region Subject Chapters
        SubjectChapter GetChapterById(int chapterId);
        ChapterTopicViewModel GetChapters(int courseId);
        List<SubjectChapter> GetAllChapters(int branchId);
        int AddMoreChapters(int registerCourseId, int increaseBy);
        int CreateOrUpdateChapters(List<SubjectChapter> subjectChapters);
        void DeleteSubjectChater(SubjectChapter subjectChapter);
        #endregion

        #region Chapter Topic
        ChapterTopic GetTopicById(int topicId);
        ChapterTopicViewModel GetTopics(int chapterId);
        List<ChapterTopic> GetAllTopics(int branchId);
        int AddMoreTopics(int chapterId, int increaseBy);
        int CreateOrUpdateTopics(List<ChapterTopic> topics);
        void DeleteTopic(ChapterTopic topic);
        #endregion

        #region Term Chapters
        List<TermChapter> GetTermChapters(int branchId, int yearId = 0, int termId = 0, int chapterId = 0, int registerCourseId = 0, int classId = 0, int sectionId = 0);
        List<TermChapterModel> GetTermChaptersModel(int branchId, int yearId = 0, int termId = 0, int chapterId = 0, int registerCourseId = 0, int classId = 0, int sectionId = 0);
        int SaveTermChapters(List<TermChapter> objs);
        int DeleteTermChapter(int id);
        #endregion
    }
}
