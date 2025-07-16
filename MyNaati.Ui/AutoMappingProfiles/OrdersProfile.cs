using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.Reports.OnlineOrderDetails;
using MyNaati.Ui.ViewModels.Bills;
using MyNaati.Ui.ViewModels.Shared;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {

           //CreateMap<Order, OrderCsv>()
           //     .ForMember(x => x.OrderId, opt => opt.MapFrom(f => f.Id))
           //     .ForMember(x => x.Product, opt => opt.Ignore())
           //     .ForMember(x => x.Level, opt => opt.Ignore())
           //     .ForMember(x => x.Direction, opt => opt.Ignore())
           //     .ForMember(x => x.Quantity, opt => opt.Ignore())
           //     .ForMember(x => x.Skill, opt => opt.Ignore())
           //     .ForMember(x => x.Country, opt => opt.MapFrom(m => GetLookupForNonZeroId(ServiceLocator.Resolve<ILookupProvider>().Countries, m.CountryId)))
           //     .ForMember(x => x.Suburb, opt => opt.MapFrom(m => GetLookupForNonZeroId(ServiceLocator.Resolve<ILookupProvider>().Postcodes, m.SuburbId)));

           //CreateMap<BasePurchaseWizardModel, Order>()
           //     .ForMember(x => x.Id, opt => opt.Ignore())
           //     .ForMember(x => x.CountryId, opt => opt.MapFrom(f => f.DeliveryDetailsModel.SelectedAddress.CountryId))
           //     .ForMember(x => x.DeliveryAddress, opt => opt.MapFrom(f => f.DeliveryDetailsModel.SelectedAddress.Address))
           //     .ForMember(x => x.DeliveryName, opt => opt.MapFrom(f => f.DeliveryDetailsModel.Name))
           //     .ForMember(x => x.NAATINumber, opt => opt.MapFrom(f => f.NaatiNumber))
           //     .ForMember(x => x.OrderDate, opt => opt.MapFrom(x => DateTime.Now))
           //     .ForMember(x => x.OrderItems, opt => opt.MapFrom(f => f.OrderTotalModel.Items))
           //     .ForMember(x => x.SuburbId, opt => opt.MapFrom(f => f.DeliveryDetailsModel.SelectedAddress.Suburb == null ? (int?)null : f.DeliveryDetailsModel.SelectedAddress.Suburb.SuburbId))
           //     .ForMember(x => x.Total, opt => opt.MapFrom(f => f.OrderTotalModel.TotalPrice))
           //     .ForMember(x => x.ExternalPaymentTransactionID, m => m.Ignore());

           //CreateMap<PaymentDetailsEditModel, EnteredCardDetails>()
           //     .ForMember(x => x.CardVerificationValue, opt => opt.MapFrom(f => f.CVV))
           //     .ForMember(x => x.ExpiryMonth, opt => opt.MapFrom(f => new DateTime(f.ExpiryYear, f.ExpiryMonth, 1)));

           //CreateMap<PaymentDetailsModel, EnteredCardDetails>()
           //     .ForMember(x => x.CardVerificationValue, opt => opt.MapFrom(f => f.CVV))
           //     .ForMember(x => x.ExpiryMonth, opt => opt.MapFrom(f => new DateTime(f.ExpiryYear, f.ExpiryMonth, 1)));

           //CreateMap<ProductOrderItem, OrderItem>()
           //     .ForMember(x => x.Id, opt => opt.Ignore())
           //     .ForMember(x => x.OrderItemType, opt => opt.MapFrom(f => f.Product))
           //     .ForMember(x => x.Price, opt => opt.MapFrom(f => f.IsAustraliaPricing ? f.AustraliaPrice : f.OverseasPrice))
           //     .ForMember(x => x.Total, opt => opt.MapFrom(f => f.TotalPrice))
           //     .ForMember(x => x.Order, opt => opt.Ignore())
           //     .ForMember(x => x.Expiry, opt => opt.NullSubstitute(null))
           //     //These will be set in other code
           //     .ForMember(x => x.ProductSpecificationId, opt => opt.Ignore())
           //     .ForMember(x => x.GSTApplies, opt => opt.Ignore())
           //     .ForMember(x => x.AccreditationResultId, opt => opt.Ignore());

           //CreateMap<string, OrderItemType>().ConvertUsing(x => ConvertToOrderItemType(x));
        }

        private string GetLookupForNonZeroId<T>(IEnumerable<T> lookupList, int? id)
            where T : LookupItemBase
        {
            if (id == null || id == 0)
                return string.Empty;

            if (typeof(T) == typeof(Postcode))
            {
                Postcode postcode = ((IEnumerable<Postcode>)lookupList).FirstOrDefault(l => l.SuburbId == id);
                return postcode != null ? postcode.DisplayText : string.Empty;
            }

            return lookupList.Single(l => l.SamId == id).DisplayText;
        }

        private OrderItemType ConvertToOrderItemType(string source)
        {
            if (source.Equals("laminated certificate", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.LaminatedCertificate;
            if (source.Equals("unlaminated certificate", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.UnlaminatedCertificate;
            if (source.Equals("rubber stamp", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.RubberStamp;
            if (source.Equals("self-inking stamp", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.SelfInkingStamp;
            if (source.Equals("accreditation & recognition id card", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.SingleIdCard;
            if (source.Equals("accreditation id card", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.AccreditationIdCard;
            if (source.Equals("recognition id card", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.RecognitionIdCard;
            if (source.Equals("practitioners directory registration", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.PDListingRegistration;
            if (source.Equals("practitioners directory renewal", StringComparison.InvariantCultureIgnoreCase))
                return OrderItemType.PDListingRenewal;
            else
                throw new Exception("OrderItemTypeConverter could not map bad product type string: " + source);
        }
    }    
}