using System.Configuration;
using System.Web;
using System.Web.Optimization;

namespace GarronT.CMS.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                // "~/Content/libs/jquery/jquery.min.js",
                       "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jquery.validate.js",
                     "~/Scripts/jquery.validate.unobtrusive.js",
                     "~/Scripts/Layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));




            bundles.Add(new StyleBundle("~/Content/KSPLcss").Include(
                                        "~/Content/MSTS/Admin/css/bootstrap.css",
                                          "~/Content/MSTS/Admin/css/bootstrap.min.css",
                                        "~/Content/MSTS/Admin/bootstrap-reset.css",
                                        "~/Content/MSTS/Admin/assets/font-awesome/css/font-awesome.css",
                                        "~/Content/MSTS/Admin/css/owl.carousel.css",
                                        "~/Content/MSTS/Admin/css/slidebars.css",
                                        "~/Content/MSTS/Admin/css/site.css"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap-multiselect").Include("~/Content/MSTS/admin/js/bootstrap-multiselect.js"));
            bundles.Add(new ScriptBundle("~/bundles/Project").Include("~/Scripts/Admin/Project.js"));


            bundles.Add(new ScriptBundle("~/bundles/ProductMovementReport").Include("~/Scripts/Admin/ProductMovementReport.js"));

            bundles.Add(new ScriptBundle("~/bundles/Reports").Include("~/Scripts/Admin/Reports.js"));


            bundles.Add(new ScriptBundle("~/bundles/Validate").Include("~/Scripts/jquery.validate.js",
                                                               "~/Scripts/jquery.validate.min.js",
                                                               "~/Scripts/jquery.validate.unobtrusive.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/Chosen").Include("~/Content/MSTS/admin/chosenddl/chosen.jquery.min.js",
                                                               "~/Content/MSTS/admin/chosenddl/chosen.proto.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/UploadInventory").Include("~/Scripts/Admin/UploadInventory.js"));


            bundles.Add(new ScriptBundle("~/bundles/jquery_iviewer_js1").Include(
                "~/jQuery-Plugin-for-Image-Zoom-Rotation-Plugin-iviewer/test/jqueryui.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery_iviewer_js").Include(
                //"~/jQuery-Plugin-for-Image-Zoom-Rotation-Plugin-iviewer/test/jqueryui.js",
          "~/jQuery-Plugin-for-Image-Zoom-Rotation-Plugin-iviewer/test/jquery.mousewheel.min.js",
          "~/jQuery-Plugin-for-Image-Zoom-Rotation-Plugin-iviewer/jquery.iviewer.js"
          ));

            bundles.Add(new StyleBundle("~/Content/jquery_iviewer_css").Include("~/jQuery-Plugin-for-Image-Zoom-Rotation-Plugin-iviewer/jquery.iviewer.css"));

            bundles.Add(new StyleBundle("~/jquery-confirm-master/css/jquery_confirm_css").Include("~/jquery-confirm-master/css/jquery-confirm.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-confirm_js").Include("~/jquery-confirm-master/js/jquery-confirm.js"));

            bundles.Add(new ScriptBundle("~/bundles/ReceiveInventory_js").Include("~/Scripts/Admin/ReceiveInventory.js"));

            bundles.Add(new ScriptBundle("~/bundles/AuditSetting_js").Include("~/Scripts/Admin/AuditSetting.js"));

            bundles.Add(new ScriptBundle("~/bundles/AuditClone_js").Include("~/Scripts/Admin/AuditClone.js"));
            bundles.Add(new ScriptBundle("~/bundles/AuditUpdate_js").Include("~/Scripts/Admin/AuditUpdate.js"));
            bundles.Add(new ScriptBundle("~/bundles/AuditEdit_js").Include("~/Scripts/Admin/AuditEdit.js"));


            bundles.Add(new ScriptBundle("~/bundles/ExportReport_js").Include("~/Scripts/Admin/ExportReport.js"));


             bundles.Add(new ScriptBundle("~/bundles/Dashboard_js").Include("~/Scripts/Admin/DashBoard.js"));

        

            // Code removed for clarity.
            //BundleTable.EnableOptimizations = true;
        }
    }




    public class SiteKeys
    {
        public static string StyleVersion
        {
            get
            {
                return "<link href=\"{0}?v=" + ConfigurationManager.AppSettings["version"] + "\" rel=\"stylesheet\"/>";
            }
        }
        public static string ScriptVersion
        {
            get
            {
                return "<script src=\"{0}?v=" + ConfigurationManager.AppSettings["version"] + "\"></script>";
            }
        }
    }

}
