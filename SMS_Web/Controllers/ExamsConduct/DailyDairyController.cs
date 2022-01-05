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
    public class DailyDairyController : Controller
    {
        private static int errorCode = 0;

        
        IExamRepository examRepo;
        IClassSectionRepository classSecRepo;
        //
        // GET: /ExamTerm/

        public DailyDairyController()
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
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);

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

        public ActionResult SearchDailyDairy(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_SEARCH_PAPERS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<DailyDairyModel> paperList = new List<DailyDairyModel>();
            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);

                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;

                if (Session["DailyDairyList"] != null)
                {
                    paperList = (List<DailyDairyModel>)Session["DailyDairyList"];
                }
                Session["DailyDairyList"] = null;
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
        public ActionResult UploadDailyDairy(HttpPostedFileBase File, DateTime FileDate, int ClassId = 0, int SectionId = 0) 
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
                    //if (FileExt == ConstHelper.FORMATE_PDF || FileExt == ConstHelper.FORMATE_WORD)
                    if (SectionId == 0)
                    {
                        AddWholeClassDairy(File, FileDate, ClassId);
                    }
                    else
                    {

                        string FileExt = Path.GetExtension(File.FileName).ToLower();
                        if (FileExt == ConstHelper.FORMATE_PNG || FileExt == ConstHelper.FORMATE_JPG || FileExt == ConstHelper.FORMATE_JPEG)
                        {
                            Stream str = File.InputStream;
                            BinaryReader Br = new BinaryReader(str);
                            Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

                            var tempPaper = examRepo.SearchDailyDairy(ClassId, SectionId, FileDate);
                            DailyDairy paper = new DailyDairy();
                            paper.ClassSectionId = classSecRepo.GetClassSectionByClassAndSectionId(ClassId, SectionId).ClassSectionId;
                            paper.CreatedOn = FileDate;
                            paper.UplodedFile = FileDet;
                            if (tempPaper == null || tempPaper.Count == 0)
                            {
                                examRepo.AddDailyDairy(paper);
                            }
                            else
                            {
                                paper.CreatedOn = tempPaper[0].CreatedOn;
                                paper.Id = tempPaper[0].Id;
                                examRepo.UpdateDailyDairy(paper);
                            }
                            errorCode = 2;
                        }
                        else
                        {
                            errorCode = 200;
                        }
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

        private void AddWholeClassDairy(HttpPostedFileBase File, DateTime FileDate, int ClassId = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string FileExt = Path.GetExtension(File.FileName).ToLower();
                if (FileExt == ConstHelper.FORMATE_PNG || FileExt == ConstHelper.FORMATE_JPG || FileExt == ConstHelper.FORMATE_JPEG)
                {
                    Stream str = File.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                    var sectionList = classSecRepo.GetClassSectionsByClassId(ClassId);
                    foreach (var sec in sectionList)
                    {
                        int SectionId = (int)sec.SectionId;
                        var tempPaper = examRepo.SearchDailyDairy(ClassId, SectionId, FileDate);
                        DailyDairy paper = new DailyDairy();
                        paper.ClassSectionId = classSecRepo.GetClassSectionByClassAndSectionId(ClassId, SectionId).ClassSectionId;
                        paper.CreatedOn = FileDate;
                        paper.UplodedFile = FileDet;
                        if (tempPaper == null || tempPaper.Count == 0)
                        {
                            examRepo.AddDailyDairy(paper);
                        }
                        else
                        {
                            paper.CreatedOn = tempPaper[0].CreatedOn;
                            paper.Id = tempPaper[0].Id;
                            examRepo.UpdateDailyDairy(paper);
                        }
                    }
                    errorCode = 2;
                }
                else
                {
                    errorCode = 200;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDailyDairy(DateTime FromDate, DateTime ToDate, int ClassId = 0, int SectionId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var paperList = examRepo.SearchDailyDairy(ClassId, SectionId, FromDate, ToDate);
                Session["DailyDairyList"] = paperList;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SearchDailyDairy", new { id = -59 });

        }

        [HttpGet]
        public FileStreamResult DownLoadFile(int id)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var FileById = examRepo.GetDailyDairyModelById(id);
                string fileName = FileById.ClassName + "_" + FileById.SectionNaem + "_" + FileById.CreatedOn;

                Stream stream = new MemoryStream(FileById.UploadedFile);

                var contentLength = stream.Length;
                Response.AppendHeader("Content-Length", contentLength.ToString());
                Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);

                stream.Seek(0, SeekOrigin.Begin);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return File(stream, "image/jpeg");
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