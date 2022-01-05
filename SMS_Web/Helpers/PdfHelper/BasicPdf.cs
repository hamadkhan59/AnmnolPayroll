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
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class BasicPdf
    {
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());

        public void DesignBorder(XGraphics grph, PdfPage page, int height)
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

        public void DesignSchoolHeader(XGraphics grph, PdfPage page, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                XFont font = new XFont("Verdana", 25, XFontStyle.Bold);
                if (config.SchoolName.Length > 50)
                    font = new XFont("Verdana", 15, XFontStyle.Bold);
                else if (config.SchoolName.Length > 40)
                    font = new XFont("Verdana", 18, XFontStyle.Bold);
                else if (config.SchoolName.Length > 25)
                    font = new XFont("Verdana", 20, XFontStyle.Bold);


                grph.DrawString(config.SchoolName, font, XBrushes.Black, new XRect(0, 30, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                if (config.SchoolName.Length > 55)
                    font = new XFont("Verdana", 9, XFontStyle.Bold);
                grph.DrawString(config.CampusName, font, XBrushes.Black, new XRect(0, 70, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                grph.DrawImage((Bitmap)tc.ConvertFrom((byte[])config.SchoolLogo), 25, 25, 60, 60);

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

        public void DrawPrincipalSignature(XGraphics grph, PdfPage page, int x, int y, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                if (config != null && config.PrincipalSignature != null)
                    grph.DrawImage((Bitmap)tc.ConvertFrom((byte[])config.PrincipalSignature), x, y);
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
