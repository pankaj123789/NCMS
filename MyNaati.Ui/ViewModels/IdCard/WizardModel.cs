using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.ViewModels.IdCard
{
    public class WizardModel : BasePurchaseWizardModel
    {
        public SelectOrderEditModel SelectOrderModel { get; set; }        

        public WizardModel()
        {
            
        }

        public WizardModel(ILookupProvider lookupProvider, int orderId) : base(lookupProvider, orderId)
        {
            Steps = new List<WizardStep>()
            {
                new WizardStep() { Name = "Id Cards", Action = "Select", AllowDirectNavigation = true },
                new WizardStep() { Name = "Photograph", Action = "Photograph" },
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
            {
                OrderTotalModel = new OrderTotalViewModel() { IsIdCard = true };
            }
            else
            {
                OrderTotalModel.Items.Clear();
            }

            if (SelectOrderModel.IsOrderingSingle)
            {
                var item = CreateProductOrderItem(ProductType.SingleIdCard, isAustraliaPricing, australiaPrice, overseasPrice);
                OrderTotalModel.Items.Add(item);
            }

            if (SelectOrderModel.IsOrderingForAccreditations)
            {
                var item = CreateProductOrderItem(ProductType.AccreditationIdCard, isAustraliaPricing, australiaPrice, overseasPrice);
                OrderTotalModel.Items.Add(item);
            }

            if (SelectOrderModel.IsOrderingForRecognitions)
            {
                var item = CreateProductOrderItem(ProductType.RecognitionIdCard, isAustraliaPricing, australiaPrice, overseasPrice);
                OrderTotalModel.Items.Add(item);
            }

            OrderTotalModel.TotalPrice = OrderTotalModel.Items.Sum(e => e.TotalPrice);
        }

        private ProductOrderItem CreateProductOrderItem(string productType, bool isAustraliaPricing, decimal australiaPrice, decimal overseasPrice)
        {
            var item = new ProductOrderItem()
            {
                Product = productType,
                Direction = string.Empty,
                Expiry = null,
                Skill = string.Empty,
                Level = string.Empty,
                Quantity = 1,
                AustraliaPrice = australiaPrice,
                OverseasPrice = overseasPrice,
                IsAustraliaPricing = isAustraliaPricing
            };

            return item;
        }

        protected override void UpdateStepsForOfflineOrder()
        {
            if (Steps.Any(s => s.Action.Equals("Declaration")))
            {
                Steps.Remove(Steps.Single(s => s.Action.Equals("Declaration")));
            }

            base.UpdateStepsForOfflineOrder();
        }

        protected override void UpdateStepsForOnlineOrder()
        {
            if (!Steps.Contains(Steps.SingleOrDefault(s => s.Action.Equals("Declaration"))))
            {
                var paymentMethodIndex = Steps.IndexOf(Steps.Single(s => s.Action.Equals("PaymentMethod")));
                Steps.Insert(2 + paymentMethodIndex, new WizardStep { Action = "Declaration", AllowDirectNavigation = true, Name = "Declaration" });
            }

            base.UpdateStepsForOnlineOrder();
        }
    }
}