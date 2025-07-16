using System.Collections.Generic;
using Ncms.Contracts.Models.UserSearch;

namespace Ncms.Contracts
{
    public interface IUserSearchService
    {
        IList<UserSearchModel> List(UserSearchListRequest request);
        UserSearchModel Save(UserSearchModel request);
        void Delete(int id);
    }
}
