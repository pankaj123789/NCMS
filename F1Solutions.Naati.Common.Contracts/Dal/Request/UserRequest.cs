using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UserRequest
    {
        public UserDto User { get; set; }
        public bool UpdatePassword { get; set; }
    }
}