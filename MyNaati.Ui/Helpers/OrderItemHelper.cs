using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using F1Solutions.Naati.Common.Dal.Portal;
using MyNaati.Contracts.BackOffice.AccreditationResults;

namespace MyNaati.Ui.Helpers
{
    public class OrderItemHelper
    {
        /// <summary>
        /// This method adds information to the orders that is needed by logic but not passed around by the UI.
        /// It makes some assumptions to do so, hence the name.
        /// </summary>
        /// <remarks>
        /// It'd be great if we could change the UI so this stuff isn't needed and can simply be mapped based on what 
        /// the UI is holding, but that'd most likely necessitate storing something other than view models in Session.
        /// </remarks>
        /// <param name="translator"></param>
        /// <param name="items"></param>
        /// <param name="isBoughtFromAustralia"></param>
        public static void CleanUpOrderItemsBasedOnAssumptions(ISystemValuesTranslator translator, IEnumerable<OrderItem> items, bool isBoughtFromAustralia, Credential[] credentials)
        {            
            foreach (var item in items)
            {
                //Assume that GST applies if and only if it's bought from Australia
                item.GSTApplies = isBoughtFromAustralia;
                //Assume that the product ID hasn't changed recently
                item.ProductSpecificationId = GetIdForOrderItemType(item.OrderItemType, isBoughtFromAustralia, translator);

                var orderTypesWithAccredResults = new[] { 
                    OrderItemType.LaminatedCertificate, 
                    OrderItemType.UnlaminatedCertificate, 
                    OrderItemType.RubberStamp, 
                    OrderItemType.SelfInkingStamp };

                if (orderTypesWithAccredResults.Contains(item.OrderItemType))
                {
                    // Derive the AccreditationResultId:
                    item.AccreditationResultId = GetAccreditationResultIdForOrderItem(item, credentials);
                }
            }
        }

        private static int GetIdForOrderItemType(OrderItemType type, bool isBoughtFromAustralia, ISystemValuesTranslator translator)
        {
            switch (type)
            {
                case OrderItemType.SingleIdCard:
                case OrderItemType.AccreditationIdCard:
                case OrderItemType.RecognitionIdCard:
                    return (isBoughtFromAustralia ? translator.IdCardFeeProductAustraliaId : translator.IdCardFeeProductOverseasId);
                case OrderItemType.LaminatedCertificate:
                case OrderItemType.UnlaminatedCertificate:
                    return (isBoughtFromAustralia ? translator.CertificateFeeProductAustraliaId : translator.CertificateFeeProductOverseasId);
                case OrderItemType.SelfInkingStamp:
                    return (isBoughtFromAustralia ? translator.StampFeeProductSelfInkingAustraliaId : translator.StampFeeProductSelfInkingOverseasId);
                case OrderItemType.RubberStamp:
                    return (isBoughtFromAustralia ? translator.StampFeeProductRubberAustraliaId : translator.StampFeeProductRubberOverseasId);
                case OrderItemType.PDListingRenewal:
                case OrderItemType.PDListingRegistration:
                    return (isBoughtFromAustralia ? translator.PDListingProductId : translator.PDListingOverseasProductId);
                default:
                    throw new Exception(string.Format("Unrecognised OrderItemType '{0}'", type));
            }
        }

        private static int GetAccreditationResultIdForOrderItem(OrderItem item, Credential[] credentials)
        {
            var credentialMatch = credentials.First(c => Credential.GetDirection(c).Equals(item.Direction) && c.Level == item.Level && c.Skill == item.Skill);
            return credentialMatch.AccreditationResultId;
        }
    }
}