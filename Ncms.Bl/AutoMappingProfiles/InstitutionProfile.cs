using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts.Models.Institution;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class InstitutionProfile : Profile
    {
        public InstitutionProfile()
        {
            CreateMap<EndorsedQualificationRequest, EndorsedQualificationDto>()
                .ForMember(x => x.Institution, y => y.Ignore())
                .ForMember(x => x.CredentialTypeExternalName, y => y.Ignore());

            CreateMap<EndorsedQualificationDto, EndorsedQualificationModel>();

            CreateMap<InstitutionDto, InstitutionModel>();

            CreateMap<InstitutionModel, InstitutionDto>()
                .ForMember(x => x.HasAustralianAddress, y => y.Ignore())
                .ForMember(x => x.LatestEndDateForApprovedCourse, y => y.Ignore())
                .ForMember(x => x.HasIndigenousLanguagesOnly, y => y.Ignore())
                .ForMember(x => x.EntityTypeId, y => y.Ignore())
                .ForMember(x => x.NaatiNumberDisplay, y => y.Ignore())
                .ForMember(x => x.SamId, y => y.Ignore())
                .ForMember(x => x.DisplayText, y => y.Ignore());
        }
    }
}
