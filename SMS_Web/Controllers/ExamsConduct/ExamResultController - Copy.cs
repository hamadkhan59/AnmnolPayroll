using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using PdfSharp.Pdf;
using SMS.Modules.BuildPdf;
using System.IO;
using System.IO.Compression;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class ExamResultController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        static bool isPrint = false;
        static int errorCode = 0, classErrorCode = 0, grandErrorCode;
        static int examTotalMarks = 0, examPassPercentage = 0;
        static int sheetSubjectId = 0;
        static List<int> totalExamMarksList = null;
        static List<int> examPassPercentageList = null;
        static List<string> courseNameList = null;
        //
        // GET: /ExamResult/

        
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        IClassSubjectRepository clasSubjRepo;
        ISubjectRepository subjRepo;
        public ExamResultController()
        {

            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            clasSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());;
            subjRepo = new SubjectRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_MARKS_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassList(Session.SessionID), "Id", "Name");
            ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
            ViewBag.ExamId = new SelectList(SessionHelper.ExamTypeList(Session.SessionID), "Id", "Name");
            ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
            ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
            ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
            ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
            ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
            ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);
            ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");
            voidSetSearchVeriables();

            if (Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] != null && (bool)Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] == true)
            {
                Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] = false;
                ViewData["examSheet"] = SearchMarksSheet();
            }
            ViewData["Error"] = errorCode;
            errorCode = 0;
            ViewData["totalMarks"] = examTotalMarks;
            ViewData["passPerecentage"] = examPassPercentage;
            examTotalMarks = examPassPercentage = 0;
            return View("");
        }


        private void voidSetSearchVeriables()
        {
            if (Session[ConstHelper.GLOBAL_CLASS_ID] != null)
            {
                ViewData["GlobalClassId"] = (int)Session[ConstHelper.GLOBAL_CLASS_ID];
                Session[ConstHelper.GLOBAL_CLASS_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SECTION_ID] != null)
            {
                ViewData["GlobalSectionId"] = (int)Session[ConstHelper.GLOBAL_SECTION_ID];
                Session[ConstHelper.GLOBAL_SECTION_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_YEAR_ID] != null)
            {
                ViewData["GlobalYearId"] = (int)Session[ConstHelper.GLOBAL_YEAR_ID];
                Session[ConstHelper.GLOBAL_YEAR_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SUBJECT_ID] != null)
            {
                ViewData["GlobalSujectId"] = (int)Session[ConstHelper.GLOBAL_SUBJECT_ID];
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_TERM_ID] != null)
            {
                ViewData["GlobalTermId"] = (int)Session[ConstHelper.GLOBAL_TERM_ID];
                Session[ConstHelper.GLOBAL_TERM_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] != null)
            {
                ViewData["GLobalExamTypeId"] = (int)Session[ConstHelper.GLOBAL_EXAM_TYPE_ID];
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = null;
            }
        }


        public ActionResult Promote()
        {
            ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassList(Session.SessionID), "Id", "Name");
            ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
            ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
            ViewData["Error"] = classErrorCode;
            classErrorCode = 0;
            List<StudentModel> list = new List<StudentModel>();
            if (Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] != null)
                list = (List<StudentModel>)Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH];

            voidSetSearchVeriables();
            Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = null;
            return View(list);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPromoteStudent(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            ClassId = string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId;
            SectionId = string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId;

            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(SectionId);

            int classSectionId = 0;
            if (classId > 0 && sectionId > 0)
                classSectionId = classSecRepo.GetClassSectionId(classId, sectionId);

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            if (classSectionId > 0)
                Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = studentRepo.SearchStudents(RollNo, Name, FatherName, classSectionId, FatherCnic, branchId, AdmissionNo, FatherContact);
            else if (classId > 0)
                Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = studentRepo.SearchClassStudents(RollNo, Name, FatherName, classId, FatherCnic, branchId, AdmissionNo, FatherContact);
            else
                Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = studentRepo.SearchStudents(RollNo, Name, FatherName, FatherCnic, branchId, AdmissionNo, FatherContact);

            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;

            return RedirectToAction("Promote");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePromoteStudent(int[] studentIds, int PromoteClassId, int PromoteSectionId)
        {
            try
            {
                int clasSectionId = classSecRepo.GetClassSectionId(PromoteClassId, PromoteSectionId);
                if (clasSectionId > 0)
                {
                    foreach (int id in studentIds)
                    {
                        var student = studentRepo.GetStudentById(id);
                        student.ClassSectionId = clasSectionId;
                        studentRepo.UpdateStudent(student);
                    }
                }
                classErrorCode = 100;
            }
            catch (Exception exc)
            {
                classErrorCode = 420;
            }
            return RedirectToAction("Promote");
        }

        public ActionResult ClassResult(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_CLASS_RESULT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassList(Session.SessionID), "Id", "Name");
            ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
            ViewBag.ExamId = new SelectList(SessionHelper.ExamTypeList(Session.SessionID), "Id", "Name");
            ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
            ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
            ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
            ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
            ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
            
            
            examTotalMarks = examPassPercentage = 0;
            List<ExamResultViewModel> ervmList = null;
            if (Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] == true)
            {
                Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] = false;
                ervmList = SearchClassSheet();
            }
            ViewData["totalMarksList"] = totalExamMarksList;
            ViewData["passPerecentageList"] = examPassPercentageList;
            ViewData["courseNameList"] = courseNameList;
            ViewData["Error"] = classErrorCode;

            voidSetSearchVeriables();

            classErrorCode = 0;
            if(ervmList != null && ervmList.Count > 0)
                return View(ervmList);
            else
                return View("");
        }

        public ActionResult GrandResult(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_GRAND_RESULT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassList(Session.SessionID), "Id", "Name");
            ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
            ViewBag.ExamId = new SelectList(SessionHelper.ExamTypeList(Session.SessionID), "Id", "Name");
            ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
            ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
            ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
            ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
            ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
            ViewData["Operation"] = 0;
            examTotalMarks = examPassPercentage = 0;
            List<List<string>> resultDataset = null;
            if (Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] == true)
            {
                Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] = false;
                resultDataset = SearchGrandResult(branchId);
            }
            ViewData["Error"] = grandErrorCode;
            grandErrorCode = 0;
            ViewData["totalMarksList"] = totalExamMarksList;
            ViewData["passPerecentageList"] = examPassPercentageList;
            ViewData["courseNameList"] = courseNameList;
            voidSetSearchVeriables();
            if (resultDataset != null && resultDataset.Count > 0)
                return View(resultDataset);
            else
                return View("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchMarksSheet(string ClassId, string SectionId, string Year, string ExamTypeId, string subjectId, string TermId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(ClassId) == true || string.IsNullOrEmpty(SectionId) == true || string.IsNullOrEmpty(Year) == true
                || string.IsNullOrEmpty(ExamTypeId) == true || string.IsNullOrEmpty(subjectId) == true)
            {
                errorCode = 1420;
            }
            else
            {
                Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] = true;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                Session[ConstHelper.SEARCH_EXAM_RESULT_EXAM_ID] = int.Parse(ExamTypeId);
                Session[ConstHelper.SEARCH_EXAM_RESULT_SUBJECT_ID] = int.Parse(subjectId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = int.Parse(ExamTypeId);
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = int.Parse(subjectId);
                Session[ConstHelper.GLOBAL_TERM_ID] = int.Parse(TermId);

            }
            return RedirectToAction("Index", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchClassSheet(string ClassId, string SectionId, string Year, string ExamTypeId, string TermId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] = true;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                Session[ConstHelper.SEARCH_CLASS_SHEET_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID] = int.Parse(ExamTypeId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = int.Parse(ExamTypeId);
                Session[ConstHelper.GLOBAL_TERM_ID] = int.Parse(TermId);

            }
            catch (Exception exc)
            { }
            return RedirectToAction("ClassResult", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchGrandResult(string ClassId, string SectionId, string Year, string ExamTypeId, string TermId, string RollNo)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] = true;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                if(!RollNo.Equals(""))
                    Session[ConstHelper.SEARCH_GRAND_STUDENT_ID] = studentRepo.GetStudentByRollNoAndClassSectionId(RollNo, (int) Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID]).id;
                else
                    Session[ConstHelper.SEARCH_GRAND_STUDENT_ID] = -1;
                if (ExamTypeId != null && ExamTypeId.Length > 0 && !ExamTypeId.Equals("0"))
                    Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID] = int.Parse(ExamTypeId);
                else
                    Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID] = -1;
                if (TermId != null && TermId.Length > 0 && !TermId.Equals("0"))
                    Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID] = int.Parse(TermId);
                else
                    Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID] = -1;
                Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR] = Year;


                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = int.Parse(ExamTypeId);
                Session[ConstHelper.GLOBAL_TERM_ID] = int.Parse(TermId);

            }
            catch (Exception exc)
            { }
            return RedirectToAction("GrandResult", new { id = -59 });
        }

        public static IEnumerable<IEnumerable<T>> ToChunks<T>(IEnumerable<T> enumerable,
                                                      int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList(); // Prevent multiple execution of IEnumerable.
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }

        private List<List<string>> SearchGrandResult(int branchId)
        {
            int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
            int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
            int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
            int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
            string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];
            
            var resultds = new DataSet();
            List<List<string>> result = new List<List<string>>();
            try
            {
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Get_Session_Result]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                        + "@termId = " + grandTermId + ","
                                                                        + "@ExamTypeId = " + grandExamTypeId + ","
                                                                        + "@studentId = " + grandStudentId + ","
                                                                        + "@year = '" + grandYear + "',"
                                                                        + "@BranchId = " + branchId;
                //var cmd = db.Database.Connection.CreateCommand();
                //cmd.CommandType = CommandType.Text;
                //cmd.CommandText = spQuery;

                //db.Database.Connection.Open();
                //var reader = cmd.ExecuteReader();

                //do
                //{
                //    var tb = new DataTable();
                //    tb.Load(reader);
                //    resultds.Tables.Add(tb);
                //} while (reader.IsClosed == false);

                //db.Database.Connection.Close();

                List<Exam> examList = new List<Exam>(); 
                resultds = examRepo.BuildStudentGrandList(spQuery);
                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    
                    int chunkSize = GetTableChunkSize(tb);
                    if (examList == null || examList.Count == 0)
                    {
                        for (int chucnkCount = 0; chucnkCount * chunkSize < tb.Rows.Count; chucnkCount++)
                        {
                            int examTypeId = int.Parse(tb.Rows[chucnkCount * chunkSize]["ExamTypeId"].ToString());
                            examList.AddRange(examRepo.GetExamByExamType(examTypeId, grandClassSectionId));
                        }
                    }
                    DataSet ds = convertTabletoDataset(tb, chunkSize, examList);
                    List<string> tableList = new List<string>();
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        tableList.Add(ConvertDataTableToHTML(ds.Tables[k], grandClassSectionId, grandTermId, grandExamTypeId, grandYear));
                    }
                    result.Add(tableList);
                }
                grandErrorCode = 0;
                ViewData["Operation"] = 1;
            }
            catch (Exception exc)
            {
                grandErrorCode = 420;
            }
            //} while (reader.IsClosed == false && reader.NextResult() != null);
            return result;
        }

        //private DataSet convertStudentTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        //{

        //    DataSet ds = new DataSet();
        //    for (int i = 0; i < dt.Rows.Count / chunkSize; i++)
        //    {
        //        ds.Tables.Add(dt.Copy());
        //        ds.Tables[i].TableName = "StudentResultTable" + i;
        //        ds.Tables[i].Rows.Clear();
        //    }
        //    var startRollNO = dt.Rows[0][4].ToString();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        ds.Tables[i / chunkSize].ImportRow(dt.Rows[i]);
        //    }
        //    return ds;
        //}

        private DataSet convertStudentTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        {

            DataSet ds = new DataSet();
            int tblCount = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tableName = "StudentResultTable" + dt.Rows[i][2];
                if(ds.Tables == null || ds.Tables.Count == 0)
                {
                    ds.Tables.Add(dt.Copy());
                    ds.Tables[tblCount].TableName = tableName;
                    ds.Tables[tblCount].Rows.Clear();
                    tblCount++;
                }
                else if (ds.Tables.Contains(tableName) == false)
                {
                    ds.Tables.Add(dt.Copy());
                    ds.Tables[tblCount].TableName = tableName;
                    ds.Tables[tblCount].Rows.Clear();
                    tblCount++;
                }
            }
            var startRollNO = dt.Rows[0][4].ToString();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tableName = "StudentResultTable" + dt.Rows[i][2];
                ds.Tables[tableName].ImportRow(dt.Rows[i]);
            }
            return ds;
        }

        private DataSet convertTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        {

            dt.Columns.Add("Total");
            dt.Columns.Add("Grade");
            

            DataSet ds = new DataSet();
            for (int i = 0; i < dt.Rows.Count / chunkSize; i++)
            {
                ds.Tables.Add(dt.Copy());
                ds.Tables[i].TableName = "StudentResultTable" + i;
                ds.Tables[i].Rows.Clear();
            }
            var startRollNO = dt.Rows[0][4].ToString();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int studentTotal = 0;
                string startSubject = examList[0].Subject.Name;
                int subjectCount = 0;
                foreach (Exam ex in examList)
                {
                    if (ex.Subject.Name == startSubject && subjectCount > 0)
                        break;
                    int marksTemp = 0;
                    if (!dt.Rows[i][ex.Subject.Name].ToString().Equals(""))
                        marksTemp = int.Parse(dt.Rows[i][ex.Subject.Name].ToString());
                    studentTotal += marksTemp;
                    subjectCount++;
                }
                dt.Rows[i]["Total"] = studentTotal.ToString();
                ds.Tables[i / chunkSize].ImportRow(dt.Rows[i]);
                
            }

            for (int i = 0; i < ds.Tables.Count; i++ )
            {
                DataTable table = ds.Tables[i];
                DataRow totalMarksRow = table.NewRow();
                DataRow passPercentageRow = table.NewRow();
                DataRow totalPassRow = table.NewRow();
                DataRow totalFailROw = table.NewRow();
                DataRow resultPercentageRow = table.NewRow();
                DataRow seperatorRow = table.NewRow();
                

                totalMarksRow["StudentName"] = "Total Marks";
                passPercentageRow["StudentName"] = "Pass Percentage";
                totalPassRow["StudentName"] = "Pass Count";
                totalFailROw["StudentName"] = "Fail Count";
                resultPercentageRow["StudentName"] = "Result Percentage";
                int totalMarks = 0;
                int examId = 0, termId = 0;

                if (table.Columns.Contains("ExamTypeName"))
                {
                    examId = int.Parse(table.Rows[0]["ExamTypeId"].ToString());
                }
                else if (table.Columns.Contains("TermName"))
                {
                    termId = int.Parse(table.Rows[0]["ExamTermId"].ToString());
                }

                int termCount = 0, sessionCount = 0;
                int startSubjectId = examList[0].Subject.Id;
                foreach (Exam ex in examList)
                {
                    int passCount = 0, failCount = 0;
                    if (examId > 0)
                    {
                        if (ex.ExamType.Id == examId)
                        {
                            totalMarksRow[ex.Subject.Name] = ex.ExamType.Percent_Of_Total;
                            passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                            totalMarks += (int)ex.ExamType.Percent_Of_Total;

                            for (int m = 0; m < table.Rows.Count; m++)
                            {
                                string grade = GetGrade(int.Parse(table.Rows[m][ex.Subject.Name].ToString()), (int)ex.ExamType.Percent_Of_Total, (int)ex.PassPercentage);
                                if (grade.Equals("F"))
                                    failCount++;
                                else
                                    passCount++;
                            }
                            totalPassRow[ex.Subject.Name] = passCount;
                            totalFailROw[ex.Subject.Name] = failCount;
                            if ((passCount - failCount) > 0)
                                resultPercentageRow[ex.Subject.Name] = ((passCount) * 100) / (passCount + failCount);
                            else
                                resultPercentageRow[ex.Subject.Name] = 0;
                        }
                    }
                    else if (termId > 0)
                    {
                        if (ex.Subject.Id == startSubjectId && termCount > 0)
                            break;
                        if (ex.ExamType.ExamTerm.Id == termId)
                        {
                            termCount++;
                            totalMarksRow[ex.Subject.Name] = ex.ExamType.ExamTerm.Percentage;
                            passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                            totalMarks += (int)ex.ExamType.ExamTerm.Percentage;

                            for (int m = 0; m < table.Rows.Count; m++)
                            {
                                string grade = GetGrade(int.Parse(table.Rows[m][ex.Subject.Name].ToString()), (int)ex.ExamType.ExamTerm.Percentage, (int)ex.PassPercentage);
                                if (grade.Equals("F"))
                                    failCount++;
                                else
                                    passCount++;
                            }
                            totalPassRow[ex.Subject.Name] = passCount;
                            totalFailROw[ex.Subject.Name] = failCount;
                            if ((passCount - failCount) > 0)
                                resultPercentageRow[ex.Subject.Name] = ((passCount) * 100) / (passCount + failCount);
                            else
                                resultPercentageRow[ex.Subject.Name] = 0;
                        }
                    }
                    else
                    {
                        if (ex.Subject.Id == startSubjectId && sessionCount > 0)
                            break;
                        sessionCount++;
                        totalMarksRow[ex.Subject.Name] = "100";
                        passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                        totalMarks += 100;
                        for (int m = 0; m < table.Rows.Count; m++)
                        {
                            string grade = GetGrade(int.Parse(table.Rows[m][ex.Subject.Name].ToString()), 100, (int)ex.PassPercentage);
                            if (grade.Equals("F"))
                                failCount++;
                            else
                                passCount++;
                        }
                        totalPassRow[ex.Subject.Name] = passCount;
                        totalFailROw[ex.Subject.Name] = failCount;
                        if ((passCount - failCount) > 0)
                            resultPercentageRow[ex.Subject.Name] = ((passCount - failCount) * 100) / (passCount + failCount);
                        else
                            resultPercentageRow[ex.Subject.Name] = 0;
                    }
                }
                totalMarksRow["Total"] = totalMarks;
                table.Rows.Add(seperatorRow);
                table.Rows.Add(totalMarksRow);
                table.Rows.Add(passPercentageRow);
                table.Rows.Add(totalPassRow);
                table.Rows.Add(totalFailROw);
                table.Rows.Add(resultPercentageRow);

                for (int cInt = 0; cInt < table.Rows.Count; cInt++)
                {
                    if (!table.Rows[cInt]["Total"].ToString().Equals(""))
                    {
                        string grade = GetGrade(int.Parse(table.Rows[cInt]["Total"].ToString()), totalMarks, 50);
                        table.Rows[cInt]["Grade"] = grade;
                    }
                    else
                        break;

                }
            }
            return ds;
        }

        private int GetStudentTableChunkSize(DataTable dt)
        {
            int count = 1;
            var startSubject = dt.Rows[0]["SubjectName"].ToString();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["SubjectName"].ToString().Equals(startSubject))
                    break;
                count++;
            }
            return count;
        }

        private int GetTableChunkSize(DataTable dt)
        {
            int count = 1;
            var startRollNO = dt.Rows[0]["RollNo"].ToString();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["RollNo"].ToString().Equals(startRollNO))
                    break;
                count++;
            }
            return count;
        }

        private static string ConvertDataTableToHTML(DataTable dt, int grandClassSectionId, int grandTermId, int grandExamTypeId, string grandYear)
        {
            string html = "<div class="+"x_panel"+"> <div class="+"x_title"+">";

            if (dt.Columns.Contains("ExamTypeName"))
            {
                html += "<h2>Exam Result : " + dt.Rows[0]["ExamTypeName"].ToString() + "</h2> ";   
            }
            else if (dt.Columns.Contains("TermName"))
            {
                html += "<h2>Term Result : " + dt.Rows[0]["TermName"].ToString() + "</h2>";
            }
            else
            {
                html += "<h2>Complete Session Result</h2> ";
            }

            html += "<ul class=" + "\"nav navbar-right panel_toolbox\"" + "><li><a class=" + "collapse-link" + "><i class=" + "\"fa fa-chevron-up\"" + "></i></a></li><li><a class=" + "close-link" + "><i class=" + "\"fa fa-close\"" + "></i></a></li></ul>";
            html += "<div class="+"clearfix"+"></div></div><div class="+"x_content"+">";


            while (!dt.Columns[0].ColumnName.Equals("StudentId"))
            {
                dt.Columns.Remove(dt.Columns[0].ColumnName);
            }

            html += "<div class="+"form-group"+"> <div class="+"table-responsive"+"> <table class=" + "\"table table-striped jambo_table bulk_action\"" + ">";
            //add header row
            html += " <thead> <tr> <th></th>";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string columnName = dt.Columns[i].ColumnName;
                if (columnName.Equals("StudentId"))
                    columnName = "";
                else if (columnName.Equals("RollNo"))
                    columnName = "Roll No";
                else if (columnName.Equals("StudentName"))
                    columnName = "Student Name";
                html += "<th>" + columnName + "</th>";
            }
            html += "</tr> </thead>";
            html += "<tbody style=" + "background-color:white;color:#2A3F54" + ">";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr> <td></td>";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].ColumnName.Equals("StudentId") && !dt.Rows[i]["StudentName"].ToString().Equals("Total Marks") && !dt.Rows[i]["StudentName"].ToString().Equals("") &&
                        !dt.Rows[i]["StudentName"].ToString().Equals("Pass Percentage") && !dt.Rows[i]["StudentName"].ToString().Equals("Pass Count") &&
                        !dt.Rows[i]["StudentName"].ToString().Equals("Fail Count") && !dt.Rows[i]["StudentName"].ToString().Equals("Result Percentage"))
                        html += "<td>" + "<a href=" + "/ExamResult/StudentGrandResult/" + dt.Rows[i][j].ToString() + "?termId="+grandTermId+"&&examTypeId="+grandExamTypeId+"&&classSectionId="+grandClassSectionId+"&&year="+grandYear + " style=" + "color:blue;" + ">View</a>" + "</td>";
                    else
                        html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                }
                html += "</tr>";
            }
            html += "</tbody> </table></div></div></div></div>";
            return html;
        }

        private static string ConvertStudentDataTableToHTML(DataTable dt)
        {
            string html = "<div class=" + "x_panel" + "> <div class=" + "x_title" + ">";

            if (dt.Columns.Contains("ExamTypeName"))
            {
                html += "<h2>Exam Result : " + dt.Rows[0]["ExamTypeName"].ToString() + "</h2> ";
            }
            else if (dt.Columns.Contains("TermName"))
            {
                html += "<h2>Term Result : " + dt.Rows[0]["TermName"].ToString() + "</h2>";
            }
            else
            {
                html += "<h2>Complete Session Result</h2> ";
            }

            html += "<ul class=" + "\"nav navbar-right panel_toolbox\"" + "><li><a class=" + "collapse-link" + "><i class=" + "\"fa fa-chevron-up\"" + "></i></a></li><li><a class=" + "close-link" + "><i class=" + "\"fa fa-close\"" + "></i></a></li></ul>";
            //html += "<div class=" + "clearfix" + "></div></div>";
            html += "<div class=" + "clearfix" + "></div></div><div class=" + "x_content" + " style=" + "display: block;" + ">";


            while (!dt.Columns[0].ColumnName.Equals("SubjectName"))
            {
                dt.Columns.Remove(dt.Columns[0].ColumnName);
            }

            html += "<div class=" + "form-group row" + "> <div class=" + "table-responsive" + "> <table class=" + "\"table table-striped jambo_table bulk_action\"" + ">";
            //add header row
            html += " <thead> <tr> <th></th>";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string columnName = dt.Columns[i].ColumnName;
                if (columnName.Equals("SubjectName"))
                    columnName = "Subject Name";
                else if (columnName.Equals("ObtainedMarks"))
                    columnName = "Obtained Marks";
                else if (columnName.Equals("Total"))
                    columnName = "Total Marks";

                html += "<th>" + columnName + "</th>";
            }
            html += "</tr> </thead>";
            html += "<tbody style=" + "background-color:white;color:#2A3F54" + ">";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr> <td></td>";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                }
                html += "</tr>";
            }
            html += "</tbody> </table></div></div></div></div>";
            return html;
        }

        private ActionResult PrintMarksSheet(int examTypeId, int classSectionId, int courseId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            ExamType examType = examRepo.GetExamTypeById(examTypeId);
            Subject subject = subjRepo.GetSubjectById(courseId);
            ClassSection classSection = classSecRepo.GetClassSectionById(classSectionId);

            SubjectSheet pdf = new SubjectSheet();
            PdfDocument document = pdf.CreatePdf(examTypeId, classSectionId, courseId);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Marks Sheet : " + examType.Name + "_" + classSection.Class.Name + "_" + subject.Name + "_" + DateTime.Now.ToString() + ".pdf" };
            //}  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMarksSheet(int[] examResultIds, int[] studentIds, int[] ObtMarks, int[] CourseIds, int[] ExamTypeIds, int passPercentage, int totalMarks)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            bool temp = isPrint;
            try
            {
                int studentId = studentIds[0];
                int classSectionId = (int)studentRepo.GetStudentById(studentId).ClassSectionId;
                int examTypeId = ExamTypeIds[0];
                int courseId = CourseIds[0];
                if (isPrint)
                {
                    temp = true;
                    isPrint = false;
                    return PrintMarksSheet(examTypeId, classSectionId, courseId);
                }
                Exam exam = examRepo.GetExamByExamType(examTypeId, classSectionId, courseId);
                if (exam == null)
                {
                    exam = new Exam();
                    exam.ClassSectionId = classSectionId;
                    exam.CourseId = CourseIds[0];
                    exam.ExamTypeId = ExamTypeIds[0];
                    exam.TotalMarks = totalMarks;
                    exam.PassPercentage = passPercentage;
                    examRepo.AddExam(exam);
                }
                else
                {
                    exam.TotalMarks = totalMarks;
                    exam.PassPercentage = passPercentage;
                    examRepo.UpdateExam(exam);
                }

                int count = 0;
                foreach (int id in studentIds)
                {
                    int examResultId = examResultIds[count];
                    ExamResult er = examRepo.GetExamResultById(examResultId);
                    if (er == null)
                    {
                        er = new ExamResult();
                        er.ExamId = exam.Id;
                        er.StudentId = id;
                        er.ObtainedMarks = ObtMarks[count];
                        er.CreatedOn = DateTime.Now;
                        examRepo.AddExamResult(er);
                    }
                    else
                    {
                        er.ObtainedMarks = ObtMarks[count];
                        examRepo.UpdateExamResult(er);
                    }
                    count++;
                }
                errorCode = 2;
            }
            catch (Exception exc)
            {
                if (temp)
                {
                    errorCode = 2420;
                }
                else
                {
                    errorCode = 420;
                }
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        private List<ExamResultViewModel> SearchClassSheet()
        {
            List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();
            IList<ExamResult> examResultList = null;
            totalExamMarksList = new List<int>();
            examPassPercentageList = new List<int>();
            courseNameList = new List<string>();

            int classExamTypeId = (int)Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID];
            int examClassSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
            try
            {
                List<Exam> examList = examRepo.GetExamByExamType(classExamTypeId, examClassSectionId);
                if (examList != null && examList.Count > 0)
                {
                    foreach (Exam exam in examList)
                    {
                        examTotalMarks = (int)exam.TotalMarks;
                        examPassPercentage = (int)exam.PassPercentage;
                        totalExamMarksList.Add(examTotalMarks);
                        examPassPercentageList.Add(examPassPercentage);
                        courseNameList.Add(exam.Subject.Name);

                        examResultList = examRepo.GetExamResultByExamId(exam.Id);
                        foreach (ExamResult er in examResultList)
                        {
                            ExamResultViewModel ervm = new ExamResultViewModel();
                            ervm.Name = er.Student.Name;
                            ervm.FatherName = er.Student.FatherName;
                            ervm.Id = (int)er.id;
                            ervm.StudentId = (int)er.StudentId;
                            ervm.CourseId = sheetSubjectId;
                            ervm.ExamTypeId = (int)exam.ExamTypeId;
                            ervm.RollNumber = er.Student.RollNumber;
                            ervm.ObtMarks = er.ObtainedMarks.ToString();
                            examResultViewModelList.Add(ervm);
                        }
                    }
                }
                
            }
            catch
            {
                classErrorCode = 420;
            }
            return examResultViewModelList;
        }

        private IList<ExamResultViewModel> SearchMarksSheet()
        {
            int classSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
            int examTypeId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_EXAM_ID];
            int sheetSubjectId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_SUBJECT_ID];

            List<ExamResultViewModel> examResultViewModelList  = new List<ExamResultViewModel>();
            IList<ExamResult> examResultList = null;
            try
            {
                Exam exam = examRepo.GetExamByExamType(examTypeId, classSectionId, sheetSubjectId);
                if (exam != null)
                {
                    examTotalMarks = (int)exam.TotalMarks;
                    examPassPercentage = (int)exam.PassPercentage;
                    examResultList = examRepo.GetExamResultByExamId(exam.Id);
                    foreach (ExamResult er in examResultList)
                    {
                        ExamResultViewModel ervm = new ExamResultViewModel();
                        ervm.Name = er.Student.Name;
                        ervm.Id = (int)er.id;
                        ervm.StudentId = (int)er.StudentId;
                        ervm.CourseId = sheetSubjectId;
                        ervm.ExamTypeId = examTypeId;
                        ervm.RollNumber = er.Student.RollNumber;
                        ervm.ObtMarks = er.ObtainedMarks.ToString();
                        examResultViewModelList.Add(ervm);
                    }
                }
                else
                {
                    IList<StudentModel> studentList = studentRepo.GetStudentByClassSectionId(classSectionId);
                    foreach (StudentModel st in studentList)
                    {
                        ExamResultViewModel ervm = new ExamResultViewModel();
                        ervm.Name = st.Name;
                        ervm.Id = 0;
                        ervm.CourseId = sheetSubjectId;
                        ervm.ExamTypeId = examTypeId;
                        ervm.StudentId = (int)st.Id;
                        ervm.RollNumber = st.RollNumber;
                        examResultViewModelList.Add(ervm);
                    }
                    //Exam newExam = new Exam();
                    //exam.ExamTypeId = examTypeId;
                    //exam.CourseId = sheetSubjectId;
                    //exam.ClassSectionId = classSectionId;

                }
                errorCode = 0;
            }
            catch
            {
                errorCode = 420;
            }
            return examResultViewModelList;
        }

        [HttpGet]
        public void setPrint()
        {
            isPrint = true;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClassResultPdf(string teacherRemarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            ClassResultPdf pdf = new ClassResultPdf();
            int classExamTypeId = (int)Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID];
            int examClassSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
            PdfDocument document = pdf.CreatePdf(classExamTypeId, examClassSectionId);
            ClassSection clsec = classSecRepo.GetClassSectionById(examClassSectionId);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Class Sheet : " + clsec.Class.Name + "_" + clsec.Section.Name + "_" + DateTime.Now.ToString() + ".pdf" };
            //}  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudentResultPdf(string teacherRemarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            int studentResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
            int studentExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];

            StudentResultPdf pdf = new StudentResultPdf();
            Student st = studentRepo.GetStudentById(studentResultId);
            ExamType er = examRepo.GetExamTypeById(studentExamTypeId);
            PdfDocument document = pdf.CreatePdf(st, er, teacherRemarks, CourseName, TotalMarks, ObtMarks, Grade, false, er.Percent_Of_Total);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");
            
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Student Sheet : " + st.RollNumber + "_" + st.Name + "_" + er.Name + "_" + st.ClassSection.Class.Name + "_" + st.ClassSection.Section.Name + "_" + DateTime.Now.ToString() + ".pdf" };
            //}  
        }

        public ActionResult StudentResult(int id, int examTypeId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Session[ConstHelper.STUDENT_RESULT_STUDENT_ID] = id;
            Session[ConstHelper.STUDENT_RESULT_EXAM_ID] = examTypeId;
            var studentObj = studentRepo.GetStudentById(id);

            List<Exam> examList = examRepo.GetExamByExamTypeId(examTypeId, (int) studentObj.ClassSectionId);
            ViewData["student"] = studentObj;
            ViewData["examType"] = examRepo.GetExamTypeById(examTypeId);

            List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();

            try
            {
                foreach (Exam exam in examList)
                {
                    ExamResult er = examRepo.GetExamResultByExamAndStudentId(exam.Id, id);
                    if (er != null)
                    {
                        ExamResultViewModel ervm = new ExamResultViewModel();
                        ervm.Name = er.Student.Name;
                        ervm.FatherName = er.Student.FatherName;
                        ervm.Id = (int)er.id;
                        ervm.StudentId = (int)er.StudentId;
                        ervm.CourseId = sheetSubjectId;
                        ervm.RollNumber = er.Student.RollNumber;
                        ervm.ObtMarks = er.ObtainedMarks.ToString();
                        ervm.totalMarks = exam.TotalMarks.ToString();
                        ervm.CourseName = subjRepo.GetSubjectById((int)exam.CourseId).Name;
                        ervm.Grade = GetGrade((int)er.ObtainedMarks, (int)exam.TotalMarks, (int)exam.PassPercentage);
                        examResultViewModelList.Add(ervm);
                    }
                }
            }
            catch (Exception)
            { }
            return View(examResultViewModelList);
        }

        public ActionResult CreatePdfOfGrandResult()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
            int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
            int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
            int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
            string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];

            DataSet resultds = new DataSet();
            var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Get_Session_Result]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                        + "@termId = " + grandTermId + ","
                                                                        + "@ExamTypeId = " + grandExamTypeId + ","
                                                                        + "@studentId = " + grandStudentId + ","
                                                                        + "@year = '" + grandYear + "',"
                                                                        + "@BranchId = " + branchId;
            resultds = examRepo.BuildStudentGrandList(spQuery);

            List<Exam> examList = new List<Exam>(); 

            List<DataSet> dsList = new List<DataSet>();
            for (int i = 0; i < resultds.Tables.Count; i++)
            {
                DataTable tb = resultds.Tables[i];

                int chunkSize = GetTableChunkSize(tb);
                if (examList == null || examList.Count == 0)
                {
                    for (int chucnkCount = 0; chucnkCount * chunkSize < tb.Rows.Count; chucnkCount++)
                    {
                        int examTypeId = int.Parse(tb.Rows[chucnkCount * chunkSize]["ExamTypeId"].ToString());
                        examList.AddRange(examRepo.GetExamByExamType(examTypeId, grandClassSectionId));
                    }
                }
                DataSet ds = convertTabletoDataset(tb, chunkSize, examList);
                dsList.Add(ds);
            }

            List<string> subjectList = new List<string>();
            ClassGrandResultPdf pdf = new ClassGrandResultPdf();
            string startSubject = examList[0].Subject.Name;
            int subjCount = 0;
            foreach (Exam ex in examList)
            {
                if (ex.Subject.Name == startSubject && subjCount > 0)
                    break;
                subjCount++;
                subjectList.Add(ex.Subject.Name);
            }
            ClassSection clsec = classSecRepo.GetClassSectionById(grandClassSectionId);
            string className = clsec.Class.Name;
            string sectionName = clsec.Section.Name;

            PdfDocument document = pdf.CreatePdf(className, sectionName, subjectList, dsList);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Grand Result Sheet : " + className + "_" + sectionName + "_" + DateTime.Now.ToString() + ".pdf" };
            //}  
        }

        public ActionResult StudentGrandResult(int id, int termId, int examTypeId, int classSectionId, string year)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = classSectionId;
            Session[ConstHelper.STUDENT_RESULT_STUDENT_ID] = id;
            Session[ConstHelper.STUDENT_RESULT_YEAR] = year;
            Session[ConstHelper.STUDENT_RESULT_TERM_ID] = termId;
            Session[ConstHelper.STUDENT_RESULT_EXAM_ID] = examTypeId;
            ViewData["student"] = studentRepo.GetStudentById(id);
            //ViewData["examType"] = db.ExamTypes.Find(examTypeId);

            var resultds = new DataSet();
            List<string> result = new List<string>();
            try
            {
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Get_Student_Result]
		                                                        @ClassSectionId = " + classSectionId + ","
                                                                        + "@termId = " + termId + ","
                                                                        + "@ExamTypeId = " + examTypeId + ","
                                                                        + "@studentId = " + id + ","
                                                                        + "@year = '" + year + "'";
                resultds = examRepo.BuildStudentGrandList(spQuery);
                List<Exam> examList = null;

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    if (examList == null)
                    {
                        int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
                        examList = examRepo.GetExamByExamType(examTypeIdTemp, classSectionId);
                    }
                    int chunkSIze = GetStudentTableChunkSize(tb);
                    DataSet ds = convertStudentTabletoDataset(tb, chunkSIze, examList);
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        DataTable table = AddGradeAndTotalToTable(ds.Tables[k]);
                        result.Add(ConvertStudentDataTableToHTML(table));
                    }
                }
                ViewData["Error"] = 0;
            }
            catch (Exception exc)
            {
                ViewData["Error"] = 420;
            }
            return View(result);
        }

        public ActionResult CreatePdfStudentTranscriptResult(string teacherRemarks)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            int studentClassSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
            int studentGrandResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
            string studentYear = (string)Session[ConstHelper.STUDENT_RESULT_YEAR];
            int studentTermId = (int)Session[ConstHelper.STUDENT_RESULT_TERM_ID];
            int studentGrandExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];

            Student st = studentRepo.GetStudentById(studentGrandResultId);
            ClassSection clsec = classSecRepo.GetClassSectionById(studentClassSectionId);
            string className = clsec.Class.Name;
            string sectionName = clsec.Section.Name;

            DataSet resultds = new DataSet();
            var spQuery = @"DECLARE	@return_value int
                                                    EXEC	@return_value = [dbo].[sp_Get_Student_TranscriptResult]
		                                                    @ClassSectionId = " + studentClassSectionId + ","
                                                                    + "@termId = " + studentTermId + ","
                                                                    + "@ExamTypeId = " + studentGrandExamTypeId + ","
                                                                    + "@studentId = " + studentGrandResultId + ","
                                                                    + "@year = '" + studentYear + "'";
            resultds = examRepo.BuildStudentGrandList(spQuery);

            //List<Exam> examList = null;

            //List<DataSet> dsList = new List<DataSet>();
            //for (int i = 0; i < resultds.Tables.Count; i++)
            //{
            //    DataTable tb = resultds.Tables[i];
            //    if (examList == null)
            //    {
            //        int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
            //        examList = examRepo.GetExamByExamType(examTypeIdTemp, studentClassSectionId);
            //    }
            //    int chunkSIze = GetStudentTableChunkSize(tb);
            //    DataSet ds = convertStudentTabletoDataset(tb, chunkSIze, examList);
            //    for (int k = 0; k < ds.Tables.Count; k++)
            //    {
            //        DataTable table = AddGradeAndTotalToTable(ds.Tables[k]);
            //    }
            //    dsList.Add(ds);
            //}
            StudentTranscriptResult pdf = new StudentTranscriptResult();
            PdfDocument document = pdf.CreatePdf(st.RollNumber, st.Name, className, sectionName, resultds);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Student Grand Result_" + className + "_" + sectionName + "_" + st.RollNumber + "_" + st.Name + "_" + DateTime.Now.ToString() + ".pdf" };
            //}  

        }

        public ActionResult CreatePdfStudentGrandResult(string teacherRemarks)
        {
            CreatePdfStudentTranscriptResult(teacherRemarks);
            return null;
//            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
//            {
//                return RedirectToAction("Index", "Login");
//            }

//            int studentClassSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
//            int studentGrandResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
//            string studentYear = (string)Session[ConstHelper.STUDENT_RESULT_YEAR];
//            int studentTermId = (int)Session[ConstHelper.STUDENT_RESULT_TERM_ID];
//            int studentGrandExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];

//            Student st = studentRepo.GetStudentById(studentGrandResultId);
//            ClassSection clsec = classSecRepo.GetClassSectionById(studentClassSectionId);
//            string className = clsec.Class.Name;
//            string sectionName = clsec.Section.Name;

//            DataSet resultds = new DataSet();
//            var spQuery = @"DECLARE	@return_value int
//                                                    EXEC	@return_value = [dbo].[sp_Get_Student_Result]
//		                                                    @ClassSectionId = " + studentClassSectionId + ","
//                                                                    + "@termId = " + studentTermId + ","
//                                                                    + "@ExamTypeId = " + studentGrandExamTypeId + ","
//                                                                    + "@studentId = " + studentGrandResultId + ","
//                                                                    + "@year = '" + studentYear + "'";
//            resultds = examRepo.BuildStudentGrandList(spQuery);
            
//            List<Exam> examList = null;

//            List<DataSet> dsList = new List<DataSet>();
//            for (int i = 0; i < resultds.Tables.Count; i++)
//            {
//                DataTable tb = resultds.Tables[i];
//                if (examList == null)
//                {
//                    int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
//                    examList = examRepo.GetExamByExamType(examTypeIdTemp, studentClassSectionId);
//                }
//                int chunkSIze = GetStudentTableChunkSize(tb);
//                DataSet ds = convertStudentTabletoDataset(tb, chunkSIze, examList);
//                for (int k = 0; k < ds.Tables.Count; k++)
//                {
//                    DataTable table = AddGradeAndTotalToTable(ds.Tables[k]);
//                }
//                dsList.Add(ds);
//            }
//            StudentGrandResultPdf pdf = new StudentGrandResultPdf();
//            PdfDocument document = pdf.CreatePdf(st.RollNumber, st.Name, className, sectionName, teacherRemarks, dsList);

//            MemoryStream stream = new MemoryStream();
//            document.Save(stream, false);
//            stream.Seek(0, SeekOrigin.Begin);
//            return File(stream, "application/pdf");

        }

        public ActionResult CreatePdfOfAllStudents()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
            int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
            int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
            int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
            string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];

            var studentList = studentRepo.GetStudentByClassSectionId(grandClassSectionId);
            PdfDocument[] docList = new PdfDocument[studentList.Count];

            ClassSection clsec = classSecRepo.GetClassSectionById(grandClassSectionId);
            string className = clsec.Class.Name;
            string sectionName = clsec.Section.Name;

            int studentCOunt = 0;
            foreach (StudentModel st in studentList)
            {
                DataSet resultds = new DataSet();
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Get_Student_Result]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                        + "@termId = " + grandTermId + ","
                                                                        + "@ExamTypeId = " + grandExamTypeId + ","
                                                                        + "@studentId = " + st.Id + ","
                                                                        + "@year = '" + grandYear + "'";
                List<Exam> examList = null;
                resultds = examRepo.BuildStudentGrandList(spQuery);                
                
                List<DataSet> dsList = new List<DataSet>();
                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    if (tb != null && tb.Rows.Count > 0)
                    {
                        if (examList == null)
                        {
                            int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
                            examList = examRepo.GetExamByExamType(grandExamTypeId, grandClassSectionId);
                        }
                        int chunkSIze = GetStudentTableChunkSize(tb);
                        DataSet ds = convertStudentTabletoDataset(tb, chunkSIze, examList);
                        for (int k = 0; k < ds.Tables.Count; k++)
                        {
                            DataTable table = AddGradeAndTotalToTable(ds.Tables[k]);
                        }
                        dsList.Add(ds);
                    }
                }
                if (dsList.Count > 0)
                {
                    StudentGrandResultPdf pdf = new StudentGrandResultPdf();
                    docList[studentCOunt] = pdf.CreatePdf(st.RollNumber, st.Name, className, sectionName, "", dsList);
                    studentCOunt++;
                }
            }

            using (var compressedFileStream = new MemoryStream())
            {
                //Create an archive and store the stream in memory.
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                {
                    for (int i = 0; i < studentCOunt; i++)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            docList[i].Save(stream, false);
                            //stream.Position = 0;
                            stream.Seek(0, SeekOrigin.Begin);

                            var zipEntry = zipArchive.CreateEntry(studentList[i].RollNumber+"-"+studentList[i].Name+".pdf");
                            
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                stream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }

                return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "StudentGrandResult_"+className+"_"+ sectionName+ DateTime.Now.ToString() + ".zip" };
            }
        }

        private DataTable AddGradeAndTotalToTable(DataTable dt)
        {
            dt.Columns.Add("Total");
            dt.Columns.Add("Grade");

            if(dt.Columns.Contains("ExamTypeName"))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    string grade = GetGrade(int.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["ExamPercentage"].ToString()), 50);
                    dt.Rows[i]["Grade"] = grade;
                    dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["ExamPercentage"].ToString());
                }
            }
            else if (dt.Columns.Contains("TermName"))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    string grade = GetGrade(int.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["TermPercentage"].ToString()), 50);
                    dt.Rows[i]["Grade"] = grade;
                    dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["TermPercentage"].ToString());
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    string grade = GetGrade(int.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), 100, 50);
                    dt.Rows[i]["Grade"] = grade;
                    dt.Rows[i]["Total"] = 100;
                }
            }
            return dt;
        }
        private string GetGrade(int obtained, int totalMarks, int passPercentage)
        {
            int obtPercentage = (obtained * 100) / totalMarks;
            string grade = null;
            if (obtPercentage >= 95)
                grade = "A+";
            else if (obtPercentage >= 90)
                grade = "A";
            else if (obtPercentage >= 85)
                grade = "A-";
            else if (obtPercentage >= 80)
                grade = "B+";
            else if (obtPercentage >= 75)
                grade = "B";
            else if (obtPercentage >= 70)
                grade = "B-";
            else if (obtPercentage >= 65)
                grade = "C+";
            else
                grade = "C";

            return grade;
        }
        //private string GetGrade(int obtained, int totalMarks, int passPercentage)
        //{
        //    int obtPercentage = (obtained * 100) / totalMarks;
        //    string grade = null;
        //    if (obtPercentage >= 85)
        //        grade = "A+";
        //    else if (obtPercentage >= 80)
        //        grade = "A";
        //    else if (obtPercentage >= 75)
        //        grade = "B+";
        //    else if (obtPercentage >= 70)
        //        grade = "B";
        //    else if (obtPercentage >= 65)
        //        grade = "C+";
        //    else if (obtPercentage >= 60)
        //        grade = "C";
        //    else if (obtPercentage < passPercentage)
        //        grade = "F";
        //    else
        //        grade = "D";

        //    return grade;
        //}

        [HttpGet]
        public void setExamDetail(int totalMarks, int passPerecentage)
        {
            examTotalMarks = totalMarks;
            examPassPercentage = passPerecentage;
        }

    }
}