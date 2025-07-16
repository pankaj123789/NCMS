namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class UpdatePhotoDto
    {
        public int NaatiNumber { get; set; }
        public string FilePath { get; set; }
        public string TokenToRemoveFromFilename { get; set; }
    }
}