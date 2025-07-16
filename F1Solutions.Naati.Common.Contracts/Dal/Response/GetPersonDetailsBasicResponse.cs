using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetPersonDetailsBasicResponse : ServiceResponse
    {
        public PersonDetailsBasicDto PersonDetails { get; set; }
    }
}