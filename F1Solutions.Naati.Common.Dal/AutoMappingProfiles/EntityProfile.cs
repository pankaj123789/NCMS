using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            CreateMap<UpdatePersonDetailsRequest, Person>()
                .ForMember(x => x.Entity, y => y.Ignore())
                .ForMember(x => x.SponsorInstitution, y => y.Ignore())
                .ForMember(x => x.ReleaseDetails, y => y.Ignore())
                .ForMember(x => x.DoNotInviteToDirectory, y => y.Ignore())
                .ForMember(x => x.EnteredDate, y => y.Ignore())
                .ForMember(x => x.ExpertiseFreeText, y => y.Ignore())
                .ForMember(x => x.NameOnAccreditationProduct, y => y.Ignore())
                .ForMember(x => x.ScanRequired, y => y.Ignore())
                .ForMember(x => x.AllowAutoRecertification, y => y.Ignore())
                .ForMember(x => x.IsEportalActive, y => y.Ignore())
                .ForMember(x => x.PersonalDetailsLastUpdatedOnEportal, y => y.Ignore())
                .ForMember(x => x.WebAccountCreateDate, y => y.Ignore())
                .ForMember(x => x.AllowVerifyOnline, y => y.Ignore())
                .ForMember(x => x.ShowPhotoOnline, y => y.Ignore())
                .ForMember(x => x.InterculturalCompetency, y => y.Ignore())
                .ForMember(x => x.KnowledgeTest, y => y.Ignore())
                .ForMember(x => x.EthicalCompetency, y => y.Ignore())
                .ForMember(x => x.RevalidationScheme, y => y.Ignore())
                .ForMember(x => x.ExaminerSecurityCode, y => y.Ignore())
                .ForMember(x => x.ExaminerTrackingCategory, y => y.Ignore())
                .ForMember(x => x.PractitionerNumber, y => y.Ignore())
                .ForMember(x => x.Addresses, y => y.Ignore())
                .ForMember(x => x.GivenName, y => y.Ignore())
                .ForMember(x => x.AlternativeGivenName, y => y.Ignore())
                .ForMember(x => x.OtherNames, y => y.Ignore())
                .ForMember(x => x.Surname, y => y.Ignore())
                .ForMember(x => x.AlternativeSurname, y => y.Ignore())
                .ForMember(x => x.Title, y => y.Ignore())
                .ForMember(x => x.TitleId, y => y.Ignore())
                .ForMember(x => x.AccreditationCountNonPopulated, y => y.Ignore())
                .ForMember(x => x.RecognitionCountNonPopulated, y => y.Ignore())
                .ForMember(x => x.SuburbOrCountry, y => y.Ignore())
                .ForMember(x => x.HasPhoto, y => y.Ignore())
                .ForMember(x => x.PhotoDate, y => y.Ignore())
                .ForMember(x => x.Photo, y => y.Ignore())
                .ForMember(x => x.Id, y => y.MapFrom(z => z.PersonId))
                .ForMember(x => x.BirthCountry, y => y.Ignore())
                .ForMember(x => x.MfaCode, y => y.Ignore())
                .ForMember(x => x.MfaExpireStartDate, y => y.Ignore())
                .ForMember(x => x.EmailCodeExpireStartDate, y => y.Ignore())
                .ForMember(x => x.AccessDisabledByNcms, y => y.Ignore())
                .ForMember(x => x.LastEmailCode, y => y.Ignore());

            //CreateMap<UpdatePersonDetailsRequest, Entity>();

            CreateMap<UpdatePersonSettingsRequest, NaatiEntity>()
                .ForMember(x => x.EntityTypeId, y => y.Ignore())
                .ForMember(x => x.WebsiteInPD, y => y.Ignore())
                .ForMember(x => x.WebsiteUrl, y => y.Ignore())
                .ForMember(x => x.Note, y => y.Ignore())
                .ForMember(x => x.NaatiNumber, y => y.Ignore())
                .ForMember(x => x.Id, y => y.Ignore());

            CreateMap<UpdatePersonSettingsRequest, Person>()
                .ForMember(x => x.Entity, y => y.Ignore())
                .ForMember(x => x.BirthCountry, y => y.Ignore())
                .ForMember(x => x.SponsorInstitution, y => y.Ignore())
                .ForMember(x => x.EnteredDate, y => y.Ignore())
                .ForMember(x => x.ExpertiseFreeText, y => y.Ignore())
                .ForMember(x => x.NameOnAccreditationProduct, y => y.Ignore())
                .ForMember(x => x.ScanRequired, y => y.Ignore())
                .ForMember(x => x.IsEportalActive, y => y.Ignore())
                .ForMember(x => x.PersonalDetailsLastUpdatedOnEportal, y => y.Ignore())
                .ForMember(x => x.WebAccountCreateDate, y => y.Ignore())
                .ForMember(x => x.ExaminerSecurityCode, y => y.Ignore())
                .ForMember(x => x.PractitionerNumber, y => y.Ignore())
                .ForMember(x => x.Addresses, y => y.Ignore())
                .ForMember(x => x.GivenName, y => y.Ignore())
                .ForMember(x => x.AlternativeGivenName, y => y.Ignore())
                .ForMember(x => x.OtherNames, y => y.Ignore())
                .ForMember(x => x.Surname, y => y.Ignore())
                .ForMember(x => x.AlternativeSurname, y => y.Ignore())
                .ForMember(x => x.Title, y => y.Ignore())
                .ForMember(x => x.TitleId, y => y.Ignore())
                .ForMember(x => x.AccreditationCountNonPopulated, y => y.Ignore())
                .ForMember(x => x.RecognitionCountNonPopulated, y => y.Ignore())
                .ForMember(x => x.SuburbOrCountry, y => y.Ignore())
                .ForMember(x => x.HasPhoto, y => y.Ignore())
                .ForMember(x => x.PhotoDate, y => y.Ignore())
                .ForMember(x => x.Photo, y => y.Ignore())
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.MfaCode, y => y.Ignore())
                .ForMember(x => x.MfaExpireStartDate, y => y.Ignore())
                .ForMember(x => x.EmailCodeExpireStartDate, y => y.Ignore())
                .ForMember(x => x.LastEmailCode, y => y.Ignore())
                ; 

            CreateMap<AddressDetailsDto, Address>()
                .ForMember(x => x.Postcode, y => y.Ignore())
                .ForMember(x => x.Entity, y => y.Ignore())
                .ForMember(x => x.Country, y => y.Ignore())
                .ForMember(x => x.EndDate, y => y.Ignore())
                .ForMember(x => x.StartDate, y => y.Ignore())
                .ForMember(x => x.SubscriptionExpiryDate, y => y.Ignore())
                .ForMember(x => x.SubscriptionRenewSentDate, y => y.Ignore())
                .ForMember(x => x.OdAddressVisibilityType, y => y.Ignore())
                .ForMember(x => x.ContactPerson, y => y.Ignore())
                .ForMember(x => x.Id, y => y.Ignore()); 

            CreateMap<PhoneDetailsDto, Phone>()
                .ForMember(x => x.Entity, y => y.Ignore())
                .ForMember(x => x.CountryCode, y => y.Ignore())
                .ForMember(x => x.AreaCode, y => y.Ignore())
                .ForMember(x => x.Id, y => y.Ignore());

            CreateMap<PersonName, PersonNameDto>()
                .ForMember(x => x.TitleId, y => y.MapFrom(z => z.Title.Id))
                .ForMember(x => x.PersonNameId, y => y.MapFrom(x => x.Id));

            CreateMap<PersonNameDto, PersonName>()
                .ForMember(x => x.Title, y => y.Ignore())
                .ForMember(x => x.AlternativeGivenName, y => y.NullSubstitute(string.Empty))
                .ForMember(x => x.AlternativeSurname, y => y.NullSubstitute(string.Empty))
                .ForMember(x => x.OtherNames, y => y.NullSubstitute(string.Empty))
                .ForMember(x => x.Person, y => y.Ignore())
                .ForMember(x => x.Id, y => y.MapFrom(z => z.PersonNameId));

        }
    }
}
