using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace SMS_Web
{
    class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files)
        {
            return files;
        }
    }
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/SMSScripts").Include(
            //            //"~/Scripts/modernizr-*",
            //            "~/Scripts/jquery-ui-1.8.24.js",
            //            "~/Scripts/jquery-1.8.2.intellisense.js",
            //            "~/Scripts/toastr.js",
            //            "~/scripts/custom.js"

            //            //"~/Scripts/jquery-1.8.2.min.js",
            //            //"~/Scripts/jquery.validate.js",
            //            //"~/Scripts/jquery.validate.min.js"
            //            ));

            var bundle = new ScriptBundle("~/bundles/SMSvendorScript").Include(
                        "~/Scripts/jquery-1.8.2.intellisense.js",
                        "~/Scripts/jquery-1.8.2.min.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/jquery-ui-1.8.24.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/vendors/Custom/AppDetail.js",
                        "~/vendors/Custom/AppData.js",
                        "~/vendors/jquery/dist/jquery.min.js",
                        //"~/vendors/bootstrap/dist/js/bootstrap.min.js",
                        "~/vendors/fastclick/lib/fastclick.js",
                        "~/vendors/validator/validator.js",
                        "~/vendors/nprogress/nprogress.js",
                        "~/vendors/iCheck/icheck.min.js",
                        "~/vendors/DateJS/build/date.js",
                        "~/vendors/validator/validator.js",
                        "~/vendors/moment/min/moment.min.js",
                        "~/vendors/bootstrap-daterangepicker/daterangepicker.js",
                        "~/vendors/datatables.net/js/jquery.dataTables.min.js",
                       "~/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js",
                       "~/vendors/datatables.net-buttons/js/dataTables.buttons.min.js",
                       "~/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
                       "~/vendors/datatables.net-buttons/js/buttons.flash.min.js",
                       "~/vendors/datatables.net-buttons/js/buttons.html5.min.js",
                       "~/vendors/datatables.net-buttons/js/buttons.print.min.js",
                       "~/vendors/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                       "~/vendors/datatables.net-keytable/js/dataTables.keyTable.min.js",
                       "~/vendors/datatables.net-responsive/js/dataTables.responsive.min.js",
                       "~/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js",
                       "~/vendors/datatables.net-scroller/js/dataTables.scroller.min.js",
                       "~/vendors/jszip/dist/jszip.min.js",
                       "~/vendors/pdfmake/build/pdfmake.min.js",
                       "~/vendors/pdfmake/build/vfs_fonts.js"
                        );
            bundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundle);

            bundles.Add(new ScriptBundle("~/js").Include(
                        "~/js/bootstrap.js",
                        "~/js/bootstrap.min.js",
                        "~/js/homepage.js"));

            //bundles.Add(new StyleBundle("~/bundles/SMSDataTables").Include(
            //           "~/vendors/datatables.net/js/jquery.dataTables.min.js",
            //           "~/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js",
            //           "~/vendors/datatables.net-buttons/js/dataTables.buttons.min.js",
            //           "~/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
            //           "~/vendors/datatables.net-buttons/js/buttons.flash.min.js",
            //           "~/vendors/datatables.net-buttons/js/buttons.html5.min.js",
            //           "~/vendors/datatables.net-buttons/js/buttons.print.min.js",
            //           "~/vendors/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
            //           "~/vendors/datatables.net-keytable/js/dataTables.keyTable.min.js",
            //           "~/vendors/datatables.net-responsive/js/dataTables.responsive.min.js",
            //           "~/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js",
            //           "~/vendors/datatables.net-scroller/js/dataTables.scroller.min.js",
            //           "~/vendors/jszip/dist/jszip.min.js",
            //           "~/vendors/pdfmake/build/pdfmake.min.js",
            //           "~/vendors/pdfmake/build/vfs_fonts.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            bundles.Add(new StyleBundle("~/bundles/SMSvendors").Include(
                        "~/vendors/bootstrap/dist/css/bootstrap.min.css",
                        "~/vendors/font-awesome/css/font-awesome.min.css",
                        "~/vendors/nprogress/nprogress.css",
                        "~/vendors/iCheck/skins/flat/green.css",
                        "~/vendors/datatables.net-bs/css/dataTables.bootstrap.min.css",
                        "~/vendors/datatables.net-buttons-bs/css/buttons.bootstrap.min.css",
                        "~/vendors/datatables.net-fixedheader-bs/css/fixedHeader.bootstrap.min.css",
                        "~/vendors/datatables.net-responsive-bs/css/responsive.bootstrap.min.css",
                        "~/vendors/datatables.net-scroller-bs/css/scroller.bootstrap.min.css"
                        ));




            bundles.Add(new StyleBundle("~/bundles/SMScss").Include(
                        "~/css/Layout.css",
                        "~/css/toastr.min.css",
                        "~/css/custom.min.css",
                        "~/css/customStyle.css"));


            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));


        }
    }
}