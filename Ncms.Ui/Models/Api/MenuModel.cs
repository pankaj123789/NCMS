using System.Collections.Generic;

namespace Ncms.Ui.Models.Api
{
    public class MenuModel
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Route { get; set; }
        public IEnumerable<MenuModel> Children { get; set; }

    }
}