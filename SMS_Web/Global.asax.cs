using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace SMS_Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapRoute(
            //    "Class_Details",                                              // Route name
            //    "{controller}/{id}",                           // URL with parameters
            //    new { controller = "ClassSection", action = "Index", id = "" }  // Parameter defaults
            //);

           routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

           //routes.MapRoute(
           //    "Class_Details",                                              // Route name
           //    "{controller}/{id}",                           // URL with parameters
           //    new { controller = "ClassSection", action = "Index", id = "" }  // Parameter defaults
           //);

            //routes.MapRoute(
            //    "Class_Details",
            //    "Class/{id}",
            //    new { controller = "Class", action = "Index" }
            //    );
        }
        protected void Application_Start()
        {
            string[] err = new string[1];
            err[0] = "in start method;";
            //System.IO.File.WriteAllLines(@"E:\log.txt", err);
            try
            {

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new RazorViewEngine()); 

                AreaRegistration.RegisterAllAreas();

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                err[0] = errorMessage;
                //System.IO.File.WriteAllLines(@"E:\log.txt", err);
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            //Session["init"] = 0;
        }

        //void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    HttpCookie decryptedCookie =
        //       Context.Request.Cookies[FormsAuthentication.FormsCookieName];

        //    FormsAuthenticationTicket ticket =
        //       FormsAuthentication.Decrypt(decryptedCookie.Value);

        //    var identity = new GenericIdentity(ticket.Name);
        //    var principal = new GenericPrincipal(identity, null);

        //    HttpContext.Current.User = principal;
        //    Thread.CurrentPrincipal = HttpContext.Current.User;
        //}
    }
}