using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.ViewModels.Certificate
{
    public class WizardModel : BasePurchaseWizardModel
    {
        private const string DeclarationStepName = "Declaration";

        public SelectCertificateViewModel SelectCertificateModel { get; set; }        

        public WizardModel()
        { }

        public WizardModel(ILookupProvider lookupProvider, int orderId)
            : base(lookupProvider, orderId)
        {
            Steps = new List<WizardStep>()
            {
                new WizardStep() { Name = "Certificates", Action = "Select", AllowDirectNavigation = true },
                new WizardStep() { Name = "Delivery Details", Action = "DeliveryDetails" },
                new WizardStep() { Name = "Payment Method", Action = "PaymentMethod" },
                new WizardStep() { Name = "Payment Details", Action = "PaymentDetails" },
                new WizardStep() { Name = "Declaration", Action = "Declaration"},
                new WizardStep() { Name = "Review", Action = "Review" }
            };
        }

        public void UpdateOrderTotalModel(bool isAustraliaPricing, decimal australiaPrice, decimal overseasPrice)
        {
            if (OrderTotalModel == null)
                OrderTotalModel = new OrderTotalViewModel();
            else
                OrderTotalModel.Items.Clear();

            foreach (var certificate in SelectCertificateModel.Certificates)
            {
                if (certificate.QuantityLaminated > 0)
                {
                    var order = new ProductOrderItem
                    {
                        Product = ProductType.LaminatedCertificate,
                        Direction = certificate.Direction,
                        Expiry = certificate.Expiry,
                        Skill = certificate.Skill,
                        Level = certificate.Level,
                        Quantity = certificate.QuantityLaminated,
                        AustraliaPrice = australiaPrice,
                        OverseasPrice = overseasPrice,
                        IsAustraliaPricing = isAustraliaPricing
                    };

                    OrderTotalModel.Items.Add(order);
                }

                if (certificate.QuantityUnlaminated > 0)
                {
                    var order = new ProductOrderItem
                    {
                        Product = ProductType.UnlaminatedCertificate,
                        Direction = certificate.Direction,
                        Expiry = certificate.Expiry,
                        Skill = certificate.Skill,
                        Level = certificate.Level,
                        Quantity = certificate.QuantityUnlaminated,
                        AustraliaPrice = australiaPrice,
                        OverseasPrice = overseasPrice,
                        IsAustraliaPricing = isAustraliaPricing
                    };

                    OrderTotalModel.Items.Add(order);
                }
            }

            OrderTotalModel.TotalPrice = OrderTotalModel.Items.Sum(i => i.TotalPrice);


        }

        public void CopySelectionQuantitiesFrom(SelectCertificateViewModel source)
        {
            int certNumber = 0;
            SelectCertificateModel.TotalLaminated = 0;
            SelectCertificateModel.TotalUnlaminated = 0;

            foreach (var cert in source.Certificates)
            {
                SelectCertificateModel.Certificates[certNumber].QuantityLaminated = cert.QuantityLaminated;
                SelectCertificateModel.Certificates[certNumber].QuantityUnlaminated = cert.QuantityUnlaminated;

                SelectCertificateModel.TotalLaminated += cert.QuantityLaminated;
                SelectCertificateModel.TotalUnlaminated += cert.QuantityUnlaminated;

                certNumber++;
            }
        }

        protected override void UpdateStepsForOfflineOrder()
        {
            if(Steps.Any(s => s.Action.Equals(DeclarationStepName)))
            {
                Steps.Remove(Steps.Single(s => s.Action.Equals(DeclarationStepName)));
            }
            base.UpdateStepsForOfflineOrder();
        }

        protected override void UpdateStepsForOnlineOrder()
        {
            if (!Steps.Contains(Steps.SingleOrDefault(s => s.Action.Equals(DeclarationStepName))))
            {
                var paymentMethodIndex = Steps.IndexOf(Steps.Single(s => s.Action.Equals("PaymentMethod")));
                Steps.Insert(2 + paymentMethodIndex, new WizardStep { Action = DeclarationStepName, AllowDirectNavigation = true, Name = DeclarationStepName });
            }

            base.UpdateStepsForOnlineOrder();
        }
    }
}