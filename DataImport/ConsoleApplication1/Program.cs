using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //ExcelDataImport.InitializeExcelImport(ConfigurationManager.AppSettings["class-filePath"]);
            //ExcelDataImport.ImportClassData();

            //ExcelDataImport.InitializeExcelImport(ConfigurationManager.AppSettings["section-filePath"]);
            //ExcelDataImport.ImportSectionData();

            //ExcelDataImport.InitializeExcelImport(ConfigurationManager.AppSettings["classSection-filePath"]);
            //ExcelDataImport.ImportClassSectionData();

            //ExcelDataImport.InitializeExcelImport(ConfigurationManager.AppSettings["student-filePath"]);
            //ExcelDataImport.ImportStudentData();

            StaffExcelDataImport.InitializeExcelImport(ConfigurationManager.AppSettings["staff-filePath"]);
            StaffExcelDataImport.ImportStaffData();

            //ExcelDataImport.UpdateStudentCurrentAddress();
            //ExcelDataImport.AddFinanceeadsAccounts();
        }

    }
}
