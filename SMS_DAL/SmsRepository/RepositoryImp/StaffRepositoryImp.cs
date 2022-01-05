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

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class StaffRepositoryImp : IStaffRepository
    {
        private SC_WEBEntities2 dbContext1;

        public StaffRepositoryImp(SC_WEBEntities2 context)
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

        public List<Staff> GetAllStaff()
        {
            return dbContext.Staffs.ToList();
        }

        #region SESSION_FUNCTION
        public int AddSession(Session session)
        {
            int result = -1;
            if (session != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Sessions.Add(session);
                dbContext.SaveChanges();
                result = session.Id;
            }

            return result;
        }

        public Session GetSessionById(int sessionId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sessions.Where(x => x.Id == sessionId).FirstOrDefault();
        }

        public Session GetCurrentSession()
        {
            return dbContext.Sessions.OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public Session GetSessionByName(string sessionName)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sessions.Where(x => x.Name == sessionName).FirstOrDefault();
        }

        public Session GetSessionByNameAndId(string sessionName, int sessionId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sessions.Where(x => x.Name == sessionName && x.Id != sessionId).FirstOrDefault();
        }

        public List<Session> GetAllSessions()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sessions.ToList();
        }

        public void UpdateSession(Session session)
        {
            if (session != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(session).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public Session GetSessionInDates(Session session, int branchId)
        {
            return dbContext.Sessions.Where(x => (x.From_Date >= session.From_Date && x.From_Date <= session.To_Date)
                            || (x.To_Date >= session.From_Date && x.To_Date <= session.To_Date)
                            && x.BranchId == branchId).FirstOrDefault();
        }

        public Session GetEditSessionInDates(Session session)
        {
            return dbContext.Sessions.Where(x => ((x.From_Date >= session.From_Date && x.From_Date <= session.To_Date)
                            || (x.To_Date >= session.From_Date && x.To_Date <= session.To_Date)) && x.Id != session.Id).FirstOrDefault();
        }

        public void DeleteSession(Session session)
        {
            if (session != null)
            {
                dbContext.Sessions.Remove(session);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region DESIGNATION_FUNCTION
        public int AddDesignation(Designation designation)
        {
            int result = -1;
            if (designation != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Designations.Add(designation);
                dbContext.SaveChanges();
                result = designation.Id;
            }

            return result;
        }

        public Designation GetDesignationById(int designationId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Designations.Where(x => x.Id == designationId).FirstOrDefault();
        }

        public Designation GetDesignationByName(string designationName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Designations.Where(x => x.Name == designationName && x.BranchId == branchId).FirstOrDefault();
        }

        public Designation GetDesignationByNameAndId(string designationName, int designationId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Designations.Where(x => x.Name == designationName && x.Id == designationId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<Designation> GetAllDesignationsByCategoryId(int categoryId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Designations.Where(x => x.CatagoryId == categoryId).ToList();
        }

        public List<Designation> GetAllDesignations()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Designations.Include(x => x.DesignationCatagory).OrderBy(x => x.Id).ToList();
        }

        public void UpdateDesignation(Designation designation)
        {
            if (designation != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(designation).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteDesignation(Designation designation)
        {
            if (designation != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Designations.Remove(designation);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region DESIGNATION_CATEGORY_FUNCTION
        public int AddDesignationCategory(DesignationCatagory category)
        {
            int result = -1;
            if (category != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.DesignationCatagories.Add(category);
                dbContext.SaveChanges();
                result = category.Id;
            }

            return result;
        }

        public DesignationCatagory GetDesignationCategoryById(int categoryId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.DesignationCatagories.Where(x => x.Id == categoryId).FirstOrDefault();
        }

        public DesignationCatagory GetDesignationCategoryByName(string categoryName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.DesignationCatagories.Where(x => x.CatagoryName == categoryName && x.BranchId == branchId).FirstOrDefault();
        }

        public DesignationCatagory GetDesignationCategoryByNameAndId(string categoryName, int categoryId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.DesignationCatagories.Where(x => x.CatagoryName == categoryName && x.Id != categoryId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<DesignationCatagory> GetAllDesignationCategories()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.DesignationCatagories.ToList();
        }

        public void UpdateDesignationCategory(DesignationCatagory category)
        {
            if (category != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(category).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteDesignationCategory(DesignationCatagory category)
        {
            if (category != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.DesignationCatagories.Remove(category);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region ALLOWNCE_FUNCTION
        public int AddAllownce(Allownce allownce)
        {
            int result = -1;
            if (allownce != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Allownces.Add(allownce);
                dbContext.SaveChanges();
                result = allownce.Id;
            }

            return result;
        }

        public Allownce GetAllownceById(int allownceId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Allownces.Where(x => x.Id == allownceId).FirstOrDefault();
        }

        public Allownce GetAllowncenByName(string allownceName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Allownces.Where(x => x.Name == allownceName && x.BranchId == branchId).FirstOrDefault();
        }

        public Allownce GetAllownceByNameAndId(string allownceName, int allownceId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Allownces.Where(x => x.Name == allownceName && x.Id != allownceId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<Allownce> GetAllAllownces()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Allownces.OrderBy(x => x.Id).ToList();
        }


        public void UpdateAllownce(Allownce allownce)
        {
            if (allownce != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(allownce).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteAllownce(Allownce allownce)
        {
            if (allownce != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Allownces.Remove(allownce);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region STAFF_ATTENDANCE_POLICY_FUNCTION
        public int AddStaffAttandancePolicy(StaffAttandancePolicy policy)
        {
            int result = -1;
            if (policy != null)
            {
                dbContext.StaffAttandancePolicies.Add(policy);
                dbContext.SaveChanges();
                result = policy.Id;
            }

            return result;
        }

        public StaffAttandancePolicy GetStaffAttandancePolicyById(int policyId)
        {
            return dbContext.StaffAttandancePolicies.Where(x => x.Id == policyId).FirstOrDefault();
        }

        public StaffAttandancePolicy GetStaffAttandancePolicyByDesignationId(int designationId, int branchId)
        {
            return dbContext.StaffAttandancePolicies.Where(x => x.DesignationId == designationId && x.BranchId == branchId).FirstOrDefault();
        }

        public StaffAttandancePolicy GetStaffAttandancePolicyByDesignationIdAndId(int designationId, int policyId, int branchId)
        {
            return dbContext.StaffAttandancePolicies.Where(x => x.DesignationId == designationId && x.Id != policyId
                && x.BranchId == branchId).FirstOrDefault();
        }

        public List<StaffAttandancePolicy> GetAllStaffAttandancePolicies()
        {
            return dbContext.StaffAttandancePolicies.OrderBy(x => x.Id).ToList();
        }

        public List<StaffAttandancePolicyModel> GetAllStaffAttandancePoliciesModel()
        {
            var query = from policy in dbContext.StaffAttandancePolicies
                        join design in dbContext.Designations on policy.DesignationId equals design.Id
                        select new StaffAttandancePolicyModel
                        {
                            BranchId = policy.BranchId,
                            DesignationId = design.Id,
                            DesignationName = design.Name,
                            EarlyOutTime = policy.EarlyOutTime,
                            HalfDayTime = policy.HalfDayTime,
                            Id = policy.Id,
                            LateInCount = policy.LateInCount,
                            LateInTime = policy.LateInTime,
                            LeaveInMonth = policy.LeaveInMonth,
                            LeaveInYear = policy.LeaveInYear,
                            SalaryDeduction = policy.SalaryDeduction,
                            CategoryId = design.CatagoryId,
                            IsSalaryClubbed = policy.IsSundayClubed
                        };
            return query.ToList();
        }

        public void UpdateStaffAttandancePolicy(StaffAttandancePolicy policy)
        {
            if (policy != null)
            {
                dbContext.Entry(policy).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteStaffAttandancePolicy(StaffAttandancePolicy policy)
        {
            if (policy != null)
            {
                dbContext.StaffAttandancePolicies.Remove(policy);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region STAFF_DEGREE_FUNCTION
        public List<StaffDegree> GetStaffDegreeByStaffId(int staffId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.StaffDegrees.Where(x => x.StaffId == staffId).ToList();
        }

        public int AddStaffDegree(StaffDegree staffDegree)
        {
            int result = -1;
            if (staffDegree != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.StaffDegrees.Add(staffDegree);
                dbContext.SaveChanges();
                result = staffDegree.Id;
            }

            return result;
        }

        public void DeleteStaffAllownce(StaffAllownce staffAllownce)
        {
            if (staffAllownce != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.StaffAllownces.Remove(staffAllownce);
                dbContext.SaveChanges();
            }
        }
        #endregion


        #region STAFF_ALLOWNCE_FUNCTION
        public StaffAllownce GetStaffAllownceById(int Id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.StaffAllownces.Find(Id);
        }
        public List<StaffAllownce> GetStaffAllownceByStaffId(int staffId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.StaffAllownces.Where(x => x.StaffId == staffId).Include(x => x.Allownce).OrderBy(x => x.Id).ToList();
        }

        public StaffAllownce GetStaffAllownceByStaffId(int staffId, int allownceId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.StaffAllownces.Where(x => x.StaffId == staffId && x.AllownceId == allownceId).FirstOrDefault();
        }

        public int AddStaffAllownce(StaffAllownce staffAllownce)
        {
            int result = -1;
            if (staffAllownce != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.StaffAllownces.Add(staffAllownce);
                dbContext.SaveChanges();
                result = staffAllownce.Id;
            }

            return result;
        }

        public void UpdateStaffAllownce(StaffAllownce staffAllownce)
        {
            if (staffAllownce != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                StaffAllownce sessionObject = dbContext.StaffAllownces.Find(staffAllownce.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(staffAllownce);
                //dbContext.Entry(classSection).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteStaffDegree(StaffDegree staffDegree)
        {
            if (staffDegree != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.StaffDegrees.Remove(staffDegree);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region STAFF_FUNCTION
        public int AddStaff(Staff staff)
        {
            int result = -1;
            if (staff != null)
            {
                dbContext.Staffs.Add(staff);
                dbContext.SaveChanges();
                result = staff.StaffId;
            }

            return result;
        }

        public Staff GetStaffById(int staffId)
        {
            return dbContext.Staffs.Where(x => x.StaffId == staffId).FirstOrDefault();
        }

        public StaffModel GetStaffModelById(int staffId)
        {
            var query = from staff in dbContext.Staffs
                        join design in dbContext.Designations on staff.DesignationId equals design.Id
                        join cat in dbContext.BehaviourCategories on design.CatagoryId equals cat.ID
                        where (staffId == 0 || staff.StaffId == staffId)
                        select new StaffModel
                        {
                            StaffId = staff.StaffId,
                            StaffImage = staff.StaffImage,
                            ImageLocation = staff.ImageLocation,
                            Name = staff.Name,
                            FatherName = staff.FatherName,
                            PhoneNumber = staff.PhoneNumber,
                            DesignationName = design.Name,
                            CategoryName = cat.Name,
                            JoinDate = staff.JoinDate,
                            LeavingDate = staff.LeavingDate,
                            CNIC = staff.CNIC,
                            Salary = staff.Salary, 
                            Allownces = staff.Allownces,
                            DateOfBirth = staff.DateOfBirth, 

                        };
            return query.FirstOrDefault();
        }


        public void AddStaffAdvance(StaffAdvance advance)
        {
            dbContext.StaffAdvances.Add(advance);
            dbContext.SaveChanges();
        }

        public void UpdateStaffAdvance(StaffAdvance advance)
        {
            dbContext.Entry(advance).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<StaffAdvance> GetSTaffAdvancesByStaffId(int staffId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.StaffAdvances.Where(x => x.StaffId == staffId).ToList();
        }

        public StaffAdvance GetStaffAdvancesById(int id)
        {
            return dbContext.StaffAdvances.Where(x => x.Id == id).Include(x => x.Staff).FirstOrDefault();
        }

        public void AddStaffMiscWithdraw(StaffMiscWithdraw withdraw)
        {
            dbContext.StaffMiscWithdraws.Add(withdraw);
            dbContext.SaveChanges();
        }

        public void UpdateStaffMiscWithdraw(StaffMiscWithdraw withdraw)
        {
            dbContext.Entry(withdraw).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<StaffMiscWithdraw> GetStaffMiscWithdrawByStaffId(int staffId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.StaffMiscWithdraws.Where(x => x.StaffId == staffId).ToList();
        }

        public StaffMiscWithdraw GetStaffMiscWithdrawsById(int id)
        {
            return dbContext.StaffMiscWithdraws.Where(x => x.Id == id).Include(x => x.Staff).FirstOrDefault();
        }


        public Staff GetStaffByNameAndFatherName(string name, string fatherName)
        {
            return dbContext.Staffs.Where(x => x.Name == name && x.FatherName == fatherName).FirstOrDefault();
        }

        public Staff GetStaffByNameAndFatherNameAndId(string name, string fatherName, int staffId)
        {
            return dbContext.Staffs.Where(x => x.Name == name && x.FatherName == fatherName && x.StaffId != staffId).FirstOrDefault();
        }

        public int GetStaffMiscWithdraws(string forMonth, int staffId)
        {
            int miscWithdraws = 0;

            var count = dbContext.StaffMiscWithdraws.Where(x => x.ForMonth == forMonth && x.StaffId == staffId).Count();
            if (count > 0)
            {
                miscWithdraws = (int)dbContext.StaffMiscWithdraws.Where(x => x.ForMonth == forMonth && x.StaffId == staffId).Sum(x => x.WithdrawAmount);
            }

            return miscWithdraws;
        }

        public List<Staff> SearchStaff(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId)
        {
            return dbContext.Staffs.Where(x => (0 == categoryId || x.Designation.CatagoryId == categoryId) &&
                   (0 == designationId || x.DesignationId == designationId) &&
                   (0 == staffId || x.StaffId == staffId) && ("" == staffName || x.Name == staffName)
                   && x.IsLeft == false
                   && ("" == fatherName || x.FatherName == fatherName) && x.BranchId == branchId).ToList();
        }

        public List<StaffModel> SearchStaffModel(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId)
        {
            var query = from staff in dbContext.Staffs
                        join design in dbContext.Designations on staff.DesignationId equals design.Id
                        join cat in dbContext.DesignationCatagories on design.CatagoryId equals cat.Id
                        where (categoryId == 0 || cat.Id == categoryId)
                        && (designationId == 0 || design.Id == designationId)
                        && (staffId == 0 || staff.StaffId == staffId)
                        && ("" == staffName || staff.Name == staffName)
                        && staff.IsLeft == false && staff.BranchId == branchId
                        && ("" == fatherName || staff.FatherName == fatherName)
                        select new StaffModel
                        {
                            StaffId = staff.StaffId,
                            StaffImage = staff.StaffImage,
                            ImageLocation = staff.ImageLocation,
                            Name = staff.Name,
                            FatherName = staff.FatherName,
                            PhoneNumber = staff.PhoneNumber,
                            DesignationName = design.Name,
                            CategoryName = cat.CatagoryName,
                            IsLeft = staff.IsLeft
                        };
            return query.ToList();
        }

        public List<Staff> SearchAllStaff(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId)
        {
            return dbContext.Staffs.Where(x => (0 == categoryId || x.Designation.CatagoryId == categoryId) &&
                   (0 == designationId || x.DesignationId == designationId) &&
                   (0 == staffId || x.StaffId == staffId) && ("" == staffName || x.Name == staffName)
                   && ("" == fatherName || x.FatherName == fatherName) && x.BranchId == branchId).ToList();
        }

        public List<StaffModel> SearchAllStaffModel(int categoryId, int designationId, int staffId, string staffName, string fatherName, int branchId)
        {
            var query = from staff in dbContext.Staffs
                        join design in dbContext.Designations on staff.DesignationId equals design.Id
                        join cat in dbContext.DesignationCatagories on design.CatagoryId equals cat.Id
                        where (categoryId == 0 || cat.Id == categoryId)
                        && (designationId == 0 || design.Id == designationId)
                        && (staffId == 0 || staff.StaffId == staffId)
                        && ("" == staffName || staff.Name == staffName)
                        && staff.BranchId == branchId
                        && ("" == fatherName || staff.FatherName == fatherName)
                        select new StaffModel
                        {
                            StaffId = staff.StaffId,
                            StaffImage = staff.StaffImage,
                            ImageLocation = staff.ImageLocation,
                            Name = staff.Name,
                            FatherName = staff.FatherName,
                            PhoneNumber = staff.PhoneNumber,
                            DesignationName = design.Name,
                            CategoryName = cat.CatagoryName,
                            IsLeft = staff.IsLeft
                        };
            return query.ToList();
        }

        public List<StaffAdvanceModel> SearchAdvance(int categoryId, int designationId, int staffId, DateTime fromDate, DateTime toDate, int branchId)
        {
            toDate = toDate.AddDays(1);
            var query = from staffAdvances in dbContext.StaffAdvances
                        join staff in dbContext.Staffs on staffAdvances.StaffId equals staff.StaffId
                        join designation in dbContext.Designations on staff.DesignationId equals designation.Id
                        where (categoryId == 0 || designation.CatagoryId == categoryId)
                        && (designationId == 0 || designation.Id == designationId)
                        && (staffId == 0 || staff.StaffId == staffId) && (staff.BranchId == branchId)
                        && staff.IsLeft == false
                        && (EntityFunctions.TruncateTime(staffAdvances.CreatedOn) >= fromDate.Date && EntityFunctions.TruncateTime(staffAdvances.CreatedOn) <= toDate.Date)
                        //&& (staffAdvances.CreatedOn >= toDate && staffAdvances.CreatedOn <= toDate)
                        select new StaffAdvanceModel
                        {
                            Id = staffAdvances.Id,
                            StaffId = staff.StaffId,
                            StaffName = staff.Name,
                            Date = (DateTime)staffAdvances.CreatedOn,
                            Amount = (int)staffAdvances.AdvanceAmount,
                            Remarks = staffAdvances.Remarks
                        };

            return query.ToList();
        }

        public List<StaffMiscWithdrawModel> SearchMiscWithdraws(int categoryId, int designationId, int staffId, DateTime fromDate, DateTime toDate, int branchId)
        {
            toDate = toDate.AddDays(1);
            var query = from staffAdvances in dbContext.StaffMiscWithdraws
                        join staff in dbContext.Staffs on staffAdvances.StaffId equals staff.StaffId
                        join designation in dbContext.Designations on staff.DesignationId equals designation.Id
                        where (categoryId == 0 || designation.CatagoryId == categoryId)
                        && (designationId == 0 || designation.Id == designationId)
                        && (staffId == 0 || staff.StaffId == staffId) && (staff.BranchId == branchId)
                        && staff.IsLeft == false
                        && (EntityFunctions.TruncateTime(staffAdvances.CreatedOn) >= fromDate.Date && EntityFunctions.TruncateTime(staffAdvances.CreatedOn) <= toDate.Date)
                        //&& (staffAdvances.CreatedOn >= toDate && staffAdvances.CreatedOn <= toDate)
                        select new StaffMiscWithdrawModel
                        {
                            Id = staffAdvances.Id,
                            StaffId = staff.StaffId,
                            StaffName = staff.Name,
                            ForMonth = staffAdvances.ForMonth,
                            Date = (DateTime)staffAdvances.CreatedOn,
                            Amount = (int)staffAdvances.WithdrawAmount,
                            Remarks = staffAdvances.Remarks
                        };

            return query.ToList();
        }

        public void UpdateStaff(Staff staff)
        {
            if (staff != null)
            {
                Staff sessionObject = dbContext.Staffs.Find(staff.StaffId);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(staff);
                //dbContext.Entry(classSection).State = EntityState.Modified;
                dbContext.SaveChanges();

                //dbContext.Entry(staff).State = EntityState.Modified;
                //dbContext.SaveChanges();
            }
        }

        public void DeleteStaff(Staff staff)
        {
            if (staff != null)
            {
                dbContext.Staffs.Remove(staff);
                dbContext.SaveChanges();
            }
        }

        public List<Staff> GetStaffByDesignation(string designatioName)
        {
            return dbContext.Staffs.Where(x => x.Designation.Name.ToLower().Contains(designatioName)).ToList();
        }
        #endregion

        #region SESSION_SUBJECTS_FUNCTIONS
        public StaffSubjectsStatsViewModel GetStaffSubjectStats(int branchId)
        {
            var response = new StaffSubjectsStatsViewModel();
            var staffList = dbContext.Staffs.Where(n => n.BranchId == branchId).ToList();
            var classes = dbContext.Classes.Where(n => n.BranchId == branchId).ToList();

            var sessionSubjects = GetAllSessionSubjectsModel().Where(n => n.BranchId == branchId).ToList();

            response.Staff = staffList.Select(n => n.Name).ToList();

            foreach (var classObj in classes)
            {
                var staffClassSubjectCount = new StaffClassSubjectCount();
                staffClassSubjectCount.ClassId = classObj.Id;
                staffClassSubjectCount.ClassName = classObj.Name;
                foreach (var staff in staffList)
                {
                    staffClassSubjectCount.SubjectCount.Add(sessionSubjects.Where(n => n.TeacherId == staff.StaffId && n.ClassId == classObj.Id).Count());
                }

                response.StaffClassSubjectCount.Add(staffClassSubjectCount);
            }
            return response;
        }
        public List<SessionSubject> GetAllSessionSubjects()
        {
            return dbContext.SessionSubjects.OrderBy(x => x.Id).ToList();
        }

        public List<SessionSubjectModel> GetAllSessionSubjectsModel()
        {
            var query = from ss in dbContext.SessionSubjects
                        join subj in dbContext.Subjects on ss.SubjectId equals subj.Id
                        join staff in dbContext.Staffs on ss.TeacherId equals staff.StaffId
                        join clsec in dbContext.ClassSections on ss.ClassSectionId equals clsec.ClassSectionId
                        join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        select new SessionSubjectModel
                        {
                            BranchId = (int)staff.BranchId,
                            ClassId = cls.Id,
                            ClassName = cls.Name,
                            ClassSectionId = clsec.ClassSectionId,
                            From_Date = ss.From_Date,
                            Id = ss.Id,
                            SectionId = sec.Id,
                            SectionName = sec.Name,
                            SessionYear = ss.SessionYear,
                            StaffName = staff.Name,
                            SubjectId = subj.Id,
                            SubjectName = subj.Name,
                            TeacherId = ss.TeacherId,
                            To_Date = ss.To_Date
                        };
            return query.ToList();
        }

        public SessionSubject GetSessionSubjectById(int Id)
        {
            return dbContext.SessionSubjects.Find(Id);
        }

        public void DeleteSessionSubject(SessionSubject subject)
        {
            dbContext.SessionSubjects.Remove(subject);
            dbContext.SaveChanges();
        }

        public int AddSessionSubject(SessionSubject sessionSubject)
        {
            int result = -1;
            if (sessionSubject != null)
            {
                dbContext.SessionSubjects.Add(sessionSubject);
                dbContext.SaveChanges();
                result = sessionSubject.Id;
            }

            return result;
        }

        public SessionSubject GetTeacherSessionSubjectInDates(SessionSubject subject)
        {
            return dbContext.SessionSubjects.Where(x => ((x.From_Date >= subject.From_Date && x.From_Date <= subject.To_Date)
                            || (x.To_Date >= subject.From_Date && x.To_Date <= subject.To_Date))
                            && x.TeacherId == subject.TeacherId && x.SubjectId == subject.SubjectId && x.ClassSectionId == subject.ClassSectionId).FirstOrDefault();
        }

        public SessionSubject GetSessionSubjectInDates(SessionSubject subject)
        {
            return dbContext.SessionSubjects.Where(x => ((x.From_Date >= subject.From_Date && x.From_Date <= subject.To_Date)
                            || (x.To_Date >= subject.From_Date && x.To_Date <= subject.To_Date))
                            && x.SubjectId == subject.SubjectId && x.ClassSectionId == subject.ClassSectionId).FirstOrDefault();
        }
        #endregion

        #region STAFF_SALARY_FUNCTIONS
        public List<StaffSalariesViewModel> GetSalaryStatsByMonth(int branchId, DateTime from, DateTime to)
        {
            return dbContext.StaffSalaries.Where(n => n.Staff.BranchId == branchId
                && EntityFunctions.TruncateTime(n.PaidDate) >= from.Date
                && EntityFunctions.TruncateTime(n.PaidDate) <= to.Date)
                .GroupBy(n => n.ForMonth)
                .Select(n => new StaffSalariesViewModel() { Month = n.Key, PaidDate = n.FirstOrDefault().PaidDate,
                    PayableSalary = n.Sum(x => x.SalaryAmount),
                    Salary = n.Sum(x => x.PaidAmount),
                    Bonus = n.Sum(x => x.Bonus),
                    AdvanceAdjustment = n.Sum(x => x.AdvanceAdjustment),
                    SalaryDeductions = n.Sum(x => x.Deduction) })
                .OrderBy(n => n.PaidDate).ToList();
        }
        public List<StaffSalary> SearchSalaries(int searchCatagoryId, int searchDesginationId, int searchStaffId, string forMonth, int branchId, bool deleteFlag = false)
        {
            if(deleteFlag)
                DeleteUnPaidStaffSalary(searchStaffId, forMonth);

            return dbContext.StaffSalaries.Where(x => (0 == searchCatagoryId || x.Staff.Designation.CatagoryId == searchCatagoryId)
                        && x.Staff.IsLeft == false
                        && (0 == searchDesginationId || x.Staff.DesignationId == searchDesginationId) && (0 == searchStaffId || x.StaffId == searchStaffId)
                        && x.ForMonth == forMonth && x.Staff.BranchId == branchId).Include(x => x.Staff).ToList();
        }

        public void DeleteUnPaidStaffSalary(int staffId, string forMonth)
        {
            var salary = dbContext.StaffSalaries.Where(x => x.StaffId == staffId && x.ForMonth == forMonth && x.PaidDate == null).FirstOrDefault();
            if (salary != null)
            {
                dbContext.StaffSalaries.Remove(salary);
                dbContext.SaveChanges();
            }
        }

        public List<StaffSalary> GetLastSixMonthSalaries(int staffId, int branchId)
        {
            return dbContext.StaffSalaries.Where(x => x.StaffId == staffId
                        && x.Staff.IsLeft == false
                        && x.PaidDate != null
                        && x.Staff.BranchId == branchId).OrderByDescending(x => x.SalaryId)
                        .Include(x => x.Staff).Skip(1).Take(6).ToList();
        }

        public StaffSalary SearchStaffSalaries(int staffId, string forMonth)
        {
            return dbContext.StaffSalaries.Where(x => x.StaffId == staffId && x.ForMonth == forMonth).FirstOrDefault(); ;
        }

        public void DeleteStaffSalary(int salaryId)
        {
            var staffSalary = dbContext.StaffSalaries.Find(salaryId);
            dbContext.StaffSalaries.Remove(staffSalary);
            dbContext.SaveChanges();
        }

        public List<StaffSalaryHistoryViewModel> SearchStaffSalaryDetail(string spQuery)
        {
            return dbContext.Database.SqlQuery<StaffSalaryHistoryViewModel>(spQuery).ToList();
        }

        public List<QueryResult> SearchAttendanceDetail(string spQuery)
        {
            return dbContext.Database.SqlQuery<QueryResult>(spQuery).ToList();
        }

        public int AddStaffSalary(StaffSalary staffSalary)
        {
            int result = -1;
            if (staffSalary != null)
            {
                dbContext.StaffSalaries.Add(staffSalary);
                dbContext.SaveChanges();
                result = staffSalary.SalaryId;
            }

            return result;
        }

        public void UpdateStaffSalary(StaffSalary salary)
        {
            dbContext.Entry(salary).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int AddStaffSalaryIncrementHostory(StaffSalaryIncrementHistory staffSalaryIncrementHistory)
        {
            int result = -1;
            if (staffSalaryIncrementHistory != null)
            {
                dbContext.StaffSalaryIncrementHistories.Add(staffSalaryIncrementHistory);
                dbContext.SaveChanges();
                result = staffSalaryIncrementHistory.IncrementId;
            }

            return result;
        }

        public StaffSalaryIncrementHistory GetStaffSalaryIncrementHistory(int incrementId)
        {
            return dbContext.StaffSalaryIncrementHistories.Find(incrementId);
        }

        public List<StaffSalaryIncrementHistory> GetStaffSalaryIncrementHistoryByAllownceId(int allownceId)
        {
            return dbContext.StaffSalaryIncrementHistories.Where(x => x.AllownceId == allownceId).ToList();
        }

        public void DeleteStaffSalaryIncrementHistory(StaffSalaryIncrementHistory staffSalaryIncrementHistory)
        {
            dbContext.StaffSalaryIncrementHistories.Remove(staffSalaryIncrementHistory);
            dbContext.SaveChanges();
        }

        #endregion

        #region STAFF_ATTENDANCE_FUNCTION
        public StaffAttandance GetStaffAttendanceById(int staffAttendanceId)
        {
            return dbContext.StaffAttandances.Find(staffAttendanceId);
        }

        public void DeleteStaffAttendance(StaffAttandance attendance)
        {
            dbContext.StaffAttandances.Remove(attendance);
            dbContext.SaveChanges();
        }
        public List<StaffAttandance> SearchAttendnace(DateTime markDate, int searchCatagoryId, int searchDesignationId, int branchId, int statusId = 0)
        {
            return dbContext.StaffAttandances.Where(x => EntityFunctions.TruncateTime(x.Date) == markDate.Date
                && (0 == searchCatagoryId || x.Staff.Designation.CatagoryId == searchCatagoryId)
                                            && x.Staff.IsLeft == false
                                            && (0 == searchDesignationId || x.Staff.Designation.Id == searchDesignationId)
                                            && x.Staff.BranchId == branchId
                                            && (statusId == 0 || statusId == x.Status)).Include(x => x.Staff).Include(x => x.StudentAttendanceStatu).ToList();
        }

        public List<StaffAttandanceModel> SearchStaffAttendnaceModel(DateTime markDate, int searchCatagoryId, int searchDesignationId, int branchId, int statusId = 0)
        {
            var query = from att in dbContext.StaffAttandances
                        join staff in dbContext.Staffs on att.StaffId equals staff.StaffId
                        join status in dbContext.StudentAttendanceStatus on att.Status equals status.Id
                        join design in dbContext.Designations on staff.DesignationId equals design.Id
                        join cat in dbContext.DesignationCatagories on design.CatagoryId equals cat.Id
                        where EntityFunctions.TruncateTime(att.Date) == markDate.Date
                                && staff.IsLeft == false
                                && (0 == searchCatagoryId || design.CatagoryId == searchCatagoryId)
                                && (0 == searchDesignationId || design.Id == searchDesignationId)
                                && staff.BranchId == branchId
                                && (statusId == 0 || statusId == att.Status)
                        select new StaffAttandanceModel
                        {
                            CodeName = status.CodeName,
                            CreatedOn = att.CreatedOn,
                            Date = att.Date,
                            FatherName = staff.FatherName,
                            Id = att.Id,
                            Name = staff.Name,
                            OutTime = att.OutTime,
                            StaffId = staff.StaffId,
                            Status = status.Id,
                            Time = att.Time
                        };
            return query.ToList();
        }


        public List<AttendanceStats> GetAttendanceStats(int branchId, DateTime date)
        {
            var stats = new List<AttendanceStats>();
            var attendance = dbContext.StaffAttandances.Where(n => n.Staff.BranchId == branchId && EntityFunctions.TruncateTime(n.Date) == date.Date).ToList();
            var status = dbContext.StudentAttendanceStatus.ToList();
            foreach (var st in status)
            {
                var stat = new AttendanceStats();
                stat.StatusId = st.Id;
                stat.StatusCode = st.CodeName;
                stat.Count = attendance.Where(n => n.StudentAttendanceStatu.Id == st.Id).Count();

                stats.Add(stat);
            }

            return stats;
        }

        public List<AttendanceStats> GetAttendanceStatsByMonth(int branchId, DateTime from, DateTime to)
        {
            var stats = new List<AttendanceStats>();
            var attendance = dbContext.StaffAttandances.Where(n => n.Staff.BranchId == branchId
                && EntityFunctions.TruncateTime(n.Date) >= from.Date
                && EntityFunctions.TruncateTime(n.Date) <= to.Date).ToList();
            var status = dbContext.StudentAttendanceStatus.ToList();
            foreach (var st in status)
            {
                var stat = new AttendanceStats();
                stat.StatusId = st.Id;
                stat.StatusCode = st.CodeName;
                stat.Count = attendance.Where(n => n.StudentAttendanceStatu.Id == st.Id).Count();

                stats.Add(stat);
            }

            return stats;
        }

        public DashboardDataViewModels GetAttendanceStatsByDate(int branchId, DateTime from, DateTime to, string view = "day")
        {
            var response = new DashboardDataViewModels();
            response.StaffAttendanceStats = new List<AttendanceStats>();
            var attendance = dbContext.StaffAttandances.Where(n => n.Staff.BranchId == branchId
                && EntityFunctions.TruncateTime(n.Date) >= from.Date
                && EntityFunctions.TruncateTime(n.Date) <= to.Date).ToList();
            var status = dbContext.StudentAttendanceStatus.ToList();

            if (view == "month")
            {
                var yearMonths = attendance.Select(n => n.Date.Value.Year + n.Date.Value.Month).Distinct();
                response.Months = new List<string>();
                foreach (var st in status)
                {
                    var stat = new AttendanceStats();
                    stat.StatusId = st.Id;
                    stat.StatusCode = st.CodeName;
                    stat.Data = new List<int>();

                    var attendanceByStatus = attendance.Where(n => n.StudentAttendanceStatu.Id == st.Id);
                    foreach (var yearMonth in yearMonths)
                    {
                        var monthEntries = attendance.Where(n => (n.Date.Value.Year + n.Date.Value.Month) == yearMonth);
                        string month = monthEntries.First().Date.Value.ToString("MMM") + "-" + monthEntries.First().Date.Value.ToString("yyyy");
                        if (response.Months.Contains(month) == false)
                            response.Months.Add(month);

                        stat.Data.Add(attendanceByStatus.Where(n => (n.Date.Value.Year + n.Date.Value.Month) == yearMonth).Count());
                    }
                    response.StaffAttendanceStats.Add(stat);
                }
            }
            else
            {
                response.Dates = attendance.OrderBy(n => n.Date).Select(n => n.Date.Value.Date).Distinct().ToList();
                foreach (var st in status)
                {
                    var stat = new AttendanceStats();
                    stat.StatusId = st.Id;
                    stat.StatusCode = st.CodeName;
                    stat.Data = new List<int>();

                    var attendanceByStatus = attendance.Where(n => n.StudentAttendanceStatu.Id == st.Id);
                    foreach (var date in response.Dates)
                    {
                        stat.Data.Add(attendanceByStatus.Where(n => n.Date.Value.Date == date.Date).Count());
                    }

                    response.StaffAttendanceStats.Add(stat);
                }
            }
            return response;
        }
        public List<StaffAttandance> SearchAttendnace(DateTime fromDate, DateTime toDate, int searchCatagoryId, int searchDesignationId, int staffId, int branchId, int statusId = 0)
        {
            return dbContext.StaffAttandances.Where(x => (EntityFunctions.TruncateTime(x.Date) >= fromDate.Date && EntityFunctions.TruncateTime(x.Date) <= toDate.Date)
                && (0 == searchCatagoryId || x.Staff.Designation.CatagoryId == searchCatagoryId)
                && (0 == staffId || x.Staff.StaffId == staffId)
                && x.Staff.IsLeft == false
                && (0 == searchDesignationId || x.Staff.Designation.Id == searchDesignationId)
                && x.Staff.BranchId == branchId
                && (statusId > 0 ? statusId == x.Status : true)).OrderByDescending(x => x.Date).ToList();
        }


        public List<StaffAttandanceModel> SearchStaffAttendnaceModel(DateTime fromDate, DateTime toDate, int searchCatagoryId, int searchDesignationId, int staffId, int branchId, int statusId = 0)
        {
            var query = from att in dbContext.StaffAttandances
                        join staff in dbContext.Staffs on att.StaffId equals staff.StaffId
                        join status in dbContext.StudentAttendanceStatus on att.Status equals status.Id
                        join design in dbContext.Designations on staff.DesignationId equals design.Id
                        join cat in dbContext.DesignationCatagories on design.CatagoryId equals cat.Id
                        where (EntityFunctions.TruncateTime(att.Date) >= fromDate.Date
                                && EntityFunctions.TruncateTime(att.Date) <= toDate.Date)
                                && staff.IsLeft == false
                                && (staffId == 0 || staffId == staff.StaffId)
                                && (0 == searchCatagoryId || design.CatagoryId == searchCatagoryId)
                                && (0 == searchDesignationId || staff.Designation.Id == searchDesignationId)
                                && staff.BranchId == branchId
                                && (statusId > 0 ? statusId == att.Status : true)
                        select new StaffAttandanceModel
                        {
                            CodeName = status.CodeName,
                            CreatedOn = att.CreatedOn,
                            Date = att.Date,
                            FatherName = staff.FatherName,
                            Id = att.Id,
                            Name = staff.Name,
                            OutTime = att.OutTime,
                            StaffId = staff.StaffId,
                            Status = status.Id,
                            Time = att.Time
                        };
            return query.OrderBy(x => x.Id).ToList();
        }

        public List<StaffAttandance> SearchAttendnaceForTheDay(DateTime fromDate)
        {
            return dbContext.StaffAttandances.Where(x => EntityFunctions.TruncateTime(x.Date) == fromDate.Date).ToList();
        }


        public StaffAttandance SearchStaffDailyAttendance(DateTime attedanceDate, int staffId)
        {
            return dbContext.StaffAttandances.Where(x => EntityFunctions.TruncateTime(x.Date) == attedanceDate.Date
                && x.StaffId == staffId).FirstOrDefault();
        }

        public StaffAttandance GetStaffDailyAttendance(DateTime attedanceDate, int staffId)
        {
            //var query = from stAt in dbContext.StaffAttandances
            //            join st in dbContext.Staffs on stAt.StaffId equals st.StaffId
            //            where stAt.Date.Value.Year == attedanceDate.Year &&
            //            stAt.Date.Value.Month == attedanceDate.Month && stAt.Date.Value.Day == attedanceDate.Day
            //            && st.StaffId == staffId
            //            select stAt;

            //return query.FirstOrDefault();
            var staffAttendance = dbContext.StaffAttandances.Where(x => x.Date.Value.Year == attedanceDate.Year &&
                x.Date.Value.Month == attedanceDate.Month && x.Date.Value.Day == attedanceDate.Day
                && x.StaffId == staffId).FirstOrDefault();
            if (staffId == 0)
            {
                staffAttendance = dbContext.StaffAttandances.Where(x => x.Date.Value.Year == attedanceDate.Year &&
               x.Date.Value.Month == attedanceDate.Month && x.Date.Value.Day == attedanceDate.Day).FirstOrDefault();
            }
            return staffAttendance;
        }

        public int AddStaffAttendnace(StaffAttandance staffAttandance)
        {
            int result = -1;
            if (staffAttandance != null)
            {
                dbContext.StaffAttandances.Add(staffAttandance);
                dbContext.SaveChanges();
                result = staffAttandance.Id;
            }

            return result;
        }

        public int AddStaffAttendnaceLogs(StaffAttendanceLog log)
        {
            int result = -1;
            if (log != null)
            {
                dbContext.StaffAttendanceLogs.Add(log);
                dbContext.SaveChanges();
                result = log.Id;
            }

            return result;
        }

        public int GetAttendanceLogId(int staffId, string dateTimeStr)
        {
            var log = dbContext.StaffAttendanceLogs.Where(x => x.StaffId == staffId && x.DateTimeString == dateTimeStr).FirstOrDefault();
            return log == null ? 0 : log.Id;
        }

        public void UpdateStaffAttendance(StaffAttandance staffAttendance)
        {
            if (staffAttendance != null)
            {
                StaffAttandance sessionObject = dbContext.StaffAttandances.Find(staffAttendance.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(staffAttendance);
                //dbContext.Entry(classSection).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public StaffAttendanceDetail GetTopStaffAttendanceDetailByAttId(int attendanceId)
        {
            int count = dbContext.StaffAttendanceDetails.Where(x => x.AttendanceId == attendanceId).Count();
            StaffAttendanceDetail detail = null;
            if (count > 0)
            {
                detail = dbContext.StaffAttendanceDetails.Where(x => x.AttendanceId == attendanceId).OrderByDescending(x => x.Id).FirstOrDefault();
            }

            return detail;
        }

        public StaffAttendanceDetail GetFirstStaffAttendanceDetailByAttId(int attendanceId)
        {
            int count = dbContext.StaffAttendanceDetails.Where(x => x.AttendanceId == attendanceId).Count();
            StaffAttendanceDetail detail = null;
            if (count > 0)
            {
                detail = dbContext.StaffAttendanceDetails.Where(x => x.AttendanceId == attendanceId).OrderBy(x => x.Id).FirstOrDefault();
            }

            return detail;
        }

        public List<StaffAttendanceDetail> GetStaffAttendanceDetailByAttId(int attendanceId)
        {
            return dbContext.StaffAttendanceDetails.Where(x => x.AttendanceId == attendanceId).OrderBy(x => x.Id).ToList();
        }

        public StaffAttendanceDetail GetStaffAttendanceDetailById(int attendanceId)
        {
            return dbContext.StaffAttendanceDetails.Where(x => x.Id == attendanceId).FirstOrDefault();
        }


        public int AddStaffAttendnaceDetail(StaffAttendanceDetail staffAttandanceDetail)
        {
            int result = -1;
            if (staffAttandanceDetail != null)
            {
                dbContext.StaffAttendanceDetails.Add(staffAttandanceDetail);
                dbContext.SaveChanges();
                result = staffAttandanceDetail.Id;
            }

            return result;
        }

        public void UpdateStaffAttendanceDetail(StaffAttendanceDetail staffAttandanceDetail)
        {
            if (staffAttandanceDetail != null)
            {
                StaffAttendanceDetail sessionObject = dbContext.StaffAttendanceDetails.Find(staffAttandanceDetail.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(staffAttandanceDetail);
                //dbContext.Entry(classSection).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteStaffAttendanceDetail(StaffAttendanceDetail staffAttandanceDetail)
        {
            if (staffAttandanceDetail != null)
            {
                dbContext.StaffAttendanceDetails.Remove(staffAttandanceDetail);
                dbContext.SaveChanges();
            }
        }

        public List<StaffAttendanceDetailModel> GetStaffAttendnaceDetailByStaffId(DateTime attendnaceDate, int staffId)
        {
            //EntityFunctions.TruncateTime(request.CreatedOn) >= FromDate.Date
            List<StaffAttendanceDetailModel> list = new List<StaffAttendanceDetailModel>();

            var query = from att in dbContext.StaffAttandances
                        join attDetail in dbContext.StaffAttendanceDetails on att.Id equals attDetail.AttendanceId
                        where EntityFunctions.TruncateTime(att.Date) == attendnaceDate.Date
                        && att.StaffId == staffId
                        select new StaffAttendanceDetailModel
                        {
                            Id = attDetail.Id,
                            AttendnaceDate = (DateTime)att.Date,
                            AttendnaceId = att.Id,
                            StaffId = (int)att.StaffId,
                            TimeIn = attDetail.TimeIn,
                            TimeOut = attDetail.TimeOut
                        };
            list = query.ToList();
            return list;
        }

        #endregion


        #region NON_UI_FUNCTIONS

        public List<AllowLeave> GetAllAllowLeaves()
        {
            return dbContext.AllowLeaves.OrderBy(x => x.Id).ToList();
        }

        public List<LateInCount> GetAllLateInCOunts()
        {
            return dbContext.LateInCounts.OrderBy(x => x.Id).ToList();
        }

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            return dbContext.PaymentMethods.OrderBy(x => x.Id).ToList();
        }

        public List<MeritalStatu> GetAllMeritalStatuses()
        {
            return dbContext.MeritalStatus.OrderBy(x => x.Id).ToList();
        }

        public List<StaffType> GetAllStaffTypes()
        {
            return dbContext.StaffTypes.ToList();
        }
        #endregion


        #region API_FUNCTIONS
        public string AddStaffBioMatric(string staffId, string bioHash)
        {
            try
            {
                Staff staff = GetStaffById(int.Parse(staffId));
                staff.BioMatricHash = bioHash;
                UpdateStaff(staff);
                return "000";
            }
            catch
            {
                return "420";
            }
        }

        public void AddStaffPaymentApprovalHistory(StaffPaymentApproval approval)
        {
            dbContext.StaffPaymentApprovals.Add(approval);
            dbContext.SaveChanges();
        }

        public List<string> GetAPIAllStaff()
        {
            List<Staff> staffList = GetAllStaff();
            List<string> list = new List<string>();
            foreach (Staff staff in staffList)
            {
                string staffInfo = staff.StaffId + "," + staff.Name + "," + staff.FatherName + ","
                                    + ((staff.BioMatricHash == null || staff.BioMatricHash.Length == 0) ? "NO" : "YES");
                list.Add(staffInfo);
            }

            return list;
        }

        public string GetAPIStaffById(string staffId)
        {
            Staff staff = GetStaffById(int.Parse(staffId));
            return staff.StaffId + "," + staff.Name + "," + staff.FatherName + ","
                                    + ((staff.BioMatricHash == null || staff.BioMatricHash.Length == 0) ? "NO" : "YES");
        }

        #endregion


        public int GetDesignationCount(int CategoryId)
        {
            return dbContext.Designations.Where(x => x.CatagoryId == CategoryId).Count();
        }

        public int GetStaffCount(int DesignationId)
        {
            return dbContext.Staffs.Where(x => x.DesignationId == DesignationId).Count();
        }

        public int GetStudentCount(int SessionId)
        {
            int studentCount = dbContext.Students.Where(x => x.SessionId == SessionId).Count();

            if (studentCount == 0)
            {
                studentCount = dbContext.StudentInquiries.Where(x => x.SessionId == SessionId).Count();
            }

            return studentCount;
        }

        public void AddStaffAttendanceRequest(StaffAttendanceRequest request)
        {
            if (request != null)
            {
                dbContext.StaffAttendanceRequests.Add(request);
                dbContext.SaveChanges();
            }
        }

        public void UpdateStaffAttendanceRequest(StaffAttendanceRequest request)
        {
            if (request != null)
            {
                dbContext.Entry(request).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public StaffAttendanceRequest GetStaffAttendanceRequest(int RequestId)
        {
            return dbContext.StaffAttendanceRequests.Find(RequestId);
        }

        public void AddStaffAttendanceRequestDetail(StaffAttendanceRequestDetail requestDetail)
        {
            if (requestDetail != null)
            {
                dbContext.StaffAttendanceRequestDetails.Add(requestDetail);
                dbContext.SaveChanges();
            }
        }

        public void UpdateStaffAttendanceRequestDetail(StaffAttendanceRequestDetail requestDetail)
        {
            if (requestDetail != null)
            {
                dbContext.Entry(requestDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public StaffAttendanceRequestDetail GetStaffAttendanceRequestDetail(int RequestDetailId)
        {
            return dbContext.StaffAttendanceRequestDetails.Find(RequestDetailId);
        }

        public List<StaffAttendanceRequestModel> GetStaffAttendanceRequests(DateTime FromDate, DateTime ToDate, int StaffId)
        {
            if (ToDate.Date == FromDate.Date)
                ToDate = ToDate.AddDays(1);
            var query = from request in dbContext.StaffAttendanceRequests
                        join staffs in dbContext.Staffs on request.StaffId equals staffs.StaffId
                        join status in dbContext.AttendanceRequestStatus on request.StatusId equals status.Id
                        join designation in dbContext.Designations on staffs.DesignationId equals designation.Id
                        join user in dbContext.Users on request.UserId equals user.Id
                            into qleft
                        from user in qleft.DefaultIfEmpty()
                        where EntityFunctions.TruncateTime(request.CreatedOn) >= FromDate.Date
                                && EntityFunctions.TruncateTime(request.CreatedOn) <= ToDate.Date
                                && (StaffId == 0 || request.StaffId == StaffId)
                        select new StaffAttendanceRequestModel
                        {
                            Id = request.Id,
                            StaffId = request.StaffId,
                            StatusId = (int)request.StatusId,
                            Name = staffs.Name,
                            FatherName = staffs.FatherName,
                            Designation = designation.Name,
                            RequestStatus = status.StatusDescription,
                            RequestDate = (DateTime)request.CreatedOn,
                            Comments = request.Remarks,
                            ApprovedBy = user.UserName,
                            Remarks = request.Remarks
                        };

            return query.OrderByDescending(x => x.Id).ToList();
        }

        public List<StaffAttendanceRequestModel> GetStaffAttendanceRequests(int StaffAttendanceRequestId)
        {
            var query = from request in dbContext.StaffAttendanceRequests
                        join detail in dbContext.StaffAttendanceRequestDetails on request.Id equals detail.StaffAttendanceRequestId
                        join staff in dbContext.Staffs on request.StaffId equals staff.StaffId
                        join designation in dbContext.Designations on staff.DesignationId equals designation.Id
                        join att in dbContext.StaffAttandances on detail.AttendanceId equals att.Id
                        join status in dbContext.StudentAttendanceStatus on detail.AttendanceStatusId equals status.Id
                        join reqSt in dbContext.StudentAttendanceStatus on detail.StatusId equals reqSt.Id
                        where detail.StaffAttendanceRequestId == StaffAttendanceRequestId
                        select new StaffAttendanceRequestModel
                        {
                            DetailId = detail.Id,
                            AttendanceId = att.Id,
                            Id = request.Id,
                            AttendanceDate = (DateTime)att.Date,
                            Status = status.CodeName,
                            StatusId = reqSt.Id,
                            StatusRequested = reqSt.CodeName,
                            Comments = detail.Comments,
                            TimeIn = detail.InTime,
                            TimeOut = detail.OutTime,
                            StaffId = request.StaffId,
                            Name = staff.Name,
                            FatherName = staff.FatherName,
                            Designation = designation.Name,
                            AttendanceStatusId = (int)status.Id,
                            AttendanceTimeIn = detail.AttendanceInTime,
                            AttendanceTimeOut = detail.AttendanceOutTime,
                            Remarks = request.Remarks
                        };

            return query.ToList();
        }

        public List<StaffAttandance> SearchStaffAttendnace(DateTime FromDate, DateTime ToDate, int staffId)
        {
            return dbContext.StaffAttandances.Where(x => EntityFunctions.TruncateTime(x.Date) >= FromDate.Date
                && EntityFunctions.TruncateTime(x.Date) <= ToDate.Date
                                            && x.Staff.IsLeft == false
                                            && (staffId == 0 || staffId == x.StaffId)).Include(x => x.Staff).Include(x => x.StudentAttendanceStatu).ToList();
        }

        public List<StaffAttandanceModel> SearchStaffAttendnaceModel(DateTime FromDate, DateTime ToDate, int staffId)
        {
            var query = from att in dbContext.StaffAttandances
                        join staff in dbContext.Staffs on att.StaffId equals staff.StaffId
                        join status in dbContext.StudentAttendanceStatus on att.Status equals status.Id
                        where (EntityFunctions.TruncateTime(att.Date) >= FromDate.Date
                                && EntityFunctions.TruncateTime(att.Date) <= ToDate.Date)
                                && staff.IsLeft == false
                                && (staffId == 0 || staffId == staff.StaffId)
                        select new StaffAttandanceModel
                        {
                            CodeName = status.CodeName,
                            CreatedOn = att.CreatedOn, 
                            Date = att.Date, 
                            FatherName = staff.FatherName, 
                            Id = att.Id, 
                            Name = staff.Name,
                            OutTime = att.OutTime,
                            StaffId = staff.StaffId,
                            Status = status.Id,
                            Time = att.Time
                        };
            return query.ToList();
        }

        public List<StaffAdvanceModel> SearchStaffAdvance(int staffId, DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            var query = from staffAdvances in dbContext.StaffAdvances
                        join staff in dbContext.Staffs on staffAdvances.StaffId equals staff.StaffId
                        join designation in dbContext.Designations on staff.DesignationId equals designation.Id
                        where (staffId == 0 || staff.StaffId == staffId)
                        && staff.IsLeft == false
                        && (EntityFunctions.TruncateTime(staffAdvances.CreatedOn) >= fromDate.Date 
                        && EntityFunctions.TruncateTime(staffAdvances.CreatedOn) <= toDate.Date)
                        //&& (staffAdvances.CreatedOn >= toDate && staffAdvances.CreatedOn <= toDate)
                        select new StaffAdvanceModel
                        {
                            Id = staffAdvances.Id,
                            StaffId = staff.StaffId,
                            StaffName = staff.Name,
                            Date = (DateTime)staffAdvances.CreatedOn,
                            Amount = (int)staffAdvances.AdvanceAmount,
                            Remarks = staffAdvances.Remarks
                        };

            return query.ToList();
        }

        public List<StaffMiscWithdrawModel> SearchStaffMiscWithdraw(int staffId, DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            var query = from staffAdvances in dbContext.StaffMiscWithdraws
                        join staff in dbContext.Staffs on staffAdvances.StaffId equals staff.StaffId
                        join designation in dbContext.Designations on staff.DesignationId equals designation.Id
                        where (staffId == 0 || staff.StaffId == staffId)
                        && staff.IsLeft == false
                        && (EntityFunctions.TruncateTime(staffAdvances.CreatedOn) >= fromDate.Date
                        && EntityFunctions.TruncateTime(staffAdvances.CreatedOn) <= toDate.Date)
                        //&& (staffAdvances.CreatedOn >= toDate && staffAdvances.CreatedOn <= toDate)
                        select new StaffMiscWithdrawModel
                        {
                            Id = staffAdvances.Id,
                            StaffId = staff.StaffId,
                            StaffName = staff.Name,
                            Date = (DateTime)staffAdvances.CreatedOn,
                            Amount = (int)staffAdvances.WithdrawAmount,
                            Remarks = staffAdvances.Remarks
                        };

            return query.ToList();
        }

        public List<StaffSalaryModel> SearchStaffSalary(int staffId, DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1);
            var query = from salary in dbContext.StaffSalaries
                        join staff in dbContext.Staffs on salary.StaffId equals staff.StaffId
                        join designation in dbContext.Designations on staff.DesignationId equals designation.Id
                        where (staffId == 0 || staff.StaffId == staffId)
                        && staff.IsLeft == false
                        && (EntityFunctions.TruncateTime(salary.PaidDate) >= fromDate.Date
                        && EntityFunctions.TruncateTime(salary.PaidDate) <= toDate.Date)
                        //&& (staffAdvances.CreatedOn >= toDate && staffAdvances.CreatedOn <= toDate)
                        select new StaffSalaryModel
                        {
                            Id = salary.SalaryId,
                            StaffId = staff.StaffId,
                            Name = staff.Name,
                            PaidDate = (DateTime)salary.PaidDate,
                            AdvanceAdjustment = salary.AdvanceAdjustment == null ? 0 : salary.AdvanceAdjustment,
                            Bonus = salary.Bonus == null ? 0 : salary.Bonus,
                            Month = salary.ForMonth,
                            Salary = salary.SalaryAmount,
                            PaidAmount = salary.PaidAmount,
                            SalaryDeductions = salary.Deduction
                        };

            return query.ToList();
        }

        public List<ClassModel> GetStaffClass(int StaffId)
        {
            var sessionYear = GetCurrentSession().Id.ToString();
            var query = from subject in dbContext.SessionSubjects
                        join clsec in dbContext.ClassSections on subject.ClassSectionId equals clsec.ClassSectionId
                        join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                        where subject.TeacherId == StaffId && subject.SessionYear == sessionYear
                        select new ClassModel
                        {
                            ClassId = cls.Id,
                            ClassName = cls.Name
                        };

            return query.ToList();
        }


        public List<SectionModel> GetStaffClassSection(int StaffId, int ClassId)
        {
            var sessionYear = GetCurrentSession().Id.ToString();
            var query = from subject in dbContext.SessionSubjects
                        join clsec in dbContext.ClassSections on subject.ClassSectionId equals clsec.ClassSectionId
                        join cls in dbContext.Classes on clsec.ClassId equals cls.Id
                        join sec in dbContext.Sections on clsec.SectionId equals sec.Id
                        where subject.TeacherId == StaffId && subject.SessionYear == sessionYear
                        select new SectionModel
                        {
                            SectionId = sec.Id,
                            SectionName = sec.Name
                        };

            return query.ToList();
        }

        private BioMatrixCount GetBioMatrixCountObj()
        {
            return dbContext.BioMatrixCounts.FirstOrDefault();
        }

        public void UpdateBioMatrixLogCount(int count)
        {
            var logCount = GetBioMatrixCountObj();
            logCount.LogsCount = count;
            dbContext.Entry(logCount).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int GetBioMatrixLogCount()
        {
            var logCount = GetBioMatrixCountObj();
            return (int)logCount.LogsCount;
        }

        public void AddStaffHolidays(StaffHoliday holiday)
        {
            dbContext.StaffHolidays.Add(holiday);
            dbContext.SaveChanges();
        }

        public StaffHoliday GetStaffHolidayById(int holidayId)
        {
            return dbContext.StaffHolidays.Find(holidayId);
        }

        public StaffHoliday GetStaffHolidayByDate(DateTime date, int branchId)
        {
            return dbContext.StaffHolidays.Where(x => EntityFunctions.TruncateTime(x.Date) == date.Date
            && x.BranchId == branchId).FirstOrDefault();
        }

        public void DeleteStaffHoliday(StaffHoliday holiday)
        {
            if (holiday != null)
            {
                dbContext.StaffHolidays.Remove(holiday);
                dbContext.SaveChanges();
            }
        }

        public List<StaffHoliday> GetAllStaffHoliday()
        {
            return dbContext.StaffHolidays.ToList();
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
