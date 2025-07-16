using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;
using Ncms.Contracts.Models.Application;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class CredentialApplicationFieldProfile : Profile
    {
        public CredentialApplicationFieldProfile()
        {
            CreateMap<CredentialApplicationField, CredentialApplicationFieldDto>()
                .ForMember(x => x.FieldTypeId, y => y.MapFrom(z => z.Id));

            CreateMap<CredentialApplicationFieldOptionOption, CredentialApplicationFieldOptionDto>()
                .ForMember(x => x.DisplayName, y => y.MapFrom(z => z.CredentialApplicationFieldOption.DisplayName))
                .ForMember(x => x.FieldOptionId, y => y.MapFrom(z => z.Id));

            CreateMap<CredentialApplicationFieldModel, ApplicationFieldData>();
        }
    }
}
