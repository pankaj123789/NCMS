using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<GeoLocationModel, GeoLocationDto>();

            CreateMap<GeoGeometryModel, GeoGeometryDto>();

            CreateMap<GeoResultModel, GeoResultDto>();

            CreateMap<GeoComponentModel, GeoComponentDto>();

            CreateMap<ParsedAddressDto, ParseAddressResponse>();
        }
    }
}
