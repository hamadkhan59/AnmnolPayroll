using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.Reports
{
    public class DAL_Student_Reports : DAL.DABase
    {
        public DataSet GetStudentAdmissionData(DateTime fromDate, DateTime toDate, int branchId)
        {

            var sql = @"select distinct st.id, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, sec.Name as Section, 
                            CONVERT(VARCHAR(10), st.AdmissionDate, 103) as AdmissionDate, st.AdmissionNo as SrNo, st.Contact_1 as Contact
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
                            where ( -1 in (-1) or cls.Id in (-1) ) and ( -1 in (-1) or sec.Id in (-1) )
                            and st.BranchId = " + branchId + " and CAST(st.AdmissionDate as date) >= '"
                            + getDate(fromDate) + "' and CAST(st.AdmissionDate as date) <= '" + getDate(toDate) + "' order by st.id";

            return ExecuteDataSet(sql);

        }

        public DataSet GetStudentAttendanceSheet(int classId, int sectionId, int genderId, int branchId)
        {

            var sql = @"select distinct st.id, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, sec.Name as Section, st.AdmissionDate, st.AdmissionNo as SrNo, st.Contact_1 as Contact
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
                            where ( 0 in ({0}) or cls.Id in ({0}) ) and ( 0 in ({1}) or sec.Id in ({1}) ) and st.LeavingStatus = 1
                            and ( 0 in ({2}) or st.GenderCode in ({2}) )
                            and st.BranchId = " + branchId + " order by st.id";
            sql = string.Format(sql, classId, sectionId, genderId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetStudentStrengthData(int branchId)
        {

//            var sql = @"select cls.Name as Class, sec.Name as Section,
//                        count(st.id) as Contact
//                        from Student st, ClassSection clsec,
//                        Classes cls, Section sec
//                        where st.ClassSectionId = clsec.ClassSectionId
//                        and cls.Id = clsec.ClassId
//                        and sec.Id = clsec.SectionId
//                        and st.BranchId = {0}
//                        Group By cls.Name, sec.Name, cls.Id, sec.Id
//                        order by cls.Id, sec.Id";

            var sql = @"select cls.Name as Class, sec.Name as Section,
						sum(case when st.GenderCode = 1 then 1 else 0 end) as Name, 
						sum(case when st.GenderCode = 2 then 1 else 0 end) as Status, 
                        count(st.id) as Contact, 
                        (select count(st.id)
                        from Student st, ClassSection clsec,
                        Classes clas, Section sec
                        where st.ClassSectionId = clsec.ClassSectionId
                        and clas.Id = clsec.ClassId
                        and sec.Id = clsec.SectionId and clas.Id = cls.Id) as SrNo
                        from Student st, ClassSection clsec,
                        Classes cls, Section sec
                        where st.ClassSectionId = clsec.ClassSectionId
                        and cls.Id = clsec.ClassId
                        and sec.Id = clsec.SectionId
                        and st.BranchId = {0}
                        Group By cls.Name, sec.Name, cls.Id, sec.Id
                        order by cls.Id, sec.Id";
            sql = string.Format(sql, branchId);
            return ExecuteDataSet(sql);
        }

        public DataSet GetStudentLeavingData(DateTime fromDate, DateTime toDate, int branchId)
        {

            var sql = @"select st.id, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, sec.Name as Section,
                            CONVERT(VARCHAR(10), st.LeavingDate, 103) as LeavingDate, lr.LeavingReason, st.ClearDues, 
                            st.AdmissionNo as SrNo, st.Contact_1 as Contact
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
							left outer join LeavingReason lr on lr.ReasonId = st.ReasonId
                            where ( -1 in (-1) or cls.Id in (-1) ) and ( -1 in (-1) or sec.Id in (-1) )
                            and st.BranchId = " + branchId + " and CAST(st.LeavingDate as date) >= '" + getDate(fromDate) +
                            "' and CAST(st.LeavingDate as date) <= '" + getDate(toDate) + "'  and st.LeavingStatus != 1 order by st.id";

            return ExecuteDataSet(sql);

        }

        public DataSet GetStudentAdmissionData(DateTime fromDate, DateTime toDate, int classId, int secId, int branchId)
        {

            var sql = @"select st.RollNumber, st.Name, st.FatherName, cls.Name as Class, sec.Name as Section, 
                            CONVERT(VARCHAR(10), st.AdmissionDate, 103) as AdmissionDate, st.AdmissionNo as SrNo, st.Contact_1 as Contact
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
                            where ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) and ( 0 in (" + secId + ") or sec.Id in (" + secId + ") )" +
                                           "and st.BranchId = " + branchId +
                        "and CAST(st.AdmissionDate as date) >= '" + getDate(fromDate) + "' and CAST(st.AdmissionDate as date) <= '" + getDate(toDate) + "' order by st.id";

            return ExecuteDataSet(sql);

        }

        public DataSet GetStudentLeavingData(DateTime fromDate, DateTime toDate, int classId, int secId, int branchId)
        {

            var sql = @"select st.id, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, sec.Name as Section, 
                            CONVERT(VARCHAR(10), st.LeavingDate, 103) as LeavingDate, lr.LeavingReason, st.ClearDues, 
                            st.AdmissionNo as SrNo, st.Contact_1 as Contact
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
							left outer join LeavingReason lr on lr.ReasonId = st.ReasonId
                            where ( 0 in (" + classId + ") or cls.Id in (" + classId + ") ) and ( 0 in (" + secId + ") or sec.Id in (" + secId + ") )" +
                                            "and st.BranchId = " + branchId +
                        "and CAST(st.LeavingDate as date) >= '" + getDate(fromDate) + "' and CAST(st.LeavingDate as date) <= '" + getDate(toDate) + "' and st.LeavingStatus != 1 order by st.id";

            return ExecuteDataSet(sql);

        }

        public DataSet GetYearlyStudentAdmissionData(string year, int classId, int secId, int branchId)
        {

            var sql = @"select cls.Name as Class, sec.Name as Section, convert(varchar, DATEPART(m,AdmissionDate))+' - '+ DateName( month , DateAdd( month ,  DATEPART(m,AdmissionDate) , 0 ) - 1 ) as AdmissionMonth,
                                count(AdmissionDate)  as AdmissionCount, count(case when st.LeavingStatus = 1 then null else 1 end) as LeavingCount, st.AdmissionNo as SrNo, st.Contact_1 as Contact
                                from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							    Left outer join Classes cls on clsec.ClassId = cls.Id 
                                left outer join Section sec on clsec.SectionId = sec.Id
                                where ( '-1' in ('" + year + "') or  DATEPART(YYYY,AdmissionDate) in ( '" + year + "')) and ( 0 in (" + classId + ") or st.id in (" + classId +
                                ") ) and ( 0 in (" + secId + ") or st.id in (" + secId + ") ) " + "and st.BranchId = " + branchId +
                            " group by DATEPART(m,AdmissionDate), cls.Name, sec.Name, st.AdmissionNo, st.Contact_1";

            return ExecuteDataSet(sql);

        }

        public DataSet GetStudentAttandanceData(int classId, int secId, string rollno, string name, DateTime toDate, DateTime fromDate, int branchId, int attendanceStatus)
        {
            if (rollno.Length == 0)
                rollno = "0";
            var sql = @"select st.RollNumber, st.Name, cls.Name as Class, 
                            sec.Name as Section, CONVERT(VARCHAR(10),  att.AttandanceDate, 103) as AttandanceDate, stas.CodeName AS Status, st.FatherName, 
                            st.Contact_1 as Contact, st.AdmissionNo as SrNo
                            from Attandance att, Student st, Classes cls, Section sec, 
                            ClassSection clsSec, StudentAttendanceStatus stas
                            where att.StudentID = st.id and cls.Id = clsSec.ClassId 
                            and sec.Id = clsSec.SectionId and st.Id = att.StudentID
                            and st.ClassSectionId = clsSec.ClassSectionId
							and stas.Id = att.StatusId
                            and CAST(att.AttandanceDate as date) >= '" + getDate(fromDate) + "' and CAST(att.AttandanceDate as date) <= '" + getDate(toDate) + "'" +
            " and (0 in (" + attendanceStatus + ") or stas.Id in (" + attendanceStatus + ")) and (0 in (" + secId + ") or sec.Id in (" + secId + ")) and (0 in (" + classId + ") or cls.Id in (" + classId + ")) "
            + " and (0 in ('" + rollno + "') or st.RollNumber in (" + rollno + ")) " +
            "and st.BranchId = " + branchId + 
            " and  st.Name like '%" + name + "%' order by st.RollNumber, AttandanceDate ";

            return ExecuteDataSet(sql);

        }

        public DataSet GetDailyAttendanceSheet(DateTime fromDate, bool isEvs = false)
        {
            var query = @"select cls.Name as class, sec.Name as Section, cls.Id, sec.id, '' as IsEVS,
                        count(st.id) as Total, sum(case when st.GenderCode = 1 then 1 else 0 end) as BoysTotal,
                         sum(case when st.GenderCode = 2 then 1 else 0 end) as GirlsTotal,
                         sum(case when (st.GenderCode = 1 and att.StatusId = 1) then 1 else 0 end) as BoysPresent,
                          sum(case when (st.GenderCode = 2 and att.StatusId = 1) then 1 else 0 end) as GirlsPresent,
                           sum(case when (st.GenderCode = 1 and att.StatusId = 2) then 1 else 0 end) as BoysAbsent,
                          sum(case when (st.GenderCode = 2 and att.StatusId = 2) then 1 else 0 end) as GirlsAbsent,
                          sum(case when (att.StatusId = 1) then 1 else 0 end) as PresentCount,
                          sum(case when (att.StatusId = 2) then 1 else 0 end) as AbsentCount
                         from Attandance att 
                        inner join Student st on st.id = att.StudentID
                        inner join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                        inner join Classes cls on cls.Id = clsec.ClassId
                        inner join Section sec on sec.Id = clsec.SectionId
                        inner join Gender gen on gen.Id = st.GenderCode
                        where CAST(att.AttandanceDate as date) = '{0}' and st.BranchId = 1 and st.LeavingStatus = 1
                        group by cls.Id, sec.id, cls.Name, sec.Name
                        order by cls.Id, sec.id";
            if (isEvs)
            {
                query = @"select cls.Name as class, sec.Name as Section, cls.Id, sec.id, 'EVS Students' as IsEVS,
                        count(st.id) as Total, sum(case when st.GenderCode = 1 then 1 else 0 end) as BoysTotal,
                         sum(case when st.GenderCode = 2 then 1 else 0 end) as GirlsTotal,
                         sum(case when (st.GenderCode = 1 and att.StatusId = 1) then 1 else 0 end) as BoysPresent,
                          sum(case when (st.GenderCode = 2 and att.StatusId = 1) then 1 else 0 end) as GirlsPresent,
                           sum(case when (st.GenderCode = 1 and att.StatusId = 2) then 1 else 0 end) as BoysAbsent,
                          sum(case when (st.GenderCode = 2 and att.StatusId = 2) then 1 else 0 end) as GirlsAbsent,
                          sum(case when (att.StatusId = 1) then 1 else 0 end) as PresentCount,
                          sum(case when (att.StatusId = 2) then 1 else 0 end) as AbsentCount
                         from Attandance att 
                        inner join Student st on st.id = att.StudentID
                        inner join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                        inner join Classes cls on cls.Id = clsec.ClassId
                        inner join Section sec on sec.Id = clsec.SectionId
                        inner join Gender gen on gen.Id = st.GenderCode
                        where CAST(att.AttandanceDate as date) = '{0}' and st.BranchId = 1 and st.isEvs <> 1 and st.LeavingStatus = 1
                        group by cls.Id, sec.id, cls.Name, sec.Name
                        
Union 

select cls.Name as class, sec.Name as Section, cls.Id, sec.id, 'Non EVS Students' as IsEVS,
                        count(st.id) as Total, sum(case when st.GenderCode = 1 then 1 else 0 end) as BoysTotal,
                         sum(case when st.GenderCode = 2 then 1 else 0 end) as GirlsTotal,
                         sum(case when (st.GenderCode = 1 and att.StatusId = 1) then 1 else 0 end) as BoysPresent,
                          sum(case when (st.GenderCode = 2 and att.StatusId = 1) then 1 else 0 end) as GirlsPresent,
                           sum(case when (st.GenderCode = 1 and att.StatusId = 2) then 1 else 0 end) as BoysAbsent,
                          sum(case when (st.GenderCode = 2 and att.StatusId = 2) then 1 else 0 end) as GirlsAbsent,
                          sum(case when (att.StatusId = 1) then 1 else 0 end) as PresentCount,
                          sum(case when (att.StatusId = 2) then 1 else 0 end) as AbsentCount
                         from Attandance att 
                        inner join Student st on st.id = att.StudentID
                        inner join ClassSection clsec on clsec.ClassSectionId = st.ClassSectionId
                        inner join Classes cls on cls.Id = clsec.ClassId
                        inner join Section sec on sec.Id = clsec.SectionId
                        inner join Gender gen on gen.Id = st.GenderCode
                        where CAST(att.AttandanceDate as date) = '{0}' and st.BranchId = 1 and st.isEvs = 1 and st.LeavingStatus = 1
                        group by cls.Id, sec.id, cls.Name, sec.Name
                        order by cls.Id, sec.id";
            }
            query = string.Format(query, getDate(fromDate));
            return ExecuteDataSet(query);

        }

        public DataSet GetStudentContactDirectory(int classId, int sectionId, int branchId)
        {

//            var sql = @"select st.SrNo, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, 
//                            sec.Name as Section, st.Contact_1 as Contact, st.MotherContact1 as AttandanceDate
//                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
//							Left outer join Classes cls on clsec.ClassId = cls.Id 
//                            left outer join Section sec on clsec.SectionId = sec.Id
//                            where (st.LeavingDate is null or st.LeavingDate = st.AdmissionDate)
//                            order by AdmissionDate";
            var sql = @"select st.id, st.AdmissionNo as SrNo, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, 
                            sec.Name as Section, st.Contact_1 as Contact, st.MotherName as MotherName,
                            st.FatherCNIC as CNIC, st.FatherOccupation as Occupation,
                            CONVERT(VARCHAR(10),  st.DateOfBirth, 103) as DOB, CONVERT(VARCHAR(10),  st.AdmissionDate, 103) as AdmissionDate,
                            st.CurrentAddress as Address, case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_PICS,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_CNIC,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_BIRTH,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_LEAVING
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
                            where st.LeavingStatus = 1
                            and (0 in ({0}) or cls.id in ({0})) and (0 in ({1}) or sec.id in ({1}))
                            and st.BranchId = {2}
                            order by st.id";
            sql = string.Format(sql, classId, sectionId, branchId);

            return ExecuteDataSet(sql);


        }

        public DataSet GetStudentContactDirectoryAdmissionNoWise(int classId, int sectionId, int branchId)
        {
            var sql = @"select st.id, st.AdmissionNo as SrNo, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, 
                            sec.Name as Section, st.Contact_1 as Contact, st.MotherName as MotherName,
                            st.FatherCNIC as CNIC, st.FatherOccupation as Occupation, st.DateOfBirth as DOB, st.AdmissionDate,
                            st.CurrentAddress as Address, case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_PICS,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_CNIC,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_BIRTH,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_LEAVING
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
                            where st.LeavingStatus = 1
                            and (0 in ({0}) or cls.id in ({0})) and (0 in ({1}) or sec.id in ({1}))
                            and st.BranchId = {2}
                            order by cast(st.AdmissionNo as int)";
            sql = string.Format(sql, classId, sectionId, branchId);

            return ExecuteDataSet(sql);
        }


        public DataSet GetStudentSmsHistoryReport(int classId, int secId, string rollno, string name, DateTime toDate, DateTime fromDate, int branchId)
        {
            if (rollno.Length == 0)
                rollno = "0";
            var sql = @"select st.RollNumber, st.Name, cls.Name as Class, 
                            sec.Name as Section, CONVERT(VARCHAR(10),  smsHis.SentDate, 103) as AdmissionDate,
                            smsHis.Message AS Address, smsHis.Response AS Status, st.FatherName, st.Contact_1 as Contact, st.AdmissionNo as SrNo
                            from SmsHistory smsHis, Student st, Classes cls, Section sec, 
                            ClassSection clsSec 
                            where smsHis.StdId = st.id and cls.Id = clsSec.ClassId 
                            and sec.Id = clsSec.SectionId and st.ClassSectionId = clsSec.ClassSectionId
							and CAST(smsHis.SentDate as date) >= '" + getDate(fromDate) + "' and CAST(smsHis.SentDate as date) <= '" + getDate(toDate) + "'" +
            " and (0 in (" + secId + ") or sec.Id in (" + secId + ")) and (0 in (" + classId + ") or cls.Id in (" + classId + ")) "
            + " and (0 in ('" + rollno + "') or st.RollNumber in (" + rollno + ")) " +
            "and st.BranchId = " + branchId +
            " and  st.Name like '%" + name + "%' order by smsHis.id desc ";

            return ExecuteDataSet(sql);
        }

        public DataSet GetStudentTerminatedReport(int classId, int sectionId, int branchId)
        {

            //            var sql = @"select st.SrNo, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, 
            //                            sec.Name as Section, st.Contact_1 as Contact, st.MotherContact1 as AttandanceDate
            //                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
            //							Left outer join Classes cls on clsec.ClassId = cls.Id 
            //                            left outer join Section sec on clsec.SectionId = sec.Id
            //                            where (st.LeavingDate is null or st.LeavingDate = st.AdmissionDate)
            //                            order by AdmissionDate";
            var sql = @"select st.id, st.AdmissionNo as SrNo, st.RollNumber, st.Name, st.FatherName, cls.Name as Class, 
                            sec.Name as Section, st.Contact_1 as Contact, st.MotherName as MotherName,
                            st.FatherCNIC as CNIC, st.FatherOccupation as Occupation, st.DateOfBirth as DOB, st.AdmissionDate,
                            st.CurrentAddress as Address, case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_PICS,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_CNIC,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_BIRTH,
                             case when st.ChildPictures = 1 then 'Yes' else 'No' end AS AVA_LEAVING,
                             case when st.LeavingStatus = 2 then 'DISCHARGED' else 'TERMINATED' end AS Status
                            from Student st Left outer join ClassSection clsec on st.ClassSectionId = clsec.ClassSectionId
							Left outer join Classes cls on clsec.ClassId = cls.Id 
                            left outer join Section sec on clsec.SectionId = sec.Id
                            where (NOT(st.LeavingStatus = 1))
                            and (0 in ({0}) or cls.id in ({0})) and (0 in ({1}) or sec.id in ({1}))
                            and st.BranchId = {2}
                            order by st.id";
            sql = string.Format(sql, classId, sectionId, branchId);

            return ExecuteDataSet(sql);


        }

        public DataSet GetStudentStrength()
        {
            string sql = @"EXEC	 [dbo].[sp_Student_Strength]";

            return ExecuteDataSet(sql);
        }

        private string getDate(DateTime date)
        {
            return date.Year + "-" + (date.Month.ToString().Length == 1 ? "0" + date.Month : date.Month.ToString())
                + "-" + (date.Day.ToString().Length == 1 ? "0" + date.Day : date.Day.ToString());
        }
    }
}
