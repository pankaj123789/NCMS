using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyNaati.Ui.Common
{
    public static class LookupExtensionMethods
    {
        public static IList<SelectListItem> ToSelectList<T>(this IEnumerable<T> inputList, Func<T, string> getText, Func<T, string> getValue)
        {
            return inputList
                .Select(i => new SelectListItem(){ Text = getText(i), Value = getValue(i)})
                .ToList();
        }

        public static IList<SelectListItem> Append(this IList<SelectListItem> inputList, string text, object value)
        {
            return inputList.Append(text, value.ToString());
        }

        public static IList<SelectListItem> Append(this IList<SelectListItem> inputList, string text, string value)
        {
            inputList.Add(new SelectListItem()
            {
                Text = text,
                Value = value
            });
            return inputList;
        }

        public static IList<SelectListItem> PrependDefaultItem(this IList<SelectListItem> inputList)
        {
            inputList.Insert(0, new SelectListItem() { });
            return inputList;
        }
    }
}