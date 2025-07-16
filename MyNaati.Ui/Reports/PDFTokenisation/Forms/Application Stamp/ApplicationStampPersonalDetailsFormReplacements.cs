//using MyNaati.Ui.ViewModels.Shared;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.Application_Stamp
//{
//    public class ApplicationStampPersonalDetailsFormReplacements : ITokenReplacement
//    {
//        private BasePurchaseWizardModel mStampBasePurchaseWizardModel;
//        private DeliveryDetailsEditModel mStampDeliveryDetailsEditModel;
//        private OrderTotalViewModel mStampOrderTotalViewModel;

//        public ApplicationStampPersonalDetailsFormReplacements(BasePurchaseWizardModel basePurchaseWizardModel,
//            DeliveryDetailsEditModel deliveryDetailsEditModel,
//            OrderTotalViewModel orderTotalViewModel)
//        {
//            mStampBasePurchaseWizardModel = basePurchaseWizardModel;
//            mStampDeliveryDetailsEditModel = deliveryDetailsEditModel;
//            mStampOrderTotalViewModel = orderTotalViewModel;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "StampOrderReferenceNumber":
//                    return string.Format("{0} {1}", "Order Reference Number:", mStampBasePurchaseWizardModel.ReferenceNumber);
//                case "StampPurchaserName":
//                    return mStampBasePurchaseWizardModel.Name;
//                case "StampNAATINumber":
//                    return mStampBasePurchaseWizardModel.NaatiNumber.ToString();
//                case "StampDeliveryName":
//                    return mStampDeliveryDetailsEditModel.Name;
//                case "StampDeliveryAddress":
//                    if (mStampDeliveryDetailsEditModel.SelectedAddress.IsFromAustralia)
//                    {
//                        return string.Format("{0} \n{1}", mStampDeliveryDetailsEditModel.SelectedAddress.Address.Trim(),
//                                             mStampDeliveryDetailsEditModel.SelectedAddress.Suburb.DisplayText.Trim());
//                    }
//                    else
//                    {
//                        return string.Format("{0} \n{1}", mStampDeliveryDetailsEditModel.SelectedAddress.Address.Trim(),
//                                             mStampDeliveryDetailsEditModel.SelectedAddress.Country.Trim());
//                    }
//                case "StampTotalQty":
//                    return mStampOrderTotalViewModel.TotalQuantity.ToString();
//                case "StampTotalFee":
//                    return string.Format("{0:C}", mStampOrderTotalViewModel.TotalPrice);
//                case "StampTotalAmount":
//                    return mStampOrderTotalViewModel.TotalPrice.ToString("0.00");
//            }
//            return null;
//        }
//    }
//}