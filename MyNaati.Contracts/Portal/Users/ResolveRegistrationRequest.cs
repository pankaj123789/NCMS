namespace MyNaati.Contracts.Portal.Users
{
    
    public class ResolveRegistrationRequest
    {
        public ResolveRegistrationRequest(int id, int naatiNumber)
        {
            Id = id;
            NaatiNumber = naatiNumber;
        }

        
        public int Id { get; set; }

        
        public int NaatiNumber { get; set; }
    }
}