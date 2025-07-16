namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class DeleteSubmittedMaterialRequest
    {
        public int MaterialId { get; set; }
        public bool PermanentDelete { get; set; }
    }
}