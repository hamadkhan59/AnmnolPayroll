using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Helpers.ExcelHelpers
{
    public class StaffReportExelHelper : ExcelHelper
    {
        DataSet salaryReport;

        public StaffReportExelHelper() : base()
        {
            salaryReport = new DataSet();
        }

        public Stream GetAllStaffAttendanceReportExcel()
        {
            AddDataToExcel();
            var tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".xlsx");
            xlWorkbook.SaveAs(tempFile);
            xlWorkbook.Close();
            xlApp.Quit();
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(tempFile));
            File.Delete(tempFile);
            stream.Position = 0;
            return stream;
        }

        private void AddDataToExcel()
        {
            if (salaryReport != null && salaryReport.Tables != null && salaryReport.Tables.Count > 0)
            {
                for (int j = 0; j < salaryReport.Tables.Count; j++)
                {
                    DataTable table = salaryReport.Tables[j];
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        for (int k = 0; k < table.Columns.Count; k++)
                        {
                            int currRow = j * 4 + i + 1;
                            if(i == 0)
                                SetCellHeadertValue(currRow, k + 1, table.Rows[i][k].ToString());
                            else
                                SetCellValue(currRow, k + 1, table.Rows[i][k].ToString());

                        }
                    }
                }
            }
        } 

        public void AddStaffDetail(DataSet staffDs)
        {
            if (staffDs != null && staffDs.Tables != null && staffDs.Tables.Count > 0)
            {
                DataTable table = new DataTable();
                DataTable stTable = staffDs.Tables[0];
                string staffId = "StaffId : " + stTable.Rows[0]["StaffId"].ToString();
                string dutyHours = stTable.Rows[0]["DutyHours"].ToString();
                int currentRow = salaryReport.Tables.Count * 4;
                int currentCol = 0;
                table.Columns.Add(staffId);

                for (int i = 0; i < stTable.Rows.Count; i++)
                {
                    string colName = stTable.Rows[i]["DOM"].ToString();
                    if(!table.Columns.Contains(colName))
                        table.Columns.Add(colName);
                }


                table.Rows.Add();
                table.Rows.Add();
                table.Rows.Add();
                table.Rows.Add();
                table.Rows[0][0] = staffId;
                table.Rows[1][0] = $"Hours({dutyHours})";
                table.Rows[2][0] = "TimeIn";
                table.Rows[3][0] = "TimeOut";
                for (int i = 0; i < stTable.Rows.Count; i++)
                {
                    table.Rows[0][i + 1] = stTable.Rows[i]["DOM"].ToString(); 
                    table.Rows[1][i + 1] = stTable.Rows[i]["Hours"].ToString();
                    table.Rows[2][i + 1] = stTable.Rows[i]["TimeIn"].ToString();
                    table.Rows[3][i + 1] = stTable.Rows[i]["TimeOut"].ToString();
                }

                salaryReport.Tables.Add(table);
            }
        }

    }
}