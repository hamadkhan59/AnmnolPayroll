
/////////////////////////////////////////
// (C)2010-2011 B.B Company.           //
// All rights reserved.                //
// mailto:Behzad.khosravifar@gmail.com //
// Creator: Behzad Khosravifar         //
/////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using SMS_DAL;
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;

namespace SMS_Web.MakeClassSchedule.Algorithm
{
    /// <summary>
    /// Reads configration file (LINQ.DB) and stores parsed objects
    /// </summary>
    public class Configuration
    {
        // Global instance
        static Configuration _instance = new Configuration();
        // Returns reference to global instance
        public static Configuration GetInstance { get { return _instance; } }

        #region Properties
        public int REQUIRED_DAY_HOURS = 7;
        IStaffRepository staffRepo;
        // Parsed professors
        private Dictionary<int, Professor> _professors = new Dictionary<int, Professor>();

        // Parsed student groups
        private Dictionary<int, StudentsGroup> _studentGroups = new Dictionary<int, StudentsGroup>();

        // Parsed courses
        private Dictionary<int, Course> _courses = new Dictionary<int, Course>();

        // Parsed rooms
        private Dictionary<int, Room> _rooms = new Dictionary<int, Room>();
        public Dictionary<int, Room> Rooms
        {
            get { return _rooms; }
        }

        // Parsed classes
        private List<CourseClass> _courseClasses = new List<CourseClass>();

        // Inidicate that configuration is not prsed yet
        private bool _isEmpty;

        #endregion

        // Initialize data
        public Configuration()
        {
            _isEmpty = true;
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        }

        // Frees used resources
        ~Configuration()
        {
            _professors.Clear();

            _studentGroups.Clear();

            _courses.Clear();

            _rooms.Clear();

            _courseClasses.Clear();
        }

        // Parse file and store parsed object
        public void ParseFile(string SessionID)
        {
            // clear previously parsed objects
            _professors.Clear();
            _studentGroups.Clear();
            _courses.Clear();
            _rooms.Clear();
            _courseClasses.Clear();

            Room.RestartIDs();
            //
            // Save Professor Data
            //
            var teachers = SessionHelper.TeacherList(SessionID, "teacher");//.Where(n => n.StaffId <= 5);
            foreach (var any in teachers)
            {
                ProfessorInfoCompiler pIc = new ProfessorInfoCompiler();
                Professor p;
                if (pIc.StartScanner(""))
                {
                    p = new Professor(any.StaffId, any.Name, pIc.CompiledData);
                    _professors.Add(p.GetId, p);
                }
            }
            //
            // Save StudentsGroup Data
            //
            var sections = SessionHelper.ClassSectionList(SessionID);//.Where(n => (n.SectionId < 4033 || n.SectionId ==4034) && n.ClassId <= 22067);
            foreach (var any in sections)
            {
                StudentsGroup sg;
                string sg_name = string.Format(CultureInfo.CurrentCulture, "{0}", any.ClassName + " " + any.SectionName);
                sg = new StudentsGroup(any.ClassSectionId, sg_name, 1);
                _studentGroups.Add(sg.GetId, sg);
            }
            //
            // Save Course Data
            //
            var subjects = SessionHelper.ClassSubjectList(SessionID);//.Where(n => (n.ClassSection.Section.Id < 4033 || n.ClassSection.Section.Id == 4034) && n.ClassSection.Class.Id <= 22067); ;
            foreach (var any in subjects)
            {
                Course c;
                c = new Course(any.RegisterCourseId, any.ClassName + " " + any.SubjectName);
                _courses.Add(c.GetId, c);
            }
            //
            // Save Room Data
            //
            var requiredRooms = sections.Count();
            for (var i = 1; i <= requiredRooms; i++)
            {
                Room r = new Room(i, "Room_"+i, "Lecture Room", 100);
                _rooms.Add(r.GetId, r);
            }
                //foreach (var any in db.Rooms)
                //{
                //    Room r;
                //    r = new Room(any.Room_ID, any.Name_Room, any.Type_Room, any.Size_No);
                //    _rooms.Add(r.GetId, r);
                //}
            //
            // Save CourseClass Data -----------------------------------------------------------------------------
            //

            var sessionSubjects = staffRepo.GetAllSessionSubjects();//.Where(n => (n.ClassSection.Section.Id < 4033 || n.ClassSection.Section.Id == 4034) && n.ClassSection.Class.Id <= 22067);

            var maxSectionSubjects = sessionSubjects.GroupBy(n => n.ClassSectionId).Select(n => new { section = n.Key, subjects = n.Count() }).OrderByDescending(n => n.subjects).Select(n => n.subjects).FirstOrDefault();
            var maxTeacherSubjects = sessionSubjects.GroupBy(n => n.TeacherId).Select(n => new { teacher = n.Key, subjects = n.Count() }).OrderByDescending(n => n.subjects).Select(n => n.subjects).FirstOrDefault();
            if (maxSectionSubjects > maxTeacherSubjects)
            {
                REQUIRED_DAY_HOURS = maxSectionSubjects;
            }
            else
            {
                REQUIRED_DAY_HOURS = maxTeacherSubjects;
            }

            var classId = 0;
            foreach (var any in sessionSubjects)
            {
                //
                // set Professor by best priority
                //
                //var prof = (from p1 in db.Priority_Professors
                //            join p2 in db.Professors on p1.Professor_ID equals p2.ID
                //            where (p1.Class_ID == any.Class_ID)
                //            orderby p1.Priority
                //            select new
                //            {
                //                p1.Professor_ID,
                //                p2.Name_Professor,
                //                p2.Schedule
                //            }).ToArray()[0];

                ProfessorInfoCompiler pIc = new ProfessorInfoCompiler();
                Professor p = (pIc.StartScanner("")) ?
                    new Professor(any.TeacherId.Value, any.Staff.Name, pIc.CompiledData) :
                    new Professor(any.TeacherId.Value, any.Staff.Name, pIc.CompiledData);
                //
                // set selected course for class
                //
                Course c = new Course(0,null);
                var classSubject = subjects.Where(n => n.SubjectId == any.SubjectId.Value && n.ClassSectionId == any.ClassSectionId).FirstOrDefault();
                if (classSubject != null)
                {
                    c = new Course(classSubject.RegisterCourseId, any.ClassSection.Class.Name + " " + any.Subject.Name);
                }
                //
                // set StudentsGroup in List
                //
                List<StudentsGroup> g = new List<StudentsGroup>();
                //foreach (var lstGroup in (from gil in db.Group_ID_Lists
                //                          join groups in db.Groups on gil.Group_ID equals groups.ID
                //                          where gil.Class_ID == any.Class_ID
                //                          select new
                //                          {
                //                              gil.Group_ID,
                //                              groups.Size_No,
                //                              sg_name = string.Format(CultureInfo.CurrentCulture, "{0}  {1}  {2}-{3}",
                //                                                                                 groups.Branch.Degree,
                //                                                                                 groups.Branch.Branch_Name,
                //                                                                                 groups.Semester_Entry_Year,
                //                                                                                 (groups.Semester_Entry_FS) ? "1" : "2")
                //                          }))
                //{
                //    StudentsGroup sg = new StudentsGroup(lstGroup.Group_ID, lstGroup.sg_name, lstGroup.Size_No);
                //    g.Add(sg);
                //}

                StudentsGroup sg = new StudentsGroup(any.ClassSection.ClassSectionId, any.ClassSection.Class.Name + " " + any.ClassSection.Section.Name, 1);
                g.Add(sg);

                //
                // save class by created data
                //
                classId++;
                CourseClass cc = new CourseClass(p, c, g, "Lecture Room", 1, classId);
                _courseClasses.Add(cc);
            }
            //----------------------------------------------------------------------------------------------------------------
            //
            _isEmpty = false;
        }

        // Returns pointer to professor with specified ID
        // If there is no professor with such ID method returns NULL
        public Professor GetProfessorById(int id)
        {
            if (_professors.ContainsKey(id))
                return _professors[id];
            return null;
        }

        // Returns number of parsed professors
        public int GetNumberOfProfessors() { return (int)_professors.Count; }

        // Returns pointer to student group with specified ID
        // If there is no student group with such ID method returns NULL
        public StudentsGroup GetStudentsGroupById(int id)
        {
            if (_studentGroups.ContainsKey(id))
                return _studentGroups[id];
            return null;
        }

        // Returns number of parsed student groups
        public int GetNumberOfStudentGroups() { return (int)_studentGroups.Count; }

        // Returns pointer to course with specified ID
        // If there is no course with such ID method returns NULL
        public Course GetCourseById(int id)
        {
            if (_courses.ContainsKey(id))
                return _courses[id];
            return null;
        }

        public int GetNumberOfCourses() { return (int)_courses.Count; }

        // Returns pointer to room with specified ID
        // If there is no room with such ID method returns NULL
        public Room GetRoomById(int id)
        {
            if (_rooms.ContainsKey(id))
                return _rooms[id];
            return null;
        }

        // Returns number of parsed rooms
        public int GetNumberOfRooms() { return (int)_rooms.Count; }

        // Returns reference to list of parsed classes
        public List<CourseClass> GetCourseClasses() { return _courseClasses; }

        // Returns number of parsed classes
        public int GetNumberOfCourseClasses() { return (int)_courseClasses.Count; }

        // Returns TRUE if configuration is not parsed yet
        public bool IsEmpty() { return _isEmpty; }
    }
}
