using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.BackOffice.PersonalDetails;

namespace MyNaati.Contracts.BackOffice.AccreditationResults
{
    //
    public interface IAccreditationResultService : IInterceptableservice
    {
        
        PersonCredentialsResponse GetCurrentCredentialsForPerson(NaatiNumberRequest request);
        
        PersonCredentialsResponse GetPreviousCredentialsForPerson(NaatiNumberRequest request);
        
        bool IsRevalidationScheme(NaatiNumberRequest request);
    }
}