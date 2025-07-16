using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class SkillProfile : Profile
    {
        public SkillProfile()
        {
            CreateMap<SkillDto, SkillModel>();

            CreateMap<SkillDetailsDto, SkillDetailsModel>();

            CreateMap<SkillRequest, SaveSkillRequest>()
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<LanguageRequest, SaveLanguageRequest>()
                .ForMember(x => x.UserId, y => y.Ignore());
        }
    }
}
