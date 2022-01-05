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
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;

namespace SMS.Modules.BuildPdf
{
    public class AwardsList : BasicPdf
    {
        private SC_WEBEntities2 db = SessionHelper.dbContext;
        private IClassSectionRepository secRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        private ISubjectRepository subjRepo = new SubjectRepositoryImp(new SC_WEBEntities2());

        public PdfDocument CreatePdf(int ExamTypeId, int classSectionId, int branchId)
        {
            List<Exam> examList = db.Exams.Where(x => x.ExamTypeId == ExamTypeId && x.ClassSectionId == classSectionId).ToList();
            ClassSectionModel clsec = secRepo.GetClassSectionsModelById(classSectionId);

            PdfDocument pdfdoc = new PdfDocument();
            pdfdoc.Info.Title = "First Pdf";
            PdfPage page = pdfdoc.AddPage();

            XGraphics grph = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 30, 130, 550, 30);
            grph.DrawRectangle(XPens.RoyalBlue, 27, 127, 556, 36);
            //if (examName.Equals(""))
            //    examName = "Complete Session Result";
            string examName = db.ExamTypes.Find(ExamTypeId).Name;
            grph.DrawString(examName, font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

            font = new XFont("Verdana", 10, XFontStyle.Regular);

            grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(50, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString(clsec.ClassName.ToUpper(), font, XBrushes.Black, new XRect(97, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawLine(XPens.Black, 77, 190, 355, 190);

            grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(360, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString(clsec.SectionName.ToUpper(), font, XBrushes.Black, new XRect(427, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawLine(XPens.Black, 397, 190, 550, 190);

            Populate_Result(page, pdfdoc, grph, examList, classSectionId);

            base.DesignBorder(grph, page, (int)page.Height);
            base.DesignSchoolHeader(grph, page, branchId);

            return pdfdoc;
        }

        private void Populate_Result(PdfPage page, PdfDocument doc, XGraphics grph, List<Exam> examList, int classSectionId)
        {
            grph.DrawRectangle(XPens.RoyalBlue, 25, 220, 50, 20);
            grph.DrawRectangle(XPens.RoyalBlue, 75, 220, 120, 20);
            var courseList = db.RegisterCourses.Where(x => x.ClassSectionId == classSectionId).ToList();
            int count = courseList.Count;
           
            int length = 390 / count;
            for (int i = 0; i < count; i++)
            {
                grph.DrawRectangle(XPens.RoyalBlue, 195 + (i * length), 220, length, 20);
            }

            grph.DrawRectangle(XPens.RoyalBlue, 27, 222, 46, 16);
            grph.DrawRectangle(XPens.RoyalBlue, 77, 222, 116, 16);
            for (int i = 0; i < count; i++)
            {
                grph.DrawRectangle(XPens.RoyalBlue, 197 + (i * length), 222, length - 4, 16);
            }

            XFont font = new XFont("Verdana", 6, XFontStyle.Bold);

            grph.DrawString("Roll No", font, XBrushes.Black, new XRect(30, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            grph.DrawString("Name", font, XBrushes.Black, new XRect(100, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            string[] subj = new string[count - 2];

            int j = 0;

            foreach (RegisterCourse exam in courseList)
            {
                string name = subjRepo.GetSubjectById(exam.SubjectId).Name;
                //name = name.Replace(" ", "\r\n");

                name = String.Join(Environment.NewLine,
                name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                XTextFormatter tf = new XTextFormatter(grph);

                if (name.Length > 8)
                    tf.DrawString(name, font, XBrushes.Black, new XRect(200 + (j * length), 222, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                else
                    tf.DrawString(name, font, XBrushes.Black, new XRect(205 + (j * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                j++;
            }

            //grph.DrawString("Total", font, XBrushes.Black, new XRect(205 + (j * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //grph.DrawString("Grade", font, XBrushes.Black, new XRect(205 + ((j + 1) * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            font = new XFont("Verdana", 6, XFontStyle.Regular);

            var studentList = db.Students.Where(x => x.ClassSectionId == classSectionId).ToList();
            int rowPos = 0;
            int position = 220;
            //int examId = examList[0].Id;
            //List<ExamResult> erList = db.ExamResults.Where(x => x.ExamId == examId).ToList();
            //int[] passCount = new int[examList.Count];
            //int[] failCount = new int[examList.Count];
            foreach (Student st in studentList)
            {
                int newPosition = position + (rowPos + 1) * 20;
                if (newPosition >= page.Height - 100)
                {
                    rowPos = 0;
                    position = 100;
                    page = doc.AddPage();
                    grph = XGraphics.FromPdfPage(page);
                    newPosition = position + (rowPos + 1) * 20;
                }

                grph.DrawRectangle(XPens.RoyalBlue, 25, newPosition, 50, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 75, newPosition, 120, 20);
                for (int jk = 0; jk < count; jk++)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 195 + (jk * length), newPosition, length, 20);
                }

                newPosition += 5;
                grph.DrawString(st.RollNumber.ToString(), font, XBrushes.Black, new XRect(35, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(st.Name.ToString(), font, XBrushes.Black, new XRect(100, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                int kl = 0;
                foreach (RegisterCourse exam in courseList)
                {
                    grph.DrawString("", font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    kl++;
                }
                rowPos++;
            }
                //foreach (Exam exam in examList)
                //{
                //    ExamResult erTemp = db.ExamResults.Where(x => x.ExamId == exam.Id && x.StudentId == er.StudentId).FirstOrDefault();
                //    grph.DrawString("", font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //    obtMarksTotal = obtMarksTotal + (int)erTemp.ObtainedMarks;
                //    marksTotal = marksTotal + (int)exam.TotalMarks;
                //    if (GetGrade((int)erTemp.ObtainedMarks, (int)exam.TotalMarks, 50) == "F")
                //        failCount[kl]++;
                //    else
                //        passCount[kl]++;

                //    kl++;
                //}
                
            //    grph.DrawString("", font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //    kl++;
            //    grph.DrawString("", font, XBrushes.Black, new XRect(205 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            //    obtMarksTotal = 0;
            //    marksTotal = 0;
            //    rowPos++;
            //    ij++;
            //    borderPos = newPosition;
            //    summaryIndex = kl;
            //}
            //borderPos += 20;
            //int borderCount = 0;
            //grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            //grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //borderPos = borderPos + 20;
            //grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            //grph.DrawString("Pass Percentage", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //borderPos = borderPos + 20;
            //grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            //grph.DrawString("Total Pass", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //borderPos = borderPos + 20;
            //grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            //grph.DrawString("Total Fail", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //borderPos = borderPos + 20;
            //grph.DrawRectangle(XPens.RoyalBlue, 75, borderPos - 5, 120, 20);
            //grph.DrawString("Result Persentage", font, XBrushes.Black, new XRect(100, borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            //borderPos = borderPos - 80;
            //foreach (Exam exam in examList)
            //{
            //    grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //    grph.DrawString(exam.TotalMarks.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //    borderPos = borderPos + 20;
            //    grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //    grph.DrawString(exam.PassPercentage.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //    borderPos = borderPos + 20;
            //    grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //    grph.DrawString(passCount[borderCount].ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //    borderPos = borderPos + 20;
            //    grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //    grph.DrawString(failCount[borderCount].ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //    borderPos = borderPos + 20;
            //    grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //    grph.DrawString(Math.Round((((double)(passCount[borderCount] - failCount[borderCount]) / (passCount[borderCount] + failCount[borderCount])) * 100),2).ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //    borderPos = borderPos - 80;
            //    borderCount++;
            //}
            //grph.DrawRectangle(XPens.RoyalBlue, 195 + (borderCount * length), borderPos - 5, length, 20);
            //grph.DrawString(marksTotal.ToString(), font, XBrushes.Black, new XRect(205 + (borderCount * length), borderPos, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            //grph.DrawString("Prepared By ___________________________________________", font, XBrushes.Black, new XRect(50, borderPos + (rowPos + 2) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

        }

        private string GetGrade(int obtained, int totalMarks, int passPercentage)
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
