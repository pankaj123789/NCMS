//using System;
//using System.Web.Security;
//using Ncms.Contracts;
//using Ninject;

//namespace Ncms.Ui.Security
//{
//    public class MembershipProvider : System.Web.Security.MembershipProvider
//    {
//        public override bool ValidateUser(string username, string password)
//        {
//            throw new NotImplementedException();
//        }

//        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
//        {
//            throw new NotImplementedException();
//        }

//        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
//        {
//            throw new NotImplementedException();
//        }

//        public override int GetNumberOfUsersOnline()
//        {
//            throw new NotImplementedException();
//        }

//        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool DeleteUser(string username, bool deleteAllRelatedData)
//        {
//            throw new NotImplementedException();
//        }

//        public override string GetUserNameByEmail(string email)
//        {
//            throw new NotImplementedException();
//        }

//        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
//        {
//            throw new NotImplementedException();
//        }

//        public override MembershipUser GetUser(string username, bool userIsOnline)
//        {
//            var service = NinjectWebCommon.Kernel.Get<IUserService>();
//            var user = service.Get();

//            if (user == null)
//                return null;

//            var membershipUser = new MembershipUser(Name,
//                username,
//                user.Id,
//                string.Empty,
//                string.Empty,
//                string.Empty,
//                true,
//                false,
//                DateTime.MinValue,
//                DateTime.MinValue,
//                DateTime.MinValue,
//                DateTime.MinValue,
//                DateTime.MinValue);

//            return membershipUser;
//        }

//        public override bool UnlockUser(string userName)
//        {
//            throw new NotImplementedException();
//        }

//        public override void UpdateUser(MembershipUser user)
//        {
//            throw new NotImplementedException();
//        }

//        public override string ResetPassword(string username, string answer)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool ChangePassword(string username, string oldPassword, string newPassword)
//        {
//            throw new NotImplementedException();
//        }

//        public override string GetPassword(string username, string answer)
//        {
//            throw new NotImplementedException();
//        }

//        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
//        {
//            throw new NotImplementedException();
//        }

//        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
//        {
//            throw new NotImplementedException();
//        }

//        public override string PasswordStrengthRegularExpression
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override int MinRequiredNonAlphanumericCharacters
//        {
//            get { return 0; }
//        }

//        public override int MinRequiredPasswordLength
//        {
//            get { return 9; }
//        }

//        public override MembershipPasswordFormat PasswordFormat
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override bool RequiresUniqueEmail
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override int PasswordAttemptWindow
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override int MaxInvalidPasswordAttempts
//        {
//            get { return 3; }
//        }

//        public override string ApplicationName
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public override bool RequiresQuestionAndAnswer
//        {
//            get { return false; }
//        }

//        public override bool EnablePasswordReset
//        {
//            get { return true; }
//        }

//        public override bool EnablePasswordRetrieval
//        {
//            get { return false; }
//        }
//    }
//}