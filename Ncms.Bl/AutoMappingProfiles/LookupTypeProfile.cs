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
    public class LookupTypeProfile : Profile
    {
        public LookupTypeProfile()
        {
            CreateMap<LookupTypeModel, LookupTypeModel>();

            CreateMap<LookupTypeDto, LookupTypeModel>();

            CreateMap<SkillLookupTypeModel, LookupTypeModel>();

            CreateMap<LookupTypeDetailedDto, LookupTypeDetailedModel>().IncludeBase<LookupTypeDto, LookupTypeModel>();

            CreateMap<CredentialLookupTypeDto, CredentialLookupTypeModel>()
                .IncludeBase<LookupTypeDto, LookupTypeModel>();

            CreateMap<SkillLookupDto, SkillLookupTypeModel>().IncludeBase<LookupTypeDto, LookupTypeModel>();

            CreateMap<TestTaskLookupTypeDto, TestTaskLookupTypeModel>().IncludeBase<LookupTypeDto, LookupTypeModel>();
        }
    }
}
