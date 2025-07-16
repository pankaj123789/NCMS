using System.Collections.Generic;
using System.Web.Security;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {
            CreateMap<MembershipUser, ePortalUser>()
                .ForMember(x => x.IsActive, y => y.MapFrom(x => x.IsApproved))
                .ForMember(x => x.IsLocked, y => y.MapFrom(x => x.IsLockedOut))
                .ForMember(x => x.UserId, y => y.Ignore());
        }
    }
}