using F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace MyNaati.Ui.ViewModels.VerifyCredential
{
    public class VerifyCredentialModel
    {
        public string Name { get; set; }
        public List<CredentialDetail> CredentialDetails { get; set; }
        public IList<CertificationDetail> CertificationDetails { get; set; }
        public IList<CertificationDetail> PastCertificationDetails { get; set; }


        public DateTime GeneratedOn { get; set; }

        public string PractitionerId { get; set; }
        public string Location { get; set; }
        public List<string> QrCodes { get; set; }
        public string Message { get; set; }

        public bool IsDeceased { get; set; }
        public string DigitalStampValid { get; set; }

        public string PractitionerImage { get; set; }

        public VerifyCredentialModel()
        {
            CredentialDetails = new List<CredentialDetail>();

            QrCodes = new List<string>();            
        }
    }

    public class CredentialDetail
    {
        public string Credential { get; set; }
        public string Skill { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}