namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PanelMembershipSummaryDto
    {
        public int PanelMembershipId { get; set; }
        public int Overidue { get; set; }
        public int InProgress { get; set; }
        public int PersonId { get; set; }
    }
}