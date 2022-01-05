using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ChapterTopicViewModel
    {
        public int ClassSectionId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int RegisterCourseId { get; set; }
        public int ChapterId { get; set; }

        public List<SubjectChapter> SubjectChapters { get; set; }
        public List<ChapterTopic> ChapterTopics { get; set; }
    }
}
