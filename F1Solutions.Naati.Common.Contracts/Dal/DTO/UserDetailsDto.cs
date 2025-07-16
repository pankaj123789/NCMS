using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OfficeId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
    }
}