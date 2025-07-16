namespace Ncms.Contracts.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OfficeId { get; set; }
        public string DomainName { get; set; }
        public string Password { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
    }
}