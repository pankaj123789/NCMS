using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class SystemProfile : Profile
    {
        public SystemProfile()
        {
            CreateMap<SystemValueDto, SystemValueModel>();

            CreateMap<ConfigDetailsDto, ConfigDetailsModel>();
        }
    }
}
