using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts
{
    public interface IMyNaatiIntegrationService
    {
        GenericResponse<bool> DeleteUser(string username);

        BusinessServiceResponse RenameUser(string oldUserName, string newUserName);

        GenericResponse<bool> UnlockUser(string username);

        GenericResponse<MyNaatiUserDetailsModel> GetUser(string userName);
    }
}
