using System;
using System.Collections.Generic;
using System.Drawing;


namespace MyNaati.Ui.ViewModels.VerifyPractitioner
{
    public class VerifyPractitionerModel
    {
        public string Name { get; set; }
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

        public VerifyPractitionerModel()
        {
            CertificationDetails = new List<CertificationDetail>();
            PastCertificationDetails = new List<CertificationDetail>();
            QrCodes = new List<string>();            
        }
    }

    public class CertificationDetail
    {
        public string Certification { get; set; }
        public string Skill { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}