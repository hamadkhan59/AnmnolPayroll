using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SMS_DAL;
using System.Drawing;
using System.ComponentModel;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class StudentPdf : BasicPdf
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        private ISecurityRepository secRepo;
        private IClassSectionRepository clasecRepo;
        private IStudentRepository studentRepo;
        public StudentPdf()
        {
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
            clasecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        }
        public PdfDocument CreateCharahcterCertificatePdf(int[] studentIds)
        {

            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                foreach (int id in studentIds)
                {
                    Student student = studentRepo.GetStudentById(id);

                    createCharacterCertificate(student, pdfdoc);
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

        public PdfDocument CreateSchoolLeavingCertificatePdf(int[] studentIds, string actitvities, string remarks)
        {
            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                foreach (int id in studentIds)
                {
                    Student student = studentRepo.GetStudentById(id);

                    creataSchoolLeavingCertificate(student, pdfdoc, actitvities, remarks);
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

        private void creataSchoolLeavingCertificate(Student student, PdfDocument pdfdoc, string actitvities, string remarks)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                XGraphics grph = XGraphics.FromPdfPage(page);
                var classSecModel = clasecRepo.GetClassSectionsModelById((int)student.ClassSectionId);
                var religion = clasecRepo.GetReligionById((int)student.ReligionCode);

                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                grph.DrawString("School Leaving Certificate", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                if (student.StdImage != null)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap stdImag = (Bitmap)tc.ConvertFrom((byte[])student.StdImage);
                    Bitmap image = new Bitmap(stdImag, new Size(120, 120));
                    grph.DrawImage(image, page.Width - 160, 190);
                }

                font = new XFont("Verdana", 10, XFontStyle.Bold);

                XFont fonts = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Sr. No", font, XBrushes.RoyalBlue, new XRect(70, 230, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(student.SrNo.ToUpper(), fonts, XBrushes.Black, new XRect(225, 227, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 106, 240, 400, 240);

                grph.DrawString("Date Of Admission", font, XBrushes.RoyalBlue, new XRect(70, 250, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string[] str = student.AdmissionDate.ToString().Split(' ');
                grph.DrawString(str[0].ToUpper(), fonts, XBrushes.Black, new XRect(225, 247, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 175, 260, 400, 260);

                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(70, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(student.Name.ToUpper(), fonts, XBrushes.Black, new XRect(236, 287, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 106, 300, page.Width - 70, 300);

                grph.DrawString("Father Name", font, XBrushes.RoyalBlue, new XRect(70, 310, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(student.FatherName.ToUpper(), fonts, XBrushes.Black, new XRect(236, 307, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 145, 320, page.Width - 70, 320);

                grph.DrawString("Date Of Birth", font, XBrushes.RoyalBlue, new XRect(70, 330, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                str = student.DateOfBirth.ToString().Split(' ');
                grph.DrawString(str[0].ToUpper(), fonts, XBrushes.Black, new XRect(236, 327, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 145, 340, page.Width - 70, 340);

                grph.DrawString("Religion", font, XBrushes.RoyalBlue, new XRect(70, 350, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(religion.Name.ToUpper(), fonts, XBrushes.Black, new XRect(236, 347, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 117, 360, page.Width - 70, 360);

                grph.DrawString("Class Admitted In", font, XBrushes.RoyalBlue, new XRect(70, 370, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(classSecModel.ClassName, fonts, XBrushes.Black, new XRect(236, 367, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 175, 380, page.Width - 70, 380);

                grph.DrawString("Class Studied In", font, XBrushes.RoyalBlue, new XRect(70, 390, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(classSecModel.ClassName, fonts, XBrushes.Black, new XRect(236, 387, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 165, 400, page.Width - 70, 400);

                grph.DrawString("Date of Leaving", font, XBrushes.RoyalBlue, new XRect(70, 410, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                str = DateTime.Now.ToString().Split(' ');
                grph.DrawString(str[0].ToUpper(), fonts, XBrushes.Black, new XRect(236, 407, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 160, 420, page.Width - 70, 420);

                grph.DrawString("Activities Excelled", font, XBrushes.RoyalBlue, new XRect(70, 430, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(actitvities.ToUpper(), fonts, XBrushes.Black, new XRect(236, 427, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 175, 440, page.Width - 70, 440);

                grph.DrawString("Conduct", font, XBrushes.RoyalBlue, new XRect(70, 450, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(remarks.ToUpper(), fonts, XBrushes.Black, new XRect(236, 447, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 118, 460, page.Width - 70, 460);

                //grph.DrawString("Subjects Studied", font, XBrushes.RoyalBlue, new XRect(70, 470, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                //if (SubjectLista != null && SubjectLista.Length > 0)
                //    grph.DrawString(SubjectLista.ToUpper(), fonts, XBrushes.Black, new XRect(236, 467, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 175, 480, page.Width - 70, 480);

                //if (SubjectListb != null && SubjectListb.Length > 0)
                //    grph.DrawString(SubjectListb.ToUpper(), fonts, XBrushes.Black, new XRect(150, 487, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 70, 500, page.Width - 70, 500);


                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(70, 570, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 106, 580, 256, 580);

                grph.DrawLine(XPens.Black, 356, 580, page.Width - 70, 580);
                font = new XFont("Verdana", 7, XFontStyle.Regular);
                grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(356, 585, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                base.DesignSchoolHeader(grph, page, (int)student.BranchId);
                DesignBoreder(grph, page, 620);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void createCharacterCertificate(Student student, PdfDocument pdfdoc)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();

                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                grph.DrawString("Character Certificate", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                if (student.StdImage != null)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                    Bitmap stdImag = (Bitmap)tc.ConvertFrom((byte[])student.StdImage);
                    Bitmap image = new Bitmap(stdImag, new Size(120, 120));
                    grph.DrawImage(image, page.Width - 160, 190);
                }
                var classSecModel = clasecRepo.GetClassSectionsModelById((int)student.ClassSectionId);

                font = new XFont("Verdana", 12, XFontStyle.Regular);
                grph.DrawString("This is to certify that Mr/Mrs__________________________", font, XBrushes.Black, new XRect(70, 250, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                XFont fonts = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString(student.Name.ToUpper(), fonts, XBrushes.Black, new XRect(250, 247, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("S.o/D.o___________________________", font, XBrushes.Black, new XRect(70, 270, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(student.FatherName.ToUpper(), fonts, XBrushes.Black, new XRect(130, 267, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(",enrolled in class___", font, XBrushes.Black, new XRect(320, 270, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("_______is studying in school since_______________________. He/she is the ", font, XBrushes.Black, new XRect(70, 290, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                fonts = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString(classSecModel.ClassName, fonts, XBrushes.Black, new XRect(75, 287, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                string[] str = student.AdmissionDate.ToString().Split(' ');
                grph.DrawString(str[0].ToUpper(), fonts, XBrushes.Black, new XRect(285, 287, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("most well-behaved student. He/she bears a reputable character and very ", font, XBrushes.Black, new XRect(70, 310, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("hardworking and co-operative. He/she has no antecedents which render him", font, XBrushes.Black, new XRect(70, 330, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("/her unsuitable for any other institution. He/she is strongly recommended for", font, XBrushes.Black, new XRect(70, 350, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("any institution of high standards.", font, XBrushes.Black, new XRect(70, 370, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(70, 470, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 106, 480, 256, 480);

                grph.DrawLine(XPens.Black, 356, 480, page.Width - 70, 480);
                font = new XFont("Verdana", 7, XFontStyle.Regular);
                grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(356, 485, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                DesignBoreder(grph, page, 520);

                base.DesignSchoolHeader(grph, page, (int)student.BranchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void DesignBoreder(XGraphics grph, PdfPage page, int height)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                for (int i = 10; i < 15; i++)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, i, i, page.Width - i - i, height - i - i);
                }


                grph.DrawRectangle(XPens.RoyalBlue, 6, 6, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, 21, 21, 21, 25);
                grph.DrawLine(XPens.RoyalBlue, 21, 21, 25, 21);
                grph.DrawLine(XPens.RoyalBlue, 21, 25, 6, 25);
                grph.DrawLine(XPens.RoyalBlue, 25, 21, 25, 6);

                grph.DrawRectangle(XPens.RoyalBlue, page.Width - 20, height - 20, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, height - 20, page.Width - 20, height - 24);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, height - 20, page.Width - 24, height - 20);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, height - 24, page.Width - 5, height - 24);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 24, height - 20, page.Width - 24, height - 5);

                grph.DrawRectangle(XPens.RoyalBlue, page.Width - 20, 6, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, 21, page.Width - 24, 21);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, 21, page.Width - 20, 25);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 24, 21, page.Width - 24, 6);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, 25, page.Width - 5, 25);

                grph.DrawRectangle(XPens.RoyalBlue, 6, height - 20, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, 21, height - 20, 25, height - 20);
                grph.DrawLine(XPens.RoyalBlue, 21, height - 20, 21, height - 25);
                grph.DrawLine(XPens.RoyalBlue, 25, height - 20, 25, height - 5);
                grph.DrawLine(XPens.RoyalBlue, 21, height - 25, 6, height - 25);

                grph.DrawLine(XPens.RoyalBlue, 6, 25, 6, height - 25);
                grph.DrawLine(XPens.RoyalBlue, 25, 6, page.Width - 24, 6);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 5, 25, page.Width - 5, height - 24);
                grph.DrawLine(XPens.RoyalBlue, 25, height - 5, page.Width - 24, height - 5);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void DesignSchoolHeader(XGraphics grph, PdfPage page, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                grph.DrawImage((Bitmap)tc.ConvertFrom((byte[])config.SchoolLogo), 25, 25);

                XFont font = new XFont("Verdana", 25, XFontStyle.Bold);
                if (config.SchoolName.Length > 30)
                    font = new XFont("Verdana", 20, XFontStyle.Bold);
                else if (config.SchoolName.Length > 40)
                    font = new XFont("Verdana", 18, XFontStyle.Bold);
                else if (config.SchoolName.Length > 50)
                    font = new XFont("Verdana", 15, XFontStyle.Bold);
                grph.DrawString(config.SchoolName, font, XBrushes.Black, new XRect(0, 30, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                if (config.SchoolName.Length > 55)
                    font = new XFont("Verdana", 9, XFontStyle.Bold);

                if (config.CampusName.Length > 55)
                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                else if (config.CampusName.Length > 50)
                    font = new XFont("Verdana", 10, XFontStyle.Bold);
                else if (config.CampusName.Length > 40)
                    font = new XFont("Verdana", 12, XFontStyle.Bold);
                else
                    font = new XFont("Verdana", 15, XFontStyle.Bold);

                grph.DrawString(config.CampusName, font, XBrushes.Black, new XRect(0, 70, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                for (int i = 0; i < 5; i++)
                {
                    grph.DrawLine(XPens.RoyalBlue, 10, 90 + i, page.Width - 10, 90 + i);
                }
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