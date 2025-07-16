namespace Ncms.Contracts.Models.Application
{
    public class ApplicationRequest
    {
        public ApplicationRequest()
        {
        }
        public int[] NaatiNumberIntList{ get; set; }
        public string ApplicationReferenceString { get; set; }
        public string[] ApplicationOwnerIntList { get; set; }
        public string PersonNameString { get; set; }
        public string PhoneNumberString { get; set; }
        public bool? ActiveApplicationBoolean { get; set; }
        public string[] ApplicationTypeIntList { get; set; }
        public string[] CredentialRequestTypeIntList { get; set; }
        public string[] ApplicationStatusIntList { get; set; }
        public string[] CredentialRequestStatusIntList { get; set; }
    }

    public class ApplicationExport
    {
        public int? Skip { get; set; }

        public int? Take { get; set; }

        public string Filter { get; set; }

        public ApplicationRequest DeserializedFilter { get; set; }
    }
}
