namespace Ncms.Contracts
{
    public interface ISecurityService
    {
        bool AuthenticateNonWindowsUser(ref string username, string password);
        bool AuthenticateWindowsUser(string username);
        bool AuthenticateWindowsUser(ref string username, string password);
        string GetPasswordHash(string password);
    }
}
