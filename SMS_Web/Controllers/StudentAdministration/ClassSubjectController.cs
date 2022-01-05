using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using System.Web.Script.Serialization;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class ClassSubjectController : Controller
    {
        //private cs db = new cs();
        private static int errorCode = 0;
        
        private IClassSubjectRepository classSubjRepo;
        private IClassRepository classRepo;
        private ISectionRepository secRepo;
        private IClassSectionRepository classSecRepo;
        private ISubjectRepository subjectRepo;
        //
        // GET: /ClassSubject/

        public ClassSubjectController()
        {

            classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());;
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            subjectRepo = new SubjectRepositoryImp(new SC_WEBEntities2());;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        }


        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_CLASS_SUBJECTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            RegisterCourse registercourse = new RegisterCourse();

            try
            {
                if (id > 0)
                    errorCode = 0;
                //var registercourses = db.RegisterCourses.Include(r => r.ClassSection).Include(r => r.Subject);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                registercourse = classSubjRepo.GetRegisterCourseById(id);//.Include(r => r.ClassSection).Include(r => r.Subject);
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);
                ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
                ViewData["Operation"] = id;
                Session[ConstHelper.CLASS_SUBJECT_ID] = id;

                if (id == 0)
                {
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                    ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");
                }
                else
                {
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", registercourse.ClassSection.ClassId);
                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name", registercourse.ClassSection.SectionId);
                    ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name", registercourse.Subject.Id);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(registercourse);
        }

      
        [HttpPost]
        public ActionResult GetSectionList(int classId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            //Here I'll bind the list of cities corresponding to selected state's state id  
            List<Section> sections = classSecRepo.GetSectionsByClassId(classId);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(sections);
            return Json(result, JsonRequestBehavior.AllowGet);
        }  

        //
        // POST: /ClassSubject/Create


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterCourse registercourse, int? ResultOrder, int? ClassId, int? SectionId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (ResultOrder != null)
                {
                    var clsecModel = classSecRepo.GetClassSectionsModelById(registercourse.ClassSectionId);
                    if (SectionId == 0 || SectionId == null)
                    {
                        registercourse.ResultOrder = (int)ResultOrder;
                        AddSubjectToClass(registercourse, (int)ClassId);
                        errorCode = 2;
                    }
                    else
                    {
                        var tempObj = classSubjRepo.GetRegisterCourseByClassSectionAndSUbjectId(registercourse.ClassSectionId, registercourse.SubjectId);
                        if (tempObj == null)
                        {
                            ClassSection cls = classSecRepo.GetClassSectionByClassAndSectionId((int)clsecModel.ClassId, (int)clsecModel.SectionId);
                            registercourse.ClassSection = null;
                            registercourse.ClassSectionId = cls.ClassSectionId;
                            registercourse.SubjectId = registercourse.SubjectId;
                            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                            registercourse.BranchId = branchId;
                            registercourse.Branch = null;
                            registercourse.Subject = null;
                            //if (ModelState.IsValid)
                            //{
                            classSubjRepo.AddRegisterCourse(registercourse);
                            errorCode = 2;
                            //}
                            //else
                            //    errorCode = 1;
                        }
                        else
                        {
                            errorCode = 11;
                        }
                    }
                }
                else
                {
                    errorCode = 12;
                }
                SessionHelper.InvalidateClassSubjectCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegisterCourse registercourse, int? ResultOrder)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ClassSection cls = classSecRepo.GetClassSectionByClassAndSectionId((int)registercourse.ClassSection.ClassId, (int)registercourse.ClassSection.SectionId);
                registercourse.ClassSectionId = cls.ClassSectionId;

                var tempObj = classSubjRepo.GetRegisterCourseByClassSectionAndSUbjectId(registercourse.ClassSectionId, registercourse.Subject.Id);
                if (tempObj != null && tempObj.RegisterCourseId == (int)Session[ConstHelper.CLASS_SUBJECT_ID])
                {
                    registercourse.ClassSection = null;
                    registercourse.RegisterCourseId = (int)Session[ConstHelper.CLASS_SUBJECT_ID];
                    registercourse.SubjectId = registercourse.Subject.Id;
                    registercourse.ResultOrder = (int)ResultOrder;
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    registercourse.BranchId = branchId;
                    registercourse.Branch = null;
                    registercourse.Subject = null;
                    if (ModelState.IsValid)
                    {
                        classSubjRepo.UpdateRegisterCourse(registercourse);
                        errorCode = 2;
                        SessionHelper.InvalidateClassSubjectCache = false;
                    }
                    else
                        errorCode = 1;
                }
                else
                {
                    errorCode = 11;
                }
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0});

        }

        public void AddSubjectToClass(RegisterCourse registeCourse, int ClassId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var classSectionList = classSecRepo.GetClassSectionsByClassId(ClassId);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                foreach (var classSection in classSectionList)
                {
                    var getCourse = classSubjRepo.GetRegisterCourseByClassSectionAndSUbjectId(classSection.ClassSectionId, registeCourse.SubjectId);
                    if (getCourse == null)
                    {
                        var tempCourse = registeCourse;
                        tempCourse.ClassSectionId = classSection.ClassSectionId;
                        tempCourse.SubjectId = tempCourse.SubjectId;
                        tempCourse.ClassSection = null;
                        tempCourse.Subject = null;
                        tempCourse.BranchId = branchId;
                        tempCourse.Branch = null;
                        //db.RegisterCourses.Add(tempCourse);
                        //db.SaveChanges();
                        classSubjRepo.AddRegisterCourse(tempCourse);
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
        

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                RegisterCourse registercourse = classSubjRepo.GetRegisterCourseById(id);
                if (registercourse == null)
                {
                    return HttpNotFound();
                }
                int classSubjectCount = classSubjRepo.GetTimeTableCount(registercourse.RegisterCourseId);
                if (classSubjectCount == 0)
                {
                    classSubjRepo.DeleteRegisterCourse(registercourse);
                    errorCode = 4;
                    SessionHelper.InvalidateClassSubjectCache = false;
                }
                else
                    errorCode = 40;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Delete Class SUbject :" + ex.Message);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

       
    }
}