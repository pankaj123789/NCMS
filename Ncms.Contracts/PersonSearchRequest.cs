namespace Ncms.Contracts
{
    public class PersonSearchRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string Filter { get; set; }
    }
}
