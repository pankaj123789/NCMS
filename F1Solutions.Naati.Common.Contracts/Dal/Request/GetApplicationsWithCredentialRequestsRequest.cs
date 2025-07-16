namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetApplicationsWithCredentialRequestsRequest
    {
        public int[] NaatiNumber { get; set; }
        public string ApplicationReference { get; set; }
        public string[] ApplicationOwner { get; set; }
        public string[] GivenNames { get; set; }
        public string[] ContactNumber { get; set; }
        public bool? ActiveApplication { get; set; }
        public string[] ApplicationType { get; set; }
        public string[] CredentialRequestType { get; set; }
        public string[] ApplicationStatus { get; set; }
        public string[] CredentialRequestStatus { get; set; }
    }
}