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
    public class TermChaptersController : Controller
    {
        private IClassSubjectRepository classSubjRepo;
        private IClassSectionRepository classSecRepo;
        private ISubjectRepository subjectRepo;
        private static int errorCode = -1;

        public TermChaptersController()
        {
            classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());
            subjectRepo = new SubjectRepositoryImp(new SC_WEBEntities2()); ;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        }

        #region Chapters
        //
        // GET: /TermChapters/
        public ActionResult Index(int yearId = 0, int termId = 0, int registerCourseId = 0, int classId = 0, int sectionId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ST_BEH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            setFilters(ref yearId, ref termId, ref registerCourseId, ref classId, ref sectionId);
            var termChapters = classSecRepo.GetTermChapters(UserPermissionController.GetLoginBranchId(Session.SessionID), yearId, termId, 0, registerCourseId, classId, sectionId);
            ViewData["TermChapters"] = termChapters;
            ViewData["Error"] = errorCode;
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "CreateTermChapters")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTermChapters(string Year, string TermId, string ClassId, string SectionId, string ClassSubjectId, string ChapterId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_EXAM_AWARD_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(SectionId);
            int classSubjectId = int.Parse(ClassSubjectId);
            int yearId = int.Parse(Year);
            int termId = int.Parse(TermId);
            int chapterId = int.Parse(ChapterId);
            if (classSecRepo.GetTermChapters(UserPermissionController.GetLoginBranchId(Session.SessionID), 0, termId, chapterId).Any())
            {
                errorCode = 403;
            }
            else
            {
                var obj = new TermChapter();
                obj.TermId = termId;
                obj.ChapterId = chapterId;
                try
                {
                    errorCode = classSecRepo.SaveTermChapters(new List<TermChapter> { obj });
                }
                catch (Exception ex)
                {
                    errorCode = 402;
                }
            }
            return RedirectToAction("Index", new { yearId = yearId, termId = termId, registerCourseId = classSubjectId, classId = classId, sectionId = sectionId });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "SearchTermChapters")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchTermChapters(string Year, string TermId, string ClassId, string SectionId, string ClassSubjectId, string ChapterId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_EXAM_AWARD_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            int classId = int.Parse(ClassId);
            int sectionId = int.Parse(SectionId);
            int classSubjectId = int.Parse(ClassSubjectId);
            int yearId = int.Parse(Year);
            int termId = int.Parse(TermId);
            int chapterId = int.Parse(ChapterId);
            return RedirectToAction("Index", new { yearId = yearId, termId = termId, registerCourseId = classSubjectId, classId = classId, sectionId = sectionId });
        }

        public ActionResult Delete(int id, int yearId = 0, int termId = 0, int registerCourseId = 0, int classId = 0, int sectionId = 0)
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
                errorCode = classSecRepo.DeleteTermChapter(id);
            }
            catch (Exception ex)
            {
                errorCode = 402;
            }
            return RedirectToAction("Index", new { yearId = yearId, termId = termId, registerCourseId = registerCourseId, classId = classId, sectionId = sectionId });
        }
        #endregion

        #region private methods
        private void setFilters(ref int yearId,ref int termId,ref int registerCourseId,ref int classId,ref int sectionId)
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
            ViewData["classSubject"] = allClassCourses;

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            var allChapters = classSecRepo.GetAllChapters(branchId);
            var classChapters = allChapters.Where(n => n.RegisterCourseId == localRegisterCourseId);
            ViewData["allChapters"] = allChapters;
            ViewBag.ChapterId = new SelectList(classChapters, "Id", "Name");

            var terms = SessionHelper.ExamTermList(Session.SessionID);
            if (termId == 0 && terms.Any())
            {
                termId = terms.FirstOrDefault().Id;
            }
            ViewBag.TermId = new SelectList(terms, "Id", "TermName", termId);
            ViewData["examTerm"] = terms;

            var years = SessionHelper.YearList;
            if(yearId == 0 && years.Any())
            {
                yearId = years.FirstOrDefault().Id;
            }
            ViewBag.YearId = new SelectList(years, "Id", "Year1", yearId);
        }

        #endregion
    }
}
