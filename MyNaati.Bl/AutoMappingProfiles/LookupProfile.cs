using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using PaymentDto = MyNaati.Contracts.BackOffice.PaymentDto;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class LookupProfile : Profile
    {
        public LookupProfile()
        {
            CreateMap<LookupTypeDto, LookupContract>();

            CreateMap<FormLookupTypeDto, FormLookupContract>().IncludeBase<LookupTypeDto, LookupContract>();

            CreateMap<SkillLookupDto, SkillLookupContract>().IncludeBase<LookupTypeDto, LookupContract>()
                .ForMember(x => x.Language1Name, y => y.Ignore())
                .ForMember(x => x.Language2Name, y => y.Ignore())
                .ForMember(x => x.DirectionDisplayName, y => y.Ignore());

            CreateMap<CredentialLookupTypeDto, CredentialLookupTypeContract>().IncludeBase<LookupTypeDto, LookupContract>();

            CreateMap<CredentialLookupTypeDto, LookupContract>();

            CreateMap<PanelMembershipLookupDto, PanelMembershipLookupModel>()
                .ForMember(x => x.Tasks, y => y.MapFrom(x => new List<MaterialRequestTaskModel>()));
        }
    }
}
