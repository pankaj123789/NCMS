namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetTestMaterialsFileRequest
    {
        public int TestMaterialId { get; set; }
        public int NAATINumber { get; set; }
        public string TempFileStorePath { get; set; }
    }
}