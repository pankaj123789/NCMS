using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.Reports.PDFTokenisation.Forms.Application_Certificate
{
    //public class ApplicationCertificatePersonalDetailsFormReplacements : ITokenReplacement
    //{
    //    private BasePurchaseWizardModel mCertificateBasePurchaseWizardModel;
    //    private DeliveryDetailsEditModel mDeliveryDetailsEditModel;
    //    private OrderTotalViewModel mOrderTotalViewModel;

    //    public ApplicationCertificatePersonalDetailsFormReplacements(BasePurchaseWizardModel basePurchaseWizardModel, 
    //        DeliveryDetailsEditModel deliveryDetailsEditModel,
    //        OrderTotalViewModel orderTotalViewModel)
    //    {
    //        mCertificateBasePurchaseWizardModel = basePurchaseWizardModel;
    //        mDeliveryDetailsEditModel = deliveryDetailsEditModel;
    //        mOrderTotalViewModel = orderTotalViewModel;
    //    }

    //    public string GetReplacement(string title)
    //    {
    //        switch (title)
    //        {
    //            case "CertificateOrderReferenceNumber":
    //                return string.Format("{0} {1}", "Order Reference Number:", mCertificateBasePurchaseWizardModel.ReferenceNumber);
    //            case "CertifiatePurchaserName":
    //                return mCertificateBasePurchaseWizardModel.Name;
    //            case "CertificateNAATINumber":
    //                return mCertificateBasePurchaseWizardModel.NaatiNumber.ToString();
    //            case "CertificateDeliveryName":
    //                return mDeliveryDetailsEditModel.Name;
    //            case "CertificateDeliveryAddress":
    //                if (mDeliveryDetailsEditModel.SelectedAddress.IsFromAustralia)
    //                {
    //                    return string.Format("{0} \n{1}", mDeliveryDetailsEditModel.SelectedAddress.Address.Trim(),
    //                                         mDeliveryDetailsEditModel.SelectedAddress.Suburb.DisplayText.Trim());
    //                }
    //                else
    //                {
    //                    return string.Format("{0} \n{1}", mDeliveryDetailsEditModel.SelectedAddress.Address.Trim(),
    //                                         mDeliveryDetailsEditModel.SelectedAddress.Country.Trim());
    //                }
    //            case "CertificateTotalQty":
    //                return mOrderTotalViewModel.TotalQuantity.ToString();
    //            case "CertificateTotalFee":
    //                return string.Format("{0:C}", mOrderTotalViewModel.TotalPrice);
    //            case "CertificateTotalAmount":
    //                return mOrderTotalViewModel.TotalPrice.ToString("0.00");
    //        }
    //        return null;
    //    }
    //}
}