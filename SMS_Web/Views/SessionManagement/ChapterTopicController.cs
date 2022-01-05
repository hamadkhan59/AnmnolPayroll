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
            ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassList(Session.SessionID), "Id", "Name");
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

            return View();
        }

        public ActionResult StaffDashboard()
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
            ViewData["teacherSubjects"] = staffRepo.GetAllSessionSubjects();

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
            var dummyChapter = -1;
            setChaptersFilters(ref registerCourseId, ref classId, ref sectionId, ref dummyChapter);
            var model = classSecRepo.GetChapters(registerCourseId);
            ViewData["Error"] = errorCode;
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

            errorCode = 0;
            if (increaseBy < 1)
            {
                errorCode = 401;
            }
            else
            {
                errorCode = classSecRepo.AddMoreChapters(registerCourseId, increaseBy);
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

            errorCode =  classSecRepo.CreateOrUpdateChapters(viewModel.SubjectChapters);
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
                var chapter = classSecRepo.GetChapterById(id);
                if (chapter == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    classSecRepo.DeleteSubjectChater(chapter);
                }
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Delete Class Section:" + ex.Message);
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

            setChaptersFilters(ref registerCourseId,ref classId,ref sectionId,ref chapterId);
            var model = classSecRepo.GetTopics(chapterId);
            ViewData["Error"] = errorCode;
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

            errorCode = 0;
            if (increaseBy < 1)
            {
                errorCode = 401;
            }
            else
            {
                errorCode = classSecRepo.AddMoreTopics(chapterId, increaseBy);
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

            errorCode = classSecRepo.CreateOrUpdateTopics(viewModel.ChapterTopics);
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
                var Topic = classSecRepo.GetTopicById(id);
                if (Topic == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    classSecRepo.DeleteTopic(Topic);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Delete Class Section:" + ex.Message);
                errorCode = 402;
            }
            return RedirectToAction("Topics", new { chapterId= chapterId, registerCourseId = registerCourseId, classId = classId, sectionId = sectionId });
        }

        #endregion

        #region private methods
        private void setChaptersFilters(ref int registerCourseId, ref int classId, ref int sectionId, ref int chapterId)
        {
            var activeClasses = SessionHelper.ActiveClassList(Session.SessionID);
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

            var classSectionSubjects = allClassCourses.Where(n => n.ClassSection.ClassId == localClassId
            && n.ClassSection.SectionId == localSectionId).ToList();

            if (registerCourseId == 0 && classSectionSubjects.Any())
            {
                registerCourseId = classSectionSubjects.FirstOrDefault().RegisterCourseId;
            }
            var localRegisterCourseId = registerCourseId;

            var selectedCourseSubject = allClassCourses.FirstOrDefault(n => n.RegisterCourseId == localRegisterCourseId);
            if (selectedCourseSubject != null)
            {
                ViewBag.SubjectId = new SelectList(classSectionSubjects, "RegisterCourseId", "Subject.Name", selectedCourseSubject.RegisterCourseId);
            }
            else
            {
                ViewBag.SubjectId = new SelectList(classSectionSubjects, "RegisterCourseId", "Subject.Name");
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

            
        }

        #endregion
    }
}
