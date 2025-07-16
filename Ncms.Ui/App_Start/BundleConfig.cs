using System.Web.Optimization;

namespace Ncms.Ui
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.signalR-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.livequery.js",
                        "~/Scripts/hashtable.js",
						"~/Scripts/jquery.numberformatter-1.2.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js",
                        "~/scripts/knockout.mapping-latest.js",
                        "~/scripts/knockout.viewmodel.{version}.js",
                        "~/scripts/knockout.validation.js",
                        "~/Scripts/knockout.chart.js",
                        "~/scripts/knockout-sortable.js",
                        "~/scripts/knockout-jqAutocomplete.js",
                        "~/scripts/knockout-postbox.js",
                        "~/scripts/knockout.dirtyFlag.js",
                        "~/scripts/knockout-jqueryui.js",
						"~/scripts/knockout.dragdrop.js",
						"~/scripts/toastr.js",
                        "~/scripts/q.js"));

            // Scripts in the App folder are only bundled here in development
            // Release builds should have these scripts bundled at build, by r.js
            bundles.Add(new ScriptBundle("~/bundles/debug").Include(
                        "~/App/services/constant.js",
                        "~/App/extensions/*.js",
                        "~/App/customBindings/*.js",
                        "~/App/validationRules/*.js",
                        "~/App/components/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                        "~/scripts/OverlayScrollbars.js",
                        "~/scripts/moment-with-locales.js",
                        "~/scripts/moment-duration-format.js",
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/datatables/natural.js",
                        "~/Scripts/datatables/dataTables.buttons.js",
                        "~/Scripts/datatables/buttons.html5.js",
                        "~/Scripts/datatables/buttons.print.js",
                        "~/Scripts/datatables/dataTables.select.js",
                        "~/Scripts/DataTables/dataTables.bootstrap.js",
                        "~/Scripts/DataTables/datetime-moment.js",
                        "~/Scripts/DataTables/datetime.js",
                        "~/Scripts/jquery.fileDownload.js",
                        "~/Scripts/jquery.ui.widget.js",
                        "~/Scripts/jquery.iframe-transport.js",
						"~/Scripts/js.cookie.js",
                        "~/Scripts/Chart.js",
                        "~/Scripts/Chart.titleInside.js",
                        "~/Scripts/jquery.fileupload.js",
                        "~/scripts/splitter.js",
                        "~/Scripts/fullcalendar/fullcalendar.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/constant").Include(
               ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/clockpicker.js",
                      "~/Scripts/Inputmask/inputmask.js",
                      "~/Scripts/Inputmask/jquery.inputmask.js",
                      "~/Scripts/Inputmask/inputmask.extensions.js",
                      "~/Scripts/Inputmask/inputmask.date.extensions.js",
					  "~/Scripts/Inputmask/inputmask.numeric.extensions.js",
                      "~/Scripts/locales/bootstrap-datepicker.en-AU.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                    "~/Template/libs/jquery/jquery.sparkline/dist/jquery.sparkline.retina.js",
                    "~/Template/js/ui-load.js",
                    "~/Template/js/ui-jp.config.js",
                    "~/Template/js/ui-jp.js",
                    "~/Template/js/ui-nav.js",
                    "~/Template/js/ui-toggle.js",
                    "~/Template/js/ui-client.js"
                    ));
        }
    }
}
