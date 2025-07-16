using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class TestSessionAvailabilityObject
    {
        public int CredentialRequestId { get; set; }
        public int TestSessionId { get; set; }
        public int ApplicationId { get; set; }
    }
}