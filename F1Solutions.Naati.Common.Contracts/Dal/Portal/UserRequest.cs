using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public class UserRequest
    {

        public int NaatiNumber { get; set; }

        public Guid AspUserId { get; set; }
    }

    public class UserResponse
    {

        public int NaatiNumber { get; set; }

        public Guid AspUserId { get; set; }
    }
}
