using System.Web.Mvc;

namespace MyNaati.Ui.Security
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = string.Join(", ", roles);
            Order = 1;
        }
    }
}