using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;

namespace MyNaati.Contracts.BackOffice.Legacy
{
    
    public interface IPersonalDetailsService
    {
        
        PersonalDetailsGetAddressesResponse GetAddresses(PersonNaatiNumberRequest request);

        
        PersonalDetailsGetPersonResponse GetPerson(PersonNaatiNumberRequest request);
    }
}