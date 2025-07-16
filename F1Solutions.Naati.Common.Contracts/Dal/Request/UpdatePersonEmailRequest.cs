using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdatePersonEmailRequest
    {
        public EmailDetailsDto Email { get; set; }
        public bool AllowUpdateWhenMyNaatiRegistered { get; set; }
    }
}