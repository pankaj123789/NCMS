namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetVenuesRequest
    {
        public int? TestLocationId { get; set; }

        public bool ActiveOnly { get; set; }
    }
}