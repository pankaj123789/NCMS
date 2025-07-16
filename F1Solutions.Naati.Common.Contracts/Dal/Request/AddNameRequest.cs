using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AddNameRequest
    {
        public int NaatiNumber { get; set; }
        public PersonNameDto Name { get; set; }
    }
}