using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class WiiseProfile : Profile
    {
        public WiiseProfile()
        {
            CreateMap<LineItem, LineItem>()
                .ForMember(x => x.ItemCode, y => y.Ignore())
                .ForMember(x => x.TaxType, y => y.Ignore())
                .ForMember(x => x.TaxAmount, y => y.Ignore())
                .ForMember(x => x.LineAmount, y => y.Ignore())
                .ForMember(x => x.DiscountRate, y => y.Ignore())
                .ForMember(x => x.LineItemID, y => y.Ignore())
                .ForMember(x => x.DiscountAmount, y => y.Ignore())
                .ForMember(x => x.RepeatingInvoiceID, y => y.Ignore());

            CreateMap<Account, BankAccountDto>();


            CreateMap<BrandingTheme, InvoiceBrandingThemeDto>();

            CreateMap<CreatePaymentModel, WiiseCreatePaymentModel>();

            CreateMap<CreatePaymentRequest, WiiseCreatePaymentRequest>()
                .ForMember(x => x.PrerequisiteRequestId, y => y.Ignore());
        }
    }
}
