using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SMS_Web.Helpers.PdfHelper;
using SMS_DAL;
using System.Globalization;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{


    public class SalarySlipPdf : BasicPdf
    {
        string monthOfTheYear;
        string currentYear;
        private SC_WEBEntities2 db = SessionHelper.dbContext;
        static int BranchId = 0;

        public PdfDocument CreatePdf(int[] StaffIds, string month, string year, int[] Indexes, int[] LateIN, int[] HalfDays, int[] Absents, int[] BasicSalary, int[] Allownces, int[] Deduction, int[] GrossSalary, int[] AdvancsAdjustment, int[] SecDeductions, int branchId)
        {
            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //monthOfTheYear = month;
                currentYear = year;
                //int monthNumber = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
                monthOfTheYear = new DateTime(2018, int.Parse(month), 1).ToString("MMMM", CultureInfo.InvariantCulture);
                pdfdoc.Info.Title = "First Pdf";

                string forMonth = year + "-" + month.ToString();

                for (int i = 0; i < StaffIds.Count(); i++)
                {
                    int STaffIdTemp = StaffIds[i];
                    int staffIndex = Indexes[i];
                    StaffSalary salary = db.StaffSalaries.Where(x => x.StaffId == STaffIdTemp && x.ForMonth == forMonth).FirstOrDefault();
                    CreatePdf1(salary, pdfdoc, LateIN[staffIndex], HalfDays[staffIndex], Absents[staffIndex], BasicSalary[staffIndex], Allownces[staffIndex], Deduction[staffIndex], GrossSalary[staffIndex], AdvancsAdjustment[staffIndex], SecDeductions[staffIndex]);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        public PdfDocument CreateSheetPdf(int[] StaffIds, string month, string year, int[] Indexes, int[] LateIN, int[] HalfDays, int[] Absents, int[] PaidAmount, int[] Bonus, int[] Deduction, int[] AdvancsAdjustment, int[] SecDeductions, int[] GrossSalary, int[] EarlyOut)
        {
            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //monthOfTheYear = month;
                currentYear = year;
                //int monthNumber = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
                monthOfTheYear = new DateTime(2018, int.Parse(month), 1).ToString("MMMM", CultureInfo.InvariantCulture);
                pdfdoc.Info.Title = "First Pdf";

                string forMonth = year + "-" + month.ToString();

                for (int i = 0; i < Indexes.Count(); i++)
                {
                    int staffIndex = Indexes[i];
                    int STaffIdTemp = StaffIds[staffIndex];
                    StaffSalary salary = db.StaffSalaries.Where(x => x.StaffId == STaffIdTemp && x.ForMonth == forMonth).FirstOrDefault();
                    Staff staf = db.Staffs.Find(STaffIdTemp);
                    int allownce = 0;
                    BranchId = (int)staf.BranchId;
                    //int salary = staf.Salary == null ? 0 : staf.Salary;
                    if (db.StaffAllownces.Where(x => x.StaffId == STaffIdTemp).Count() > 0)
                    {
                        allownce = (int)db.StaffAllownces.Where(x => x.StaffId == STaffIdTemp).Sum(x => x.Amount);
                    }
                    //CreatePdf(salary, pdfdoc, LateIN[staffIndex], HalfDays[staffIndex], Absents[staffIndex], (int)staf.Salary, allownce, Deduction[staffIndex], GrossSalary[staffIndex], AdvancsAdjustment[staffIndex], SecDeductions[staffIndex]);
                    CreatePdf(salary, pdfdoc, LateIN[staffIndex], HalfDays[staffIndex], Absents[staffIndex], (int)staf.Salary, allownce, Deduction[staffIndex], GrossSalary[staffIndex], AdvancsAdjustment[staffIndex], SecDeductions[staffIndex], PaidAmount[staffIndex], Bonus[staffIndex], EarlyOut[staffIndex]);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        private void CreatePdf(StaffSalary staffSalary, PdfDocument pdfdoc, int LateIN, int HalfDays, int Absents, int BasicSalary, int Allownces, int Deduction, int GrossSalary, int advanceAdjustment, int secDeduction, int PaidAmount, int Bonus, int EarlyOut)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                page.Height = 460;

                int GrossSalaryCal = GrossSalary - advanceAdjustment - secDeduction + Bonus;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                string name = "Pay Slip " + monthOfTheYear + " " + currentYear;
                grph.DrawString(name, font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Staff Id", font, XBrushes.RoyalBlue, new XRect(50, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffSalary.StaffId.ToString(), font, XBrushes.Black, new XRect(150, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 120, 190, 255, 190);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Name ", font, XBrushes.RoyalBlue, new XRect(320, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffSalary.Staff.Name, font, XBrushes.Black, new XRect(390, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 360, 190, 555, 190);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Designation", font, XBrushes.RoyalBlue, new XRect(50, 210, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffSalary.Staff.Designation.Name, font, XBrushes.Black, new XRect(150, 205, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 120, 220, 255, 220);

                grph.DrawRectangle(XPens.Black, 50, 230, 250, 175);
                grph.DrawRectangle(XPens.Black, 300, 230, 260, 175);
                grph.DrawLine(XPens.Black, 50, 255, 300, 255);
                grph.DrawLine(XPens.Black, 300, 255, 560, 255);
                grph.DrawLine(XPens.Black, 50, 280, 300, 280);
                grph.DrawLine(XPens.Black, 300, 280, 560, 280);

                grph.DrawLine(XPens.Black, 180, 230, 180, 255);
                grph.DrawLine(XPens.Black, 270, 230, 270, 255);
                grph.DrawLine(XPens.Black, 420, 230, 420, 255);
                grph.DrawLine(XPens.Black, 450, 230, 450, 255);
                grph.DrawLine(XPens.Black, 535, 230, 535, 255);
                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Attendance Detail", font, XBrushes.Black, new XRect(60, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawString("Late Ins / Early Out", font, XBrushes.Black, new XRect(185, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(LateIN.ToString() + "/" + EarlyOut.ToString(), font, XBrushes.Black, new XRect(275, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Half Days", font, XBrushes.Black, new XRect(305, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(HalfDays.ToString(), font, XBrushes.Black, new XRect(425, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Absents", font, XBrushes.Black, new XRect(455, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Absents.ToString(), font, XBrushes.Black, new XRect(540, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Allownce Detail", font, XBrushes.Black, new XRect(130, 263, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Salary Detail", font, XBrushes.Black, new XRect(400, 263, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 8, XFontStyle.Regular);
                var staffAllownces = db.StaffAllownces.Where(x => x.StaffId == staffSalary.StaffId).ToList();
                int count = 290;
                foreach (StaffAllownce allonces in staffAllownces)
                {
                    grph.DrawString(allonces.Allownce.Name, font, XBrushes.Black, new XRect(70, count, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(allonces.Amount.ToString(), font, XBrushes.Black, new XRect(220, count, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    count = count + 20;
                }
                //grph.DrawString("Late Ins", font, XBrushes.Black, new XRect(70, 335, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(LateIN.ToString(), font, XBrushes.Black, new XRect(220, 335, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                //grph.DrawString("Half Days", font, XBrushes.Black, new XRect(70, 355, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(HalfDays.ToString(), font, XBrushes.Black, new XRect(220, 355, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                //grph.DrawString("Absents", font, XBrushes.Black, new XRect(70, 375, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(Absents.ToString(), font, XBrushes.Black, new XRect(220, 375, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Basic Salary", font, XBrushes.Black, new XRect(320, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(BasicSalary.ToString(), font, XBrushes.Black, new XRect(470, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Allownces", font, XBrushes.Black, new XRect(320, 305, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Allownces.ToString(), font, XBrushes.Black, new XRect(470, 305, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Attendance Base Deduction", font, XBrushes.Black, new XRect(320, 320, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Deduction.ToString(), font, XBrushes.Black, new XRect(470, 320, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                int i = 0;
                if (secDeduction > 0)
                {
                    i += 15;
                    grph.DrawString("Security Deduction", font, XBrushes.Black, new XRect(320, 320 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(secDeduction.ToString(), font, XBrushes.Black, new XRect(470, 320 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                if (advanceAdjustment > 0)
                {
                    i += 15;
                    grph.DrawString("Advance Adjustment", font, XBrushes.Black, new XRect(320, 320 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(advanceAdjustment.ToString(), font, XBrushes.Black, new XRect(470, 320 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                if (Bonus > 0)
                {
                    i += 15;
                    grph.DrawString("Bonus", font, XBrushes.Black, new XRect(320, 320 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(Bonus.ToString(), font, XBrushes.Black, new XRect(470, 320 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                grph.DrawLine(XPens.Black, 300, 385, 560, 385);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(320, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(GrossSalaryCal.ToString(), font, XBrushes.Black, new XRect(470, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 300, 400, 560, 400);

                grph.DrawLine(XPens.Black, 50, 385, 300, 385);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(70, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Allownces.ToString(), font, XBrushes.Black, new XRect(220, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 300, 400, 560, 400);

                grph.DrawString("Total salary earned in month of " + monthOfTheYear + " is Rs: " + GrossSalaryCal.ToString() + "/-", font, XBrushes.Black, new XRect(70, 410, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);



                base.DesignBorder(grph, page, 450);
                base.DesignSchoolHeader(grph, page, BranchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void CreatePdf1(StaffSalary staffSalary, PdfDocument pdfdoc, int LateIN, int HalfDays, int Absents, int BasicSalary, int Allownces, int Deduction, int GrossSalary, int advanceAdjustment, int secDeduction)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                page.Height = 460;

                GrossSalary = GrossSalary - advanceAdjustment - secDeduction;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                string name = "Pay Slip " + monthOfTheYear + " " + currentYear;
                grph.DrawString(name, font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Staff Id", font, XBrushes.RoyalBlue, new XRect(50, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffSalary.StaffId.ToString(), font, XBrushes.Black, new XRect(150, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 120, 190, 255, 190);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Name ", font, XBrushes.RoyalBlue, new XRect(320, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffSalary.Staff.Name, font, XBrushes.Black, new XRect(390, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 360, 190, 555, 190);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Designation", font, XBrushes.RoyalBlue, new XRect(50, 210, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffSalary.Staff.Designation.Name, font, XBrushes.Black, new XRect(150, 205, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 120, 220, 255, 220);

                grph.DrawRectangle(XPens.Black, 50, 230, 250, 175);
                grph.DrawRectangle(XPens.Black, 300, 230, 260, 175);
                grph.DrawLine(XPens.Black, 50, 255, 300, 255);
                grph.DrawLine(XPens.Black, 300, 255, 560, 255);
                grph.DrawLine(XPens.Black, 50, 280, 300, 280);
                grph.DrawLine(XPens.Black, 300, 280, 560, 280);

                grph.DrawLine(XPens.Black, 180, 230, 180, 255);
                grph.DrawLine(XPens.Black, 270, 230, 270, 255);
                grph.DrawLine(XPens.Black, 420, 230, 420, 255);
                grph.DrawLine(XPens.Black, 450, 230, 450, 255);
                grph.DrawLine(XPens.Black, 535, 230, 535, 255);
                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Attendance Detail", font, XBrushes.Black, new XRect(60, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawString("LateIns", font, XBrushes.Black, new XRect(185, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(LateIN.ToString(), font, XBrushes.Black, new XRect(275, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Half Days", font, XBrushes.Black, new XRect(305, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(HalfDays.ToString(), font, XBrushes.Black, new XRect(425, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Absents", font, XBrushes.Black, new XRect(455, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Absents.ToString(), font, XBrushes.Black, new XRect(540, 238, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Allownce Detail", font, XBrushes.Black, new XRect(130, 263, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Salary Detail", font, XBrushes.Black, new XRect(400, 263, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 8, XFontStyle.Regular);
                var staffAllownces = db.StaffAllownces.Where(x => x.StaffId == staffSalary.StaffId).ToList();
                int count = 290;
                foreach (StaffAllownce allonces in staffAllownces)
                {
                    grph.DrawString(allonces.Allownce.Name, font, XBrushes.Black, new XRect(70, count, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(allonces.Amount.ToString(), font, XBrushes.Black, new XRect(220, count, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    count = count + 20;
                }
                //grph.DrawString("Late Ins", font, XBrushes.Black, new XRect(70, 335, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(LateIN.ToString(), font, XBrushes.Black, new XRect(220, 335, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                //grph.DrawString("Half Days", font, XBrushes.Black, new XRect(70, 355, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(HalfDays.ToString(), font, XBrushes.Black, new XRect(220, 355, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                //grph.DrawString("Absents", font, XBrushes.Black, new XRect(70, 375, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(Absents.ToString(), font, XBrushes.Black, new XRect(220, 375, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Basic Salary", font, XBrushes.Black, new XRect(320, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(BasicSalary.ToString(), font, XBrushes.Black, new XRect(470, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Allownces", font, XBrushes.Black, new XRect(320, 310, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Allownces.ToString(), font, XBrushes.Black, new XRect(470, 310, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Attendance Deduction", font, XBrushes.Black, new XRect(320, 330, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Deduction.ToString(), font, XBrushes.Black, new XRect(470, 330, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                int i = 0;
                if (secDeduction > 0)
                {
                    i += 20;
                    grph.DrawString("Security Deduction", font, XBrushes.Black, new XRect(320, 330 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(secDeduction.ToString(), font, XBrushes.Black, new XRect(470, 330 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                if (advanceAdjustment > 0)
                {
                    i += 20;
                    grph.DrawString("Advance Adjustment", font, XBrushes.Black, new XRect(320, 330 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(advanceAdjustment.ToString(), font, XBrushes.Black, new XRect(470, 330 + i, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                grph.DrawLine(XPens.Black, 300, 385, 560, 385);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(320, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(GrossSalary.ToString(), font, XBrushes.Black, new XRect(470, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 300, 400, 560, 400);

                grph.DrawLine(XPens.Black, 50, 385, 300, 385);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(70, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Allownces.ToString(), font, XBrushes.Black, new XRect(220, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 300, 400, 560, 400);

                grph.DrawString("Total salary earned in month of " + monthOfTheYear + " is Rs: " + GrossSalary.ToString() + "/-", font, XBrushes.Black, new XRect(70, 410, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);



                base.DesignBorder(grph, page, 450);
                base.DesignSchoolHeader(grph, page, BranchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }
    }
}
