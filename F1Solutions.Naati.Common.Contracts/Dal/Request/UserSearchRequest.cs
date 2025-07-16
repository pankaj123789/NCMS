namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UserSearchRequest
    {
        public int UserId { get; set; }
        public string Type { get; set; }
        public string  Query { get; set; }
    }
}