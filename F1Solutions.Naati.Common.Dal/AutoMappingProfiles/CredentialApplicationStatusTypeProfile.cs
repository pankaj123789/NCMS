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
    public class CredentialApplicationStatusTypeProfile : Profile
    {
        public CredentialApplicationStatusTypeProfile()
        {
            CreateMap<CredentialApplicationStatusType, LookupTypeDetailedDto>()
                .ForMember(x => x.ExtraData, y => y.Ignore());
        }
    }
}
