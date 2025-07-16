using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Ui.ViewModels.Certificate;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.ExaminerTools;
using MyNaati.Ui.ViewModels.Products;
using MyNaati.Ui.ViewModels.Stamp;
using MaterialRequestRoundAttachmentDto = MyNaati.Contracts.BackOffice.MaterialRequestRoundAttachmentDto;
using MaterialRequestTaskModel = MyNaati.Ui.ViewModels.ExaminerTools.MaterialRequestTaskModel;
using RolePlayerSettingsDto = MyNaati.Contracts.BackOffice.RolePlayerSettingsDto;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class TestMaterialProfile : Profile
    {
        public TestMaterialProfile()
        {
            CreateMap<MaterialContract, MaterialModel>();

            //CreateMap<TestMaterialContract, TestModel>()
            //    .ForMember(x => x.TestResultID, y => y.Ignore())
            //    .ForMember(x => x.TestSittingId, y => y.Ignore())
            //    .ForMember(x => x.Supplementary, y => y.Ignore())
            //    .ForMember(x => x.SkillDisplayName, y => y.Ignore())
            //    .ForMember(x => x.CredentialTypeExternalName, y => y.Ignore())
            //    .ForMember(x => x.TestDate, y => y.Ignore())
            //    .ForMember(x => x.MaterialID, y => y.Ignore())
            //    .ForMember(x => x.Status, y => y.Ignore())
            //    .ForMember(x => x.HasAssets, y => y.Ignore())
            //    .ForMember(x => x.TestMarkingTypeId, y => y.Ignore())
            //    .ForMember(x => x.HasDefaultSpecification, y => y.Ignore());

            CreateMap<MaterialRequestSearchResultContract, MaterialRequestSearchResultModel>();


            
            CreateMap<MaterialRequestTaskContract, MaterialRequestTaskModel>();

            CreateMap<MaterialRequestPanelMembershipContract, MaterialRequestPanelMembershipModel>();

            CreateMap<TestMaterialRequestContract, TestMaterialRequestModel>()
                .ForMember(x => x.AvailableDocumentTypes, y => y.Ignore())
                .ForMember(x => x.MaterialRequestCoordinatorLoadingPercentage, y => y.Ignore())
                .ForMember(x => x.MaterialRequestTaskTypes, y => y.Ignore())
                .ForMember(x => x.LatestRoundNotes, y => y.Ignore());

            CreateMap<MaterialRequestTaskModel, MaterialRequestTaskContract>();

            CreateMap<MaterialRequestPanelMembershipModel, MaterialRequestPanelMembershipContract>();

            CreateMap<PostMaterialRequestMembersRequestModel, UpdateMaterialRequestMembersRequest>()
                .ForMember(x => x.NaatiNumber, y => y.Ignore());

            CreateMap<MaterialRequestRoundAttachmentDto, MaterialRequestRoundAttachmentModel>()
                .ForMember(x=>x.SoftDeleteDate, y => y.Ignore());

            CreateMap<MaterialRequestLinkContract, MaterialRequestRoundLinkModel>();
        }
    }
}