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
    public class ProductSpecificationProfile : Profile
    {
        public ProductSpecificationProfile()
        {
            CreateMap<ProductSpecification, ProductSpecificationDetailsDto>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.GlCode, y => y.MapFrom(z => z.GLCode.Code))
                .ForMember(x => x.Code, y => y.MapFrom(z => z.Code))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.Description))
                .ForMember(x => x.CostPerUnit, y => y.MapFrom(z => z.CostPerUnit))
                .ForMember(x => x.Inactive, y => y.MapFrom(z => z.Inactive))
                .ForMember(x => x.GstApplies, y => y.MapFrom(z => z.GSTApplies));
        }
    }
}
