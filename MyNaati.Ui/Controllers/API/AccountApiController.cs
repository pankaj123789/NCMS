using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.Models;
using MyNaati.Ui.UI;

namespace MyNaati.Ui.Controllers.API
{
    public class AccountApiController : BaseApiController
    {
        private readonly IMembershipProviderService mMembershipService;
        private readonly IFormsAuthenticationService mFormsService;
        private readonly IExaminerHelper mExaminerHelper;
        private readonly IPersonalDetailsService mPersonalDetailsService;

        public AccountApiController(IFormsAuthenticationService formsService, IMembershipProviderService membershipService, IExaminerHelper examinerHelper, IPersonalDetailsService personalDetailsService)
        {
            mFormsService = formsService;
            mMembershipService = membershipService;
            mExaminerHelper = examinerHelper;
            mPersonalDetailsService = personalDetailsService;
        }

        [HttpPost]
        public HttpResponseMessage LogOn(LogOnModel model)
        {
            return ResponseMessage(() =>
            {
                var naatiNumber = User.NaatiNumber();
                    if (!ModelState.IsValid)
                    {
                        return GetErrors();
                    }

                    if (mMembershipService.ValidateUser(model.UserName, model.Password))
                    {
                        mFormsService.SignIn(model.UserName, model.RememberMe);
                        mExaminerHelper.LoadExaminerRoles(model.UserName, naatiNumber);
                        mFormsService.LoadPractitioner(model.UserName, naatiNumber);

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                    model.CredentialsRejected = true;
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                });
        }
    }

}