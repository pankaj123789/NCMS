//using MyNaati.Ui.ViewModels.PDListing;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.PD_Listing
//{
//    public class PdListingPersonalDetailsFromReplacements : ITokenReplacement
//    {
//        private WizardModel mPdListingWizardModel;

//        public PdListingPersonalDetailsFromReplacements(WizardModel basePurchaseWizardModel)
//        {
//            mPdListingWizardModel = basePurchaseWizardModel;
//        }

//        public string GetReplacement(string title)
//        {
//            switch(title)
//            {
//                case "PDListingReferenceNumber":
//                    return string.Format("{0} {1}", "Reference Number:", mPdListingWizardModel.ReferenceNumber);
//                case "PDListingPurchaserName":
//                    return mPdListingWizardModel.Name;
//                case "PDListingNAATINumber":
//                    return mPdListingWizardModel.NaatiNumber.ToString();
//                case "PDListingListingExpiry":
//                    return mPdListingWizardModel.ListingEndDate.ToString("dd MMM yyyy");
//                case "PDListingAdministrationFee":
//                    return string.Format("{0:C}", mPdListingWizardModel.OrderTotalModel.TotalPrice);        
//                case "PDListingTotalFee":
//                    return mPdListingWizardModel.OrderTotalModel.TotalPrice.ToString("0.00");
//                case "PDListingExpiryDate":
//                    return mPdListingWizardModel.ListingEndDate.ToString("dd MMM yyyy");
//            }
//            return null;
//        }
//    }
//}