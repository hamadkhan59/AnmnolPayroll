using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using SMS_DAL.ViewModel;
using System.Net;
using System.IO;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class FeePlanRepositoryImp : IFeePlanRepository
    {
        private SC_WEBEntities2 dbContext1;

        IStudentRepository studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        public FeePlanRepositoryImp(SC_WEBEntities2 context)
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

        #region FEE_HEAD

        public int AddFeeHead(FeeHead head)
        {
            int result = -1;
            if (head != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.FeeHeads.Add(head);
                dbContext.SaveChanges();
                result = head.Id;
            }
            return result;
        }

        public FeeHead GetFeeHeadByName(string name, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.FeeHeads.Where(x => x.Name == name && x.BranchId == branchId).FirstOrDefault();
        }

        public FeeHead GetFeeHeadByNameAndId(string name, int headId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.FeeHeads.Where(x => x.Name == name && x.Id != headId && x.BranchId == branchId).FirstOrDefault();
        }

        public void UpdateFeeHead(FeeHead head)
        {
            if (head != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(head).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public FeeHead GetFeeHeadById(int headId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.FeeHeads.Find(headId);
        }

        public void DeleteFeeHead(FeeHead head)
        {
            if (head != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.FeeHeads.Remove(head);
                dbContext.SaveChanges();
            }
        }

        public List<FeeHead> GetAllFeeHeads(int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.FeeHeads.Where(x => x.BranchId == branchId).ToList();
        }

        public List<FeeHead> GetAllFeeHeads()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.FeeHeads.ToList();
        }

		public List<FeeModel> GetFeeStatus(int sid, DateTime dt, DateTime curDate)
        {
            if (dt == null)
            {
                dt = DateTime.Now;
            }
            if (curDate == null)
            {
                curDate = DateTime.Now;
            }
            Student student = studentRepo.GetStudentById(sid);
            dbContext.Configuration.LazyLoadingEnabled = false;
            List<FeeModel> list = (from ic in dbContext.IssuedChallans
                                 join id in dbContext.ChallanStudentDetails on ic.ChallanToStdId equals id.Id
                                 where id.StdId == student.id && ic.PaidDate >= dt && ic.PaidDate <= curDate
                                 select new FeeModel
                                 {
                                     date = ic.PaidDate,
                                     amount = ic.Amount,
                                     month = ic.ForMonth,
                                     name = student.Name
                                 }).ToList();
            return list;
        }

        #endregion

        #region FEE_CHALAN

        public int AddChallan(Challan chalan)
        {
            int result = -1;
            try
            {
                if (chalan != null)
                {
                    dbContext.Configuration.LazyLoadingEnabled = false;
                    dbContext.Challans.Add(chalan);
                    dbContext.SaveChanges();
                    result = chalan.Id;
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

        public void ProceedCurrentMonthAttendance(int Status, DateTime AttandanceDate)
        {
            var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_PrceedCurrentDayAttendance]
		                                                        @status = " + Status + ","
                                                                + "@attendanceDate = '" + getDate(AttandanceDate) + "'";

            dbContext.Database.SqlQuery<List<string>>(spQuery).FirstOrDefault();
        }

        private string getDate(DateTime date)
        {
            return date.Year + "-" + date.Month + "-" + date.Day;
        }

        public string GetPreviousIssuedMonth(int issueChallanId)
        {
            string month = "";
            var challan = dbContext.IssuedChallans.Where(x => x.Id == issueChallanId).FirstOrDefault();
            int challanToStudentId = (int) challan.ChallanToStdId;

            var issuedMonth = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == challanToStudentId && x.IssuedFlag == 1).OrderByDescending(x => x.Id).FirstOrDefault();
            if (issuedMonth != null)
            {
                month = issuedMonth.ForMonth;
            }

            return month;
        }

        public string GetPreviousMonth(string month)
        {
            int monthId = dbContext.Months.Where(x => x.Month1 == month).FirstOrDefault().Id - 1;
            monthId = monthId == 0 ? 12 : monthId;
            return dbContext.Months.Where(x => x.Id == monthId).FirstOrDefault().Month1;
        }

        public Challan GetChallanByName(string name, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Challans.Where(x => x.Name == name && x.BranchId == branchId).FirstOrDefault();
        }

        public Challan GetChallanByNameAndId(string name, int challanId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Challans.Where(x => x.Name == name && x.Id != challanId && x.BranchId == branchId).FirstOrDefault();
        }

        public void UpdateChallan(Challan chalan)
        {
            if (chalan != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(chalan).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public Challan GetChallanById(int chalanId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Challans.Find(chalanId);
        }

        public void DeleteChallan(Challan chalan)
        {
            if (chalan != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Challans.Remove(chalan);
                dbContext.SaveChanges();
            }
        }

        public List<Challan> GetAllChallan()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Challans.ToList();
        }

        public List<Challan> GetAllChallanByClassId(int classId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Challans.Where(x => x.ClassId == classId).ToList();
        }

        #endregion

        #region CHALLAN_DETAIL

        public int AddChallanDetail(ChallanFeeHeadDetail chalanDetail)
        {
            int result = -1;
            if (chalanDetail != null)
            {
                dbContext.ChallanFeeHeadDetails.Add(chalanDetail);
                dbContext.SaveChanges();
                result = chalanDetail.Id;
            }
            return result;
        }

        public List<ChallanDetailViewModel> GetChallDetailByChallanId(int challanId)
        {
            var detail = from dt in dbContext.ChallanFeeHeadDetails
                         join head in dbContext.FeeHeads on dt.HeadId equals head.Id
                         where dt.ChallanId == challanId
                         select new ChallanDetailViewModel
                         {
                             Id = dt.Id,
                             HeadId = dt.HeadId,
                             ChallanId = dt.ChallanId,
                             Amount = (int)dt.Amount,
                             StandardAmount = (int)head.Amount,
                             Name = head.Name
                         };

            return detail.ToList();
            //return dbContext.ChallanFeeHeadDetails.Where(c => c.ChallanId == challanId).Include(c => c.Challan).Include(c => c.FeeHead).ToList();
        }

        public List<ChallanDetailViewModel> GetChallDetailByClassIdId(int classId)
        {
            var detail = from dt in dbContext.ChallanFeeHeadDetails
                         join head in dbContext.FeeHeads on dt.HeadId equals head.Id
                         select new ChallanDetailViewModel
                         {
                             Id = dt.Id,
                             HeadId = dt.HeadId,
                             ChallanId = dt.ChallanId,
                             Amount = (int)dt.Amount,
                             StandardAmount = (int)head.Amount,
                             Name = head.Name
                         };

            return detail.ToList();
            //return dbContext.ChallanFeeHeadDetails.Where(c => c.Challan.ClassId == classId).Include(c => c.Challan).Include(c => c.FeeHead).ToList();
        }

        public ChallanFeeHeadDetail GetChallDetailById(int Id)
        {
            return dbContext.ChallanFeeHeadDetails.Find(Id);
        }

        public void UpdateChallanDetail(ChallanFeeHeadDetail chalanDetail)
        {
            if (chalanDetail != null)
            {
                ChallanFeeHeadDetail sessionObject = dbContext.ChallanFeeHeadDetails.Find(chalanDetail.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(chalanDetail);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region HISTORY
        public int AddArrearHistory(ArreartHistory history)
        {
            int result = -1;
            if (history != null)
            {
                dbContext.ArreartHistories.Add(history);
                dbContext.SaveChanges();
                result = history.ID;
            }
            return result;
        }

        public void DeleteArrearHistory(ArreartHistory history)
        {
            if (history != null)
            {
                dbContext.ArreartHistories.Remove(history);
                dbContext.SaveChanges();
            }
        }

        public void UpdateArrearHistory(ArreartHistory history)
        {
            if (history != null)
            {
                ArreartHistory sessionObject = dbContext.ArreartHistories.Find(history.ID);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(history);
                dbContext.SaveChanges();
            }
        }

        public ArreartHistory GetArrearHistoryById(int Id)
        {
            return dbContext.ArreartHistories.Find(Id);
        }

        public List<ArreartHistory> SearchArrearHistory(int FeeBalanceId = 0, int FeeHeadId = 0)
        {
            return dbContext.ArreartHistories.Where(x => (x.FeeBalanceId == FeeBalanceId || x.FeeBalanceId == 0)
                && (x.FeeHeadId == FeeHeadId || x.FeeHeadId == 0)).ToList();
        }

        public int AddStudentExtrachargesHistory(ExtraChargesHistory history)
        {
            int result = -1;
            if (history != null)
            {
                dbContext.ExtraChargesHistories.Add(history);
                dbContext.SaveChanges();
                result = history.ID;
            }
            return result;
        }


        public void DeleteExtraChargesHistory(ExtraChargesHistory history)
        {
            if (history != null)
            {
                dbContext.ExtraChargesHistories.Remove(history);
                dbContext.SaveChanges();
            }
        }

        public void UpdateExtraChargesHistory(ExtraChargesHistory history)
        {
            if (history != null)
            {
                ExtraChargesHistory sessionObject = dbContext.ExtraChargesHistories.Find(history.ID);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(history);
                dbContext.SaveChanges();
            }
        }

        public ExtraChargesHistory GetExtraChargesHistoryById(int Id)
        {
            return dbContext.ExtraChargesHistories.Find(Id);
        }

        public List<ExtraChargesHistory> SearchExtraChargesHistory(int StudentId = 0, int FeeHeadId = 0)
        {
            return dbContext.ExtraChargesHistories.Where(x => (x.StudentId == StudentId || x.StudentId == 0)
                && (x.FeeHeadId == FeeHeadId || x.FeeHeadId == 0)).ToList();
        }

        public List<ExtraChargesHistory> SearchExtraChargesHistoryForMonth(int StudentId, string currentMonth)
        {
            return dbContext.ExtraChargesHistories.Where(x => (x.StudentId == StudentId || x.StudentId == 0)
                && (x.Description.Contains(currentMonth))).ToList();
        }

        public int AddPaymentHistory(PaymentHistory history)
        {
            int result = -1;
            if (history != null)
            {
                dbContext.PaymentHistories.Add(history);
                dbContext.SaveChanges();
                result = history.ID;
            }
            return result;
        }

        public int getPaymentHistorySum(int feeBalanceId, DateTime fromDate, DateTime toDate)
        {
            int sum = 0;
            var list = dbContext.PaymentHistories.Where(x => x.FeeBalanceId == feeBalanceId && x.CreatedOn >= fromDate && x.CreatedOn <= toDate).ToList();
            if (list != null && list.Count() > 0)
                sum = (int) list.Sum(x => x.PayAmount);
            return sum;
        }

        public void DeletePaymentHistory(PaymentHistory history)
        {
            if (history != null)
            {
                dbContext.PaymentHistories.Remove(history);
                dbContext.SaveChanges();
            }
        }

        public void UpdatePaymentHistory(PaymentHistory history)
        {
            if (history != null)
            {
                PaymentHistory sessionObject = dbContext.PaymentHistories.Find(history.ID);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(history);
                dbContext.SaveChanges();
            }
        }

        public PaymentHistory GetPaymentHistoryById(int Id)
        {
            return dbContext.PaymentHistories.Find(Id);
        }

        public List<PaymentHistory> SearchPaymentHistory(int PaymentType = 0, int IssueChallanId = 0, int FeeBalanceId = 0, int FeeHeadId = 0)
        {
            return dbContext.PaymentHistories.Where(x => (x.FeeBalanceId == FeeBalanceId || FeeBalanceId == 0)
                && (x.FeeHeadId == FeeHeadId || FeeHeadId == 0) && (x.IssueChallanId == IssueChallanId || IssueChallanId == 0)
                && (x.PaymentType == PaymentType || PaymentType == 0)).ToList();
        }

        #endregion


        #region BankAccount

        public int AddBankAccount(BankAccount bankAccount)
        {
            int result = -1;
            if (bankAccount != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.BankAccounts.Add(bankAccount);
                dbContext.SaveChanges();
                result = bankAccount.Id;
            }
            return result;
        }

        public BankAccount GetBankAccountByAccountNo(string bankAccount, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.BankAccounts.Where(x => x.AccountNo == bankAccount && x.BranchId == branchId).FirstOrDefault();
        }

        public BankAccount GetBankAccountByAccountNoAndId(string bankAccount, int accountId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.BankAccounts.Where(x => x.AccountNo == bankAccount && x.Id != accountId && x.BranchId == branchId).FirstOrDefault();
        }

        public void UpdateBankAccount(BankAccount bankAccount)
        {
            if (bankAccount != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                BankAccount sessionObject = dbContext.BankAccounts.Find(bankAccount.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(bankAccount);
                dbContext.SaveChanges();
            }
        }

        public List<BankAccount> GetAllBankAccount()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.BankAccounts.ToList();
        }
        public BankAccount GetBankAccountById(int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.BankAccounts.Where(x => x.Id == id).FirstOrDefault();
        }

        public void DeleteBankAccount(BankAccount bankAccount)
        {
            if (bankAccount != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.BankAccounts.Remove(bankAccount);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region ISSUE_CHALAN_FUNCTIONS

        public int GetIssuedChallanCount(string forMonth, int branchId)
        {
            int count = dbContext.IssuedChallans.Where(x => x.ForMonth == forMonth && x.BranchId == branchId && x.IssuedFlag == 1).ToList().Count();
            return count;
        }

        public int GetIssueChallanCount(int classSectionId, string currentMonth, int branchId)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where (student.ClassSectionId == classSectionId || classSectionId == 0) &&
                             (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                             && student.LeavingStatus == 1
                             && issueChallan.IssuedFlag == 1
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 Chalan = chalan.Name,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)
                             };

            return callanList.Count();
        }

        public List<IssuedChallanViewModel> SearchIssueChallan(int classSectionId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where (student.ClassSectionId == classSectionId || classSectionId == 0) &&
                             (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
                               (ContactNo == null || student.Contact_1 == null || student.Contact_1.Contains(ContactNo)) &&
                             (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                             && student.LeavingStatus == 1
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 Chalan = chalan.Name,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)
                             };

            return callanList.ToList();
            //return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == x.ChallanStudentDetail.Id
            //    && (x.ChallanStudentDetail.Student.ClassSectionId == classSectionId
            //    && (rollNumber == null || x.ChallanStudentDetail.Student.RollNumber.Contains(rollNumber))
            //            && (name == null || x.ChallanStudentDetail.Student.Name.Contains(name))
            //            && (fatherCnic == null || x.ChallanStudentDetail.Student.FatherCNIC.Contains(fatherCnic))
            //            && (fatherName == null || x.ChallanStudentDetail.Student.FatherName.Contains(fatherName)))
            //            && x.ForMonth == currentMonth && x.BranchId == branchId).ToList();
        }

        public List<IssuedChallanViewModel> SearchIssueChallanByAdmissionNo(string AdmissionNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where student.AdmissionNo == AdmissionNo
                             && student.LeavingStatus == 1
                             && issueChallan.IssuedFlag == 1
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 Chalan = chalan.Name,
                                 ForMonth = issueChallan.ForMonth,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)
                             };

            IssuedChallanViewModel viewModel =  callanList.OrderByDescending(x => x.Id).FirstOrDefault();
            if (viewModel == null)
                viewModel = new IssuedChallanViewModel();
            List<IssuedChallanViewModel> list = new List<IssuedChallanViewModel>();
            list.Add(viewModel);
            return list;
        }

        public List<IssuedChallanViewModel> SearchClassIssueChallan(int classId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join classSec in dbContext.ClassSections on student.ClassSectionId equals classSec.ClassSectionId
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where (classSec.ClassId == classId || classId == 0) &&
                             (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
                               (ContactNo == null || student.Contact_1 == null || student.Contact_1.Contains(ContactNo)) &&
                             (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                             && student.LeavingStatus == 1
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 Chalan = chalan.Name,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)
                             };

            return callanList.Distinct().ToList();
            //return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == x.ChallanStudentDetail.Id
            //    && (x.ChallanStudentDetail.Student.ClassSectionId == classSectionId
            //    && (rollNumber == null || x.ChallanStudentDetail.Student.RollNumber.Contains(rollNumber))
            //            && (name == null || x.ChallanStudentDetail.Student.Name.Contains(name))
            //            && (fatherCnic == null || x.ChallanStudentDetail.Student.FatherCNIC.Contains(fatherCnic))
            //            && (fatherName == null || x.ChallanStudentDetail.Student.FatherName.Contains(fatherName)))
            //            && x.ForMonth == currentMonth && x.BranchId == branchId).ToList();
        }

        public List<IssuedChallanViewModel> SearchChallanByStatus(int branchId, int classId, int sectionId, int feeStatus, DateTime fromDate, DateTime to)
        {
            var pending = FeeStatus.Pending.GetHashCode();
            var unpaid = FeeStatus.Unpaid.GetHashCode();
            var paid = FeeStatus.Paid.GetHashCode();
            var callanList = dbContext.IssuedChallans.Where(issueChallan => issueChallan.ChallanStudentDetail.Student.ClassSection.ClassId == classId
                            && issueChallan.ChallanStudentDetail.Student.ClassSection.SectionId == sectionId
                            && issueChallan.ChallanStudentDetail.Student.LeavingStatus == 1
                            && (feeStatus == pending ? issueChallan.IssuedFlag == 0 : true)
                            && (feeStatus == unpaid ? issueChallan.PaidFlag == 0 && issueChallan.IssuedFlag == 1 : true)
                            && (feeStatus == paid  ? issueChallan.PaidFlag == 1 : true)
                            && (feeStatus == paid ? (issueChallan.PaidDate >= fromDate && issueChallan.PaidDate <= to) : (issueChallan.IssueDate >= fromDate && issueChallan.IssueDate <= to))
                            && (issueChallan.BranchId == branchId)).GroupBy(x => x.ChallanStudentDetail.Student.id).Select(x => 
                                new  IssuedChallanViewModel
                             {
                                 studentId = x.Key,
                                 RollNumber = x.FirstOrDefault().ChallanStudentDetail.Student.RollNumber,
                                 Fathername = x.FirstOrDefault().ChallanStudentDetail.Student.FatherName,
                                 Name = x.FirstOrDefault().ChallanStudentDetail.Student.Name,
                                 Class = x.FirstOrDefault().ChallanStudentDetail.Student.ClassSection.Class.Name,
                                 Section = x.FirstOrDefault().ChallanStudentDetail.Student.ClassSection.Section.Name,
                                 Contact_1 = x.FirstOrDefault().ChallanStudentDetail.Student.Contact_1,
                                 //IsPaid = issueChallan.PaidFlag == 1 ? "Yes" : "No",
                                 Amount = (int)x.Sum(n => n.ChalanAmount),
                                 PaidAmount = (int)x.Sum(n => n.Amount),
                                 Fine = (int)x.Sum(n => n.Fine)
                             });

            return callanList.ToList();
        }

        public List<IssuedChallanViewModel> SearchPaidChallan(int classSectionId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             from annCh in ps1.DefaultIfEmpty()
                             where (student.ClassSectionId == classSectionId) &&
                             (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
                               (ContactNo == null || student.Contact_1 == null || student.Contact_1.Contains(ContactNo)) &&
                             (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                             && issueChallan.IssuedFlag == 1
                             && student.LeavingStatus == 1
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.PaidFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine),
                                 PaidDate = (DateTime)issueChallan.PaidDate,
                                 IssuedDate = (DateTime)issueChallan.IssueDate,
                                 Chalan = chalan.Name
                             };

            return callanList.ToList();
            //return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == x.ChallanStudentDetail.Id
            //    && (x.ChallanStudentDetail.Student.ClassSectionId == classSectionId
            //    && (rollNumber == null || x.ChallanStudentDetail.Student.RollNumber.Contains(rollNumber))
            //            && (name == null || x.ChallanStudentDetail.Student.Name.Contains(name))
            //            && (fatherCnic == null || x.ChallanStudentDetail.Student.FatherCNIC.Contains(fatherCnic))
            //            && (fatherName == null || x.ChallanStudentDetail.Student.FatherName.Contains(fatherName)))
            //            && x.ForMonth == currentMonth && x.BranchId == branchId).ToList();
        }

        public List<IssuedChallanViewModel> SearchClassPaidChallan(int classId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join classSec in dbContext.ClassSections on student.ClassSectionId equals classSec.ClassSectionId
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             from annCh in ps1.DefaultIfEmpty()
                             where (classSec.ClassId == classId || classId == 0) &&
                             (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
                               (ContactNo == null || student.Contact_1 == null || student.Contact_1.Contains(ContactNo)) &&
                             (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                             && issueChallan.IssuedFlag == 1
                             && student.LeavingStatus == 1
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.PaidFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine),
                                 PaidDate = (DateTime)issueChallan.PaidDate,
                                 IssuedDate = (DateTime)issueChallan.IssueDate,
                                 Chalan = chalan.Name
                             };

            return callanList.Distinct().ToList();
            //return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == x.ChallanStudentDetail.Id
            //    && (x.ChallanStudentDetail.Student.ClassSectionId == classSectionId
            //    && (rollNumber == null || x.ChallanStudentDetail.Student.RollNumber.Contains(rollNumber))
            //            && (name == null || x.ChallanStudentDetail.Student.Name.Contains(name))
            //            && (fatherCnic == null || x.ChallanStudentDetail.Student.FatherCNIC.Contains(fatherCnic))
            //            && (fatherName == null || x.ChallanStudentDetail.Student.FatherName.Contains(fatherName)))
            //            && x.ForMonth == currentMonth && x.BranchId == branchId).ToList();
        }

        public IssuedChallanViewModel SearchFastPaidChallan(string rollNumber, string name, string fatherName, string fatherCnic, int branchId, string AdmissionNo, int chalanNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join clases in dbContext.ClassSections on student.ClassSectionId equals clases.ClassSectionId
                             join clas in dbContext.Classes on clases.ClassId equals clas.Id
                             join sec in dbContext.Sections on clases.SectionId equals sec.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             where (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
                             (issueChallan.Id == chalanNo || chalanNo == 0) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                             && issueChallan.IssuedFlag == 1
                             && student.LeavingStatus == 1
                             orderby issueChallan.Id descending
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 ChallanId = (int)issueChallan.Id,
                                 studentId = student.id,
                                 StudentChallanId = (int)issueChallan.ChallanToStdId,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 ForMonth = issueChallan.ForMonth,
                                 Class = clas.Name,
                                 Section = sec.Name,
                                 Name = student.Name,
                                 IsLcm = (bool)(issueChallan.IsLcm == null ? false : issueChallan.IsLcm),
                                 Contact_1 = student.Contact_1,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.PaidFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine),
                                 PaidDate = (DateTime)issueChallan.PaidDate,
                                 Chalan = chalan.Name
                             };

            return callanList.FirstOrDefault();
        }

        public List<IssuedChallanViewModel> GetStudentSixMonthPaymentDetail(int studentChallanId, int issueChallanId)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join clases in dbContext.ClassSections on student.ClassSectionId equals clases.ClassSectionId
                             join clas in dbContext.Classes on clases.ClassId equals clas.Id
                             join sec in dbContext.Sections on clases.SectionId equals sec.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             join chalan in dbContext.Challans on studentDetail.ChallanId equals chalan.Id
                             where issueChallan.ChallanToStdId == studentChallanId && issueChallan.Id != issueChallanId
                             && student.LeavingStatus == 1
                             orderby issueChallan.Id descending
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 ChallanId = (int)issueChallan.Id,
                                 studentId = student.id,
                                 StudentChallanId = (int)issueChallan.ChallanToStdId,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 ForMonth = issueChallan.ForMonth,
                                 Class = clas.Name,
                                 Section = sec.Name,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.PaidFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine),
                                 PaidDate = (DateTime)issueChallan.PaidDate,
                                 Chalan = chalan.Name
                             };

            return callanList.Take(6).ToList();
        }

        public List<IssuedChallanViewModel> SearchFeeArrears(int classSectionId, string rollNumber, string name, string fatherName, string fatherCnic)
        {
            var orderList = (from students in dbContext.Students
                             join feeBal in dbContext.FeeBalances on students.id equals feeBal.StudentId
                             into ps
                             from feeBal in ps.DefaultIfEmpty()
                             where (classSectionId == 0 || students.ClassSectionId == classSectionId)
                             && (rollNumber == "" || students.RollNumber.Contains(rollNumber)) &&
                             (fatherCnic == null || students.FatherCNIC == null || students.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || students.Name == null || students.Name.Contains(name)) &&
                              (fatherName == null || students.FatherName == null || students.FatherName.Contains(fatherName))
                             && students.LeavingStatus == 1
                             select new IssuedChallanViewModel
                             {
                                 studentId = students.id,
                                 RollNumber = students.RollNumber,
                                 Name = students.Name,
                                 Fathername = students.FatherName,
                                 Class = students.ClassSection.Class.Name,
                                 Section = students.ClassSection.Section.Name,
                                 FeeBalance = (feeBal.Balance == null ? 0 : feeBal.Balance)
                             }).Distinct().ToList();
            return orderList;
        }

        public List<IssuedChallanViewModel> SearchFeeArrearsByAdmissionNo(string AdmissionNo)
        {
            var orderList = (from students in dbContext.Students
                             join feeBal in dbContext.FeeBalances on students.id equals feeBal.StudentId
                             into ps
                             from feeBal in ps.DefaultIfEmpty()
                             where students.SrNo.Contains(AdmissionNo)
                             && students.LeavingStatus == 1
                             select new IssuedChallanViewModel
                             {
                                 studentId = students.id,
                                 RollNumber = students.RollNumber,
                                 Name = students.Name,
                                 Fathername = students.FatherName,
                                 Class = students.ClassSection.Class.Name,
                                 Section = students.ClassSection.Section.Name,
                                 FeeBalance = (feeBal.Balance == null ? 0 : feeBal.Balance)
                             }).Distinct().ToList();
            return orderList;
        }

        public void SaveFeeArrearDetail(FeeArrearsDetail detail)
        {
            dbContext.FeeArrearsDetails.Add(detail);
            dbContext.SaveChanges();
            UpdateFeeBalance((int)detail.FeeBalanceId);
        }

        public void UpdateFeeArrearDetail(FeeArrearsDetail detail)
        {
            FeeArrearsDetail sessionObject = dbContext.FeeArrearsDetails.Find(detail.ID);
            dbContext.Entry(sessionObject).CurrentValues.SetValues(detail);
            dbContext.SaveChanges();

            UpdateFeeBalance((int)detail.FeeBalanceId);
        }

        public void UpdateFeeBalance(int feeBalanceId)
        {
            int arrearAmount = (int) dbContext.FeeArrearsDetails.Where(x => x.FeeBalanceId == feeBalanceId).Sum(x => x.HeadAmount);

            FeeBalance balance = dbContext.FeeBalances.Where(x => x.Id == feeBalanceId).FirstOrDefault();
            balance.Balance = arrearAmount;

            dbContext.Entry(balance).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public MonthDetailModel GetLastMonthSummary()
        {
            var issuedChalan = dbContext.IssuedChallans.OrderByDescending(x => x.Id).FirstOrDefault();

            MonthDetailModel model = new MonthDetailModel();
            if (issuedChalan != null)
            {
                string forMonth = issuedChalan.ForMonth;

                var query = from chalan in dbContext.IssuedChallans
                            where chalan.ForMonth == forMonth
                            select chalan;

                model.Month = forMonth;
                int received = 0;
                var paidList = query.Where(x => x.PaidFlag == 1);
                if(paidList != null && paidList.Count() > 0)
                    received = (int)query.Where(x => x.PaidFlag == 1).Sum(x => x.Amount == null ? 0 : x.Amount);
                model.TotalAmount = (int)query.Sum(x => x.ChalanAmount);
                model.TotalReceived = received;
                model.TotalPending = model.TotalAmount - model.TotalReceived;
            }
            else
            {
                model.Month = "Fee System Start";
                model.TotalReceived = 0;
                model.TotalAmount = 0;
                model.TotalPending = 0;
            }
            return model;
        }

        public IssuedChallan GetPreviousIssuedChallan(int challanStudentId, int issueChallanId)
        {
            return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == challanStudentId && x.Id != issueChallanId && x.IssuedFlag == 1).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public List<FeeArrearViewModel> GetLastMonthsUnPaidFee(int studentChallanId, int challanId)
        {
            var issuedChallan = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == studentChallanId && x.Id != challanId && x.IssuedFlag == 1 && (x.PaidFlag == 0 || x.Amount != x.ChalanAmount)).OrderBy(x => x.Id).ToList();
            List<FeeArrearViewModel> biglist = new List<FeeArrearViewModel>();
            List<FeeArrearViewModel> list = new List<FeeArrearViewModel>();

            foreach (var chln in issuedChallan)
            {
                list = (from challanDetail in dbContext.IssueChalanDetails
                        where challanDetail.IssueChallanId == chln.Id
                        //&& challanDetail.Type == 1
                        select new FeeArrearViewModel
                        {
                            IssueChallanDetailId = challanDetail.ID,
                            FeeHeadId = challanDetail.FeeHeadId,
                            FeeBalanceId = 0,
                            ArrearAmount = challanDetail.TotalAmount - (challanDetail.PayAmount == null ? 0 : challanDetail.PayAmount) - (challanDetail.Discount == null ? 0 : challanDetail.Discount),
                            HeadName = challanDetail.FeeHead.Name
                        }).Distinct().ToList();

                foreach (var obj in list)
                {
                    obj.HeadName += "-" + chln.ForMonth;
                }

                biglist.AddRange(list.Where(x => x.ArrearAmount > 0).ToList());
            }

            return biglist;
        }

        public List<FeeArrearViewModel> GetLastMonthUnPaidFee(int studentChallanId, int challanId)
        {
            var issuedChallan = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == studentChallanId && x.Id != challanId && x.IssuedFlag == 1).OrderByDescending(x => x.Id).FirstOrDefault();
            List<FeeArrearViewModel> list = new List<FeeArrearViewModel>();
            if (issuedChallan != null && issuedChallan.PaidFlag == 0)
            {

                list = (from challanDetail in dbContext.IssueChalanDetails
                        where challanDetail.IssueChallanId == issuedChallan.Id
                        && challanDetail.Type == 1
                        select new FeeArrearViewModel
                        {
                            FeeHeadId = challanDetail.FeeHeadId,
                            FeeBalanceId = 0,
                            ArrearAmount = challanDetail.TotalAmount,
                            HeadName = challanDetail.FeeHead.Name
                        }).Distinct().ToList();

                foreach (var obj in list)
                {
                    obj.HeadName += "-" + issuedChallan.ForMonth;
                }

            }

            return list;
        }

        public void setArrearDiscount(int feeHeadId, int discount, int studentId)
        {
            var studentChallan = dbContext.ChallanStudentDetails.Where(x => x.StdId == studentId).FirstOrDefault();
            var query = from challanDetail in dbContext.IssueChalanDetails
                        join IssuedChallan in dbContext.IssuedChallans on challanDetail.IssueChallanId equals IssuedChallan.Id
                        where IssuedChallan.ChallanToStdId == studentChallan.Id
                        && challanDetail.TotalAmount != ((int)(challanDetail.PayAmount == null ? 0 : challanDetail.PayAmount) 
                        + (int)(challanDetail.Discount == null ? 0 : challanDetail.Discount))
                        && challanDetail.FeeHeadId == feeHeadId && (challanDetail.Type == 2 || challanDetail.Type == 1)
                        select challanDetail;
            var challanDetailList = query.OrderBy(x => x.ID).OrderByDescending(x => x.Type).ToList();
            foreach (var detail in challanDetailList)
            {
                if (discount > 0)
                {
                    detail.Discount = detail.Discount == null ? 0 : detail.Discount;
                    int unpaid = (int)detail.TotalAmount - (int)(detail.PayAmount == null ? 0 : detail.PayAmount);
                    if (unpaid - discount > 0)
                        detail.Discount += discount;
                    else
                    {
                        detail.Discount += unpaid;
                    }
                    discount -= unpaid;
                    dbContext.Entry(detail).State = EntityState.Modified;
                }
            }

            dbContext.SaveChanges();
        }

        public FeeArrearsDetail GetFeeArrearDetail(int feeBalanceId, int headId)
        {
            FeeArrearsDetail sessionObject = dbContext.FeeArrearsDetails.Where(x => x.FeeBalanceId == feeBalanceId && x.FeeHeadId == headId).FirstOrDefault();
            return sessionObject;
        }

        public List<FeeArrearViewModel> GetStudentArrearDetail(int studentId)
        {
            var arrearDetail = (from feeArreareDetail in dbContext.FeeArrearsDetails
                                join feeBalanace in dbContext.FeeBalances on feeArreareDetail.FeeBalanceId equals feeBalanace.Id
                                where feeBalanace.StudentId == studentId
                                select new FeeArrearViewModel
                                {
                                    FeeHeadId = feeArreareDetail.FeeHeadId,
                                    FeeBalanceId = feeArreareDetail.FeeBalanceId,
                                    ArrearAmount = feeArreareDetail.HeadAmount,
                                    HeadName = feeArreareDetail.FeeHead.Name
                                }).Distinct().ToList();

            var feeHeadList = dbContext.FeeHeads.ToList();
            
            if (arrearDetail.Count == 0)
            {
                arrearDetail = (from feeHead in dbContext.FeeHeads
                                select new FeeArrearViewModel { FeeHeadId = feeHead.Id, HeadName = feeHead.Name, ArrearAmount = 0 }).Distinct().ToList();
            }

            if (arrearDetail.Count != feeHeadList.Count())
            {
                foreach (var head in feeHeadList)
                {
                    var headTemp = arrearDetail.Where(x => x.FeeHeadId == head.Id).FirstOrDefault();
                    if (headTemp == null)
                    {
                        FeeArrearsDetail detail = new FeeArrearsDetail();
                        detail.FeeBalanceId = arrearDetail[0].FeeBalanceId;
                        detail.FeeHeadId = head.Id;
                        detail.HeadAmount = 0;

                        dbContext.FeeArrearsDetails.Add(detail);
                    }
                }

                arrearDetail = (from feeArreareDetail in dbContext.FeeArrearsDetails
                                join feeBalanace in dbContext.FeeBalances on feeArreareDetail.FeeBalanceId equals feeBalanace.Id
                                where feeBalanace.StudentId == studentId
                                select new FeeArrearViewModel
                                {
                                    FeeHeadId = feeArreareDetail.FeeHeadId,
                                    FeeBalanceId = feeArreareDetail.FeeBalanceId,
                                    ArrearAmount = feeArreareDetail.HeadAmount,
                                    HeadName = feeArreareDetail.FeeHead.Name
                                }).Distinct().ToList();

                dbContext.SaveChanges();
            }

            return arrearDetail;
        }

        public List<IssuedChallanDetailModel> GetPendingChallanDetailModel(int challanStudentId, string forMonth)
        {
            var detailQuery = from challan in dbContext.IssuedChallans
                              join detail in dbContext.IssueChalanDetails on challan.Id equals detail.IssueChallanId
                              join head in dbContext.FeeHeads on detail.FeeHeadId equals head.Id
                              where challan.ChallanToStdId == challanStudentId 
                              //&& (challan.Amount != challan.ChalanAmount || challan.Amount == null)
                              && challan.ForMonth != forMonth && ( detail.PayAmount != detail.TotalAmount || challan.PaidFlag == 0)
                              && (challan.IsLcm != true || challan.IsLcm == null)
                              select new IssuedChallanDetailModel
                              {
                                  IssueChallanDetailId = detail.ID,
                                  IssueChallanId = (int)challan.Id,
                                  ForMonth = challan.ForMonth,
                                  HeadName = head.Name,
                                  FeeHeadId = head.Id,
                                  HeadDetail = head.Name + " (" + challan.ForMonth + ")" + (detail.Type == 3 ? " Extra Charges" : ""),
                                  Discount = (int)(detail.Discount == null ? 0 : detail.Discount),
                                  TotalAmount = (int)(detail.TotalAmount == null ? 0 : detail.TotalAmount),
                                  PaidAmount = (int)(detail.PayAmount == null ? 0 : detail.PayAmount),
                                  PendingAmount = (int)(detail.TotalAmount == null ? 0 : detail.TotalAmount) - (int)(detail.PayAmount == null ? 0 : detail.PayAmount) - (int)(detail.Discount == null ? 0 : detail.Discount)
                              };
            List<IssuedChallanDetailModel> detailList = detailQuery.Distinct().ToList();

            return detailList;
        }

        public List<StudentExtraChargesViewModel> SearchStudentExtraCharges(string forMonth, int classId, int sectionId, string rollNo, int feeHeadId, int Amount)
        {
            var chargesList = (from extraCharges in dbContext.StudentExtraCharges
                               join feeHead in dbContext.FeeHeads on extraCharges.FeeHeadId equals feeHead.Id
                               join clas in dbContext.Classes on extraCharges.ClassId equals clas.Id
                               into ps
                               from clas in ps.DefaultIfEmpty()
                               join section in dbContext.Sections on extraCharges.SectionId equals section.Id
                               into ps1
                               from section in ps1.DefaultIfEmpty()
                               where (forMonth == "" || extraCharges.ForMonth == forMonth)
                               && (classId == 0 || extraCharges.ClassId == classId)
                               && (sectionId == 0 || extraCharges.SectionId == sectionId)
                               && (rollNo == "-1" || extraCharges.RollNumber == rollNo)
                               && (Amount == 0 || extraCharges.HeadAmount == Amount)
                               && (feeHeadId == 0 || extraCharges.FeeHeadId == feeHeadId)
                               select new StudentExtraChargesViewModel
                               {
                                   Id = extraCharges.ID,
                                   HeadAmount = extraCharges.HeadAmount,
                                   HeadName = extraCharges.FeeHead.Name,
                                   Descroption = extraCharges.Description,
                                   Criteria = extraCharges.Criteria,
                                   ForMonth = extraCharges.ForMonth,
                                   CreatedOn = extraCharges.CreatedOn
                               }).Distinct().OrderByDescending(x => x.Id).ToList();

            return chargesList;
        }

        public void SaveStudentEtraCharges(StudentExtraCharge extraCharges)
        {
            dbContext.StudentExtraCharges.Add(extraCharges);
            dbContext.SaveChanges();

            var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Extra_Charges]
		                                                        @ClassId = " + extraCharges.ClassId + ","
                                                                + "@SectionId = " + extraCharges.SectionId + ","
                                                                + "@chargesAmount = " + extraCharges.HeadAmount + ","
                                                                + "@feeHeadId = " + extraCharges.FeeHeadId + ","
                                                                + "@rollNO = '" + extraCharges.RollNumber + "',"
                                                                + "@extraChargesId = " + extraCharges.ID + ","
                                                                + "@ForMonth = '" + extraCharges.ForMonth + "',"
                                                                + "@BranchId = " + extraCharges.BranchId;

            dbContext.Database.SqlQuery<List<string>>(spQuery).FirstOrDefault();
        }

        public void DeleteStudentEtraCharges(int extraChargesId, int branchId)
        {
            StudentExtraCharge extraCharges = dbContext.StudentExtraCharges.Find(extraChargesId);
            if (extraCharges != null)
            {
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Extra_Charges_Delete]
		                                                        @ClassId = " + extraCharges.ClassId + ","
                                                                    + "@SectionId = " + extraCharges.SectionId + ","
                                                                    + "@feeHeadId = " + extraCharges.FeeHeadId + ","
                                                                    + "@ForMonth = '" + extraCharges.ForMonth + "',"
                                                                    + "@rollNO = '" + extraCharges.RollNumber + "',"
                                                                    + "@BranchId = " + branchId;


                dbContext.Database.SqlQuery<List<string>>(spQuery).FirstOrDefault();

                dbContext.StudentExtraCharges.Remove(extraCharges);
                dbContext.SaveChanges();
            }
        }

        public StudentExtraCharge GetStudentExtraCharges(int extraChargesId, int branchId)
        {
            return dbContext.StudentExtraCharges.Where(x => x.ID == extraChargesId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<StudentExtraChargesDetail> GetStudentExtraChargesByStudent(int studentId)
        {
            return dbContext.StudentExtraChargesDetails.Where(x => x.StudentId == studentId).ToList();
        }

        public List<StudentExtraChargesDetail> GetStudentExtraChargesByStudent(int studentId, string forMonth)
        {
            return dbContext.StudentExtraChargesDetails.Where(x => x.StudentId == studentId && x.ForMonth == forMonth).ToList();
        }

        public List<IssueChalanDetail> GetIssueChallanDetail(int chalanId)
        {
            return dbContext.IssueChalanDetails.Where(x => x.IssueChallanId == chalanId).Include(x => x.FeeHead).OrderBy(x => x.Type).ToList();
        }

        public IssueChalanDetail GetIssuedChallanDetail(int chalanId, int feeHeadId, int type)
        {
            return dbContext.IssueChalanDetails.Where(x => x.IssueChallanId == chalanId && x.FeeHeadId == feeHeadId && x.Type == type).FirstOrDefault();
        }

        public IssueChalanDetail GetIssuedChallanDetailById(int issueChalanDetailId)
        {
            return dbContext.IssueChalanDetails.Where(x => x.ID == issueChalanDetailId).FirstOrDefault();
        }

        public void UpdateIssueChalanDetail(IssueChalanDetail detail)
        {
            dbContext.Entry(detail).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int GetChallanDiscount(int issueChallanId, int FeeHeadId)
        {
            int discount = 0;
            var discountList = dbContext.IssueChalanDetails.Where(x => x.IssueChallanId == issueChallanId && x.FeeHeadId == FeeHeadId).ToList();
            if (discountList != null && discountList.Count > 0)
            {
                discount = (int)discountList.Sum(x => x.Discount);
            }
            return discount;
        }

        public void UpdateIssueChalanPaidAmount(int issueChallanId, int payToType, int PayedTo)
        {
            IssuedChallan challan = dbContext.IssuedChallans.Find(issueChallanId);
            challan.Amount = dbContext.IssueChalanDetails.Where(x => x.IssueChallanId == issueChallanId).Sum(x => x.PayAmount);
            challan.PayedToType = payToType;
            challan.PayedTo = PayedTo;
            if (challan.ChalanAmount == challan.Amount)
                challan.PaidFlag = 1;
            dbContext.Entry(challan).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<StudentModel> SearchStudentForChallan(int classSectionId, string rollNumber, string name, string fatherName, string fatherCnic)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              where student.ClassSectionId == classSectionId &&
                              (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
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
                                  Contact_1 = student.Contact_1,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();

            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && (rollNumber == null || x.RollNumber.Contains(rollNumber))
            //            && (name == null || x.Name.Contains(name)) && (fatherName == null || x.FatherName.Contains(fatherName)))
            //            .Include(x => x.ClassSection).ToList();
        }

        public List<StudentModel> SearchClassStudentForChallan(int classd, string rollNumber, string name, string fatherName, string fatherCnic)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              where clSec.ClassId == classd &&
                              (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
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
                                  Contact_1 = student.Contact_1,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();

            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && (rollNumber == null || x.RollNumber.Contains(rollNumber))
            //            && (name == null || x.Name.Contains(name)) && (fatherName == null || x.FatherName.Contains(fatherName)))
            //            .Include(x => x.ClassSection).ToList();
        }

        public List<StudentModel> SearchNewStudentForChallan(int classSectionId, string rollNumber, string name, string fatherName, string fatherCnic, string forMonth)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              join stdetl in dbContext.ChallanStudentDetails on student.id equals stdetl.StdId
                              where student.ClassSectionId == classSectionId &&
                              (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
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
                                  Contact_1 = student.Contact_1,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus,
                                  ChallanToStdId = stdetl.Id
                              };

            var studentList = StudentList.OrderByDescending(x => x.Id).ToList();

            List<StudentModel> newStudents = new List<StudentModel>();

            foreach (var model in studentList)
            {
                var list = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == model.ChallanToStdId && x.ForMonth == forMonth).ToList();
                if (list != null && list.Count > 0)
                {
                    //break;
                }
                else
                {
                    newStudents.Add(model);
                }
            }

            return newStudents;
            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && (rollNumber == null || x.RollNumber.Contains(rollNumber))
            //            && (name == null || x.Name.Contains(name)) && (fatherName == null || x.FatherName.Contains(fatherName)))
            //            .Include(x => x.ClassSection).ToList();
        }


        public List<StudentModel> SearchClassNewStudentForChallan(int classId, string rollNumber, string name, string fatherName, string fatherCnic, string forMonth)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              join stdetl in dbContext.ChallanStudentDetails on student.id equals stdetl.StdId
                              where (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus == 1
                              && clSec.ClassId == classId
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  Contact_1 = student.Contact_1,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus,
                                  ChallanToStdId = stdetl.Id
                              };

            var studentList = StudentList.Distinct().OrderByDescending(x => x.Id).ToList();

            List<StudentModel> newStudents = new List<StudentModel>();

            foreach (var model in studentList)
            {
                var list = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == model.ChallanToStdId && x.ForMonth == forMonth).ToList();
                if (list != null && list.Count > 0)
                {
                    //break;
                }
                else
                {
                    newStudents.Add(model);
                }
            }

            return newStudents;
            //return dbContext.Students.Where(x => x.ClassSectionId == classSectionId && (rollNumber == null || x.RollNumber.Contains(rollNumber))
            //            && (name == null || x.Name.Contains(name)) && (fatherName == null || x.FatherName.Contains(fatherName)))
            //            .Include(x => x.ClassSection).ToList();
        }


        public List<StudentModel> SearchNewStudentForChallan(string rollNumber, string name, string fatherName, string fatherCnic)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              join stdetl in dbContext.ChallanStudentDetails on student.id equals stdetl.StdId
                              where (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
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
                                  Contact_1 = student.Contact_1,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus,
                                  ChallanToStdId = stdetl.Id,

                              };

            var studentList = StudentList.OrderByDescending(x => x.Id).ToList();

            List<StudentModel> newStudents = new List<StudentModel>();

            foreach (var model in studentList)
            {
                var list = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == model.ChallanToStdId).ToList();
                if (list != null && list.Count > 0)
                {
                    break;
                }
                else
                {
                    newStudents.Add(model);
                }
            }

            return newStudents;

            //return dbContext.Students.Where(x => (rollNumber == null || x.RollNumber.Contains(rollNumber))
            //            && (name == null || x.Name.Contains(name)) && (fatherName == null || x.FatherName.Contains(fatherName)))
            //            .Include(x => x.ClassSection).ToList();
        }


        public List<StudentModel> SearchStudentForChallan(string rollNumber, string name, string fatherName, string fatherCnic, int branchId)
        {

            var StudentList = from student in dbContext.Students
                              join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                              join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                              join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                              where (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (student.LeavingStatus == null || student.LeavingStatus != null)
                              && (student.ReasonId == null || student.ReasonId != null)
                              && student.LeavingStatus == 1
                              && student.BranchId == branchId
                              select new StudentModel
                              {
                                  Id = student.id,
                                  ClassName = clas.Name,
                                  SectionName = sec.Name,
                                  RollNumber = student.RollNumber,
                                  Name = student.Name,
                                  Contact_1 = student.Contact_1,
                                  FatherName = student.FatherName,
                                  StdImage = student.StdImage,
                                  LeavingStatus = (string)(student.LeavingStatu.StatusName == null ? "" : student.LeavingStatu.StatusName),
                                  LeavingReason = (string)(student.LeavingReason.LeavingReason1 == null ? "" : student.LeavingReason.LeavingReason1),
                                  AdmissionDate = (DateTime)(student.AdmissionDate == null ? DateTime.Now : student.AdmissionDate),
                                  LeavingDate = (DateTime)(student.LeavingDate == null ? DateTime.Now : student.LeavingDate),
                                  LeavingRemarks = student.LeavingRemarks,
                                  ClearDues = student.ClearDues,
                                  LeavingStatusCode = (int)student.LeavingStatus
                              };

            return StudentList.ToList();

            //return dbContext.Students.Where(x => (rollNumber == null || x.RollNumber.Contains(rollNumber))
            //            && (name == null || x.Name.Contains(name)) && (fatherName == null || x.FatherName.Contains(fatherName)))
            //            .Include(x => x.ClassSection).ToList();
        }

        public FeeBalance GetFeeBalanceByStudentId(int studentId)
        {
            var feeBalance = dbContext.FeeBalances.Where(x => x.StudentId == studentId).FirstOrDefault();

            if (feeBalance == null)
            {
                feeBalance = new FeeBalance();
                feeBalance.Advance = 0;
                feeBalance.Balance = 0;
                feeBalance.StudentId = studentId;
                feeBalance.CreatedOn = DateTime.Now;

                dbContext.FeeBalances.Add(feeBalance);
                dbContext.SaveChanges();
            }

            return feeBalance;
        }


		public FeeBalance GetFeeBalanceById(int feeBalanceId)
        {
            return dbContext.FeeBalances.Where(x => x.Id == feeBalanceId).FirstOrDefault();
        }
		
        public int AddFeeBalance(FeeBalance balance)
        {
            int result = -1;
            if (balance != null)
            {
                dbContext.FeeBalances.Add(balance);
                dbContext.SaveChanges();
                result = (int)balance.Id;
            }
            return result;
        }

        public void UpdateFeeBalance(FeeBalance balance)
        {
            if (balance != null)
            {
                FeeBalance sessionObject = dbContext.FeeBalances.Find(balance.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(balance);
                dbContext.SaveChanges();
            }
        }

        public List<AnnualCharge> GetAnnualChargesByStudentIdAndMonth(int studentId, string currentMonth)
        {
            return dbContext.AnnualCharges.Where(x => x.StudentId == studentId && x.ForMonth == currentMonth).ToList();
        }

        public AnnualCharge GetAnnualChargesByStudentId(int studentId)
        {
            return dbContext.AnnualCharges.Where(x => x.StudentId == studentId).FirstOrDefault();
        }

        public ChallanStudentDetail GetStudentChallanDetailByStudentId(int studentId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.ChallanStudentDetails.Where(x => x.StdId == studentId).Include(x => x.Challan).FirstOrDefault();
        }

        public ChallanStudentDetail GetStudentChallanDetailById(int Id)
        {
            var issuedChallan = dbContext.IssuedChallans.Find(Id);
            return dbContext.ChallanStudentDetails.Where(x => x.Id == issuedChallan.ChallanToStdId).FirstOrDefault();
            //return dbContext.ChallanStudentDetails.Where(x => x.Id == Id).Include(x => x.Student).FirstOrDefault();
        }

        public void SaveFeeDiscount(int[] Discount, int[] DetailId)
        {
            int i = 0;
            foreach (int id in DetailId)
            {
                var detail = dbContext.IssueChalanDetails.Where(x => x.ID == id).FirstOrDefault();
                if (Discount[i] > 0)
                {
                    detail.Discount = Discount[i];
                    dbContext.Entry(detail).State = EntityState.Modified;
                }
                i++;
            }
            dbContext.SaveChanges();
        }
        public void UpdateStudentChallanDetail(ChallanStudentDetail chalanStudentDetail)
        {
            dbContext.Entry(chalanStudentDetail).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int AddStudentChallanDetail(ChallanStudentDetail chalanStudentDetail)
        {
            int result = -1;
            if (chalanStudentDetail != null)
            {
                dbContext.ChallanStudentDetails.Add(chalanStudentDetail);
                dbContext.SaveChanges();
                result = (int)chalanStudentDetail.Id;
            }
            return result;
        }

        public FeeSubmissionDetails GetMonthlyFeeStats(int branchId, DateTime from, DateTime to, string view = "month")
        {
            var response = new FeeSubmissionDetails();
            var fee = dbContext.IssuedChallans.Where(n => n.BranchId == branchId
                && n.IssuedFlag == 1
                && n.PaidDate != null && n.PaidDate >= from && n.PaidDate <= to).OrderBy(n => n.PaidDate).ToList();
            response.Receivable = (double)fee.Sum(s => s.ChalanAmount);
            response.PaidFee = (double)fee.Where(n => n.PaidFlag == 1).Sum(n => n.Amount);
            response.UnpaidFee = (double)fee.Where(n => n.PaidFlag == 0).Sum(n => n.ChalanAmount);
            double partialPaid = (double)fee.Where(n => n.PaidFlag == 1).Sum(n => n.ChalanAmount - n.Amount);
            response.UnpaidFee += partialPaid;
            response.PaidToday = (double)fee.Where(n => n.PaidFlag == 1 && n.PaidDate != null && n.PaidDate.Value.Date == DateTime.Now.Date).Sum(n => n.Amount);

            response.FeeByDate = new List<FeeByDate>();

            if (view == "month")
            {
                var yearMonths = fee.Select(n => n.PaidDate.Value.Year + n.PaidDate.Value.Month).Distinct();
                foreach (var yearMonth in yearMonths)
                {
                    var feeByDateDTO = new FeeByDate();
                    var monthEntries = fee.Where(n => (n.PaidDate.Value.Year + n.PaidDate.Value.Month) == yearMonth);

                    feeByDateDTO.Month = monthEntries.First().PaidDate.Value.ToString("MMM") + "-" + monthEntries.First().PaidDate.Value.ToString("yyyy");
                    feeByDateDTO.DepositedFeeAmount = (double)monthEntries.Where(n => n.PaidFlag == 1).Sum(n => n.Amount);
                    feeByDateDTO.PendingFeeAmount = (double)monthEntries.Where(n => n.PaidFlag == 0).Sum(n => n.Amount);
                    partialPaid = (double)monthEntries.Where(n => n.PaidFlag == 1).Sum(n => n.ChalanAmount - n.Amount);
                    feeByDateDTO.PendingFeeAmount += partialPaid;
                    feeByDateDTO.TotalFeeAmount = (double)monthEntries.Sum(n => n.Amount);
                    response.FeeByDate.Add(feeByDateDTO);
                }
            }
            else
            {
                var paidDates = fee.Select(n => n.PaidDate.Value.Date).Distinct();
                foreach (var date in paidDates)
                {
                    var feeByDateDTO = new FeeByDate();
                    var feeByDate = fee.Where(n => n.PaidDate.Value.Date == date).ToList();

                    feeByDateDTO.Date = date;
                    feeByDateDTO.DepositedFeeAmount = (double)feeByDate.Where(n => n.PaidFlag == 1).Sum(n => n.Amount);
                    feeByDateDTO.PendingFeeAmount = (double)feeByDate.Where(n => n.PaidFlag == 0).Sum(n => n.Amount);
                    partialPaid = (double)feeByDate.Where(n => n.PaidFlag == 1).Sum(n => n.ChalanAmount - n.Amount);
                    feeByDateDTO.PendingFeeAmount += partialPaid;
                    feeByDateDTO.TotalFeeAmount = (double)feeByDate.Sum(n => n.Amount);
                    response.FeeByDate.Add(feeByDateDTO);
                }
            }
            

            return response;
        }

        public FeeSubmissionDetails GetFeeDetailsByStatus(int branchId, DateTime from, DateTime to)
        {
            var response = new FeeSubmissionDetails();
            var fee = dbContext.IssuedChallans.Where(n => n.BranchId == branchId && n.PaidDate >= from && n.PaidDate <= to).ToList();
            response.PendingFee = (double)fee.Where(n => n.IssuedFlag == 0).Select(s => s.ChalanAmount).Sum();
            response.Receivable = (double)fee.Where(n => n.IssuedFlag == 1).Select(s => s.ChalanAmount).Sum();
            response.PaidFee = (double)fee.Where(n => n.PaidFlag == 1).Select(n => n.Amount).Sum();
            response.UnpaidFee = (double)fee.Where(n => n.PaidFlag == 0).Select(n => n.ChalanAmount).Sum();
            double partialPaid = (double)fee.Where(n => n.PaidFlag == 1).Sum(n => n.ChalanAmount - n.Amount);
            response.UnpaidFee += partialPaid;

            return response;
        }

        public List<FeeByClassSection> GetFeeDetailsByClassSection(int branchId, int feeStatus, DateTime from, DateTime to, int classId = 0)
        {
            var pending = FeeStatus.Pending.GetHashCode();
            var unpaid = FeeStatus.Unpaid.GetHashCode();
            var paid = FeeStatus.Paid.GetHashCode();
            return dbContext.IssuedChallans.Where(n => n.BranchId == branchId
                && (feeStatus == pending ? n.IssuedFlag == 0 : true)
                && (feeStatus == unpaid ? n.PaidFlag == 0 && n.IssuedFlag == 1 : true)
                && (feeStatus == paid  ? n.PaidFlag == 1 : true)
                && (feeStatus == paid ? (n.PaidDate >= from && n.PaidDate <= to) : (n.IssueDate >= from && n.IssueDate <= to))
                && (classId > 0 ? n.ChallanStudentDetail.Student.ClassSection.ClassId == classId : true)).GroupBy(n => classId > 0 ? n.ChallanStudentDetail.Student.ClassSection.SectionId : n.ChallanStudentDetail.Student.ClassSection.ClassId)
                .Select(n => new FeeByClassSection
                {
                    Key = n.Key,
                    Status = feeStatus,
                    From = from,
                    To = to,
                    ClassId = n.FirstOrDefault().ChallanStudentDetail.Student.ClassSection.ClassId,
                    ClassName = n.FirstOrDefault().ChallanStudentDetail.Student.ClassSection.Class.Name,
                    SectionId = n.FirstOrDefault().ChallanStudentDetail.Student.ClassSection.SectionId,
                    SectionName = n.FirstOrDefault().ChallanStudentDetail.Student.ClassSection.Section.Name,
                    Fee = n.Sum(x => (feeStatus == paid ? x.Amount : x.ChalanAmount))
                }).ToList();
        }

        public int GetChallanAmountByChallanId(int challanId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return (int)dbContext.ChallanFeeHeadDetails.Where(x => x.ChallanId == challanId).Sum(x => x.Amount);
        }

        public int AddIssueChallan(IssuedChallan issueChallan)
        {
            int result = -1;
            if (issueChallan != null)
            {
                dbContext.IssuedChallans.Add(issueChallan);
                dbContext.SaveChanges();
                result = (int)issueChallan.Id;
            }
            return result;
        }

        public int AddAnnualCharges(AnnualCharge charges)
        {
            int result = -1;
            if (charges != null)
            {
                dbContext.AnnualCharges.Add(charges);
                dbContext.SaveChanges();
                result = (int)charges.Id;
            }
            return result;
        }

        public void DeleteAnnualCharges(AnnualCharge charges)
        {
            dbContext.AnnualCharges.Remove(charges);
            dbContext.SaveChanges();
        }

        public IssuedChallan GetIssueChallanByIdAndMonth(int id, string month)
        {
            return dbContext.IssuedChallans.Where(x => x.Id == id && x.ForMonth == month).Include(x => x.ChallanStudentDetail).FirstOrDefault();
        }

        public void DeleteIssueChallan(IssuedChallan challan)
        {
            dbContext.IssuedChallans.Remove(challan);
            dbContext.SaveChanges();
        }

        public void DeleteIssueChallanDetail(IssueChalanDetail challanDetail)
        {
            dbContext.IssueChalanDetails.Remove(challanDetail);
            dbContext.SaveChanges();
        }

        public IssuedChallan GetIssueChallanById(int id)
        {
            return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == id).Include(x => x.ChallanStudentDetail).FirstOrDefault();
        }

        public IssuedChallan GetIssueChallanByChalanId(int id)
        {
            return dbContext.IssuedChallans.Where(x => x.Id == id).Include(x => x.ChallanStudentDetail).FirstOrDefault();
        }

        public IssuedChallan GetIssueChallanByChalanStudentId(int id)
        {
            return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == id).FirstOrDefault();
        }

        public IssuedChallan GetPaidChallanByChalanStudentId(int id)
        {
            var list = dbContext.IssuedChallans.Where(x => x.ChallanToStdId == id && x.IssuedFlag == 1).ToList();
            if (list.Count == 1 && list[0].PaidFlag == 0)
                return null;
            else
                return dbContext.IssuedChallans.Where(x => x.ChallanToStdId == id && x.IssuedFlag == 1).FirstOrDefault();
        }

        public List<IssuedChallanViewModel> GetIssueChallanByChallanNo(int chalanNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where issueChallan.Id == chalanNo
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.FatherName,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 //DueDate = issueChallan.IssuedFlag == 1 ? issueChallan.DueDate.Value.Date.ToString() : "",
                                 IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)
                             };

            return callanList.ToList();
            //return dbContext.IssuedChallans.Where(x => x.Id == chalanNo).ToList();
        }

        public List<IssuedChallanViewModel> GetPaidChallanByChallanNo(int chalanNo)
        {
            var callanList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where issueChallan.Id == chalanNo
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.Id,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.FatherName,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 //DueDate = issueChallan.IssuedFlag == 1 ? issueChallan.DueDate.Value.Date.ToString() : "",
                                 IsPaid = issueChallan.PaidFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)
                             };

            return callanList.ToList();
            //return dbContext.IssuedChallans.Where(x => x.Id == chalanNo).ToList();
        }

        public void UpdateIssueChallan(IssuedChallan issueChallan)
        {
            if (issueChallan != null)
            {
                IssuedChallan sessionObject = dbContext.IssuedChallans.Find(issueChallan.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(issueChallan);
                dbContext.SaveChanges();
            }
        }

        public void UpdateAnnualCharges(AnnualCharge charges)
        {
            if (charges != null)
            {
                AnnualCharge sessionObject = dbContext.AnnualCharges.Find(charges.Id);
                dbContext.Entry(sessionObject).CurrentValues.SetValues(charges);
                dbContext.SaveChanges();
            }
        }

        public List<FinanceFourthLvlAccount> GetFeePaidAccounts(int accountType, int financeAccountId)
        {
            if (accountType == 1)
                accountType = 13;
            else
                accountType = 14;
            return dbContext.FinanceFourthLvlAccounts.Where(x => x.ThirdLvlAccountId == accountType && (financeAccountId == 0 || financeAccountId == x.Id)).ToList();
        }

        public IssueChallanConfig GetFine(int branchId)
        {
            return dbContext.IssueChallanConfigs.Where(x => x.BranchId == branchId).FirstOrDefault();
        }

        public void AddFineValue(IssueChallanConfig fine)
        {
            dbContext.IssueChallanConfigs.Add(fine);
            dbContext.SaveChanges();
        }

        public void UpdateFineValue(IssueChallanConfig fine)
        {
            dbContext.Entry(fine).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region FEE_NONUI_FUNCTIONS

        public List<AccountType> GetAllAccountTypes()
        {
            return dbContext.AccountTypes.ToList();
        }

        public List<FinanceFourthLvlAccount> GetFeeAccountDetails(int branchId)
        {
            //return dbContext.FinanceFourthLvlAccounts.Where(x => (x.ThirdLvlAccountId == 14 || x.ThirdLvlAccountId == 13)
            //    && x.BranchId == branchId).ToList();

            return dbContext.FinanceFourthLvlAccounts.Where(x => (x.ThirdLvlAccountId == 14 || x.ThirdLvlAccountId == 13)
                ).ToList();
        }

        public List<Month> GetAllMonths()
        {
            return dbContext.Months.ToList();
        }

        public List<StudentPerChallan> GetAllStudentPerChallans()
        {
            return dbContext.StudentPerChallans.ToList();
        }

        public List<Year> GetAllYears()
        {
            return dbContext.Years.OrderByDescending(x => x.Id).ToList();
        }

        public List<ResultType> GetAllResultTypes()
        {
            return dbContext.ResultTypes.OrderByDescending(x => x.Id).ToList();
        }
        public int GetDefinedFine()
        {
            return (int)dbContext.Fines.Where(x => x.Id == 1).FirstOrDefault().Fine1;
        }

        //public Fine GetFine()
        //{
        //    return dbContext.Fines.Where(x => x.Id == 1).FirstOrDefault();
        //}

        public void UpdateFine(Fine fine)
        {
            dbContext.Entry(fine).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public SchoolConfig GetSchoolConfig()
        {
            return dbContext.SchoolConfigs.FirstOrDefault();
        }
        #endregion

        #region

        public void SaveAdmissionCharges(StudentAdmissionCharge charges)
        {
            dbContext.StudentAdmissionCharges.Add(charges);
            dbContext.SaveChanges();
        }

        public StudentAdmissionCharge GetStudentAdmissionChargesById(int id)
        {
            return dbContext.StudentAdmissionCharges.Where(c => c.Id == id).FirstOrDefault();
        }

        public List<StudentAdmissionCharge> GetStudentAdmissionChargesByStudentId(int id)
        {
            return dbContext.StudentAdmissionCharges.Where(c => c.StudentId == id).Include(x => x.FeeHead).ToList();
        }

        public List<StudentAdmissionCharge> GetPositiveStudentAdmissionChargesByStudentId(int id)
        {
            return dbContext.StudentAdmissionCharges.Where(c => c.StudentId == id && c.Amount > 0).ToList();
        }


        public void UpdateStudentAdmissionCharges(StudentAdmissionCharge charges)
        {
            dbContext.Entry(charges).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        #endregion



        #region SMS_NOTIFICATION

        public int AddSmsMessage(SmsMessage branch)
        {
            int result = -1;
            if (branch != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.SmsMessages.Add(branch);
                dbContext.SaveChanges();
                result = branch.Id;
            }

            return result;
        }

        public SmsMessage GetMessageById(int messageId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.SmsMessages.Where(x => x.Id == messageId).FirstOrDefault();
        }

        public List<SmsMessage> GetAllMessages()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.SmsMessages.ToList();
        }

        public void UpdateSmsMessage(SmsMessage bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(bracnh).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteSmsMessage(SmsMessage bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.SmsMessages.Remove(bracnh);
                dbContext.SaveChanges();
            }
        }

        public int AddSmsHistory(SmsHistory branch)
        {
            int result = -1;
            if (branch != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.SmsHistories.Add(branch);
                dbContext.SaveChanges();
                result = branch.Id;
            }

            return result;
        }

        public SmsHistory GetSmsHistoryById(int messageId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.SmsHistories.Where(x => x.Id == messageId).FirstOrDefault();
        }

        public List<SmsHistory> GetSmsHistoryForAttendace(int staffId, int stdId, DateTime sentDate, DateTime nextDate, int attendanceStatus)
        {
            //return dbContext.SmsHistories.Where(n => n.StaffId == staffId
            //    && (n.StdId == stdId)
            //    && (n.AttendanceStatus == attendanceStatus)
            //    && (((DateTime)n.SentDate).Year == sentDate.Year)
            //    && (((DateTime)n.SentDate).Month == sentDate.Month)
            //    && (((DateTime)n.SentDate).Day == sentDate.Day))
            //    .Select(n => new SmsHistory
            //    {
            //        Id = n.Id,
            //        StaffId = n.StaffId,
            //        StdId = n.StdId,
            //        Message = n.Message,
            //        SentDate = n.SentDate,
            //        Response = n.Response,
            //        UserId = n.UserId,
            //        EventId = n.EventId,
            //        AttendanceStatus = n.AttendanceStatus
            //    }).ToList();

            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.SmsHistories.Where(x => x.StaffId == staffId && x.StdId == stdId && x.AttendanceStatus == attendanceStatus && x.SentDate > sentDate && x.SentDate < nextDate).ToList();

            //dbContext.Configuration.LazyLoadingEnabled = false;
            //return dbContext.SmsHistories.Where(x => x.StaffId == staffId && x.StdId == stdId && x.AttendanceStatus == attendanceStatus && x.SentDate.ToString().Split(' ')[0] == getDate(sentDate)).ToList();
        }

        public List<SmsHistory> GetAllSmsHistory()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.SmsHistories.ToList();
        }

        public void UpdateSmsHistory(SmsHistory bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(bracnh).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteSmsHistory(SmsHistory bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.SmsHistories.Remove(bracnh);
                dbContext.SaveChanges();
            }
        }

        public SmsVender GetSmsVenderByBranchId(int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.SmsVenders.Where(x => x.BranchId == branchId).FirstOrDefault();
        }

        public List<SmsVender> GetAllSmsVenders()
        {
            return dbContext.SmsVenders.ToList();
        }
        public List<SmsEventParam> GetAllSmsEventParam()
        {
            return dbContext.SmsEventParams.ToList();
        }
        public List<SmsModel> GetSMSEvents(int BranchId)
        {
            var query = from events in dbContext.SmsEvents
                        join temp in dbContext.SmsMessages on events.SmsTemplateId equals temp.Id into ps
                        from temp in ps.DefaultIfEmpty()
                        where events.BranchId == BranchId
                        select new SmsModel
                        {
                            EventId = events.Id,
                            EventName = events.EventName,
                            EventDetail = events.EventDetail,
                            SmsFlag = events.SmsFlag,
                            SmsFlagDescription = events.SmsFlag == false ? "No" : "Yes",
                            TemplateName = (temp == null ? "" : temp.Name),
                            TemplateText = (temp == null ? "" : temp.Message)
                        };
            return query.ToList();
        }

        public List<SmsEvent> GetAllSMSEvents()
        {
            return dbContext.SmsEvents.ToList();
        }

        public SmsEvent GetSmsEventById(int eventId)
        {
            return dbContext.SmsEvents.Find(eventId);
        }

        public SmsEvent GetSmsEventByName(string smsEventName)
        {
            return dbContext.SmsEvents.Where(x => x.EventName == smsEventName).FirstOrDefault();
        }

        public void UpdateSmsEvent(SmsEvent smsEvent)
        {
            dbContext.Entry(smsEvent).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<SmsParam> GetSmsParams()
        {
            return dbContext.SmsParams.ToList();
        }
        #endregion

        public List<IssuedChallanViewModel> SearchIssueChallanForSms(int classSectionId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo)
        {
            var balanceList = from issueChallan in dbContext.IssuedChallans
                             join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
                             join student in dbContext.Students on studentDetail.StdId equals student.id
                             join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
                             join clas in dbContext.Classes on clSec.ClassId equals clas.Id
                             join sec in dbContext.Sections on clSec.SectionId equals sec.Id
                             join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
                             from feeBal in ps.DefaultIfEmpty()
                             join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
                             from annCh in ps1.DefaultIfEmpty()
                             where (student.ClassSectionId == classSectionId) &&
                             (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
                             (student.Contact_1 != null && student.Contact_1 != "") &&
                              (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
                              (name == null || student.Name == null || student.Name.Contains(name)) &&
                              (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
                              && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
                             (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) && (issueChallan.PaidFlag == 0) &&
                             (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
                              && student.LeavingStatus == 1 //&& feeBal.Balance > 0
                             select new IssuedChallanViewModel
                             {
                                 Id = (int)issueChallan.ChallanToStdId,
                                 studentId = student.id,
                                 Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
                                 Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
                                 RollNumber = student.RollNumber,
                                 Fathername = student.FatherName,
                                 Name = student.Name,
                                 Contact_1 = student.Contact_1,
                                 Class = clas.Name,
                                 Section = sec.Name,
                                 PaidDate = (DateTime)issueChallan.PaidDate,
                                 DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
                                 IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
                                 AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
                                 Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
                                 PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
                                 Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)

                             };

            var studentList = balanceList.ToList();

            if (studentList != null && studentList.Count > 0)
            {
                foreach (var std in studentList)
                {
                    std.Balance = GetUnapidFee(std.Id);
                }

                studentList = studentList.Where(x => x.Balance > 0).ToList();
            }

            return studentList;

            //var callanList = from issueChallan in dbContext.IssuedChallans
            //                 join studentDetail in dbContext.ChallanStudentDetails on issueChallan.ChallanToStdId equals studentDetail.Id
            //                 join student in dbContext.Students on studentDetail.StdId equals student.id
            //                 join clSec in dbContext.ClassSections on student.ClassSectionId equals clSec.ClassSectionId
            //                 join clas in dbContext.Classes on clSec.ClassId equals clas.Id
            //                 join sec in dbContext.Sections on clSec.SectionId equals sec.Id
            //                 join feeBal in dbContext.FeeBalances on student.id equals feeBal.StudentId into ps
            //                 from feeBal in ps.DefaultIfEmpty()
            //                 join annCh in dbContext.AnnualCharges on student.id equals annCh.StudentId into ps1
            //                 from annCh in ps1.DefaultIfEmpty()
            //                 where (student.ClassSectionId == classSectionId) &&
            //                 (rollNumber == null || student.RollNumber.Contains(rollNumber)) &&
            //                 (student.Contact_1 != null && student.Contact_1 != "") &&
            //                  (fatherCnic == null || student.FatherCNIC == null || student.FatherCNIC.Contains(fatherCnic)) &&
            //                  (name == null || student.Name == null || student.Name.Contains(name)) &&
            //                  (fatherName == null || student.FatherName == null || student.FatherName.Contains(fatherName))
            //                  && (AdmissionNo == null || student.AdmissionNo == null || student.AdmissionNo.Contains(AdmissionNo)) &&
            //                 (currentMonth == null || issueChallan.ForMonth.Contains(currentMonth)) && (issueChallan.PaidFlag == 0) &&
            //                 (student.BranchId == branchId) && (issueChallan.BranchId == branchId)
            //                  && student.LeavingStatus == 1 && feeBal.Balance > 0
            //                 select new IssuedChallanViewModel
            //                 {
            //                     Id = (int)issueChallan.ChallanToStdId,
            //                     studentId = student.id,
            //                     Balance = (int)(feeBal.Balance == null ? 0 : feeBal.Balance),
            //                     Advance = (int)(feeBal.Advance == null ? 0 : feeBal.Advance),
            //                     RollNumber = student.RollNumber,
            //                     Fathername = student.FatherName,
            //                     Name = student.Name,
            //                     Contact_1 = student.Contact_1,
            //                     Class = clas.Name,
            //                     Section = sec.Name,
            //                     PaidDate = (DateTime)issueChallan.PaidDate,
            //                     DueDate = (DateTime)(issueChallan.IssuedFlag == 1 ? issueChallan.DueDate : DateTime.Now),
            //                     IsPaid = issueChallan.IssuedFlag == 1 ? "Yes" : "No",
            //                     AnnualCharges = (int)(annCh.Charges == null ? 0 : annCh.Charges),
            //                     Amount = (int)(issueChallan.ChalanAmount == null ? 0 : issueChallan.ChalanAmount),
            //                     PaidAmount = (int)(issueChallan.Amount == null ? 0 : issueChallan.Amount),
            //                     Fine = (int)(issueChallan.Fine == null ? 0 : issueChallan.Fine)

            //                 };

            //return callanList.ToList();
        }

        private int GetUnapidFee(int challanToStudentId)
        {
            var query = from detail in dbContext.IssueChalanDetails
                        join issue in dbContext.IssuedChallans on detail.IssueChallanId equals issue.Id
                        where issue.ChallanToStdId == challanToStudentId
                        select new {
                            TotalAmount = detail.TotalAmount == null ? 0 : detail.TotalAmount,
                            PayAmount = detail.PayAmount == null ? 0 : detail.PayAmount,
                            Discount = detail.Discount == null ? 0 : detail.Discount
                        };

            var balance = query.ToList();
            int amount = (int)balance.Sum(x => x.TotalAmount - x.PayAmount - x.Discount);

            return amount;
        }

        public void saveIssuedChallanDetail(IssueChalanDetail detail)
        {
            detail.Discount = 0;
            dbContext.IssueChalanDetails.Add(detail);
            dbContext.SaveChanges();
        }

        public List<ArreartHistory> GetLastMonthArrears(int issuedChalanId, int FeeBalanceId)
        {
            var currentMonth = dbContext.IssuedChallans.Find(issuedChalanId);
            var lastMonth = dbContext.IssuedChallans.Where(x => x.Id != issuedChalanId && x.ChallanToStdId == currentMonth.ChallanToStdId && x.IssuedFlag == 1).OrderByDescending(x => x.Id).FirstOrDefault();
            List<ArreartHistory> list = new List<ArreartHistory>();
            if (lastMonth != null)
            {
                DateTime fromDate = (DateTime)lastMonth.IssueDate;
                DateTime toDate = (DateTime)currentMonth.IssueDate;
                list = dbContext.ArreartHistories.Where(x => x.CreatedOn >= fromDate && x.CreatedOn <= toDate && x.FeeBalanceId == FeeBalanceId).ToList();
            }
            return list;
        }

        public List<IssueChalanDetail> GetLastMonthUnpaidDetail(int issuedChalanId)
        {
            var currentMonth = dbContext.IssuedChallans.Find(issuedChalanId);
            var lastMonth = dbContext.IssuedChallans.Where(x => x.Id != issuedChalanId && x.ChallanToStdId == currentMonth.ChallanToStdId && x.IssuedFlag == 1).OrderByDescending(x => x.Id).FirstOrDefault();
            List<IssueChalanDetail> detail = null;

            if (lastMonth != null && lastMonth.PaidFlag == 1)
            {
                detail = dbContext.IssueChalanDetails.Where(x => x.IssueChallanId == lastMonth.Id).ToList();
            }
            return detail;
        }

        public void AddFeeIncrementHistoryDtail(FeeIncrementHistoryDetail detail)
        {
            dbContext.FeeIncrementHistoryDetails.Add(detail);
            dbContext.SaveChanges();
        }

        public FeeIncrementHistoryDetail GetLastFeeIncrementHistoryDetail(int studentId)
        {
            return dbContext.FeeIncrementHistoryDetails.Where(x => x.StudentId == studentId && x.IsRevoked == false).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public FeeIncrementHistoryDetail GetFeeIncrementHistoryDetail(int detailId)
        {
            return dbContext.FeeIncrementHistoryDetails.Find(detailId);
        }

        public void UpdateFeeIncreentHistoryDetail(FeeIncrementHistoryDetail detail)
        {
            dbContext.Entry(detail).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<FeeIncrementModel> GetFeeIncrementHistoryDetailList(int studentId)
        {
            var query = from history in dbContext.FeeIncrementHistoryDetails
                        join student in dbContext.Students on history.StudentId equals student.id
                        join head in dbContext.FeeHeads on history.FeeHeadId equals head.Id
                        where history.StudentId == studentId
                        select new FeeIncrementModel
                        {
                            IncrementId = history.Id,
                            Name = student.Name,
                            FatherName = student.FatherName,
                            RollNO = student.RollNumber,
                            Amount = (int)history.Amount,
                            Percentage = (int)history.Percentage,
                            HeadName = head.Name,
                            Description = history.Description,
                            IncrementDate = (DateTime)history.CreatedOn,
                            Revoked = (history.IsRevoked == true ? "Yes" : "No")
                        };
            return query.OrderByDescending(x => x.IncrementId).ToList();
        }

        public int GetHeadChallanDetailCount(int FeeHeadId)
        {
            return dbContext.ChallanFeeHeadDetails.Where(x => x.HeadId == FeeHeadId).Count();
        }

        public int GetChallanDetailCount(int ChallanId)
        {
            return dbContext.ChallanFeeHeadDetails.Where(x => x.ChallanId == ChallanId).Count();
        }

        public FeeHead GetAdmissionChargesHead()
        {
            var head = dbContext.FeeHeads.Where(x => x.Name.ToLower().Contains("admission charges")).FirstOrDefault();
            if (head == null)
            {
                head = dbContext.FeeHeads.Where(x => x.Name.ToLower().Contains("admission fee")).FirstOrDefault();
            }
            return head;
        }

        public List<IssuedChallanDetailModel> GetMonthlyWaveOffFeeDetail(int studentId)
        {
            var detailQuery = from challan in dbContext.IssuedChallans
                              join detail in dbContext.IssueChalanDetails on challan.Id equals detail.IssueChallanId
                              join head in dbContext.FeeHeads on detail.FeeHeadId equals head.Id
                              join chStd in dbContext.ChallanStudentDetails on challan.ChallanToStdId equals chStd.Id
                              where chStd.StdId == studentId
                              && (detail.PayAmount != detail.TotalAmount || challan.PaidFlag == 0)
                              && (challan.IsLcm != true || challan.IsLcm == null)
                              && detail.Type != 2
                              select new IssuedChallanDetailModel
                              {
                                  IssueChallanDetailId = detail.ID,
                                  IssueChallanId = (int)challan.Id,
                                  ForMonth = challan.ForMonth,
                                  HeadName = head.Name,
                                  FeeHeadId = head.Id,
                                  HeadDetail = head.Name + " (" + challan.ForMonth + ")" + (detail.Type == 3 ? " Extra Charges" : ""),
                                  Discount = (int)(detail.Discount == null ? 0 : detail.Discount),
                                  TotalAmount = (int)(detail.TotalAmount == null ? 0 : detail.TotalAmount),
                                  PaidAmount = (int)(detail.PayAmount == null ? 0 : detail.PayAmount),
                                  PendingAmount = (int)(detail.TotalAmount == null ? 0 : detail.TotalAmount) - (int)(detail.PayAmount == null ? 0 : detail.PayAmount) - (int)(detail.Discount == null ? 0 : detail.Discount)
                              };
            List<IssuedChallanDetailModel> detailList = detailQuery.Distinct().ToList();

            detailList = detailList.Where(x => x.PendingAmount > 0).ToList();

            foreach (var detail in detailList)
            {
                if (detail.Discount > 0)
                    detail.HeadDetail += "(" + "Discount :: " + detail.Discount + ")";
            }
            return detailList;
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
