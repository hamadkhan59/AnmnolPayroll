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
using SMS_DAL;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class StaffForm : BasicPdf
    {
        public PdfDocument CreatePdf(Staff staff)
        {
            PdfDocument pdfdoc = new PdfDocument();
            pdfdoc.Info.Title = "First Pdf";

            CreatePdf(staff, pdfdoc);

            return pdfdoc;
        }

        private void CreatePdf(Staff staff, PdfDocument pdfdoc)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                //page.Height = 560;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                string imageLoc = staff.ImageLocation;
                if (staff.StaffImage != null)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap bitmap1 = (Bitmap)tc.ConvertFrom(staff.StaffImage);
                    grph.DrawImage(bitmap1, 460, 170, 110, 110);
                }
                else
                    grph.DrawRectangle(XPens.RoyalBlue, 460, 170, 110, 110);
                grph.DrawString("Staff Joining Form", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawRectangle(XPens.Khaki, XBrushes.Khaki, 30, 210, 425, 20);
                grph.DrawString("Staff Information: ", font, XBrushes.Black, new XRect(40, 215, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);


                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(40, 260, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staff.Name, font, XBrushes.Black, new XRect(100, 255, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 75, 270, 295, 270);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Staff Id.", font, XBrushes.RoyalBlue, new XRect(310, 260, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staff.StaffId.ToString(), font, XBrushes.Black, new XRect(370, 255, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 355, 270, 455, 270);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Catagory", font, XBrushes.RoyalBlue, new XRect(40, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staff.Designation.DesignationCatagory.CatagoryName, font, XBrushes.Black, new XRect(100, 285, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 95, 300, 300, 300);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Designation", font, XBrushes.RoyalBlue, new XRect(310, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staff.Designation.Name, font, XBrushes.Black, new XRect(400, 285, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 380, 300, 565, 300);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                //grph.DrawString("Relegion", font, XBrushes.RoyalBlue, new XRect(390, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //font = new XFont("Verdana", 10, XFontStyle.Regular);
                ////string secName = clasMang.GetSectionName(student.SectionId);
                //grph.DrawString(staff.Relegion.Name, font, XBrushes.Black, new XRect(460, 285, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 445, 300, 565, 300);


                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Gender", font, XBrushes.RoyalBlue, new XRect(40, 320, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staff.Gender1.Gender1, font, XBrushes.Black, new XRect(100, 315, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 85, 330, 270, 330);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Date Of Birth", font, XBrushes.RoyalBlue, new XRect(280, 320, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                string[] date = staff.DateOfBirth.Value.Date.ToString().Split(' ');
                grph.DrawString(date[0], font, XBrushes.Black, new XRect(380, 315, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 365, 330, 565, 330);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Nationality", font, XBrushes.RoyalBlue, new XRect(40, 350, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.Nationality != null && staff.Nationality.Length > 0)
                {
                    grph.DrawString(staff.Nationality, font, XBrushes.Black, new XRect(120, 345, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 110, 360, 300, 360);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Relegion", font, XBrushes.RoyalBlue, new XRect(310, 350, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staff.Relegion.Name, font, XBrushes.Black, new XRect(380, 345, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 365, 360, 565, 360);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Mobile No", font, XBrushes.RoyalBlue, new XRect(40, 380, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherPhoneNo != null && staff.FatherPhoneNo.Length > 0)
                {
                    grph.DrawString(staff.FatherPhoneNo, font, XBrushes.Black, new XRect(140, 375, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 100, 390, 300, 390);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Phone No", font, XBrushes.RoyalBlue, new XRect(310, 380, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                //string secName = clasMang.GetSectionName(student.SectionId);
                if (staff.FatherPhoneNo1 != null && staff.FatherPhoneNo1.Length > 0)
                {
                    grph.DrawString(staff.FatherPhoneNo1, font, XBrushes.Black, new XRect(410, 375, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 370, 390, 565, 390);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Email", font, XBrushes.RoyalBlue, new XRect(40, 410, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherEmail != null && staff.FatherEmail.Length > 0)
                {
                    grph.DrawString(staff.FatherEmail, font, XBrushes.Black, new XRect(90, 405, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 75, 420, 565, 420);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawRectangle(XPens.Khaki, XBrushes.Khaki, 30, 445, 545, 20);
                grph.DrawString("Parents Information: ", font, XBrushes.Black, new XRect(40, 450, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 30, 415, 575, 415);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(40, 495, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherName != null && staff.FatherName.Length > 0)
                {
                    grph.DrawString(staff.FatherName, font, XBrushes.Black, new XRect(100, 490, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 75, 505, 345, 505);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Occupation", font, XBrushes.RoyalBlue, new XRect(355, 495, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherOccupation != null && staff.FatherOccupation.Length > 0)
                {
                    grph.DrawString(staff.FatherOccupation, font, XBrushes.Black, new XRect(440, 490, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 425, 505, 565, 505);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("CNIC", font, XBrushes.RoyalBlue, new XRect(40, 525, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherCNIC != null && staff.FatherCNIC.Length > 0)
                {
                    grph.DrawString(staff.FatherCNIC, font, XBrushes.Black, new XRect(100, 520, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 75, 535, 200, 535);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Contact1", font, XBrushes.RoyalBlue, new XRect(210, 525, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherPhoneNo != null && staff.FatherPhoneNo.Length > 0)
                {
                    grph.DrawString(staff.FatherPhoneNo, font, XBrushes.Black, new XRect(280, 520, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 265, 535, 380, 535);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Contact2", font, XBrushes.RoyalBlue, new XRect(390, 525, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                //string secName = clasMang.GetSectionName(student.SectionId);
                if (staff.FatherPhoneNo1 != null && staff.FatherPhoneNo1.Length > 0)
                {
                    grph.DrawString(staff.FatherPhoneNo1, font, XBrushes.Black, new XRect(460, 520, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 445, 535, 565, 535);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Email", font, XBrushes.RoyalBlue, new XRect(40, 555, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (staff.FatherEmail != null && staff.FatherEmail.Length > 0)
                {
                    grph.DrawString(staff.FatherEmail, font, XBrushes.Black, new XRect(90, 550, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 75, 565, 565, 565);

                font = new XFont("Verdana", 10, XFontStyle.Bold);


                //string temp = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaa aaaaaaaaa aaaaaaaa aaaaaaa bbbbbbbbbbbbbb bbbbbbbbbbb bbbbbbbbbbbbb bbbbb bbbbbbbb bbbb";
                if (staff.CurrentAddress == null || staff.CurrentAddress.Length == 0)
                    staff.CurrentAddress = "";
                string[] half = CutString(staff.CurrentAddress);
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Current Address", font, XBrushes.RoyalBlue, new XRect(40, 615, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(half[0], font, XBrushes.Black, new XRect(150, 610, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 135, 625, 565, 625);

                grph.DrawString(half[1], font, XBrushes.Black, new XRect(60, 645, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 40, 655, 565, 655);

                if (staff.PermanentAddress == null || staff.PermanentAddress.Length == 0)
                    staff.PermanentAddress = "";
                half = CutString(staff.PermanentAddress);
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Permanent Address", font, XBrushes.RoyalBlue, new XRect(40, 675, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(half[0], font, XBrushes.Black, new XRect(170, 670, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 155, 685, 565, 685);

                grph.DrawString(half[1], font, XBrushes.Black, new XRect(50, 650, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 40, 715, 565, 715);

                font = new XFont("Verdana", 6, XFontStyle.Regular);
                grph.DrawString("Staff Signature", font, XBrushes.Black, new XRect(50, 770, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 40, 765, 200, 765);

                grph.DrawString("Principal Signature", font, XBrushes.Black, new XRect(400, 770, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 380, 765, 565, 765);



                base.DesignBorder(grph, page, (int)page.Height);
                base.DesignSchoolHeader(grph, page, (int)staff.BranchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //Process.Start(pdfFilename);

        }


        private string[] CutString(string temp)
        {
            string[] cuts = temp.Split(' ');
            string[] halfs = new string[2];
            halfs[0] = "";
            halfs[1] = "";
            bool seccondHalf = false;
            int length = 0;
            foreach (string str in cuts)
            {
                length += str.Length + 1;
                if (length < 70 && !seccondHalf)
                    halfs[0] += str + " ";
                else
                {
                    seccondHalf = true;
                    halfs[1] += str + " ";
                }
            }
            return halfs;
        }
    }
}
