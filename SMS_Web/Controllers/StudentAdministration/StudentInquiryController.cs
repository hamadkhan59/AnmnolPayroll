using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Filters;
using System.IO;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class StudentInquiryController : Controller
    {

        
        private IStudentRepository studentRepo;
        private IClassRepository classRepo;
        private IStaffRepository staffRepo;

        static int errorCode = 0;

        public StudentInquiryController()
        {
            
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());;
        }

        //
        // GET: /StudentInquiry/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_STUDENT_INQUIRY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            StudentInquiry studentInquiry = new StudentInquiry();

            try
            {
                ViewData["Error"] = errorCode;
                errorCode = 0;
                if (id > 0)
                    studentInquiry = studentRepo.GetStudentInquiryById(id);

                ViewData["Operation"] = id;
                //ViewData["class"] = SessionHelper.ClassList(Session.SessionID);
                if (Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] = false;
                    ViewData["inquiryList"] = SearchInquiries();
                }

                if (studentInquiry == null)
                {
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    ViewData["inquiryNumber"] = studentRepo.GetInquiryNumber(branchId);
                    ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
                    ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name");
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    ViewBag.SessionId = new SelectList(SessionHelper.SessionListDD(Session.SessionID), "Id", "Name");
                    ViewBag.TakenBy = new SelectList(SessionHelper.StaffListDD(Session.SessionID), "StaffId", "Name");
                    ViewBag.CheckedBy = new SelectList(SessionHelper.StaffListDD(Session.SessionID), "StaffId", "Name");
                    ViewBag.TestStatus = new SelectList(SessionHelper.TestStatusList, "ID", "StatusName");
                }
                else
                {
                    ViewData["inquiryNumber"] = studentInquiry.InquiryNumber;
                    ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1", studentInquiry.GenderCode);
                    ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name", studentInquiry.ReligionCode);
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", studentInquiry.ClassId);
                    ViewBag.SessionId = new SelectList(SessionHelper.SessionListDD(Session.SessionID), "Id", "Name", studentInquiry.SessionId);
                    ViewBag.TakenBy = new SelectList(SessionHelper.StaffListDD(Session.SessionID), "StaffId", "Name", studentInquiry.TakenBy);
                    ViewBag.CheckedBy = new SelectList(SessionHelper.StaffListDD(Session.SessionID), "StaffId", "Name", studentInquiry.CheckedBy);
                    ViewBag.TestStatus = new SelectList(SessionHelper.TestStatusList, "ID", "StatusName", studentInquiry.CheckedBy);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View(studentInquiry);
        }

        public ActionResult Search()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_STUDENT_INQUIRY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                //ViewData["class"] = SessionHelper.ClassList(Session.SessionID);
                if (Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] = false;
                    ViewData["inquiryList"] = SearchInquiries();
                }

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                voidSetSearchVeriables();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View(new StudentInquiry());
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "ID")]StudentInquiry studentinquiry, int SessionId, int TestStatusID, int TakenBy, int CheckedBy, int ClassId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                RemoveExtraFromModelState(ModelState);

                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Student Inquiry Model is valid, creating new Student Inquiry");
                    studentinquiry.StdImage = (byte []) Session[ConstHelper.STUDENT_IMAGE];
                    if (SessionId == 0)
                    {
                        errorCode = 520;
                        return RedirectToAction("Index", new { id = 0 });
                    }
                    studentinquiry.SessionId = SessionId;
                    studentinquiry.TestStatus = TestStatusID;
                    if (studentinquiry.Staff != null)
                    {
                        studentinquiry.TakenBy = TakenBy;
                        studentinquiry.CheckedBy = CheckedBy;
                    }
                    studentinquiry.ClassId = ClassId;
                    studentinquiry.InquiryNumber = studentRepo.GetInquiryNumber(branchId);
                    studentinquiry.Session = null;
                    studentinquiry.Staff = null;
                    studentinquiry.Staff1 = null;
                    studentinquiry.Class = null;
                    studentinquiry.TestStatu = null;

                    studentinquiry.BranchId = branchId;
                    //studentinquiry.Branch = null;

                    int returnStatus = studentRepo.AddStudentInquiry(studentinquiry);
                    if (returnStatus == -1)
                        errorCode = 420;
                    else
                    {
                        errorCode = 2;
                    }
                }
                else
                {
                    //ViewData["class"] = SessionHelper.ClassList(Session.SessionID);
                    LogWriter.WriteLog("Student Inquiry Model is invalid");
                    ViewData["inquiryNumber"] = studentRepo.GetInquiryNumber(branchId);
                    ViewBag.GenderCode = new SelectList(studentRepo.GetAllGenders(), "Id", "Gender1");
                    ViewBag.ReligionCode = new SelectList(studentRepo.GetAllReligion(), "Id", "Name");
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    ViewBag.SessionId = new SelectList(studentRepo.GetAllSessions(), "Id", "Name");
                    ViewBag.TakenBy = new SelectList(staffRepo.GetAllStaff(), "StaffId", "Name");
                    ViewBag.CheckedBy = new SelectList(staffRepo.GetAllStaff(), "StaffId", "Name");
                    ViewBag.TestStatus = new SelectList(studentRepo.GetAllTestStatus(), "ID", "StatusName");
                    return View(studentinquiry);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        [HttpPost]
        public void UploadImage()
        {
            var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage"];
            using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
            {
                Session[ConstHelper.STUDENT_IMAGE] = binaryReader.ReadBytes(httpPostedFile.ContentLength);
            }
        }

        [HttpPost]
        [ActionName("Search")]
        [OnAction(ButtonName = "Search")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceDate(string ClassId, DateTime fromDate, DateTime toDate, string name, string fatherName, string fatherCnic, string inquiryNumber)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                int classId = 0;
                if (!string.IsNullOrEmpty(ClassId))
                    classId = int.Parse(ClassId);
                Session[ConstHelper.STUDENT_INQUIRY_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.STUDENT_INQUIRY_FROM_DATE] = fromDate;
                Session[ConstHelper.STUDENT_INQUIRY_TO_DATE] = toDate;
                Session[ConstHelper.STUDENT_INQUIRY_NAME] = name;
                Session[ConstHelper.STUDENT_INQUIRY_FATHER_NAME] = fatherName;
                Session[ConstHelper.STUDENT_INQUIRY_FATHER_CNIC] = fatherCnic;
                Session[ConstHelper.STUDENT_INQUIRY_INQUIRY_NO] = inquiryNumber;
                Session[ConstHelper.STUDENT_INQUIRY_SEARCH_FLAG] = true;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Search");
        }
        //
        // GET: /StudentInquiry/Edit/5

        private void voidSetSearchVeriables()
        {
            if (Session[ConstHelper.GLOBAL_CLASS_ID] != null)
            {
                ViewData["GlobalClassId"] = (int)Session[ConstHelper.GLOBAL_CLASS_ID];
                Session[ConstHelper.GLOBAL_CLASS_ID] = null;
            }

            if (Session[ConstHelper.STUDENT_INQUIRY_FROM_DATE] != null)
            {
                ViewData["InquiryFromDate"] = (DateTime)Session[ConstHelper.STUDENT_INQUIRY_FROM_DATE];
                Session[ConstHelper.STUDENT_INQUIRY_FROM_DATE] = null;
            }

            if (Session[ConstHelper.STUDENT_INQUIRY_TO_DATE] != null)
            {
                ViewData["InquiryToDate"] = (DateTime)Session[ConstHelper.STUDENT_INQUIRY_TO_DATE];
                Session[ConstHelper.STUDENT_INQUIRY_TO_DATE] = null;
            }
        }

        private List<StudentInquiryModel> SearchInquiries()
        {
            List<StudentInquiryModel> studentInquiryList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classId = (int)Session[ConstHelper.STUDENT_INQUIRY_CLASS_ID];
                DateTime fromDate = (DateTime)Session[ConstHelper.STUDENT_INQUIRY_FROM_DATE];
                DateTime toDate = (DateTime)Session[ConstHelper.STUDENT_INQUIRY_TO_DATE];
                string name = (string)Session[ConstHelper.STUDENT_INQUIRY_NAME];
                string fatherName = (string)Session[ConstHelper.STUDENT_INQUIRY_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.STUDENT_INQUIRY_FATHER_CNIC];
                string inquiryNumber = (string)Session[ConstHelper.STUDENT_INQUIRY_INQUIRY_NO];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (!string.IsNullOrEmpty(inquiryNumber))
                {
                    studentInquiryList = studentRepo.SearchInquiryByInquiryNo(inquiryNumber, branchId);
                }
                else
                {
                    studentInquiryList = studentRepo.SearchStudentInquiry(classId, name, fatherName, fatherCnic, fromDate, toDate, branchId);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return studentInquiryList;
        }

        private void RemoveExtraFromModelState(ModelStateDictionary ModelState)
        {
            ModelState.Remove("Class.Name");
            ModelState.Remove("Session.Name");
            ModelState.Remove("Staff.Name");
            ModelState.Remove("Staff.FatherName");
            ModelState.Remove("Staff.PhoneNumber");
            ModelState.Remove("Staff.CurrentAddress");
            ModelState.Remove("Staff.CNIC");
            ModelState.Remove("Staff.FatherCNIC");
            ModelState.Remove("Staff1.Name");
            ModelState.Remove("Staff1.FatherName");
            ModelState.Remove("Staff1.PhoneNumber");
            ModelState.Remove("Staff1.CurrentAddress");
            ModelState.Remove("Staff1.CNIC");
            ModelState.Remove("Staff1.FatherCNIC");
        }

        //
        // POST: /StudentInquiry/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentInquiry studentinquiry, string InquiryNumber, int SessionId, int TestStatusID, int TakenBy, int CheckedBy, int ClassId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                RemoveExtraFromModelState(ModelState);
                if (ModelState.IsValid)
                {
                    studentinquiry.StdImage = (byte[])Session[ConstHelper.STUDENT_IMAGE];
                    studentinquiry.SessionId = SessionId;
                    studentinquiry.TestStatus = TestStatusID;
                    if (studentinquiry.Staff != null)
                    {
                        studentinquiry.TakenBy = TakenBy;
                        studentinquiry.CheckedBy = CheckedBy;
                    }
                    //studentinquiry.TakenBy = studentinquiry.Staff.StaffId;
                    //studentinquiry.CheckedBy = studentinquiry.Staff1.StaffId;
                    studentinquiry.InquiryNumber = InquiryNumber;
                    studentinquiry.ClassId = ClassId;
                    studentinquiry.Session = null;
                    studentinquiry.Staff = null;
                    studentinquiry.Staff1 = null;
                    studentinquiry.Class = null;
                    studentinquiry.TestStatu = null;

                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    studentinquiry.BranchId = branchId;
                    //studentinquiry.Branch = null;

                    studentRepo.UpdateStudentInquiry(studentinquiry);
                    
                    errorCode = 2;
                }
                else
                {
                    ViewData["inquiryNumber"] = studentinquiry.InquiryNumber;
                    ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1", studentinquiry.GenderCode);
                    ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name", studentinquiry.ReligionCode);
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", studentinquiry.ClassId);
                    ViewBag.SessionId = new SelectList(SessionHelper.SessionList(Session.SessionID), "Id", "Name", studentinquiry.SessionId);
                    ViewBag.TakenBy = new SelectList(SessionHelper.StaffList(Session.SessionID), "StaffId", "Name", studentinquiry.TakenBy);
                    ViewBag.CheckedBy = new SelectList(SessionHelper.StaffList(Session.SessionID), "StaffId", "Name", studentinquiry.CheckedBy);
                    ViewBag.TestStatus = new SelectList(SessionHelper.TestStatusList, "ID", "StatusName", studentinquiry.CheckedBy);
                    return View(studentinquiry);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        //
        // GET: /StudentInquiry/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            StudentInquiry studentinquiry = studentRepo.GetStudentInquiryById(id);
            if (studentinquiry == null)
            {
                return HttpNotFound();
            }
            return View(studentinquiry);
        }

        //
        // POST: /StudentInquiry/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }


            StudentInquiry studentinquiry = studentRepo.GetStudentInquiryById(id);
            studentRepo.DeleteStudentInquiry(studentinquiry);
            return RedirectToAction("Index");
        }

    }
}