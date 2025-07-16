namespace F1Solutions.Naati.Common.Contracts.Dal.Services
{
    public interface ITokenAuthorisationService
    {
        string GetFreshAccessToken();

        string GetBaseAddress();
    }
}