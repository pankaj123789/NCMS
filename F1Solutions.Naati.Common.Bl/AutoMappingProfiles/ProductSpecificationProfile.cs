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
    public class ProductSpecificationProfile : Profile
    {
        public ProductSpecificationProfile()
        {
            CreateMap<ProductSpecificationDetailsDto, ProductSpecificationModel>()
                .ForMember(x => x.CredentialFeeProductId, y => y.Ignore())
                .ForMember(x => x.DisplayNameWithGlCode, y => y.MapFrom(x => $"{x.GlCode.Trim()} - {x.Name}"));
        }
    }
}
