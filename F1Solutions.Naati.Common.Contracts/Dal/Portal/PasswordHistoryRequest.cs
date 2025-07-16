using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public class PasswordHistoryRequest
    {

        public Guid UserId { get; set; }

        public string Password { get; set; }

        public int DeleteCount { get; set; }
    }
}
