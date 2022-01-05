using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class ChapterTopicController : Controller
    {
        private IClassSubjectRepository classSubjRepo;
        private IClassSectionRepository classSecRepo;
        private ISubjectRepository subjectRepo;
        private IStaffRepository staffRepo;
        private static int errorCode = -1;

        public ChapterTopicController()
        {
            classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());
            subjectRepo = new SubjectRepositoryImp(new SC_WEBEntities2());
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Dashboard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_COURSE_COVERAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["allCourses"] = SessionHelper.ClassSubjectList(Session.SessionID);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["allChapters"] = classSecRepo.GetAllChapters(branchId);
                ViewData["allTopics"] = classSecRepo.GetAllTopics(branchId);

                var terms = SessionHelper.ExamTermList(Session.SessionID);
                ViewBag.TermId = new SelectList(terms, "Id", "TermName");
                ViewData["examTerm"] = terms;

                var years = SessionHelper.YearList;
                ViewBag.YearId = new SelectList(years, "Id", "Year1");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View();
        }

        public ActionResult StaffDashboard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_COURSE_COVERAGE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                var terms = SessionHelper.ExamTermList(Session.SessionID);
                ViewBag.TermId = new SelectList(terms, "Id", "TermName");
                ViewData["examTerm"] = terms;

                var years = SessionHelper.YearList;
                ViewBag.YearId = new SelectList(years, "Id", "Year1");

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["allChapters"] = classSecRepo.GetAllChapters(branchId);
                ViewData["allTopics"] = classSecRepo.GetAllTopics(branchId);
                ViewData["allCourses"] = SessionHelper.ClassSubjectList(Session.SessionID);

                ViewBag.TeacherId = new SelectList(SessionHelper.TeacherList(Session.SessionID, "teacher"), "StaffId", "Name");
                ViewData["teacherSubjects"] = staffRepo.GetAllSessionSubjectsModel();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View();
        }

        #region Chapters
        //
        // GET: /ChapterTopic/
        public ActionResult Index(int registerCourseId=0, int classId=0, int sectionId=0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            ChapterTopicViewModel model = new ChapterTopicViewModel();

            try
            {
                var dummyChapter = -1;
                setChaptersFilters(ref registerCourseId, ref classId, ref sectionId, ref dummyChapter);
                model = classSecRepo.GetChapters(registerCourseId);
                ViewData["Error"] = errorCode;
                errorCode = -1;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(model);
        }


        public ActionResult IncreaseChapters(int registerCourseId, int increaseBy, int classId=0, int sectionId=0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 0;
                if (increaseBy < 1)
                {
                    errorCode = 401;
                }
                else
                {
                    errorCode = classSecRepo.AddMoreChapters(registerCourseId, increaseBy);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return RedirectToAction("Index", "ChapterTopic", new { registerCourseId = registerCourseId, classId = classId, sectionId = sectionId });
        }

        [HttpPost]
        [OnAction(ButtonName = "UpdateChapters")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateChapters(ChapterTopicViewModel viewModel)
        {
            errorCode = 0;
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = classSecRepo.CreateOrUpdateChapters(viewModel.SubjectChapters);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return RedirectToAction("Index", "ChapterTopic", new { registerCourseId = viewModel.RegisterCourseId, classId = viewModel.ClassId, sectionId = viewModel.SectionId });
        }

        public ActionResult Delete(int id, int registerCourseId, int classId = 0, int sectionId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var chapter = classSecRepo.GetChapterById(id);
                if (chapter == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    classSecRepo.DeleteSubjectChater(chapter);
                }
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 402;
            }
            return RedirectToAction("Index", new { registerCourseId = registerCourseId, classId = classId, sectionId = sectionId });
        }
        #endregion
        #region Topics
        public ActionResult Topics(int chapterId=0, int registerCourseId=0, int classId = 0, int sectionId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            ChapterTopicViewModel model = new ChapterTopicViewModel();

            try
            {
                setChaptersFilters(ref registerCourseId, ref classId, ref sectionId, ref chapterId);
                model = classSecRepo.GetTopics(chapterId);
                ViewData["Error"] = errorCode;
                errorCode = -1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View(model);
        }


        public ActionResult IncreaseTopics(int chapterId, int registerCourseId, int increaseBy, int classId = 0, int sectionId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 0;
                if (increaseBy < 1)
                {
                    errorCode = 401;
                }
                else
                {
                    errorCode = classSecRepo.AddMoreTopics(chapterId, increaseBy);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return RedirectToAction("Topics", new { chapterId = chapterId, registerCourseId = registerCourseId, classId = classId, sectionId = sectionId });
        }

        [HttpPost]
        [OnAction(ButtonName = "UpdateTopics")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTopics(ChapterTopicViewModel viewModel)
        {
            errorCode = 0;
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = classSecRepo.CreateOrUpdateTopics(viewModel.ChapterTopics);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Topics", new { chapterId = viewModel.ChapterId, registerCourseId = viewModel.RegisterCourseId, classId = viewModel.ClassId, sectionId = viewModel.SectionId });
        }

        public ActionResult DeleteTopic(int id, int chapterId, int registerCourseId, int classId = 0, int sectionId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var Topic = classSecRepo.GetTopicById(id);
                if (Topic == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    classSecRepo.DeleteTopic(Topic);
                }

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 402;
            }
            return RedirectToAction("Topics", new { chapterId= chapterId, registerCourseId = registerCourseId, classId = classId, sectionId = sectionId });
        }

        #endregion

        #region private methods
        private void setChaptersFilters(ref int registerCourseId, ref int classId, ref int sectionId, ref int chapterId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var activeClasses = SessionHelper.ActiveClassListDD(Session.SessionID);
                var sectionList = SessionHelper.SectionList(Session.SessionID);
                var allClassCourses = SessionHelper.ClassSubjectList(Session.SessionID);

                if (classId == 0 && activeClasses.Any())
                {
                    classId = activeClasses.FirstOrDefault().Id;
                }
                if (sectionId == 0 && sectionList.Any())
                {
                    sectionId = sectionList.FirstOrDefault().Id;
                }
                var localClassId = classId;
                var localSectionId = sectionId;

                ViewBag.ClassId = new SelectList(activeClasses, "Id", "Name", classId);
                ViewBag.SectionId = new SelectList(sectionList, "Id", "Name", sectionId);

                var classSectionSubjects = allClassCourses.Where(n => n.ClassId == localClassId
                && n.SectionId == localSectionId).ToList();

                if (registerCourseId == 0 && classSectionSubjects.Any())
                {
                    registerCourseId = classSectionSubjects.FirstOrDefault().RegisterCourseId;
                }
                var localRegisterCourseId = registerCourseId;

                var selectedCourseSubject = allClassCourses.FirstOrDefault(n => n.RegisterCourseId == localRegisterCourseId);
                if (selectedCourseSubject != null)
                {
                    ViewBag.SubjectId = new SelectList(classSectionSubjects, "RegisterCourseId", "SubjectName", selectedCourseSubject.RegisterCourseId);
                }
                else
                {
                    ViewBag.SubjectId = new SelectList(classSectionSubjects, "RegisterCourseId", "SubjectName");
                }

                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["allCourses"] = allClassCourses;

                if (chapterId > -1)
                {
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    var allChapters = classSecRepo.GetAllChapters(branchId);
                    var classChapters = allChapters.Where(n => n.RegisterCourseId == localRegisterCourseId);
                    if (chapterId == 0 && classChapters.Any())
                    {
                        chapterId = classChapters.FirstOrDefault().Id;
                    }
                    ViewData["allChapters"] = allChapters;
                    ViewBag.ChapterId = new SelectList(classChapters, "Id", "Name", chapterId);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        #endregion
    }
}
