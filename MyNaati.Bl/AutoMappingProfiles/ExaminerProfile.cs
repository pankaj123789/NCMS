using AutoMapper;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.Panel;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class ExaminerProfile : Profile
    {
        public ExaminerProfile()
        {
            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.ExaminerUnavailableContract, ExaminerUnavailableContract>();

            CreateMap<RolePlayerSettingsDto, F1Solutions.Naati.Common.Contracts.Dal.DTO.RolePlayerSettingsDto>()
                .ForMember(x => x.Senior, y => y.Ignore())
                .ForMember(x => x.Rating, y => y.Ignore());

            CreateMap<RolePlayerSettingsRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.RolePlayerSettingsRequest>();

            CreateMap<SaveUnavailabilityRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.SaveUnavailabilityRequest>();

            CreateMap<GetRolePlayerSettingsRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.GetRolePlayerSettingsRequest>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.DTO.RolePlayerSettingsDto, Contracts.BackOffice.RolePlayerSettingsDto>();

            CreateMap<ValidateExaminerSecurityCodeRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.ValidateExaminerSecurityCodeRequest>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.ValidateExaminerSecurityCodeResponse, ValidateExaminerSecurityCodeResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.SaveAttachmentResponse,MyNaati.Contracts.BackOffice.SaveAttachmentResponse>();

        }
    }
}
