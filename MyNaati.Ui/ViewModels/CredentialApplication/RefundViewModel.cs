using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;

namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class RefundViewModel
    {
        public string RefundPercentage { get; set; }
        public bool IsEligibleForRefund { get; set; }
        public bool RefundNotCalculated { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public string Policy { get; set; }
        public string Comments { get; set; }
        public string BankDetails { get; set; }
        public bool IsManualRefund { get; set; }
        public bool IsPreMigrationRefundRequest { get; set; }
        public string PreMigrationRequestMessage { get; set; }
    }
}