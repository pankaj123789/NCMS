using System.Web.Optimization;
using MyNaati.Ui;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(BundleConfig), "RegisterBundles")]

namespace MyNaati.Ui
{
    public class BundleConfig
    {
        public static void RegisterBundles()
        {
            // When <compilation debug="true" />, the full readable version is rendered.
            // When set to <compilation debug="false" />, the minified version will be rendered automatically instead.

            BundleTable.Bundles.Add(new StyleBundle("~/Content/bootstrap-fileinput/css/css")
                .Include("~/Content/bootstrap-fileinput/css/fileinput.css"));

            BundleTable.Bundles.Add(new StyleBundle("~/Content/themes/base/base")
                .Include("~/Content/themes/base/jquery-ui.css")
                .Include("~/Content/themes/base/autocomplete.css"));

            BundleTable.Bundles.Add(new StyleBundle("~/Content/DataTables/css/css")
                .Include("~/Content/DataTables/css/jquery.dataTables.css")
                .Include("~/Content/DataTables/css/buttons.dataTables.css")
                .Include("~/Content/DataTables/css/buttons.bootstrap.css")
                .Include("~/Content/DataTables/css/responsive.dataTables.css"));

            BundleTable.Bundles.Add(new StyleBundle("~/Bundles/Styles")
                .Include("~/Content/chosen.css")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/font-awesome.css")
                .Include("~/Content/bootstrap-multiselect.css")
                .Include("~/Content/animate.css")
                .Include("~/Content/bootstrap-toggle.css")
                .Include("~/Content/toastr.css")
                .Include("~/Content/Overrides.css")
                .Include("~/Content/fonts.css"));

            BundleTable.Bundles.Add(new StyleBundle("~/Bundles/Forms")
                .Include("~/Content/Forms.css"));

            BundleTable.Bundles.Add(new StyleBundle("~/Bundles/Styles/FileUpload")
                .Include("~/Scripts/dropzone/dropzone.css")
                .Include("~/Scripts/dropzone/basic.css"));

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/Scripts")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/jquery.unobtrusive-ajax.js")
                .Include("~/Scripts/jquery.validate.js")
                .Include("~/Scripts/jquery.validate.unobtrusive.js")
                .Include("~/Scripts/jquery-ui-{version}.js")
                .Include("~/Scripts/handlebars-v4.0.5.js")
                .Include("~/Scripts/DataTables/jquery.dataTables.js")
                .Include("~/Scripts/DataTables/dataTables.bootstrap.js")
                .Include("~/Scripts/DataTables/dataTables.buttons.js")
                .Include("~/Scripts/DataTables/dataTables.responsive.js")
                .Include("~/Scripts/DataTables/buttons.bootstrap.js")
                .Include("~/Scripts/DataTables/responsive.bootstrap.js")
                .Include("~/Scripts/DataTables/datetime.js")
                .Include("~/Scripts/autosize.js")
                .Include("~/Scripts/chosen.jquery.js")
                .Include("~/Scripts/plugins/piexif.min.js")
                .Include("~/Scripts/plugins/sortable.min.js")
                .Include("~/Scripts/plugins/purify.min.js")
                .Include("~/Scripts/bootstrap-toggle.js")
                .Include("~/Scripts/toastr.js")
                .Include("~/Scripts/fileinput.js")
                .Include("~/Scripts/Chart.js")
                .Include("~/Scripts/jquery.fileDownload.js")
                .Include("~/Scripts/Chart.titleInside.js")
            );

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/mvcfoolproof").Include(
                "~/Scripts/MicrosoftAjax.js",
                "~/Scripts/MicrosoftMvcAjax.js",
                "~/Scripts/MicrosoftMvcValidation.js",
                "~/Scripts/MvcFoolproofJQueryValidation.js",
                "~/Scripts/MvcFoolproofValidation.js",
                "~/Scripts/mvcfoolproof.unobtrusive.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/q.js",
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout.validation.js",
                        "~/Scripts/knockout.chart.js",
                        "~/scripts/knockout.mapping-latest.js",
                        "~/scripts/knockout.viewmodel.{version}.js",
                        "~/scripts/knockout-jqueryui.js",
                        "~/scripts/moment.js",
                        "~/scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap-multiselect.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/debug").Include(
                        "~/App/customBindings/*.js",
                        "~/App/validationRules/*.js",
                        "~/App/components/*.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/render").Include(
                        "~/Scripts/jsrender.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/Scripts/FileUpload")
                .Include("~/Scripts/dropzone/dropzone.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/Scripts/Loading")
                .Include("~/Scripts/Custom/loading-overlay.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/Scripts/PdSearch")
                .Include("~/Scripts/Custom/pd-search.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/Scripts/PersonalDetails")
                .Include("~/Scripts/Custom/personal-details.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/template").Include(
                "~/Template/libs/jquery/jquery.sparkline/dist/jquery.sparkline.retina.js"
                ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Bundles/Scripts/Credentials")
                .Include("~/Scripts/Custom/credentials.js"));
        }
    }
}

