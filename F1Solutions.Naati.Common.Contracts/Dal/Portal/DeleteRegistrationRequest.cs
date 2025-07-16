namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class DeleteRegistrationRequest
    {
        public DeleteRegistrationRequest(int deleteId)
        {
            Id = deleteId;
        }

        
        public int Id { get; set; }

    }
}