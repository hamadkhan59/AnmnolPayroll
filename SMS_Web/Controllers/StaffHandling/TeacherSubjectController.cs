using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StaffHandling
{
    public class TeacherSubjectController : Controller
    {
        private static int errorCode = 0;
        //
        // GET: /TeacherSubject/

        
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IClassSectionRepository classSecRepo;
        IClassSubjectRepository classSubjRepo;
        ISubjectRepository subjRepo;
        IStaffRepository staffRepo;
        public TeacherSubjectController()
        {
            
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());;
            subjRepo = new SubjectRepositoryImp(new SC_WEBEntities2());;
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_TEACHER_SUBJECTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.TeacherId = new SelectList(SessionHelper.TeacherListDD(Session.SessionID, "teacher"), "StaffId", "Name");
                ViewData["teacherSubjects"] = staffRepo.GetAllSessionSubjectsModel();
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //var sessionsubjects = db.SessionSubjects.Include(s => s.Subject).Include(s => s.Staff);
            return View("");
        }

        //
        // GET: /TeacherSubject/Details/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SessionSubject subject = staffRepo.GetSessionSubjectById(id);
                if (subject != null)
                {
                    staffRepo.DeleteSessionSubject(subject);
                    errorCode = 4;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }


        //
        // GET: /TeacherSubject/Create

        //
        // POST: /TeacherSubject/Create
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]SessionSubject sessionSubject, int ClassId, int SectionId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Model state is valid, creatign the teacher subjects");
                    sessionSubject.ClassSectionId = classSecRepo.GetClassSectionId(ClassId, SectionId);
                    sessionSubject.ClassSection = null;
                    if (checkDates(sessionSubject) == true)
                    {
                        var session = staffRepo.GetCurrentSession();
                        sessionSubject.SessionYear = session.Id.ToString();
                        staffRepo.AddSessionSubject(sessionSubject);
                        errorCode = 2;
                    }
                    else
                    {
                        //errorCode = 11;
                        ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                        ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                        ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                        ViewBag.TeacherId = new SelectList(SessionHelper.TeacherList(Session.SessionID, "teacher"), "StaffId", "Name");
                        ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID); ;
                        ViewBag.SubjectId = new SelectList(subjRepo.GetAllSubjectes(), "Id", "Name");
                        ViewData["Error"] = errorCode;
                        ViewData["teacherSubjects"] = staffRepo.GetAllSessionSubjectsModel();
                        errorCode = 0;
                        return View(sessionSubject);
                    }
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

        private bool checkDates(SessionSubject subject)
        {
            var sessionObj = staffRepo.GetTeacherSessionSubjectInDates(subject);
            if (sessionObj == null)
            {
                sessionObj = staffRepo.GetSessionSubjectInDates(subject);
                if (sessionObj == null)
                    return true;
                else
                    errorCode = 12;
            }
            else
                errorCode = 11;
            return false;    

        }
        
    }
}