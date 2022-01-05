using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class ExcelHelper
    {
        static Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        static Microsoft.Office.Interop.Excel.Workbook xlWorkbook = null;
        static Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = null;
        static Microsoft.Office.Interop.Excel.Range xlRange = null;
        static int rowCount = 0;
        static int colCount = 0;
        public static void InitializeExcelHelper(string FilePath)
        {
            xlWorkbook = xlApp.Workbooks.Open(FilePath);
            xlWorksheet = xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;
            rowCount = xlRange.Rows.Count;
            colCount = xlRange.Columns.Count;
        }

        public static int RowCount()
        {
            return rowCount;
        }

        public static int ColCount()
        {
            return colCount;
        }

        public static string GetCellValue(int rowIndex, int colIndex)
        {
            string value = "";
            if (xlRange.Cells[rowIndex, colIndex].Value2 != null)
                value = xlRange.Cells[rowIndex, colIndex].Value2.ToString();
            return value;
        }

        public static string GetCellValue(int rowIndex, string colName)
        {
            return xlRange.Cells[rowIndex, colName].Value2.ToString();
        }

        public static void CloseExcelSheets()
        {
            //xlRange.Clear();
            //xlWorksheet.cl
            xlWorkbook.Close();
        }
    }
}
