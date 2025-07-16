using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.PractitionerDirectory;
using MyNaati.Ui.ViewModels.PDSearch;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class PDSearchProfile : Profile
    {
        public PDSearchProfile()
        {
            //CreateMap<PDSearchModel, PractitionerSearchCriteria>()
            //    .ForMember(y => y.AccreditationLevelId, opt => opt.MapFrom(src => (src.AccreditationLevelId == 0) ? (int?)null : src.AccreditationLevelId))
            //    .ForMember(y => y.CountryId, opt => opt.MapFrom(src => (src.CountryId == 0) ? (int?)null : src.CountryId))
            //    .ForMember(y => y.FirstLanguageId, opt => opt.MapFrom(src => (src.FirstLanguageId == 0) ? (int?)null : src.FirstLanguageId))
            //    .ForMember(y => y.SecondLanguageId, opt => opt.MapFrom(src => (src.SecondLanguageId == 0) ? (int?)null : src.SecondLanguageId))
            //    .ForMember(y => y.StateId, opt => opt.MapFrom(src => (src.StateId == 0) ? (int?)null : src.StateId))
            //    .ForMember(x => x.PageNumber, x => x.Ignore())
            //    .ForMember(x => x.PageSize, x => x.Ignore())
            //    .ForMember(x => x.SortOrder, x => x.Ignore());

            //CreateMap<PractitionerDirectoryGetContactDetailsResponse, ContactDetailsModel>()
            //    .ForMember(x => x.DefaultContryId, opt => opt.Ignore())
            //    .ForMember(x => x.ContactDetails, opt => opt.Ignore())
            //    .ForMember(x => x.Credentails, opt => opt.Ignore())
            //    .ForMember(x => x.Accreditations, opt => opt.Ignore());
        }



    }

}