using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMS_DAL;
using System.Drawing;
using PdfSharp.Drawing;
using System.ComponentModel;
using PdfSharp.Drawing.BarCodes;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;

namespace SMS_Web.Helpers
{
    public class StudentHelper
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        private ISecurityRepository secRepo;
        private IStudentRepository studentRepo;
        private IClassSectionRepository clsecRepo;
        public StudentHelper()
        {
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
            clsecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        }
        public Image CreateCard(Student student)
        {
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId((int)student.BranchId);
            var gender = clsecRepo.GetGenderById((int) student.GenderCode);
            var clsecModel = clsecRepo.GetClassSectionsModelById((int)student.ClassSectionId);
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
            Image cardImag = (Bitmap)tc.ConvertFrom((byte[])config.CardImage);
            Graphics grph = Graphics.FromImage(cardImag);
            Image cardLogo = new Bitmap((Bitmap)tc.ConvertFrom((byte[])config.SchoolLogo), new Size(50, 50));

            grph.DrawImage(cardLogo, 8, 8);
            var sconfig = secRepo.GetSchoolConfigByBranchId((int)student.BranchId);
            int fontSize = SessionHelper.getCardFontSize(sconfig.SchoolName);
            Font font = new System.Drawing.Font("Segoe UI", fontSize, FontStyle.Bold);
            grph.DrawString(sconfig.SchoolName, font, Brushes.White, new Rectangle(90, 10, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);
            font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            grph.DrawString(sconfig.CampusName.ToUpper(), font, Brushes.White, new Rectangle(180, 45, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);

            font = new System.Drawing.Font("Segoe UI", 15, FontStyle.Bold);
            grph.DrawString(student.Name.ToUpper(), font, Brushes.Black, new Rectangle(15, 90, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);

            font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            if (gender.Gender1.Equals("Male"))
                grph.DrawString("S.o        :  " + student.FatherName, font, Brushes.Black, new Rectangle(15, 140, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);
            else
                grph.DrawString("D.o        :  " + student.FatherName, font, Brushes.Black, new Rectangle(15, 140, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);

            grph.DrawString("Sr.No    :  " + student.SrNo, font, Brushes.Black, new Rectangle(15, 160, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);
            grph.DrawString("Class     :  " + clsecModel.ClassName, font, Brushes.Black, new Rectangle(15, 180, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);
            grph.DrawString("Roll.No :  " + student.RollNumber, font, Brushes.Black, new Rectangle(15, 200, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);

            grph.DrawString("Ph: 0427174951", font, Brushes.White, new Rectangle(20, 270, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);
            grph.DrawString("Email: sent.marry@gmail.com", font, Brushes.White, new Rectangle(200, 270, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);

           
            if (student.StdImage != null)
            {
                //cardLogo = Image.FromFile(student.ImageLocation);
                cardLogo = new Bitmap((Bitmap)tc.ConvertFrom((byte[])student.StdImage), new Size(120, 150));
                grph.DrawImage(cardLogo, 310, 80);
            }

            string[] month = {"January", "February", "March", "April", "May","June",
                              "July", "August", "September", "October", "November", "December"};

            string monthoty = month[2] + ", " + DateTime.Now.Year.ToString();
            font = new System.Drawing.Font("Segoe UI", 6, FontStyle.Bold);
            grph.DrawString("Vaildated till: " + monthoty, font, Brushes.Black, new Rectangle(310, 250, cardImag.Width, cardImag.Height), StringFormat.GenericDefault);

            //string[] str = s.Split(' ');
            //font = new System.Drawing.Font("Segoe UI", 8, FontStyle.Regular);
            //grph.DrawString(str[0] + "-" + str[0], font, Brushes.Black, new Rectangle(312, 250, image.Width, image.Height), StringFormat.GenericDefault);
            CreateBarcode(clsecModel.ClassId + "-" + clsecModel.SectionId + "-" + student.RollNumber, cardImag  );

            //cardImag.Save(storedLocation + "/" + student.Name + "-" + student.RollNumber + ".jpg");
            return cardImag;
        }

        private void CreateBarcode(string code, Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            XGraphics grph = XGraphics.FromGraphics(graph, new XSize(image.Width, image.Height));
            BarCode barCode = BarCode.FromType(CodeType.Code3of9Standard, code, new XSize(180, 20), CodeDirection.LeftToRight);

            grph.DrawBarCode(barCode, Brushes.Black, new Font("Free 3 of 9", 10, GraphicsUnit.World), new XPoint(20, 230));
        }

        
    }
}