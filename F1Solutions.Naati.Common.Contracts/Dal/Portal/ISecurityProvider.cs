namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public interface ISecurityProvider
    {
        string CurrentUserEmail { get; }

    }

    public interface IDataSecurityProvider : ISecurityProvider
    {
        bool CurrentUserCanAccess(string email);
    }
}
