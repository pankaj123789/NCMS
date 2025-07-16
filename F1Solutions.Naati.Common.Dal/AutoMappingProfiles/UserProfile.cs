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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserSearch, UserSearchDto>()
                .ForMember(x => x.SearchId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.User.Id));
        }
    }
}
