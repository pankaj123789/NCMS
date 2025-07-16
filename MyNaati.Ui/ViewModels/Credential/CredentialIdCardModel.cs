using MyNaati.Ui.ViewModels.Account;
using System.Collections.Generic;
using System.Drawing;

namespace MyNaati.Ui.ViewModels.Credential
{
    public class CredentialIdCardModel
    {
        public List<string> CredentialIdCards { get; set; }

        public MfaAndAccessCodeModel MfaAndAccessCodeModel { get;set;}

        public CredentialIdCardModel()
        {
            MfaAndAccessCodeModel = new MfaAndAccessCodeModel();
        }
    }
}