using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.Http;
using SMS_DAL;
using SMS_DAL.ViewModel;
using Common;
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.Reports;
using System.Data;

namespace SMS_Web.Controllers
{
    public class CommonController : ApiController
    {
        public SC_WEBEntities2 db = SessionHelper.dbContext;
        private ITransportRepository accountRepo = new TransportRepositoryImp(new SC_WEBEntities2());
        private IStaffRepository staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        private DAL_Staff_Reports reportStaff = new DAL_Staff_Reports();

        [HttpGet]
        public HttpResponseMessage getReports(int moduleId)
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            List<REPORT> report = db.REPORTS.Where(x => x.MODULE_ID == moduleId).ToList();
            foreach(var data in report)
            {
                options += "<option value='" + data.REPORT_ID + "'>" + data.REPORT_NAME + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getReportFilters(int reportId)
        {
            var filters = (from r in db.REPORTS join rf in db.REPORT_FILTERS on r.REPORT_ID equals rf.REPORT_ID
                                            where r.REPORT_ID == reportId
                                            orderby r.REPORT_ID
                                            select new { rf.FILTER_ID
                                                        }).ToList(); 
            

            return Request.CreateResponse(HttpStatusCode.OK, filters);
        }
        [HttpGet]
        public HttpResponseMessage getYears()
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            for (int i = 2010; i <= DateTime.Now.Year;i++ )
            {
                options += "<option value='" + i + "'>" + i + "</option>";
            }
            return Request.CreateResponse(HttpStatusCode.OK, options);
        }
        [HttpGet]
        public HttpResponseMessage loadModes()
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            options += "<option value='" + 0 + "'>" + "Both" + "</option>";
            options += "<option value='" + 1 + "'>" + "Debit" + "</option>";
            options += "<option value='" + 2 + "'>" + "Credit" + "</option>";
            return Request.CreateResponse(HttpStatusCode.OK, options);
        }
        [HttpPost]
        public HttpResponseMessage setStudentReportSession(ReportModel model)
        {
            model.firstLevel = 0;
            model.secondLevel = 0;
            model.thirdLevel = 0;
            model.fourthLevel = 0;
            model.fifthLevel = 0;
            model.mode = 0;
            model.month = "";
            //if(model.rollNo == null || model.rollNo == string.Empty)
            //{
            //    model.rollNo = "0";
            //}

            SessionManager.SetReportSession(model);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        [HttpPost]
        public HttpResponseMessage setFinanceReportSession(ReportModel model)
        {
            //model.firstLevel = 0;
            //model.secondLevel = 0;
            //model.thirdLevel = 0;
            //model.fourthLevel = 0;
            //model.fifthLevel = 0;
            //model.mode = 0;
            //model.month = "";
            //if(model.rollNo == null || model.rollNo == string.Empty)
            //{
            //    model.rollNo = "0";
            //}

            SessionManager.SetReportSession(model);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        [HttpPost]
        public HttpResponseMessage setFeeReportSession(ReportModel model)
        {
            model.firstLevel = 0;
            model.secondLevel = 0;
            model.thirdLevel = 0;
            model.fourthLevel = 0;
            model.fifthLevel = 0;
            model.mode = 0;
            //model.month = "";
            //if (model.rollNo == null || model.rollNo == string.Empty)
            //{
            //    model.rollNo = "0";
            //}

            SessionManager.SetReportSession(model);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        [HttpPost]
        public HttpResponseMessage setStaffReportSession(ReportModel model)
        {
            model.firstLevel = 0;
            model.secondLevel = 0;
            model.thirdLevel = 0;
            model.fourthLevel = 0;
            model.fifthLevel = 0;
            model.mode = 0;
            //model.month = "";
            //if (model.rollNo == null || model.rollNo == string.Empty)
            //{
            //    model.rollNo = "0";
            //}

            SessionManager.SetReportSession(model);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
        

        [HttpGet]
        public HttpResponseMessage loadSecondLevel(int firstId)
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            List<FinanceSeccondLvlAccount> second = db.FinanceSeccondLvlAccounts.Where(x => x.FirstLvlAccountId == firstId).ToList();
            foreach (var data in second)
            {
                options += "<option value='" + data.Id + "'>" + data.AccountName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }
        [HttpGet]
        public HttpResponseMessage loadthirdLevel(int secondId)
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            List<FinanceThirdLvlAccount> third = db.FinanceThirdLvlAccounts.Where(x => x.SeccondLvlAccountId == secondId).ToList();
            foreach (var data in third)
            {
                options += "<option value='" + data.Id + "'>" + data.AccountName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }
        [HttpGet]
        public HttpResponseMessage loadFourthLevel(int thirdId)
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            List<FinanceFourthLvlAccount> fourth = db.FinanceFourthLvlAccounts.Where(x => x.ThirdLvlAccountId == thirdId).ToList();
            foreach (var data in fourth)
            {
                options += "<option value='" + data.Id + "'>" + data.AccountName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }
        [HttpGet]
        public HttpResponseMessage loadFifthLevel(int fourthId)
        {
            string options = "";
            options += "<option value='" + "-1" + "'>" + "Please Select" + "</option>";
            List<FinanceFifthLvlAccount> fifth = db.FinanceFifthLvlAccounts.Where(x => x.FourthLvlAccountId == fourthId).ToList();
            foreach (var data in fifth)
            {
                options += "<option value='" + data.Id + "'>" + data.AccountName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }
        //[HttpPost]
        //[Route("api/Common/loadPreviousLevel")]
        //public HttpResponseMessage loadPreviousLevel(int level, int id)
        //{
        //    string options = "";
        //    int level = obj.level;
        //    int id = obj.id;
        //    if (level == 5)
        //    {
        //        FinanceFifthLvlAccount fifth = db.FinanceFifthLvlAccounts.Where(x => x.AccountId == id).FirstOrDefault();
        //        FinanceFourthLvlAccount fourth = db.FinanceFourthLvlAccounts.Where(x => x.AccountId == fifth.FourthLvlAccountId).FirstOrDefault();
        //        if (fourth != null)
        //        {
        //            options = fourth.AccountName;
        //        }

        //    }
        //    else if (level == 4)
        //    {
        //        FinanceFourthLvlAccount fourth = db.FinanceFourthLvlAccounts.Where(x => x.AccountId == id).FirstOrDefault();
        //        FinanceThirdLvlAccount third = db.FinanceThirdLvlAccounts.Where(x => x.AccountId == fourth.ThirdLvlAccountId).FirstOrDefault();
        //        if (third != null)
        //        {
        //            options = third.AccountName;
        //        }
        //    }
        //    else if (level == 3)
        //    {
        //        FinanceThirdLvlAccount third = db.FinanceThirdLvlAccounts.Where(x => x.AccountId == id).FirstOrDefault();
        //        FinanceSeccondLvlAccount second = db.FinanceSeccondLvlAccounts.Where(x => x.AccountId == third.SeccondLvlAccountId).FirstOrDefault();
        //        if (second != null)
        //        {
        //            options = second.AccountName;
        //        }
        //    }
        //    else if (level == 2)
        //    {
        //        FinanceSeccondLvlAccount second = db.FinanceSeccondLvlAccounts.Where(x => x.AccountId == id).FirstOrDefault();
        //        FinanceSeccondLvlAccount first = db.FinanceSeccondLvlAccounts.Where(x => x.AccountId == second.FirstLvlAccountId).FirstOrDefault();
        //        if (first != null)
        //        {
        //            options = first.AccountName;
        //        }
        //    }
        //    else
        //    {
        //        options = "No previous level.";
        //    }


        //    return Request.CreateResponse(HttpStatusCode.OK, options);
        //}
        [HttpGet]
        [AttributeRouting.Web.Mvc.Route("api/Common/loadPreviousLevels")]
        public HttpResponseMessage loadPreviousLevels(int flag,int id)
        {
            List<FinanceLevel> obj = new List<FinanceLevel>();
            FinanceLevel data1 = new FinanceLevel();
            FinanceLevel data2= new FinanceLevel();
            FinanceLevel data3 = new FinanceLevel();
            FinanceFourthLvlAccount fourth = db.FinanceFourthLvlAccounts.Where(x => x.Id == id).FirstOrDefault();

            FinanceThirdLvlAccount third = db.FinanceThirdLvlAccounts.Where(x => x.Id == fourth.ThirdLvlAccountId).FirstOrDefault();
            data1.id = third.Id;
            data1.name= third.AccountName;
            obj.Add(data1);

            FinanceSeccondLvlAccount second = db.FinanceSeccondLvlAccounts.Where(x => x.Id == third.SeccondLvlAccountId).FirstOrDefault();
            data2.id = second.Id;
            data2.name = second.AccountName;
            obj.Add(data2);

            FinanceFirstLvlAccount first = db.FinanceFirstLvlAccounts.Where(x => x.Id == second.FirstLvlAccountId).FirstOrDefault();
            data3.id = first.Id;
            data3.name = first.AccountName;
            obj.Add(data3);

            FinanceLevelList a = new FinanceLevelList();
            a.list = obj;
            return Request.CreateResponse(HttpStatusCode.OK, a);
        }

        [HttpGet]
        public HttpResponseMessage UpdateBioMatrixLogCount()
        {
            string response = "SUCCESS";
            staffRepo.UpdateBioMatrixLogCount(0);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage getSections(int classId, int isLoadAll = 0)
        {
            string options = "";
            if(isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            List<SectionDD> sections = SessionHelper.SectionListDD(classId);
            foreach (var sec in sections)
            {
                options += "<option value='" + sec.Id + "'>" + sec.Name + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getDriverStops(int driverId)
        {
            string options = "";
            
            var list = accountRepo.GetAllTransportDriverStopModel();
            list = list.Where(x => x.DriverId == driverId).ToList();
            foreach (var stop in list)
            {
                var tableRow = "<tr><td>" + stop.StopName +
               "</td><td>" + stop.StopRent +"</td></tr>";
                options += tableRow;
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getFinanceAccount(int typeId, int branchId, int isLoadAll)
        {
            string options = "";
            if (isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            var list = SessionHelper.FeeFinanceAccountList(branchId);
            list = list.Where(x => x.ThirdLvlAccountId == typeId).ToList();
            foreach (var account in list)
            {
                options += "<option value='" + account.Id + "'>" + account.AccountName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getClassChallan(int classId, int branchId, int isLoadAll)
        {
            string options = "";
            if (isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            var list = SessionHelper.ChallanList(branchId);
            list = list.Where(x => x.ClassId == classId).ToList();
            foreach (var account in list)
            {
                options += "<option value='" + account.Id + "'>" + account.Name + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getDesignation(int categoryId, int isLoadAll = 0)
        {
            string options = "";
            if (isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            var designations = SessionHelper.DesignationDD(categoryId);
            foreach (var desgn in designations)
            {
                options += "<option value='" + desgn.Id + "'>" + desgn.Name + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getClassSubject(int classId, int sectionId, int isLoadAll = 0)
        {
            string options = "";
            if (isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            var subjects = SessionHelper.ClassSubjectList(classId, sectionId);
            foreach (var subj in subjects)
            {
                options += "<option value='" + subj.SubjectId + "'>" + subj.SubjectName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getTermByYear(int yearId, int branchId, int isLoadAll = 0)
        {
            string options = "";
            if (isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            var terms = SessionHelper.ExamTermList(branchId);
            terms = terms.Where(x => x.Year == yearId).ToList();
            foreach (var term in terms)
            {
                options += "<option value='" + term.Id + "'>" + term.TermName + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getExamByTerm(int termId, int branchId, int isLoadAll = 0)
        {
            string options = "";
            if (isLoadAll == 1)
                options += "<option value='" + "0" + "'>" + "All" + "</option>";
            var exams = SessionHelper.ExamTypeList(branchId);
            exams = exams.Where(x => x.TermId == termId).ToList();
            foreach (var exam in exams)
            {
                options += "<option value='" + exam.Id + "'>" + exam.Name + "</option>";
            }

            return Request.CreateResponse(HttpStatusCode.OK, options);
        }

        [HttpGet]
        public HttpResponseMessage getLateInStaffReportData()
        {
            DataSet ds = new DataSet();
            ds = reportStaff.GetStaffLateInReport(DateTime.Now, DateTime.Now);
            string tableBody = @"<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>StaffId</b></td> <td> <b>Name</b> </td> <td> <b> TimeIn</b> </td></tr>";

            if (ds != null && ds.Tables != null)
            {
                for (int loopCount = 0; loopCount < ds.Tables[0].Rows.Count; loopCount++)
                {
                    tableBody += "<tr><td>" + ds.Tables[0].Rows[loopCount][0] + "</td><td> " + ds.Tables[0].Rows[loopCount][1] + "</td><td> " + ds.Tables[0].Rows[loopCount][2] + "</td> </tr>";
                }
            }
            tableBody += "</table>";
            return Request.CreateResponse(HttpStatusCode.OK, tableBody);
        }

        [HttpGet]
        public HttpResponseMessage getAbsentStaffReportData()
        {
            DataSet ds = new DataSet();
            ds = reportStaff.GetStaffAbsentReport(DateTime.Now, DateTime.Now);
            string tableBody = @"<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>StaffId</b></td> <td> <b>Name</b> </td><td> <b>Status</b> </td></tr>";

            if (ds != null && ds.Tables != null)
            {
                for (int loopCount = 0; loopCount < ds.Tables[0].Rows.Count; loopCount++)
                {
                    tableBody += "<tr><td>" + ds.Tables[0].Rows[loopCount][0] + "</td><td> " + ds.Tables[0].Rows[loopCount][1] + "</td><td>Absent</td></tr>";
                }
            }

            tableBody += "</table>";
            return Request.CreateResponse(HttpStatusCode.OK, tableBody);
        }

        [HttpGet]
        public HttpResponseMessage getExtraHoursStaffReportData()
        {
            DataSet ds = new DataSet();
            ds = reportStaff.GetExtraHoursStaffReport(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1));
            string tableBody = @"<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 600 + "><tr bgcolor='#4da6ff'><td><b>StaffId</b></td> <td> <b>Name</b> </td><td> <b>Total Hours</b> </td>";
            tableBody += "<td><b>Working Hours</b></td> <td> <b>Extra Hours</b> </td><td> <b>Time In</b> </td><td> <b>Time Out</b> </td></tr>";

            if (ds != null && ds.Tables != null)
            {
                for (int loopCount = 0; loopCount < ds.Tables[0].Rows.Count; loopCount++)
                {
                    var row = "";
                    if (loopCount > 0 && ds.Tables[0].Rows[loopCount][0].ToString() == ds.Tables[0].Rows[loopCount - 1][0].ToString())
                    {
                        row += "<tr><td></td><td></td><td></td><td></td><td></td>";
                        row += "<td>" + ds.Tables[0].Rows[loopCount][4] + "</td><td> " + ds.Tables[0].Rows[loopCount][5] + "</td></tr>";
                    }
                    else
                    {
                        row += "<tr><td>" + ds.Tables[0].Rows[loopCount][0] + "</td><td> " + ds.Tables[0].Rows[loopCount][1] + "</td>";
                        row += "<td>" + ds.Tables[0].Rows[loopCount][2] + "</td><td> " + ds.Tables[0].Rows[loopCount][3] + "</td>";
                        row += "<td>" + ds.Tables[0].Rows[loopCount][6] + "</td><td> " + ds.Tables[0].Rows[loopCount][4] + "</td>";
                        row += "<td>" + ds.Tables[0].Rows[loopCount][5] + "</td></tr>";
                    }
                    tableBody += row;
                }
            }

            tableBody += "</table>";
            return Request.CreateResponse(HttpStatusCode.OK, tableBody);
        }
    }
}
