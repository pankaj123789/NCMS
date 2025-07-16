using System;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using PaymentDto = MyNaati.Contracts.BackOffice.PaymentDto;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class AccountingProfile : Profile
    {
        public AccountingProfile()
        {
            CreateMap<AccountingEndOfPeriodRequest, EndOfPeriodRequest>();

            CreateMap<GetAccountingInvoiceRequestContract, GetInvoicesRequest>()
                .IncludeBase<AccountingEndOfPeriodRequest, EndOfPeriodRequest>()
                .ForMember(x => x.ExcludeCreditNotes, y => y.Ignore())
                .ForMember(x => x.IncludeVoidedStatus, y => y.Ignore());

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.DTO.PaymentDto, PaymentDto>();

            CreateMap<InvoiceDto, AccountingInvoiceDto>()
                .ForMember(x => x.GST, y => y.Ignore())
                .ForMember(x => x.Code, y => y.Ignore());

            CreateMap<PaymentCreateRequestModel, CreatePaymentModel>()
                .ForMember(x => x.Date, y => y.MapFrom(z => z.DatePaid)) //not 100% sure
                .ForMember(x => x.EftMachine, y => y.Ignore())
                .ForMember(x => x.BSB, y => y.Ignore())
                .ForMember(x => x.ChequeNumber, y => y.Ignore())
                .ForMember(x => x.ChequeBankName, y => y.Ignore())
                .ForMember(x => x.CreditNoteNumber, y => y.Ignore())
                .ForMember(x => x.OrderNumber, y => y.Ignore());


        CreateMap<CreatePaymentResponse, PaymentCreateResponseModel>()
                .ForMember(x => x.InvoiceId, y => y.Ignore())
                .ForMember(x => x.UnHandledExceptionMessage, y => y.Ignore())
                .ForMember(x => x.UnHandledException, y => y.Ignore());
        }
    }
}
