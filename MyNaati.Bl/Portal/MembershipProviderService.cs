using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web.Security;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using MyNaati.Bl.Portal.Security;
using MyNaati.Contracts.Portal;
using IsolationLevel = System.Transactions.IsolationLevel;
using UserRequest = F1Solutions.Naati.Common.Contracts.Dal.Portal.UserRequest;

namespace MyNaati.Bl.Portal
{
    public class MembershipProviderService : IMembershipProviderService
    {
        private readonly IEmailService mEmailService;
        private readonly IPersonQueryService mPersonQueryService;
        private readonly IUserService mUserService;
        private readonly IPasswordService mPasswordService;
        private readonly IConfigurationService mConfigurationService;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;

        private PasswordGenerator mPasswordGenerator;
        private const string TEMPORARY_PASSWORD_USER_GROUP = "TemporaryPasswordUsers";
        private const string DEFAULT_PASSWORD_QUESTION = "What is your password?";

        public MembershipProviderService(PasswordGenerator passwordGenerator,
            IEmailService emailService,
            IUserService userService,
            IPersonQueryService personQueryService,
            IPasswordService passwordService,
            IConfigurationService configurationService,
            ISecretsCacheQueryService secretsProvider,
            IAutoMapperHelper autoMapperHelper)
        {
            mConfigurationService = configurationService;
            mPasswordGenerator = passwordGenerator;
            mEmailService = emailService;
            mUserService = userService;
            mPersonQueryService = personQueryService;
            mPasswordService = passwordService;
            mSecretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
        }

        public void ActivateUser(string userName)
        {
            UpdateUserStatus(userName, true);
        }

        public void DeactivateUser(string userName)
        {
            UpdateUserStatus(userName, false);
        }

        private void UpdateUserStatus(string userName, bool activate)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var user = Membership.GetUser(userName);

                if (user == null)
                    return;

                user.IsApproved = activate;

                if (activate && user.IsLockedOut)
                    user.UnlockUser();

                Membership.UpdateUser(user);

                int? naatiNumber = null;
                int testInt;
                if (int.TryParse(user.UserName, out testInt))
                {
                    naatiNumber = testInt;
                    mPersonQueryService.UpdateMyNaatiDetails(new MyNaatiAccountDetails { NaatiNumber = naatiNumber.GetValueOrDefault(), Active = activate });
                }
                scope.Complete();
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            LoggingHelper.LogInfo("Attempting to change password for {MyNaatiUser}", username);

            bool changedPassword = false;
            try
            {
                var user = Membership.GetUser(username);

                if (user != null)
                {
                    changedPassword = user.ChangePassword(oldPassword, newPassword);

                    //Remove user from Temporary password group
                    if (Roles.IsUserInRole(user.UserName, TEMPORARY_PASSWORD_USER_GROUP) && changedPassword)
                        Roles.RemoveUserFromRole(user.UserName, TEMPORARY_PASSWORD_USER_GROUP);
                }
            }
            catch (Exception)
            {
                return changedPassword;
            }
            return changedPassword;
        }

        public MembershipCreateResult CreateUser(string username, string password, string passwordQuestion,
            string passwordAnswer, bool isApproved, object providerUserKey, bool addEmail, string givenName, string familyName, bool isNonCandidate, int naatiNumber)
        {
            LoggingHelper.LogInfo("Attempting to create user {MyNaatiUser}", username);

            MembershipCreateStatus status;
            bool emailSuccess = false;

            if (string.IsNullOrEmpty(password))
            {
                password = mPasswordGenerator.GetNewEPortalPassword();
            }

            //LoggingHelper.LogInfo($"Register: {username} {password}");

            if (string.IsNullOrWhiteSpace(passwordQuestion))
            {
                passwordQuestion = DEFAULT_PASSWORD_QUESTION;
            }

            if (string.IsNullOrWhiteSpace(passwordAnswer))
            {
                passwordQuestion = password;
            }
            
            MembershipUser user = null;

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                user = Membership.CreateUser(username, password, username, passwordQuestion, passwordAnswer, isApproved,
                    providerUserKey, out status);

                if (user?.ProviderUserKey != null)
                {
                    var webUser = new UserRequest
                    {
                        NaatiNumber = naatiNumber,
                        AspUserId = (Guid)user?.ProviderUserKey
                    };

                    mPasswordService.SavePasswordHistory(new PasswordHistoryRequest
                    {
                        DeleteCount = mConfigurationService.NumberPasswordsStore(),
                        UserId = (Guid)user.ProviderUserKey,
                        Password = mPasswordService.HashPassword(user.ProviderUserKey.ToString(), password)
                    });
                    mUserService.CreateUser(webUser);
                }

                if (status == MembershipCreateStatus.Success)
                {
                    // Add user to Temporary Password group 
                    Roles.AddUserToRole(user.UserName, TEMPORARY_PASSWORD_USER_GROUP);

                    //  Send a message for SAM to be updated
                    mPersonQueryService.UpdateMyNaatiDetails(new MyNaatiAccountDetails
                    {
                        NaatiNumber = naatiNumber,
                        DateCreated = DateTime.Now,
                        LastUpdate = DateTime.Now,
                        Active = true,
                    });

                    scope.Complete();
                }
            }

            if (status == MembershipCreateStatus.Success)
            {
                //  Send an email to the new user
                var request = new SendEmailRequest(EmailTemplate.RegisterSuccess, username);

                request.Tokens.Add(EmailTokens.GivenName, givenName);
                request.Tokens.Add(EmailTokens.Password, password);
                request.Tokens.Add(EmailTokens.NaatiNumber, naatiNumber.ToString());
                var emailResponse = mEmailService.SendMail(request);
                if (emailResponse.Success)
                {
                    emailSuccess = true;

                }
            }

            var ePortalUser = new ePortalUser();
            if (user != null)
            {
                _autoMapperHelper.Mapper.Map(user, ePortalUser);
            }

            //log error messages, if any
            string errorFragment = InterpretUserCreationStatus(status, emailSuccess);

            if (!string.IsNullOrEmpty(errorFragment))
            {
                LoggingHelper.LogWarning("Unable to create user with email '{MyNaatiUser}'. The problem was: {Error}.", username, errorFragment);
            }

            return new MembershipCreateResult(ePortalUser, status, password, emailSuccess);
        }

        /// <returns>An empty string if no error detected, or a description otherwise.</returns>
        public string InterpretUserCreationStatus(MembershipCreateStatus createStatus, bool emailSuccess)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists"; // TOOD MOVE ALL THIS TO RESOURCE FILES!
                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists";
                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid";
                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid";
                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid";
                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid";
                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid";
                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error";
                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled";
                default:
                    if (createStatus != MembershipCreateStatus.Success)
                        return createStatus.ToString();
                    else if (emailSuccess == false)
                        return "Unable to send approval email";
                    else
                        return "";
            }
        }

        public ePortalUser GetUser(string username, bool userIsOnline)
        {
            var user = Membership.GetUser(username, userIsOnline);
            if (user == null)
            {
                return null;
            }

            return new ePortalUser
            {
                Email = user.Email,
                IsActive = user.IsApproved,
                IsLocked = user.IsLockedOut,
                Username = user.UserName,
                CreationDate = user.CreationDate,
                UserId = (Guid)(user.ProviderUserKey ?? Guid.Empty),
                LastPasswordChangedDate = user.LastPasswordChangedDate,
                LastLoginDate = user.LastLoginDate
            };
        }

        public ePortalUser[] GetUsers(string[] userNames)
        {
            var userCount = userNames.Count();
            var results = new List<ePortalUser>();

            for (var i = 0; i < userCount; i++)
            {
                var user = GetUser(userNames[i], false);

                if (user != null)
                    results.Add(user);
            }

            return results.ToArray();
        }

        public string ResetPassword(string primaryEmail)
        {
            var user = Membership.GetUser(primaryEmail);

            if (user != null && user.IsApproved)
            {
                LoggingHelper.LogInfo("Resetting password for {MyNaatiUser}", primaryEmail);

                user.UnlockUser();
                // Add user to the TemporaryPasswordUser Group
                if (!Roles.IsUserInRole(user.UserName, TEMPORARY_PASSWORD_USER_GROUP))
                    Roles.AddUserToRole(user.UserName, TEMPORARY_PASSWORD_USER_GROUP);

                return user.ResetPassword();
            }

            return null;
        }

        public bool UpdateUserEmailAddressIfPresent(string usernameToFind, string newEmail)
        {
            LoggingHelper.LogInfo("Changing username/email {MyNaatiUser} to {NewUserName}", usernameToFind, newEmail);

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                var user = Membership.GetUser(usernameToFind);
                if (user != null)
                {
                    user.Email = newEmail;
                    Membership.UpdateUser(user);
                    scope.Complete();
                    return true;
                }

                return false;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            return Membership.ValidateUser(username, password);
        }

        public bool IsLockedOut(string username)
        {
            var user = Membership.GetUser(username);
            if (user != null && user.IsLockedOut)
                return true;
            return false;
        }

        public bool UnlockUser(string username)
        {
            LoggingHelper.LogInfo("Unlocking myNAATI user {UnlockedUserName}", username);
            var user = Membership.GetUser(username);
            return user.UnlockUser();
        }

        public bool DeleteUser(string username)
        {
            LoggingHelper.LogInfo("Deleting myNAATI user {DeletedUserName}", username);

            var user = Membership.GetUser(username);

            var userDetails = mUserService.GetUser((Guid)user.ProviderUserKey);
            var roles = Roles.GetRolesForUser(username);
            if (roles.Any())
            {
                Roles.RemoveUserFromRoles(username, roles);
            }
            mUserService.DeleteUser(userDetails.AspUserId);
            var response = Membership.DeleteUser(username);
            mPersonQueryService.DeleteMyNaatiDetails(userDetails.NaatiNumber);
            return response;
        }

        public BusinessServiceResponse RenameUser(string oldUsername, string newUsername)
        {
            LoggingHelper.LogInfo("Renaming myNAATI user {OldUsername} to {NewUsername}", oldUsername, newUsername);

            oldUsername.NotNullOrWhiteSpace("oldUsername");
            newUsername.NotNullOrWhiteSpace("newUsername");

            var response = new BusinessServiceResponse();
            var user = Membership.GetUser(oldUsername);

            if (user == null)
            {
                response.Success = false;
                response.Errors.Add($"myNAATI user {oldUsername} does not exist.");
                return response;
            }

            if (oldUsername == newUsername)
            {
                response.Success = false;
                response.Errors.Add("New username and old username are the same.");
                return response;
            }

            var connectionString = mSecretsProvider.Get("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                            UPDATE aspnet_Users SET UserName=@NewUsername,LoweredUserName=@LoweredNewUsername WHERE UserId=@UserId
                            UPDATE aspnet_Membership SET Email=@NewUsername,LoweredEmail=@LoweredNewUsername WHERE UserId=@UserId";

                    SqlParameter parameter = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
                    parameter.Value = user.ProviderUserKey;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter("@NewUsername", SqlDbType.VarChar);
                    parameter.Value = newUsername;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter("@LoweredNewUsername", SqlDbType.VarChar);
                    parameter.Value = newUsername.ToLower();
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                }
            }
            return response;
        }

        public BusinessServiceResponse SetMfaDetails(int naatiNumber, string mfaSecret)
        {
            var result = mPersonQueryService.SetPersonMfaDetails(new F1Solutions.Naati.Common.Contracts.Dal.Request.PersonMfaRequest()
            {
                NaatiNumber = naatiNumber,
                MfaCode = mfaSecret,
                MfaExpireStartDate = mfaSecret != null?DateTime.Now:(DateTime?)null
            });

            return result;
        }

        public GenericResponse<PersonMfaResponse> GetMfaDetails(int naatiNumber)
        {
            return mPersonQueryService.GetPersonMfaDetails(naatiNumber);
        }

        public BusinessServiceResponse DisableMfa(int naatiNumber)
        {
            var result = mPersonQueryService.SetPersonMfaDetails(new F1Solutions.Naati.Common.Contracts.Dal.Request.PersonMfaRequest()
            {
                NaatiNumber = naatiNumber,
                Disable = true
            });

            return result;
        }

        public GenericResponse<bool> GetAccessDisabledByNcms(int naatiNumber)
        {
            return mPersonQueryService.GetAccessDisabledByNcms(naatiNumber);
        }
    }
}