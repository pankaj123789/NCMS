using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using MyNaati.Contracts.BackOffice;
using DeleteAttachmentResponse = MyNaati.Contracts.BackOffice.DeleteAttachmentResponse;
using DeleteMaterialResponse = MyNaati.Contracts.BackOffice.DeleteMaterialResponse;
using GetDocumentResponse = MyNaati.Contracts.BackOffice.GetDocumentResponse;
using GetTestAssetsFileResponse = MyNaati.Contracts.BackOffice.GetTestAssetsFileResponse;
using GetTestMaterialsFileResponse = MyNaati.Contracts.BackOffice.GetTestMaterialsFileResponse;
using MaterialRequestRoundAttachmentDto = F1Solutions.Naati.Common.Contracts.Dal.DTO.MaterialRequestRoundAttachmentDto;
using SaveMaterialResponse = MyNaati.Contracts.BackOffice.SaveMaterialResponse;
using SubmitMaterialResponse = MyNaati.Contracts.BackOffice.SubmitMaterialResponse;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.MaterialContract, MaterialContract>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.TestMaterialContract, TestMaterialContract>();

            CreateMap<SaveMaterialRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.SaveMaterialRequest>();

            CreateMap<SaveMaterialResponse, F1Solutions.Naati.Common.Contracts.Dal.Response.SaveMaterialResponse>();

            CreateMap<DeleteMaterialRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.DeleteMaterialRequest>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.DeleteMaterialResponse, DeleteMaterialResponse>();

            CreateMap<SubmitMaterialRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.SubmitMaterialRequest>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.SubmitMaterialResponse, SubmitMaterialResponse>();

            CreateMap<SaveAttachmentRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.SaveAttachmentRequest>();

            //CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.SaveAttachmentResponse, SaveAttachmentRequest>();


            CreateMap<DeleteAttachmentRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.DeleteAttachmentRequest>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.DeleteAttachmentResponse, DeleteAttachmentResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetTestAssetsFileResponse, GetTestAssetsFileResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetAttendeesTestSpecificationTestMaterialResponse, F1Solutions.Naati.Common.Contracts.Dal.Response.GetAttendeesTestSpecificationTestMaterialResponse>();


            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetDocumentResponse, GetDocumentResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetTestMaterialsFileResponse, GetTestMaterialsFileResponse>()
                .IncludeBase<F1Solutions.Naati.Common.Contracts.Dal.Response.GetDocumentResponse, GetDocumentResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.DocumentTypeContract, DocumentTypeContract>();

            CreateMap<MaterialRequestTaskDto, MaterialRequestTaskContract>();

            CreateMap<MaterialRequestPanelMembershipDto, MaterialRequestPanelMembershipContract>();

            CreateMap<GetMaterialRequestRoundAttachmentsRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.GetMaterialRequestRoundAttachmentsRequest>()
                .ForMember(x => x.PersonId, y => y.Ignore());

            CreateMap<MaterialRequestRoundAttachmentDto, MyNaati.Contracts.BackOffice.MaterialRequestRoundAttachmentDto>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetMaterialRequestAttachmentsResponse, GetMaterialRequestRoundAttachmentsResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetMaterialRequestAttachmentResponse, GetMaterialRequestRoundAttachmentResponse>();

            CreateMap<MaterialRequestTaskContract, MaterialRequestTaskDto>();

            CreateMap<MaterialRequestPanelMembershipContract, MaterialRequestPanelMembershipDto>()
                .ForMember(x => x.PayRoll, y => y.Ignore());

            CreateMap<UpdateMaterialRequestMembersRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.UpdateMaterialRequestMembersRequest>();

            CreateMap<NoteDto, NoteModel>();
        }
    }
}
