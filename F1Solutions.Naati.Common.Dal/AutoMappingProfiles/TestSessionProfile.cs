using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class TestSessionProfile : Profile
    {
        public TestSessionProfile()
        {
            CreateMap<UpdateTestSessionRequest, TestSession>()
                .ForMember(x => x.Venue, y => y.Ignore())
                .ForMember(x => x.CredentialType, y => y.Ignore())
                .ForMember(x => x.Completed, y => y.Ignore())
                .ForMember(x => x.PublicNote, y => y.Ignore())
                .ForMember(x => x.AllowSelfAssign, y => y.Ignore())
                .ForMember(x => x.OverrideVenueCapacity, y => y.Ignore())
                .ForMember(x => x.NewCandidatesOnly, y => y.Ignore())
                .ForMember(x => x.Capacity, y => y.Ignore())
                .ForMember(x => x.RehearsalDateTime, y => y.Ignore())
                .ForMember(x => x.RehearsalNotes, y => y.Ignore())
                .ForMember(x => x.AllowAvailabilityNotice, y => y.Ignore())
                .ForMember(x => x.DefaultTestSpecification, y => y.Ignore())
                .ForMember(x => x.Id, y => y.MapFrom(z => z.TestSessionId));

            CreateMap<CreateOrUpdateTestSessionRequest, TestSession>()
                .ForMember(x => x.Venue, y => y.Ignore())
                .ForMember(x => x.CredentialType, y => y.Ignore())
                .ForMember(x => x.DefaultTestSpecification, y => y.Ignore());

            CreateMap<TestSessionSkillDto, TestSessionSkill>()
                .ForMember(x => x.TestSession, y => y.Ignore())
                .ForMember(x => x.Skill, y => y.Ignore());

            CreateMap<TestSessionSkill, TestSessionSkillDto>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Skill.DisplayName))
                .ForMember(x => x.TestSessionId, y => y.MapFrom(z => z.TestSession.Id))
                .ForMember(x => x.SkillId, y => y.MapFrom(z => z.Skill.Id));

            CreateMap<Venue, VenueDto>()
                .ForMember(x => x.VenueId, y => y.MapFrom(z => z.Id));
        }
    }
}
