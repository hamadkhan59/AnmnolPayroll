using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SMSApi.Controllers
{
    public class PdfFilesController : Controller
    {
        // GET: DownloadFile
        IExamRepository examRepo;
        public PdfFilesController()
        {
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
        }

        [HttpGet]
        public string DownLoadStudentPaper(int id)
        {
            var FileById = examRepo.GetExamPaperModelById(id);
            string fileName = FileById.ExamTypeName + "_" + FileById.ClassName + "_" + FileById.SectionNaem + "_" + FileById.SubjectName;
            fileName = fileName + ".pdf";

            Stream stream = new MemoryStream(FileById.UploadedFile);

            stream.Seek(0, SeekOrigin.Begin);
            System.Web.Mvc.FileStreamResult fs = File(stream, "application/pdf");
            fs.FileStream.Seek(0, SeekOrigin.Begin);
            FileStream str = System.IO.File.Create(Path.Combine(Server.MapPath("~/PdfFiles"), fileName));
            fs.FileStream.CopyTo(str);
            str.Close();
            stream.Close();

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";
            baseUrl += "PdfFiles//" + fileName;

            return baseUrl;

        }


    }
}