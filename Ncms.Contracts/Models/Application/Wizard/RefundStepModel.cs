namespace Ncms.Contracts.Models.Application.Wizard
{
    public class RefundStepModel
    {
        public double? RefundPercentage { get; set; }
        public decimal? RefundAmount { get; set; }
        public int RefundMethodTypeId { get; set; }
        public int CredentialWorkflowFeeId { get; set; }
        public string Comments { get; set; }
        public string BankDetails { get; set; }
    }
}
