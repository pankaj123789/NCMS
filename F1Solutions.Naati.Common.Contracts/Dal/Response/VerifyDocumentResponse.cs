using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public  class VerifyDocumentResponse
    {
        public string Name { get; set; }
        public string PractitionerId { get; set; }
        public string CertificationType { get; set; }
        public string Skill { get; set; }
        public DateTime DateIssued { get; set; }
    }
}
