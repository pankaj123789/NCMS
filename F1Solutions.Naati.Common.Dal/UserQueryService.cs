using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Newtonsoft.Json;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class UserQueryService : IUserQueryService
    {
        private readonly INcmsUserCacheQueryService _ncmsUserCacheQueryService;
        private readonly INcmsUserPermissionQueryService _ncmsUserPermissionQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public UserQueryService(INcmsUserCacheQueryService ncmsUserCacheQueryService, INcmsUserPermissionQueryService ncmsUserPermissionQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _ncmsUserCacheQueryService = ncmsUserCacheQueryService;
            _ncmsUserPermissionQueryService = ncmsUserPermissionQueryService;
            _autoMapperHelper = autoMapperHelper;
        }
        public UserDetailsDto GetUser(string userName)
        {
            return _ncmsUserCacheQueryService.GetUser(userName);
        }

        public IEnumerable<UserSearchDto> FindUserSearch(UserSearchRequest request)
        {
            var query = NHibernateSession.Current.Query<UserSearch>();

            query = query.Where(x => x.User.Id == request.UserId);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.SearchName == request.Query);
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                query = query.Where(x => x.SearchType == request.Type);
            }

            var userSearchesList = query.ToList();

            return userSearchesList.Select(_autoMapperHelper.Mapper.Map<UserSearchDto>).ToList();
        }

        public int SaveUserSearch(UserSearchDto request)
        {
            var data = NHibernateSession.Current.Get<UserSearch>(request.SearchId);
            UserSearch userSearch = data ?? new UserSearch();

            userSearch.User = NHibernateSession.Current.Load<User>(request.UserId);
            userSearch.SearchName = request.SearchName;
            userSearch.SearchType = request.SearchType;
            userSearch.CriteriaJson = request.CriteriaJson;
            userSearch.ModifiedDate = DateTime.Now;
            NHibernateSession.Current.SaveOrUpdate(userSearch);
            NHibernateSession.Current.Flush();
            return userSearch.Id;
        }

        public void DeleteUserSearch(int searchId)
        {
            var userSearch = NHibernateSession.Current.Get<UserSearch>(searchId);
            NHibernateSession.Current.Delete(userSearch);
            NHibernateSession.Current.Flush();
        }

        public IEnumerable<string> GetUserRolesByUserName(string userName)
        {
            var result = NHibernateSession.Current.Query<UserRole>()
                .Where(x => x.User.UserName == userName && x.User.Active)
                .Select(y => y.SecurityRole.Name).ToList();

            return result;
        }

        public bool IsUserInRoles(UserInRoleRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                throw new ArgumentNullException(nameof(request.UserName));
            }
            var roles = request.Roles.ToList();
            return NHibernateSession.Current.Query<UserRole>()
                 .Any(x => x.User.UserName == request.UserName && roles.Contains(x.SecurityRole.Name) && x.User.Active);
        }

        public IEnumerable<UserDto> UserSearch()
        {
            DateTime dt1 = DateTime.Now;
            UserRole userRoleAlias = null;
            Office officeAlias = null;
            Institution institutionAlias = null;

            var users = NHibernateSession.Current.QueryOver<User>()
                .Left.JoinAlias(x => x.UserRoles, () => userRoleAlias)
                .Left.JoinAlias(x => x.Office, () => officeAlias)
                .Left.JoinAlias(() => officeAlias.Institution, () => institutionAlias)
                .TransformUsing(Transformers.DistinctRootEntity).List();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                result.Add(MapToUserDto(user));
            }
            return result;
        }

        private UserDto MapToUserDto(User domain)
        {
            return new UserDto
            {
                Id = domain.Id,
                FullName = domain.FullName,
                UserName = domain.UserName,
                Email = domain.Email,
                Office = domain.Office.Institution.InstitutionAbberviation,
                UserRoles = domain.UserRoles.Select(x => x.SecurityRole.DisplayName).ToList(),
                RoleIds = domain.UserRoles.Select(x => x.SecurityRole.Id).ToList(),
                OfficeId = domain.Office.Id,
                Active = domain.Active,
                SystemUser = domain.SystemUser,
                Notes = domain.Note,
                NonWindowsUser = domain.NonWindowsUser,
                Password = domain.Password,
                LastPasswordChangeDate = domain.LastPasswordChangeDate,
                FailedPasswordAttemptCount = domain.FailedPasswordAttemptCount,
                IsLockedOut = domain.IsLockedOut,
                LastLockoutDate = domain.LastLockoutDate
            };
        }

        public IEnumerable<UserRoleDto> GetUserRoles()
        {
            return NHibernateSession.Current.Query<SecurityRole>().Select(x => MapToUserRoleDto(x)).ToList();
        }

        private UserRoleDto MapToUserRoleDto(SecurityRole role)
        {
            return new UserRoleDto
            {
                Id = role.Id,
                RoleName = role.Name,
                DisplayName = role.DisplayName,
                Description = role.Description,
                System = role.System
            };
        }

        public UserCheckResponse UserCheck(UserRequest request)
        {
            var userCheckResponse = new UserCheckResponse();

            var existingUsers = NHibernateSession.Current.QueryOver<User>().Where(x => x.UserName == request.User.UserName).List<User>();
            if (request.User.Id > 0)
            {
                var user = NHibernateSession.Current.Get<User>(request.User.Id);
                userCheckResponse.UserName = user.UserName;
                userCheckResponse.UserEmail = user.Email;
            }

            userCheckResponse.IsUserExist = existingUsers.Any();
            return userCheckResponse;
        }

        public CreateOrUpdateResponse CreateOrUpdateUser(UserRequest request)
        {
            var office = NHibernateSession.Current.Get<Office>(request.User.OfficeId);

            if (office == null)
            {
                throw new WebServiceException($"Office not found (ID {request.User.OfficeId})");
            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                var user = request.User.Id > 0 ? NHibernateSession.Current.Get<User>(request.User.Id) : new User();

                user.UserName = request.User.UserName;
                user.NonWindowsUser = request.User.NonWindowsUser;
                user.Active = request.User.Active;
                user.FullName = request.User.FullName;
                user.Email = request.User.Email;
                user.Note = request.User.Notes;
                user.Office = office;
                if (request.UpdatePassword)
                {
                    user.Password = request.User.Password;
                }
                user.FailedPasswordAttemptCount = request.User.FailedPasswordAttemptCount;
                user.IsLockedOut = request.User.IsLockedOut;
                user.LastPasswordChangeDate = request.User.LastPasswordChangeDate;

                NHibernateSession.Current.SaveOrUpdate(user);

                if (request.User.Id > 0)
                {
                    var userRoles = NHibernateSession.Current.QueryOver<UserRole>().Where(x => x.User.Id == request.User.Id).List<UserRole>();
                    var existingRoleIds = userRoles.Select(x => x.SecurityRole.Id).Distinct().ToList();
                    var roleIdsToRemove = existingRoleIds.Except(request.User.RoleIds.Distinct()).ToList();
                    var roleIdsToAdd = request.User.RoleIds.Distinct().Except(existingRoleIds).ToList();

                    if (roleIdsToRemove.Count > 0)
                    {
                        var userRolesToRemove = NHibernateSession.Current.Query<UserRole>().Where(x => x.User.Id == request.User.Id && roleIdsToRemove.Contains(x.SecurityRole.Id)).ToList();
                        userRolesToRemove.ForEach(NHibernateSession.Current.Delete);
                    }

                    if (roleIdsToAdd.Count > 0)
                    {
                        foreach (var roleIdToAdd in roleIdsToAdd)
                        {
                            var role = NHibernateSession.Current.Get<SecurityRole>(roleIdToAdd);
                            var userRole = new UserRole
                            {
                                User = user,
                                SecurityRole = role
                            };
                            NHibernateSession.Current.Save(userRole);
                        }
                    }
                }

                transaction.Commit();
                return new CreateOrUpdateResponse { Id = user.Id };
            }
        }

        public IEnumerable<int> GetExistingRolesByUserId(int userId)
        {
            return NHibernateSession.Current.Query<UserRole>().Where(x => x.User.Id == userId).Select(x => x.SecurityRole.Id).ToList();
        }

        public void RefreshLocalUserCache(string userName)
        {
            _ncmsUserCacheQueryService.RefreshUserCache(userName);
            _ncmsUserPermissionQueryService.RefreshUserCache(userName);
        }

        public UserDetailsResponse GetUserDetailsById(int id)
        {
            var response = new UserDetailsResponse();

            var user = NHibernateSession.Current.Get<User>(id);
            if (user != null)
            {
                response.Data = MapToUserDto(user);
            }
            return response;
        }
    }
}
