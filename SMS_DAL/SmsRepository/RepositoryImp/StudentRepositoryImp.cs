using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using SMS_DAL.ViewModel;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class StudentRepositoryImp : IStudentRepository
    {
        private SC_WEBEntities2 dbContext1;

        public StudentRepositoryImp(SC_WEBEntities2 context)
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

        #region STUDENT_FUNCTIONS
        public int AddStudent(Student student)
        {
            int result = -1;
            try
            {
                if (student != null)
                {
                    dbContext.Students.Add(student);
                    dbContext.SaveChanges();
                    result = student.id;
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return result;
        }
        public Student GetStudentById(int studentId)
        {
            return dbContext.Students.Find(studentId);
        }

        public Student GetStudentBySrNo(string srNo)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Students.Where(x => x.SrNo == srNo).FirstOrDefault();
        }
		
		public List<StudentModel> GetAllStudentByParentCnic(string fatherCnic)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            var query = from student in dbContext.Students
                        join clasec in dbContext.ClassSections on student.ClassSectionId equals clasec.ClassSectionId
                        join clas in dbContext.Classes on clasec.ClassId equals clas.Id
                        join sec in dbContext.Sections on clasec.SectionId equals sec.Id
                        where student.FatherCNIC == fatherCnic || student.Contact_1 == fatherCnic
                        select new StudentModel { 
                            Id = student.id,
                            LeavingStatusCode = (int)student.LeavingStatus,
                            ClassName = clas.Name,
                            SectionName = sec.Name,
                            RollNumber = student.RollNumber,
                            Name = student.Name,
                            FatherName = student.FatherName,
                            LeavingRemarks = student.LeavingRemarks,
                            BranchId = (int) student.BranchId
                            //AdmissionDate = (DateTime) student.AdmissionDate,
                            //LeavingDate = (DateTime)student.LeavingDate,
                            //StdImage = student.StdImage
                        };

            return query.ToList();
            //return dbContext.Students.Where(x => x.FatherCNIC == fatherCnic).ToList();
        }
		

        public Student GetStudentByAdmissionNo(string admissionNo, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Students.Where(x => x.AdmissionNo == admissionNo && x.BranchId == branchId).FirstOrDefault();
        }
        public int GetStudentId(string rollNo, string studentName, string fatherName)
        {
            return dbContext.Students.Where(x => x.RollNumber == rollNo && x.Name == studentName && x.FatherName == fatherName).FirstOrDefault().id;
        }

        public int GetVanStrength(int driverId)
        {
            return dbContext.Students.Where(x => x.DriverId == driverId).Count();
            //var list = dbContext.Students.Where(x => x.DriverId == driverId).ToList();
            //if (list == null || list.Count == 0)
            //    return 0;
            //else
            //    return list.Count;
        }

        public List<StudentModel> GetStudentByClassSectionId(int classSectionId)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where ( student.ClassSectionId == classSectionId || classSectionId == 0 )
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  BranchId = (int)student.BranchId,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();

            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && x.LeavingStatus == 1).ToList();
        }


        public List<StudentModel> GetStudentByClassSectionIdAndExamYear(int classSectionId, int year)
        {
            var StudentList = from result in dbContext.ExamResults
                              join student in dbContext.Students on result.StudentId equals student.id
                              join exam in dbContext.Exams on result.ExamId equals exam.Id
                              join type in dbContext.ExamTypes on exam.ExamTypeId equals type.Id
                              join term in dbContext.ExamTerms on type.TermId equals term.Id
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where (exam.ClassSectionId == classSectionId) && term.Year == year
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  BranchId = (int)student.BranchId,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.Distinct().ToList();

            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && x.LeavingStatus == 1).ToList();
        }

        public List<StudentModel> GetStudentByClassId(int classId)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where (clas.Id == classId)
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  BranchId = (int)student.BranchId,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.Distinct().OrderBy(x => x.ClassName).OrderBy(x => x.SectionName).ToList();

            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && x.LeavingStatus == 1).ToList();
        }

        public StudentModel GetStudentModelByStudentId(int studentId)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.id == studentId
                              select new StudentModel
                              {
                                  Id = student.id,
                                  BranchId = (int)student.BranchId,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.FirstOrDefault();

            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && x.LeavingStatus == 1).ToList();
        }

        public Student GetStudentByRollNoAndClassSectionId(string rollNO, int classSectionId)
        {
            return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && x.RollNumber == rollNO).FirstOrDefault();
        }

        public List<StudentModel> GetStudentByAdmissionDate(DateTime fromDate, DateTime toDate, int classId, int sectionId, int branchId)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where clas.Id == classId && sec.Id == sectionId &&
                              student.BranchId == branchId &&
                              (student.AdmissionDate >= fromDate && student.AdmissionDate <= toDate)
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
								  AdmissionNo = student.AdmissionNo,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();

            //return dbContext.Students.Where(x => x.AdmissionDate >= fromDate && x.AdmissionDate <= toDate
            //    && (classId == 0 || x.ClassSection.ClassId == classId) && (classId == 0 || x.ClassSection.ClassId == classId)
            //    && x.BranchId == branchId).ToList();
        }

        public SchoolConfig GetSchoolConfigById(int id)
        {
            return dbContext.SchoolConfigs.Find(id);
        }
        public string GetMaxSrNo()
        {
            string srNo = "";
            int count = dbContext.Students.Where(x => x.AdmissionDate.Value.Year == DateTime.Now.Year).Count();
            count++;
            srNo = "SC-" + DateTime.Now.Year.ToString() + "-" + count.ToString().PadLeft(4, '0');
            return srNo;
        }

        public string GetMaxAdmissionNo()
        {
            int admissionNo = 0;
            string sql = "select max(cast (AdmissionNo as int)) from Student";
            admissionNo = dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
            admissionNo++;
            return admissionNo.ToString();
        }

        public void UpdateStudent(Student student)
        {
            if (student != null)
            {
                var sessionObject = dbContext.Students.Find(student.id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(student);

                //dbContext.Entry(student).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void MakeClassSequential(int classSectionId)
        {
            string query = @"EXEC	[dbo].[sp_Class_Sequence]
		                @ClassSectionId = {0}";
            query = string.Format(query, classSectionId);
            dbContext.Database.ExecuteSqlCommand(query);
        }

        public string GetMaxRollNo(int classId, int sectionId)
        {
            int classSectionId = dbContext.ClassSections.Where(x => x.ClassId == classId && x.SectionId == sectionId).FirstOrDefault().ClassSectionId;
            string maxRollNo = "";
            if (classSectionId > 0)
            {
                int studentCount = dbContext.Students.Where(x => x.ClassSectionId == classSectionId).Count();
                if (studentCount > 0)
                {
                    string sql = "select isnull(RollNumber,'0') from Student where id = (  Select Max(cast(id as int)) from Student where ClassSectionId = {0})";
                    sql = string.Format(sql, classSectionId);
                    maxRollNo = dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
                }
                else
                {
                    maxRollNo = "0";
                }
            }

            if (maxRollNo.Contains("-") == true)
            {
                int index = maxRollNo.LastIndexOf("-");
                string prefix = maxRollNo.Substring(0, index + 1);
                string value = maxRollNo.Substring(index + 1, maxRollNo.Length - index - 1);
                var srNO = int.Parse(value) + 1;
                maxRollNo = prefix + srNO.ToString(new string('0', value.Length));
            }
            else
            {
                var prefix = Regex.Match(maxRollNo, "^\\D+").Value;
                var number = Regex.Replace(maxRollNo, "^\\D+", "");

                var srNO = int.Parse(number) + 1;
                maxRollNo = prefix + srNO.ToString(new string('0', number.Length));
            }
            return maxRollNo;
        }
        public void DeleteStudent(Student student)
        {
            if (student != null)
            {
                dbContext.Students.Remove(student);
                dbContext.SaveChanges();
            }
        }

        public List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo, string contactNo)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.ClassSectionId == classSectionId &&
                              student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
								  AdmissionNo = student.AdmissionNo,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();
            //List<Student> studentList = dbContext.Students.Where(x => x.ClassSectionId == classSectionId
            //        && (rollNo == null || x.RollNumber.Contains(rollNo)) && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }

        public List<StudentModel> SearchDischargedStudents(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo, string contactNo)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.ClassSectionId == classSectionId &&
                              student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus != 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();
            //List<Student> studentList = dbContext.Students.Where(x => x.ClassSectionId == classSectionId
            //        && (rollNo == null || x.RollNumber.Contains(rollNo)) && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }

        public List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo, string contactNo)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  AdmissionNo = student.AdmissionNo,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            var stList = StudentList.ToList();
            return stList;

            //List<Student> studentList = dbContext.Students.Where(x => (rollNo == null || x.RollNumber.Contains(rollNo)) 
            //    && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }

        public StudentModel SearchStudentsModel(string rollNo, string studentName, string fatherName, string fatherCnic, string admissionNo, int branchId)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              where student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  AdmissionNo = student.AdmissionNo,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            var stList = StudentList.ToList();
            return stList[0];
        }

        public List<StudentModel> SearchDischargedStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo, string contactNo)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
                              && student.LeavingStatus != 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();

            //List<Student> studentList = dbContext.Students.Where(x => (rollNo == null || x.RollNumber.Contains(rollNo)) 
            //    && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }

        public List<StudentModel> SearchClassStudents(string rollNo, string studentName, string fatherName, int classId, string fatherCnic, int branchId, string admissionNo, string contactNo)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where clas.Id == classId &&
                              student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
								  AdmissionNo = student.AdmissionNo,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.Distinct().ToList();
            //List<Student> studentList = dbContext.Students.Where(x => x.ClassSectionId == classSectionId
            //        && (rollNo == null || x.RollNumber.Contains(rollNo)) && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }

        public List<StudentModel> SearchDischargedClassStudents(string rollNo, string studentName, string fatherName, int classId, string fatherCnic, int branchId, string admissionNo, string contactNo)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where clas.Id == classId &&
                              student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus != 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.Distinct().ToList();
            //List<Student> studentList = dbContext.Students.Where(x => x.ClassSectionId == classSectionId
            //        && (rollNo == null || x.RollNumber.Contains(rollNo)) && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }

        public List<string> GetFatherNameList()
        {
            var list = dbContext.Students.ToList().Select(x => x.FatherName + ", " + x.Name);
            return list.ToList();
        }

        public List<string> GetStudentNameList()
        {
            //var list = dbContext.Students.ToList().Select(x => "{" + "\"Name\"" + ":" + "\"" + x.Name + "\""  + ", "
            //                           + "\"Father\"" + ":" + "\"" + x.FatherName + "\"" + "}");

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                             
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  FatherCnic = student.FatherCNIC,
                                  AdmissionNo = student.AdmissionNo,
                                  CurrentAddress = student.CurrentAddress,
                                  Contact_1 = student.Contact_1,
                                  BranchId = (int) student.BranchId,
                                  SrNo = student.SrNo

                              };

            var list = StudentList.ToList().Select(x => x.AdmissionNo + "," + x.Name + ","
                                       + x.FatherName + "," + x.FatherCnic + "," + x.Contact_1 + "," + x.CurrentAddress.Replace(",", ".")
                                       + "," + x.ClassName + ","
                                       + x.SectionName + ","
                                       + x.RollNumber + "," + x.SrNo + "," + x.BranchId);

            //var list = dbContext.Students.Where(x => x.LeavingStatus == 1).ToList().Select(x => x.AdmissionNo + "," + x.Name + ","
            //                           + x.FatherName + "," + x.FatherCNIC + "," + x.Contact_1 + "," + x.CurrentAddress
            //                           + "," + ((x.ClassSection == null || x.ClassSection.Class == null) ? "" : x.ClassSection.Class.Name) + ","
            //                           + ((x.ClassSection == null || x.ClassSection.Section == null) ? "" : x.ClassSection.Section.Name) + ","
            //                           + x.RollNumber + "," + x.SrNo + "," + x.BranchId);

            return list.ToList();
        }

		public List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.ClassSectionId == classSectionId &&
                              student.BranchId == branchId &&
                              ( rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              //&& (admissionNo == null  || student.AdmissionNo.Contains(admissionNo))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();
            //List<Student> studentList = dbContext.Students.Where(x => x.ClassSectionId == classSectionId
            //        && (rollNo == null || x.RollNumber.Contains(rollNo)) && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
            //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
            //        && x.BranchId == branchId).ToList();
            //return studentList;
        }
		
        //public List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo, string contactNo)
        //{
        //    var StudentList = from student in dbContext.Students
        //                      join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
        //                      join clas in dbContext.Classes on clSec.ClassId equals clas.Id
        //                      join sec in dbContext.Sections on clSec.SectionId equals sec.Id
        //                      //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
        //                      //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
        //                      where student.BranchId == branchId &&
        //                      (rollNo == null || student.RollNumber.Contains(rollNo)) &&
        //                      (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
        //                      (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
        //                      (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
        //                      && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
        //                      && (contactNo == null || student.Contact_1 == null || student.Contact_1.Contains(contactNo))
        //                      select new StudentModel
        //                      {
        //                          Id = student.id,
        //                          ClassName = clas.Name,
        //                          SectionName = sec.Name,
        //                          RollNumber = student.RollNumber,
        //                          Name = student.Name,
        //                          FatherName = student.FatherName,
        //                          StdImage = student.StdImage,
        //                          Contact_1 = student.Contact_1,
        //                          LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
        //                          LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
        //                          AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
        //                          LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
        //                          LeavingRemarks = student.LeavingRemarks,
        //                          ClearDues = student.ClearDues,
        //                          IsPromoted = (bool)student.IsPromoted,
        //                          AdmissionNo = student.AdmissionNo,
        //                          LeavingStatusCode = (int)student.LeavingStatus
        //                      };

        //    return StudentList.ToList();

        //    //List<Student> studentList = dbContext.Students.Where(x => (rollNo == null || x.RollNumber.Contains(rollNo)) 
        //    //    && (fatherCnic == null || x.FatherCNIC.Contains(fatherCnic))
        //    //        && (studentName == null || x.Name.Contains(studentName)) && (fatherName == null || x.FatherName.Contains(fatherName))
        //    //        && x.BranchId == branchId).ToList();
        //    //return studentList;
        //}
        #endregion



        #region STUDENT_INQUIRY_FUNCTIONS

        public int AddStudentInquiry(StudentInquiry studentInquiry)
        {
            int result = -1;
            if (studentInquiry != null)
            {
                dbContext.StudentInquiries.Add(studentInquiry);
                dbContext.SaveChanges();
                result = studentInquiry.ID;
            }

            return result;
        }

        public string GetInquiryNumber(int branchId)
        {
            string inquiryNumber = DateTime.Now.Year.ToString();
            dbContext.Configuration.LazyLoadingEnabled = false;
            int inquiryId = dbContext.StudentInquiries.Where(x => x.BranchId == branchId).Count() + 1;
            return inquiryNumber + inquiryId.ToString().PadLeft(6, '0');

        }
        public void UpdateStudentInquiry(StudentInquiry studentInquiry)
        {
            if (studentInquiry != null)
            {
                var sessionObject = dbContext.StudentInquiries.Find(studentInquiry.ID);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(studentInquiry);
                dbContext.SaveChanges();
            }
        }

        public void DeleteStudentInquiry(StudentInquiry studentInquiry)
        {
            if (studentInquiry != null)
            {
                dbContext.StudentInquiries.Remove(studentInquiry);
                dbContext.SaveChanges();
            }
        }

        public StudentInquiry GetStudentInquiryById(int studentInquiryId)
        {
            return dbContext.StudentInquiries.Find(studentInquiryId);
        }

        public List<StudentInquiryModel> SearchInquiryByInquiryNo(string inquiryNumber, int branchId)
        {
            var query = from inqiuiry in dbContext.StudentInquiries
                        join clas in dbContext.Classes on inqiuiry.ClassId equals clas.Id
                        where (inqiuiry.InquiryNumber == inquiryNumber
                        && inqiuiry.BranchId == branchId)
                        select new StudentInquiryModel
                        {
                            Id = inqiuiry.ID,
                            InquiryNo = inqiuiry.InquiryNumber,
                            Name = inqiuiry.Name,
                            FatherCnic = inqiuiry.FatherCNIC,
                            FatherContact = inqiuiry.Contact_1,
                            FatherName = inqiuiry.FatherName,
                            InquiryDate = (DateTime)inqiuiry.AdmissionDate,
                            StdImage = inqiuiry.StdImage,
                            ClassName = clas.Name
                        };

            return query.ToList();
        }

        public List<StudentInquiry> SearchStudentInquiryByInquiryNo(string inquiryNumber, int branchId)
        {
            return dbContext.StudentInquiries.Where(x => x.InquiryNumber == inquiryNumber && x.BranchId == branchId).ToList();
        }

        public List<StudentInquiryModel> SearchStudentInquiry(int classId, string name, string fatherName, string fatherCnic, DateTime fromDate, DateTime toDate, int branchId)
        {
            var query = from inqiuiry in dbContext.StudentInquiries
                        join clas in dbContext.Classes on inqiuiry.ClassId equals clas.Id
                        where (classId == 0 || inqiuiry.ClassId == classId)
                        && (inqiuiry.Name == null || inqiuiry.Name.Contains(name))
                        && (inqiuiry.FatherName == null || inqiuiry.FatherName.Contains(fatherName))
                        && (inqiuiry.FatherCNIC == null || inqiuiry.FatherCNIC.Contains(fatherCnic))
                        && (EntityFunctions.TruncateTime(inqiuiry.AdmissionDate) >= fromDate.Date
                        && EntityFunctions.TruncateTime(inqiuiry.AdmissionDate) <= toDate.Date
                        && inqiuiry.BranchId == branchId)
                        select new StudentInquiryModel
                        {
                            Id = inqiuiry.ID,
                            InquiryNo = inqiuiry.InquiryNumber,
                            Name = inqiuiry.Name,
                            FatherCnic = inqiuiry.FatherCNIC,
                            FatherContact = inqiuiry.Contact_1,
                            FatherName = inqiuiry.FatherName,
                            InquiryDate = (DateTime) inqiuiry.AdmissionDate,
                            StdImage = inqiuiry.StdImage,
                            ClassName = clas.Name
                        };

            return query.ToList();
        }

        //public List<StudentInquiry> SearchStudentInquiry(int classId, string name, string fatherName, string fatherCnic, DateTime fromDate, DateTime toDate, int branchId)
        //{
        //    var query = dbContext.StudentInquiries.Where(x => (classId == 0 || x.ClassId == classId) && (x.Name == null || x.Name.Contains(name)) &&
        //        (x.FatherName == null || x.FatherName.Contains(fatherName)) && (x.FatherCNIC == null || x.FatherCNIC.Contains(fatherCnic))
        //            && (EntityFunctions.TruncateTime(x.AdmissionDate) >= fromDate.Date && EntityFunctions.TruncateTime(x.AdmissionDate) <= toDate.Date
        //            && x.BranchId == branchId));
        //    return query.ToList();
        //}

        #endregion



        #region NON_UI_FUNCTION

        public void DeleteStudentDocs(int studentId)
        {
            List<StudentDocument> docsList = dbContext.StudentDocuments.Where(x => x.StudentId == studentId).ToList();

            foreach (StudentDocument doc in docsList)
            {
                dbContext.StudentDocuments.Remove(doc);
                dbContext.SaveChanges();
            }
        }

        public byte[] GetStudentImageDoc(int studentId, int imageId)
        {
            byte[] byteArray = null;
            if (dbContext.StudentDocuments.Where(x => x.StudentId == studentId && x.DocumentType == imageId).FirstOrDefault() != null)
            {
                byteArray = dbContext.StudentDocuments.Where(x => x.StudentId == studentId && x.DocumentType == imageId).FirstOrDefault().DocumentImage;
            }
            return byteArray;

        }
        public void AddStudentDocs(StudentDocument studentDoc)
        {
            dbContext.StudentDocuments.Add(studentDoc);
            dbContext.SaveChanges();
        }

        public List<Relegion> GetAllReligion()
        {
            return dbContext.Relegions.ToList();
        }
        public List<Gender> GetAllGenders()
        {
            return dbContext.Genders.ToList();
        }
        public List<StudentAttendanceStatu> GetAllAttendanceStatus()
        {
            return dbContext.StudentAttendanceStatus.ToList();
        }
        public List<Session> GetAllSessions()
        {
            return dbContext.Sessions.ToList();
        }

        public List<TestStatu> GetAllTestStatus()
        {
            return dbContext.TestStatus.ToList();
        }

        public List<AdmissionType> GetAllAdmissionTypes()
        {
            return dbContext.AdmissionTypes.ToList();
        }
        #endregion

        public List<Student> GetStudentByParentCnic(string contactNo)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Students.Where(x => x.Contact_1 == contactNo).ToList();
        }

        public List<StudentModel> SearchStudentsForSms(string rollNo, string studentName, string fatherName, int classSectionId, string fatherCnic, int branchId, string admissionNo)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              where student.ClassSectionId == classSectionId &&
                              student.BranchId == branchId &&
                              (student.Contact_1 != null && student.Contact_1 != "") &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                                  //&& (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (admissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();
        }

        public List<StudentModel> SearchStudentsForSms(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              where student.BranchId == branchId &&
                              (student.Contact_1 != null && student.Contact_1 != "") &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                                  //&& (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && (admissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  Contact_1 = student.Contact_1,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  IsPromoted = (bool)student.IsPromoted,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();
        }

        public List<StudentModel> SearchStudents(string rollNo, string studentName, string fatherName, string fatherCnic, int branchId, string admissionNo)
        {
            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              //join leaSt in dbContext.LeavingStatus on student.LeavingStatus equals leaSt.Id
                              //join leaRes in dbContext.LeavingReasons on student.ReasonId equals leaRes.ReasonId
                              where student.BranchId == branchId &&
                              (rollNo == null || student.RollNumber.Contains(rollNo)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (studentName == null || student.Name == null || student.Name.Contains(studentName)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (admissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(admissionNo))
                              //&& (admissionNo == null  || student.AdmissionNo.Contains(admissionNo))
                              && student.LeavingStatus == 1
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  AdmissionNo = student.AdmissionNo,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();
        }


        public StudentAdmissionModel GetStudentAdmissionData(DateTime fromDate, DateTime toDate, int branchId)
        {
            StudentAdmissionModel model = new StudentAdmissionModel();
            int admittedCount = 0, leftCount = 0, inquiryCount = 0;
            try
            {
                admittedCount = dbContext.Students.Where(x => x.LeavingStatus == 1 && x.BranchId == branchId
                            && EntityFunctions.TruncateTime(x.AdmissionDate) >= fromDate.Date
                            && EntityFunctions.TruncateTime(x.AdmissionDate) <= toDate.Date).Count();

                leftCount = dbContext.Students.Where(x => x.LeavingStatus != 1 && x.BranchId == branchId 
                            && EntityFunctions.TruncateTime(x.LeavingDate) >= fromDate.Date
                            && EntityFunctions.TruncateTime(x.LeavingDate) <= toDate.Date).Count();

                inquiryCount = dbContext.StudentInquiries.Where(x => x.BranchId == branchId
                            && EntityFunctions.TruncateTime(x.AdmissionDate) >= fromDate.Date
                            && EntityFunctions.TruncateTime(x.AdmissionDate) <= toDate.Date).Count();

                model.Admitted = admittedCount;
                model.Left = leftCount;
                model.Inquiries = inquiryCount;
            }
            catch
            {

            }
            return model;
        }

        public ClassStudentAdmissionViewModel GetClassAdmissionStats(int branchId, DateTime fromDate, DateTime toDate)
        {
            var classes = dbContext.Classes.Where(x => x.IsActive == true && x.BranchId == branchId).ToList();
            ClassStudentAdmissionViewModel response = new ClassStudentAdmissionViewModel();
            foreach (var clas in classes)
            {
                var admittedQuery = from std in dbContext.Students
                                    join clsec in dbContext.ClassSections on std.ClassSectionId equals clsec.ClassSectionId
                                    join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                                    where std.BranchId == branchId && std.LeavingStatus == 1 && cls.Id == clas.Id
                                    && EntityFunctions.TruncateTime(std.AdmissionDate) >= fromDate.Date
                                    && EntityFunctions.TruncateTime(std.AdmissionDate) <= toDate.Date
                                    select std;

                var leftQuery = from std in dbContext.Students
                                    join clsec in dbContext.ClassSections on std.ClassSectionId equals clsec.ClassSectionId
                                    join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                                    where std.BranchId == branchId && std.LeavingStatus != 1 && cls.Id == clas.Id
                                    && EntityFunctions.TruncateTime(std.LeavingDate) >= fromDate.Date
                                    && EntityFunctions.TruncateTime(std.LeavingDate) <= toDate.Date
                                    select std;

                var inquiryQuery = from std in dbContext.StudentInquiries
                               join cls in dbContext.Classes on std.ClassId equals cls.Id
                               where std.BranchId == branchId && std.ClassId == clas.Id
                               && EntityFunctions.TruncateTime(std.AdmissionDate) >= fromDate.Date
                               && EntityFunctions.TruncateTime(std.AdmissionDate) <= toDate.Date
                               select std;
                int admissionCount = 0, leftCount = 0, inquiryCount = 0;

                StudentAdmissionModel model = new StudentAdmissionModel();
                admissionCount = admittedQuery.Count();
                leftCount = leftQuery.Count();
                inquiryCount = inquiryQuery.Count();

                if (admissionCount == 0 && leftCount == 0 && inquiryCount == 0)
                { }
                else
                {
                    model.Admitted = admissionCount;
                    model.Left = leftCount;
                    model.Inquiries = inquiryCount;
                    response.Class.Add(clas.Name);
                    response.StudentAdmissionDetail.Add(model);
                }
            }

            return response;
        }

        public StudentAdmissionViewModel GetAdmissionLineStats(int branchId, DateTime fromDate, DateTime toDate, string view = "month")
        {
            var response = new StudentAdmissionViewModel();
            var entries = dbContext.Students.Where(n => n.BranchId == branchId
                && EntityFunctions.TruncateTime(n.AdmissionDate) >= fromDate.Date
                && EntityFunctions.TruncateTime(n.AdmissionDate) <= toDate.Date).ToList();

            var admitted = dbContext.Students.Where(x => x.LeavingStatus == 1 && x.BranchId == branchId
                            && EntityFunctions.TruncateTime(x.AdmissionDate) >= fromDate.Date
                            && EntityFunctions.TruncateTime(x.AdmissionDate) <= toDate.Date).ToList();

            var left = dbContext.Students.Where(x => x.LeavingStatus != 1 && x.BranchId == branchId
                        && EntityFunctions.TruncateTime(x.LeavingDate) >= fromDate.Date
                        && EntityFunctions.TruncateTime(x.LeavingDate) <= toDate.Date).ToList();

            var inquiry = dbContext.StudentInquiries.Where(x => x.BranchId == branchId
                        && EntityFunctions.TruncateTime(x.AdmissionDate) >= fromDate.Date
                        && EntityFunctions.TruncateTime(x.AdmissionDate) <= toDate.Date).ToList();

            

            if (view == "month")
            {
                var yearMonths = entries.Select(n => n.AdmissionDate.Value.Year + n.AdmissionDate.Value.Month).Distinct().ToList();
                foreach (var yearMonth in yearMonths)
                {
                    var monthEntries = entries.Where(n => (n.AdmissionDate.Value.Year + n.AdmissionDate.Value.Month) == yearMonth).ToList();
                    response.Months.Add(monthEntries[0].AdmissionDate.Value.ToString("MMM") + "-" + monthEntries[0].AdmissionDate.Value.ToString("yyyy"));

                    var monthAdmitted = admitted.Where(n => (n.AdmissionDate.Value.Year + n.AdmissionDate.Value.Month) == yearMonth);
                    var monthLeft = left.Where(n => (n.LeavingDate.Value.Year + n.LeavingDate.Value.Month) == yearMonth);
                    var monthInquiry = inquiry.Where(n => (n.AdmissionDate.Value.Year + n.AdmissionDate.Value.Month) == yearMonth);

                    int admittedCount = 0, leftCount = 0, inquiryCount = 0;

                    if (monthAdmitted != null && monthAdmitted.Count() > 0)
                    {
                        admittedCount = monthAdmitted.Count();
                    }

                    if (monthLeft != null && monthLeft.Count() > 0)
                    {
                        leftCount = monthLeft.Count();
                    }

                    if (monthInquiry != null && monthInquiry.Count() > 0)
                    {
                        inquiryCount = monthInquiry.Count();
                    }

                    response.Left.Add(leftCount);
                    response.Admitted.Add(admittedCount);
                    response.Inquiries.Add(inquiryCount);
                }
            }
            else
            {
                var dates = entries.Select(n => n.AdmissionDate.Value.Date).Distinct().ToList();
                foreach (var date in dates)
                {
                    response.Days.Add(date);
                    var monthAdmitted = admitted.Where(n => n.AdmissionDate.Value.Date == date);
                    var monthLeft = left.Where(n => n.LeavingDate.Value.Date == date);
                    var monthInquiry = inquiry.Where(n => n.AdmissionDate.Value.Date == date);

                    int admittedCount = 0, leftCount = 0, inquiryCount = 0;

                    if (monthAdmitted != null && monthAdmitted.Count() > 0)
                    {
                        admittedCount = monthAdmitted.Count();
                    }

                    if (monthLeft != null && monthLeft.Count() > 0)
                    {
                        leftCount = monthLeft.Count();
                    }

                    if (monthInquiry != null && monthInquiry.Count() > 0)
                    {
                        inquiryCount = monthInquiry.Count();
                    }

                    response.Left.Add(leftCount);
                    response.Admitted.Add(admittedCount);
                    response.Inquiries.Add(inquiryCount);
                }
            }

            return response;
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
