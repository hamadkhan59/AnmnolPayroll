using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace SMSApi.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public async Task sendApiCall()
        {
            try
            {
                string myJson = "{'recipient': 'all','isTopic':'true','title':'ABC','body':'Qwertykdjs','apiKey':'AAAAYL8DFD4:APA91bF7nFGkdmOcd7evdP1HDTTuPCbiwWooOiidn4rum6RTL2MaSxsVA-brZxW7m0G3ijgbaQ1EuvvvVoArrwU4dcko1IPfetNvddOscZSWkRbaCUwpfekoTGA7UT9NG4BlMbMdx9ZQ','application':'org.my.app','customData':[{'param': 'message', 'value': 'qwertyy'}]}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://cordova-plugin-fcm.appspot.com");
                    var content = new StringContent(myJson, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("/push/freesend", content);
                    string resultContent = await result.Content.ReadAsStringAsync();   
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
