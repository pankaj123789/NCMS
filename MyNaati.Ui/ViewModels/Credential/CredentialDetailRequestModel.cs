using System;
using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.Credential
{

    public class CredentialModel
    {
        public int CredentialId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public bool ShowInOnlineDirectory { get; set; }
        public string CredentialName { get; set; }
        public string CredentialCatergoryName { get; set; }
        public string Direction { get; set; }

        public bool Certification { get; set; }
        public string Status { get; set; }
    }

   

    public class CredentialDetailRequestModel
    {
        public List<CredentialModel> Credentials { get; set; }
        public bool MfaConfigured { get; set; }
    }
}