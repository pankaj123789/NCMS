using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class EndorsedQualificationProfile : Profile
    {
        public EndorsedQualificationProfile()
        {
            CreateMap<EndorsedQualification, EndorsedQualificationDto>()
                .ForMember(x => x.EndorsedQualificationId, y => y.MapFrom(z => z.Id));
        }
    }
}
