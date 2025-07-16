namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class ManageTestModel
    {
        public int? TestSessionId { get; set; }
        public int? TestSessionCredentialRequestId { get; set; }
        public string CustomerNo { get; set; }
        public string TestStart { get; set; }
        public string ExpectedCompletion { get; set; }
        public string Application { get; set; }
        public string TestDateString { get; set; }
        public string CredentialType { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string Skill { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string VenueCoordinates { get; set; }

        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        
        public bool CanChangeRejectTestDate { get; set; }
    }

}