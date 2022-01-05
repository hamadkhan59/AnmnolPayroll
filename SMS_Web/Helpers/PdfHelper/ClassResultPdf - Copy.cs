using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
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
using PdfSharp.Drawing.Layout;
using SMS_DAL.ViewModel;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;

namespace SMS.Modules.BuildPdf
{
    public class ClassResultPdf : BasicPdf
    {

        static int BranchId = 0;
        private SC_WEBEntities2 db = SessionHelper.dbContext;
        IClassSectionRepository classSecRepo;
        IExamRepository examRepo;

        public ClassResultPdf()
        {
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2()); ;
        }

        public PdfDocument CreatePdf(int ExamTypeId, int classSectionId, int branchId, DateTime IssuedDate, int[] Position, int[] StudentId)
        {
            List<Exam> examList = db.Exams.Where(x => x.ExamTypeId == ExamTypeId && x.ClassSectionId == classSectionId).ToList();
            //ClassSection clsec = db.ClassSections.Find(classSectionId);
            ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(classSectionId);
            BranchId = branchId;
            PdfDocument pdfdoc = new PdfDocument();
            pdfdoc.Info.Title = "First Pdf";
            PdfPage page = pdfdoc.AddPage();

            XGraphics grph = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 130, 537, 30);
            grph.DrawRectangle(XPens.RoyalBlue, 25, 127, 543, 36);
            //if (examName.Equals(""))
            //    examName = "Complete Session Result";
            string examName = db.ExamTypes.Find(ExamTypeId).Name;
            grph.DrawString(examName, font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

            font = new XFont("Verdana", 8, XFontStyle.Regular);

            grph.DrawRectangle(XPens.RoyalBlue, 25, 175, 105, 15);
            grph.DrawRectangle(XPens.RoyalBlue, 130, 175, 180, 15);
            grph.DrawRectangle(XPens.RoyalBlue, 25, 190, 105, 15);
            grph.DrawRectangle(XPens.RoyalBlue, 130, 190, 180, 15);
            grph.DrawRectangle(XPens.RoyalBlue, 310, 190, 100, 15);
            grph.DrawRectangle(XPens.RoyalBlue, 410, 190, 158, 15);


            grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(35, 177, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            string currDate = IssuedDate.Day.ToString().PadLeft(2, '0') + "-" + IssuedDate.Month.ToString().PadLeft(2, '0') + "-" + IssuedDate.Year.ToString();
            grph.DrawString(currDate, font, XBrushes.Black, new XRect(140, 177, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(35, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString(clsec.ClassName.ToUpper(), font, XBrushes.Black, new XRect(140, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //grph.DrawLine(XPens.Black, 77, 190, 355, 190);

            grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(320, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString(clsec.SectionName.ToUpper(), font, XBrushes.Black, new XRect(420, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //grph.DrawLine(XPens.Black, 397, 190, 550, 190);

            Populate_Result(page, pdfdoc, grph, examList, classSectionId, Position, StudentId);

            base.DesignBorder(grph, page, (int)page.Height);
            base.DesignSchoolHeader(grph, page, BranchId);

            return pdfdoc;
        }

        private void Populate_Result(PdfPage page, PdfDocument doc, XGraphics grph, List<Exam> examList, int classSectionId, int[] Position, int[] StudentId)
        {
            grph.DrawRectangle(XPens.RoyalBlue, 25, 220, 50, 20);
            grph.DrawRectangle(XPens.RoyalBlue, 75, 220, 120, 20);
            int count = examList.Count + 2;
            int marksTotal = 0, obtMarksTotal = 0;
            int summaryIndex = 0;
            int length = 390 / count;
            for (int i = 0; i < count; i++)
            {
                if (i == count - 1)
                    grph.DrawRectangle(XPens.RoyalBlue, 195 + (i * length), 220, length - 10, 20);
                else
                    grph.DrawRectangle(XPens.RoyalBlue, 195 + (i * length), 220, length, 20);
            }

            grph.DrawRectangle(XPens.RoyalBlue, 27, 222, 46, 16);
            grph.DrawRectangle(XPens.RoyalBlue, 77, 222, 116, 16);
            for (int i = 0; i < count; i++)
            {
                if (i == count - 1)
                    grph.DrawRectangle(XPens.RoyalBlue, 197 + (i * length), 222, length - 10 - 4, 16);
                else
                    grph.DrawRectangle(XPens.RoyalBlue, 197 + (i * length), 222, length - 4, 16);
            }

            XFont font = new XFont("Verdana", 6, XFontStyle.Bold);

            grph.DrawString("Roll No", font, XBrushes.Black, new XRect(30, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString("Name", font, XBrushes.Black, new XRect(100, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            string[] subj = new string[count - 2];

            int j = 0;
            foreach (Exam exam in examList)
            {
                string name = db.Subjects.Find(exam.CourseId).Name;
                name = String.Join(Environment.NewLine,
                name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                XTextFormatter tf = new XTextFormatter(grph);

                if (name.Length > 8)
                    tf.DrawString(name, font, XBrushes.Black, new XRect(200 + (j * length), 222, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                else
                    tf.DrawString(name, font, XBrushes.Black, new XRect(205 + (j * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                j++;
            }

            grph.DrawString("Total", font, XBrushes.Black, new XRect(205 + (j * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString("Grade", font, XBrushes.Black, new XRect(205 + ((j + 1) * length) - 5, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString("Percent", font, XBrushes.Black, new XRect(205 + ((j + 2) * length) - 5, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString("Pos", font, XBrushes.Black, new XRect(205 + ((j + 3) * length) - 5, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            font = new XFont("Verdana", 6, XFontStyle.Regular);

            int rowPos = 0;
            int position = 220;
            int ij = 0;
            int examId = examList[0].Id;
            //List<ExamResult> erList = db.ExamResults.Where(x => x.ExamId == examId ).ToList();
            List<ExamResultViewModel> erList = examRepo.GetExamResultModelByExamId(examId); ;
            int[] passCount = new int[examList.Count];
            int[] failCount = new int[examList.Count];
            int borderPos = 0;
            foreach (ExamResultViewModel er in erList)
            {
                if (er.LeavingStatus == 1)
                {
                    int newPosition = position + (rowPos + 1) * 20;
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

                    grph.DrawRectangle(XPens.RoyalBlue, 25, newPosition, 50, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 75, newPosition, 120, 20);
                    for (int jk = 0; jk < count; jk++)
                    {
                        if (jk == count - 1)
                            grph.DrawRectangle(XPens.RoyalBlue, 195 + (jk * length), newPosition, length - 10, 20);
                        else
                            grph.DrawRectangle(XPens.RoyalBlue, 195 + (jk * length), newPosition, length, 20);
                    }

                    newPosition += 5;
                    grph.DrawString(er.RollNumber.ToString(), font, XBrushes.Black, new XRect(35, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(er.Name.ToString(), font, XBrushes.Black, new XRect(100, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    int kl = 0;
                    foreach (Exam exam in examList)
                    {
                        ExamResult erTemp = db.ExamResults.Where(x => x.ExamId == exam.Id && x.StudentId == er.StudentId).FirstOrDefault();
                        grph.DrawString(erTemp.ObtainedMarks.ToString(), font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        obtMarksTotal = obtMarksTotal + (int)erTemp.ObtainedMarks;
                        marksTotal = marksTotal + (int)exam.TotalMarks;
                        if (SessionHelper.GetGrade((int)erTemp.ObtainedMarks, (int)exam.TotalMarks, 50) == "F")
                            failCount[kl]++;
                        else
                            passCount[kl]++;

                        kl++;
                    }

                    grph.DrawString(obtMarksTotal.ToString(), font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    kl++;
                    grph.DrawString(SessionHelper.GetGrade(obtMarksTotal, marksTotal, 50), font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    kl++;
                    grph.DrawString(Math.Round(((decimal)obtMarksTotal / (decimal)marksTotal) * 100m, 2).ToString(), font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    kl++;
                    int studentIndex = StudentId.ToList().IndexOf((int)er.StudentId);
                    grph.DrawString(Position[studentIndex].ToString(), font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    obtMarksTotal = 0;
                    marksTotal = 0;
                    rowPos++;
                    ij++;
                    borderPos = newPosition;
                    summaryIndex = kl;
                }
            }
            borderPos += 20;
            int borderCount = 0;
            font = new XFont("Verdana", 6, XFontStyle.Bold);
            grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            grph.DrawString("", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            borderPos = borderPos + 20;
            grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            borderPos = borderPos + 20;
            //grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            //grph.DrawString("Pass Percentage", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //borderPos = borderPos + 20;
            grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            grph.DrawString("Total Pass", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            borderPos = borderPos + 20;
            grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            grph.DrawString("Total Fail", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            borderPos = borderPos + 20;
            grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            grph.DrawString("Result Persentage", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            marksTotal = 0;
            borderPos = borderPos - 80;
            foreach (Exam exam in examList)
            {
                grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
                grph.DrawString("", font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                borderPos = borderPos + 20;

                grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
                grph.DrawString(exam.TotalMarks.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                marksTotal += int.Parse(exam.TotalMarks.ToString());
                borderPos = borderPos + 20;

                //grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
                //grph.DrawString(exam.PassPercentage.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //borderPos = borderPos + 20;
                grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
                grph.DrawString(passCount[borderCount].ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                borderPos = borderPos + 20;
                grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
                grph.DrawString(failCount[borderCount].ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                borderPos = borderPos + 20;
                grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
                grph.DrawString(Math.Round((((double)(passCount[borderCount]) / (passCount[borderCount] + failCount[borderCount])) * 100), 2).ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                borderPos = borderPos - 80;
                borderCount++;
            }
            grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //grph.DrawString(marksTotal.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            borderPos = borderPos + 20;
            grph.DrawString(marksTotal.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);

            marksTotal = 0;
            rowPos += 5;
            grph.DrawString("Prepared By ___________________________________________", font, XBrushes.Black, new XRect(50, borderPos + (rowPos + 2) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

        }

        private string GetGrade(int obtained, int totalMarks, int passPercentage)
        {
            int obtPercentage = 0;
            if(totalMarks > 0)
                obtPercentage = (obtained * 100) / totalMarks;
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
            else if (obtPercentage == 0)
                grade = "";
            else
                grade = "C";

            return grade;
        }
        //private string GetGrade(int obtained, int totalMarks, int passPercentage)
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
