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
using System.IO;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class ExamPaperController : Controller
    {
        private static int errorCode = 0;

        
        IExamRepository examRepo;
        IClassSectionRepository classSecRepo;
        //
        // GET: /ExamTerm/

        public ExamPaperController()
        {
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());;
        }
        
        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_UPLOAD_PAPERS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["branchId"] = branchId;

                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        public ActionResult SearchPapers(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_SEARCH_PAPERS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<ExamPaperModel> paperList = new List<ExamPaperModel>();
            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["branchId"] = branchId;

                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;

                if (Session["ExamPapersList"] != null)
                {
                    paperList = (List<ExamPaperModel>)Session["ExamPapersList"];
                }
                Session["ExamPapersList"] = null;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(paperList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPaper(HttpPostedFileBase File, int ExamTypeId = 0, int ClassId = 0, int SectionId = 0, int SubjectId = 0) 
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (File != null)
                {
                    string FileExt = Path.GetExtension(File.FileName).ToLower();
                    //if (FileExt == ConstHelper.FORMATE_PDF || FileExt == ConstHelper.FORMATE_WORD)
                    if (FileExt == ConstHelper.FORMATE_PDF)
                    {
                        Stream str = File.InputStream;
                        BinaryReader Br = new BinaryReader(str);
                        Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

                        var tempPaper = examRepo.SearchExamPapers(ClassId, SectionId, SubjectId, ExamTypeId);
                        ExamPaper paper = new ExamPaper();
                        paper.ClassSectionId = classSecRepo.GetClassSectionByClassAndSectionId(ClassId, SectionId).ClassSectionId;
                        paper.CourseId = SubjectId;
                        paper.CreatedOn = DateTime.Now;
                        paper.ExamTypeId = ExamTypeId;
                        paper.UplodedFile = FileDet;
                        if (tempPaper == null || tempPaper.Count == 0)
                        {
                            examRepo.AddExamPaper(paper);
                        }
                        else
                        {
                            paper.CreatedOn = tempPaper[0].CreatedOn;
                            paper.Id = tempPaper[0].Id;
                            examRepo.UpdateExamPaper(paper);
                        }
                        errorCode = 2;
                    }
                    else
                    {
                        errorCode = 200;
                    }
                }
                else
                {
                    errorCode = 201;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = -59 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchUploadedPaper(int ExamTypeId = 0, int ClassId = 0, int SectionId = 0, int SubjectId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var paperList = examRepo.SearchExamPapers(ClassId, SectionId, SubjectId, ExamTypeId);
                Session["ExamPapersList"] = paperList;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SearchPapers", new { id = -59 });

        }

        [HttpGet]
        public ActionResult DownLoadFile(int id)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var FileById = examRepo.GetExamPaperModelById(id);
                string fileName = FileById.ExamTypeName + "_" + FileById.ClassName + "_" + FileById.SectionNaem + "_" + FileById.SubjectName;
                fileName = fileName + ".pdf";

                Stream stream = new MemoryStream(FileById.UploadedFile);

                var contentLength = stream.Length;
                Response.AppendHeader("Content-Length", contentLength.ToString());
                Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);

                stream.Seek(0, SeekOrigin.Begin);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }
        
    }
}