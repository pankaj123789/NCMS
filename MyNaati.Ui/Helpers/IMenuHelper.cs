using System.Collections.Generic;
using System.Security.Principal;
using MyNaati.Ui.ViewModels.Home;

namespace MyNaati.Ui.Helpers
{
    public interface IMenuHelper
    {
        IEnumerable<MenuLinkModel> Menus(IPrincipal user);
    }
}
