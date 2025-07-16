using System.Linq;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class BasePurchaseWizardModel : BaseWizardModel
    {
        private const string PaymentDetailsStepName = "PaymentDetails";
        private const string DeclarationStepName = "Declaration";

        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string ReferenceNumber { get; set; }
        public int OrderIdForDB { get; set; }

        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string PersonTitle { get; set; }

        public DeliveryDetailsEditModel DeliveryDetailsModel { get; set; }
        public PaymentMethodEditModel PaymentMethodModel { get; set; }
        public PaymentDetailsEditModel PaymentDetailsModel { get; set; }
        public OrderTotalViewModel OrderTotalModel { get; set; }
        public DeclarationEditModel CertificateDeclarationModel { get; set; }
        public DeclarationEditModel IdDeclarationEditModel { get; set; }

        public bool IsPaymentSuccessful { get; set; }

        public bool IsProcessed { get; set; }

        public BasePurchaseWizardModel()
        {
            
        }

        public BasePurchaseWizardModel(ILookupProvider lookupProvider, int orderId)
        {
            DeliveryDetailsModel = new DeliveryDetailsEditModel();
            PaymentMethodModel = new PaymentMethodEditModel();
            PaymentDetailsModel = new PaymentDetailsEditModel();
            ReferenceNumber = orderId.ToString();
            OrderIdForDB = orderId;

            CertificateDeclarationModel = new DeclarationEditModel { UserAgrees = false };
            IdDeclarationEditModel = new DeclarationEditModel {UserAgrees = false};
        }

        public void UpdatePaymentMethod(BasePurchaseWizardModel model)
        {
            if (model.PaymentMethodModel != null)
            {
                this.PaymentMethodModel.PaymentMethodId = model.PaymentMethodModel.PaymentMethodId;

                if (this.PaymentMethodModel.IsOnlineOrder)
                    UpdateStepsForOnlineOrder();
                else
                    UpdateStepsForOfflineOrder();
            }
        }

        public void UpdatePaymentDetails(BasePurchaseWizardModel model)
        {
            if (model.PaymentDetailsModel != null)
            {
                this.PaymentDetailsModel.Type = model.PaymentDetailsModel.Type;
                this.PaymentDetailsModel.NameOnCard = model.PaymentDetailsModel.NameOnCard;
                this.PaymentDetailsModel.CardNumber = model.PaymentDetailsModel.CardNumber;
                this.PaymentDetailsModel.CVV = model.PaymentDetailsModel.CVV;
                this.PaymentDetailsModel.ExpiryMonth = model.PaymentDetailsModel.ExpiryMonth;
                this.PaymentDetailsModel.ExpiryYear = model.PaymentDetailsModel.ExpiryYear;
                this.PaymentDetailsModel.ExpiryMonths = model.PaymentDetailsModel.ExpiryMonths;
                this.PaymentDetailsModel.ExpiryYears = model.PaymentDetailsModel.ExpiryYears;
            }

        }

        protected virtual void UpdateStepsForOfflineOrder()
        {
            var paymentDetailsStep = Steps.Single(s => s.Action.Equals(PaymentDetailsStepName));
            paymentDetailsStep.Skipped = true;
            paymentDetailsStep.HideInStepList = true;
        }

        protected virtual void UpdateStepsForOnlineOrder()
        {
            var paymentDetailsStep = Steps.Single(s => s.Action.Equals(PaymentDetailsStepName));
            paymentDetailsStep.Skipped = false;
            paymentDetailsStep.HideInStepList = false;
            
        }
    }
}
