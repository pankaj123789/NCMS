using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models.Institution;
using Ncms.Contracts.Models.Person;
using Ncms.Contracts.Models.UserSearch;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class SearchProfile : Profile
    {
        public SearchProfile()
        {
            CreateMap<LanguageSearchResultDto, LanguageSearchResultModel>()
                .ForMember(x => x.SkillsAttached, y => y.Ignore());

            CreateMap<SkillSearchResultDto, SkillSearchResultModel>();

            CreateMap<VenueSearchResultDto, VenueSearchResultModel>();

            //CreateMap<EntitySearchRequest, SearchRequest>() this makes no sense since there are no common fields at all
            //    .ForMember(x => x.Skip, y => y.Ignore())
            //    .ForMember(x => x.Take, y => y.Ignore())
            //    .ForMember(x => x.Filter, y => y.Ignore());

            CreateMap<SearchRequest, GetEmailSearchRequest>()
                .ForMember(x => x.Filters, y => y.Ignore())
                .ForMember(x => x.SortingOptions, y => y.Ignore());

            CreateMap<UserDto, UsersSearchResultModel>();

            CreateMap<EndorsedQualificationSearchResultDto, EndorsedQualificationSearchResultModel>();

            CreateMap<SearchRequest, EmailTemplateSearchRequest>()
                .ForMember(x => x.Filter, y => y.Ignore());

            CreateMap<UserSearchRequest, UserSearchListRequest>();

            CreateMap<UserSearchListRequest, UserSearchRequest>()
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<UserSearchDto, UserSearchModel>();

            CreateMap<UserSearchModel, UserSearchDto>();
        }
    }
}
