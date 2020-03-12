using System.Web;
using System.Web.Optimization;

namespace MVC_PWx
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/bundles/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/notiflix-2.0.0.css",
                      "~/Content/site.css",
                      "~/Content/Bundled.css")
                      .IncludeDirectory(
                      "~/Content/dashio-css", "*.css"));

            bundles.Add(new StyleBundle("~/bundles/extensions").Include(
                        "~/Content/lib/jquery.backstretch.min.js",
                        "~/Content/lib/jquery.dcjqaccordion.2.7.js",
                        "~/Content/lib/jquery.scrollTo.min.js",
                        "~/Content/lib/jquery.nicescroll.js",
                        "~/Content/lib/jquery.sparkline.js",
                        "~/Content/lib/gritter/js/jquery.gritter.js",
                        "~/Content/lib/gritter-conf.js",
                        "~/Content/lib/notiflix-2.0.0.js"));

            bundles.Add(new StyleBundle("~/bundles/charts").Include(
                      "~/Content/lib/sparkline-chart.js",
                      "~/Content/lib/zabuto_calendar.js"));

            bundles.Add(new StyleBundle("~/bundles/common").Include(
                      "~/Content/lib/common-scripts.js",
                      "~/Scripts/notifications.js",
                      "~/Scripts/Site.js"));
        }
    }
}
