using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Bl.AutoMappingProfiles
{
    public class PodHistoryProfile : Profile
    {
        public PodHistoryProfile()
        {
            CreateMap<PodHistoryDto, PodHistoryModel>();
        }
    }
}
