using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;
using SearchRequest = Ncms.Contracts.SearchRequest;
using TestComponentModel = Ncms.Contracts.Models.TestComponentModel;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class TestMaterialProfile : Profile
    {
        public TestMaterialProfile()
        {
            CreateMap<TestSittingMaterialDto, TestSittingMaterialModel>();

            CreateMap<TestSittingMaterialModel, TestSittingMaterialDto>();

            CreateMap<TestMaterialAssignmentModel, TestMaterialAssignmentDto>();

            CreateMap<SearchRequest, TestMaterialSearchRequest>()
                .ForMember(x => x.Filter, y => y.Ignore());

            CreateMap<TestMaterialSearchDto, TestMaterialSearchModel>();

            CreateMap<TestSpecificationDetailsDto, TestSpecificationDetailsModel>();

            CreateMap<SpecificationSkillDto, SpecificationSkillModel>();

            CreateMap<TestMaterialApplicantDto, TestMaterialApplicantModel>();

            CreateMap<PanelDto, TestMaterialPanelModel>();

            CreateMap<PanelMembershipDto, TestMaterialPanelMembershipModel>();

            CreateMap<ExaminerTestMaterialDto, TestMaterialExaminerModel>();

            CreateMap<TestMaterialLinkDto, TestMaterialLinkModel>();

            CreateMap<TestMaterialDto, TestMaterialModel>()
                .ForMember(x => x.TypeId, y => y.MapFrom(z => z.TestComponentTypeId))
                .ForMember(x => x.TypeLabel, y => y.MapFrom(z => z.TestComponentTypeName))
                .ForMember(x => x.StatusId, y => y.Ignore())
                .ForMember(x => x.ApplicantsRangeTypeId, y => y.Ignore())
                .ForMember(x => x.TypeDescription, y => y.Ignore())
                .ForMember(x => x.LastUsedDate, y => y.Ignore());

            CreateMap<TestMaterialLinkModel, TestMaterialLinkDto>();

            CreateMap<TestMaterialModel, TestMaterialDto>()
                .ForMember(x => x.TestComponentTypeId, y => y.MapFrom(z => z.TypeId))
                .ForMember(x => x.TestComponentTypeName, y => y.Ignore())
                .ForMember(x => x.HasFile, y => y.Ignore())
                .ForMember(x => x.Notes, y => y.Ignore())
                .ForMember(x => x.TestSittingTestMaterialId, y => y.Ignore())
                .ForMember(x => x.TestSittingId, y => y.Ignore())
                .ForMember(x => x.TestSpecificationId, y => y.Ignore())
                .ForMember(x => x.TestSpecificationActive, y => y.Ignore())
                .ForMember(x => x.SourceTestMaterialId, y => y.Ignore());

            CreateMap<TestMaterialDetailDto, TestMaterialModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.TypeId, y => y.MapFrom(z => z.TypeId))
                .ForMember(x => x.TypeLabel, y => y.MapFrom(z => z.TypeLabel))
                .ForMember(x => x.Title, y => y.MapFrom(z => z.Title))
                .ForMember(x => x.StatusId, y => y.MapFrom(z => z.StatusId))
                .ForMember(x => x.TestMaterialDomainId, y => y.MapFrom(z => z.TestMaterialDomainId))
                .ForMember(x => x.TypeDescription, y => y.MapFrom(z => z.TypeDescription))
                .ForMember(x => x.ApplicantsRangeTypeId, y => y.MapFrom(z => z.ApplicantsRangeTypeId))
                .ForMember(x => x.LastUsedDate, y => y.MapFrom(z => z.LastUsedDate))
                .ForMember(x => x.CredentialTypeId, y => y.Ignore())
                .ForMember(x => x.CredentialType, y => y.Ignore())
                .ForMember(x => x.DefaultMaterialRequestHours, y => y.Ignore())
                .ForMember(x => x.Links, y => y.Ignore())
                .ForMember(x => x.LanguageId, y => y.Ignore())
                .ForMember(x => x.Language, y => y.Ignore())
                .ForMember(x => x.SkillId, y => y.Ignore())
                .ForMember(x => x.SkillName, y => y.Ignore())
                .ForMember(x => x.Available, y => y.Ignore());
        }
    }
}
