using System;
using System.Collections.Generic;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Contracts.Models.Panel;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class PanelProfile : Profile
    {
        public PanelProfile()
        {
            CreateMap<PanelMembershipLookupDto, PanelMembershipLookupModel>()
                .IncludeBase<LookupTypeDto, LookupTypeModel>()
                .ForMember(x => x.Tasks, y => y.MapFrom(z => new List<MaterialRequestTaskModel>()))
                .ForMember(x => x.PreSelected, y => y.Ignore())
                .ForMember(x => x.IsCoordinator, y => y.Ignore());

            CreateMap<PanelDto, PanelModel>()
                .ForMember(x => x.CommissionedDate, y => y.MapFrom(z => z.ComissionedDate));

            CreateMap<PanelMembershipInfoDto, PanelMembershipInfoModel>();

            CreateMap<PanelTypeDto, PanelTypeModel>();
        }
    }
}
