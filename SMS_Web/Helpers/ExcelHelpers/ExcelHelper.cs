using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Web.Helpers.ExcelHelpers
{
    public class ExcelHelper
    {
        public static Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        public static Microsoft.Office.Interop.Excel.Workbook xlWorkbook = null;
        public static Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = null;
        public static Microsoft.Office.Interop.Excel.Range xlRange = null;
        static int rowCount = 0;
        static int colCount = 0;
        public ExcelHelper()
        {
            xlWorkbook = xlApp.Workbooks.Add();
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

        public static void SetCellValue(int rowIndex, int colIndex, string value)
        {
            xlRange.Cells[rowIndex, colIndex].Value2 = value;
            xlRange.Cells[rowIndex, colIndex].Font.Size = 9;
            int width = 8;
            if (colIndex > 1)
                width = 4;
            xlWorksheet.Columns[colIndex].ColumnWidth = width;
        }

        public static void SetCellHeadertValue(int rowIndex, int colIndex, string value)
        {
            xlRange.Cells[rowIndex, colIndex].Value2 = value;
            xlRange.Cells[rowIndex, colIndex].Font.Size = 10;
            xlRange.Cells[rowIndex, colIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);
            int width = 8;
            if (colIndex > 1)
                width = 4;
            xlWorksheet.Columns[colIndex].ColumnWidth = width;
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
