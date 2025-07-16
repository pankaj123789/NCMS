namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class AddressDetailsDto
    {
        public int? AddressId { get; set; }
        public int EntityId { get; set; }
        public string StreetDetails { get; set; }
        public string Postcode { get; set; }
        public int? PostcodeId { get; set; }
        public string CountryName { get; set; }
        public int? CountryId { get; set; }
        public string Suburb { get; set; } // actually contains Suburb, Postcode, State - see AddressSelect.sql
        public string SuburbName { get; set; }
        public string StateAbbreviation { get; set; }
        public int? SuburbId { get; set; }
        public string Note { get; set; }
        public bool PrimaryContact { get; set; }
        public bool ValidateInExternalTool { get; set; }
        public bool Invalid { get; set; }
        public bool ExaminerCorrespondence { get; set; }
        public bool IsExaminer { get; set; }
        public int OdAddressVisibilityTypeId { get; set; }
        public string OdAddressVisibilityTypeName => DisplayName;
        public string DisplayName { get; set; }
        public bool IsOrganisation { get; set; }
    }
}