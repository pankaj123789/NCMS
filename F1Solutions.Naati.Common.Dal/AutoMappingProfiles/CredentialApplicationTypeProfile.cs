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
    public class CredentialApplicationTypeProfile : Profile
    {
        public CredentialApplicationTypeProfile()
        {
            CreateMap<CredentialApplicationTypeDocumentType, CredentialApplicationTypeDocumentTypeDto>();

            CreateMap<CredentialApplicationType, CredentialApplicationTypeDto>()
                .ForMember(x => x.Category, y => y.Ignore());

            //CreateMap<CredentialApplicationTypeDto, CredentialApplicationType>()
            //    .ForMember(x => x.DisplayBills, y => y.Ignore())
            //    .ForMember(x => x.AllowMultiple, y => y.Ignore())
            //    .ForMember(x => x.CredentialApplicationTypeCategory, y => y.Ignore());
        }
    }
}
