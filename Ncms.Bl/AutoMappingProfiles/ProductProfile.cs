using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts.Models.Accounting;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductSpecificationDetailsDto, ProductSpecificationLookupModel>()
                .ForMember(x => x.DisplayNameWithGlCode, y => y.MapFrom(x => $"{x.GlCode.Trim()} - {x.Name}"));

            //Already mapped in common.dal
            //CreateMap<ProductSpecificationDetailsDto, ProductSpecificationModel>()
            //    .IncludeBase<ProductSpecificationDetailsDto, ProductSpecificationLookupModel>()
            //    .ForMember(x => x.DisplayNameWithGlCode, y => y.MapFrom(x => $"{x.GlCode.Trim()} - {x.Name}"));
        }
    }
}
