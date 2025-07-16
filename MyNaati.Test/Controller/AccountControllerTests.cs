using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Security;
using MyNaati.Contracts.BackgroundTask;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.Controllers;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.Models;
using MyNaati.Ui.Reports.OnlineOrderDetails;
using NSubstitute;
using Xunit;

namespace MyNaati.Test.Controller
{
    public class AccountControllerTests
    {
        private readonly IConfigurationService mConfigService = Substitute.For<IConfigurationService>();
        private readonly IEmailService mEmailService = Substitute.For<IEmailService>();
      
        private readonly IExaminerHelper mExamineHelpers = Substitute.For<IExaminerHelper>();
        private readonly ILookupProvider mLookup = Substitute.For<ILookupProvider>();
        private readonly IMembershipProviderService mMembershipService = Substitute.For<IMembershipProviderService>();
        private readonly IFormsAuthenticationService mFormsService = Substitute.For<IFormsAuthenticationService>();
        private readonly IPasswordService mPasswordService = Substitute.For<IPasswordService>();

        private readonly IPersonalDetailsService mPersonService = Substitute.For<IPersonalDetailsService>();
        private readonly IEmailCodeVerificationService mEmailCodeVerificationService = Substitute.For<IEmailCodeVerificationService>();


        private readonly ICredentialApplicationService mCredentialApplicationService =
            Substitute.For<ICredentialApplicationService>();

        private readonly IRegisterHelper mRegisterHelper = Substitute.For<IRegisterHelper>();

        private readonly IUserService mUserService = Substitute.For<IUserService>();

        [Fact]
        public void When_ChangePasswordWithExistedPasswordHistory_ReturnViewWithModelStateError()
        {
            var controllerContext = Substitute.For<ControllerContext>();
            controllerContext.HttpContext.User.Identity.Name.Returns("example");

            var controller = new AccountController(mFormsService, mMembershipService, mPersonService,
                mEmailService, mUserService,
                mExamineHelpers, mRegisterHelper, mLookup, mConfigService, mPasswordService,
                mCredentialApplicationService, mEmailCodeVerificationService);
            controller.ControllerContext = controllerContext;
            mMembershipService.GetUser(Arg.Any<string>(), Arg.Any<bool>())
                .Returns(new ePortalUser {UserId = new Guid()});
            mPasswordService.ExistedPasswordHistory(Arg.Any<PasswordHistoryRequest>()).Returns(true);
            var result =
                controller.ChangePassword(new ChangePasswordModel
                {
                    ConfirmPassword = "",
                    NewPassword = "",
                    OldPassword = ""
                });

            Assert.True(controller.ModelState.Count > 0);
            Assert.Contains(controller.ModelState.Keys, x => x.Equals("NewPassword"));
        }

        //this cannot be done because of service locator need to refactor
        //[Fact]
        //public void When_ChangePasswordSuccessful_SavePasswordHistory()
        //{
        //    var controllerContext = Substitute.For<ControllerContext>();
        //    controllerContext.HttpContext.User.Identity.Name.Returns("example");

        //    var controller = new AccountController(mFormsService, mMembershipService, mPersonService,
        //        mEmailService, mUserService, mEportalService, mUserCsvGenerator, mUserRequestGenerator,
        //        mExamineHelpers, mRegisterHelper, mLookup, mConfigService, mPasswordService);
        //    controller.ControllerContext = controllerContext;

        //    mMembershipService.GetUser(Arg.Any<string>(), Arg.Any<bool>())
        //        .Returns(new ePortalUser { UserId = new Guid() });
        //    mMembershipService.ChangePassword(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        //    mPasswordService.ExistedPasswordHistory(Arg.Any<PasswordHistoryRequest>()).Returns(false);

        //   var result =
        //        controller.ChangePassword(new ChangePasswordModel
        //        {
        //            ConfirmPassword = "",
        //            NewPassword = "",
        //            OldPassword = ""
        //        });

        //    mPasswordService.ReceivedWithAnyArgs(1).SavePasswordHistory(Arg.Any<PasswordHistoryRequest>());

        //}

        public static IEnumerable<object[]> ChangePasswordModel
            => new[]
            {
                new object[] {new ChangePasswordModel {ConfirmPassword = "", NewPassword = "", OldPassword = ""}},
                new object[] {new ChangePasswordModel {ConfirmPassword = "", NewPassword = "abc", OldPassword = "cba"}},
                new object[]
                {
                    new ChangePasswordModel
                    {
                        ConfirmPassword = "",
                        NewPassword = "abc123!!!!",
                        OldPassword = "cba123!!!!"
                    }
                },
                new object[]
                {
                    new ChangePasswordModel
                    {
                        ConfirmPassword = "",
                        NewPassword = "abc123123123",
                        OldPassword = "abc123123123"
                    }
                },
                new object[]
                {
                    new ChangePasswordModel
                    {
                        ConfirmPassword = "",
                        NewPassword = "!!!123123123",
                        OldPassword = "!!!123123123"
                    }
                },
                new object[] {new ChangePasswordModel {ConfirmPassword = "abc", NewPassword = "", OldPassword = ""}},
                new object[]
                    {new ChangePasswordModel {ConfirmPassword = "abc", NewPassword = "abc", OldPassword = "cba"}},
                new object[]
                {
                    new ChangePasswordModel
                    {
                        ConfirmPassword = "abc",
                        NewPassword = "abc123!!!!",
                        OldPassword = "cba123!!!!"
                    }
                },
                new object[]
                {
                    new ChangePasswordModel
                    {
                        ConfirmPassword = "abc",
                        NewPassword = "abc123123123",
                        OldPassword = "abc123123123"
                    }
                },
                new object[]
                {
                    new ChangePasswordModel
                    {
                        ConfirmPassword = "abc",
                        NewPassword = "!!!123123123",
                        OldPassword = "!!!123123123"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(ChangePasswordModel))]
        public void When_ChangePassword_ModelStateIsInValid(ChangePasswordModel model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            Assert.True(validationResults.Count > 0);
        }

        [Fact(Skip = "Integration test. Callse portal")]
        public void WhenConcurrentUsersRequestToCreateAnAccount_ThenAccountsAreCreated()
        {

            var models = new List<FormUrlEncodedContent>();
            for (var i = 90; i < 1000; i++)
            {
                var email = "newTestEmail" + i +"Batch"+ DateTime.Now.Ticks + "@altf4solutions.com.au";
                var data = new Dictionary<string, string>()
                {
                    { "Email", email},
                    { "ConfirmEmail", email},
                    { "GivenName", email},
                    { "GivenDateOfBirthName",  DateTime.Now.AddDays(-10).ToString("dd/mm/yy")},
                    { "DetailsRequired",  "true"},
                 
                };

                models.Add(new FormUrlEncodedContent(data));
            }
         
            var tasks = new List<Task<HttpResponseMessage>>();

            foreach (var model in models)
            {
                var client = new HttpClient();
                tasks.Add(client.PostAsync("http://localhost/MyNaati.Web/Account/Register", model));
            }


            Task.WaitAll(tasks.ToArray());


            foreach (var task in tasks)
            {
              var response =  task.Result.Content.ReadAsStringAsync().Result;
              Assert.Contains("You have been successfully registered to use", response);  
            }
        }
    }
}