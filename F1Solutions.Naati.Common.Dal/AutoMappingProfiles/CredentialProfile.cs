using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;
using NHibernate.Type;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class CredentialProfile : Profile
    {
        public CredentialProfile()
        {
            CreateMap<Credential, CredentialDto>()
                .ForMember(x => x.Status, y => y.Ignore())//set manually
                .ForMember(x => x.StatusId, y => y.Ignore())//set manually
                .ForMember(x => x.Certification, y => y.Ignore())
                .ForMember(x => x.CredentialTypeInternalName, y => y.Ignore())//set manually
                .ForMember(x => x.CredentialTypeExternalName, y => y.Ignore())
                .ForMember(x => x.SkillDisplayName, y => y.Ignore())//set manually
                .ForMember(x => x.SkillId, y => y.Ignore())
                .ForMember(x => x.CredentialTypeId, y => y.Ignore())
                .ForMember(x => x.CategoryId, y => y.Ignore())
                .ForMember(x => x.StoredFileIds, y => y.Ignore())
                .ForMember(x => x.CredentialCategoryName, y => y.Ignore());//set manually
        }
    }
}
