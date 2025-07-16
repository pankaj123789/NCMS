//using MyNaati.Ui.ViewModels.PDListing;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.PD_Listing
//{
//    public class PdListingWorkAreasAndAddressFormReplacements : ITokenReplacement
//    {
//        private readonly WizardModel mWizardModel;

//        public PdListingWorkAreasAndAddressFormReplacements(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "PDListingWorkAreas":
//                    return mWizardModel.ExpertiseCommaSeparated;
//                case "PDListingAddress":
//                    return mWizardModel.AddressToShow;
//            }
//            return null;
//        }
//    }
//}