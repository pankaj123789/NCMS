using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.ViewModels.Stamp
{
    public class WizardModel : BasePurchaseWizardModel
    {
        public SelectStampViewModel SelectStampModel { get; set; }
        public DeclarationEditModel DeclarationModel { get; set; }

        private const string DeclarationStepName = "Declaration";

        public WizardModel()
        { }

        public WizardModel(ILookupProvider lookupProvider, int orderId)
            : base(lookupProvider, orderId)
        {
            Steps = new List<WizardStep>()
            {
                new WizardStep() { Name = "Translator Stamps", Action = "Select", AllowDirectNavigation = true },
                new WizardStep() { Name = "Delivery Details", Action = "DeliveryDetails" },
                new WizardStep() { Name = "Payment Method", Action = "PaymentMethod" },
                new WizardStep() { Name = "Payment Details", Action = "PaymentDetails" },
                new WizardStep() { Name = "Declaration", Action = DeclarationStepName },
                new WizardStep() { Name = "Review", Action = "Review" }
            };

            DeclarationModel = new DeclarationEditModel();
        }

        public void UpdateOrderTotalModel(bool isAustraliaPricing,
            decimal australiaRubberPrice, decimal overseasRubberPrice, decimal australiaSelfInkingPrice, decimal overseasSelfInkingPrice)
        {
            if (OrderTotalModel == null)
                OrderTotalModel = new OrderTotalViewModel();
            else
                OrderTotalModel.Items.Clear();

            foreach (var stamp in SelectStampModel.StampOrders)
            {
                if (stamp.QuantityRubber > 0)
                {
                    var order = new ProductOrderItem
                    {
                        Product = ProductType.RubberStamp,
                        Direction = stamp.Direction,
                        Expiry = stamp.Expiry,
                        Skill = stamp.Skill,
                        Level = stamp.Level,
                        Quantity = stamp.QuantityRubber,
                        AustraliaPrice = australiaRubberPrice,
                        OverseasPrice = overseasRubberPrice,
                        IsAustraliaPricing = isAustraliaPricing
                    };

                    OrderTotalModel.Items.Add(order);
                }

                if (stamp.QuantitySelfInking > 0)
                {
                    var order = new ProductOrderItem
                    {
                        Product = ProductType.SelfInkingStamp,
                        Direction = stamp.Direction,
                        Expiry = stamp.Expiry,
                        Skill = stamp.Skill,
                        Level = stamp.Level,
                        Quantity = stamp.QuantitySelfInking,
                        AustraliaPrice = australiaSelfInkingPrice,
                        OverseasPrice = overseasSelfInkingPrice,
                        IsAustraliaPricing = isAustraliaPricing
                    };

                    OrderTotalModel.Items.Add(order);
                }                
            }

            OrderTotalModel.TotalPrice = OrderTotalModel.Items.Sum(i => i.TotalPrice);
        }

        public void CopySelectionQuantitiesFrom(SelectStampViewModel source)
        {
            int certNumber = 0;
            SelectStampModel.TotalRubber = 0;
            SelectStampModel.TotalSelfInking = 0;

            foreach (var stamp in source.StampOrders)
            {
                SelectStampModel.StampOrders[certNumber].QuantityRubber = stamp.QuantityRubber;
                SelectStampModel.StampOrders[certNumber].QuantitySelfInking = stamp.QuantitySelfInking;

                SelectStampModel.TotalRubber += stamp.QuantityRubber;
                SelectStampModel.TotalSelfInking += stamp.QuantitySelfInking;

                certNumber++;
            }
        }

        protected override void UpdateStepsForOfflineOrder()
        {
            var declarationStep = Steps.Single(s => s.Action == DeclarationStepName);
            declarationStep.HideInStepList = true;
            declarationStep.Skipped = true;

            base.UpdateStepsForOfflineOrder();
        }

        protected override void UpdateStepsForOnlineOrder()
        {
            var declarationStep = Steps.Single(s => s.Action == DeclarationStepName);
            declarationStep.HideInStepList = false;
            declarationStep.Skipped = false;

            base.UpdateStepsForOnlineOrder();
        }
    }
}