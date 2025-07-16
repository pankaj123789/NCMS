using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class CredentialWorkflowProfile : Profile
    {
        public CredentialWorkflowProfile()
        {
            CreateMap<RefundPolicyParameter, RefundPolicyParameterData>();
            CreateMap<CredentialApplicationRefundPolicy, CredentialApplicationRefundPolicyData>();

            CreateMap<CredentialWorkflowFee, CredentialWorkflowFeeDto>()
                .ForMember(x => x.OnPaymentActionType, y => y.MapFrom(z => Util.GetValueOrNull(z.OnPaymentCreatedSystemActionType, w => (SystemActionTypeName) w.Id)))
                .ForMember(x => x.OnInvoiceActionType, y => y.MapFrom(z => Util.GetValueOrNull(z.OnInvoiceCreatedSystemActionType, w => (SystemActionTypeName) w.Id)))
                .ForMember(x => x.Invoice, y => y.Ignore());

            CreateMap<EmailTemplate, EmailTemplateDetailDto>()
                .ForMember(x => x.EmailTemplateDetails, y => y.Ignore())
                .ForMember(x => x.SystemActionEventType, y => y.Ignore());
        }
    }
}
