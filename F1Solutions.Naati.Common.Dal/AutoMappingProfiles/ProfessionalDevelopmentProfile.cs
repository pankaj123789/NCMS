using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class ProfessionalDevelopmentProfile : Profile
    {
        public ProfessionalDevelopmentProfile()
        {
            CreateMap<ProfessionalDevelopmentRequirement, ProfessionalDevelopmentRequirementResponse>()
                .ForMember(x => x.Points, y => y.Ignore());

            CreateMap<ProfessionalDevelopmentSection, ProfessionalDevelopmentSectionResponse>();
        }
    }
}
