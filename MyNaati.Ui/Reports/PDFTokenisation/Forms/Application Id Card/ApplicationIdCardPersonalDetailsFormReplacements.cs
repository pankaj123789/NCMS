//using MyNaati.Ui.ViewModels.Shared;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.Application_Id_Card
//{
//    public class ApplicationIdCardPersonalDetailsFormReplacements : ITokenReplacement
//    {
//        private BasePurchaseWizardModel mIdCardBasePurchaseWizardModel;

//        public ApplicationIdCardPersonalDetailsFormReplacements(BasePurchaseWizardModel basePurchaseWizardModel)
//        {
//            mIdCardBasePurchaseWizardModel = basePurchaseWizardModel;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "OrderReferenceNumber":
//                    return string.Format("{0} {1}","Order Reference Number:", mIdCardBasePurchaseWizardModel.ReferenceNumber);
//                case "PurchaserName":
//                    return mIdCardBasePurchaseWizardModel.Name;
//                case "NAATINumber":
//                    return mIdCardBasePurchaseWizardModel.NaatiNumber.ToString();
//                case "DeliveryName":
//                    return mIdCardBasePurchaseWizardModel.DeliveryDetailsModel.Name;
//                case "DeliveryAddress":
//                    if(mIdCardBasePurchaseWizardModel.DeliveryDetailsModel.SelectedAddress.IsFromAustralia)
//                    {
//                        return string.Format("{0} \n{1}", mIdCardBasePurchaseWizardModel.DeliveryDetailsModel.SelectedAddress.Address.Trim(),
//                                             mIdCardBasePurchaseWizardModel.DeliveryDetailsModel.SelectedAddress.Suburb.DisplayText.Trim());
//                    }
//                    else
//                    {
//                        return string.Format("{0} \n{1}", mIdCardBasePurchaseWizardModel.DeliveryDetailsModel.SelectedAddress.Address.Trim(),
//                                             mIdCardBasePurchaseWizardModel.DeliveryDetailsModel.SelectedAddress.Country.Trim());
//                    }
//                case "TotalQty":
//                    return mIdCardBasePurchaseWizardModel.OrderTotalModel.TotalQuantity.ToString();
//                case "TotalFee":                    
//                    return string.Format("{0:C}", mIdCardBasePurchaseWizardModel.OrderTotalModel.TotalPrice);
//                case "TotalAmount":
//                    return  mIdCardBasePurchaseWizardModel.OrderTotalModel.TotalPrice.ToString("0.00");
//            }
//            return null;
//        }
//    }
//}