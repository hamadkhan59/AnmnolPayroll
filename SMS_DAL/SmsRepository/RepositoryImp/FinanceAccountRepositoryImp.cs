using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMS_DAL.ViewModel.FinanceDashboardViewModel;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class FinanceAccountRepositoryImp : IFinanceAccountRepository
    {
        private SC_WEBEntities2 dbContext1;

        public FinanceAccountRepositoryImp(SC_WEBEntities2 context)   
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

        public FinanceDashboardViewModel GetFinanceStats(int branchId, DateTime fromDate, DateTime toDate, string view = "month", int firstLvlId = 0, int secondLvlId = 0, int thirdLvlId = 0, int fourthLvlId = 0, int fifthLvlId = 0)
        {
            var response = new FinanceDashboardViewModel();
            var entries = dbContext.JournalEntries.Where(n => n.BranchId == branchId
                && EntityFunctions.TruncateTime(n.CreatedOn) >= fromDate.Date
                && EntityFunctions.TruncateTime(n.CreatedOn) <= toDate.Date);
            //var creditDetails = entries.SelectMany(n => n.JournalEntryCreditDetails)
            //    .Where(n => (fifthLvlId > 0 ? n.FifthLvlAccountId == fifthLvlId : true)
            //    && (fourthLvlId > 0 ? n.FinanceFifthLvlAccount.FourthLvlAccountId == fourthLvlId : true)
            //    && (thirdLvlId > 0 ? n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == thirdLvlId : true)
            //    && (secondLvlId > 0 ? n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == secondLvlId : true)
            //    && (firstLvlId > 0 ? n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == firstLvlId : true));

            var creditQry = from detail in dbContext.JournalEntryCreditDetails
                            join entry in dbContext.JournalEntries on detail.EntryId equals entry.EntryId
                            join fifth in dbContext.FinanceFifthLvlAccounts on detail.FifthLvlAccountId equals fifth.Id
                            join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                            join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                            join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                            join first in dbContext.FinanceFirstLvlAccounts on second.FirstLvlAccountId equals first.Id
                            where entry.BranchId == branchId
                            && (EntityFunctions.TruncateTime(entry.CreatedOn) >= fromDate.Date
                            && EntityFunctions.TruncateTime(entry.CreatedOn) <= toDate.Date)
                            && (fifthLvlId > 0 ? detail.FifthLvlAccountId == fifthLvlId : true)
                            && (fourthLvlId > 0 ? fourth.Id == fourthLvlId : true)
                            && (thirdLvlId > 0 ? third.Id == thirdLvlId : true)
                            && (secondLvlId > 0 ? second.Id == secondLvlId : true)
                            && (firstLvlId > 0 ? first.Id == firstLvlId : true)
                            select new JournalEntryDetailModel
                            {
                                Amount = detail.Amount,
                                Count = detail.Count,
                                Description = detail.Description,
                                EntryId = detail.EntryId,
                                FifthLvlAccountId = detail.FifthLvlAccountId,
                                FifthLvlLvlAccountName = fifth.AccountName,
                                FirstLvlAccountId = first.Id,
                                FirstLvlLvlAccountName = first.AccountName,
                                FourthLvlAccountId = fourth.Id,
                                FourthLvlLvlAccountName = fourth.AccountName,
                                ID = detail.ID,
                                SeccondLvlAccountId = second.Id,
                                SeccondLvlLvlAccountName = second.AccountName,
                                ThirdLvlAccountId = third.Id,
                                ThirdLvlLvlAccountName = third.AccountName
                            };

            var creditDetails = creditQry.Distinct().ToList();

            //var debitDetails = entries.SelectMany(n => n.JournalEntryDebitDetails)
            //    .Where(n => (fifthLvlId > 0 ? n.FifthLvlAccountId == fifthLvlId : true)
            //    && (fourthLvlId > 0 ? n.FinanceFifthLvlAccount.FourthLvlAccountId == fourthLvlId : true)
            //    && (thirdLvlId > 0 ? n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == thirdLvlId : true)
            //    && (secondLvlId > 0 ? n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == secondLvlId : true)
            //    && (firstLvlId > 0 ? n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == firstLvlId : true));

            var debitQry = from detail in dbContext.JournalEntryDebitDetails
                           join entry in dbContext.JournalEntries on detail.EntryId equals entry.EntryId
                           join fifth in dbContext.FinanceFifthLvlAccounts on detail.FifthLvlAccountId equals fifth.Id
                           join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                           join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                           join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                           join first in dbContext.FinanceFirstLvlAccounts on second.FirstLvlAccountId equals first.Id
                           where entry.BranchId == branchId
                           && (EntityFunctions.TruncateTime(entry.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(entry.CreatedOn) <= toDate.Date)
                           && (fifthLvlId > 0 ? detail.FifthLvlAccountId == fifthLvlId : true)
                           && (fourthLvlId > 0 ? fourth.Id == fourthLvlId : true)
                           && (thirdLvlId > 0 ? third.Id == thirdLvlId : true)
                           && (secondLvlId > 0 ? second.Id == secondLvlId : true)
                           && (firstLvlId > 0 ? first.Id == firstLvlId : true)
                           select new JournalEntryDetailModel
                            {
                                Amount = detail.Amount,
                                Count = detail.Count,
                                Description = detail.Description,
                                EntryId = detail.EntryId,
                                FifthLvlAccountId = detail.FifthLvlAccountId,
                                FifthLvlLvlAccountName = fifth.AccountName,
                                FirstLvlAccountId = first.Id,
                                FirstLvlLvlAccountName = first.AccountName,
                                FourthLvlAccountId = fourth.Id,
                                FourthLvlLvlAccountName = fourth.AccountName,
                                ID = detail.ID,
                                SeccondLvlAccountId = second.Id,
                                SeccondLvlLvlAccountName = second.AccountName,
                                ThirdLvlAccountId = third.Id,
                                ThirdLvlLvlAccountName = third.AccountName
                            };

            var debitDetails = debitQry.Distinct().ToList();

            if (view == "month")
            {
                var yearMonths = entries.Select(n => n.CreatedOn.Value.Year + n.CreatedOn.Value.Month).Distinct();
                foreach (var yearMonth in yearMonths)
                {
                    var monthEntries = entries.Where(n => (n.CreatedOn.Value.Year + n.CreatedOn.Value.Month) == yearMonth);
                    var entryIds = monthEntries.Select(n => n.EntryId).ToList();
                    response.Months.Add(monthEntries.First().CreatedOn.Value.ToString("MMM") + "-" + monthEntries.First().CreatedOn.Value.ToString("yyyy"));

                    var creditList = creditDetails.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    var cashAmount = creditList.Where(n => n.ThirdLvlLvlAccountName == "Cash").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.CreditCash.Add(cashAmount);
                    var chqueAmount = creditList.Where(n => n.ThirdLvlLvlAccountName == "Bank").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.CreditCheque.Add(chqueAmount);

                    var debitList = debitDetails.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    cashAmount = debitList.Where(n => n.ThirdLvlLvlAccountName == "Cash").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.DebitCash.Add(cashAmount);
                    chqueAmount = debitList.Where(n => n.ThirdLvlLvlAccountName == "Bank").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.DebitCheque.Add(chqueAmount);
                }
            }
            else
            {
                var dates = entries.Select(n => EntityFunctions.TruncateTime(n.CreatedOn)).Distinct();
                foreach (var date in dates)
                {
                    response.Days.Add(date.Value);
                    var entryIds = entries.Where(n => EntityFunctions.TruncateTime(n.CreatedOn) == date).Select(n => n.EntryId).ToList();
                    
                    var creditList = creditDetails.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    var cashAmount = creditList.Where(n => n.ThirdLvlLvlAccountName == "Cash").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.CreditCash.Add(cashAmount);
                    var chqueAmount = creditList.Where(n => n.ThirdLvlLvlAccountName == "Bank").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.CreditCheque.Add(chqueAmount);

                    var debitList = debitDetails.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    cashAmount = debitList.Where(n => n.ThirdLvlLvlAccountName == "Cash").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.DebitCash.Add(cashAmount);
                    chqueAmount = debitList.Where(n => n.ThirdLvlLvlAccountName == "Bank").Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.DebitCheque.Add(chqueAmount);
                }
            }
            
            return response;
        }

        public FinanceDashboardViewModel GetRevenueExpenseStats(int branchId, DateTime fromDate, DateTime toDate, string view = "month")
        {
            var response = new FinanceDashboardViewModel();
            var entries = dbContext.JournalEntries.Where(n => n.BranchId == branchId
                && EntityFunctions.TruncateTime(n.CreatedOn) >= fromDate.Date
                && EntityFunctions.TruncateTime(n.CreatedOn) <= toDate.Date);

            var expenseQry = from detail in dbContext.JournalEntryDebitDetails
                           join entry in dbContext.JournalEntries on detail.EntryId equals entry.EntryId
                           join fifth in dbContext.FinanceFifthLvlAccounts on detail.FifthLvlAccountId equals fifth.Id
                           join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                           join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                           join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                           join first in dbContext.FinanceFirstLvlAccounts on second.FirstLvlAccountId equals first.Id
                           where entry.BranchId == branchId
                           && (EntityFunctions.TruncateTime(entry.CreatedOn) >= fromDate.Date
                           && EntityFunctions.TruncateTime(entry.CreatedOn) <= toDate.Date)
                           && first.Id == 5
                           select new JournalEntryDetailModel
                           {
                               Amount = detail.Amount,
                               Count = detail.Count,
                               Description = detail.Description,
                               EntryId = detail.EntryId,
                               FifthLvlAccountId = detail.FifthLvlAccountId,
                               FifthLvlLvlAccountName = fifth.AccountName,
                               FirstLvlAccountId = first.Id,
                               FirstLvlLvlAccountName = first.AccountName,
                               FourthLvlAccountId = fourth.Id,
                               FourthLvlLvlAccountName = fourth.AccountName,
                               ID = detail.ID,
                               SeccondLvlAccountId = second.Id,
                               SeccondLvlLvlAccountName = second.AccountName,
                               ThirdLvlAccountId = third.Id,
                               ThirdLvlLvlAccountName = third.AccountName
                           };


            //var expenseDetail = entries.SelectMany(n => n.JournalEntryDebitDetails)
            //    .Where(n => n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == 5);

            var expenseDetail = expenseQry.Distinct().ToList();


            var revenueQry = from detail in dbContext.JournalEntryDebitDetails
                             join entry in dbContext.JournalEntries on detail.EntryId equals entry.EntryId
                             join fifth in dbContext.FinanceFifthLvlAccounts on detail.FifthLvlAccountId equals fifth.Id
                             join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                             join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                             join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                             join first in dbContext.FinanceFirstLvlAccounts on second.FirstLvlAccountId equals first.Id
                             where entry.BranchId == branchId
                             && (EntityFunctions.TruncateTime(entry.CreatedOn) >= fromDate.Date
                             && EntityFunctions.TruncateTime(entry.CreatedOn) <= toDate.Date)
                             && first.Id == 10
                             select new JournalEntryDetailModel
                             {
                                 Amount = detail.Amount,
                                 Count = detail.Count,
                                 Description = detail.Description,
                                 EntryId = detail.EntryId,
                                 FifthLvlAccountId = detail.FifthLvlAccountId,
                                 FifthLvlLvlAccountName = fifth.AccountName,
                                 FirstLvlAccountId = first.Id,
                                 FirstLvlLvlAccountName = first.AccountName,
                                 FourthLvlAccountId = fourth.Id,
                                 FourthLvlLvlAccountName = fourth.AccountName,
                                 ID = detail.ID,
                                 SeccondLvlAccountId = second.Id,
                                 SeccondLvlLvlAccountName = second.AccountName,
                                 ThirdLvlAccountId = third.Id,
                                 ThirdLvlLvlAccountName = third.AccountName
                             };

            //var revenueDetail = entries.SelectMany(n => n.JournalEntryDebitDetails)
            //    .Where(n => n.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == 10);

            var revenueDetail = revenueQry.Distinct().ToList();

            if (view == "month")
            {
                var yearMonths = entries.Select(n => n.CreatedOn.Value.Year + n.CreatedOn.Value.Month).Distinct();
                foreach (var yearMonth in yearMonths)
                {
                    var monthEntries = entries.Where(n => (n.CreatedOn.Value.Year + n.CreatedOn.Value.Month) == yearMonth);
                    var entryIds = monthEntries.Select(n => n.EntryId).ToList();
                    response.Months.Add(monthEntries.First().CreatedOn.Value.ToString("MMM") + "-" + monthEntries.First().CreatedOn.Value.ToString("yyyy"));

                    var expenseList = expenseDetail.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    var expenseAmount = expenseList.Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.Expenses.Add(expenseAmount);

                    var revenueList = revenueDetail.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    var revenueAmount = revenueList.Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.Revenue.Add(revenueAmount);
                }
            }
            else
            {
                var dates = entries.Select(n => EntityFunctions.TruncateTime(n.CreatedOn)).Distinct();
                foreach (var date in dates)
                {
                    response.Days.Add(date.Value);
                    var entryIds = entries.Where(n => EntityFunctions.TruncateTime(n.CreatedOn) == date).Select(n => n.EntryId).ToList();

                    var expenseList = expenseDetail.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    var expenseAmount = expenseList.Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.Expenses.Add(expenseAmount);

                    var revenueList = revenueDetail.Where(n => entryIds.Contains(n.EntryId.Value)).ToList();
                    var revenueAmount = revenueList.Sum(n => (n.Amount != null ? n.Amount.Value : 0));
                    response.Revenue.Add(revenueAmount);
                }
            }

            return response;
        }

        #region FINANCE_FIRST_LVL_FUNCTIONS
        public int AddFinanceFirstLvlAccount(FinanceFirstLvlAccount firstLvlAccount)
        {
            int result = -1;
            if (firstLvlAccount != null)
            {
                dbContext.FinanceFirstLvlAccounts.Add(firstLvlAccount);
                dbContext.SaveChanges();
                result = firstLvlAccount.Id;
            }

            return result;
        }

        public FinanceFirstLvlAccount GetFinanceFirstLvlAccountById(int firstLvlAccountId)
        {
            return dbContext.FinanceFirstLvlAccounts.Where(x => x.Id == firstLvlAccountId).FirstOrDefault();
        }

        public FinanceFirstLvlAccount GetFinanceFirstLvlAccountByName(string firstLvlAccountName)
        {
            return dbContext.FinanceFirstLvlAccounts.Where(x => x.AccountName == firstLvlAccountName).FirstOrDefault();
        }

        public FinanceFirstLvlAccount GetFinanceFirstLvlAccountByNameAndId(string firstLvlAccountName, int firstLvlAccountId)
        {
            return dbContext.FinanceFirstLvlAccounts.Where(x => x.AccountName == firstLvlAccountName && x.Id != firstLvlAccountId).FirstOrDefault();
        }

        public List<FinanceFirstLvlAccount> GetAllFinanceFirstLvlAccounts()
        {
            return dbContext.FinanceFirstLvlAccounts.ToList();    
        }

        public void UpdateFinanceFirstLvlAccount(FinanceFirstLvlAccount firstLvlAccount)
        {
            if (firstLvlAccount != null)
            {
                dbContext.Entry(firstLvlAccount).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteFinanceFirstLvlAccount(FinanceFirstLvlAccount clas)
        {
            if (clas != null)
            {
                dbContext.FinanceFirstLvlAccounts.Remove(clas);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region FINANCE_SECCOND_LVL_FUNCTIONS
        public int AddFinanceSeccondLvlAccount(FinanceSeccondLvlAccount seccondLvlAccount)
        {
            int result = -1;
            if (seccondLvlAccount != null)
            {
                dbContext.FinanceSeccondLvlAccounts.Add(seccondLvlAccount);
                dbContext.SaveChanges();
                result = seccondLvlAccount.Id;
            }

            return result;
        }

        public FinanceSeccondLvlAccount GetFinanceSeccondLvlAccountById(int seccondLvlAccountId)
        {
            return dbContext.FinanceSeccondLvlAccounts.Where(x => x.Id == seccondLvlAccountId).FirstOrDefault();
        }

        public FinanceSeccondLvlAccount GetFinanceSeccondLvlAccountByName(string seccondLvlAccountName)
        {
            return dbContext.FinanceSeccondLvlAccounts.Where(x => x.AccountName == seccondLvlAccountName).FirstOrDefault();
        }

        public FinanceSeccondLvlAccount GetFinanceSeccondLvlAccountByNameAndId(string seccondLvlAccountName, int seccondLvlAccountId)
        {
            return dbContext.FinanceSeccondLvlAccounts.Where(x => x.AccountName == seccondLvlAccountName && x.Id != seccondLvlAccountId).FirstOrDefault();
        }

        public List<FinanceSeccondLvlAccount> GetAllFinanceSeccondLvlAccounts()
        {
            return dbContext.FinanceSeccondLvlAccounts.ToList();
        }

        public List<FinanceSeccondLvlAccountModel> GetAllFinanceSeccondLvlAccountsModel()
        {
            var query = from second in dbContext.FinanceSeccondLvlAccounts
                        select new FinanceSeccondLvlAccountModel
                        {
                            AccountDescription = second.AccountDescription,
                            AccountName = second.AccountName, 
                            FirstLvlAccountId = second.FirstLvlAccountId,
                            Id = second.Id
                        };

            return query.ToList();
            //return dbContext.FinanceSeccondLvlAccounts.ToList();
        }

        public void UpdateFinanceSeccondLvlAccount(FinanceSeccondLvlAccount firstLvlAccount)
        {
            if (firstLvlAccount != null)
            {
                dbContext.Entry(firstLvlAccount).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteFinanceSeccondLvlAccount(FinanceSeccondLvlAccount clas)
        {
            if (clas != null)
            {
                dbContext.FinanceSeccondLvlAccounts.Remove(clas);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region FINANCE_THIRD_LVL_FUNCTIONS
        public int AddFinanceThirdLvlAccount(FinanceThirdLvlAccount thirdLvlAccount)
        {
            int result = -1;
            if (thirdLvlAccount != null)
            {
                dbContext.FinanceThirdLvlAccounts.Add(thirdLvlAccount);
                dbContext.SaveChanges();
                result = thirdLvlAccount.Id;
            }

            return result;
        }

        public FinanceThirdLvlAccount GetFinanceThirdLvlAccountById(int thirdLvlAccountId)
        {
            return dbContext.FinanceThirdLvlAccounts.Where(x => x.Id == thirdLvlAccountId).FirstOrDefault();
        }

        public FinanceThirdLvlAccount GetFinanceThirdLvlAccountByName(string thirdLvlAccountName, int branchId)
        {
            return dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == thirdLvlAccountName && x.BranchId == 1).FirstOrDefault();
        }

        public FinanceThirdLvlAccount GetFinanceThirdLvlAccountByNameAndId(string thirdLvlAccountName, int thirdLvlAccountId, int branchId)
        {
            return dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == thirdLvlAccountName && x.Id != thirdLvlAccountId && x.BranchId == 1).FirstOrDefault();
        }

        public List<FinanceThirdLvlAccount> GetAllFinanceThirdLvlAccounts()
        {
            return dbContext.FinanceThirdLvlAccounts.ToList();
        }

        public List<FinanceThirdLvlAccountModel> GetAllFinanceThirdLvlAccountsModel()
        {
            var query = from third in dbContext.FinanceThirdLvlAccounts
                        join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                        select new FinanceThirdLvlAccountModel
                        {
                            AccountDescription = third.AccountDescription,
                            AccountName = third.AccountName,
                            BranchId = third.BranchId,
                            FirstLvlAccountId = second.FirstLvlAccountId,
                            Id = third.Id,
                            SeccondLvlAccountId = second.Id
                        };
            return query.ToList();
            //return dbContext.FinanceThirdLvlAccounts.ToList();
        }


        public void UpdateFinanceThirdLvlAccount(FinanceThirdLvlAccount thirdLvlAccount)
        {
            if (thirdLvlAccount != null)
            {
                dbContext.Entry(thirdLvlAccount).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteFinanceThirdLvlAccount(FinanceThirdLvlAccount thirdLvlAccount)
        {
            if (thirdLvlAccount != null)
            {
                dbContext.FinanceThirdLvlAccounts.Remove(thirdLvlAccount);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region FINANCE_FOURTH_LVL_FUNCTIONS
        public int AddFinanceFourthLvlAccount(FinanceFourthLvlAccount fourthLvlAccount)
        {
            int result = -1;
            if (fourthLvlAccount != null)
            {
                dbContext.FinanceFourthLvlAccounts.Add(fourthLvlAccount);
                dbContext.SaveChanges();
                result = fourthLvlAccount.Id;
            }

            return result;
        }

        public FinanceFourthLvlAccount GetFinanceFourthLvlAccountById(int fourthLvlAccountId)
        {
            return dbContext.FinanceFourthLvlAccounts.Where(x => x.Id == fourthLvlAccountId).FirstOrDefault();
        }

        public FinanceFourthLvlAccount GetFinanceFourthLvlAccountByName(string fourthLvlAccountName, int branchId)
        {
            return dbContext.FinanceFourthLvlAccounts.Where(x => x.AccountName == fourthLvlAccountName && x.BranchId == branchId).FirstOrDefault();
        }

        public FinanceFourthLvlAccount GetFinanceFourthLvlAccountByNameAndId(string fourthLvlAccountName, int fourthLvlAccountId, int branchId)
        {
            return dbContext.FinanceFourthLvlAccounts.Where(x => x.AccountName == fourthLvlAccountName && x.Id != fourthLvlAccountId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<FinanceFourthLvlAccount> GetAllFinanceFourthLvlAccounts()
        {
            return dbContext.FinanceFourthLvlAccounts.ToList();
        }

        public List<FinanceFourthLvlAccountModel> GetAllFinanceFourthLvlAccountsModel()
        {
            var query = from fourth in dbContext.FinanceFourthLvlAccounts
                        join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                        join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                        select new FinanceFourthLvlAccountModel
                        {
                            AccountDescription = fourth.AccountDescription,
                            AccountName = fourth.AccountName,
                            BranchId = fourth.BranchId,
                            FirstLvlAccountId = second.FirstLvlAccountId,
                            Id = fourth.Id,
                            Value = (int)(fourth.Value == null ? 0 : fourth.Value),
                            SeccondLvlAccountId = second.Id,
                            ThirdLvlAccountId = third.Id,
                            IsPettyCashAccount = fourth.IsPettyCashAccount ?? false
                        };
            return query.ToList();
            //return dbContext.FinanceFourthLvlAccounts.ToList();
        }

        public void UpdateFinanceFourthLvlAccount(FinanceFourthLvlAccount fourthLvlAccount)
        {
            var SessionObj = GetFinanceFourthLvlAccountById(fourthLvlAccount.Id);
            if (fourthLvlAccount != null)
            {
                fourthLvlAccount.Value = SessionObj.Value;
                dbContext.Entry(SessionObj).CurrentValues.SetValues(fourthLvlAccount);
                //dbContext.Entry(fourthLvlAccount).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteFinanceFourthLvlAccount(FinanceFourthLvlAccount fourthLvlAccount)
        {
            if (fourthLvlAccount != null)
            {
                dbContext.FinanceFourthLvlAccounts.Remove(fourthLvlAccount);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region FINANCE_FIFTH_LVL_FUNCTIONS
        public int AddFinanceFifthLvlAccount(FinanceFifthLvlAccount fifthLvlAccount)
        {
            int result = -1;
            if (fifthLvlAccount != null)
            {
                dbContext.FinanceFifthLvlAccounts.Add(fifthLvlAccount);
                dbContext.SaveChanges();
                result = fifthLvlAccount.Id;
            }

            return result;
        }

        public FinanceFifthLvlAccount GetFinanceFifthLvlAccountById(int fifthLvlAccountId)
        {
            return dbContext.FinanceFifthLvlAccounts.Where(x => x.Id == fifthLvlAccountId).FirstOrDefault();
        }

        public FinanceFifthLvlAccount GetFinanceFifthLvlAccountByName(string fifthLvlAccountName, int branchId)
        {
            return dbContext.FinanceFifthLvlAccounts.Where(x => x.AccountName == fifthLvlAccountName 
                        && x.BranchId == branchId && x.IsPettyCashAccount == false).FirstOrDefault();
        }

        public FinanceFifthLvlAccount GetPettyCashFinanceFifthLvlAccountByName(string fifthLvlAccountName, int branchId)
        {
            return dbContext.FinanceFifthLvlAccounts.Where(x => x.AccountName == fifthLvlAccountName
                        && x.BranchId == branchId && x.IsPettyCashAccount == true).FirstOrDefault();
        }

        public FinanceFifthLvlAccount GetFinanceFifthLvlAccount(string fifthLvlAccountName, int fourthLvlId)
        {
            var account = dbContext.FinanceFifthLvlAccounts.Where(x => x.AccountName == fifthLvlAccountName && x.FourthLvlAccountId == fourthLvlId).FirstOrDefault();
            if (account == null)
            {
                account = new FinanceFifthLvlAccount();
                account.FourthLvlAccountId = fourthLvlId;
                account.AccountName = fifthLvlAccountName;
                dbContext.FinanceFifthLvlAccounts.Add(account);
                dbContext.SaveChanges();
            }

            return account;
        }


        public FinanceFifthLvlAccount GetFinanceFifthLvlAccountByNameAndId(string fifthLvlAccountName, int fifthLvlAccountId, int branchId)
        {
            return dbContext.FinanceFifthLvlAccounts.Where(x => x.AccountName == fifthLvlAccountName 
                && x.Id != fifthLvlAccountId && x.BranchId == branchId && x.IsPettyCashAccount == false).FirstOrDefault();
        }

        public FinanceFifthLvlAccount GetPettCashFinanceFifthLvlAccountByNameAndId(string fifthLvlAccountName, int fifthLvlAccountId, int branchId)
        {
            return dbContext.FinanceFifthLvlAccounts.Where(x => x.AccountName == fifthLvlAccountName
                && x.Id != fifthLvlAccountId && x.BranchId == branchId && x.IsPettyCashAccount == false).FirstOrDefault();
        }

        public List<FinanceFifthLvlAccount> GetAllFinanceFifthLvlAccounts()
        {
            return dbContext.FinanceFifthLvlAccounts.ToList();
        }

        public List<FinanceFifthLvlAccountModel> GetAllFinanceFifthLvlAccountsModel()
        {
            var query = from fifth in dbContext.FinanceFifthLvlAccounts
                        join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                        join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                        join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                        select new FinanceFifthLvlAccountModel
                        {
                            AccountDescription = fifth.AccountDescription,
                            AccountName = fifth.AccountName,
                            FourthLvlAccountId = fourth.Id,
                            BranchId = fourth.BranchId,
                            FirstLvlAccountId = second.FirstLvlAccountId,
                            FinanceFourthLvlAccountAccountName = fourth.AccountName,
                            Id = fifth.Id,
                            Value = (int)(fifth.Value == null ? 0 : fifth.Value),
                            SeccondLvlAccountId = second.Id,
                            ThirdLvlAccountId = third.Id,
                            IsPettyCashAccount = fifth.IsPettyCashAccount ?? false
                        };
            return query.ToList();
            //return dbContext.FinanceFourthLvlAccounts.ToList();
        }

        public void UpdateFinanceFifthLvlAccount(FinanceFifthLvlAccount fifthLvlAccount)
        {
            var SessionObj = GetFinanceFifthLvlAccountById(fifthLvlAccount.Id);
            if (fifthLvlAccount != null)
            {
                fifthLvlAccount.Count = SessionObj.Count;
                fifthLvlAccount.Value = SessionObj.Value;
                dbContext.Entry(SessionObj).CurrentValues.SetValues(fifthLvlAccount);
                //dbContext.Entry(fifthLvlAccount).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteFinanceFifthLvlAccount(FinanceFifthLvlAccount fifthLvlAccount)
        {
            if (fifthLvlAccount != null)
            {
                dbContext.FinanceFifthLvlAccounts.Remove(fifthLvlAccount);
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region JOURNAL_ENTRY_FUNCTIONS
        public int AddJurnalEntry(JournalEntry journalEntry)
        {
            int result = -1;
            if (journalEntry != null)
            {
                dbContext.JournalEntries.Add(journalEntry);
                dbContext.SaveChanges();
                result = journalEntry.EntryId;
            }

            return result;
        }

        public int UpdateJurnalEntry(JournalEntry journalEntry)
        {
            int result = -1;
            if (journalEntry != null)
            {
                dbContext.Entry(journalEntry).State = EntityState.Modified;
                dbContext.SaveChanges();
                result = journalEntry.EntryId;
            }

            return result;
        }

        public JournalEntry GetJournalEntry(int entryId)
        {
            return dbContext.JournalEntries.Find(entryId);
        }

        public JournalEntryModel GetJournalEntryModel(int entryId)
        {
            var query = from entry in dbContext.JournalEntries
                        where entry.EntryId == entryId
                        select new JournalEntryModel
                        {
                            BranchId = entry.BranchId,
                            ChequeNo = entry.ChequeNo,
                            CreatedOn = entry.CreatedOn,
                            CreditAmount = entry.CreditAmount,
                            CreditDescription = entry.CreditDescription,
                            DebitAmount = entry.DebitAmount,
                            DebitDescription = entry.DebitDescription,
                            EntryId = entry.EntryId,
                            EntryType = entry.EntryType,
                            IsDeleted = entry.IsDeleted,
                            LongDescription = entry.LongDescription,
                            SystemGenerated = entry.SystemGenerated
                        };

            var journalEntry = query.FirstOrDefault();

            var debitQuery = from detail in dbContext.JournalEntryDebitDetails
                             join account in dbContext.FinanceFifthLvlAccounts on detail.FifthLvlAccountId equals account.Id
                             where detail.EntryId == entryId
                             select new JournalEntryDetailModel
                             {
                                 Amount = detail.Amount,
                                 Count = detail.Count,
                                 Description = detail.Description,
                                 EntryId = detail.EntryId,
                                 FifthLvlAccountId = detail.FifthLvlAccountId,
                                 FifthLvlLvlAccountName = account.AccountName,
                                 ID = detail.ID
                             };

            var creditQuery = from detail in dbContext.JournalEntryCreditDetails
                             join account in dbContext.FinanceFifthLvlAccounts on detail.FifthLvlAccountId equals account.Id
                             where detail.EntryId == entryId
                             select new JournalEntryDetailModel
                             {
                                 Amount = detail.Amount,
                                 Count = detail.Count,
                                 Description = detail.Description,
                                 EntryId = detail.EntryId,
                                 FifthLvlAccountId = detail.FifthLvlAccountId,
                                 FifthLvlLvlAccountName = account.AccountName,
                                 ID = detail.ID
                             };

            journalEntry.JournalEntryCreditDetails = creditQuery.ToList();
            journalEntry.JournalEntryDebitDetails = debitQuery.ToList();

            return journalEntry;
        }

        public void DeleteJournalEntry(JournalEntry entry)
        {
            dbContext.JournalEntries.Remove(entry);
        }

        public IList<JournalEntry> SearchJournalEntries(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount, 
            int FinanceSeccondLvlAccount, int FinanceThirdLvlAccount, int FinanceFourthLvlAccount, int FinanceFifthLvlAccount, int FinanceMode, int branchId )
        {
            List<JournalEntry> entryList = null;
            if (FinanceMode == 2)
            {
                entryList = (from entry in dbContext.JournalEntries
                                join debitDetail in dbContext.JournalEntryDebitDetails
                                    on entry.EntryId equals debitDetail.EntryId
                                where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                    && (FinanceFirstLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                    && (FinanceSeccondLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                    && (FinanceThirdLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                    && (FinanceFourthLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                    && (FinanceFifthLvlAccount == 0 || debitDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                             select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();
            }
            else if (FinanceMode == 1)
            {
                entryList = (from entry in dbContext.JournalEntries
                             join creditDetail in dbContext.JournalEntryCreditDetails
                                 on entry.EntryId equals creditDetail.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                 && (FinanceFirstLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFifthLvlAccount == 0 || creditDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                             select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();
            }
            else if (FinanceMode == 3)
            {
                entryList = (from entry in dbContext.JournalEntries
                             join debitDetail in dbContext.JournalEntryDebitDetails
                                 on entry.EntryId equals debitDetail.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                 && (FinanceFirstLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFifthLvlAccount == 0 || debitDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                             select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();

                List<JournalEntry> entryList1 = null;

                entryList1 = (from entry in dbContext.JournalEntries
                             join creditDetail in dbContext.JournalEntryCreditDetails
                                 on entry.EntryId equals creditDetail.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                 && (FinanceFirstLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFifthLvlAccount == 0 || creditDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                              select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();

                entryList1.Concat(entryList);

                entryList = (List<JournalEntry>)entryList1.Distinct().ToList();
            }
            return entryList;
        }

        public IList<JournalEntry> SearchPettyCashJournalEntries(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount,
            int FinanceSeccondLvlAccount, int FinanceThirdLvlAccount, int FinanceFourthLvlAccount, int FinanceFifthLvlAccount, int FinanceMode, int branchId)
        {
            List<JournalEntry> entryList = null;
            if (FinanceMode == 2)
            {
                entryList = (from entry in dbContext.JournalEntries
                             join debitDetail in dbContext.JournalEntryDebitDetails
                                 on entry.EntryId equals debitDetail.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                 && (FinanceFirstLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFifthLvlAccount == 0 || debitDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                 && entry.IsPettyCash == true
                                 && entry.BranchId == branchId
                             select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();
            }
            else if (FinanceMode == 1)
            {
                entryList = (from entry in dbContext.JournalEntries
                             join creditDetail in dbContext.JournalEntryCreditDetails
                                 on entry.EntryId equals creditDetail.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                 && (FinanceFirstLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFifthLvlAccount == 0 || creditDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                                 && entry.IsPettyCash == true
                             select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();
            }
            else if (FinanceMode == 3)
            {
                entryList = (from entry in dbContext.JournalEntries
                             join debitDetail in dbContext.JournalEntryDebitDetails
                                 on entry.EntryId equals debitDetail.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                 && (FinanceFirstLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || debitDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                 && (FinanceFifthLvlAccount == 0 || debitDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                                 && entry.IsPettyCash == true
                             select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();

                List<JournalEntry> entryList1 = null;

                entryList1 = (from entry in dbContext.JournalEntries
                              join creditDetail in dbContext.JournalEntryCreditDetails
                                  on entry.EntryId equals creditDetail.EntryId
                              where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate)
                                  && (FinanceFirstLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId == FinanceFirstLvlAccount)
                                  && (FinanceSeccondLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId == FinanceFirstLvlAccount)
                                  && (FinanceThirdLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FinanceFourthLvlAccount.ThirdLvlAccountId == FinanceFirstLvlAccount)
                                  && (FinanceFourthLvlAccount == 0 || creditDetail.FinanceFifthLvlAccount.FourthLvlAccountId == FinanceFirstLvlAccount)
                                  && (FinanceFifthLvlAccount == 0 || creditDetail.FifthLvlAccountId == FinanceFirstLvlAccount)
                                     && entry.BranchId == branchId
                                  && entry.IsPettyCash == true
                              select entry).OrderByDescending(entry => entry.EntryId).Distinct().ToList();

                entryList1.Concat(entryList);

                entryList = (List<JournalEntry>)entryList1.Distinct().ToList();
            }
            return entryList;
        }


        public IList<JournalEntry> SearchJournalVoucher(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount,
            int FinanceSeccondLvlAccount, int FinanceThirdLvlAccount, int FinanceFourthLvlAccount, int FinanceFifthLvlAccount, int JvNo, int branchId)
        {
            List<JournalEntry> entryList = null;
            if (JvNo > 0)
            {
                entryList = (from entry in dbContext.JournalEntries
                             join voucher in dbContext.JournalVouchers
                                 on entry.EntryId equals voucher.EntryId
                             where voucher.Id == JvNo && voucher.IsCleared == 0
                                    && entry.BranchId == branchId
                             select entry).Distinct().ToList();
            }
            else
            {
                entryList = (from entry in dbContext.JournalEntries
                             join voucher in dbContext.JournalVouchers
                                 on entry.EntryId equals voucher.EntryId
                             where (entry.CreatedOn >= FromDate && entry.CreatedOn <= ToDate) && voucher.IsCleared == 0
                                 && (FinanceFirstLvlAccount == 0 || entry.DebitFirstLvlId == FinanceFirstLvlAccount)
                                 && (FinanceSeccondLvlAccount == 0 || entry.DebitSeccondLvlId == FinanceFirstLvlAccount)
                                 && (FinanceThirdLvlAccount == 0 || entry.DebitThirdLvlId == FinanceFirstLvlAccount)
                                 && (FinanceFourthLvlAccount == 0 || entry.DebitFourthLvlId == FinanceFirstLvlAccount)
                                    && entry.BranchId == branchId
                             select entry).Distinct().ToList();
            }
            return entryList;
        }
        public int GetMaxVouxherId()
        {
            int count = 1;
            if (dbContext.JournalEntries.ToList().Count > 0)
                count = dbContext.JournalEntries.Max(x => x.EntryId) + 1;
            return count;
        }
        #endregion

        #region JOURNAL_ENTRY_DEBIT_DETAILS_FUNCTIONS
        public int AddJurnalEntryDebitDetail(JournalEntryDebitDetail journalEntryDebitDetail)
        {
            int result = -1;
            if (journalEntryDebitDetail != null)
            {
                dbContext.JournalEntryDebitDetails.Add(journalEntryDebitDetail);
                dbContext.SaveChanges();
                result = journalEntryDebitDetail.ID;
            }

            return result;
        }

        public int AddJurnalEntryCreditDetail(JournalEntryCreditDetail journalEntryCreditDetail)
        {
            int result = -1;
            if (journalEntryCreditDetail != null)
            {
                dbContext.JournalEntryCreditDetails.Add(journalEntryCreditDetail);
                dbContext.SaveChanges();
                result = journalEntryCreditDetail.ID;
            }

            return result;
        }

        public IList<JournalEntryDebitDetail> GetJounralEntryDebitDetail(int entryId)
        {
            return dbContext.JournalEntryDebitDetails.Where(x => x.EntryId == entryId).ToList();
        }

        public IList<JournalEntryCreditDetail> GetJounralEntryCreditDetail(int entryId)
        {
            return dbContext.JournalEntryCreditDetails.Where(x => x.EntryId == entryId).ToList();
        }

        public void DeleteJounralEntryDebitDetail(JournalEntryDebitDetail journalEntryDebitDetail)
        {
            if (journalEntryDebitDetail != null)
            {
                dbContext.JournalEntryDebitDetails.Remove(journalEntryDebitDetail);
                dbContext.SaveChanges();
            }
        }

        public void DeleteJounralEntryCreditDetail(JournalEntryCreditDetail journalEntryCreditDetail)
        {
            if (journalEntryCreditDetail != null)
            {
                dbContext.JournalEntryCreditDetails.Remove(journalEntryCreditDetail);
                dbContext.SaveChanges();
            }
        }


        #endregion

        #region Finance Accounts

        public List<FinanceFifthLvlAccount> GetFeeFinanceAccounts(int branchId)
        {
            List<FinanceFifthLvlAccount> list = GetFinanceAccountsFifthLvl("Bank");
            List<FinanceFifthLvlAccount> list1 = GetFinanceAccountsFifthLvl("Cash");
            if(list1.Count > 0)
                list = list.Concat(list1).ToList();
            //list = (List<FinanceFifthLvlAccount>)list.Concat(list1);
            return list;
        }

        public List<FinanceFifthLvlAccountModel> GetFeeFinanceAccountsModel(int branchId)
        {
            List<FinanceFifthLvlAccountModel> list = GetFinanceAccountsFifthLvlModel("Bank");
            List<FinanceFifthLvlAccountModel> list1 = GetFinanceAccountsFifthLvlModel("Cash");
            if (list1.Count > 0)
                list = list.Concat(list1).ToList();
            //list = (List<FinanceFifthLvlAccount>)list.Concat(list1);
            return list;
        }

        public List<FinanceFourthLvlAccount> GetFinanceAccounts(string accountName, int branchId)
        {
            int thirdLvlAccount = dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == accountName).FirstOrDefault().Id;
            return dbContext.FinanceFourthLvlAccounts.Where(x => x.ThirdLvlAccountId == thirdLvlAccount && x.BranchId == branchId).ToList();
        }

        public List<FinanceFifthLvlAccount> GetFinanceAccountsFifthLvl(string accountName, int branchId, int level = 3)
        {
            List<FinanceFifthLvlAccount> accountList = null;
            if (level == 3)
            {
                int thirdLvlAccount = dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == accountName && x.BranchId == 1).FirstOrDefault().Id;
                accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FinanceFourthLvlAccount.ThirdLvlAccountId == thirdLvlAccount
                && x.BranchId == branchId && x.IsPettyCashAccount == false).ToList();
            }
            else
            {
                int fourthLvlAccount = dbContext.FinanceFourthLvlAccounts.Where(x => x.AccountName == accountName && x.BranchId == branchId).FirstOrDefault().Id;
                accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FourthLvlAccountId == fourthLvlAccount
                && x.BranchId == branchId && x.IsPettyCashAccount == false).ToList();
            }
            return accountList;
        }

        public List<FinanceFifthLvlAccount> GetPettyCashFinanceAccountsFifthLvl(string accountName, int branchId, int level = 3)
        {
            List<FinanceFifthLvlAccount> accountList = null;
            if (level == 3)
            {
                int thirdLvlAccount = dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == accountName && x.BranchId == 1).FirstOrDefault().Id;
                accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FinanceFourthLvlAccount.ThirdLvlAccountId == thirdLvlAccount 
                && x.BranchId == branchId && x.IsPettyCashAccount == true).ToList();
            }
            else
            {
                int fourthLvlAccount = dbContext.FinanceFourthLvlAccounts.Where(x => x.AccountName == accountName && x.BranchId == branchId).FirstOrDefault().Id;
                accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FourthLvlAccountId == fourthLvlAccount 
                && x.BranchId == branchId && x.IsPettyCashAccount == true).ToList();
            }
            return accountList;
        }

        public List<FinanceFifthLvlAccount> GetFinanceAccountsFifthLvl(string accountName)
        {
            int level = 3;
            List<FinanceFifthLvlAccount> accountList = null;
            if (level == 3)
            {
                int thirdLvlAccount = dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == accountName).FirstOrDefault().Id;
                accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FinanceFourthLvlAccount.ThirdLvlAccountId == thirdLvlAccount).ToList();
            }
            else
            {
                int fourthLvlAccount = dbContext.FinanceFourthLvlAccounts.Where(x => x.AccountName == accountName).FirstOrDefault().Id;
                accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FourthLvlAccountId == fourthLvlAccount).ToList();
            }
            return accountList;
        }

        public List<FinanceFifthLvlAccountModel> GetFinanceAccountsFifthLvlModel(string accountName)
        {
            int level = 3;
            List<FinanceFifthLvlAccountModel> accountList = null;
            if (level == 3)
            {
                int thirdLvlAccount = dbContext.FinanceThirdLvlAccounts.Where(x => x.AccountName == accountName).FirstOrDefault().Id;

                var query = from fifth in dbContext.FinanceFifthLvlAccounts
                            join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                            join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                            join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                            join first in dbContext.FinanceFirstLvlAccounts on second.FirstLvlAccountId equals first.Id
                            where third.Id == thirdLvlAccount
                            select new FinanceFifthLvlAccountModel
                            {
                                Id = fifth.Id,
                                AccountDescription = fifth.AccountDescription,
                                AccountName = fifth.AccountName,
                                BranchId = fifth.BranchId,
                                FirstLvlAccountId = first.Id,
                                FourthLvlAccountId = fourth.Id,
                                SeccondLvlAccountId = second.Id,
                                ThirdLvlAccountId = third.Id
                            };
                accountList = query.ToList();
                //accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FinanceFourthLvlAccount.ThirdLvlAccountId == thirdLvlAccount).ToList();
            }
            else
            {
                int fourthLvlAccount = dbContext.FinanceFourthLvlAccounts.Where(x => x.AccountName == accountName).FirstOrDefault().Id;
                var query = from fifth in dbContext.FinanceFifthLvlAccounts
                            join fourth in dbContext.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                            join third in dbContext.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                            join second in dbContext.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                            join first in dbContext.FinanceFirstLvlAccounts on second.FirstLvlAccountId equals first.Id
                            where fourth.Id == fourthLvlAccount
                            select new FinanceFifthLvlAccountModel
                            {
                                Id = fifth.Id,
                                AccountDescription = fifth.AccountDescription,
                                AccountName = fifth.AccountName,
                                BranchId = fifth.BranchId,
                                FirstLvlAccountId = first.Id,
                                FourthLvlAccountId = fourth.Id,
                                SeccondLvlAccountId = second.Id,
                                ThirdLvlAccountId = third.Id
                            };
                accountList = query.ToList();
                //accountList = dbContext.FinanceFifthLvlAccounts.Where(x => x.FourthLvlAccountId == fourthLvlAccount).ToList();
            }
            return accountList;
        }

        public void CreateJournalVoucher(JournalVoucher voucher)
        {
            dbContext.JournalVouchers.Add(voucher);
        }

        public void UpdateJournalVoucher(JournalVoucher voucher)
        {
            if (voucher != null)
            {
                dbContext.Entry(voucher).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public JournalVoucher GetJournalVoucher(int entryId)
        {
            return dbContext.JournalVouchers.Where(x => x.EntryId == entryId).FirstOrDefault();
        }
        #endregion

        public List<FinanceMode> GetAllFinanceModes()
        {
            return dbContext.FinanceModes.ToList();
        }

        public int GetEntriesCount(int FinanceAccountId)
        {
            int entryCount = dbContext.JournalEntryCreditDetails.Where(x => x.FifthLvlAccountId == FinanceAccountId).Count();
            if (entryCount == 0)
            {
                entryCount = dbContext.JournalEntryDebitDetails.Where(x => x.FifthLvlAccountId == FinanceAccountId).Count();
            }

            return entryCount;
        }

        public int GetIssueChallanCount(int FinanceAccountId)
        {
            return dbContext.IssuedChallans.Where(x => x.PayedTo == FinanceAccountId).Count();
        }

        public int GetFinanceSeccondLvlCount(int FinanceAccountId)
        {
            return dbContext.FinanceSeccondLvlAccounts.Where(x => x.FirstLvlAccountId == FinanceAccountId).Count();
        }

        public int GetFinanceThirdLvlCount(int FinanceAccountId)
        {
            return dbContext.FinanceThirdLvlAccounts.Where(x => x.SeccondLvlAccountId == FinanceAccountId).Count();
        }

        public int GetFinanceFourthLvlCount(int FinanceAccountId)
        {
            return dbContext.FinanceFourthLvlAccounts.Where(x => x.ThirdLvlAccountId == FinanceAccountId).Count();
        }

        public int GetFinanceFifthLvlCount(int FinanceAccountId)
        {
            return dbContext.FinanceFifthLvlAccounts.Where(x => x.FourthLvlAccountId == FinanceAccountId).Count();
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
