using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class VenueProfile : Profile
    {
        public VenueProfile()
        {
            CreateMap<VenueRequest, SaveVenueRequest>()
                .ForMember(x => x.ModifiedByNaati, y => y.Ignore())
                .ForMember(x => x.ModifiedDate, y => y.Ignore())
                .ForMember(x => x.ModifiedUser, y => y.Ignore());
        }
    }
}
