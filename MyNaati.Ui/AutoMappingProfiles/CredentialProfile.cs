using AutoMapper;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Ui.ViewModels.Certificate;
using MyNaati.Ui.ViewModels.Credential;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.Products;
using MyNaati.Ui.ViewModels.Stamp;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class CredentialProfile : Profile
    {
        public CredentialProfile()
        {
           CreateMap<Credential, AvailableProduct>()
               .ForMember(x => x.Expiry, y => y.MapFrom(z => z.ExpiryDate))
               .ForMember(x => x.Direction, y => y.MapFrom(z => Credential.GetDirection(z)));

           CreateMap<Credential, CertificateOrder>()
               .ForMember(x => x.Expiry, y => y.MapFrom(z => z.ExpiryDate))
               .ForMember(x => x.Direction, y => y.MapFrom(z => Credential.GetDirection(z)))
               .ForMember(x => x.QuantityLaminated, y => y.MapFrom(z => 0))
               .ForMember(x => x.QuantityUnlaminated, y => y.MapFrom(z => 0));          

           CreateMap<Credential, StampOrder>()
               .ForMember(x => x.Expiry, y => y.MapFrom(z => z.ExpiryDate))
               .ForMember(x => x.Direction, y => y.MapFrom(z => Credential.GetDirection(z)))
               .ForMember(x => x.QuantityRubber, y => y.MapFrom(z => 0))
               .ForMember(x => x.QuantitySelfInking, y => y.MapFrom(z => 0));

           CreateMap<CredentialRequestContract, CredentialRequestModel>();

           CreateMap<CredentialRequestRequestModel, CredentialRequestRequestContract>()
               .ForMember(x => x.UserName, y => y.Ignore());

           CreateMap<CredentialContract, CredentialModel>()
               .ForMember(x => x.CredentialId, y => y.MapFrom(z => z.Id))
               .ForMember(x => x.CredentialName, y => y.MapFrom(z => z.CredentialTypeExternalName))
               .ForMember(x => x.Certification, y => y.MapFrom(z => z.Certification))
               .ForMember(x => x.Direction, y => y.MapFrom(z => z.SkillDisplayName))
               .ForMember(x => x.StartDate, y => y.MapFrom(z => z.StartDate))
               .ForMember(x => x.ExpiryDate, y => y.MapFrom(z => z.ExpiryDate))
               .ForMember(x => x.ShowInOnlineDirectory, y => y.MapFrom(z => z.ShowInOnlineDirectory))
               .ForMember(x => x.Status, y => y.MapFrom(z => z.Status))
               .ForMember(x => x.CredentialCatergoryName, y => y.MapFrom(z => z.CredentialCategoryName));
        }
    }
}