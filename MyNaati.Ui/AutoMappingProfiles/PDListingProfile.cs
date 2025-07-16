using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.PDListing;
using MyNaati.Contracts.BackOffice.PractitionerDirectory;
using MyNaati.Ui.ViewModels.PDListing;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class PDListingProfile : Profile
    {
        public PDListingProfile()
        {
            CreateMap<AddressListing, WizardModel.AddressEditModel>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.AddressId))
                .ForMember(x => x.Address, m => m.MapFrom(x => x.FullAddress))
                .ForMember(x => x.InDirectory, m => m.MapFrom(x => x.InDirectory))
                .ForMember(x => x.OdAddressVisibilityTypeId, m => m.MapFrom(x => x.OdAddressVisibilityTypeId))
                .ForMember(x => x.OdAddressVisibilityTypeName, m => m.MapFrom(x => x.OdAddressVisibilityTypeName))
                .ForMember(x => x.Location, m => m.MapFrom(x => x.Location));
                
            CreateMap<ContactDetailListing, WizardModel.ContactDetailsEditModel>()
                .ForMember(x => x.Id, m => m.MapFrom(x => x.ContactDetailsId))
                .ForMember(x => x.InDirectory, m => m.MapFrom(x => x.IncludeInPractitionerDirectory));

            CreateMap<ExpertiseItem, WizardModel.WorkAreasEditModel>()
                .ForMember(x => x.IsChecked, m => m.MapFrom(x => x.IncludeInPractitionerDirectory));

            CreateMap<CredentialListing, WizardModel.CredentialsEditModel>()
                .ForMember(x => x.InDirectory, m => m.MapFrom(x => x.IncludeInPractitionerDirectory));

            CreateMap<WizardModel.AddressEditModel, AddressListingUpdate>()
                .ForMember(x => x.AddressId, m => m.MapFrom(x => x.Id));

            CreateMap<WizardModel.ContactDetailsEditModel, ContactDetailsUpdate>();

            CreateMap<WizardModel.CredentialsEditModel, CredentialUpdate>();

            CreateMap<WizardModel.WorkAreasEditModel, ExpertiseItem>()
                .ForMember(x => x.IncludeInPractitionerDirectory, m => m.MapFrom(x => x.IsChecked));
        }
    }
}
