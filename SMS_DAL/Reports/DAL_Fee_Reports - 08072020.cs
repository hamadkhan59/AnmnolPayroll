﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.Reports
{
    public class DAL_Fee_Reports : DAL.DABase
    {
        public DataSet GetFeeCollectionReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int paidFlag, int branchId)
        {
            string sql = @"select st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, cls.Name as Class, sec.Name as Section, st.FatherName as FatherName, st.AdmissionNo as Description,
                                case when isch.Paidflag = 0 then 'N' else 'Y' end as Paid, 
                                case when isch.PaidFlag = 0 then 'N/A' else LEFT(CONVERT(VARCHAR, isch.PaidDate, 120), 10) end as PaidDate, 
                                case when isch.Paidflag = 0 then 0 else isnull(isch.Amount,0) end as Amount,  
                                isch.ForMonth, case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                                case when fb.Balance is null then 0 else fb.Balance end as Balance,
                                isch.ChalanAmount as Fine  
                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                                and clsec.SectionId = sec.Id and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
                                + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
                                "and ( -1 in (" + paidFlag + ") or isch.PaidFlag in (" + paidFlag + ") ) " +
                                "and st.BranchId = " + branchId +
                                "and isch.PaidDate >= '" + getDate(fromDate) + "' and isch.PaidDate <= '" + getDate(toDate) + "' order by st.id desc";


            return ExecuteDataSet(sql);   
        }

//        public DataSet GetFeeCollectionReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int paidFlag, int branchId)
//        {
//            string sql = @"select st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, cls.Name as Class, sec.Name as Section, 
//                                case when isch.Paidflag = 0 then 'N' else 'Y' end as Paid, 
//                                case when isch.PaidFlag = 0 then 'N/A' else LEFT(CONVERT(VARCHAR, isch.PaidDate, 120), 10) end as PaidDate, isch.Amount,  
//                                isch.ForMonth, case when fb.Advance is null then 0 else fb.Advance end as Advance, 
//                                case when fb.Balance is null then 0 else fb.Balance end as Balance,
//                                case when isch.Fine is null then 0 else isch.Fine end as Fine
//                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
//                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
//									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
//                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
//                                and clsec.SectionId = sec.Id and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
//                                + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
//                                "and ( -1 in (" + paidFlag + ") or isch.PaidFlag in (" + paidFlag + ") ) " +
//                                "and st.BranchId = " + branchId +
//                                "and isch.PaidDate >= '" + getDate(fromDate) + "' and isch.PaidDate <= '" + getDate(toDate) + "' order by st.id desc";


//            return ExecuteDataSet(sql);
//        }

        public DataSet GetFeeDefaulterTerminatedReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int paidFlag, int branchId)
        {
            string sql = @"select st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, cls.Name as Class, sec.Name as Section, 
                                case when isch.Paidflag = 0 then 'N' else 'Y' end as Paid, 
                                case when isch.PaidFlag = 0 then 'N/A' else LEFT(CONVERT(VARCHAR, isch.PaidDate, 120), 10) end as PaidDate, 
                                case when isch.Paidflag = 0 then 0 else isnull(isch.Amount,0) end as Amount,  
                                isch.ForMonth, case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                                case when fb.Balance is null then 0 else fb.Balance end as Balance,
                                case when isch.Fine is null then 0 else isch.Fine end as Fine
                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                                and clsec.SectionId = sec.Id and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
                                + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
                                "and ( -1 in (" + paidFlag + ") or isch.PaidFlag in (" + paidFlag + ") ) " +
                                "and st.BranchId = " + branchId +
                                "and isch.PaidDate >= '" + getDate(fromDate) + "' and isch.PaidDate <= '" + getDate(toDate) + "' and st.LeavingStatus = 3 order by cast(st.RollNumber as int)";


            return ExecuteDataSet(sql);
        }

        public DataSet GeProspectusAndRegisterationCollection(int classId, string name, string fatherName, string sessionYear, string inquiryNumber, int bracnhId)
        {
            try
            {
                string sql = @"select stIq.InquiryNumber as RollNo, stIq.Name, stIq.FatherName as PaidMonth, cls.Name as Class, 
                                stIq.AdmissionDate as PaidDate, stIq.ProspectusFee as Advance, stIq.RegisterationFee as Amount
                                from StudentInquiry stIq left outer join Classes cls on stIq.ClassId = cls.Id
                                where (0 in ({0}) or cls.Id in ({0})) 
                                and stIq.Name like '%{1}%' and stIq.FatherName like '%{2}%' 
                                and stIq.InquiryNumber like '%{3}%' and year(stIq.AdmissionDate) like '%{4}%'
                                and stIq.BranchId = {5}
                                and (stIq.ProspectusFee > 0 or stIq.RegisterationFee > 0)";

                sql = string.Format(sql, classId, name, fatherName, inquiryNumber, sessionYear, bracnhId);
                return ExecuteDataSet(sql);
            }
            catch (Exception e)
            { }
            return null;
        }

        public DataSet GetUnpaidStudentsFeeHeadWise(int classId, int sectionId, string RollNo, int paidFlag, string forMonth, 
            string name, string fatherName, int feeHeadId, int bracnhId)
        {
            DataTable dt = new DataTable();
            if (RollNo == "0")
                RollNo = "";
            try
            {
                string sql = @"select st.RollNumber as RollNo, st.Name, st.FatherName as PaidMonth, cls.Name as Class, sec.Name as Section, isCh.ForMonth, 
                                fh.Name as FeeHead, chTf.Amount
                                from IssuedChallan isCh left outer join ChallanStudentDetail chts on isCh.ChallanToStdId = chts.Id
                                left outer join Challan ch on chts.ChallanId = ch.Id left outer join Student st on chts.StdId = st.id
                                left outer join ChallanFeeHeadDetail chTf  on chTf.ChallanId = ch.Id left outer join FeeHeads fh on fh.Id = chTf.HeadId 
							    Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                left outer join Classes cls on clsec.ClassId = cls.Id left outer join Section sec on sec.Id = clsec.SectionId

                                where isCh.IssuedFlag = {7}
                                and (0 in ({0}) or cls.Id in ({0})) and (0 in ({1}) or sec.Id in ({1})) 
                                and st.RollNumber like '%{2}%' and st.Name like '%{3}%' and isch.ForMonth like '%{4}%'
                                and (0 in ({5}) or fh.Id in ({5})) and st.FatherName like '%{6}%' and chTf.Amount > 0
                                and st.BranchId = {8}
                                order by fh.Id, cast(st.RollNumber as int) ";

                sql = string.Format(sql, classId, sectionId, RollNo, name, forMonth, feeHeadId, fatherName, paidFlag, bracnhId);
                return ExecuteDataSet(sql);
            }
            catch (Exception e)
            { }
            return null;
        }


        public DataSet GetAdmissionChargesReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int bracnhId)
        {
            string sql = @"select distinct st.RollNumber as RollNo, st.Name, st.FatherName as MonthNumber, cls.Name as Class, sec.Name as Section, st.Contact_1 as PhoneNumber, st.AdmissionNo as Description,
                            st.AdmissionDate as PaidDate, isnull((select sum(Amount) from StudentAdmissionCharges where StudentId = st.id
                            and HeadId <> isnull((Select Id from FeeHeads where Name like 'Tuition Fee%'),0)),0) as Amount, isnull(apc.Amount,0) as Paid
                            from Classes cls, Section sec,Student st, StudentAdmissionCharges sac, ClassSection clsec, SACPaidCatalog apc
                                where clsec.ClassSectionId = st.ClassSectionId and clsec.ClassId = cls.Id 
                                and st.id = sac.StudentId and clsec.SectionId = sec.Id and st.id = apc.StudentId
	                            and ( '0' in ('{2}') or st.RollNumber in ('{2}') ) 
                                    and ( 0 in ({0}) or sec.Id in ({0}) ) and ( 0 in ({1}) or cls.Id in ({1}) )
                                and st.AdmissionDate >=  '{3}' and st.AdmissionDate <= '{4}' and st.BranchId = {5}";
            sql = string.Format(sql, sectionId, classId, RollNo, getDate(fromDate), getDate(toDate), bracnhId);
            return ExecuteDataSet(sql);  
        }

        public DataSet GetAnnualChargesReportData(int classId, int sectionId, string forMonth, int branchId)
        {
            string sql = @"select distinct st.RollNumber as RollNo, st.Name, st.FatherName as MonthNumber, cls.Name as Class, sec.Name as Section ,
                            sac.ForMonth as PaidDate, isnull(sac.Charges,0) as Amount
                            from Classes cls, Section sec, ClassSection clsec, Student st left outer join AnnualCharges sac on st.id = sac.StudentId
                                where clsec.ClassSectionId = st.ClassSectionId and clsec.ClassId = cls.Id and st.id = sac.StudentId and clsec.SectionId = sec.Id 
                                    and ( 0 in ({0}) or sec.Id in ({0}) ) and ( 0 in ({1}) or cls.Id in ({1}) )
                                and  sac.ForMonth like '%{2}%' and st.BranchId = {3}";
            sql = string.Format(sql, classId, sectionId, forMonth, branchId);
            return ExecuteDataSet(sql);  
        }

        public DataSet GetIssueChallanReportData(DateTime fromDate, DateTime toDate, int branchId)
        {
            string sql = @"select  distinct concat('000',isCh.Id) as Advance, case when isCh.PaidFlag = 0 then 'No' else 'Yes' end as Paid, 
                                st.Name, st.RollNumber as RollNo, cls.Name as Class, sec.Name as Section, 
                                ch.Name as ForMonth, case when isCh.Amount is null then 0 else isCh.Amount end as Amount, isCh.DueDate as PaidDate,
                                isch.IssueDate as Fine
                                from Student st left outer join FeeBalance fb on st.id = fb.StudentId
								Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                left outer join Classes cls 
                                on clsec.ClassId = cls.Id left outer join Section sec on clsec.SectionId = sec.Id,
                                 Challan ch, ChallanStudentDetail chStd Left Outer join  IssuedChallan isCh on chStd.Id = isch.ChallanToStdId 
                                where st.id = chStd.StdId and ch.Id = chStd.ChallanId 
                                and isCh.IssueDate >= '{0}' and isCh.IssueDate <= '{1}' and st.BranchId = {2}";

            sql = string.Format(sql, getDate(fromDate), getDate(toDate), branchId);

            return ExecuteDataSet(sql);  
        }

        public DataSet GetBadDebtorsReportData(int branchId)
        {
            string sql = @"select distinct case when isCh.DueDate is null then 'No' else 'Yes' end as Paid, 
                                st.Name, st.RollNumber as RollNo, cls.Name as Class, sec.Name as Section, 
                                ch.Name as ForMonth, case when isCh.Amount is null then 0 else isCh.Amount end as Amount, 
                                isch.ForMonth as Fine, st.Contact_1 as PhoneNumber
                                from Student st left outer join FeeBalance fb on st.id = fb.StudentId 
                                Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                left outer join Classes cls 
                                on clsec.ClassId = cls.Id left outer join Section sec on clsec.SectionId = sec.Id,
                                    Challan ch, ChallanStudentDetail chStd Left Outer join  IssuedChallan isCh on chStd.Id = isch.ChallanToStdId 
                                where st.id = chStd.StdId and ch.Id = chStd.ChallanId 
                                and PaidFlag = 0 and (st.LeavingDate is not null or st.LeavingDate <> st.AdmissionDate) and st.BranchId = {0}";

            sql = string.Format(sql, branchId);

            return ExecuteDataSet(sql);

        }

        public DataSet GetFeePaymentSlip(int issueChallanId)
        {
            string sql = @"select distinct cls.Name as Class, sec.Name as Section,
                            std.Name, std.RollNumber as RollNo, std.FatherName,  fh.Name as FeeHead,  
                            case when ph.FeeType = '1' then 'Monthly Fee' when ph.FeeType = '2'then 'Fee Arrears'
	                            when ph.FeeType = '3' then 'Extra Charges' end as Type,
	                            PayAmount as Balance, ph.Description
                            from PaymentHistory ph, FeeHeads fh, IssuedChallan isCh,
                            ChallanStudentDetail chStd, Student std, ClassSection clsec,
                            Classes cls, Section sec
                            where ph.FeeHeadId = fh.Id and isCh.id = ph.IssueChallanId
                            and chStd.Id = isCh.ChallanToStdId and std.id = chStd.StdId
                            and std.ClassSectionId = clsec.ClassSectionId
                            and clsec.ClassId = cls.Id and clsec.SectionId = sec.Id
                            and IssueChallanId = {0}";
            sql = string.Format(sql, issueChallanId);

            return ExecuteDataSet(sql);
        }
        public DataSet GetAdvanceStudentsReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int paidFlag, int branchId)
        {
            string sql = @"select st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, cls.Name as Class, sec.Name as Section, 
                                case when isch.Paidflag = 0 then 'N' else 'Y' end as Paid, 
                                case when isch.PaidFlag = 0 then 'N/A' else LEFT(CONVERT(VARCHAR, isch.PaidDate, 120), 10) end as PaidDate, isch.Amount,  
                                isch.ForMonth, case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                                case when fb.Balance is null then 0 else fb.Balance end as Balance,
                                case when isch.Fine is null then 0 else isch.Fine end as Fine
                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                                and clsec.SectionId = sec.Id and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
                                + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
                                "and ( isch.PaidFlag in (1) ) " +
                                "and st.BranchId = " + branchId +
                                "and isch.PaidDate >= '" + getDate(fromDate) + "' and isch.PaidDate <= '" + getDate(toDate) + "' and Advance > 0 ";

            return ExecuteDataSet(sql);
        }

        public DataSet GetMonthlyCollectionReportData(int classId, int sectionId, string RollNo, int paidFlag, string forMonth, string name, string fatherName, int branchId)
        {
            string sql = @"select st.RollNumber as RollNo, st.Name, st.FatherName, st.FatherCnic, cls.Name as Class, sec.Name as Section, 
                                case when isch.Paidflag = 0 then 'N' else 'Y' end as Paid, 
                                case when isch.PaidFlag = 0 then 'N/A' else LEFT(CONVERT(VARCHAR, isch.PaidDate, 120), 10) end as PaidDate, isch.Amount,  
                                isch.ForMonth, case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                                case when fb.Balance is null then 0 else fb.Balance end as Balance,
                                case when isch.Fine is null then 0 else isch.Fine end as Fine
                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
                                    Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                                and clsec.SectionId = sec.Id and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
                                + " and st.Name like '%" + name + "%' and st.FatherName like '%" + fatherName + "%' "
                                + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
                                "and ( -1 in (" + paidFlag + ") or isch.PaidFlag in (" + paidFlag + ") ) " +
                                "and st.BranchId = " + branchId +
                                "and isch.ForMonth like '%" + forMonth + "%'";

            return ExecuteDataSet(sql);
        }

    
        public DataSet GetYearlyCollectionReportData(int classId, int sectionId, string forMonth, int branchId)
        {
            string sql = @"select cls.Name as Class, sec.Name as Section, sum(isch.Amount) as Amount, DATEPART(m,isch.PaidDate) as MonthNumber,
                                sum(case when isch.Fine is null then 0 else isch.Fine end) as Fine,
                                CONVERT(varchar, DATEPART(m,isch.PaidDate))+' - '+DateName( month , DateAdd( month ,  DATEPART(m,isch.PaidDate) , 0 ) - 1 ) as PaidMonth
                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
                                    Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                                and clsec.SectionId = sec.Id and ( -1 in (-1) or st.RollNumber in (-1) )  and ( 0 in (" + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) "
                                 + "   and ( -1 in (-1) or isch.PaidFlag in (-1) ) "
                                 + "   and isch.ForMonth like '%" + forMonth + "%'" +
                                 "and st.BranchId = " + branchId 
                                + " group by clsec.ClassId, clsec.SectionId, cls.Name, sec.Name, DateName( month , DateAdd( month ,  DATEPART(m,isch.PaidDate) , 0 ) - 1 ),"
                                + "  DATEPART(m,isch.PaidDate) order by DATEPART(m,isch.PaidDate)";

            return ExecuteDataSet(sql);
        }

        public DataSet GetFeeHeadsReport(int branchId)
        {
            string sql = @"select head.Name as FeeHead, sum(chFee.Amount) as Amount
                                from IssuedChallan isCh, ChallanStudentDetail chStd, ChallanFeeHeadDetail chFee,
                                FeeHeads head, Challan ch
                                where isCh.PaidFlag = 1 and isch.ChallanToStdId = chStd.Id
                                and chStd.Id = chFee.ChallanId and chFee.HeadId = head.Id
                                and chFee.ChallanId = ch.Id and head.BranchId = {0}
                                group by head.Name";

            sql = string.Format(sql, branchId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetFeeBreakUpReport(int branchId)
        {
            string sql = @"select sum(chFee.Amount)/(count(chStd.StdId)/(select count(*) from ChallanFeeHeadDetail where ChallanId = chFee.ChallanId)) as Amount, 
                                count(chStd.StdId)/(select count(*) from ChallanFeeHeadDetail where ChallanId = chFee.ChallanId ) as StudentCount
                                from ChallanFeeHeadDetail chFee, ChallanStudentDetail chStd, Challan ch, FeeHeads head
                                where ch.Id = chFee.ChallanId 
                                and chFee.HeadId = head.Id
                                and chStd.ChallanId = ch.Id and head.BranchId = {0}
                                group by chFee.ChallanId";

            sql = string.Format(sql, branchId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetFeeDetailReport(int branchId, string forMonth)
        {
            string sql = @"select heads.Name as FeeHead, SUM(case when isch.PaidFlag = 1 then 1 else 0 end) as Paid,
                            SUM(case when isch.PaidFlag <> 1 then 1 else 0 end) as UnPaid,
                            Sum(isChDet.TotalAmount) as TotalFee, SUM(isChDet.PayAmount) as PaidFee
                            from IssuedChallan isCh, IssueChalanDetail isChDet, FeeHeads heads
                            where isCh.Id = isChDet.IssueChallanId and heads.Id = isChDet.FeeHeadId
                            and isCh.ForMonth like '%{0}%' and isCh.BranchId = {1}
                            Group By heads.Name";

            sql = string.Format(sql, forMonth, branchId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetClassWiseFeeDetailReport(int branchId, string forMonth)
        {
            string sql = @"select cls.Name as CLass, sec.Name as Section, heads.Name as FeeHead, 
                        SUM(case when isch.PaidFlag = 1 then 1 else 0 end) as Paid,
                        SUM(case when isch.PaidFlag <> 1 then 1 else 0 end) as UnPaid,
                        Sum(isChDet.TotalAmount) as TotalFee, SUM(isChDet.PayAmount) as PaidFee
                        from IssuedChallan isCh, IssueChalanDetail isChDet, FeeHeads heads, 
                        Classes cls, Section sec, ChallanStudentDetail chs,
                        Student st Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                        where isCh.Id = isChDet.IssueChallanId and heads.Id = isChDet.FeeHeadId
                        and chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                        and clsec.SectionId = sec.Id
                        and isCh.ForMonth like '%{0}%' and isCh.BranchId = {1}
                        Group By cls.Name, sec.Name, heads.Name";

            sql = string.Format(sql, forMonth, branchId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetFeeBalanceReportData(int classId, int sectionId, string RollNo, string name, string fatherName, int branchId)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"select distinct st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, st.FatherName, 
                                cls.Name as Class, sec.Name as Section, 
                                case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                                case when fb.Balance is null then 0 else fb.Balance end as Balance
                                from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where clsec.ClassId = cls.Id and clsec.SectionId = sec.Id and
                                 ( '0' in ('{0}') or st.RollNumber in ('{0}') ) and ( 0 in ({1}) or sec.Id in ({1}) ) 
                                 and ( 0 in ({2}) or cls.Id in ({2}) ) and st.Name like '%{3}%' and st.FatherName like '%{4}%' 
                                 and st.BranchId = {5}   and fb.Balance > 0";
                sql = string.Format(sql, RollNo, sectionId, classId, name, fatherName, branchId);

                return ExecuteDataSet(sql);
            }
            catch (Exception e)
            { }
            return null;
        }


        public DataSet GetPaymentHistoryReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int branchId)
        {
            string sql = @"select top 10  st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, st.FatherName,
                            cls.Name as Class, sec.Name as Section, fhs.Name as FeeHead,
							case when ph.FeeType = '1' then 'Monthly Fee' when ph.FeeType = '2'then 'Fee Arrears'
							when ph.FeeType = '3' then 'Extra Charges' end as Type,
                            case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                            isnull(ph.PayAmount, 0) as Balance, ph.Description
                            from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec, 
                            PaymentHistory ph, FeeHeads fhs, 
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
									where isch.Id = ph.IssueChallanId and chs.Id = isch.ChallanToStdId and chs.StdId = st.id 
									and clsec.ClassId = cls.Id and fhs.Id = ph.FeeHeadId 
                                and clsec.SectionId = sec.Id and fb.Id = ph.FeeBalanceId and ( ph.PayAmount > 0 ) and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
                            + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
                            "and st.BranchId = " + branchId +
                            "and ph.CreatedOn >= '" + getDate(fromDate) + "' and ph.CreatedOn <= '" + getDate(toDate) + "' order by cls.Id, sec.Id, st.Id";

            sql = string.Format(sql, sectionId, classId, RollNo, fromDate, toDate);

            return ExecuteDataSet(sql);
        }

        public DataSet GetPaymentDetailedReportData(DateTime fromDate, DateTime toDate, int classId, int sectionId, string RollNo, int branchId)
        {
           
            string sql = @"select distinct st.Contact_1 as PhoneNumber, st.RollNumber as RollNo, st.Name, st.FatherName,
                            cls.Name as Class, sec.Name as Section, icd.UpdateOn as UpdatedOn,
                            case when icd.Type = 1 then 'Regular Fee' when icd.Type = 2 then 'Extra Charges' else 'Arrear Fee' end as Type,
                            case when fb.Advance is null then 0 else fb.Advance end as Advance, 
                            fh.Name as FeeHeadName, icd.TotalAmount as TotalAmount, icd.PayAmount as PayAmount, isch.ForMonth as ForMonth
                            from ChallanStudentDetail chs, IssuedChallan isch, Classes cls, Section sec, IssueChalanDetail icd, FeeHeads fh,
                                    Student st left outer join FeeBalance fb on st.id = fb.StudentId
									Left outer join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                                where chs.Id = isch.ChallanToStdId and chs.StdId = st.id and clsec.ClassId = cls.Id
                                and icd.IssueChallanId = isch.Id and fh.Id = icd.FeeHeadId and icd.PayAmount > 0
                                and clsec.SectionId = sec.Id and ( 0 in (" + RollNo + ") or st.RollNumber in (" + RollNo + ") ) "
                            + " and ( 0 in ( " + sectionId + ") or sec.Id in (" + sectionId + ") ) and ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) " +
                            "and st.BranchId = " + branchId +
                            "and isch.IssueDate >= '" + getDate(fromDate) + "' and isch.IssueDate <= '" + getDate(toDate) + "'";

            sql = string.Format(sql, sectionId, classId, RollNo, fromDate, toDate);
            return ExecuteDataSet(sql);
        }


        private string getDate(DateTime date)
        {
            return date.Year + "-" + date.Month + "-" + date.Day;
        }

        public DataSet GetBillingStrength(string ForMonth)
        {
            string sql = @"EXEC [dbo].[sp_Billing_Strength]
		                    @ForMonth = N'{0}'";
            sql = string.Format(sql, ForMonth);

            return ExecuteDataSet(sql);
        }
    }
}
