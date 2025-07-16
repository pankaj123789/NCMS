using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Recaptcha;

namespace MyNaati.Ui.UI
{
    /// <summary>
    /// Static class for the Extension method to create MvcHtmlString so
    /// the captcha renders correctly, otherwise it is HTMLEncoded and you
    /// see the HTML on the page and not the captcha image.
    /// </summary>
    public static class RecaptchaHelper
    {
        public static MvcHtmlString GenerateMvcHtmlCaptcha(this HtmlHelper htmlHelper)
        {
            var htmlString = htmlHelper.GenerateCaptcha();
            htmlString = htmlString.Replace("theme : 'default'", "theme : 'clean'");
            MvcHtmlString current = MvcHtmlString.Create(htmlString);
            return current;
        }
    }

    public static class HtmlHelperExtensions
    {
        const string KOSettingsPrefix = "kosettings:";

        public static MvcHtmlString KOSettings(this HtmlHelper htmlHelper)
        {
            var settings = ConfigurationManager.AppSettings;
            var json = new List<string>();

            foreach (var key in settings.AllKeys)
            {
                if (!key.StartsWith(KOSettingsPrefix))
                    continue;

                var value = ParseValue(settings[key]);

                json.Add($"\t\t'{key.Replace(KOSettingsPrefix, String.Empty)}': {value}");
            }

            var values = String.Join(",\n", json);
            var htmlString = $@"
<script type=""text/javascript"">
    window.kosettings = {{
{values}
    }};
</script>
";
            return MvcHtmlString.Create(htmlString);
        }

        private static string ParseValue(string value)
        {
            var boolValue = false;

            if (bool.TryParse(value, out boolValue))
            {
                value = boolValue.ToString().ToLower();
            }
            else
            {
                value = $"'{value}'";
            }

            return value;
        }
    }
}