namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class Sam4AddressData
    {
        public string StreetDetails { get; set; }
        public int? PostcodeId { get; set; }
        public int? CountryId { get; set; }
        public string Note { get; set; }
        public bool PrimaryContact { get; set; }
        public bool IncludeInPd { get; set; }
        public bool Invalid { get; set; }
        public bool ValidateInExternalTool { get; set; }
    }
}