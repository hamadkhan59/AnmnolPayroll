using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.Reports
{
    public class DAL_Staff_Reports : DAL.DABase
    {
        public DataSet GetNewStaffData(int catagoryId, int desId, DateTime fromDate, DateTime toDate, int branchId)
        {
            var sql = @"select st.Name, FatherName, dsgn.Name as Designation, st.Education, st.Year, st.Marks_Or_Cgpa as Result,
                                st.Total_Marks_Or_Cgpa as total, st.Salary, st.PhoneNumber as ContactNo,
								CONVERT(VARCHAR(10),  st.JoinDate, 103) as JoininDate, st.currentAddress as Address
                                from Staff st, Designation dsgn
                                where st.DesignationId = dsgn.Id and CAST(st.JoinDate as date) >= '" + getDate(fromDate) + "' and CAST(st.JoinDate as date) <= '" + getDate(toDate) + "'" +
                                " and st.BranchId = " + branchId +
                                " and (-1 in (" + desId + ") or st.DesignationId in (" + desId + ")) and ( -1 in (" + catagoryId + ") or dsgn.CatagoryId in (" + catagoryId + "))";
            return ExecuteDataSet(sql);
        }

        public DataSet GetNewStaffAttendanceLogs()
        {
            //var sql = @"select * from StaffAttendanceLogs where staffid = 122 order by datetime";
            //var sql = @"select * from StaffAttendanceLogs";
            //var sql = @"select * from StaffAttendanceLogs where id >= 10738";
            var sql = @"select * from StaffAttendanceLogs where DateTimeString like '1/6/2022%'";
            return ExecuteDataSet(sql);
        }


        public DataSet GeStaffContactDirectoryData(int branchId)
        {
            var sql = @"select RIGHT('00000'++CAST(st.StaffId AS VARCHAR(5)),5) as Year, st.Name, FatherName, dsgn.Name as Designation,
                                st.Education, st.Year, st.Marks_Or_Cgpa as Result,
                                st.Total_Marks_Or_Cgpa as total, st.Salary, st.PhoneNumber as ContactNo,
								CONVERT(VARCHAR(10),  st.JoinDate, 103) as JoininDate, st.currentAddress as Address
                                from Staff st, Designation dsgn
                                where st.DesignationId = dsgn.Id and st.BranchId = {0} order by st.StaffId";
            sql = string.Format(sql, branchId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetLeavingStaffData(int catagoryId, int desId, DateTime fromDate, DateTime toDate, int branchId)
        {
            var sql = @"select st.Name, FatherName, dsgn.Name as Designation, st.Education, st.Year, st.Marks_Or_Cgpa as Result,
                                st.Total_Marks_Or_Cgpa as total, st.Salary, CONVERT(VARCHAR(10),  st.JoinDate, 103) as JoininDate, 
                                CONVERT(VARCHAR(10),  st.LeavingDate, 103) as LeavingDate, st.currentAddress as Address
                                from Staff st, Designation dsgn
                                where st.DesignationId = dsgn.Id and CAST(st.LeavingDate as date) >= '" + getDate(fromDate) + "' and CAST(st.LeavingDate as date) <= '" + getDate(toDate) + "'" +
                                " and st.BranchId = " + branchId +
                                " and (-1 in (" + desId + ") or st.DesignationId  in (" + desId + ")) and ( -1 in (" + catagoryId + ") or dsgn.CatagoryId in (" + catagoryId + "))";

            return ExecuteDataSet(sql);
        }

        public DataSet GetStaffMonthlySalaryData(string forMonth, int catagoryId, int desId, int branchId, int staffId = -1)
        {
            staffId = staffId == 0 ? -1 : staffId;
            //var sql = @"select REPLACE(STR(st.StaffId, 5), SPACE(1), '0')  as StaffId, st.Name, st.FatherName, desgn.Name as Designation, isnull(stsal.Bonus,0) as ContactNo,
            //                        stsal.PaidAmount as Salary, stsal.ForMonth as Year, isnull(stsal.AdvanceAdjustment,0) as Result, isnull(stsal.Deduction,0) as Total,
            //                        isnull(stsal.MiscWithdraw,0) as Education, isnull(stsal.SundaysDeduction,0) as Address
            //                        from Staff st, StaffSalary stsal, Designation desgn
            //                         where st.StaffId = stsal.StaffId and stsal.ForMonth like '%" + forMonth + "%' and (-1 in (" + desId + ") or st.DesignationId in (" + desId + "))"
            //                            + " and (-1 in (" + staffId + ") or st.StaffId in (" + staffId + ")) and st.DesignationId = desgn.Id" +
            //                            " and st.BranchId = " + branchId +
            //                        " and ( -1 in (" + catagoryId + ") or desgn.CatagoryId in (" + catagoryId + ")) group by st.StaffId, st.Name, stsal.Bonus, st.FatherName, stsal.PaidAmount, stsal.ForMonth, desgn.Name, stsal.AdvanceAdjustment, stsal.Deduction ";

            var sql = @"select REPLACE(STR(st.StaffId, 5), SPACE(1), '0')  as StaffId, st.Name, st.FatherName, desgn.Name as Designation, isnull(stsal.Bonus,0) as ContactNo,
                                    stsal.PaidAmount as Salary, stsal.ForMonth as Year, isnull(stsal.AdvanceAdjustment,0) as Result, isnull(stsal.Deduction,0) as Total,
                                    isnull(stsal.MiscWithdraw,0) as Education, isnull(stsal.SundaysDeduction,0) as Address
                                    from Staff st, StaffSalary stsal, Designation desgn
                                     where st.StaffId = stsal.StaffId and stsal.ForMonth like '%" + forMonth + "%' and (-1 in (" + desId + ") or st.DesignationId in (" + desId + "))"
                                       + " and (-1 in (" + staffId + ") or st.StaffId in (" + staffId + ")) and st.DesignationId = desgn.Id" +
                                       " and st.BranchId = " + branchId +
                                   " and ( -1 in (" + catagoryId + ") or desgn.CatagoryId in (" + catagoryId + ")) "
                                   + "group by st.StaffId, st.Name, stsal.Bonus, st.FatherName, stsal.PaidAmount, stsal.ForMonth, desgn.Name, stsal.AdvanceAdjustment, stsal.Deduction, stsal.MiscWithdraw, stsal.SundaysDeduction";


            return ExecuteDataSet(sql);
        }

        public DataSet GetStaffYearlySalaryData(string forMonth, int desId, int catagoryId, int branchId, int staffId = -1)
        {
            var sql = @"select stsal.ForMonth as Year, sum(stsal.PaidAmount) as Salary, sum(isnull(stsal.Bonus,0)) as ContactNo,
                            sum(isnull(stsal.AdvanceAdjustment,0)) as Result, sum(isnull(stsal.Deduction,0)) as Total
                            from Staff st, StaffSalary stsal, Designation desgn
                                where st.StaffId = stsal.StaffId and stsal.ForMonth like '%" + forMonth + "%' and (-1 in (" + desId + ") or st.DesignationId in (" + desId + ")) "
                                 + " and (-1 in (" + staffId + ") or st.StaffId in (" + staffId + ")) and st.DesignationId = desgn.Id " +
                                 " and st.BranchId = " + branchId +
                            " and ( -1 in (" + catagoryId + ") or desgn.CatagoryId in (" + catagoryId + ")) group by stsal.ForMonth";
            return ExecuteDataSet(sql);
        }

        public DataSet GetStaffattandanceReport(DateTime fromDate, DateTime toDate)
        {
            var spQuery = @"EXEC [dbo].[sp_StaffAttendancedetailReport]
		                            @FromDate = '{0}', @ToDate = '{1}' ";
            spQuery = string.Format(spQuery, getDate(fromDate), getDate(toDate));
            ExecuteNonQuery(spQuery);

            var sql = @"select StaffId as Year, Name, FatherName, MobileNo as Designation,
                        TotalHours as JoininDate, WorkingHours as Address, [EarningAmount] AS Salary,
                        isnull(WorkingDays,0) as Total, isnull(ExtraHours,0) as Education
                        from [StaffAttendanceReport] order by StaffId";
            return ExecuteDataSet(sql);
        }

        public DataSet GetStaffattandanceReportForStaff(DateTime fromDate, DateTime toDate)
        {
            var spQuery = @"EXEC [dbo].[sp_StaffAttendancedetailReport]
		                            @FromDate = '{0}', @ToDate = '{1}' ";
            spQuery = string.Format(spQuery, getDate(fromDate), getDate(toDate));
            ExecuteNonQuery(spQuery);

            var sql = @"Select sar.StaffId as Year, sar.Name, sar.TotalHours  as JoininDate, 
                        sar.WorkingHours  as Address, sad.TimeIn as Designation, sad.TimeOut as Education,
                        isnull(ExtraHours,0) as Total
                        from StaffAttendanceReport sar inner join StaffAttendanceDetail sad
                        on sar.AttendanceId = sad.AttendanceId
                        order  by sar.StaffId, sad.Id";

            if (fromDate.Date == toDate.Date)
            {
                sql = @"Select sar.StaffId as Year, sar.Name, sar.TotalHours  as JoininDate, 
                        sar.WorkingHours  as Address, isnull(sad.TimeIn, 'Absent') as Designation, sad.TimeOut as Education,
                        isnull(ExtraHours,0) as Total
                        from StaffAttendanceReport sar left outer join StaffAttendanceDetail sad
                        on sar.AttendanceId = sad.AttendanceId
                        order  by sar.StaffId, sad.Id";
            }
            return ExecuteDataSet(sql);
        }

        public DataSet GetStaffattandanceReportMonthly(DateTime fromDate, DateTime toDate, int staffId)
        {
            var spQuery = @"EXEC [dbo].[sp_StaffAttendancedetailReportMonthly]
		                            @FromDateMonthly = '{0}', @ToDateMonthly = '{1}', @StaffIdMonthly = {2} ";
            spQuery = string.Format(spQuery, getDate(fromDate), getDate(toDate), staffId);
            ExecuteNonQuery(spQuery);

            //var sql = @"select distinct sta.Id, sta.Date as Result, sar.Name, sar.TotalHours as JoininDate,
            //            sar.WorkingHours  as Address, sta.Time  as Designation, sta.OutTime  as Education
            //            from StaffAttendanceReport sar 
            //            inner join StaffAttandance sta on sar.AttendanceId = sta.Id 
            //            inner join StaffAttendanceDetail stad on sta.Id = stad.AttendanceId
            //            order by sta.Id";

            var sql = @"select distinct sta.Id, sar.AttendanceId, sta.Date as Result, sar.Name, sar.TotalHours as JoininDate,
                        sar.WorkingHours  as Address,
						(select top(1) TimeIn from StaffAttendanceDetail 
						where AttendanceId = sar.AttendanceId order by Id)  as Designation, 
						(select top(1) TimeOut from StaffAttendanceDetail 
						where AttendanceId = sar.AttendanceId order by Id desc)  as Education
                        from StaffAttendanceReport sar 
                        inner join StaffAttandance sta on sar.AttendanceId = sta.Id 
                        order by sta.Date";

            return ExecuteDataSet(sql);
        }

        public DataSet GetStaffattandanceData(int catagoryId, int desId, int StaffId, DateTime fromDate, DateTime toDate, int branchId)
        {
            DataTable dt = new DataTable();
            StaffId = (StaffId == 0 ? -1 : StaffId);
            var sql = @"select RIGHT('00000'+CAST(st.StaffId AS VARCHAR(5)),5) as StaffId, st.Name, St.FatherName, st.JoinDate, desgn.Name as Designation, ctgry.CatagoryName,
                        CONVERT(VARCHAR(10),  sta.Date, 103) as ForMonth, sta.Time as Total, case when sta.OutTime is null then '00:00' else sta.OutTime end as Result, 
                        CONVERT(VARCHAR(10),  st.JoinDate, 103) as JoininDate, case when sta.Status = 1 then 'P' else 'A' end as Year
                        from Staffattandance sta, Staff st, Designation desgn, DesignationCatagory ctgry
                        where sta.StaffId = st.StaffId and st.DesignationId = desgn.Id and desgn.CatagoryId = ctgry.Id and
                        ('-1' in ('{0}') or st.StaffId in ('{0}')) and ('-1' in ('{1}') or st.DesignationId in ('{1}'))
                        and ('-1' in ('{2}') or ctgry.Id in ('{2}')) 
                        and sta.Date >= CONVERT(varchar, '{3}', 23) and sta.Date <= CONVERT(varchar, '{4}', 23) 
                        and st.BranchId = {5} 
                        order by sta.Id desc";
            sql = string.Format(sql, StaffId, desId, catagoryId, getDate(fromDate), getDate(toDate), branchId);

            return ExecuteDataSet(sql);
        }


        public DataSet GetStaffSmsHistoryData(int catId, int desId, int staffId, DateTime fromDate, DateTime toDate, int branchId)
        {
            var sql = @"select st.StaffId AS StaffId , st.Name as Name, ctgry.CatagoryName as Designation, 
                            desgn.Name as Education, CONVERT(VARCHAR(10),  smsHis.SentDate, 103) AS JoininDate, smsHis.Message AS Result, smsHis.Response AS Address, st.FatherName as FatherName, st.PhoneNumber AS ContactNo, st.StaffId AS SrNo
                            from SmsHistory smsHis, Staff st, Designation desgn, DesignationCatagory ctgry
                            where smsHis.StaffId = st.StaffId and st.DesignationId = desgn.Id and desgn.CatagoryId = ctgry.Id
							and CAST(smsHis.SentDate as date) >= '" + getDate(fromDate) + "' and CAST(smsHis.SentDate as date) <= '" + getDate(toDate) + "'" +
            " and (0 in (" + staffId + ") or st.StaffId in (" + staffId + ")) and (-1 in (" + desId + ") or desgn.Id in (" + desId + ")) and (-1 in (" + catId + ") or ctgry.Id in (" + catId + ")) "
            + "and st.BranchId = " + branchId + " order by smsHis.id desc ";

            return ExecuteDataSet(sql);
        }

        public DataSet GetAttendanceDetail(int staffId, DateTime fromDate, DateTime toDate)
        {
            var resultds = new DataSet();
            List<List<string>> result = new List<List<string>>();
            var spQuery = @"DECLARE	@return_value int
                            EXEC	@return_value = [dbo].[sp_StaffAttendancedetail]
		                            @StaffId = " + staffId + ","
                                            + "@FromDate = '" + getDate(fromDate) + "',"
                                            + "@ToDate = '" + getDate(toDate) + "'";

            return ExecuteDataSet(spQuery);
        }

        public int GetStaffApprovalData(int staffId)
        {
            var resultds = new DataSet();
            List<List<string>> result = new List<List<string>>();
            var spQuery = @"EXEC	[dbo].[sp_GetStaffPaymentApproval]
		                     @StaffId = " + staffId;

            var resultDs = ExecuteDataSet(spQuery);
            int count = int.Parse(resultDs.Tables[0].Rows[0][0].ToString());

            return count;
        }

        public void ResetStaffApprovalData(int staffId)
        {
            var resultds = new DataSet();
            List<List<string>> result = new List<List<string>>();
            var spQuery = @"EXEC	[dbo].[sp_ResetStaffPaymentApproval]
		                    @StaffId = " + staffId;

            var resultDs = ExecuteDataSet(spQuery);
        }


        public DataSet GetStaffAbsentReport(DateTime fromDate, DateTime toDate)
        {
            var spQuery = @"select st.StaffId, st.Name
                            from StaffAttandance sta 
                            inner join Staff st on sta.StaffId = st.StaffId
                            where Status = 2 and Date >= '{0}' and Date <= '{1}'";
            spQuery = string.Format(spQuery, getDate(fromDate), getDate(toDate));
            return ExecuteDataSet(spQuery);
        }

        public DataSet GetStaffLateInReport(DateTime fromDate, DateTime toDate)
        {
            string logfileLocation = ConfigurationManager.AppSettings["lateInTime"];

            var spQuery = @"select st.StaffId, st.Name, (select top(1) TimeIn from StaffAttendanceDetail 
                            where AttendanceId = sta.Id order by Id) as TimeIn
                            from StaffAttandance sta 
                            inner join Staff st on sta.StaffId = st.StaffId
                            where Status = 1 and Date >= '{0}' and Date <= '{1}'
                            and (select top(1) TimeIn from StaffAttendanceDetail 
                            where AttendanceId = sta.Id order by Id) > '{2}'";
            spQuery = string.Format(spQuery, getDate(fromDate), getDate(toDate), logfileLocation);
            return ExecuteDataSet(spQuery);
        }

        public DataSet GetExtraHoursStaffReport(DateTime fromDate, DateTime toDate)
        {
            var spQuery = @"EXEC [dbo].[sp_StaffAttendancedetailReport]
		                            @FromDate = '{0}', @ToDate = '{1}' ";
            spQuery = string.Format(spQuery, getDate(fromDate), getDate(toDate));
            ExecuteNonQuery(spQuery);

            var sql = @"Select sar.StaffId as Year, sar.Name, sar.TotalHours  as JoininDate, 
                        sar.WorkingHours  as Address, sad.TimeIn as Designation, sad.TimeOut as Education,
                        isnull(ExtraHours,0) as Total
                        from StaffAttendanceReport sar inner join StaffAttendanceDetail sad
                        on sar.AttendanceId = sad.AttendanceId
                        where isnull(ExtraHours,0) >= 4
                        order  by sar.StaffId, sad.Id";
            return ExecuteDataSet(sql);
        }

        private string getDate(DateTime date)
        {
            return date.Year + "-" + (date.Month.ToString().Length == 1 ? "0" + date.Month : date.Month.ToString())
                + "-" + (date.Day.ToString().Length == 1 ? "0" + date.Day : date.Day.ToString());
        }

    }
}
