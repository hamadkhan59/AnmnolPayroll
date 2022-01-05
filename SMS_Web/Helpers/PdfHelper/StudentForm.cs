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
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class StudentForm : BasicPdf
    {
        private IClassSectionRepository clasecRepo;
        public StudentForm()
        {
            clasecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        }
        public PdfDocument CreatePdf(Student student)
        {
            PdfDocument pdfdoc = new PdfDocument();
            pdfdoc.Info.Title = "First Pdf";

            CreatePdf(student, pdfdoc);

            return pdfdoc;
        }

        string isNull(string token)
        {
            if (string.IsNullOrEmpty(token))
                token = "";
            return token;
        }
        private void CreatePdf(Student student, PdfDocument pdfdoc)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                var classSecModel = clasecRepo.GetClassSectionsModelById((int)student.ClassSectionId);
                var religion = clasecRepo.GetReligionById((int)student.ReligionCode);
                var gender = clasecRepo.GetGenderById((int)student.GenderCode);
                PdfPage page = pdfdoc.AddPage();
                //page.Height = 560;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                string imageLoc = student.ImageLocation;
                if (student.StdImage != null)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap bitmap1 = (Bitmap)tc.ConvertFrom(student.StdImage);
                    grph.DrawImage(bitmap1, 460, 170, 110, 110);
                }
                else
                    grph.DrawRectangle(XPens.RoyalBlue, 460, 170, 110, 110);
                grph.DrawString("Student Admission Form", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawRectangle(XPens.Khaki, XBrushes.Khaki, 30, 210, 425, 20);
                grph.DrawString("Student Information: ", font, XBrushes.Black, new XRect(40, 215, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);


                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(40, 260, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(student.Name), font, XBrushes.Black, new XRect(100, 255, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 75, 270, 295, 270);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Roll No.", font, XBrushes.RoyalBlue, new XRect(310, 260, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(student.RollNumber), font, XBrushes.Black, new XRect(370, 255, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 355, 270, 455, 270);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(40, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(classSecModel.ClassName), font, XBrushes.Black, new XRect(100, 285, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 75, 300, 200, 300);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(210, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(classSecModel.SectionName), font, XBrushes.Black, new XRect(280, 285, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 255, 300, 380, 300);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Relegion", font, XBrushes.RoyalBlue, new XRect(390, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                //string secName = clasMang.GetSectionName(student.SectionId);
                grph.DrawString(isNull(religion.Name), font, XBrushes.Black, new XRect(460, 285, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 445, 300, 565, 300);


                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Gender", font, XBrushes.RoyalBlue, new XRect(40, 320, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(gender.Gender1), font, XBrushes.Black, new XRect(100, 315, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 85, 330, 270, 330);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Date Of Birth", font, XBrushes.RoyalBlue, new XRect(280, 320, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                string[] date = student.DateOfBirth.Value.Date.ToString().Split(' ');
                grph.DrawString(date[0], font, XBrushes.Black, new XRect(380, 315, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 365, 330, 565, 330);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Nationality", font, XBrushes.RoyalBlue, new XRect(40, 350, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (student.Nationality != null && student.Nationality.Length > 0)
                {
                    grph.DrawString(isNull(student.Nationality), font, XBrushes.Black, new XRect(110, 345, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 105, 360, 300, 360);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Cast", font, XBrushes.RoyalBlue, new XRect(310, 350, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (student.Cast != null && student.Cast.Length > 0)
                {
                    grph.DrawString(isNull(student.Cast), font, XBrushes.Black, new XRect(370, 345, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 340, 360, 565, 360);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawRectangle(XPens.Khaki, XBrushes.Khaki, 30, 395, 545, 20);
                grph.DrawString("Parents Information: ", font, XBrushes.Black, new XRect(40, 400, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 30, 415, 575, 415);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(40, 445, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(student.FatherName), font, XBrushes.Black, new XRect(100, 440, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 75, 455, 345, 455);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Occupation", font, XBrushes.RoyalBlue, new XRect(355, 445, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (student.FatherOccupation != null && student.FatherOccupation.Length > 0)
                {
                    grph.DrawString(isNull(student.FatherOccupation), font, XBrushes.Black, new XRect(440, 440, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 425, 455, 565, 455);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("CNIC", font, XBrushes.RoyalBlue, new XRect(40, 475, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(student.FatherCNIC), font, XBrushes.Black, new XRect(100, 470, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 75, 485, 200, 485);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Contact1", font, XBrushes.RoyalBlue, new XRect(210, 475, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(student.Contact_1), font, XBrushes.Black, new XRect(280, 470, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 265, 485, 380, 485);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Contact2", font, XBrushes.RoyalBlue, new XRect(390, 475, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                //string secName = clasMang.GetSectionName(student.SectionId);
                if (student.Contact_2 != null && student.Contact_2.Length > 0)
                {
                    grph.DrawString(isNull(student.Contact_2), font, XBrushes.Black, new XRect(460, 470, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 445, 485, 565, 485);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Email", font, XBrushes.RoyalBlue, new XRect(40, 505, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (student.Email != null && student.Email.Length > 0)
                {
                    grph.DrawString(isNull(student.Email), font, XBrushes.Black, new XRect(80, 500, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 75, 515, 265, 515);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Mother Name", font, XBrushes.RoyalBlue, new XRect(275, 505, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(isNull(student.MotherName), font, XBrushes.Black, new XRect(385, 500, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 370, 515, 565, 515);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Contact No", font, XBrushes.RoyalBlue, new XRect(40, 535, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (student.MotherContact1 != null && student.MotherContact1.Length > 0)
                {
                    grph.DrawString(isNull(student.MotherContact1), font, XBrushes.Black, new XRect(110, 530, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 105, 545, 300, 545);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Contact No(Emr) ", font, XBrushes.RoyalBlue, new XRect(310, 535, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                if (student.MotherContact2 != null && student.MotherContact2.Length > 0)
                {
                    grph.DrawString(isNull(student.MotherContact2), font, XBrushes.Black, new XRect(420, 530, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                grph.DrawLine(XPens.Black, 410, 545, 565, 545);

                //string temp = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaa aaaaaaaaa aaaaaaaa aaaaaaa bbbbbbbbbbbbbb bbbbbbbbbbb bbbbbbbbbbbbb bbbbb bbbbbbbb bbbb";
                if (student.CurrentAddress == null || student.CurrentAddress.Length == 0)
                    student.CurrentAddress = "";
                string[] half = CutString(student.CurrentAddress);
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Current Address", font, XBrushes.RoyalBlue, new XRect(40, 565, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(half[0], font, XBrushes.Black, new XRect(140, 560, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 135, 575, 565, 575);

                grph.DrawString(half[1], font, XBrushes.Black, new XRect(50, 595, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 40, 605, 565, 605);

                if (student.PermanentAddress == null || student.PermanentAddress.Length == 0)
                    student.PermanentAddress = "";
                half = CutString(student.PermanentAddress);
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Permanent Address", font, XBrushes.RoyalBlue, new XRect(40, 625, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(half[0], font, XBrushes.Black, new XRect(170, 620, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 160, 635, 565, 635);

                grph.DrawString(half[1], font, XBrushes.Black, new XRect(50, 650, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 40, 665, 565, 665);

                if (student.ChildPictures)
                    grph.DrawRectangle(XPens.Black, 50, 685, 10, 10);
                else
                    grph.DrawRectangle(XPens.Black, XBrushes.Black, 50, 685, 10, 10);
                grph.DrawString("Child Pictures(3)", font, XBrushes.Black, new XRect(70, 685, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                if (student.CNIC)
                    grph.DrawRectangle(XPens.Black, 180, 685, 10, 10);
                else
                    grph.DrawRectangle(XPens.Black, XBrushes.Black, 180, 685, 10, 10);

                grph.DrawString("Father CNIC", font, XBrushes.Black, new XRect(200, 685, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                if (student.BirthCertificate)
                    grph.DrawRectangle(XPens.Black, 285, 685, 10, 10);
                else
                    grph.DrawRectangle(XPens.Black, XBrushes.Black, 285, 685, 10, 10);

                grph.DrawString("Birth Certificate", font, XBrushes.Black, new XRect(305, 685, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                if (student.SchoolLeaving)
                    grph.DrawRectangle(XPens.Black, 410, 685, 10, 10);
                else
                    grph.DrawRectangle(XPens.Black, XBrushes.Black, 410, 685, 10, 10);

                grph.DrawString("School Leaving Certificate", font, XBrushes.Black, new XRect(430, 685, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);


                grph.DrawString("Parents Signature", font, XBrushes.Black, new XRect(50, 720, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 40, 735, 200, 735);

                grph.DrawString("Principal Signature", font, XBrushes.Black, new XRect(400, 720, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 380, 735, 565, 735);



                base.DesignBorder(grph, page, (int)page.Height);
                base.DesignSchoolHeader(grph, page, (int)student.BranchId);

                //Process.Start(pdfFilename);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
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
