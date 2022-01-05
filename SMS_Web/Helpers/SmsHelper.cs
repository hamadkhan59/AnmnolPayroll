using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace SMS_Web.Helpers
{
    public class SmsHelper
    {

        private static IStudentRepository secRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        public static void SendSMSToStudent(int studentId, string message)
        {
            var student = secRepo.GetStudentById(studentId);

            if (student != null)
            {
                if (string.IsNullOrEmpty(student.Contact_1) == false)
                {
                    string username = "alnusrat";
                    string password = "alnusrat@786";
                    string sender = "Al-Nusrat";
                    string toNumber = student.Contact_1;
                    string portable = "";
                    string unicode = "1";

                    string detail = "Roll No : " + student.RollNumber + ", " + "Name : " + student.Name;
                    message.Replace("{detail}", detail);
                    message.Replace("{parentdetail}", student.FatherName);     

                    string api = "http://www.brandedsmsportal.com//API/?action=compose&username=" + username
                                    + "&password=" + password
                                    + "&sender=" + sender
                                    + "&to=" + ConvertUniversalNumber(toNumber)
                                    + "&message=" + message
                                    + "&portable=" + portable
                                    + "&unicode=" + unicode;

                    HttpWebRequest request = WebRequest.Create(api) as HttpWebRequest;
                    //optional
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    Stream stream = response.GetResponseStream();

                    StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

                    string responseText = GetResponseText(readStream.ReadToEnd());

                }
            }
        }


        public static int SendHajanaSMSToStudent(int studentId, string message)
        {
            var student = secRepo.GetStudentById(studentId);

            if (student != null)
            {
                if (string.IsNullOrEmpty(student.Contact_1) == false)
                {
                    string sender = "Al-Nusrat";
                    string toNumber = student.Contact_1;

                    string detail = "Roll No : " + student.RollNumber + ", " + "Name : " + student.Name;
                    message.Replace("{detail}", detail);
                    message.Replace("{parentdetail}", student.FatherName);

                    string api = "http://www.hajanaone.com/api/sendsms.php?apikey=ITNK86U3eUJC4V94X2UX"
                                    + "&phone=" + ConvertUniversalNumber(toNumber)
                                    + "&sender=" + sender
                                    + "&message=" + message;

                    HttpWebRequest request = WebRequest.Create(api) as HttpWebRequest;
                    //optional
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    Stream stream = response.GetResponseStream();

                    StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

                    string responseText = GetHajanaResponseText(readStream.ReadToEnd());

                    string returnedResponse = "Message Sent Successfully";

                    if (responseText.Equals(returnedResponse))
                        return 100;
                    else
                        return 300;
                }
            }

            return 0;
        }

        public static int SendHajanaGroupSMSToStudent1(int[] studentId, string message)
        {
            int errorCode = 0;
            foreach (var id in studentId)
            {
                errorCode = SendHajanaSMSToStudent(id, message);
                if(errorCode != 100)
                    break;
                Thread.Sleep(4000);
            }
            return errorCode;
        }

        public static int SendHajanaGroupSMSToStudent(int [] studentId, string message)
        {

            string toNumber = "";

            foreach (var id in studentId)
            {
                var student = secRepo.GetStudentById(id);
                if (string.IsNullOrEmpty(student.Contact_1) == false)
                {
                    if (toNumber.Length == 0)
                        toNumber = ConvertUniversalNumber(student.Contact_1);
                    else
                        toNumber += "," + ConvertUniversalNumber(student.Contact_1);
                }
            }


            if (string.IsNullOrEmpty(toNumber) == false)
            {
                string sender = "Al-Nusrat";

                string api = "http://www.hajanaone.com/api/sendsms.php?apikey=ITNK86U3eUJC4V94X2UX"
                                + "&phone=" + toNumber
                                + "&sender=" + sender
                                + "&message=" + message;

                HttpWebRequest request = WebRequest.Create(api) as HttpWebRequest;
                //optional
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream stream = response.GetResponseStream();

                StreamReader readStream = new StreamReader(stream, Encoding.UTF8);

                string responseText = GetHajanaResponseText(readStream.ReadToEnd());
                string returnedResponse = "Message Sent Successfully";

                if (responseText.Equals(returnedResponse))
                    return 100;
                else
                    return 300;
            }
            else
                return 30;
        }

        private static string ConvertUniversalNumber(string mobileNo)
        {
            string convertedMobileNo = "92";
            if (mobileNo.StartsWith("0"))
            {
                convertedMobileNo = convertedMobileNo + mobileNo.Substring(1, mobileNo.Length - 1);
            }
            else
                convertedMobileNo = mobileNo;

            return convertedMobileNo;
        }

        private static string GetResponseText(string response)
        {

            string returnedResponse = "Message Sent Successfully";
            if (!response.Contains(returnedResponse))
            {
                returnedResponse = response;
            }

            return returnedResponse;
        }

        private static string GetHajanaResponseText(string response)
        {

            string returnedResponse = "Message Sent Successfully";
            if (!response.Contains("successfully"))
            {
                returnedResponse = response;
            }

            return returnedResponse;
        }

    }
}