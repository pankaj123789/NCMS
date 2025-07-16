namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPersonDetailsRequest
    {
        public int? NaatiNumber { get; set; }
        public int? EntityId { get; set; }
        public int? PersonId { get; set; }

    }
}