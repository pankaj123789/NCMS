using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class TestSessionProfile : Profile
    {
        public TestSessionProfile()
        {
            CreateMap<AvailableTestSessionDto, AvailableTestSessionContract>()
                .ForMember(x => x.TestSessionId, y => y.MapFrom(z => z.TestSessionId))
                .ForMember(x => x.TestDate, y => y.MapFrom(z => z.TestDateTime))
                .ForMember(x => x.AvailableSeats, y => y.MapFrom(z => z.AvailableSeats))
                .ForMember(x => x.Selected, y => y.MapFrom(z => z.Selected))
                .ForMember(x => x.TestSessionDuration, y => y.MapFrom(z => z.TestSessionDuration))
                .ForMember(x => x.TestLocation, y => y.MapFrom(z => z.TestLocation))
                .ForMember(x => x.VenueAddress, y => y.MapFrom(z => z.VenueAddress))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.VenueName, y => y.MapFrom(z => z.VenueName))
                .ForMember(x => x.IsPreferedLocation, y => y.MapFrom(z => z.IsPreferedLocation));

            CreateMap<GetRolePlaySessionRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.GetRolePlaySessionRequest>()
                .ForMember(x => x.IncludeRejected, y => y.Ignore())
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(z => z.NaatiNumber.GetValueOrDefault()));


            CreateMap<TestSessionRolePlayerDetailDto, TestSessionRolePlayerDetailContract>()
                .ForMember(x => x.LanguageName, y => y.Ignore());

            CreateMap<TestSessionRolePlayerDto, RolePlaySessionContract>()
                .ForMember(x => x.CanAccept, y => y.Ignore())
                .ForMember(x => x.CanReject, y => y.Ignore())
                .ForMember(x => x.AcceptActionId, y => y.Ignore())
                .ForMember(x => x.RejectActionId, y => y.Ignore())
                .ForMember(x => x.Details, y => y.MapFrom(z => z.Details));
        }
    }
}
