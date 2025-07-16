namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveMaterialRequestRoundLinkRequest
    {
        public int MaterialRequestRoundId { get; set; }
        public int NaatiNumber { get; set; }
        public bool NcmsAvailable { get; set; }
        public string Link { get; set; }
    }
}
