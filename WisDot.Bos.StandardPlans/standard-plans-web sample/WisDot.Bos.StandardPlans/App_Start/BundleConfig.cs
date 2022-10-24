using System;
using System.Web;
using System.Web.Optimization;
using WisDot.Bos.StandardPlans.Data;

namespace WisDot.Bos.StandardPlans
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "RegisterBundles Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.unobtrusive*")
                .Include("~/Scripts/jquery-ui*"));

            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "jquery bundle done", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "jquery val done", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "modernizer bundle done", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "bootstrap bundle done", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "RegisterBundles End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
        }
    }
}
