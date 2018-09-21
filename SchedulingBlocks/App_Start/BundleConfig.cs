using System.Web;
using System.Web.Optimization;

namespace SchedulingBlocks
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/client/libs/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/client/libs/bootstrap/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/parsley").Include(
                        "~/client/scripts/parsley.js"));

            bundles.Add(new StyleBundle("~/client/css").Include(
                        "~/client/libs/bootstrap/bootstrap.css",
                        "~/client/styles/bootstrap-extensions.css",
                        "~/client/libs/font-awesome/css/font-awesome.css",
                        "~/client/styles/site.css"));

            bundles.Add(new ScriptBundle("~/client/js").Include(
                        "~/client/scripts/Slots.js",
                        "~/client/scripts/Days.js",
                        "~/client/scripts/Cart.js",
                        "~/client/scripts/Confirm.js"));
        }
    }
}