namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetPersonAndContactDetailsResponse
    {
        public GetPersonDetailsResponse PersonDetails { get; set; }
        public GetContactDetailsResponse ContactDetails { get; set; }
    }
}