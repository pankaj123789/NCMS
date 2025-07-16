using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice.Panel;
using Panel = F1Solutions.Naati.Common.Dal.Domain.Panel;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class PanelProfile : Profile
    {
        public PanelProfile()
        {
            CreateMap<PanelDto, Contracts.BackOffice.Panel.Panel>();

            CreateMap<PanelDto, Panel>()
                .ForMember(x => x.Language, y => y.Ignore())
                .ForMember(x => x.PanelType, y => y.Ignore())//cannot map from string - object
                .ForMember(x => x.CommissionedDate, y => y.MapFrom(z => z.ComissionedDate))
                .ForMember(x => x.Id, y => y.MapFrom(z => z.PanelId));

            CreateMap<MembershipDto, Membership>();

            CreateMap<GetMembershipsRequest, F1Solutions.Naati.Common.Contracts.Dal.Request.GetMembershipsRequest>()
                .ForMember(x => x.MembershipId, y => y.Ignore());
        }
    }
}
