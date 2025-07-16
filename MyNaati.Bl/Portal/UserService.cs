using System;
using System.Linq;
using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;
using MyNaati.Contracts.Portal.Users;
using Constants = F1Solutions.Naati.Common.Contracts.Dal.DTO.Constants;

namespace MyNaati.Bl.Portal
{

    public class UserService : IUserService
    {
      
        private readonly IEmailChangeRepository mEmailChangeRepository;
        private readonly IUserRepository mUserRepository;

        public UserService( IEmailChangeRepository emailChangeRepository
            , IUserRepository userRepository)
        {
            mEmailChangeRepository = emailChangeRepository;
            mUserRepository = userRepository;
        }
        #region IUserService Members

       
   
        public EmailChangeResponse RegisterEmailChange(EmailChangeRequest request)
        {
            var emailChange = mEmailChangeRepository.GetByUserId(request.UserId) ?? new EmailChange { UserId = request.UserId };
            emailChange.Expiry = request.ExpiryDate;
            emailChange.PrimaryEmailAddress = request.CurrentPrimaryEmail;
            emailChange.SecondaryEmailAddress = request.NewEmail;
            emailChange.Reference = GetRandomNumber();
            mEmailChangeRepository.SaveOrUpdate(emailChange);
            return new EmailChangeResponse { Reference = emailChange.Reference };
        }

        public GetEmailChangeResponse GetRegisteredEmailChange(GetEmailChangeRequest request)
        {
            var foundRequests = mEmailChangeRepository.GetByReference(request.Reference, request.ExpiryDate);
            return new GetEmailChangeResponse
            {
                Items = foundRequests.Select(v => new EmailChangeItem
                {
                    Id = v.Id,
                    Reference = v.Reference,
                    Expiry = v.Expiry,
                    PrimaryEmail = v.PrimaryEmailAddress,
                    SecondaryEmail = v.SecondaryEmailAddress,
                })
            };
        }

        public GetEmailChangeResponse GetRegisteredEmailChangeByUser(Guid userId)
        {
            var response = new GetEmailChangeResponse { Items = Enumerable.Empty<EmailChangeItem>() };
            var result = mEmailChangeRepository.GetByUserId(userId);
            if (result != null)
            {
                response.Items = new[]
                {
                    new EmailChangeItem
                    {
                        Id = result.Id,
                        Reference = result.Reference,
                        Expiry = result.Expiry,
                        PrimaryEmail = result.PrimaryEmailAddress,
                        SecondaryEmail = result.SecondaryEmailAddress,
                    }
                };
            }
            return response;
        }

        private int GetRandomNumber()
        {
            var random = new Random();
            return random.Next(int.MaxValue);
        }

        public object GetPanelMembership(GetRegistrationRequest request)
        {
            throw new NotImplementedException();
        }

        public void ChangeUserName(ChangeUserNameRequest request)
        {
            mEmailChangeRepository.ChangeUserName(request.EmailChangeId);
        }

        public void CreateUser(UserRequest model)
        {
            mUserRepository.CreateUser(model);
        }

        public void DeleteUser(Guid userId)
        {
            mEmailChangeRepository.RemoveEmailChangesForUser(userId);
            mUserRepository.DeleteUser(userId);
        }

        public UserResponse GetUser(Guid userId)
        {
            var user = mUserRepository.GetUser(userId);

            if (user == null || user.NaatiNumber <= 0)
            {
                throw new FaultException(Constants.INVALID_USER);
            }

            return user;
        }
        #endregion
    }
}