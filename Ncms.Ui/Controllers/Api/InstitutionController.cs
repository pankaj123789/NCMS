using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Institution;
using Ncms.Contracts.Models.Person;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/institution")]
    public class InstitutionController : BaseApiController
    {
        private readonly IInstitutionService _institutionService;
        private readonly IContactPersonService _contactPersonService;
        private readonly IPersonService _personService;

        public InstitutionController(IInstitutionService institutionService, IContactPersonService contactPersonService, IPersonService personService)
        {
            _institutionService = institutionService;
            _contactPersonService = contactPersonService;
            _personService = personService;
        }

        [HttpGet]
        [Route("contactPerson/{institutionId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Contact)]
        public HttpResponseMessage GetContactPersons(int institutionId)
        {
            return this.CreateResponse(_contactPersonService.GetContactPersons(institutionId));
        }

        [HttpGet]
        [Route("contactPersonByNaatiNo/{naatiNo}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Contact)]
        public HttpResponseMessage GetContactPersonsByNaatiNumber(int naatiNo)
        {
            return this.CreateResponse(_contactPersonService.GetContactPersonsByNaatiNumber(naatiNo));
        }

        [HttpGet]
        [Route("contactPersonById/{contactPersonId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Contact)]
        public HttpResponseMessage GetContactPersonsById(int contactPersonId)
        {
            return this.CreateResponse(_contactPersonService.GetContactPersonsById(contactPersonId));
        }

        [HttpPost]
        [Route("inactive")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Contact)]
        public HttpResponseMessage InactiveContactPerson(dynamic request)
        {
            try
            {
                return this.CreateResponse(_contactPersonService.SetContactPersonInactive((int)request.contactPersonId));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Contact)]
        public HttpResponseMessage AddContactPerson(dynamic request)
        {
            try
            {
                var contactPerson = new ContactPersonModel
                {
                    InstitutionId = request.InstitutionId ?? 0,
                    PostalAddress = request.PostalAddress,
                    Phone = request.Phone,
                    Name = request.Name,
                    Email = request.Email,
                    Description = request.Description
                };

                return this.CreateResponse(_contactPersonService.InsertContactPerson(contactPerson));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("createInstitution")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Organisation)]
        public HttpResponseMessage CreateInstitution(dynamic request)
        {
            try
            {
                var institution = new InstitutionModel()
                {
                    Name = request.Name.ToString().Trim(),
                    NaatiNumber =
                        request.NaatiNumber != null
                            ? Regex.IsMatch(request.NaatiNumber.ToString(), @"^\d+$")
                                ? (int)request.NaatiNumber
                                : 0
                            : 0
                };

                return this.CreateResponse(_institutionService.InsertInstitution(institution));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("checkDuplicatedInstitution")]
        public HttpResponseMessage CheckDuplicatedInstitution(dynamic request)
        {
            try
            {
                var institution = new InstitutionModel()
                {
                    Name = request.Name,
                    NaatiNumber =
                        request.NaatiNumber != null
                            ? Regex.IsMatch(request.NaatiNumber.ToString(), @"^\d+$")
                                ? (int)request.NaatiNumber
                                : 0
                            : 0
                };

                return this.CreateResponse(_institutionService.CheckDuplicatedInstitution(institution));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route("update")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Contact)]
        public HttpResponseMessage UpdateContactPerson(dynamic request)
        {
            try
            {
                var contactPerson = new ContactPersonModel
                {
                    ContactPersonId = request.ContactPersonId,
                    InstitutionId = request.InstitutionId ?? 0,
                    PostalAddress = request.PostalAddress,
                    Phone = request.Phone,
                    Name = request.Name,
                    Email = request.Email,
                    Description = request.Description
                };

                return this.CreateResponse(_contactPersonService.UpdateContactPerson(contactPerson));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpGet]
        [Route("{naatiNumber}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        public HttpResponseMessage GetInstitution(string naatiNumber)
        {
            int naatiNum;
            if (!int.TryParse(naatiNumber, out naatiNum))
            {
                return this.FailureResponse("Please enter a valid NAATI Number.");
            }

            return this.CreateResponse(_institutionService.GetInstitution(naatiNum));
        }

        [HttpPost]
        [Route("settings")]
        [NcmsAuthorize(SecurityVerbName.Configure, SecurityNounName.Organisation)]
        public HttpResponseMessage UpdateInstitution(dynamic request)
        {
            try
            {
                var model = new InstitutionModel
                {
                    InstitutionId = (int)request.InstitutionId,
                    NaatiNumber = (int)request.NaatiNumber,
                    TrustedPayer = (bool)request.TrustedPayer,
                };
                return this.CreateResponse(_institutionService.UpdateInstitution(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        //Regional Manager cant add name so putting this under manage
        [Route("{naatiNumber}/addname")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.Organisation)]
        public HttpResponseMessage AddName(dynamic request)
        {
            try
            {
                var model = new InstitutionModel
                {
                    InstitutionId = (int)request.InstitutionId,
                    NaatiNumber = (int)request.NaatiNumber,
                    Name = (string)request.Name,
                    AbbreviatedName = (string)request.AbbreviatedName
                };
                return this.CreateResponse(_institutionService.AddName(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }


        [HttpGet]
        [Route("list")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Organisation)]
        public HttpResponseMessage GetList([FromUri] InstitutionSearchRequest request)
        {
            var results = _institutionService.Search(request);
            return this.CreateResponse(() => results);
        }


        [HttpGet]
        [Route("searchEndorsedQualifications")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Organisation)]
        public HttpResponseMessage SearchQualifications([FromUri] EndorsedQualificationSearchRequest request)
        {
            var results = _institutionService.SearchQualifications(request);
            return this.CreateResponse(() => results);
        }

        [HttpPost]
        [NcmsAuthorize(new[] { SecurityVerbName.Create, SecurityVerbName.Update }, new[] { SecurityNounName.EndorsedQualification, SecurityNounName.EndorsedQualification })]
        [Route("createOrUpdateQualification")]
        public HttpResponseMessage CreateOrUpdateQualification(EndorsedQualificationRequest model)
        {
            return this.CreateResponse(() => _institutionService.CreateOrUpdateQualification(model));
        }

        [HttpGet]
        [Route("getEndorsedQualification/{id}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        public HttpResponseMessage GetEndorsedQualification(int id)
        {
            return this.CreateResponse(() => _institutionService.GetEndorsedQualification(id));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("address")]
        public HttpResponseMessage UpdateAddress(AddressModel model)
        {
            return this.CreateResponse(() => _personService.UpdateAddress(model));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("phone")]
        public HttpResponseMessage UpdatePhone(PhoneModel model)
        {
            return this.CreateResponse(() => _personService.UpdatePhone(model));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("email")]
        public HttpResponseMessage UpdateEmail(EmailModel model)
        {
            return this.CreateResponse(() => _personService.UpdateEmail(model));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("website")]
        public HttpResponseMessage UpdateWebsite(WebsiteModel model)
        {
            return this.CreateResponse(() => _personService.UpdateWebsite(model));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("deleteAddress")]
        public HttpResponseMessage DeleteAddress([FromBody] int addressId)
        {
            return this.CreateResponse(() => _personService.DeleteAddress(addressId));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("deletePhone")]
        public HttpResponseMessage DeletePhone([FromBody]DeleteRequestModel request)
        {
            return this.CreateResponse(() => _personService.DeletePhone(request));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("deleteEmail")]
        public HttpResponseMessage DeleteEmail([FromBody]DeleteRequestModel request)
        {
            return this.CreateResponse(() => _personService.DeleteEmail(request));
        }

        [HttpPost]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Organisation)]
        [Route("deleteWebsite")]
        public HttpResponseMessage DeleteWebsite([FromBody] int entityId)
        {
            return this.CreateResponse(() => _personService.DeleteWebsite(entityId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("{entityId}/contactdetails")]
        public HttpResponseMessage GetContactDetails(int entityId)
        {
            return this.CreateResponse(() => _personService.GetContactDetails(entityId));
        }

        [HttpGet]
        [Route("checkExaminerRole/{entityId}")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Examiner)]
        public HttpResponseMessage CheckExaminerRole(int entityId)
        {
            return this.CreateResponse(_personService.CheckExaminerRole(entityId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("suburbs")]
        public HttpResponseMessage GetSuburbs()
        {
            return this.CreateResponse(() => _personService.GetSuburbs());
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("{entityId}/address/{addressId}")]
        public HttpResponseMessage GetAddress(int entityId, int addressId)
        {
            return this.CreateResponse(() => _personService.GetAddress(entityId, addressId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("{entityId}/email/{emailId}")]
        public HttpResponseMessage GetEmail(int entityId, int emailId)
        {
            return this.CreateResponse(() => _personService.GetEmail(entityId, emailId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("{entityId}/primaryPhone")]
        public HttpResponseMessage GetPrimaryPhone(int entityId)
        {
            return this.CreateResponse(() => _personService.GetPersonPrimaryPhone(entityId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("{entityId}/phone/{phoneId}")]
        public HttpResponseMessage GetPhone(int entityId, int phoneId)
        {
            return this.CreateResponse(() => _personService.GetPhone(entityId, phoneId));
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Organisation)]
        [Route("{entityId}/website")]
        public HttpResponseMessage GetWebsite(int entityId)
        {
            return this.CreateResponse(() => _personService.GetWebsite(entityId));
        }
    }

}
