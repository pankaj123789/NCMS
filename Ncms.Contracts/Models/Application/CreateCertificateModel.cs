using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAATI.BusinessServices.API.Models.Application
{
    public class CreateCertificateModel
    {
        public  string Name { get; set; }
        public  string Surname { get; set; }
        public  string CertificationTypeId { get; set; }
        public  string CertificationTypeDisplayName { get; set; }
        public  string LanguageDirection { get; set; }
        public  DateTime CredentialStartDate { get; set; }
        public  DateTime CredentialEndDate { get; set; }

        public string PractitionerNumber { get; set; }
    }
}
