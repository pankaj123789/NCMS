namespace MyNaati.Contracts.Portal.Users
{
    
    public class GetRegistrationRequest
    {
        public GetRegistrationRequest(int getId)
        {
            Id = getId;
        }

        
        public int Id { get; set; }
    }
}