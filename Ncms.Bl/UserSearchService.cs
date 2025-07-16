using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models.UserSearch;

namespace Ncms.Bl
{
    public class UserSearchService : IUserSearchService
    {

        private readonly IUserService _userService;
        private readonly IUserQueryService _userQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public UserSearchService(IUserService userService, IUserQueryService userQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _userService = userService;
            _userQueryService = userQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public IList<UserSearchModel> List(UserSearchListRequest request)
        {
            var filter = _autoMapperHelper.Mapper.Map<UserSearchRequest>(request);
            filter.UserId = _userService.Get().Id;

            var result = _userQueryService.FindUserSearch(filter);
            var models = result.Select(x => _autoMapperHelper.Mapper.Map<UserSearchModel>(x)).ToList();
            return models;
        }

        public UserSearchModel Save(UserSearchModel request)
        {
            var data = _autoMapperHelper.Mapper.Map<UserSearchDto>(request);

            data.UserId = _userService.Get().Id;

            var userSearchId = _userQueryService.SaveUserSearch(data);
            request.SearchId = userSearchId;

            return request;
        }

        public void Delete(int id)
        {
            _userQueryService.DeleteUserSearch(id);
        }
    }
}
