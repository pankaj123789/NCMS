using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.Credentials;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.ViewModels.Credential;
using MyNaati.Ui.ViewModels.PersonalDetails;

namespace MyNaati.Ui.Controllers
{
    [Authorize]
    public class PersonalDetailsController : BaseController
    {
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly ICredentialApplicationService mCredentialApplicationService;
        private readonly ICredentialQrCodeService mCredentialQrCodeService;
        private readonly ILookupProvider mLookupProvider;
        private readonly IRegisterHelper mRegisterHelper;
        private readonly IAutoMapperHelper _autoMapperHelper;

        private const int AUSTRALIA_COUNTRY_ID = 13;
        
        public PersonalDetailsController(IPersonalDetailsService personalDetailsService, ILookupProvider lookupProvider,
            IRegisterHelper registerHelper, ICredentialApplicationService credentialApplicationService, ICredentialQrCodeService credentialQrCodeService, IAutoMapperHelper autoMapperHelper)
        {
            mPersonalDetailsService = personalDetailsService;
            mCredentialApplicationService = credentialApplicationService;
            mCredentialQrCodeService = credentialQrCodeService;
            mLookupProvider = lookupProvider;
            mRegisterHelper = registerHelper;
            _autoMapperHelper = autoMapperHelper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var personalDetailsModel = new PersonalDetailsModel();

            if (CurrentUserNaatiNumber > 0)
            {
                var request = new PersonNaatiNumberRequest
                {
                    NaatiNumber = CurrentUserNaatiNumber
                };

                var lastUpdatedResponse = mPersonalDetailsService.GetLastUpdated(request);
                var personDetails = mPersonalDetailsService.GetPerson(request);
                personalDetailsModel.LastUpdated = lastUpdatedResponse.LastUpdated;
                personalDetailsModel.NaatiNumber = personDetails.Person.NaatiNumber;
                personalDetailsModel.IsPractitioner = personDetails.Person.IsPractitioner;
                personalDetailsModel.IsFuturePractitioner = personDetails.Person.IsFuturePractitioner;
                personalDetailsModel.IsFormerPractitioner = personDetails.Person.IsFormerPractitioner;
                personalDetailsModel.IsExaminer = personDetails.Person.IsExaminer;
                personalDetailsModel.PopulateLookups(mLookupProvider);
            }

            var allCredentials = mCredentialApplicationService.GetAllCredentialsByNaatiNumber(CurrentUserNaatiNumber);

            var allCredentialDetailList = allCredentials.Results.Select(_autoMapperHelper.Mapper.Map<CredentialModel>).ToList();

            var mfaConfigured = GetMfaConfigured();

            var credentialDetailRequestModel = new CredentialDetailRequestModel
            {
                Credentials = allCredentialDetailList,
                MfaConfigured = mfaConfigured
            };

            personalDetailsModel.CredentialDetailRequest = credentialDetailRequestModel;

            return View(personalDetailsModel);
        }

        private class JsonUpdateResponse
        {
            public bool Success { get; set; }
            public IList<string> Errors { get; set; }
        }

        public class GeoLocation
        {
            public decimal Lat { get; set; }
            public decimal Lng { get; set; }
        }

        public class GeoGeometry
        {
            public GeoLocation Location { get; set; }
        }

        public class GeoComponents
        {
            public string Long_Name { get; set; }
            public string Short_Name { get; set; }
            public List<string> Types { get; set; }
        }

        public class GeoResult
        {
            public GeoGeometry Geometry { get; set; }
            public string Formatted_Address { get; set; }
            public List<string> Types { get; set; }
            public List<GeoComponents> Address_Components { get; set; }
        }

        public class GeoResponse
        {
            public string error_message { get; set; }
            public string ErrorMessage => error_message;
            public string Status { get; set; }
            public GeoResult[] Results { get; set; }
        }

        

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAddress(string addressText)
        {
            //return a list of formatted addresses
            var results = Helpers.Common.GetGoogleMapResults(addressText);

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddressCreateJson(AddressEditGoogleModel model)
        {
            return UpdateAddress(model);
        }

        [HttpPost]
        public ActionResult SetOdEmailPreference(int id, bool include)
        {
            var result = mPersonalDetailsService.SetOdEmailVisibility(id, include);
            var success = result.Success && result.Data;
            return Json(success);
        }

        [HttpPost]
        public ActionResult SetOdPhonePreference(int id, bool include)
        {
            var result = mPersonalDetailsService.SetOdPhoneVisibility(id, include);
            var success = result.Success && result.Data;
            return Json(success);
        }

        [HttpPost]
        public ActionResult SetOdAddressPreference(int id, int selectedIndex)
        {
            var result = mPersonalDetailsService.SetOdAddressVisibilityTypeId(id, selectedIndex);
            var success = result.Success && result.Data;
            return Json(success);
        }

        [HttpGet]
        // The method that returns JSON so that we can populate the view with data
        public ActionResult AddressEditJson(int id)
        {
            if (!mPersonalDetailsService.IsValidAddress(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }

            var lookupProvider = ServiceLocator.Resolve<ILookupProvider>();
            var model = new AddressEditGoogleModel();
            var request = new PersonalDetailsGetAddressRequest
            {
                AddressId = id
            };

            var response = mPersonalDetailsService.GetAddress(request);
            var countries = lookupProvider.Countries.Where(c => c.SamId == response.Address.CountryId);

            if (response.Address != null)
            {
                model.Id = response.Address.AddressId;
                model.IsFromAustralia = response.Address.CountryId == AUSTRALIA_COUNTRY_ID;
                model.IsPreferred = response.Address.IsPreferred;
                model.StreetDetails = response.Address.StreetDetails;
                model.ValidateInExternalTool = response.Address.ValidateInExternalTool;
                model.OdAddressVisibilityTypeId = response.Address.OdAddressVisibilityTypeId;
                model.OdAddressVisibilityTypeName = response.Address.OdAddressVisibilityTypeName;
                model.ExaminerCorrespondence = response.Address.ExaminerCorrespondence;

                foreach (var country in countries)
                {
                    model.CountryName = country.DisplayText;
                }
                // concatenate suburb, postcode for local addresses
                if (model.IsFromAustralia)
                {
                    IEnumerable<Postcode> postcode = lookupProvider.Postcodes.Where(p => p.SamId == response.Address.PostcodeId).ToList();
                    foreach (var code in postcode)
                    {
                        model.SuburbName = code.Suburb;
                        model.State = code.State;
                        model.Postcode = code.Code;
                    }
                }
            }

            decimal? longitude = null;
            decimal? latitude = null;

            if (response.WasSuccessful)
            {
                model.Success = true;

                var googleMapResult = Helpers.Common.GetGoogleMapResults(model.FullAddress).FirstOrDefault();

                if (googleMapResult != null)
                {
                    longitude = googleMapResult.Longitude;
                    latitude = googleMapResult.Latitude;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);

                model.Errors = ViewData.ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList();
            }
            
            return Json(new
            {
                model.CountryName,
                model.Errors,
                model.FullAddress,
                model.Id,
                model.OdAddressVisibilityTypeId,
                model.OdAddressVisibilityTypeName,
                model.IsFromAustralia,
                model.IsPreferred,
                model.Postcode,
                model.State,
                model.StreetDetails,
                model.ValidateInExternalTool,
                model.SuburbName,
                model.Success,
                Longitude = longitude,
                Latitude = latitude,
                model.ExaminerCorrespondence
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        // The method that actually updates the databased based on the information submitted in the form
        public ActionResult AddressEditJson(AddressEditGoogleModel model, string Command)
        {
            return UpdateAddress(model);
        }
        [HttpPost]
        public ActionResult ParseAddress(GeoResultModel request)
        {
            var response = mPersonalDetailsService.ParseAddress(request);
            return Json(response);
        }

        private ActionResult UpdateAddress(AddressEditGoogleModel model)
        {
            var jsonData = new JsonUpdateResponse { Success = false };
            var lookupProvider = ServiceLocator.Resolve<ILookupProvider>();
            
            var lookupCountry = lookupProvider.Countries.FirstOrDefault(x => x.DisplayText == model.CountryName);

            if (ModelState.IsValid)
            {
                var request = new PersonalDetailsUpdateAddressRequest();
                var countryId = lookupCountry == null ? 0 : lookupCountry.SamId;
                var postCodeId = 0;
                var isSuburbValid = true;

                if (model.IsFromAustralia)
                {
                    var postcode = lookupProvider.Postcodes?.FirstOrDefault(p => p.Code == model.Postcode &&
                    (p.Suburb ?? String.Empty).Equals(model.SuburbName, StringComparison.CurrentCultureIgnoreCase) &&
                    string.Equals(p.State, model.State, StringComparison.CurrentCultureIgnoreCase));

                    if (postcode == null)
                    {
                        // since this suburb was not recognised we must add it to the naati database
                        var suburb = mPersonalDetailsService.AddNewPostcode(new PersonalDetailsAddNewSuburbRequest
                        {
                            Postcode = model.Postcode,
                            State = model.State,
                            SuburbName = model.SuburbName?.ToUpper()
                        });

                        isSuburbValid = suburb.Success;

                        if (!isSuburbValid)
                        {
                            ModelState.AddModelError(string.Empty,"The suburb provided was not found. Please enter your suburb or full address and try again. If you continue to experience a problem, please use the link below to contact NAATI.");
                            postCodeId = 0;
                        }
                        else
                        {
                            postCodeId = suburb.PostcodeId;
                        }
                    }
                    else
                    {
                        postCodeId = model.IsFromAustralia ? postcode.SamId : 0;
                    }
                }

                if (isSuburbValid)
                {
                    // The UpdateAddress method handles whether a new address is created or whether the existing address is updated
                    request.Address = new PersonalEditAddress
                    {
                        AddressId = model.Id,
                        StreetDetails = model.StreetDetails,
                        IsFromAustralia = model.IsFromAustralia,
                        IsPreferred = model.IsPreferred,
                        CountryId = countryId,
                        PostcodeId = postCodeId,
                        OdAddressVisibilityTypeId = model.OdAddressVisibilityTypeId,
                        OdAddressVisibilityTypeName = model.OdAddressVisibilityTypeName,
                        ExaminerCorrespondence = model.ExaminerCorrespondence,
                        ValidateInExternalTool = model.ValidateInExternalTool
                    };

                    if (CurrentUserNaatiNumber <= 0)
                    {
                        throw new Exception("The Naati Number for the current user does not have a value. Please contact your System Administrator");
                    }
                    request.NaatiNumber = CurrentUserNaatiNumber;

                    var response = mPersonalDetailsService.UpdateAddress(request);

                    if (response.WasSuccessful)
                    {
                        jsonData.Success = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.ErrorMessage);
                    }
                }
            }

            if (!jsonData.Success)
            {
                jsonData.Errors = ViewData.ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList();
            }

            return Json(jsonData);
        }

        [HttpPost]
        public ActionResult AddressSearch()
        {
            var list = new List<AddressSearchResultItem>();

            var request = new PersonNaatiNumberRequest { NaatiNumber = CurrentUserNaatiNumber };
            var response =
                mPersonalDetailsService.GetAddresses(new NaatiNumberRequest() {NaatiNumber = CurrentUserNaatiNumber});
            var roles = mPersonalDetailsService.GetPerson(request)?.Person ?? new PersonalEditPerson();

            _autoMapperHelper.Mapper.Map(response.Addresses, list);

            foreach (var item in list)
            {
                item.IsExaminer = roles.IsExaminer;
                item.IsPractitioner = roles.IsPractitioner;
                item.IsFuturePractitioner = roles.IsFuturePractitioner;
                item.ResolveUrls(Url);
                item.ResolveIsFromAustralia();
            }

            var query = list.OrderBy(x => !x.IsPreferred).ToArray();

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = query.Length,
                recordsFiltered = query.Length,
                data = query,
                disableAddButton = query.Length >= 2
            });
        }

        [HttpPost]
        public ActionResult DeleteAddress(int id)
        {
            if (!mPersonalDetailsService.IsValidAddress(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }

            var request = new PersonalDetailsUpdateAddressRequest
            {
                Delete = true,
                Address = new PersonalEditAddress { AddressId = id },
                NaatiNumber = CurrentUserNaatiNumber
            };

            var response = mPersonalDetailsService.UpdateAddress(request);

            return Json(response);
        }

        [HttpPost]
        public ActionResult EmailCreateJson(EmailEditModel editModel)
        {
            var data = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var request = new PersonalDetailsUpdateEmailRequest();

                request.Email = _autoMapperHelper.Mapper.Map(editModel, request.Email);
                request.NaatiNumber = CurrentUserNaatiNumber;
                request.PrimaryEmail = User.Identity.Name;

                var response = mPersonalDetailsService.UpdateEmail(request);

                if (response.WasSuccessful)
                {
                    if (response.ChangePrimaryEmail)
                    {
                        RegisterEmailChange(request);
                    }

                    data.Success = true;
                }

                else
                {
                    ModelState.AddModelError("", response.ErrorMessage);
                }
                    
            }

            if (!data.Success)
            {
                data.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            return Json(data);
        }

        private void RegisterEmailChange(PersonalDetailsUpdateEmailRequest request)
        {
            int emailChangeValidHours =
                Convert.ToInt32(ConfigurationManager.AppSettings["EmailChangeValidHours"]);
            var url = Url.Action(nameof(AccountController.ChangeLogOnEmail),
                nameof(AccountController).Replace("Controller", string.Empty),
                new { reference = $"{nameof(mRegisterHelper.RegisterEmailChange)}" },
                this.Request?.Url?.Scheme);

            url = $"<a href=\"{url}\">{url}</a>";

            mRegisterHelper.RegisterEmailChange(request.PrimaryEmail, request.Email.Email, url,
                emailChangeValidHours);
        }

        [HttpGet]
        public ActionResult EmailEditJson(int id)
        {
            var model = new EmailEditModel();
            var request = new PersonalDetailsGetEmailRequest { EmailId = id };
            var response = mPersonalDetailsService.GetEmail(request);

            if (response.Email != null)
                _autoMapperHelper.Mapper.Map(response.Email, model);

            // attach any error message
            if (response.WasSuccessful)
                model.Success = true;
            else
                ModelState.AddModelError("", response.ErrorMessage);

            if (!model.Success)
            {
                model.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            model.PrimaryEmail = User.Identity.Name;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EmailEditJson(EmailEditModel editModel)
        {
            var data = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var request = new PersonalDetailsUpdateEmailRequest
                {
                    NaatiNumber = CurrentUserNaatiNumber,
                    PrimaryEmail = User.Identity.Name
                };

                request.Email = _autoMapperHelper.Mapper.Map(editModel, request.Email);
                var response = mPersonalDetailsService.UpdateEmail(request);

                if (response.WasSuccessful)
                {
                    if (response.ChangePrimaryEmail)
                    {
                        RegisterEmailChange(request);
                    }

                    data.Success = true;
                }

                else
                    ModelState.AddModelError("", response.ErrorMessage);
            }

            if (!data.Success)
            {
                data.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            return Json(data);
        }

        [HttpPost]
        public ActionResult EmailSearch()
        {
            var list = new List<EmailSearchResultItem>();

            var request = new PersonNaatiNumberRequest { NaatiNumber = CurrentUserNaatiNumber };
            var response = mPersonalDetailsService.GetEmails(request);
            var roles = mPersonalDetailsService.GetPerson(request)?.Person ?? new PersonalEditPerson();

            _autoMapperHelper.Mapper.Map(response.Emails, list);

            foreach (var item in list)
            {
                item.IsExaminer = roles.IsExaminer;
                item.IsPractitioner = roles.IsPractitioner;
                item.IsFuturePractitioner = roles.IsFuturePractitioner;
                item.ResolveUrls(Url);
            }

            var query = list.OrderBy(x => !x.IsPreferred).ToArray();
            int emailChangeValidHours = Convert.ToInt32(ConfigurationManager.AppSettings["EmailChangeValidHours"]);
            var pendinEmailChangeRequest = mRegisterHelper.HasPendingPrimaryEmailChange(User.Identity.Name, emailChangeValidHours);

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = query.Length,
                recordsFiltered = query.Length,
                data = query,
                disableAddButton = query.Length >= 2,
                pendingEmailChangeRequest = pendinEmailChangeRequest
            });
        }

        [HttpPost]
        public ActionResult DeleteEmail(int id)
        {
            var request = new PersonalDetailsUpdateEmailRequest
            {
                Delete = true,
                Email = new PersonalEditEmail { EmailId = id },
                NaatiNumber = CurrentUserNaatiNumber
            };

            var response = mPersonalDetailsService.UpdateEmail(request);

            return Json(response);
        }

        [HttpPost]
        public ActionResult PhoneCreateJson(PhoneEditModel editModel)
        {
            var jsonData = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var request = new PersonalDetailsUpdatePhoneRequest();

                request.Phone = _autoMapperHelper.Mapper.Map(editModel, request.Phone);
                request.NaatiNumber = CurrentUserNaatiNumber;

                var response = mPersonalDetailsService.UpdatePhone(request);

                if (response.WasSuccessful)
                    jsonData.Success = true;
                else
                    ModelState.AddModelError("", response.ErrorMessage);
            }

            if (!jsonData.Success)
            {
                jsonData.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            return Json(jsonData);
        }

        bool ValidatePhoneBeforeSaving(PhoneEditModel editModel)
        {
            var result = true;

            var lookupProvider = (ILookupProvider)ServiceLocator.GetService(typeof(ILookupProvider));

            if (!string.IsNullOrEmpty(editModel.Number) && Regex.IsMatch(editModel.Number, @"^[0-9]*$"))
            {
                if (editModel.Number.Length > 50)
                {
                    result = false;
                    ModelState.AddModelError("", "Phone number must be at most 50 characters long.");
                }
            }
            else
            {
                result = false;
                ModelState.AddModelError("", "Phone number can only contain numbers (0 to 9).");
            }

            return result;
        }

        public bool IsValidCountryCode(string countryCode)
        {
            return Regex.IsMatch(countryCode, @"^\+?[0-9]*$");
        }

        public bool IsLong(string pString)
        {
            long number;

            return (long.TryParse(pString, out number));
        }

        [HttpGet]
        public ActionResult PhoneEditJson(int id)
        {

            if (!mPersonalDetailsService.IsValidPhoneNumber(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }

            var model = new PhoneEditModel();
            var request = new PersonalDetailsGetPhoneRequest { PhoneId = id };
            var response = mPersonalDetailsService.GetPhone(request);

            if (response.Phone != null)
                _autoMapperHelper.Mapper.Map(response.Phone, model);

            // attach any error message
            if (response.WasSuccessful)
                model.Success = true;
            else
                ModelState.AddModelError("", response.ErrorMessage);

            if (!model.Success)
            {
                model.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PhoneEditJson(PhoneEditModel editModel)
        {
            var jsonData = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var request = new PersonalDetailsUpdatePhoneRequest();
                request.Phone = _autoMapperHelper.Mapper.Map(editModel, request.Phone);
                var response = mPersonalDetailsService.UpdatePhone(request);

                if (response.WasSuccessful)
                    jsonData.Success = true;
                else
                    ModelState.AddModelError("", response.ErrorMessage);
            }

            if (!jsonData.Success)
            {
                jsonData.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            return Json(jsonData);
        }

        [HttpPost]
        public ActionResult PhoneSearch()
        {
            var list = new List<PhoneSearchResultItem>();

            var request = new PersonNaatiNumberRequest { NaatiNumber = CurrentUserNaatiNumber };
            var response = mPersonalDetailsService.GetPhones(request);
            var roles = mPersonalDetailsService.GetPerson(request)?.Person ?? new PersonalEditPerson();

            _autoMapperHelper.Mapper.Map(response.Phones, list);

            foreach (var item in list)
            {
                item.IsExaminer = roles.IsExaminer;
                item.IsPractitioner = roles.IsPractitioner;
                item.IsFuturePractitioner = roles.IsFuturePractitioner;
                item.ResolveUrls(Url);
            }

            var query = list.OrderBy(x => !x.IsPreferred).ToArray();

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = query.Length,
                recordsFiltered = query.Length,
                data = query,
                disableAddButton = query.Length >= 2
            });
        }

        [HttpPost]
        public ActionResult DeletePhone(int id)
        {
            if (!mPersonalDetailsService.IsValidPhoneNumber(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }

            var request = new PersonalDetailsUpdatePhoneRequest
            {
                Delete = true,
                Phone = new PersonalEditPhone { PhoneId = id }
            };

            var response = mPersonalDetailsService.UpdatePhone(request);

            return Json(response);
        }

        [HttpPost]
        public ActionResult WebsiteSearch()
        {
            var request = new PersonNaatiNumberRequest
            {
                NaatiNumber = CurrentUserNaatiNumber
            };

            var websiteResponse = mPersonalDetailsService.GetWebsite(request);
            var hasWebsite = !string.IsNullOrEmpty(websiteResponse.Website);
            var data = new[]
            {
                new
                {
                    Url = websiteResponse.Website,
                    websiteResponse.IsCurrentlyListed
                }
            };

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = hasWebsite ? 1 : 0,
                recordsFiltered = hasWebsite ? 1 : 0,
                data = data.Where(x => !string.IsNullOrEmpty(x.Url)).ToArray(),
                disableAddButton = hasWebsite
            });
        }

        private JsonUpdateResponse WebsiteUpdate(WebsiteEditModel editModel)
        {
            var jsonData = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var request = new PersonalDetailsUpdateWebsiteRequest
                {
                    WebsiteUrl = editModel.Url,
                    NaatiNumber = CurrentUserNaatiNumber
                };

                mPersonalDetailsService.UpdateWebsite(request);

                jsonData.Success = true;
            }
            else
            {
                jsonData.Errors = ViewData.ModelState.Values
                        .SelectMany(m => m.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
            }

            return jsonData;
        }

        private bool GetMfaConfigured()
        {
            var mfaConfiguredResponse = mCredentialQrCodeService.GetMFAConfiguredAndValid(CurrentUserNaatiNumber);

            if (!mfaConfiguredResponse.Success)
            {
                var errors = new StringBuilder();

                foreach (var error in mfaConfiguredResponse.Errors)
                {
                    errors.Append(error);
                }

                throw new Exception(errors.ToString());
            }

            var mfaConfigured = mfaConfiguredResponse.Data;

            return mfaConfigured.MfaConfigured;
        }

        [HttpPost]
        public ActionResult WebsiteCreateJson(WebsiteEditModel editModel)
        {
            return Json(WebsiteUpdate(editModel));
        }

        [HttpPost]
        public ActionResult WebsiteEditJson(WebsiteEditModel editModel)
        {
            return Json(WebsiteUpdate(editModel));
        }
    }

    [Serializable]
    public class GoogleMapResult
    {
        public string StreetName { get; set; }
        public string SubPremise { get; set; }
        public string StreetNumber { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<string> Types { get; set; }

        public bool IsFromAustralia
        {
            get { return Country == "Australia"; }
        }

        public string Address
        {
            get
            {
                var stringBuild = new StringBuilder();

                stringBuild.Append(SubPremise).Append(StreetNumber).Append(" ").Append(StreetName);

                return stringBuild.ToString();
            }
        }

        public string LatitudeLongitude
        {
            get
            {
                var stringBuild = new StringBuilder();
                stringBuild.Append(Latitude).Append(" ").Append(Longitude);

                return stringBuild.ToString();
            }
        }

        public string SuburbAndPostCode
        {
            get
            {
                var stringBuild = new StringBuilder();
                stringBuild.Append(Suburb).Append(" ").Append(PostCode);

                return stringBuild.ToString();
            }
        }

        public string FullAddress
        {
            get
            {
                var stringBuild = new StringBuilder();

                stringBuild.Append(SubPremise);
                stringBuild.Append(StreetNumber).Append(" ");
                stringBuild.Append(StreetName).Append(" ");
                stringBuild.Append(Suburb).Append(" ");
                stringBuild.Append(State).Append(" ");
                stringBuild.Append(PostCode).Append(" ");
                stringBuild.Append(Country);

                return stringBuild.ToString();
            }
        }
    }

    public static class GeoResponseTypes
    {
        public static string Locality
        {
            get { return "locality"; }
        }

        public static string StreetAddress
        {
            get { return "street_address"; }
        }

        public static string SubPremise
        {
            get { return "subpremise"; }
        }

        public static string Route
        {
            get { return "route"; }
        }

        public static string AdministrativeArea
        {
            get { return "administrative_area_level_1"; }
        }

        public static string PostalCode
        {
            get { return "postal_code"; }
        }

        public static string Country
        {
            get { return "country"; }
        }

        public static string StreetNumber
        {
            get { return "street_number"; }
        }
    }
}
