using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models.MaterialRequest;
using SearchRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.SearchRequest;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class TestMaterialRequestProfile : Profile
    {
        public TestMaterialRequestProfile()
        {
            CreateMap<MaterialRequestDocumentInfoDto, MaterialRequestDocumentInfoModel>();

            CreateMap<MaterialRequestRoundDto, MaterialRequestRoundModel>();

            CreateMap<MaterialRequestPersonNoteDto, MaterialRequestPersonNoteModel>();

            CreateMap<MaterialRequestActionNoteDto, MaterialRequestActionNoteModel>();

            CreateMap<MaterialRequestActionPublicNoteDto, MaterialRequestActionPublicNoteModel>();

            CreateMap<OutputTestMaterialDocumentInfoDto, OutputTestMaterialDocumentInfoModel>();

            CreateMap<MaterialRequestActionDto, MaterialRequestActionModel>();

            CreateMap<MaterialRequestTaskModel, MaterialRequestTaskDto>()
                .ForMember(x => x.MaterialRequestTaskTypeDisplayName, y => y.Ignore());

            CreateMap<MaterialRequestPayrollModel, MaterialRequestPayrollDto>();

            CreateMap<MaterialRequestPanelMembershipModel, MaterialRequestPanelMembershipDto>();

            CreateMap<MaterialRequestInfoModel, MaterialRequestInfoDto>()
                .ForMember(x => x.StatusTypeDisplayName, y => y.Ignore())
                .ForMember(x => x.RequestTypeDisplayName, y => y.Ignore())
                .ForMember(x => x.EnteredOfficeId, y => y.Ignore())
                .ForMember(x => x.LastRound, y => y.Ignore())
                .ForMember(x => x.TestMaterialDomainId, y => y.Ignore());

            CreateMap<MaterialRequestPersonNoteModel, MaterialRequestPersonNoteDto>();

            CreateMap<MaterialRequestActionNoteModel, MaterialRequestActionNoteDto>();

            CreateMap<MaterialRequestActionPublicNoteModel, MaterialRequestActionPublicNoteDto>();

            CreateMap<OutputTestMaterialDocumentInfoModel, OutputTestMaterialDocumentInfoDto>()
                .ForMember(x => x.PersonId, y => y.Ignore());

            CreateMap<MaterialRequestActionModel, MaterialRequestActionDto>();

            CreateMap<UpsertMaterialRequestResultResponse, UpsertMaterialRequestResultModel>();

            CreateMap<Contracts.SearchRequest, TestMaterialRequestSearchRequest>()
                .ForMember(x => x.Skip, y => y.Ignore())
                .ForMember(x => x.Take, y => y.Ignore())
                .ForMember(x => x.Filters, y => y.Ignore());

            CreateMap<TestMaterialRequestSearchDto, TestMaterialRequestSearchResultModel>()
                .ForMember(x => x.RelationType, y => y.Ignore());

            CreateMap<MaterialRequestTaskDto, MaterialRequestTaskModel>();

            CreateMap<MaterialRequestPayrollDto, MaterialRequestPayrollModel>();

            CreateMap<MaterialRequestPanelMembershipDto, MaterialRequestPanelMembershipModel>();

            CreateMap<MaterialRequestInfoDto, MaterialRequestInfoModel>();

            CreateMap<MaterialRequestDocumentInfoModel, MaterialRequestDocumentInfoDto>()
                .ForMember(x => x.PersonId, y => y.Ignore());

            CreateMap<MaterialRequestRoundLinkModel, MaterialRequestRoundLinkDto>()
                .ForMember(x => x.PersonNaatiNumber, y => y.Ignore());

            CreateMap<MaterialRequestRoundModel, MaterialRequestRoundDto>()
                .ForMember(x => x.StatusTypeDisplayName, y => y.Ignore());

            CreateMap<MaterialRequestRoundLinkDto, MaterialRequestRoundLinkModel>();

            CreateMap<MaterialRequestRoundLinkModel, SaveMaterialRequestRoundLinkRequest>()
                .ForMember(x => x.MaterialRequestRoundId, y => y.Ignore())
                .ForMember(x => x.NaatiNumber, y => y.Ignore());

            CreateMap<MaterialRequestRoundAttachmentDto, MaterialRequestRoundAttachmentModel>()
                .ForMember(x => x.Title, y => y.Ignore());

            CreateMap<MaterialRequestRoundAttachmentModel, CreateOrReplaceMaterialRequestRoundAttachmentRequest>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore())
                .ForMember(x => x.NAATINumber, y => y.Ignore());
        }
    }
}
