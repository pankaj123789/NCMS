using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels
{
    public static class ViewModelExtensions
    {
        public static List<SelectListItem> PopulateFromEnum<T>(this List<SelectListItem> list, T selecteditem, bool addEmptyAtTop = true)
        {
            var listItems = new List<SelectListItem>();

            if (addEmptyAtTop)
            {
                listItems.Add(new SelectListItem() { Text = "", Value = "0" });
            }
            foreach (object value in Enum.GetValues(selecteditem.GetType()))
            {
                listItems.Add(new SelectListItem() { Text = value.ToString(), Value = ((int)value).ToString() });
                if (value.ToString() == selecteditem.ToString())
                {
                    listItems[listItems.Count - 1].Selected = true;
                }
            }

            return listItems;
        }
    }
}