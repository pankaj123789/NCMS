using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using F1Solutions.Naati.Common.ServiceContracts.Services;
using NAATI.Domain;

namespace NAATI.WebService.NHibernate.DataAccess.AutoMapperRegistry
{    
    public class AddressDetailsProfile : Profile
    {
        public AddressDetailsProfile()
        {
            
        }

        protected override void Configure()
        {
            base.Configure();

            this.CreateMap<Address, DeliveryDetailsDTO>()
                .ForMember(x => x.StreetDetails, m => m.MapFrom(x => x.StreetDetails))
                .ForMember(x => x.SuburbOrCountry, m => m.MapFrom(x => x.SuburbOrCountry))
                .ForMember(x => x.Name, m => m.Ignore())
                .ForMember(x => x.DeliverTo, m => m.Ignore())
                .ForMember(x => x.AddressId, m => m.MapFrom(x => x.Id));
 
        }
    }
}
