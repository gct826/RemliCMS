using System.Web;
using System.Web.Optimization;

namespace RemliCMS
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
                        "~/Scripts/foundation/foundation.js",
                        "~/Scripts/foundation/foundation*"));

            bundles.Add(new ScriptBundle("~/bundles/foundation-min").Include(
                        "~/Scripts/foundation.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/list").Include(
                        "~/Scripts/list.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jHtmlArea").Include(
                        "~/Scripts/jHtmlArea-0.8.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css",
                        "~/Content/foundation/foundation.css",
                        "~/Content/foundation/foundation.mvc.css",
                        "~/Content/foundation/foundation-icons.css",
                        "~/Content/foundation/normalize.css",
                        "~/Content/jHtmlArea/jHtmlArea.css"));

            bundles.Add(new StyleBundle("~/Content/css_rtl").Include(
                        "~/Content/site.css",
                        "~/Content/foundation/foundation_rtl.css",
                        "~/Content/foundation/foundation.mvc.css",
                        "~/Content/foundation/foundation-icons.css",
                        "~/Content/foundation/normalize.css"));

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