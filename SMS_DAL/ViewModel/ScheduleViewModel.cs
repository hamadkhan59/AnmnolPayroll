using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ScheduleViewModel
    {
        public List<LectureSlot> LectureSlots = new List<LectureSlot>();
        public List<ClassSectionLectures> ClassSectionLectures = new List<ClassSectionLectures>();
    }

    public class LectureSlot
    {
        public int SlotId { get; set; }
        public List<int> ClassSectionIds { get; set; }
        public List<int> TeacherIds { get; set; }
        public List<int> TimeTableIds { get; set; }
    }
    public class ClassSectionLectures
    {
        public int ClassSectionId { get; set; }
        public string ClassSectionName { get; set; }
        public List<Lecture> Lectures = new List<Lecture>();
    }
    public class Lecture
    {
        public int TimeTableId { get; set; }
        public int SlotId { get; set; }
        public int RoomId { get; set; }
        public int ClassSectionId { get; set; }
        public string ClassSectionName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        //public Lecture(int TimeTableId, int SlotId, string ClassSectionName, string TeacherName, string SubjectName)
        //{
        //    this.TimeTableId = TimeTableId;
        //    this.SlotId = SlotId;
        //    this.ClassSectionName = ClassSectionName;
        //    this.TeacherName = TeacherName;
        //    this.SubjectName = SubjectName;
        //}
    }
}
