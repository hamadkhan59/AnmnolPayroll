using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class VideoController : Controller
    {
        private ISecurityRepository secRepo;

        public VideoController()
        {

            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());; 
        }

        //
        // GET: /Video/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            YoutubeVideo video = new YoutubeVideo();

            try
            {
                video = secRepo.GetVideoUrl(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(video);
        }

    }
}
