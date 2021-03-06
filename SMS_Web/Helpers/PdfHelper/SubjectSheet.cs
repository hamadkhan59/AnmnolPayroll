using System;
using System.Collections.Generic;
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
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf
{
    public class SubjectSheet : BasicPdf
    {
        private SC_WEBEntities2 db = SessionHelper.dbContext;
        IClassSectionRepository classSecRepo;
        IExamRepository examRepo;

        public SubjectSheet()
        {
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
        }

        int totalMarks;
        int passPercentage;
        private Exam GetExam(int courseId, int examTypeId, int classSectionId)
        {
            Exam exam = new Exam();
            using (SC_WEBEntities2 con = new SC_WEBEntities2())
            {
                var query = from exm in con.Exams
                            where exm.CourseId == courseId && exm.ExamTypeId == examTypeId
                            && exm.ClassSectionId == classSectionId
                            select exm;
                exam = query.FirstOrDefault();
            }

            return exam;
        }

        public PdfDocument CreatePdf(int examTypeId, int classSectionId, int courseId, int BranchId, DateTime IssuedDate)
        {
            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //Exam exam = db.Exams.Where(x => x.CourseId == courseId && x.ExamTypeId == examTypeId && x.ClassSectionId == classSectionId).FirstOrDefault();
                Exam exam = GetExam(courseId, examTypeId, classSectionId);
                ExamType examType = db.ExamTypes.Find(examTypeId);
                Subject subject = db.Subjects.Find(courseId);
                //ClassSection classSection = db.ClassSections.Find(classSectionId);
                var clsecModel = classSecRepo.GetClassSectionsModelById(classSectionId);
                List<ExamResultViewModel> examResultList = examRepo.GetExamResultModelByExamId(exam.Id);
                //List<ExamResult> examResultList = db.ExamResults.Where(x => x.ExamId == exam.Id).ToList();

                totalMarks = (int)exam.TotalMarks;
                passPercentage = (int)exam.PassPercentage;
                pdfdoc.Info.Title = "First Pdf";
                PdfPage page = pdfdoc.AddPage();

                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 110, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 107, 316, 36);
                grph.DrawString("Subject Result Sheet", font, XBrushes.White, new XRect(0, 110, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                //grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                //grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                //grph.DrawString("Subject Result Sheet", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                grph.DrawRectangle(XPens.RoyalBlue, 50, 165, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 165, 170, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 180, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 195, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 180, 170, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 195, 120, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 300, 180, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 250, 195, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 380, 180, 170, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 330, 195, 120, 15);
                //grph.DrawRectangle(XPens.RoyalBlue, 400, 180, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 450, 195, 50, 15);
                //grph.DrawRectangle(XPens.RoyalBlue, 460, 180, 90, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 500, 195, 50, 15);

                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(57, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Subject", font, XBrushes.RoyalBlue, new XRect(57, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(307, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(57, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(257, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                var font1 = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Total", font1, XBrushes.RoyalBlue, new XRect(457, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawString(subject.Name.ToUpper(), font, XBrushes.Black, new XRect(140, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(examType.Name.ToUpper(), font, XBrushes.Black, new XRect(390, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(clsecModel.ClassName.ToUpper(), font, XBrushes.Black, new XRect(140, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(clsecModel.SectionName.ToUpper(), font, XBrushes.Black, new XRect(340, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(totalMarks.ToString().ToUpper(), font1, XBrushes.Black, new XRect(510, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string currDate = IssuedDate.Day.ToString().PadLeft(2, '0') + "-" + IssuedDate.Month.ToString().PadLeft(2, '0') + "-" + IssuedDate.Year.ToString();
                grph.DrawString(currDate, font, XBrushes.Black, new XRect(140, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);


                //grph.DrawString("Subject", font, XBrushes.RoyalBlue, new XRect(50, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(subject.Name.ToUpper(), font, XBrushes.Black, new XRect(110, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 90, 190, 255, 190);

                //grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(260, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(examType.Name.ToUpper(), font, XBrushes.Black, new XRect(308, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 288, 190, 450, 190);

                //grph.DrawString("Total", font, XBrushes.RoyalBlue, new XRect(455, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(totalMarks.ToString().ToUpper(), font, XBrushes.Black, new XRect(500, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 480, 190, 550, 190);

                //grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(50, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(classSection.Class.Name.ToUpper(), font, XBrushes.Black, new XRect(97, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 77, 210, 225, 210);

                //grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(230, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(classSection.Section.Name.ToUpper(), font, XBrushes.Black, new XRect(287, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 267, 210, 380, 210);

                //grph.DrawString("Pass Percentage", font, XBrushes.RoyalBlue, new XRect(385, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(passPercentage.ToString().ToUpper(), font, XBrushes.Black, new XRect(487, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 467, 210, 550, 210);

                Populate_Result(page, pdfdoc, examResultList, grph, totalMarks, BranchId);


                base.DesignBorder(grph, page, (int)page.Height);
                base.DesignSchoolHeader(grph, page, BranchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        private void Populate_Result(PdfPage page, PdfDocument doc, List<ExamResultViewModel> examResultList, XGraphics grph, int totalMarks, int BranchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 220, 50, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 100, 220, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 200, 220, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 400, 220, 75, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 475, 220, 75, 20);

                grph.DrawRectangle(XPens.RoyalBlue, 52, 222, 46, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 102, 222, 96, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 202, 222, 196, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 402, 222, 71, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 477, 222, 71, 16);

                XFont font = new XFont("Verdana", 10, XFontStyle.Bold);

                grph.DrawString("Sr.", font, XBrushes.Black, new XRect(60, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Roll No", font, XBrushes.Black, new XRect(120, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Name", font, XBrushes.Black, new XRect(220, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Marks", font, XBrushes.Black, new XRect(420, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Grade", font, XBrushes.Black, new XRect(495, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 10, XFontStyle.Regular);

                int rowPos = 0;
                int failCount = 0;
                int position = 220;
                int i = 0;
                int newPosition = 0;
                foreach (ExamResultViewModel er in examResultList)
                {
                    if (er.LeavingStatus == 1)
                    {
                        newPosition = position + (rowPos + 1) * 20;

                        if (newPosition >= page.Height - 100)
                        {
                            rowPos = 0;
                            position = 100;
                            page = doc.AddPage();
                            grph = XGraphics.FromPdfPage(page);
                            newPosition = position + (rowPos + 1) * 20;
                            base.DesignBorder(grph, page, (int)page.Height);
                            base.DesignSchoolHeader(grph, page, BranchId);
                        }
                        grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 50, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 100, newPosition, 100, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 200, newPosition, 200, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 400, newPosition, 75, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 475, newPosition, 75, 20);

                        newPosition += 5;
                        grph.DrawString((i + 1).ToString(), font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(er.RollNumber, font, XBrushes.Black, new XRect(120, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(er.Name, font, XBrushes.Black, new XRect(220, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(er.ObtMarks.ToString(), font, XBrushes.Black, new XRect(420, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                        string grade = SessionHelper.GetGrade((int)er.Obtained, totalMarks, 50);
                        if (grade.Equals("F"))
                            failCount++;
                        grph.DrawString(grade, font, XBrushes.Black, new XRect(495, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                        rowPos++;
                        i++;
                    }
                }

                newPosition = position + (rowPos + 2) * 20;

                if (newPosition >= page.Height - 100)
                {
                    rowPos = 0;
                    position = 100;
                    page = doc.AddPage();
                    grph = XGraphics.FromPdfPage(page);
                    newPosition = position + (rowPos + 1) * 20;
                    base.DesignBorder(grph, page, (int)page.Height);
                    base.DesignSchoolHeader(grph, page, BranchId);
                }

                grph.DrawRectangle(XPens.RoyalBlue, 50, position + (rowPos + 2) * 20, 150, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 200, position + (rowPos + 2) * 20, 80, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 50, position + (rowPos + 3) * 20, 150, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 200, position + (rowPos + 3) * 20, 80, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 50, position + (rowPos + 4) * 20, 150, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 200, position + (rowPos + 4) * 20, 80, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 50, position + (rowPos + 5) * 20, 150, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 200, position + (rowPos + 5) * 20, 80, 20);

                int pass = examResultList.Count() - failCount;
                int resultPercentage = (pass * 100) / examResultList.Count();
                font = new XFont("Verdana", 9, XFontStyle.Bold);
                grph.DrawString("Total Students", font, XBrushes.Black, new XRect(60, position + (rowPos + 2) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Total Pass", font, XBrushes.Black, new XRect(60, position + (rowPos + 3) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Total Fail", font, XBrushes.Black, new XRect(60, position + (rowPos + 4) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Result Percentage", font, XBrushes.Black, new XRect(60, position + (rowPos + 5) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 9, XFontStyle.Regular);
                grph.DrawString(examResultList.Count().ToString(), font, XBrushes.Black, new XRect(230, position + (rowPos + 2) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(pass.ToString(), font, XBrushes.Black, new XRect(230, position + (rowPos + 3) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(failCount.ToString(), font, XBrushes.Black, new XRect(230, position + (rowPos + 4) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(resultPercentage + "%", font, XBrushes.Black, new XRect(230, position + (rowPos + 5) * 20 + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private string GetGrade(int obtained)
        {
            int obtPercentage = (obtained * 100) / totalMarks;
            string grade = null;
            if (obtPercentage >= 95)
                grade = "A+";
            else if (obtPercentage >= 90)
                grade = "A";
            else if (obtPercentage >= 85)
                grade = "A-";
            else if (obtPercentage >= 80)
                grade = "B+";
            else if (obtPercentage >= 75)
                grade = "B";
            else if (obtPercentage >= 70)
                grade = "B-";
            else if (obtPercentage >= 65)
                grade = "C+";
            else
                grade = "C";

            return grade;
        }
        //private string GetGrade(int obtained)
        //{
        //    int obtPercentage = (obtained * 100) / totalMarks;
        //    string grade = null;
        //    if (obtPercentage >= 85)
        //        grade = "A+";
        //    else if (obtPercentage >= 80)
        //        grade = "A";
        //    else if (obtPercentage >= 75)
        //        grade = "B+";
        //    else if (obtPercentage >= 70)
        //        grade = "B";
        //    else if (obtPercentage >= 65)
        //        grade = "C+";
        //    else if (obtPercentage >= 60)
        //        grade = "C";
        //    else if (obtPercentage < passPercentage)
        //        grade = "F";
        //    else
        //        grade = "D";

        //    return grade;
        //}



    }


}
