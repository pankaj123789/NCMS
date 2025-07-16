namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetRolePlaySessionRequest
    {
        public int? TestSessionRolePlayerId { get; set; }
        public int NaatiNumber { get; set; }

        public bool IncludeRejected { get; set; }
    }
}