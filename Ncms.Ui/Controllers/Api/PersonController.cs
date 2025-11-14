using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Person;
using Ncms.Contracts.Models.User;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;
using IPersonService = Ncms.Contracts.IPersonService;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/person")]
	public class PersonController : BaseApiController
	{
		private readonly INoteService _noteService;
		private readonly IPersonService _personService;
		private readonly IEmailMessageService _emailMessageService;
	    private readonly IUserService _userService;
	    private readonly ITestSessionService _testSessionService;
        private readonly ISecretsCacheQueryService _secretsProvider;


        public PersonController(IPersonService person, INoteService note, IEmailMessageService emailMessageService, IUserService userService, ITestSessionService testSessionService, ISecretsCacheQueryService secretsProvider)
		{
			_personService = person;
			_noteService = note;
			_emailMessageService = emailMessageService;
		    _userService = userService;
		    _testSessionService = testSessionService;
            _secretsProvider = secretsProvider;

        }

		[NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Person)]
		public HttpResponseMessage Get([FromUri] QueryRequest request)
		{
			return this.CreateResponse(() => _personService.PersonSearch(request));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Person)]
		[Route("search")]
		public HttpResponseMessage GetSearch([FromUri] EntitySearchRequest request)
		{
			return this.CreateResponse(() => _personService.PersonAndAndInstitutionSearch(request));
		}
		
	

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Person)]
		[Route("create")]
		public HttpResponseMessage CreatePerson([FromBody]CreatePersonModel model)
		{
			return this.CreateResponse(() => _personService.CreatePerson(model));
		}

        [HttpPost]
        [Route("soft-delete")]

        // --- THIS IS THE FIX ---
        // Add this attribute to lock the endpoint to only the roles you listed.
        // We use the internal names from SecurityRoleScriptGenerator.cs
        //[Authorize(Roles = "SystemAdministrator,SeniorManagement,GeneralAccount")]
        // ---------------------

        public IHttpActionResult SoftDeletePerson([FromBody] DeletePersonRequestModel request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var currentAdminUser = _userService.Get();
                if (currentAdminUser == null)
                {
                    // This will be caught by [Authorize] first, but it's good practice.
                    return Unauthorized();
                }

                request.DeletedBy = currentAdminUser.DomainName;

                var response = _personService.SoftDeletePerson(request);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Errors.FirstOrDefault() ?? "An error occurred.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return InternalServerError(new Exception("An unexpected error occurred during the soft delete process."));
            }
        }

        [HttpPost]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("check")]
		public HttpResponseMessage CheckPerson([FromBody]PersonCheckModel model)
		{
			return this.CreateResponse(() => _personService.CheckPerson(model));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{naatiNumber}")]
		public HttpResponseMessage Get(string naatiNumber)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid NAATI Number.");
			}

			return this.CreateResponse(_personService.GetPerson(naatiNum));
		}

		[HttpGet]
		[Route("{naatiNumber}/emails")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		public HttpResponseMessage GetEmails(int naatiNumber)
		{
			var list = _secretsProvider.Get("SharedAccounts").Split(' ').ToList();
			return this.CreateResponse(() => _emailMessageService.GetMailMessagesFromSharedAccount(list, naatiNumber));
		}

	    [HttpPost]
	    [Route("graphEmailDetails")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Email)]
        public HttpResponseMessage GetEmails(dynamic email)
	    {
	        var graphEmailId = (string)email.GraphEmailId;
            var mailBox = (string)email.MailBox;
            return this.CreateResponse(() => _emailMessageService.GetEmailDetails(graphEmailId, mailBox));
           
	    }

        [HttpGet]
		[Route("checkExaminerRole/{entityId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Examiner)]
        public HttpResponseMessage CheckExaminerRole(int entityId)
		{
			return this.CreateResponse(_personService.CheckExaminerRole(entityId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("entity/{naatiNumber}")]
		public HttpResponseMessage GetEntity(string naatiNumber)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetEntity(naatiNum));
		}

	    [HttpGet]
	    [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
	    [Route("mynaatidetails/{username}")]
	    public HttpResponseMessage GetMyNaatiDetails(string username)
	    {
	        return this.CreateResponse(_personService.GetMyNaatiDetails(username));
	    }

	    [HttpPost]
	    [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.PersonMyNaatiRegistration)]
        [Route("unlockMyNaatiUser")]
	    public HttpResponseMessage UnlockMyNaatiUser(MyNaatiUserModel user)
	    {
	        return this.CreateResponse(_personService.UnlockMyNaatiUser(user.UserName));
	    }


	    [HttpPost]
	    [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.PersonMyNaatiRegistration)]
	    [Route("deleteMyNaatiUser")]
	    public HttpResponseMessage DeleteMyNaatiUser(MyNaatiUserModel user)
	    {
	        return this.CreateResponse(_personService.DeleteMyNaatiUser(user.UserName));
	    }

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.PersonMyNaatiRegistration)]
		[Route("deleteMfaAccount")]
		public HttpResponseMessage DeleteMfaAccount(MyNaatiUserModel user)
		{
			return this.CreateResponse(_personService.DeleteMfaAccount(user.NaatiNumber));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{entityId}/contactdetails")]
		public HttpResponseMessage GetContactDetails(int entityId)
		{
			return this.CreateResponse(() => _personService.GetContactDetails(entityId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("certificationperiods")]
		public HttpResponseMessage GetCertificationPeriods([FromUri]GetCertificationPeriodsRequestModel request)
		{
			return this.CreateResponse(() => _personService.GetCertificationPeriods(request));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Configure, SecurityNounName.Credential)]
		[Route("credentialterminatedate")]
		public HttpResponseMessage PostCredentialterminatedate(SetCredentialTerminateDateModel request)
		{
			return this.CreateResponse(() => _personService.SetCredentialTerminateDate(request));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.CertificationPeriod)]
		[Route("extendcertification")]
		public HttpResponseMessage PostExtendcertification(SetCertificationEndDateRequestModel request)
		{
			return this.CreateResponse(() => _personService.SetCertificationEndDate(request));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.CertificationPeriod)]
		[Route("certificationperiod")]
		public HttpResponseMessage PostCertificationPeriod(CertificationPeriodModel request)
		{
			return this.CreateResponse(() => _personService.SaveCertificationPeriod(request));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.CertificationPeriod)]
		[Route("checkcertificationperiod")]
		public HttpResponseMessage PostCheckCertificationPeriod(CertificationPeriodModel request)
		{
			return this.CreateResponse(() => _personService.CheckCertificationPeriod(request));
		}

	    [HttpPost]
	    [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.CertificationPeriod)]
	    [Route("RecalculateCertificationPeriodStartDate")]
	    public HttpResponseMessage PostRecalculateCertificationPeriodStartDate([FromBody]int periodId)
	    {
	        return this.CreateResponse(() => _personService.RecalculateCertificationPeriodStartDate(periodId));
	    }

        [HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{entityId}/address/{addressId}")]
		public HttpResponseMessage GetAddress(int entityId, int addressId)
		{
			return this.CreateResponse(() => _personService.GetAddress(entityId, addressId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{entityId}/phone/{phoneId}")]
		public HttpResponseMessage GetPhone(int entityId, int phoneId)
		{
			return this.CreateResponse(() => _personService.GetPhone(entityId, phoneId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{entityId}/primaryPhone")]
		public HttpResponseMessage GetPrimaryPhone(int entityId)
		{
			return this.CreateResponse(() => _personService.GetPersonPrimaryPhone(entityId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{entityId}/email/{emailId}")]
		public HttpResponseMessage GetEmail(int entityId, int emailId)
		{
			return this.CreateResponse(() => _personService.GetEmail(entityId, emailId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{entityId}/website")]
		public HttpResponseMessage GetWebsite(int entityId)
		{
			return this.CreateResponse(() => _personService.GetWebsite(entityId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("suburbs")]
		public HttpResponseMessage GetSuburbs()
		{
			return this.CreateResponse(() => _personService.GetSuburbs());
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{naatiNumber}/settings")]
		public HttpResponseMessage Settings(string naatiNumber)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetPerson(naatiNum));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Logbook)]
		[Route("{naatiNumber}/logbook/{showAll}")]
		public HttpResponseMessage Logbook(string naatiNumber, bool showAll)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetLogBook(naatiNum, showAll));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{naatiNumber}/activities/{certificationPeriodId}")]
		public HttpResponseMessage GetProfessionalDevelopmentActivities(string naatiNumber, int certificationPeriodId)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetProfessionalDevelopmentActivities(naatiNum, certificationPeriodId));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{naatiNumber}/activitypoints/{certificationPeriodId}")]
		public HttpResponseMessage GetProfessionalDevelopmentActivityPoints(string naatiNumber, int certificationPeriodId = 0)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetProfessionalDevelopmentActivityPoints(naatiNum, certificationPeriodId));
		}


		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("certificationPeriodsDetails/{naatiNumber}")]
		public HttpResponseMessage GetCertificaitonPeriods(string naatiNumber)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetCertificationPeriodDetails(naatiNum));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("{naatiNumber}/certificationPeriodsRequests")]
		public HttpResponseMessage GetCertificationPeriodsRequests(string naatiNumber)
		{
			int naatiNum;
			if (!Int32.TryParse(naatiNumber, out naatiNum))
			{
				return this.FailureResponse("Please enter a valid Customer Number.");
			}

			return this.CreateResponse(_personService.GetCertificationPeriodsRequests(naatiNum));
		}

		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("requirements")]
		public HttpResponseMessage GetProfessionalDevelopmentRequirements(int categoryId)
		{
			return this.CreateResponse(_personService.GetProfessionalDevelopmentRequirements(categoryId));
		}


		[HttpGet]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		[Route("categories")]
		public HttpResponseMessage GetProfessionalDevelopmentCategories()
		{
			return this.CreateResponse(_personService.GetProfessionalDevelopmentCategories());
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		public HttpResponseMessage UpdateDetails(PersonModel model)
		{
			return this.CreateResponse(() => _personService.UpdateDetails(model));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("settings")]
		public HttpResponseMessage UpdateSettings(PersonModel model)
		{
			return this.CreateResponse(() => _personService.UpdateSettings(model));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("address")]
		public HttpResponseMessage UpdateAddress(AddressModel model)
		{
			return this.CreateResponse(() => _personService.UpdateAddress(model));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("phone")]
		public HttpResponseMessage UpdatePhone(PhoneModel model)
		{
			return this.CreateResponse(() => _personService.UpdatePhone(model));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("email")]
		public HttpResponseMessage UpdateEmail(EmailModel model)
		{
			return this.CreateResponse(() => _personService.UpdateEmail(model));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("website")]
		public HttpResponseMessage UpdateWebsite(WebsiteModel model)
		{
			return this.CreateResponse(() => _personService.UpdateWebsite(model));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("deleteAddress")]
		public HttpResponseMessage DeleteAddress([FromBody] int addressId)
		{
			return this.CreateResponse(() => _personService.DeleteAddress(addressId));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("deletePhone")]
		public HttpResponseMessage DeletePhone([FromBody]DeleteRequestModel request)
		{
			return this.CreateResponse(() => _personService.DeletePhone(request));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("deleteEmail")]
		public HttpResponseMessage DeleteEmail([FromBody]DeleteRequestModel request)
		{
			return this.CreateResponse(() => _personService.DeleteEmail(request));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Person)]
		[Route("deleteWebsite")]
		public HttpResponseMessage DeleteWebsite([FromBody] int entityId)
		{
			return this.CreateResponse(() => _personService.DeleteWebsite(entityId));
		}

		[HttpPost]
		[NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Person)]
		[Route("{naatiNumber}/addName")]
		public HttpResponseMessage PostAddName([FromUri]int naatiNumber, [FromBody]PersonNameModel name)
		{
			return this.CreateResponse(() => _personService.AddName(naatiNumber, name));
		}

		[HttpGet]
		[Route("list")]
		[NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Person)]
		//Maybe merge with public HttpResponseMessage Get([FromUri] QueryRequest request)???
		public HttpResponseMessage GetList([FromUri] PersonSearchRequest request)
		{
			return this.CreateResponse(() => _personService.Search(request));
		}

        [HttpGet]
		[Route("{personId}/detail")]
		[NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Person)]
		public HttpResponseMessage GetDetail(int personId)
		{
			return this.CreateResponse(() => _personService.GetPersonCredentialRequests(personId));
		}

		[HttpGet]
		[Route("{naatiNumber}/summary")]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		public HttpResponseMessage GetSummary(int naatiNumber)
		{
			return this.CreateResponse(() => _personService.GetPersonSummary(naatiNumber));
		}

		[HttpGet]
		[Route("correspondence")]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		public HttpResponseMessage Getcorrespondence(int naatiNumber)
		{

			var list = ConfigurationManager.AppSettings["SharedAccount"].Split(',').ToList();
			return this.CreateResponse(() => _emailMessageService.GetMailMessagesFromSharedAccount(list, naatiNumber));
		}


		[HttpPost]
		[Route("graphEmailAttachments")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Email)]
        public HttpResponseMessage GraphEmailAttachments(dynamic email)
		{
			try
			{
				var graphEmailId = (string)email.GraphEmailId;
				var mailBox = (string)email.MailBox;
                return this.CreateResponse(() => _emailMessageService.GetMailAttachment(graphEmailId, mailBox));
			}
			catch (Exception ex)
			{
				return this.FailureResponse(ex);
			}
		}

		[HttpGet]
		[Route("allcredentialrequests")]
		[NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
		public HttpResponseMessage GetCredentials(int naatiNumber)
		{
			return this.CreateResponse(() => _personService.GetPersonCredentials(naatiNumber));
		}

	    [HttpGet]
	    [Route("allRolePlayRequests")]
	    [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Person)]
	    public HttpResponseMessage GetRolePlays(int naatiNumber)
	    {
	        return this.CreateResponse(() => _testSessionService.GetPersonRolePlays(naatiNumber));
	    }

        [HttpPost]
        [Route("documents/upload")]
        [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.Document)]
        public async Task<HttpResponseMessage> Upload()
	    {
	        return await this.ProcessMultipartFileData((fileName, fileData, provider) =>
			{
				var personId = Convert.ToInt32(provider.FormData["personId"]);
	            var id = Convert.ToInt32(provider.FormData["id"]);
	            var type = provider.FormData["type"];
	            var title = provider.FormData["title"];
	            var storedFileId = Convert.ToInt32(provider.FormData["storedFileId"]);

	            var user = _userService.Get() ?? new UserModel();
	            var request = new PersonAttachmentModel
	            {
	                FileName = fileName,
	                Type = type,
	                FilePath = fileData.LocalFileName,
	                UploadedByUserId = user.Id,
	                StoragePath = $@"{type}\{personId}\{fileName}",
	                StoredFileId = storedFileId,
	                PersonId = personId,
	                PersonAttachmentId = id,
	                Title = title
	            };

	            return _personService.CreateOrReplaceAttachment(request);
	        });

	    }

	    [HttpGet]
	    [Route("documents/{id}")]
	    [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Document)]
        public HttpResponseMessage DocumentsGet([FromUri]int id)
	    {
	        return this.CreateSearchResponse(() =>
	        {
	            return _personService.ListAttachments(id);
	        });
	    }

	    [HttpGet]
	    [Route("documentTypesToUpload")]
        //view person
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.General)]
        public HttpResponseMessage GetDocumentTypes()
	    {
	        return this.CreateResponse(() => _personService.GetDocumentTypesToUpload());
	    }

	    [HttpPost]
	    [Route("documents")]
	    [NcmsAuthorize(SecurityVerbName.Upload, SecurityNounName.Person)]
        public HttpResponseMessage DocumentsPost(PersonAttachmentModel request)
	    {
	        var user = _userService.Get() ?? new UserModel();
	        request.UploadedByUserId = user.Id;
	        return this.CreateResponse(() => _personService.CreateOrReplaceAttachment(request));
	    }

	    [HttpDelete]
	    [Route("documents/{id}")]
	    [NcmsAuthorize(SecurityVerbName.Delete, SecurityNounName.Document)]
        public HttpResponseMessage DocumentsDelete([FromUri]int id)
	    {
	        return this.CreateResponse(() => _personService.DeleteAttachment(id));
	    }

		[HttpGet]
		[Route("qrCodeSummary")]
		public HttpResponseMessage QrCodeSummary(int personId)
		{
			return this.CreateResponse(() => _personService.GetQrCodeSummary(personId));
		}

		[HttpGet]
		[Route("qrCodes")]
		public HttpResponseMessage QrCodes(int naatiNumber)
		{
			return this.CreateResponse(() => _personService.GetQrCodes(naatiNumber));
		}

		[HttpGet]
		[Route("toggleQrCode")]
		public HttpResponseMessage ToggleQrCode(int naatiNumber, Guid qrCode)
		{
			var response = _personService.ToggleQrCode(qrCode);
			if(!response.Success)
            {
				return this.CreateResponse(response);
			}
			return this.CreateResponse(() => _personService.GetQrCodes(naatiNumber));
		}
	}
}
