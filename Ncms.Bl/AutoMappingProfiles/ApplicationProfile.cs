using AutoMapper;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<ApplicationInvoiceCreateRequestModel, CreateApplicationInvoiceRequest>()
                .ForMember(x => x.Description, y => y.Ignore())
                .ForMember(x => x.UserId, y => y.Ignore());  

            CreateMap<ApplicationRefundCreateRequestModel, CreateApplicationRefundRequest>()
                .ForMember(x => x.Description, y => y.Ignore());

            CreateMap<CreateCreditNoteResponse, RefundCreateResponseModel>()
                .ForMember(x => x.CreditNoteId, y => y.MapFrom(z=> z.Id))
                .ForMember(x => x.CreditNoteNumber, y => y.MapFrom(z => z.Number));

            CreateMap<RefundModel, RefundDto>()
                .ForMember(x => x.OnCreditNoteCreatedSystemActionTypeId, y => y.Ignore())
                .ForMember(x => x.OnPaymentCreatedSystemActionTypeId, y => y.Ignore())
                .ForMember(x => x.ProductCategoryId, y => y.Ignore());

            CreateMap<RefundModel, UpdateCredentialApplicationRefundRequest>();

            CreateMap<ApprovalPendingRefundRequestDto, UpdateCredentialApplicationRefundRequest>();

            CreateMap<RefundDto, RefundModel>();

            CreateMap<ApprovalPendingRefundRequestDto, RefundApprovalModel>()
                .ForMember(x => x.Approved, y => y.Ignore());

            CreateMap<CredentialApplicationDetailedModel, UpsertApplicationRequestModel>()
                .ForMember(x => x.Sections, y => y.Ignore())
                .ForMember(x => x.StandardTestComponents, y => y.MapFrom(z => z.StandardTestComponentModelsToUpdate))
                .ForMember(x => x.RubricTestComponents, y => y.MapFrom(z => z.RubricTestComponentModelsToUpdate))
                .ForMember(x => x.ProcessFee, y => y.Ignore());

            CreateMap<CredentialApplicationEmailMessageModel, CredentialApplicationEmailMessageModel>();

            CreateMap<ApplicationActionWizardModel, ApplicationActionWizardModel>();

            CreateMap<LookupTypeDetailedModel, LookupTypeDetailedModel>().IncludeBase<LookupTypeModel, LookupTypeModel>();
            CreateMap<CredentialApplicationDetailedModel, CredentialApplicationDetailedModel>();

            CreateMap<CredentialRequestDto, CredentialRequestModel>()
                .ForMember(x => x.CredentialId, y => y.Ignore())
                .ForMember(x => x.Actions, y => y.Ignore())
                .ForMember(x => x.WorkPractices, y => y.Ignore())
                .ForMember(x => x.Briefs, y => y.Ignore());

            CreateMap<CredentialWorkflowFeeDto, CredentialWorkflowFeeModel>();

            CreateMap<GetApplicationDetailsResponse, CredentialApplicationDetailedModel>()
                .ForMember(x => x.Notes, y => y.Ignore())
                .ForMember(x => x.PersonNotes, y => y.Ignore())
                .ForMember(x => x.PdActivities, y => y.Ignore())
                .ForMember(x => x.StandardTestComponentModelsToUpdate, y => y.Ignore())
                .ForMember(x => x.RubricTestComponentModelsToUpdate, y => y.Ignore())
                .ForMember(x => x.Recertification, y => y.Ignore());

            CreateMap<Ncms.Contracts.SearchRequest, GetApplicationSearchRequest>()
                .ForMember(x => x.Filters, y => y.Ignore());

            CreateMap<ApplicationSearchDto, ApplicationSearchResultModel>();

            CreateMap<ApplicationExport, GetApplicationSearchRequest>()
                .ForMember(x => x.Filters, y => y.Ignore());//set manually

            CreateMap<CredentialApplicationInfoModel, UpsertCredentialApplicationRequest>()
                .ForMember(x => x.Fields, y => y.Ignore())//set manually
                .ForMember(x => x.CredentialRequests, y => y.Ignore())//set manually
                .ForMember(x => x.Notes, y => y.Ignore())//set manually
                .ForMember(x => x.PersonNotes, y => y.Ignore())//set manually
                .ForMember(x => x.StandardTestComponents, y => y.Ignore())//set manually
                .ForMember(x => x.RubricTestComponents, y => y.Ignore())//set manually
                .ForMember(x => x.PdActivities, y => y.Ignore())//set manually
                .ForMember(x => x.Recertification, y => y.Ignore())//set manually
                .ForMember(x => x.ProcessedFees, y => y.Ignore()) //set manually
                .ForMember(x => x.Attachments, y => y.Ignore()); //set mannually

            CreateMap<ApplicationFieldData, ApplicationFieldData>();

            CreateMap<CredentialApplicationTypeDocumentTypeDto, CredentialApplicationTypeDocumentTypeModel>();

            CreateMap<CredentialApplicationFieldDto, CredentialApplicationFieldModel>()
                .ForMember(x => x.FieldDataId, y => y.Ignore())
                .ForMember(x => x.Value, y => y.Ignore())
                .ForMember(x => x.DisplayNone, y => y.Ignore())
                .ForMember(x => x.FieldOptionId, y => y.Ignore())
                .ForMember(x => x.FieldEnable, y => y.Ignore());

            CreateMap<CredentialApplicationTypeDto, CredentialApplicationTypeModel>();

            CreateMap<CredentialApplicationDto, CredentialApplicationInfoModel>()
                .ForMember(x => x.CredentialApplicationTypeCategoryId, y => y.MapFrom(z => (int)z.ApplicationTypeCategory)); //assuming the enum matches the ids

            CreateMap<CredentialApplicationFieldDataDto, CredentialApplicationFieldModel>()
                .ForMember(x => x.DisplayNone, y => y.Ignore())
                .ForMember(x => x.FieldEnable, y => y.Ignore());
            
            CreateMap<CredentialApplicationFieldOptionDto, CredentialApplicationFieldOptionModel>();

            CreateMap<CandidateBriefFileInfoDto, CandidateBriefFileInfoModel>();

            CreateMap<ApplicationBriefDto, ApplicationBriefModel>();

            CreateMap<CredentialApplicationAttachmentDalModel, CreateOrReplaceApplicationAttachmentRequest>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore());
        }
    }
}
