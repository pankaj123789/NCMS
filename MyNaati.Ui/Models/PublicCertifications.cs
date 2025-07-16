using System.Collections.Generic;

namespace MyNaati.Ui.Models
{
    public class PublicCertifications
    {
        public int ErrorCode { get; set; }
        public PublicPractitioner Practitioner { get; set; }
        public IEnumerable<PublicCredentials> CurrentCertifications { get; set; }
        public IEnumerable<PublicCredentials> PreviousCertifications { get; set; }
    }
}