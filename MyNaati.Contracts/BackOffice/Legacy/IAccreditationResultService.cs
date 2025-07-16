using MyNaati.Contracts.BackOffice.Common;

namespace MyNaati.Contracts.BackOffice.Legacy
{

    public interface IAccreditationResultService
    {
        
        AccreditationResults.PersonCredentialsResponse GetCredentialsForPersonIncludeExpiry(PersonNaatiNumberRequest request, bool isExpandResult);

        
        bool IsRevalidationScheme(PersonNaatiNumberRequest request);
    }
}