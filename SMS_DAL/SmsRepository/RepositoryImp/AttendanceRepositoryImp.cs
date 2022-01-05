using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SMS_DAL.ViewModel;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class AttendanceRepositoryImp : IAttendanceRepository
    {
        private SC_WEBEntities2 dbContext1;

        IStudentRepository studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        public AttendanceRepositoryImp(SC_WEBEntities2 context)   
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

        public int AddAttendance(Attandance attendance)
        {
            int result = -1;
            if (attendance != null)
            {
                dbContext.Attandances.Add(attendance);
                dbContext.SaveChanges();
                result = (int)attendance.id;
            }

            return result;
        }

        public List<AttendanceStats> GetAttendanceStats(int branchId, DateTime date)
        {
            var stats = new List<AttendanceStats>();

            var attendanceList = from att in dbContext.Attandances
                                 join student in dbContext.Students on att.StudentID equals student.id
                                 join clsec in dbContext.ClassSections on student.ClassSectionId equals clsec.ClassSectionId
                                 join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                                 join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                                 join status in dbContext.StudentAttendanceStatus on att.StatusId equals status.Id
                                 where EntityFunctions.TruncateTime(att.AttandanceDate) >= date.Date
                                && student.BranchId == branchId
                                 select new AttendanceModel
                                 {
                                     Id = (int)att.id,
                                     ClassId = (int)clsec.ClassId,
                                     SectionId = (int)clsec.SectionId,
                                     ClassName = cls.Name,
                                     SectionName = sec.Name,
                                     StudentId = student.id,
                                     Name = student.Name,
                                     FatherName = student.FatherName,
                                     RollNumber = student.RollNumber,
                                     Status = status.CodeName,
                                     Contact_1 = student.Contact_1,
                                     StatusId = status.Id,
                                     AttendanceDate = (DateTime)att.AttandanceDate
                                 };

            //var attendance = dbContext.Attandances.Where(n => n.Student.BranchId == branchId && EntityFunctions.TruncateTime(n.AttandanceDate) == date.Date).ToList();
            var statuses = dbContext.StudentAttendanceStatus.ToList();
            foreach (var st in statuses)
            {
                var stat = new AttendanceStats();
                stat.StatusId = st.Id;
                stat.StatusCode = st.CodeName;
                stat.Count = attendanceList.Where(n => n.StatusId == st.Id).Count();

                stats.Add(stat);
            }

            return stats;
        }
        public List<AttendanceStats> GetAttendanceStatsByMonth(int branchId, DateTime fromDate, DateTime toDate, int? statusId = null, int? classId = null)
        {
            var stats = new List<AttendanceStats>();
            //var attendance = dbContext.Attandances.Where(n => n.Student.BranchId == branchId
            //    && EntityFunctions.TruncateTime(n.AttandanceDate) >= from.Date
            //    && EntityFunctions.TruncateTime(n.AttandanceDate) <= to.Date
            //    && (statusId.HasValue ? statusId.Value == n.StatusId : true)
            //    && (classId.HasValue ? classId.Value == n.Student.ClassSection.ClassId : true)).ToList();

            var attendanceList = from att in dbContext.Attandances
                                 join student in dbContext.Students on att.StudentID equals student.id
                                 join clsec in dbContext.ClassSections on student.ClassSectionId equals clsec.ClassSectionId
                                 join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                                 join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                                 join status in dbContext.StudentAttendanceStatus on att.StatusId equals status.Id
                                 where (EntityFunctions.TruncateTime(att.AttandanceDate) >= fromDate.Date
                                 && EntityFunctions.TruncateTime(att.AttandanceDate) <= toDate.Date)
                                && (statusId.HasValue ? statusId.Value == att.StatusId : true)
                                && student.BranchId == branchId
                                && (classId.HasValue ? classId.Value == clsec.ClassId : true)
                                 select new AttendanceModel
                                 {
                                     Id = (int)att.id,
                                     ClassId = (int)clsec.ClassId,
                                     SectionId = (int)clsec.SectionId,
                                     ClassName = cls.Name,
                                     SectionName = sec.Name,
                                     StudentId = student.id,
                                     Name = student.Name,
                                     FatherName = student.FatherName,
                                     RollNumber = student.RollNumber,
                                     Status = status.CodeName,
                                     Contact_1 = student.Contact_1,
                                     StatusId = status.Id,
                                     AttendanceDate = (DateTime)att.AttandanceDate
                                 };
            var attendance = attendanceList.OrderByDescending(x => x.AttendanceDate).ToList();

            if (statusId.HasValue)
            {
                stats = attendance.GroupBy(n => classId.HasValue ? n.SectionId : n.ClassId)
                    .Select(n => new
                    {
                        Key = n.Key,
                        Response = new AttendanceStats()
                        {
                            Count = n.Count(),
                            StatusId = n.FirstOrDefault().StatusId,
                            StatusCode = n.FirstOrDefault().Status,
                            ClassId = n.FirstOrDefault().ClassId,
                            ClassName = n.FirstOrDefault().ClassName,
                            SectionId = n.FirstOrDefault().SectionId,
                            SectionName = n.FirstOrDefault().SectionName,
                        }
                    }
                ).Select(n => n.Response).ToList();
            }
            else
            {
                var status = dbContext.StudentAttendanceStatus.ToList();
                foreach (var st in status)
                {
                    var stat = new AttendanceStats();
                    stat.StatusId = st.Id;
                    stat.StatusCode = st.CodeName;
                    stat.Count = attendance.Where(n => n.StatusId == st.Id).Count();

                    stats.Add(stat);
                }
            }

            return stats;
        }
        public DashboardDataViewModels GetAttendanceStatsByDate(int branchId, DateTime fromDate, DateTime toDate, string view = "day")
        {
            var response = new DashboardDataViewModels();
            response.StudentAttendanceStats = new List<AttendanceStats>();

            var attendance = from att in dbContext.Attandances
                                 join student in dbContext.Students on att.StudentID equals student.id
                                 join clsec in dbContext.ClassSections on student.ClassSectionId equals clsec.ClassSectionId
                                 join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                                 join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                                 join status1 in dbContext.StudentAttendanceStatus on att.StatusId equals status1.Id
                                 where (EntityFunctions.TruncateTime(att.AttandanceDate) >= fromDate.Date
                                 && EntityFunctions.TruncateTime(att.AttandanceDate) <= toDate.Date)
                                && student.BranchId == branchId
                                 select new AttendanceModel
                                 {
                                     Id = (int)att.id,
                                     ClassId = (int)clsec.ClassId,
                                     SectionId = (int)clsec.SectionId,
                                     ClassName = cls.Name,
                                     SectionName = sec.Name,
                                     StudentId = student.id,
                                     Name = student.Name,
                                     FatherName = student.FatherName,
                                     RollNumber = student.RollNumber,
                                     Status = status1.CodeName,
                                     Contact_1 = student.Contact_1,
                                     StatusId = status1.Id,
                                     AttendanceDate = (DateTime)att.AttandanceDate
                                 };

            //var attendance = dbContext.Attandances.Where(n => n.Student.BranchId == branchId
            //    && EntityFunctions.TruncateTime(n.AttandanceDate) >= from.Date
            //    && EntityFunctions.TruncateTime(n.AttandanceDate) <= to.Date).ToList();
            var status = dbContext.StudentAttendanceStatus.ToList();

            if (view == "month")
            {
                var yearMonths = attendance.Select(n => n.AttendanceDate.Year + n.AttendanceDate.Month).Distinct();
                response.Months = new List<string>();
                foreach (var st in status)
                {
                    var stat = new AttendanceStats();
                    stat.StatusId = st.Id;
                    stat.StatusCode = st.CodeName;
                    stat.Data = new List<int>();
                    
                    var attendanceByStatus = attendance.Where(n => n.StatusId == st.Id);
                    foreach (var yearMonth in yearMonths)
                    {
                        var monthEntries = attendance.Where(n => (n.AttendanceDate.Year + n.AttendanceDate.Month) == yearMonth);
                        string month = monthEntries.First().AttendanceDate.ToString("MMM") + "-" + monthEntries.First().AttendanceDate.ToString("yyyy");
                        if(response.Months.Contains(month) == false)
                            response.Months.Add(month);

                        stat.Data.Add(attendanceByStatus.Where(n => (n.AttendanceDate.Year + n.AttendanceDate.Month) == yearMonth).Count());
                    }
                    response.StudentAttendanceStats.Add(stat);
                }

            }
            else
            {
                response.Dates = attendance.OrderBy(n => n.AttendanceDate).Select(n => (DateTime)EntityFunctions.TruncateTime(n.AttendanceDate)).Distinct().ToList();
                foreach (var st in status)
                {
                    var stat = new AttendanceStats();
                    stat.StatusId = st.Id;
                    stat.StatusCode = st.CodeName;
                    stat.Data = new List<int>();

                    var attendanceByStatus = attendance.Where(n => n.StatusId == st.Id);
                    foreach (var date in response.Dates)
                    {
                        stat.Data.Add(attendanceByStatus.Where(n => (DateTime)EntityFunctions.TruncateTime(n.AttendanceDate) == date.Date).Count());
                    }

                    response.StudentAttendanceStats.Add(stat);
                }
            }
            return response;
        }

        public List<AttendanceModel> GetAttendanceByDate(int clasSectionId, DateTime markDate)
        {
            var attendanceList = from att in dbContext.Attandances
                                 join student in dbContext.Students on att.StudentID equals student.id
                                 join status in dbContext.StudentAttendanceStatus on att.StatusId equals status.Id
                                 where ( student.ClassSectionId == clasSectionId || clasSectionId == 0)
                                 && (EntityFunctions.TruncateTime(att.AttandanceDate) == markDate.Date)
                                 && student.LeavingStatus == 1
                                 select new AttendanceModel
                                 {
                                     Id = (int)att.id,
                                     StudentId = student.id,
                                     Name = student.Name,
                                     FatherName = student.FatherName,
                                     RollNumber = student.RollNumber,
                                     Status = status.CodeName,
                                     StatusId = status.Id,
                                     Contact_1 = student.Contact_1,
                                     AdmissionNo = (student.AdmissionNo == null ? "" : student.AdmissionNo),
                                     AttendanceDate = (DateTime)att.AttandanceDate
                                 };
            return attendanceList.ToList();

            //return dbContext.Attandances.Where(x => EntityFunctions.TruncateTime(x.AttandanceDate) == markDate.Date 
            //    && x.Student.ClassSectionId == clasSectionId).Include(x => x.Student).Include(x => x.StudentAttendanceStatu)
            //    .OrderBy(x => x.Student.AdmissionDate).ToList();
        }

        public List<AttendanceModel> GetAttendanceSheetByDate(int classSectionId, DateTime fromDate, DateTime toDate, int statusId = 0)
        {
            var attendanceList = from att in dbContext.Attandances
                                 join student in dbContext.Students on att.StudentID equals student.id
                                 join status in dbContext.StudentAttendanceStatus on att.StatusId equals status.Id
                                 where (classSectionId > 0 ? student.ClassSectionId == classSectionId : true)
                                 && (EntityFunctions.TruncateTime(att.AttandanceDate) >= fromDate.Date
                                 && EntityFunctions.TruncateTime(att.AttandanceDate) <= toDate.Date)
                                && (statusId > 0 ? att.StatusId == statusId : true)
                                && student.LeavingStatus == 1
                                 select new AttendanceModel
                                 {
                                     Id = (int)att.id,
                                     StudentId = student.id,
                                     Name = student.Name,
                                     FatherName = student.FatherName,
                                     RollNumber = student.RollNumber,
                                     Status = status.CodeName,
                                     Contact_1 = student.Contact_1,
                                     StatusId = status.Id,
                                     AttendanceDate = (DateTime)att.AttandanceDate
                                 };
            return attendanceList.OrderByDescending(x => x.AttendanceDate).ToList();
            //return dbContext.Attandances.Where(x => EntityFunctions.TruncateTime(x.AttandanceDate) >= fromDate.Date
            //    && EntityFunctions.TruncateTime(x.AttandanceDate) <= toDate.Date && x.Student.ClassSectionId == classSectionId)
            //    .Include(x => new Student { id = x.Student.id, RollNumber = x.Student.RollNumber, Name = x.Student.Name, FatherName = x.Student.FatherName }).Include(x => x.StudentAttendanceStatu).OrderBy(x => x.AttandanceDate).ToList();
        }

        public int GetStudentAbsents(int studentId, DateTime fromDate, DateTime toDate)
        {
            var attendanceList = from att in dbContext.Attandances
                                 where att.StudentID == studentId
                                 && (EntityFunctions.TruncateTime(att.AttandanceDate) >= fromDate.Date
                                        && EntityFunctions.TruncateTime(att.AttandanceDate) <= toDate.Date)
                                        && att.StatusId == 2
                                 select att;
            return attendanceList.Count();
            //return dbContext.Attandances.Where(x => EntityFunctions.TruncateTime(x.AttandanceDate) >= fromDate.Date
            //    && EntityFunctions.TruncateTime(x.AttandanceDate) <= toDate.Date && x.Student.ClassSectionId == classSectionId)
            //    .Include(x => new Student { id = x.Student.id, RollNumber = x.Student.RollNumber, Name = x.Student.Name, FatherName = x.Student.FatherName }).Include(x => x.StudentAttendanceStatu).OrderBy(x => x.AttandanceDate).ToList();
        }

        public Class GetClassByName(string className)
        {
            return dbContext.Classes.Where(x => x.Name == className).FirstOrDefault();
        }

        public Class GetClassByNameAndId(string className, int classId)
        {
            return dbContext.Classes.Where(x => x.Name == className && x.Id != classId).FirstOrDefault();
        }

        public List<Class> GetAllClasses()
        {
            return dbContext.Classes.ToList();    
        }

        public void UpdateAttendance(Attandance attendance)
        {
            if (attendance != null)
            {
                dbContext.Entry(attendance).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteClass(Class clas)
        {
            if (clas != null)
            {
                dbContext.Classes.Remove(clas);
                dbContext.SaveChanges();
            }
        }

	public List<AttendanceModel1> GetAttendanceStatus(int id, DateTime fromDate, DateTime toDate)
        {
            if(fromDate == null)
            {
                fromDate = DateTime.Now;
            }
            if(toDate == null)
            {
                toDate = DateTime.Now;
            }
            //Student student = studentRepo.GetStudentById(id);
            dbContext.Configuration.LazyLoadingEnabled = false;
            List<AttendanceModel1> list = (from ic in dbContext.Attandances
                                           join student in dbContext.Students on ic.StudentID equals student.id
                                          where ic.StudentID == id && ic.AttandanceDate >= fromDate && ic.AttandanceDate <= toDate
                                           select new AttendanceModel1
                                   {
                                       name = student.Name,
                                       date =  ic.AttandanceDate,
                                       status = ic.StudentAttendanceStatu.CodeName
                                   }).ToList();
            return list;
        }



        public List<AttendanceRequestModel> GetAttendanceRequests(DateTime FromDate, DateTime ToDate, int StudentId)
        {
            var query = from request in dbContext.AttendanceRequests
                        join students in dbContext.Students on request.StudentId equals students.id
                        join status in dbContext.AttendanceRequestStatus on request.StatusId equals status.Id
                        join user in dbContext.Users on request.UserId equals user.Id
                           into qleft
                        from user in qleft.DefaultIfEmpty()
                        where EntityFunctions.TruncateTime(request.CreatedOn) >= FromDate.Date
                                && EntityFunctions.TruncateTime(request.CreatedOn) <= ToDate.Date
                                && (StudentId == 0 || request.StudentId == StudentId)
                        select new AttendanceRequestModel
                        {
                            Id = request.Id,
                            StudentId = request.StudentId,
                            StatusId = (int)request.StatusId,
                            RollNumber = students.RollNumber,
                            Name = students.Name,
                            FatherName = students.FatherName,
                            AdmissionNo = students.AdmissionNo,
                            Contact_1 = students.Contact_1,
                            RequestStatus = status.StatusDescription,
                            RequestDate = (DateTime)request.CreatedOn,
                            Comments = request.Remarks,
                            ApprovedBy = user.UserName
                        };

            return query.OrderByDescending(x => x.Id).ToList();
        }

        public List<AttendanceRequestModel> GetAttendanceRequests(int AttendanceRequestId)
        {
            var query = from request in dbContext.AttendanceRequests
                        join detail in dbContext.AttendanceRequestDetails on request.Id equals detail.AttendanceRequesId
                        join student in dbContext.Students on request.StudentId equals student.id
                        join att in dbContext.Attandances on detail.AttendanceId equals att.id
                        join status in dbContext.StudentAttendanceStatus on detail.AttendanceStatusId equals status.Id
                        join reqSt in dbContext.StudentAttendanceStatus on detail.StatusId equals reqSt.Id
                        where detail.AttendanceRequesId == AttendanceRequestId
                        select new AttendanceRequestModel
                        {
                            DetailId = detail.Id,
                            AttendanceId = att.id,
                            Id = request.Id,
                            AttendanceDate = (DateTime)att.AttandanceDate,
                            Status = status.CodeName,
                            StatusId = reqSt.Id,
                            StatusRequested = reqSt.CodeName,
                            Comments = detail.Comments,
                            StudentId = student.id,
                            RollNumber = student.RollNumber,
                            Name = student.Name,
                            FatherName = student.FatherName,
                            AdmissionNo = student.AdmissionNo,
                            AttendanceStatusId = (int)status.Id,
                            Remarks = request.Remarks
                        };

            return query.ToList();
        }


        public void AddAttendanceRequest(AttendanceRequest request)
        {
            if (request != null)
            {
                dbContext.AttendanceRequests.Add(request);
                dbContext.SaveChanges();
            }
        }

        public void UpdateAttendanceRequest(AttendanceRequest request)
        {
            if (request != null)
            {
                dbContext.Entry(request).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public AttendanceRequest GetAttendanceRequest(int RequestId)
        {
            return dbContext.AttendanceRequests.Find(RequestId);
        }

        public void AddAttendanceRequestDetail(AttendanceRequestDetail requestDetail)
        {
            if (requestDetail != null)
            {
                dbContext.AttendanceRequestDetails.Add(requestDetail);
                dbContext.SaveChanges();
            }
        }

        public void UpdateAttendanceRequestDetail(AttendanceRequestDetail requestDetail)
        {
            if (requestDetail != null)
            {
                dbContext.Entry(requestDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public AttendanceRequestDetail GetAttendanceRequestDetail(int RequestDetailId)
        {
            return dbContext.AttendanceRequestDetails.Find(RequestDetailId);
        }

        public List<AttendanceModel> GetStudentAttendanceByDate(int studentId, DateTime FromDate, DateTime ToDate)
        {
            var attendanceList = from att in dbContext.Attandances
                                 join student in dbContext.Students on att.StudentID equals student.id
                                 join status in dbContext.StudentAttendanceStatus on att.StatusId equals status.Id
                                 where (student.id == studentId)
                                 && (EntityFunctions.TruncateTime(att.AttandanceDate) >= FromDate.Date)
                                 && (EntityFunctions.TruncateTime(att.AttandanceDate) <= ToDate.Date)
                                 && student.LeavingStatus == 1
                                 select new AttendanceModel
                                 {
                                     Id = (int)att.id,
                                     StudentId = student.id,
                                     Name = student.Name,
                                     FatherName = student.FatherName,
                                     RollNumber = student.RollNumber,
                                     Status = status.CodeName,
                                     StatusId = status.Id,
                                     Contact_1 = student.Contact_1,
                                     AdmissionNo = (student.AdmissionNo == null ? "" : student.AdmissionNo),
                                     AttendanceDate = (DateTime)att.AttandanceDate
                                 };
            return attendanceList.ToList();
        }

        public Attandance GetAttandanceById(int Id)
        {
            return dbContext.Attandances.Find(Id);
        }

        public void DeleteAttendance(Attandance attendance)
        {
            dbContext.Attandances.Remove(attendance);
            dbContext.SaveChanges();
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
