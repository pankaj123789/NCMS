using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class CertificationPeriodProfile : Profile
    {
        public CertificationPeriodProfile()
        {
            CreateMap<CertificationPeriod, CertificationPeriodDto>()
                .ForMember(x => x.CredentialApplicationId, y => y.MapFrom(z => z.CredentialApplication.Id))
                .ForMember(x => x.CertificationPeriodStatus, y => y.Ignore())
                .ForMember(x => x.UserId, y => y.Ignore())
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(z => z.Person.GetNaatiNumber()))
                .ForMember(x => x.Notes, y => y.Ignore());
        }
    }
}
