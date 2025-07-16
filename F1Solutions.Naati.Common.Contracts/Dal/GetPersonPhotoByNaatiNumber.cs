namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class GetPersonPhotoByNaatiNumber
    {
        public int? NaatiNumber { get; set; }
        public string PractitionerNumber { get; set; }
        public string TempFolderPath { get; set; }
    }
}