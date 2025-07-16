using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MyNaati.Ui.Helpers
{
    public class FormBoxHelper
    {        

        /// <summary>
        /// Return HTML for a set of empty date boxes in DD/MM/YYYY format.
        /// </summary>
        /// <returns></returns>
        public static HtmlString DateBox()
        {            

            var result = new StringBuilder();

            result.AppendFormat("<span class='box-background'>{0}</span>", "D");
            result.AppendFormat("<span class='box-background'>{0}</span>", "D");
            result.Append("<span class='box-container'>/</span>");
            result.AppendFormat("<span class='box-background'>{0}</span>", "M");
            result.AppendFormat("<span class='box-background'>{0}</span>", "M");
            result.Append("<span class='box-container'>/</span>");
            result.AppendFormat("<span class='box-background'>{0}</span>", "Y");
            result.AppendFormat("<span class='box-background'>{0}</span>", "Y");
            result.AppendFormat("<span class='box-background'>{0}</span>", "Y");
            result.AppendFormat("<span class='box-background'>{0}</span>", "Y");

            return new HtmlString(result.ToString());
        }

        public static HtmlString ExpiryDateBox()
        {
            var result = new StringBuilder();

            result.AppendFormat("<span class='box-background'>{0}</span>", "M");
            result.AppendFormat("<span class='box-background'>{0}</span>", "M");
            result.Append("<span class='box-container'>/</span>");
            result.AppendFormat("<span class='box-background'>{0}</span>", "Y");
            result.AppendFormat("<span class='box-background'>{0}</span>", "Y");

            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Return a set of form boxes with the given value filled out.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static HtmlString Boxes(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                result.AppendFormat("<span class='box'>{0}</span>", c);
            }
            
            return new HtmlString(result.Replace("> </", ">&nbsp;</").ToString());            
        }

        public static HtmlString Boxes(string input, int length)
        {                        
            HtmlString nospaces = Boxes(input.PadRight(length));
            return new HtmlString(nospaces.ToString().Replace("> </", ">&nbsp;</"));
        }


        public static HtmlString UntickedBox(UrlHelper url)
        {
            return new HtmlString(String.Format("<img class=\"tickbox\" src=\"{0}\" alt=\"Unticked box\" />", url.Content("~/Content/Images/UntickedBox.png")));
        }

        public static HtmlString TickedBox(UrlHelper url)
        {
            return new HtmlString(String.Format("<img class=\"tickbox\" src=\"{0}\" alt=\"Ticked box\" />", url.Content("~/Content/Images/TickedBox.png")));
        }

        public static HtmlString TickBoxFor(bool model, UrlHelper url) 
        {
            return model ? TickedBox(url) : UntickedBox(url);
        }
    }
}