namespace Ncms.Contracts
{
    public class TestSessionSearchRequest
    {
        public int? Skip { get; set; }

        public int? Take { get; set; }

        public string Filter { get; set; }
        public bool IncludeInactiveVenueFlag { get; set; }
    }
}
