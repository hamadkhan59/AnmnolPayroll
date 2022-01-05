using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.Reports
{
    public class DAL_Finance_Reports:DAL.DABase
    {

        public DataSet GetFinanaceDetailSummary(bool isActiveAccounts, int branchId)
        {
            DataTable dt = new DataTable();
            
                //string sql = @"select fh.AccountName as Head, fsh.AccountName as SubHead, 
                //                fsht.AccountName as SubHeadType, ffa.AccountName as FourthLvlAccount, 
                //                case WHEN lag(ffa.value) over (order by ffa.Id) = ffa.value 
                //                and lag(ffa.Id) over (order by ffa.Id) = ffa.Id then 0 else ffa.Value end  as CreditFourthLvl,
                //                fifa.AccountName as FifthLvlAccount, 
                //                case when fifa.Value is null then '' else fifa.Value end as CreditFifthLvl

                //                from FinanceFirstLvlAccounts  fh Left  join  FinanceSeccondLvlAccounts fsh on fh.Id = fsh.FirstLvlAccountId
                //                left join FinanceThirdLvlAccounts fsht on fsh.Id = fsht.SeccondLvlAccountId 
                //                left join FinanceFourthLvlAccounts ffa on fsht.Id = ffa.ThirdLvlAccountId 
                //                left join FinanceFifthLvlAccounts fifa on ffa.Id = fifa.FourthLvlAccountId";

        //        string sql = @"select fh.AccountName as Head, fsh.AccountName as SubHead, 
        //                        fsht.AccountName as SubHeadType, ffa.AccountName as FourthLvlAccount, 
        //                        isnull((select sum(isnull(Amount, 0)) from JournalEntryDebitDetail jedd 
								//inner join FinanceFifthLvlAccounts fifth on jedd.FifthLvlAccountId = fifth.Id
								//where fifth.FourthLvlAccountId = ffa.Id), 0)
								//-
								//isnull((select sum(isnull(Amount, 0)) from JournalEntryCreditDetail jecd 
								//inner join FinanceFifthLvlAccounts fifth on jecd.FifthLvlAccountId = fifth.Id
								//where fifth.FourthLvlAccountId = ffa.Id),0)  as CreditFourthLvl,
        //                        fifa.AccountName as FifthLvlAccount, 
        //                        isnull((select sum(isnull(Amount, 0)) from JournalEntryDebitDetail where FifthLvlAccountId = fifa.Id), 0 )
								//-isnull(( select sum(isnull(Amount, 0)) from JournalEntryCreditDetail where FifthLvlAccountId = fifa.Id), 0) as  CreditFifthLvl

        //                        from FinanceFirstLvlAccounts  fh Left  join  FinanceSeccondLvlAccounts fsh on fh.Id = fsh.FirstLvlAccountId
        //                        left join FinanceThirdLvlAccounts fsht on fsh.Id = fsht.SeccondLvlAccountId 
        //                        left join FinanceFourthLvlAccounts ffa on fsht.Id = ffa.ThirdLvlAccountId 
        //                        left join FinanceFifthLvlAccounts fifa on ffa.Id = fifa.FourthLvlAccountId";

                string sql = @"select fh.AccountName as Head, fsh.AccountName as SubHead, 
                                fsht.AccountName as SubHeadType, ffa.AccountName as FourthLvlAccount, 
                                0  as CreditFourthLvl,
                                fifa.AccountName as FifthLvlAccount, 
                                isnull((select sum(isnull(Amount, 0)) from JournalEntryDebitDetail where FifthLvlAccountId = fifa.Id), 0 )
								-isnull(( select sum(isnull(Amount, 0)) from JournalEntryCreditDetail where FifthLvlAccountId = fifa.Id), 0) as  CreditFifthLvl

                                from FinanceFirstLvlAccounts  fh Left  join  FinanceSeccondLvlAccounts fsh on fh.Id = fsh.FirstLvlAccountId
                                left join FinanceThirdLvlAccounts fsht on fsh.Id = fsht.SeccondLvlAccountId 
                                left join FinanceFourthLvlAccounts ffa on fsht.Id = ffa.ThirdLvlAccountId 
                                left join FinanceFifthLvlAccounts fifa on ffa.Id = fifa.FourthLvlAccountId";



                sql = sql + " where fsht.BranchId = {0} and ffa.BranchId = {0} and fifa.BranchId = {0} ";

                if (isActiveAccounts)
                    sql = sql + @" (select count(*) from JournalEntryDebitDetail where FifthLvlAccountId = fifa.Id) > 0
                                    and (select count(*) from JournalEntryCreditDetail where FifthLvlAccountId = fifa.Id) > 0";

                sql = sql + "order by fh.Id, fsh.Id, fsht.Id, ffa.Id, fifa.Id";

                sql = string.Format(sql, branchId);
                return ExecuteDataSet(sql);   
            
        }

        public DataSet GetJournalEntryReportData(int firstLvlId, int seccondLvlId, int thirdLvlId, int fourthLvlId,
            int fifthLvlId, int mode, DateTime fromDate, DateTime todate, int entryId, int branchId)
        {
           
                var sqlQuery = @"EXEC	[dbo].[sp_JournalEntries]
		                            @FirstLvlId = {0},
		                            @SecondLvlId = {1},
		                            @ThirdLvlId = {2},
		                            @FourthLvlId = {3},
		                            @FifthLvlId = {4},
		                            @mode = {5},
		                            @fromDate = N'{6}',
		                            @toDate = N'{7}',
		                            @branchId = {9},
                                    @entryId = {8}";

                sqlQuery = string.Format(sqlQuery, firstLvlId, seccondLvlId, thirdLvlId, fourthLvlId, fifthLvlId, mode, getDate(fromDate), getDate(todate), entryId, branchId);
                return ExecuteDataSet(sqlQuery);   
        }

        public DataSet GetPettyCashEntryReportData(int firstLvlId, int seccondLvlId, int thirdLvlId, int fourthLvlId,
            int fifthLvlId, int mode, DateTime fromDate, DateTime todate, int entryId, int branchId)
        {

            var sqlQuery = @"EXEC	[dbo].[sp_PettyCashEntries]
		                            @FirstLvlId = {0},
		                            @SecondLvlId = {1},
		                            @ThirdLvlId = {2},
		                            @FourthLvlId = {3},
		                            @FifthLvlId = {4},
		                            @mode = {5},
		                            @fromDate = N'{6}',
		                            @toDate = N'{7}',
		                            @branchId = {9},
                                    @entryId = {8}";

            sqlQuery = string.Format(sqlQuery, firstLvlId, seccondLvlId, thirdLvlId, fourthLvlId, fifthLvlId, mode, getDate(fromDate), getDate(todate), entryId, branchId);
            return ExecuteDataSet(sqlQuery);
        }

        public DataSet GetPettyCashEntryData(int entryId, int branchId)
        {

            var sqlQuery = @"EXEC	[dbo].[sp_PettyCashEntries]
		                            @FirstLvlId = NULL,
		                            @SecondLvlId = NULL,
		                            @ThirdLvlId = NULL,
		                            @FourthLvlId = NULL,
		                            @FifthLvlId = NULL,
		                            @mode = NULL,
		                            @fromDate = NULL,
		                            @toDate = NULL,
		                            @branchId = {0},
                                    @entryId = {1}";

            sqlQuery = string.Format(sqlQuery, branchId, entryId);
            return ExecuteDataSet(sqlQuery);
        }

        private string getDate(DateTime date)
        {
            return date.Year + "-" + (date.Month.ToString().Length == 1 ? "0" + date.Month : date.Month.ToString())
                + "-" + (date.Day.ToString().Length == 1 ? "0" + date.Day : date.Day.ToString());
        }

        public DataSet GetJVReportData(int firstLvlId, int seccondLvlId, int thirdLvlId, int fourthLvlId, int fifthLvlId,
            int mode, DateTime fromDate, DateTime todate, int entryId, int branchId)
        {
            
                var sqlQuery = @"EXEC	[dbo].[sp_JVJournalEntries]
		                            @FirstLvlId = {0},
		                            @SecondLvlId = {1},
		                            @ThirdLvlId = {2},
		                            @FourthLvlId = {3},
		                            @FifthLvlId = {4},
		                            @mode = {5},
		                            @fromDate = N'{6}',
		                            @toDate = N'{7}',
		                            @branchId = {9},
                                    @entryId = {8}";


                sqlQuery = string.Format(sqlQuery, firstLvlId, seccondLvlId, thirdLvlId, fourthLvlId, fifthLvlId, mode, getDate(fromDate), getDate(todate), entryId, branchId);
            return ExecuteDataSet(sqlQuery);
        }

        public DataSet GetFinanaceLiabilityBookSummary(int branchId)
        {
            DataTable dt = new DataTable();
           
                string sql = @"select fh.AccountName as Head, fsh.AccountName as SubHead, 
                                fsht.AccountName as SubHeadType, ffa.AccountName as FourthLvlAccount, 
                                case WHEN lag(ffa.value) over (order by ffa.ID) = ffa.value then 0 else ffa.Value end  as CreditFourthLvl,
                                fifa.AccountName as FifthLvlAccount, case when fifa.Value is null then '' else fifa.Value end as CreditFifthLvl

                                from FinanceFirstLvlAccounts  fh Left  join  FinanceSeccondLvlAccounts fsh on fh.Id = fsh.FirstLvlAccountId
                                left join FinanceThirdLvlAccounts fsht on fsh.Id = fsht.SeccondLvlAccountId 
                                left join FinanceFourthLvlAccounts ffa on fsht.Id = ffa.ThirdLvlAccountId 
                                left join FinanceFifthLvlAccounts fifa on ffa.Id = fifa.FourthLvlAccountId

                                where fh.AccountName like 'Liability%' and fsht.BranchId = {0} and ffa.BranchId = {0} and fifa.BranchId = {0}";

                sql = string.Format(sql, branchId);
                return ExecuteDataSet(sql);
           
        }

        public DataSet GetFeeCollectionReportData(DateTime fromDate, DateTime toDate, int branchId)
        {
            
                //       string sql = @"select FORMAT (isCh.Id, '000000000') as EntryId, CONVERT(VARCHAR(10), PaidDate, 103) as CreatedOn, 'CV' as SubHeadType,
                //                       CONCAT('Fee COllection Against Voucher : ', FORMAT (isCh.Id, '000000000')) as DebitDescription,
                //                       CONVERT(varchar(10),Amount) as DbAmount, 'DR' as Cost, '' as DbCount,
                //                       (Select AccountName from FinanceFifthLvlAccounts where Id = PayedTo) as Head,
                //                       std.RollNumber as CreditFirstLvl, std.Name as CreditSeccondLvl, cls.Name as DebitFirstLvl, sec.Name as DebitSeccondLvl
                //                       from IssuedChallan isCh, ChallanStudentDetail chStd, Student std, Classes cls, Section sec, ClassSection clsec
                //                    where clsec.ClassSectionId = std.ClassSectionId and isCh.ChallanToStdId = chStd.Id and chStd.StdId = std.id and clsec.ClassId = cls.Id
                //                    and clsec.SectionId = sec.Id and PaidFlag = 1  and PaidDate >= '{0}' and PaidDate <= '{1}' and std.BranchId = {2}
                //order by Head";

                string sql = @"select LEFT(CONVERT(VARCHAR, ph.PaidDate, 120), 10) as CreatedOn, CONVERT(varchar(10),sum(ph.PayAmount)) as DbAmount,
								st.Name as CreditSeccondLvl, FORMAT (isCh.Id, '000000000') as EntryId, 'CV' as SubHeadType,
								CONCAT('Fee Collection Against Voucher : ', FORMAT (isCh.Id, '000000000'), ', For Month : ', isCh.ForMonth) as DebitDescription,
								'DR' as Cost, '' as DbCount, (Select AccountName from FinanceFifthLvlAccounts where Id = ph.PayedTo) as Head,
								st.RollNumber as CreditFirstLvl, st.Name as CreditSeccondLvl, cls.Name as DebitFirstLvl, sec.Name as DebitSeccondLvl
                                from paymenthistory ph inner join FeeBalance fb on ph.FeeBalanceId = fb.Id
								inner join IssuedChallan isCh on ph.IssueChallanId = isCh.Id
                                inner join Student st on fb.StudentId = st.id
                                inner join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
                                inner join Classes cls on clsec.ClassId = cls.Id
                                inner join Section sec on clsec.SectionId = sec.Id
                                where  st.BranchId = 1
                                and CAST(ph.PaidDate as date) >= '{0}' and CAST(ph.PaidDate as date) <= '{1}' 
                                group by st.Contact_1, st.RollNumber, st.Name, cls.Name, sec.Name, st.FatherName, st.AdmissionNo, 
                                LEFT(CONVERT(VARCHAR, ph.PaidDate, 120), 10), ph.ForMonth, fb.Advance, fb.Balance, isCh.Id, ph.PayedTo, isCh.ForMonth
                                order By Head, CreatedOn desc";

                //query with fine
                //       string sql = @"select FORMAT (isCh.Id, '000000000') as EntryId, PaidDate as CreatedOn, 'CV' as SubHeadType,
                //                       CONCAT('Fee COllection Against Voucher : ', FORMAT (isCh.Id, '000000000')) as DebitDescription,
                //                       CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                //                       (Select AccountName from FinanceFifthLvlAccounts where Id = PayedTo) as Head,
                //                       std.RollNumber as CreditFirstLvl, std.Name as CreditSeccondLvl, cls.Name as DebitFirstLvl, sec.Name as DebitSeccondLvl
                //                       from IssuedChallan isCh, ChallanStudentDetail chStd, Student std, Classes cls, Section sec, ClassSection clsec
                //                    where clsec.ClassSectionId = std.ClassSectionId and isCh.ChallanToStdId = chStd.Id and chStd.StdId = std.id and clsec.ClassId = cls.Id
                //                    and clsec.SectionId = sec.Id and PaidFlag = 1  and PaidDate >= '{0}' and PaidDate <= '{1}' and std.BranchId = {2}
                //order by Head";

                //       string sql = @"select FORMAT (isCh.Id, '000000000') as EntryId, PaidDate as CreatedOn, 'CV' as SubHeadType,
                //                       CONCAT('Fee COllection Against Voucher : ', FORMAT (isCh.Id, '000000000')) as DebitDescription,
                //                       CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                //                       (Select AccountName from FinanceFifthLvlAccounts where Id = PayedTo) as Head,
                //                       std.RollNumber as CreditFirstLvl, std.Name as CreditSeccondLvl, cls.Name as DebitFirstLvl, sec.Name as DebitSeccondLvl
                //                       from IssuedChallan isCh, ChallanStudentDetail chStd, Student std, Classes cls, Section sec, ClassSection clsec
                //                    where clsec.ClassSectionId = std.ClassSectionId and isCh.ChallanToStdId = chStd.Id and chStd.StdId = std.id and clsec.ClassId = cls.Id
                //                    and clsec.SectionId = sec.Id and PaidFlag = 1  and PaidDate >= '{0}' and PaidDate <= '{1}' and std.BranchId = {2}
                //union
                //select FORMAT (isCh.Id, '000000000') as EntryId, PaidDate as CreatedOn, 'CV' as SubHeadType,
                //                       CONCAT('Fee COllection Against Voucher : ', FORMAT (isCh.Id, '000000000')) as DebitDescription,
                //                       CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                //                       'Cash in Hand' as Head, std.RollNumber as CreditFirstLvl, std.Name as CreditSeccondLvl, cls.Name as DebitFirstLvl, sec.Name as DebitSeccondLvl
                //                       from IssuedChallan isCh, ChallanStudentDetail chStd, Student std, Classes cls, Section sec, ClassSection clsec
                //                    where clsec.ClassSectionId = std.ClassSectionId and isCh.ChallanToStdId = chStd.Id and chStd.StdId = std.id and clsec.ClassId = cls.Id
                //                    and clsec.SectionId = sec.Id and PaidFlag = 1 and PaidDate >= '{0}' and PaidDate <= '{1}' and std.BranchId = {2}
                //                                                       and PayedTo = 0
                //order by Head";
                //order by CreatedOn desc";
                sql = string.Format(sql, getDate(fromDate), getDate(toDate), branchId);
                return ExecuteDataSet(sql);

        }

        public DataSet GetGeneralLedgerBookData(DateTime fromDate, DateTime toDate, int branchId, string accountName = "")
        {
            
                string sql = @"select distinct CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                            'CV' as SubHeadType, jedd.Description as DebitDescription,  '' as DbAmount,  'CR' as Cost, CONVERT(varchar(10), jedd.Amount) as DbCount,  
                            fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0)  as CreditFirstLvl
                                from JournalEntry je left outer join JournalEntryCreditDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								where  je.IsPettyCash = 0 and CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{2}%' and je.BranchId = {3}
								union
	                            select CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
							'CV' as SubHeadType, jedd.Description as DebitDescription,  CONVERT(varchar(10), jedd.Amount) as DbAmount, 'DB' as Cost,  '' as DbCount,  
							fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0)  as CreditFirstLvl
								from JournalEntry je left outer join JournalEntryDebitDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								where  je.IsPettyCash = 0 and CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{2}%' and je.BranchId = {3}
								--and (select count(*) from JournalEntryDebitDetail jedd where jedd.EntryId = je.EntryId) > 0
                            order by Head desc, CreatedOn desc";
                //order by CreatedOn desc";

                sql = string.Format(sql, getDate(fromDate), getDate(toDate), accountName, branchId);
                return ExecuteDataSet(sql);
            
        }

        public DataSet GetPettyCashLedgerBookData(DateTime fromDate, DateTime toDate, int branchId, string accountName = "")
        {

            string sql = @"select distinct CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                            'CV' as SubHeadType, jedd.Description as DebitDescription,  '' as DbAmount,  'CR' as Cost, CONVERT(varchar(10), jedd.Amount) as DbCount,  
                            fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0)  as CreditFirstLvl
                                from JournalEntry je left outer join JournalEntryCreditDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								where je.IsPettyCash = 1 and CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{2}%' and je.BranchId = {3}
								union
	                            select CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
							'CV' as SubHeadType, jedd.Description as DebitDescription,  CONVERT(varchar(10), jedd.Amount) as DbAmount, 'DB' as Cost,  '' as DbCount,  
							fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0)  as CreditFirstLvl
								from JournalEntry je left outer join JournalEntryDebitDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								where  je.IsPettyCash = 1 and CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{2}%' and je.BranchId = {3}
								--and (select count(*) from JournalEntryDebitDetail jedd where jedd.EntryId = je.EntryId) > 0
                            order by Head desc, CreatedOn desc";
            //order by CreatedOn desc";

            sql = string.Format(sql, getDate(fromDate), getDate(toDate), accountName, branchId);
            return ExecuteDataSet(sql);

        }
        public DataSet GetBankBookData(int paidTo, DateTime fromDate, DateTime toDate, string bankName, int branchId)
        {
            paidTo = paidTo == 0 ? -1 : paidTo;
            bankName = bankName == "All" ? "" : bankName;
            DataTable dt = new DataTable();
            
                //                string sql = @"select FORMAT (Id, '000000000') as EntryId, FORMAT (Id, '000000000') as ChequeNo,PaidDate as CreatedOn, 'CV' as SubHeadType,
                //                                CONCAT('Fee COllection Against Voucher : ', FORMAT (Id, '000000000')) as DebitDescription,
                //                                CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                //                                case when PayedTo = 0 then 'Account Office' else (Select CONCAT(BankName, ' - ', AccountNo) from Bank where BankId = PayedTo) end as Head
                //                                 from IssuedChallan where PaidFlag = 1 and PaidDate >= '{0}' and PaidDate <= '{1}'
                //                                 and (0 in ({2}) or PayedTo in ({2})) and PayedTo <> 0
                //                                union
                //	                            select FORMAT (EntryId, '000000000') as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                //                            'CV' as SubHeadType, je.CreditDescription as DebitDescription,  '' as DbAmount, 'CR' as Cost, CONVERT(varchar(10), je.CreditAmount) as DbCount,  
                //                            (Select concat(AccountName, ' - ', AccountDescription) from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId) as Head
                //                             from JournalEntry je where CreditThirdLvlId = (SELECT  [Id]
                //                              FROM [FinanceThirdLvlAccounts]
                //                              where AccountName like 'Bank') and CreatedOn >= '{0}' and CreatedOn <= '{1}'
                //                                and (0 in ({2}) or (Select BankId from Bank where AccountNo = (Select AccountDescription from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId)
                //                                and BankName = (Select AccountName from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId)) in ({2})) 
                //                                 union
                //	                            select FORMAT (EntryId, '000000000') as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                //                            'CV' as SubHeadType, je.DebitDescription as DebitDescription,  CONVERT(varchar(10), je.DebitAmount) as DbAmount, 'CR' as Cost, '' as DbCount,  
                //                            (Select concat(AccountName, ' - ', AccountDescription) from FinanceFourthLvlAccounts where Id = je.DebitFourthLvlId) as Head
                //                             from JournalEntry je where DebitThirdLvlId = (SELECT  [Id]
                //                              FROM [FinanceThirdLvlAccounts]
                //                              where AccountName like 'Bank') and CreatedOn >= '{0}' and CreatedOn <= '{1}'
                //                                and (0 in ({2}) or (Select BankId from Bank where AccountNo = (Select AccountDescription from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId)
                //                                and BankName = (Select AccountName from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId)) in ({2})) 
                //                              order by CreatedOn desc";
                string sql = @"select CONCAT('JE_', FORMAT (Id, '000000')) as EntryId, FORMAT (Id, '000000000') as ChequeNo,PaidDate as CreatedOn, 'CV' as SubHeadType,
                                CONCAT('Fee COllection Against Voucher : ', FORMAT (Id, '000000000')) as DebitDescription,
                                CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                                case when PayedTo = 0 then 'Account Office' else (Select CONCAT(BankName, ' - ', AccountNo) from Bank where BankId = PayedTo) end as Head,
                                (select sum(Amount) from IssuedChallan where PayedTo = isch.PayedTo and PaidDate <= '{0}') as CreditFirstLvl
                                 from IssuedChallan isch where PaidFlag = 1 and CAST( PaidDate as date) >= '{0}' and CAST( PaidDate as date) <= '{1}'
                                 and (-1 in ({2}) or PayedTo in ({2}))  and isch.BranchId = {4}
                                union
	                            select distinct CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                            'CV' as SubHeadType, jedd.Description as DebitDescription,  '' as DbAmount,  'CR' as Cost, CONVERT(varchar(10), jedd.Amount) as DbCount,  
                            fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0)  as CreditFirstLvl
                                from JournalEntry je left outer join JournalEntryCreditDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								inner join FinanceFourthLvlAccounts fofa on fofa.Id = fifa.FourthLvlAccountId
								inner join FinanceThirdLvlAccounts fta on fta.Id = fofa.ThirdLvlAccountId
								where  CAST( je.CreatedOn as date) >= '{0}' and je.CreatedOn <= '{1}' and fifa.AccountName like '%{3}%' and fta.AccountName like '%Bank%' and je.BranchId = {4}
								union
	                            select CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
							'CV' as SubHeadType, jedd.Description as DebitDescription,  CONVERT(varchar(10), jedd.Amount) as DbAmount, 'DB' as Cost,  '' as DbCount,  
							fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0)  as CreditFirstLvl
								from JournalEntry je left outer join JournalEntryDebitDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								inner join FinanceFourthLvlAccounts fofa on fofa.Id = fifa.Id
								inner join FinanceThirdLvlAccounts fta on fta.Id = fofa.ThirdLvlAccountId
								where  CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{3}%' and fta.AccountName like '%Bank%'
                                and je.BranchId = {4}
								--and (select count(*) from JournalEntryDebitDetail jedd where jedd.EntryId = je.EntryId) > 0
                            order by Head desc, CreatedOn desc";

                sql = string.Format(sql, getDate(fromDate), getDate(toDate), paidTo, bankName, branchId);
                return ExecuteDataSet(sql);
            
        }


        public DataSet GetCashBookData(DateTime fromDate, DateTime toDate, int paidTo, string cashName, int branchId)
        {
            paidTo = paidTo == 0 ? -1 : paidTo;
            cashName = cashName == "All" ? "" : cashName;

            DataTable dt = new DataTable();
           
                //                string sql = @"select FORMAT (Id, '000000000') as EntryId, PaidDate as CreatedOn, 'CV' as SubHeadType,
                //                                CONCAT('Fee COllection Against Voucher : ', FORMAT (Id, '000000000')) as DebitDescription,
                //                                CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                //                                'Cash in Hand' as Head
                //                                    from IssuedChallan where PaidFlag = 1 and PaidDate >= '{0}' and PaidDate <= '{1}'
                //                                    and PayedTo = 0
                //                            union
                //	                            select FORMAT (EntryId, '000000000') as EntryId, je.CreatedOn as CreatedOn, 
                //                            'CV' as SubHeadType, je.CreditDescription as DebitDescription,  '' as DbAmount, 'CR' as Cost, CONVERT(varchar(10), je.CreditAmount) as DbCount,  
                //                            (Select AccountName from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId) as Head
                //                                from JournalEntry je where CreditFourthLvlId = (Select Id from FinanceFourthLvlAccounts where AccountName = 'Cash in Hand')
                //	                            and CreatedOn >= '{0}' and CreatedOn <= '{1}'
                //                            union
                //	                            select FORMAT (EntryId, '000000000') as EntryId, je.CreatedOn as CreatedOn, 
                //                            'CV' as SubHeadType, je.DebitDescription as DebitDescription,  CONVERT(varchar(10), je.DebitAmount) as DbAmount, 'CR' as Cost,  '' as DbCount,  
                //                            (Select AccountName from FinanceFourthLvlAccounts where Id = je.CreditFourthLvlId) as Head
                //                                from JournalEntry je where DebitFourthLvlId = (Select Id from FinanceFourthLvlAccounts where AccountName = 'Cash in Hand')
                //	                            and CreatedOn >= '{0}' and CreatedOn <= '{1}'
                //                            order by CreatedOn desc";
                string sql = @"select FORMAT (Id, '000000000') as EntryId, FORMAT (Id, '000000000') as ChequeNo,PaidDate as CreatedOn, 'CV' as SubHeadType,
                                CONCAT('Fee COllection Against Voucher : ', FORMAT (Id, '000000000')) as DebitDescription,
                                CONVERT(varchar(10),Amount + (case when Fine is null then 0 else Fine end)) as DbAmount, 'DR' as Cost, '' as DbCount,
                                case when PayedTo = 0 then 'Account Office' else (Select CONCAT(BankName, ' - ', AccountNo) from Bank where BankId = PayedTo) end as Head,
                                (select sum(Amount) from IssuedChallan where PayedTo = isch.PayedTo and PaidDate <= '{0}') as CreditFirstLvl
                                 from IssuedChallan isch where PaidFlag = 1 and CAST( PaidDate as date) >= '{0}' and CAST( PaidDate as date) <= '{1}'
                                 and (-1 in ({2}) or PayedTo in ({2}))  and isch.BranchId = {4}
                                union
	                            select distinct CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                            'CV' as SubHeadType, jedd.Description as DebitDescription,  '' as DbAmount,  'CR' as Cost, CONVERT(varchar(10), jedd.Amount) as DbCount,  
                            fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0)  as CreditFirstLvl
                                from JournalEntry je left outer join JournalEntryCreditDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								inner join FinanceFourthLvlAccounts fofa on fofa.Id = fifa.FourthLvlAccountId
								inner join FinanceThirdLvlAccounts fta on fta.Id = fofa.ThirdLvlAccountId
								where  CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{3}%' and fta.AccountName like '%Cash%'
                                and je.BranchId = {4}
								union
	                            select CONCAT(je.EntryType, '_', FORMAT (je.EntryId, '000000')) as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
							'CV' as SubHeadType, jedd.Description as DebitDescription,  CONVERT(varchar(10), jedd.Amount) as DbAmount, 'DB' as Cost,  '' as DbCount,  
							fifa.AccountName as Head, isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where ent.BranchId = {4} and FifthLvlAccountId =  jedd.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn <= '{0}'),0)  as CreditFirstLvl
								from JournalEntry je left outer join JournalEntryDebitDetail jedd on je.EntryId = jedd.EntryId
								inner join FinanceFifthLvlAccounts fifa on jedd.FifthLvlAccountId = fifa.Id
								inner join FinanceFourthLvlAccounts fofa on fofa.Id = fifa.FourthLvlAccountId
								inner join FinanceThirdLvlAccounts fta on fta.Id = fofa.ThirdLvlAccountId
								where  CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and fifa.AccountName like '%{3}%' and fta.AccountName like '%Cash%'
                                and je.BranchId = {4}
								--and (select count(*) from JournalEntryDebitDetail jedd where jedd.EntryId = je.EntryId) > 0
                            order by Head desc, CreatedOn desc";

                sql = string.Format(sql, getDate(fromDate), getDate(toDate), paidTo, cashName, branchId);
                //sql = string.Format(sql, fromDate, toDate);
                return ExecuteDataSet(sql);
            
        }

        public DataSet GetAccountQuantityBookData(int branchId)
        {
            
                string sql = @"select ftla.AccountName as FourthLvlAccount, ffla.AccountName as FifthLvlAccount, Count as DbCount 
                                from FinanceFifthLvlAccounts ffla, FinanceFourthLvlAccounts ftla
                                where ffla.FourthLvlAccountId = ftla.Id and  Count > 0
                                and ffla.BranchId = {0} and ftla.BranchId = {0}";

                sql = string.Format(sql, branchId);
                return ExecuteDataSet(sql);
            
        }

        public DataSet GetFeeCollectionJournalReport(DateTime fromDate, DateTime toDate, int branchId)
        {
            
                string sql = @"select FORMAT (je.EntryId, '000000000') as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                            'CV' as SubHeadType, ffa.AccountName as Head, jed.Description as DebitDescription, 
                            CONVERT(varchar(10),jed.Amount) as DbAmount, '' as DbCount, 'CR' as Cost, 
                            isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jed.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jed.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0)  as CreditFirstLvl
                                , '' as DbCount
                            from JournalEntryDebitDetail jed, JournalEntry je, FinanceFifthLvlAccounts ffa
                            where je.EntryId = jed.EntryId and ffa.Id = jed.FifthLvlAccountId
                            and  CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and jed.FifthLvlAccountId in 
							(select distinct PayedTo from IssuedChallan where PaidFlag = 1)
                            and jed.Description like 'Fee Amount is %'
                            and jed.Amount != 0
                            and je.BranchId = {2}

                            union

                            select FORMAT (je.EntryId, '000000000') as EntryId, je.ChequeNo, je.CreatedOn as CreatedOn, 
                            'CV' as SubHeadType, ffa.AccountName as Head, jed.Description as DebitDescription, 
                            '' as DbAmount, CONVERT(varchar(10),jed.Amount) as DbCount,   'DB' as Cost, 
                            isnull((select sum(amount) from JournalEntryDebitDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jed.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0) 
							-  isnull((select sum(amount) from JournalEntryCreditDetail jed, JournalEntry ent
							where FifthLvlAccountId =  jed.FifthLvlAccountId and jed.EntryId = ent.entryId and ent.CreatedOn < '{0}'),0)  as CreditFirstLvl
                                , '' as DbCount
                            from JournalEntryCreditDetail jed, JournalEntry je, FinanceFifthLvlAccounts ffa
                            where je.EntryId = jed.EntryId and ffa.Id = jed.FifthLvlAccountId
                            and  CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}' and jed.FifthLvlAccountId in 
							(select distinct PayedTo from IssuedChallan where PaidFlag = 1)
                            and jed.Description like 'Fee Amount is %'
                            and je.BranchId = {2}
                            and jed.Amount != 0
                            order by Head desc, CreatedOn desc";
                //order by CreatedOn desc";

                sql = string.Format(sql, getDate(fromDate), getDate(toDate), branchId);
                return ExecuteDataSet(sql);
            
        }


        public DataSet GetIncomeStatement(DateTime fromdate, DateTime todate, int Level, int branchId)
        {
            DataTable dt = new DataTable();
            

                string sql = @"EXEC	 [dbo].[sp_Income_Statement]
		                                @fromDate = N'{0}',
		                                @toDate = N'{1}',
                                        @branchId = {2}";

                sql = string.Format(sql, getDate(fromdate), getDate(todate), branchId);
                ExecuteNonQuery(sql);

                Level--;
                if (Level == 0)
                    sql = "select distinct FIRST_LVL_ACCOUNT as DebitFirstLvl, FIRST_LVL_AMOUNT as CostFirstLvl, FIRST_LVL_DB_AMOUNT, FIRST_LVL_CR_AMOUNT from IncomeStatement";
                else if (Level == 1)
                    sql = @"select distinct FIRST_LVL_ACCOUNT as DebitFirstLvl, FIRST_LVL_AMOUNT as CostFirstLvl, 
                            SECCOND_LVL_ACCOUNT as DebitSeccondLvl, SECCOND_LVL_AMOUNT AS CostSeccondLvl,
                            FIRST_LVL_DB_AMOUNT, FIRST_LVL_CR_AMOUNT, SECCOND_LVL_DB_AMOUNT, SECCOND_LVL_CR_AMOUNT
                             from IncomeStatement";
                else if (Level == 2)
                    sql = @"select distinct FIRST_LVL_ACCOUNT as DebitFirstLvl, 
                            case when FIRST_LVL_AMOUNT = lag(FIRST_LVL_AMOUNT) over (ORDER BY FIRST_LVL_AMOUNT) then 0 else FIRST_LVL_AMOUNT end as CostFirstLvl,
                            SECCOND_LVL_ACCOUNT as DebitSeccondLvl, SECCOND_LVL_AMOUNT AS CostSeccondLvl,
                            THIRD_LVL_ACCOUNT as DebitThirdLvl, THIRD_LVL_AMOUNT as CostThirdLvl,
                            case when FIRST_LVL_DB_AMOUNT = lag(FIRST_LVL_DB_AMOUNT) over (ORDER BY FIRST_LVL_ACCOUNT) then 0 else FIRST_LVL_DB_AMOUNT end as FIRST_LVL_DB_AMOUNT,
                            case when FIRST_LVL_CR_AMOUNT = lag(FIRST_LVL_CR_AMOUNT) over (ORDER BY FIRST_LVL_ACCOUNT) then 0 else FIRST_LVL_CR_AMOUNT end as FIRST_LVL_CR_AMOUNT,
                            SECCOND_LVL_DB_AMOUNT, SECCOND_LVL_CR_AMOUNT, 
                            THIRD_LVL_DB_AMOUNT,THIRD_LVL_CR_AMOUNT
                             from IncomeStatement";
                else if (Level == 3)
                    sql = @"select distinct FIRST_LVL_ACCOUNT as DebitFirstLvl, 
                            case when FIRST_LVL_AMOUNT = lag(FIRST_LVL_AMOUNT) over (ORDER BY FIRST_LVL_AMOUNT) then 0 else FIRST_LVL_AMOUNT end as CostFirstLvl,
                            SECCOND_LVL_ACCOUNT as DebitSeccondLvl, SECCOND_LVL_AMOUNT AS CostSeccondLvl,
                            THIRD_LVL_ACCOUNT as DebitThirdLvl, THIRD_LVL_AMOUNT as CostThirdLvl,
                            FOURTH_LVL_ACCOUNT as DebitFourthLvl, FOURTH_LVL_AMOUNT as CostFourthLvl,
                            case when FIRST_LVL_DB_AMOUNT = lag(FIRST_LVL_DB_AMOUNT) over (ORDER BY FIRST_LVL_ACCOUNT) then 0 else FIRST_LVL_DB_AMOUNT end as FIRST_LVL_DB_AMOUNT,
                            case when FIRST_LVL_CR_AMOUNT = lag(FIRST_LVL_CR_AMOUNT) over (ORDER BY FIRST_LVL_ACCOUNT) then 0 else FIRST_LVL_CR_AMOUNT end as FIRST_LVL_CR_AMOUNT,
                            SECCOND_LVL_DB_AMOUNT, SECCOND_LVL_CR_AMOUNT, 
                            THIRD_LVL_DB_AMOUNT,THIRD_LVL_CR_AMOUNT, FOURTH_LVL_DB_AMOUNT, FOURTH_LVL_CR_AMOUNT
                             from IncomeStatement";
                else if (Level == 4)
                    sql = @"select distinct FIRST_LVL_ACCOUNT as DebitFirstLvl, 
							case when FIRST_LVL_AMOUNT = lag(FIRST_LVL_AMOUNT) over (ORDER BY FIRST_LVL_AMOUNT) then 0 else FIRST_LVL_AMOUNT end as CostFirstLvl,
                            SECCOND_LVL_ACCOUNT as DebitSeccondLvl, SECCOND_LVL_AMOUNT AS CostSeccondLvl,
                            THIRD_LVL_ACCOUNT as DebitThirdLvl, THIRD_LVL_AMOUNT as CostThirdLvl,
                            FOURTH_LVL_ACCOUNT as DebitFourthLvl, FOURTH_LVL_AMOUNT as CostFourthLvl,
                            FIFTH_LVL_ACCOUNT as DebitFifthLvl, FIFTH_LVL_AMOUNT as CostFifthLvl,
                            case when FIRST_LVL_DB_AMOUNT = lag(FIRST_LVL_DB_AMOUNT) over (ORDER BY FIRST_LVL_ACCOUNT) then 0 else FIRST_LVL_DB_AMOUNT end as FIRST_LVL_DB_AMOUNT,
							case when FIRST_LVL_CR_AMOUNT = lag(FIRST_LVL_CR_AMOUNT) over (ORDER BY FIRST_LVL_ACCOUNT) then 0 else FIRST_LVL_CR_AMOUNT end as FIRST_LVL_CR_AMOUNT,
							 SECCOND_LVL_DB_AMOUNT, SECCOND_LVL_CR_AMOUNT, 
                            THIRD_LVL_DB_AMOUNT,THIRD_LVL_CR_AMOUNT, FOURTH_LVL_DB_AMOUNT, FOURTH_LVL_CR_AMOUNT,
                            FIFTH_LVL_DB_AMOUNT, FIFTH_LVL_CR_AMOUNT
                             from IncomeStatement";

                DataSet ds = ExecuteDataSet(sql);

                sql = @"select case when sum(Amount) + sum(Fine) is null then 0 else sum(Amount) + sum(Fine) end as Revenue from IssuedChallan
                                where CAST( PaidDate as date) >= '{0}' and CAST( PaidDate as date) <= '{1}' and PaidFlag = 1";
                sql = string.Format(sql, getDate(fromdate), getDate(todate));

                int revenue = Convert.ToInt32(ExecuteScalar(sql).ToString());

                ds.Tables[0].Columns.Add("Revenue");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ds.Tables[0].Rows[i]["Revenue"] = revenue.ToString();
                }

                return ds;
            
        }

        public DataSet GetProfitData(DateTime fromdate, DateTime todate, int branchId)
        {
            

                string sql = @"EXEC	 [dbo].[sp_Income_Statement]
		                                @fromDate = N'{0}',
		                                @toDate = N'{1}',
                                        @branchId = {2}";

                sql = string.Format(sql, getDate(fromdate), getDate(todate), branchId);
                ExecuteNonQuery(sql);

                //sql = @"select (Select AccountName from FinanceFourthLvlAccounts where Id = CreditFourthLvlId) as Head, 
                //            isnull((select sum(DebitAmount) from JournalEntry where DebitThirdLvlId = (Select Id from FinanceThirdLvlAccounts where AccountName like 'Capital%') and DebitFourthLvlId = je1.CreditFourthLvlId),0) as DebitAmount, 
                //            cast(round((convert(DECIMAL(16,4), sum(CreditAmount))/(select sum(CreditAmount) from JournalEntry je 
                //            where CreditThirdLvlId = (Select Id from FinanceThirdLvlAccounts where AccountName like 'Capital%')))*100, 2) as numeric(10,2)) as CreditAmount  from JournalEntry je1
                //            where CreditThirdLvlId = (Select Id from FinanceThirdLvlAccounts where AccountName like 'Capital%')
                //                        and CreatedOn >= '{0}' and CreatedOn <= '{1}' and je.BranchId = {2} group by CreditFourthLvlId";

                sql = @"select ffa.AccountName as Head,
                        isnull((select sum(Amount) from JournalEntryDebitDetail jedd 
                        inner join JournalEntry je on jedd.EntryId = je.EntryId and jedd.FifthLvlAccountId = ffa.Id
                        where CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}'), 0) as DebitAmount,
                        isnull((select sum(Amount) from JournalEntryCreditDetail jedd 
                        inner join JournalEntry je on jedd.EntryId = je.EntryId and jedd.FifthLvlAccountId = ffa.Id
                        where CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}'), 0) as CreditAmount
                        from FinanceFifthLvlAccounts ffa
                        where ffa.AccountType = {2} and ffa.BranchId = {3}";

                sql = string.Format(sql, getDate(fromdate), getDate(todate), 1, branchId);
                DataSet ds = ExecuteDataSet(sql);

                sql = @"select case when sum(Amount) + sum(Fine) - (select  sum(distinct FIRST_LVL_AMOUNT) as CostFirstLvl from IncomeStatement) is null then 0 else
                        sum(Amount) + sum(Fine) - (select  sum(distinct FIRST_LVL_AMOUNT) as CostFirstLvl from IncomeStatement) end
                         as Revenue from IssuedChallan
                                where CAST( PaidDate as date) >= '{0}' and CAST( PaidDate as date) <= '{1}'";
                sql = string.Format(sql, getDate(fromdate), getDate(todate));

                int revenue = Convert.ToInt32(ExecuteScalar(sql).ToString());

                ds.Tables[0].Columns.Add("Revenue");

                int creditSum = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ds.Tables[0].Rows[i]["Revenue"] = revenue.ToString();
                    creditSum += int.Parse(ds.Tables[0].Rows[i][2].ToString());
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int sum = int.Parse(ds.Tables[0].Rows[i][2].ToString());
                    ds.Tables[0].Rows[i][2] = (sum * 100) / creditSum;
                }

                return ds;
            
        }


        public DataSet GetCapitalWithDrawlReportData(DateTime fromDate, DateTime toDate, int branchId)
        {
           
                //string sql = @"select (Select AccountName from FinanceFourthLvlAccounts where Id = DebitFourthLvlId) as Head, 
                //                DebitAmount as CreditAmount, CreatedOn, DebitDescription as CreditDescription from JournalEntry 
                //                where DebitThirdLvlId = (Select Id from FinanceThirdLvlAccounts where AccountName like 'Capital%')
                //                        and CreatedOn >= '{0}' and CreatedOn <= '{1}' and BranchId = {2}";
                string sql = @"select ffa.AccountName as Head, jecd.Amount as CreditAmount,
                                je.CreatedOn, jecd.Description as CreditDescription
                                from JournalEntryDebitDetail jecd
                                inner join FinanceFifthLvlAccounts ffa on jecd.FifthLvlAccountId = ffa.Id
                                inner join JournalEntry je on jecd.EntryId = je.EntryId
                                where CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}'
                                and ffa.AccountType = '{2}' and je.BranchId = {3}";
                sql = string.Format(sql, getDate(fromDate), getDate(toDate), 1, branchId);
                return ExecuteDataSet(sql);
           
        }

        public DataSet GetCapitalInvestmentReportData(DateTime fromDate, DateTime toDate, int branchId)
        {
           
                //string sql = @"select (Select AccountName from FinanceFourthLvlAccounts where Id = CreditFourthLvlId) as Head, 
                //                        CreditAmount, CreatedOn, CreditDescription from JournalEntry 
                //                        where CreditThirdLvlId = (Select Id from FinanceThirdLvlAccounts where AccountName like 'Capital%')
                //                        and CreatedOn >= '{0}' and CreatedOn <= '{1}' and BranchId = {2}";
                string sql = @"select ffa.AccountName as Head, jecd.Amount as CreditAmount,
                                    je.CreatedOn, jecd.Description as CreditDescription
                                    from JournalEntryCreditDetail jecd
                                    inner join FinanceFifthLvlAccounts ffa on jecd.FifthLvlAccountId = ffa.Id
                                    inner join JournalEntry je on jecd.EntryId = je.EntryId
                                    where CAST( je.CreatedOn as date) >= '{0}' and CAST( je.CreatedOn as date) <= '{1}'
                                    and ffa.AccountType = '{2}' and je.BranchId = {3}";
                sql = string.Format(sql, getDate(fromDate), getDate(toDate), 1, branchId);
                return ExecuteDataSet(sql);
            
        }

    }
}
