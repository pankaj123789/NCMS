//using System.Collections.Generic;
//using System.Linq;
//using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationContactGridRenderer : BasicGridRenderer
//    {
//        private ApplicationWizardModel mWizardModel;

//        public ApplicationContactGridRenderer(ApplicationWizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                return mWizardModel.PhoneList.CurrentPhones.Select(e => new string[] { e.FormattedNumber, string.Empty })
//                    .Concat(mWizardModel.EmailList.CurrentEmails.Select(e => new string[] { e.Email, e.IsPreferred ? "Yes" : string.Empty }))
//                    .Take(5)
//                    .ToList();
//            }
//        }

//        protected override string[] FieldTitles
//        {
//            get { return new[] { "ContactDetails", "ContactPreferred" }; }
//        }
//    }
//}
