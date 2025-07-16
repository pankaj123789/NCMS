using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models.Accounting;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class AccountingProfile : Profile
    {
        public AccountingProfile()
        {
            CreateMap<InvoiceBrandingThemeDto, InvoiceBrandingThemeModel>();

            CreateMap<BankAccountDto, BankAcountModel>();

            CreateMap<InvoiceRequest, GetInvoicesRequest>()
                .ForMember(x => x.IncludeVoidedStatus, y => y.Ignore());

            CreateMap<InvoiceDto, InvoiceModel>()
                .ForMember(x => x.Theme, y => y.Ignore());

            CreateMap<PaymentDto, PaymentModel>();

            CreateMap<Ncms.Contracts.Models.Accounting.EndOfPeriodRequest, GetPaymentsRequest>();

            CreateMap<InvoicePaymentCreateModel, CreatePaymentModel>()
                .ForMember(x => x.CreditNoteNumber, y => y.Ignore());

            CreateMap<InvoiceLineItemModel, InvoiceLineItemDto>();

            CreateMap<InvoiceCreateRequestModel, CreateInvoiceRequest>()
                .ForMember(x => x.UserId, y => y.Ignore())
                .ForMember(x => x.Description, y => y.Ignore());

            CreateMap<PaymentCreateRequestModel, CreatePaymentModel>()
                .ForMember(x => x.Date, y => y.Ignore())//set manually
                .ForMember(x => x.EftMachine, y => y.MapFrom(z => z.PaymentType == PaymentTypeDto.Eft))//makes sense but not 100% sure
                .ForMember(x => x.BSB, y => y.Ignore())
                .ForMember(x => x.ChequeNumber, y => y.Ignore())
                .ForMember(x => x.ChequeBankName, y => y.Ignore())
                .ForMember(x => x.CreditNoteNumber, y => y.Ignore())
                .ForMember(x => x.OrderNumber, y => y.Ignore());

            CreateMap<QueuedOperationDto, QueuedOperationModel>();

            CreateMap<CreateInvoiceResponse, InvoiceCreateResponseModel>();

            CreateMap<OfficeDto, OfficeModel>();

            CreateMap<EftMachineDto, EftMachineModel>();

            CreateMap<CreatePaymentResponse, PaymentCreateResponseModel>()
                .ForMember(x => x.InvoiceId, y => y.Ignore());
        }
    }
}
