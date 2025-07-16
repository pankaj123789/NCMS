using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner
{
    public class VerifyPractitionerServiceModel
    {
        public IList<CertificationDetail> CertificationDetails { get; set; }
        public IList<CertificationDetail> PastCertificationDetails { get; set; }

        public string PractitionerId { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string Location { get; set; }
        public List<QrCodeModel> QrCodes { get; set; }
        public string BlMessage { get; set; }
        public bool IsDeceased { get; set; }
        public bool AlowVerifyOnline { get; set; }

        public VerifyPractitionerServiceModel()
        {
            CertificationDetails = new List<CertificationDetail>();
            PastCertificationDetails = new List<CertificationDetail>();
            QrCodes = new List<QrCodeModel>();
        }
    }

    public class CertificationDetail
    {
        public string Certification { get; set; }
        public string Skill { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class QrCodeModel
    {
        public Guid QrCode { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime GeneratedOn { get; set; }
    }
}
