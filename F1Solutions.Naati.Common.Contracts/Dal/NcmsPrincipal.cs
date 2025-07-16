using System.Security.Principal;
using F1Solutions.Global.Common;

namespace F1Solutions.Naati.Common.Contracts.Dal
{

    // to be deleted with the Azure refactor
    //public class NcmsPrincipal : IPrincipal
    //{
    //    public IIdentity Identity { get; }

    //    public NcmsPrincipal(IIdentity identity)
    //    {
    //        identity.NotNull(nameof(identity));
    //        Identity = identity;
    //    }
    //    public NcmsPrincipal(string username)
    //    {
    //        username.NotNullOrWhiteSpace(nameof(username));
    //        Identity = new NcmsIdentity(username);
    //    }

    //    public bool IsInRole(string role)
    //    {
    //        return false;
    //    }
    //}

    //public class NcmsIdentity : IIdentity
    //{
    //    public string Name { get; }
    //    public string AuthenticationType { get; }
    //    public bool IsAuthenticated { get; }

    //    public NcmsIdentity(string username)
    //    {
    //        Name = username;
    //    }
    //}
}
