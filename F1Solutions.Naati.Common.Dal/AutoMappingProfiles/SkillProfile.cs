using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class SkillProfile : Profile
    {
        public SkillProfile()
        {
            CreateMap<Skill, SkillDto>()
                .ForMember(x => x.DirectionDisplayName, y => y.MapFrom(z => z.DisplayName));
        }
    }
}
