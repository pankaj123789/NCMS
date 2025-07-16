namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetMaterialFileRequest
    {
        public int MaterialId { get; set; }
        public string TempFileStorePath { get; set; }
    }
}