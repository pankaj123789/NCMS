namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class TestSessionSkillCountModel
    {
        public string DisplayName { get; set; }
        public int Attendees { get; set; }
        public int FreeSeats { get; set; }
        public int SkillId { get; set; }
        public int ConfirmedCount { get; set; }
        public int CheckedInCount { get; set; }
        public int SatCount { get; set; }
        public int AwaitingPaymentCount { get; set; }
        public int ProcessingInvoiceCount { get; set; }
    }

    public class TestSittingCountDto
    {
        public int SkillId { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public int TotalSittings { get; set; }
    }
}