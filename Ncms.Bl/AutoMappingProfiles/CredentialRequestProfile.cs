using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.Test;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class CredentialRequestProfile : Profile
    {
        public CredentialRequestProfile()
        {
            CreateMap<DowngradedCredentialRequestDto, DowngradedCredentialRequestModel>()
                .ForMember(x => x.HasCredential,  y => y.Ignore());

            CreateMap<CredentialRequestFieldDataDto, CredentialRequestFieldModel>()
                .ForMember(x => x.FieldDataId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.DisplayNone, y => y.Ignore())
                .ForMember(x => x.FieldOptionId, y => y.Ignore())
                .ForMember(x => x.FieldEnable, y => y.Ignore());

            CreateMap<CredentialRequestTestSessionDto, CredentialRequestTestSessionModel>();

            CreateMap<CredentialRequestTestSessionModel, CredentialRequestTestSessionDto>()
                .ForMember(x => x.VenueName, y => y.Ignore());

            CreateMap<CredentialRequestFieldModel, CredentialFieldData>();

            CreateMap<CandidateBriefModel, CandidateBriefDto>();

            CreateMap<CredentialRequestModel, CredentialRequestData>();

            CreateMap<CredentialRequestSummarySearchRequest, GetCredentialRequestSummarySearchRequest>()
                .ForMember(x => x.Filters, y => y.Ignore());//source string needs to be a list of objects

            CreateMap<CredentialRequestBulkActionRequest, CredentialRequestApplicantsRequest>()
                .ForMember(x => x.SkillIds, y => y.Ignore());//set elsewhere

            CreateMap<CredentialRequestApplicantDto, CredentialRequestApplicantModel>();

            CreateMap<CredentialRequestModel, AutoCreateCredentialRequestNonWizardModel>()
                .ForMember(x => x.ActionType, y => y.Ignore())
                .ForMember(x => x.ApplicationId, y => y.Ignore())
                .ForMember(x => x.CredentialRequestId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.SupplementaryTestComponents, y => y.Ignore())
                .ForMember(x => x.CredentialPreviewFiles, y => y.Ignore())
                .ForMember(x => x.TestSessionId, y => y.Ignore())
                .ForMember(x => x.TestMaterialAssignments, y => y.Ignore())
                .ForMember(x => x.Data, y => y.Ignore())
                .ForMember(x => x.Steps, y => y.Ignore())
                .ForMember(x => x.TransactionId, y => y.Ignore())
                .ForMember(x => x.OrderNumber, y => y.Ignore())
                .ForMember(x => x.PaymentReference, y => y.Ignore())
                .ForMember(x => x.PaymentAmount, y => y.Ignore())
                .ForMember(x => x.TestSittingId, y => y.Ignore());

            CreateMap<RollbackIssueCredentialModel, RollbackIssueCredentialRequest>();

            CreateMap<MoveCredentialModel, MoveCredentialRequest>()
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<TestSessionRequest, CredentialRequestApplicantsRequest>()
                .ForMember(x => x.CredentialApplicationTypeId, y => y.MapFrom(z => z.ApplicationTypeId))
                .ForMember(x => x.CredentialRequestStatusTypeId, y => y.Ignore());
        }
    }
}
