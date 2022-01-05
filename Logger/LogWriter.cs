using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class LogWriter
    {
        //private static Object thisLock = new Object();
        static string logfileLocation = ConfigurationManager.AppSettings["logfilelocation"];
        static string StartProcessMessage = "ps: Process started <{0}, {1}> UserId : {2}";
        static string EndProcessessage = "pc: Process completed successfully <{0}, {1}>";
        static string ErrorProcessessage = "ec: Error in completing the process <{0}, {1}>";
        //static int filesize = int.Parse(ConfigurationManager.AppSettings["fileSize"]);
        //static long byteCount = 1000000;
        //static long byteCount = 10;
        public static void WriteLog(string logMessage)
        {
            //CheckFileSize();
            LogWrite(logMessage);
        }

        public static void WriteErrorLog(string logMessage)
        {
            //CheckFileSize();
            LogWriteError(logMessage);
        }

        public static void WriteExceptionLog(Exception exc)
        {
            LogWriteError("@@Exception StackTrace : " + exc.StackTrace);
            LogWriteError("@@Exception Message : " + exc.Message);
            LogWriteError("@@Exception InnerException : " + exc.InnerException);
        }

        public static void WriteProcStartLog(string className, string methodName, int userId = 0)
        {
            className = GetClassName(className);
            string logMessage = string.Format(StartProcessMessage, className, methodName, userId);
            LogWrite(logMessage);
        }

        public static void WriteProcEndLog(string className, string methodName)
        {
            className = GetClassName(className);
            string logMessage = string.Format(EndProcessessage, className, methodName);
            LogWrite(logMessage);
        }
        
        public static void WriteProcErrorLog(string className, string methodName)
        {
            className = GetClassName(className);
            string logMessage = string.Format(ErrorProcessessage, className, methodName);
            LogWrite(logMessage);
        }

        static string GetClassName(string className)
        {
            string[] array = className.Split('.');
            if (array.Length > 1)
            {
                className = array[array.Length - 1];
            }
            return className;
        }

        static void LogWrite(string logMessage)
        {
            //lock(thisLock)
            //{
                string fileName = GetFileName();
                using (StreamWriter w = File.AppendText(fileName))
                {
                    Log(logMessage, w);
                }
            //}
        }

        static void LogWriteError(string logMessage)
        {
            //lock (thisLock)
            //{
                string fileName = GetFileName();
                using (StreamWriter w = File.AppendText(fileName))
                {
                    LogError(logMessage, w);
                }
            //}
        }


        //static void CheckFileSize()
        //{
        //    int fileCount = int.Parse(ConfigurationManager.AppSettings["fileCount"]);
        //    string path = logfileLocation + fileCount.ToString() + ".txt";
        //    using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
        //    {
        //        long size = file.Length;
        //        if (size >= byteCount * filesize)
        //        {
        //            fileCount++;
        //            ConfigurationManager.AppSettings.Set("fileCount", fileCount.ToString());
        //            CheckFileSize();
        //        }
        //    }

        //    //FileInfo fi = new FileInfo(logfileLocation + fileCount.ToString() + ".txt");
        //    //if (fi.Exists == false)
        //    //{
        //    //    fi.Create();
        //    //    fi = new FileInfo(logfileLocation + fileCount.ToString() + ".txt");
        //    //}

        //    //long size = fi.Length;
        //    //if (size >= byteCount * filesize)
        //    //{
        //    //    fileCount++;
        //    //    ConfigurationManager.AppSettings.Remove("fileCount");
        //    //    ConfigurationManager.AppSettings.Add("fileCount", fileCount.ToString());
        //    //}

        //}
        static void Log(string logMessage, TextWriter txtWriter)
        {
            txtWriter.Write("\r\nOnical Logs | " + DateTime.Now.ToString() + " | SCPortal | info | " + logMessage);
            txtWriter.Close();
        }

        static void LogError(string logMessage, TextWriter txtWriter)
        {
            txtWriter.Write("\r\nOnical Logs | " + DateTime.Now.ToString() + " | SCPortal | Error | " + logMessage);
            txtWriter.Close();
        }

        static string GetFileName()
        {
            string logFile = logfileLocation + DateTime.Now.ToString("ddMMyy") + ".txt";
            //if (!System.IO.File.Exists(logFile))
            //{
            //    System.IO.File.Create(logFile);
            //}

            return logFile;
        }
    }
}
