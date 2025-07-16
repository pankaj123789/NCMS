using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.User;

namespace Ncms.Bl
{
    public class UserService : IUserService
    {
        private readonly IUserQueryService _userQueryService;
        private readonly INcmsUserPermissionQueryService _ncmsUserPermissionQueryService;
        private readonly INcmsUserRefreshCacheQueryService _userRefreshCacheQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public UserService(IUserQueryService userQueryService,
            INcmsUserPermissionQueryService ncmsUserPermissionQueryService, 
            INcmsUserRefreshCacheQueryService userRefreshCacheQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _userQueryService = userQueryService;
            _ncmsUserPermissionQueryService = ncmsUserPermissionQueryService;
            _userRefreshCacheQueryService = userRefreshCacheQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public UserModel Get()
        {
            var user = GetUser(GetUserName());
            if (user == null)
            {
                LoggingHelper.LogWarning("User is null for Identity {Identity}", GetUserName());
            }
            return user;
        }

        public UserModel GetUser(string domainAndUserName)
        {
            try
            {
                var response = _userQueryService.GetUser(domainAndUserName);

                if (response == null)
                {
                    return null;
                }
                return new UserModel
                {
                    Id = response.Id,
                    Name = response.Name,
                    OfficeId = response.OfficeId,
                    DomainName = !string.IsNullOrEmpty(Environment.UserDomainName) ? Environment.UserDomainName : string.Empty,
                    Password = response.Password,
                    FailedPasswordAttemptCount = response.FailedPasswordAttemptCount
                };
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error getting current user.");
                return null;
            }
        }

        [Obsolete("Use HasPermssion() instead")]
        public bool IsInRoles(IEnumerable<string> roles)
        {
            return _userQueryService.IsUserInRoles(new UserInRoleRequest { UserName = GetUserName(), Roles = roles });
        }

        [Obsolete("Use HasPermssion() instead")]
        public bool IsInRole(string role)
        {
            return IsInRole(GetUserName(), role);
        }

        public bool IsInRole(string userName, string role)
        {
            return _userQueryService.IsUserInRoles(new UserInRoleRequest { UserName = userName, Roles = new[] { role } });
        }


        public IEnumerable<string> GetRolesByUserName(string userName)
        {
            return _userQueryService.GetUserRolesByUserName(userName);
        }

        public IEnumerable<UsersSearchResultModel> UserSearch()
        {
            return _userQueryService.UserSearch().Select(_autoMapperHelper.Mapper.Map<UsersSearchResultModel>).ToList();
        }

        public IEnumerable<UserRolesModel> GetUserRoles()
        {
            return _userQueryService.GetUserRoles()
                .Where(r => r.System == false)
                .OrderBy(x => x.DisplayName)
                .Select(_autoMapperHelper.Mapper.Map<UserRolesModel>)
                .ToList();
        }

        /// <summary>
        /// Gets a list of all rights for the current user. NOTE: It is faster to get this list from (CurrentPrincipal as NcmsPrincipal).Rights.
        /// </summary>
        public IEnumerable<UserPermissionsModel> GetUserPermissions()
        {
            var permissions = _ncmsUserPermissionQueryService.GetUserPermissionsByUserName(GetUserName());

            return permissions
                .GroupBy(x => new { x.Noun, x.NounName, x.NounDisplayName })
                .Select(g => new UserPermissionsModel
                {
                    Noun = g.Key.Noun,
                    NounName = g.Key.NounName,
                    NounDisplayName = g.Key.NounDisplayName,
                    Permissions = g.Aggregate((long)0, (x, y) => x | y.VerbMask)
                });
        }

        /// <summary>
        /// Checks if the current principal has a given permission.
        /// </summary>
        public bool HasPermission(SecurityNounName noun, SecurityVerbName verb)
        {
            return Thread.CurrentPrincipal.IsInRole($"{noun}.{verb}");
        }

        public GenericResponse<CreateOrUpdateResponse> CreateOrUpdateUser(UserDetailsModel model)
        {
            if (model.OfficeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.OfficeId));
            }
            var request = new UserRequest
            {
                User = _autoMapperHelper.Mapper.Map<UserDto>(model),
                UpdatePassword = model.UpdatePassword
            };

            if (model.NonWindowsUser)
            {
                if (model.Id == 0 && !String.IsNullOrWhiteSpace(model.Password)
                    || model.Id > 0 && model.UpdatePassword)
                {
                    var errors = ValidateUserPassword(model.Password);
                    if (errors.Any())
                    {
                        throw new UserFriendlySamException(String.Join("; ", errors));
                    }

                    request.User.Password = ServiceLocator.Resolve<ISecurityService>().GetPasswordHash(model.Password);
                    request.User.LastPasswordChangeDate = DateTime.Now;
                    request.User.FailedPasswordAttemptCount = 0;
                    request.User.IsLockedOut = false;
                }
                else
                {
                    request.User.Password = null;
                }
            }

            var userCheckResponse = _userQueryService.UserCheck(request);
            if (request.User.Id > 0)
            {
                //check user name exist during user edit
                if (String.Compare(request.User.UserName.Trim(), userCheckResponse.UserName.Trim(), StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    if (userCheckResponse.IsUserExist)
                    {
                        throw new UserFriendlySamException($"The user name {request.User.UserName} already exists.");
                    }
                }
            }
            else
            {
                //check user name exist during user creation
                if (userCheckResponse.IsUserExist)
                {
                    throw new UserFriendlySamException($"The user name {request.User.UserName} already exists.");
                }
            }

            var response = new GenericResponse<CreateOrUpdateResponse>()
            {
                Data = _userQueryService.CreateOrUpdateUser(request),
            };
            var maxDelay = _userRefreshCacheQueryService.GetDefaultRefreshDelay() * 2;
            response.Messages.Add(string.Format(Naati.Resources.User.ChangeWillTakeSomeSeconds, maxDelay));
            return response;
        }

        private IList<string> ValidateUserPassword(string password)
        {
            var errors = new List<string>();

            if (String.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password must not be empty or whitespace.");
            }

            return errors;
        }

        public IEnumerable<int> GetExistingRoleByUserId(int id)
        {
            return _userQueryService.GetExistingRolesByUserId(id).ToList();
        }

        public GenericResponse<UserDetailsModel> GetUserDetailsById(int id)
        {
            {
                if (id <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(id));
                }

                UserDetailsResponse result = _userQueryService.GetUserDetailsById(id);

                if (result.Data == null)
                {
                    throw new UserFriendlySamException("User not found.");
                }

                result.Data.Password = null;

                return result.ConvertServiceResponse<UserDto, UserDetailsModel>();
            }
        }

        public string GetUserName()
        {
            if (string.IsNullOrWhiteSpace(Thread.CurrentPrincipal.Identity?.Name))
            {
                throw new Exception("No authenticated user");
            }

            return Thread.CurrentPrincipal.Identity.Name;
        }

    }
}
