using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

namespace SMS_Web.Helpers
{
    public static class SmsService
    {
        public static string sendSmsService(int branchId, string contact, string sendingMsg)
        {
            string result = "";
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, -1);
                SmsVender smsVend = SessionHelper.SmsVenderList.Where(x => x.BranchId == branchId).FirstOrDefault();
                if (smsVend.Name == "Outreach")
                {
                    string url = "http://www.outreach.pk/api/sendsms.php/sendsms/url";
                    String strPost = "id=" + smsVend.Username + "&pass=" + smsVend.password + "&msg=" + sendingMsg + "&to=" + contact + "&mask=" + smsVend.mask + "&type=xml&lang=English";
                    result = sendSmsUsingPostData(url, strPost);
                }
                else if (smsVend.Name == "SmsBiz")
                {
                    String strPost = "http://api.bizsms.pk/api-send-branded-sms.aspx?username=" + smsVend.Username + "&pass=" + smsVend.password + "&text=" + sendingMsg + "&masking=" + smsVend.mask + "&destinationnum=" + contact + "&language=English";
                    result = sendSmsUsingLink(strPost);
                }
                Thread.Sleep(1500);
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return result;
        }

        public static string sendSmsUsingLink(string strPost)
        {
            int branchMode = int.Parse(ConfigurationManager.AppSettings["branchMode"]);
            String result = "";
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, -1);
                if (branchMode == 1)
                {
                    result = "Success";
                }
                else if (branchMode == 2)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strPost);
                    request.AllowAutoRedirect = true;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    StreamReader objSR;
                    objSR = new StreamReader(resStream, System.Text.Encoding.GetEncoding("utf-8"));
                    result = objSR.ReadToEnd();
                }
                else
                {
                    result = "Failure!";
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }

            return result;
        }

        public static string sendSmsUsingPostData(string url, string strPost)
        {

            int branchMode = int.Parse(ConfigurationManager.AppSettings["branchMode"]);
            String result = "";
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, -1);
                if (branchMode == 1)
                {
                    result = "Success";
                }
                else if (branchMode == 2)
                {
                    StreamWriter myWriter = null;
                    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
                    objRequest.Method = "POST";
                    objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
                    objRequest.ContentType = "application/x-www-form-urlencoded";
                    try
                    {
                        myWriter = new StreamWriter(objRequest.GetRequestStream());
                        myWriter.Write(strPost);
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                    finally
                    {
                        myWriter.Close();
                    }
                    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                    using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                        sr.Close();
                    }
                }
                else
                {
                    result = "Failure!";
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return result;
        }



    }
}