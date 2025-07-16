using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using NHibernate.Util;
using Newtonsoft.Json;
using GetEndorsedQualificationForApplicationFormResponse = MyNaati.Contracts.BackOffice.GetEndorsedQualificationForApplicationFormResponse;

namespace MyNaati.Bl.BackOffice
{
    public interface IApplicationFormsService
    {
        IEnumerable<ApplicationFormSectionContract> GetSections(GetFormSectionRequestContract request);
        CredentialRequestDto SaveCredentialRequest(CredentialRequestRequestContract request);
        void DeleteCredentialRequest(DeleteCredentialRequestContract request);
        CreateCredentialApplicationResponse CreateCredentialApplication(CreateApplicationRequest request);
        GetApplicationDetailsResponse SaveApplicationForm(SaveApplicationFormRequestContract request);
        GetApplicationDetailsResponse GetApplicationDetails(int applicationId);
        IEnumerable<AnswerCredentialOptionContract> GetRecertificationOptions(int naatiNumber);
        SkillLookupResponse GetLanguagesForCredentialTypes(IEnumerable<int> credentialTypes);
        LanguageLookupResponse GetLanguagesForApplicationForm(int applicationFormId, int applicationId);
        GetEndorsedQualificationForApplicationFormResponse GetEndorsedQualificationForApplicationForm(int applicationFormId, int applicationId);
        PersonDetailsResponse GetPersonDetails(int naatiNumber);
        IEnumerable<DocTypeContract> GetDocumentTypes(GetDocumentTypesRequestContract request);

      
        ApplicationFormQuestionContract ReplaceQuestionFormTokens(ReplaceFormTokenRequest request);
        WorkPracticeStatusResponse GetRecertficationtWorkPracticeStatus(WorkPracticeStatusRequest request);
        PdPointsStatusResponse GetRecertficationPdPointsStatus(PdPointsStatusRequest request);
    }

    public class ApplicationFormHelper : IApplicationFormsService
    {
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly IPersonQueryService mPersonQueryService;
        private readonly ITokenReplacementService mTokenReplacementService;


        private readonly IActivityPointsCalculatorService mActivityPointSService;
        private readonly ICredentialPointsCalculatorService mCredentialPointsCalculator;
        private readonly ISecretsCacheQueryService mSecretsProvider;
        private readonly INcmsIntegrationService _ncmsIntegrationService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ApplicationFormHelper(IApplicationQueryService applicationQueryService,
            IPersonQueryService personQueryService, ITokenReplacementService tokenReplacementService,
            IActivityPointsCalculatorService activityPointsService, ICredentialPointsCalculatorService credentialPointsCalculator,
            ISecretsCacheQueryService secretsProvider,
            INcmsIntegrationService ncmsIntegrationService,
            IAutoMapperHelper autoMapperHelper)
        {
            mApplicationQueryService = applicationQueryService;
            mPersonQueryService = personQueryService;
            mTokenReplacementService = tokenReplacementService;
            mActivityPointSService = activityPointsService;
            mCredentialPointsCalculator = credentialPointsCalculator;
            mSecretsProvider = secretsProvider;
            _ncmsIntegrationService = ncmsIntegrationService;
            _autoMapperHelper = autoMapperHelper;
        }

        public IEnumerable<ApplicationFormSectionContract> GetSections(GetFormSectionRequestContract request)
        {
            var response = mApplicationQueryService.GetCredentialApplicationFormSections(request.ApplicationFormId);

            // Mapper.AssertConfigurationIsValid();
            var sections = response.Results.Select(_autoMapperHelper.Mapper.Map<ApplicationFormSectionContract>).ToList();
            var extatokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach(var externalUrl in request.ExternalUrls)
            {
                extatokens.Add(externalUrl.Key, externalUrl.Value);
            }

            ReplaceConfigTokens(sections, extatokens);
            if (request.ApplicationId != 0)
            {
                LoadResponses(sections, request.ApplicationId);
            }

            return sections;
        }

        public CredentialRequestDto SaveCredentialRequest(CredentialRequestRequestContract request)
        {
            var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = request.UserName });
            var credentialRequest = new CredentialRequestData
            {
                Credentials = Enumerable.Empty<CredentialDto>(),
                CredentialTypeId = request.LevelId,
                Fields = Enumerable.Empty<CredentialFieldData>(),
                SkillId = request.SkillId,
                StatusChangeUserId = user.UserId.GetValueOrDefault(),
                StatusChangeDate = DateTime.Now,
                StatusTypeId = (int)CredentialRequestStatusTypeName.Draft
            };

            // Todo: Improve this
            var existingRequestsIds = mApplicationQueryService.GetCredentialRequests(request.ApplicationId,
                new[] { CredentialRequestStatusTypeName.Deleted, CredentialRequestStatusTypeName.Withdrawn }).Results.Select(x => x.Id).ToList();

            AddCredentialRequest(credentialRequest, request.ApplicationId, request.CategoryId);

            var newCredentialRequests = mApplicationQueryService.GetCredentialRequests(request.ApplicationId,
                new[] { CredentialRequestStatusTypeName.Deleted, CredentialRequestStatusTypeName.Withdrawn }).Results;

            return newCredentialRequests.First(x => !existingRequestsIds.Contains(x.Id));
        }

        public void DeleteCredentialRequest(DeleteCredentialRequestContract request)
        {
            var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = request.UserName });
            var applicationDetails = GetApplicationDetails(request.ApplicationId);
            var applicationUpsertRequest = GetUpsertApplicationRequest(applicationDetails);
            var credentialRequest = applicationUpsertRequest.CredentialRequests.FirstOrDefault(x => x.Id == request.CredentialRequestId);
            if (credentialRequest == null)
            {
                return;
            }
            credentialRequest.StatusChangeUserId = user.UserId.GetValueOrDefault();
            credentialRequest.StatusChangeDate = DateTime.Now;
            credentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.Deleted;
            mApplicationQueryService.UpsertApplication(applicationUpsertRequest);
        }

        public CreateCredentialApplicationResponse CreateCredentialApplication(CreateApplicationRequest request)
        {
            if (request.ApplicationFormId == 0 || request.NaatiNumber == 0)
            {
                LoggingHelper.LogException(new ArgumentException(nameof(request.NaatiNumber)));
                return new CreateCredentialApplicationResponse();
            }

            var applicationForm = mApplicationQueryService.GetCredentialApplicationForm(request.ApplicationFormId);
            var ncmsRequest = new CreateNcmsApplicationRequest
            {
                NaatiNumber = request.NaatiNumber,
                ApplicationTypeId = applicationForm.Result.CredentialApplicationTypeId,
                ApplicationStatusTypeId = (int)CredentialApplicationStatusTypeName.Draft,
            };

            //var response = ExecutePostSamAction(samRequest, "api/application/CreateApplicationForMyNaati",SystemActionTypeName.CreateApplication.ToString());
            //var msg = response.Content.ReadAsStringAsync().Result;
            //var result = JsonConvert.DeserializeObject<CreateApplicationResultResponse>(msg);

            var response = _ncmsIntegrationService.CreateApplication(ncmsRequest);
            if (!response.Success)
            {
                throw new Exception($"Error Creating Application for naatiNumber {ncmsRequest.NaatiNumber}");
            }

            return new CreateCredentialApplicationResponse
            {
                ApplicationId = response.Data.CredentialApplicationId,
                ApplicationReference = $"APP{response.Data.CredentialApplicationId}"
            };
        }

        public GetApplicationDetailsResponse SaveApplicationForm(SaveApplicationFormRequestContract request)
        {
            var applicationId = request.ApplicationId;
            if (applicationId == 0 || request.NaatiNumber == 0)
            {
                return new GetApplicationDetailsResponse();
            }
            var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = request.UserName });
            var applicationDetails = GetApplicationDetails(applicationId);
            //need to validate Address here. Return Errors in the ApplicationDetails 
            var formDataToSave = GetFormDataToSave(applicationDetails, request.Sections, request.NaatiNumber,
                request.ApplicationFormId, user.UserId.GetValueOrDefault());
            SaveForm(formDataToSave);
            return applicationDetails;
        }


        private void SaveForm(SaveFormData formData)
        {
            UpdatePhoneNumber(formData);
            UpdatePersonAddressRequest(formData);
            mApplicationQueryService.UpsertApplication(formData.ApplicationUpsertRequest);
        }


        private void UpdatePhoneNumber(SaveFormData formData)
        {
            if (formData.NewPhone != null && formData.PersonData.PhoneNumber != formData.NewPhone.LocalNumber)
            {
                var existingPhoneNumber =
                    mPersonQueryService.GetPersonPhones(new GetPhonesRequest { EntityId = formData.PersonData.EntityId }).Phones.FirstOrDefault(x => !x.Invalid && x.PrimaryContact);

                if (existingPhoneNumber != null)
                {
                    formData.NewPhone.AllowSmsNotification = existingPhoneNumber.AllowSmsNotification;
                    formData.NewPhone.IncludeInPd = existingPhoneNumber.IncludeInPd;
                    formData.NewPhone.PhoneId = existingPhoneNumber.PhoneId;
                }

                mPersonQueryService.UpdatePersonPhone(new UpdatePersonPhoneRequest { Phone = formData.NewPhone });
            }
        }

        private void UpdatePersonAddressRequest(SaveFormData formData)
        {
            if (formData.NewAddress != null)
            {
                AddressDetailsDto existingAddress;

                if (formData.PersonData.Address != null)
                {
                    existingAddress =
                        mPersonQueryService.GetPersonAddress(
                            new GetAddressRequest { EntityId = formData.PersonData.EntityId, AddressId = formData.PersonData.Address.Id }).Address;

                }
                else
                {
                    existingAddress = mPersonQueryService
                        .GetPersonAddresses(new GetAddressRequest { EntityId = formData.PersonData.EntityId })
                        .Addresses.FirstOrDefault(x => !x.Invalid && x.PrimaryContact);
                }

                if (existingAddress != null)
                {
                    formData.NewAddress.OdAddressVisibilityTypeId = existingAddress.OdAddressVisibilityTypeId;
                    formData.NewAddress.AddressId = existingAddress.AddressId;
                }

                mPersonQueryService.UpdatePersonAddress(new UpdatePersonAddressRequest { Address = formData.NewAddress });
            }
        }

        private SaveFormData GetFormDataToSave(GetApplicationDetailsResponse applicationDetails, IEnumerable<ApplicationFormSectionContract> sections,
            int naatiNumber, int applicationFormId, int userId)
        {
            var personData = GetPersonDetails(naatiNumber);
            var applicationUpsertRequest = GetUpsertApplicationRequest(applicationDetails);
            var saveFormData = new SaveFormData(personData, applicationUpsertRequest, applicationFormId, userId,
                applicationDetails.ApplicationInfo.ApplicationReference);

            SetResponses(saveFormData, sections.SelectMany(x => x.Questions));
            return saveFormData;
        }

        private void SetResponses(SaveFormData saveFormData, IEnumerable<ApplicationFormQuestionContract> questions)
        {
            var responsesDictioanry =
                new Dictionary<CredentialApplicationFormAnswerTypeName,
                    Action<ApplicationFormQuestionContract, SaveFormData>>
                {
                    {CredentialApplicationFormAnswerTypeName.RadioOptions, SetRadioOptionResponse},
                    {CredentialApplicationFormAnswerTypeName.CheckOptions, SetCheckOptionsResponse},
                    {CredentialApplicationFormAnswerTypeName.Input, SetInputResponse},
                    {CredentialApplicationFormAnswerTypeName.Date, SetDateResponse},
                    {CredentialApplicationFormAnswerTypeName.PersonVerification, (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.PersonDetails, SetPersonDeatilsResponse},
                    {CredentialApplicationFormAnswerTypeName.LanguageSelector, SetLanguageSelectorResponse},
                    {CredentialApplicationFormAnswerTypeName.CredentialSelector,  (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.TestLocation, SetTestLocationResponse},
                    {CredentialApplicationFormAnswerTypeName.ProductSelector, SetProductSelectorResponse},
                    {CredentialApplicationFormAnswerTypeName.DocumentUpload,  (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.PersonPhoto,  (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.CountrySelector, SetInputResponse},
                    {CredentialApplicationFormAnswerTypeName.Email, SetInputResponse},
                    {CredentialApplicationFormAnswerTypeName.TestSessions,  (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.CredentialSelectorUpgradeAndSameLevel,  (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.Fees, SetPaymentOptionResponse},
                    {CredentialApplicationFormAnswerTypeName.RecertificationCredentialSelector, SetRecertificationCredentials},
                    {CredentialApplicationFormAnswerTypeName.EndorsedQualification, SetInputResponse},
                    {CredentialApplicationFormAnswerTypeName.EndorsedQualificationInstitution, (q, f) => { }},
                    {CredentialApplicationFormAnswerTypeName.EndorsedQualificationLocation, (q, f) => { }},
                };

            foreach (var question in questions)
            {
                var responseSetter =
                    responsesDictioanry[(CredentialApplicationFormAnswerTypeName)question.AnswerTypeId];
                responseSetter(question, saveFormData);
            }
        }

        private UpsertCredentialApplicationRequest GetUpsertApplicationRequest(
            GetApplicationDetailsResponse applicationDetails)
        {
            var applicationInfo = applicationDetails.ApplicationInfo;
            var credentialRequests = applicationDetails.CredentialRequests;
            var applicationFields = applicationDetails.Fields;
            var applicationRequest = _autoMapperHelper.Mapper.Map<UpsertCredentialApplicationRequest>(applicationInfo);
            applicationRequest.Fields = applicationFields.Select(_autoMapperHelper.Mapper.Map<ApplicationFieldData>).ToList();

            applicationRequest.CredentialRequests = credentialRequests.Select(_autoMapperHelper.Mapper.Map<CredentialRequestData>).ToList();
            applicationRequest.Notes = Enumerable.Empty<ApplicationNoteData>();

            return applicationRequest;
        }

        public GetApplicationDetailsResponse GetApplicationDetails(int applicationId)
        {
            var credentialDetailsRequest = new GetApplicationDetailsRequest { ApplicationId = applicationId };
            return mApplicationQueryService.GetApplicationDetails(credentialDetailsRequest);
        }

        private void SetRadioOptionResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            if (question.Response != null)
            {
                var answerId = Convert.ToInt32(question.Response);
                var answer = question.AnswerOptions.First(x => x.Id == answerId);
                if (answer.CredentialApplicationFieldId.HasValue)
                {
                    var credentialFieldData = data.FieldsByType[answer.CredentialApplicationFieldId.Value];
                    credentialFieldData.Value = answer.FieldData ?? true.ToString();
                    credentialFieldData.FieldOptionId = answer.FieldOptionId.GetValueOrDefault();
                }
            }
            else
            {
                foreach (var answer in question.AnswerOptions)
                {

                    if (answer.CredentialApplicationFieldId.HasValue)
                    {
                        var credentialFieldData = data.FieldsByType[answer.CredentialApplicationFieldId.Value];
                        credentialFieldData.Value = null;
                        credentialFieldData.FieldOptionId = 0;
                    }
                }
            }
        }

        private void SetRecertificationCredentials(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var answered = question.Response != null && bool.Parse(question.Response.ToString());
            if (!answered)
            {
                return;
            }
            var responses = question.Responses.Cast<int>().ToList();
            var currentCredentials = mCredentialPointsCalculator.GetDefaultPeriodCredentials(data.PersonData.NaatiNumber).ToList();
            var selectedCredentials = currentCredentials.Where(x => responses.Any(y => x.CredentialRequestId == y)).ToList();

            var addedCredentials = selectedCredentials.Where(x => !data.ApplicationUpsertRequest.CredentialRequests.Any(
                c => c.SkillId == x.SkillId && c.CredentialTypeId == x.CredentialTypeId && c.StatusTypeId != (int)CredentialRequestStatusTypeName.Deleted));

            var deletedCredentials = data.ApplicationUpsertRequest.CredentialRequests.Where(existingCredntial =>
            {
                return !selectedCredentials.Any(x => x.CredentialTypeId == existingCredntial.CredentialTypeId &&
                                                     x.SkillId == existingCredntial.SkillId);
            });

            foreach (var deletedCredential in deletedCredentials)
            {
                if (deletedCredential.StatusTypeId == (int)CredentialRequestStatusTypeName.Draft)
                {
                    deletedCredential.StatusTypeId = (int)CredentialRequestStatusTypeName.Deleted;
                }
            }

            foreach (var addedCredential in addedCredentials)
            {
                var newcredentialRequest = new CredentialRequestData
                {
                    Credentials = Enumerable.Empty<CredentialDto>(),
                    CredentialTypeId = addedCredential.CredentialTypeId,
                    Fields = Enumerable.Empty<CredentialFieldData>(),
                    StatusChangeUserId = data.UserId,
                    StatusChangeDate = DateTime.Now,
                    StatusTypeId = (int)CredentialRequestStatusTypeName.Draft,
                    SkillId = addedCredential.SkillId,
                    CredentialRequestPathTypeId = 1
                };

                AddCredentialRequest(newcredentialRequest, data.ApplicationUpsertRequest.ApplicationId, addedCredential.CategoryId);
            }

        }

        private void SetPaymentOptionResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            if (!question.CredentialApplicationFieldId.HasValue || question.Response == null || String.IsNullOrWhiteSpace(question.Response.ToString()))
            {
                return;
            }

            var answer = JsonConvert.DeserializeObject<dynamic>(question.Response.ToString());
            var answerId = (int)answer.PaymentMethodType;
            var credentialFieldData = data.FieldsByType[question.CredentialApplicationFieldId.Value];
            credentialFieldData.FieldOptionId = question.Response != null ? answerId : 0;
        }

        private void LoadRadioOptionResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
            foreach (var answerOption in question.AnswerOptions.Where(x => x.CredentialApplicationFieldId.HasValue))
            {
                var field = data.FieldsByType[answerOption.CredentialApplicationFieldId.GetValueOrDefault()];

                if (field.Value?.ToUpper() == answerOption.FieldData?.ToUpper() || (field.FieldOptionId != 0 && answerOption.FieldOptionId.HasValue && field.FieldOptionId == answerOption.FieldOptionId.Value))
                {
                    question.Response = answerOption.Id;
                    break;
                }
            }
        }

        private void SetCheckOptionsResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var responses = question.Responses.Cast<int>().ToList();

            if (responses.Any())
            {
                foreach (var answer in question.AnswerOptions)
                {
                    if (answer.CredentialApplicationFieldId.HasValue)
                    {
                        var credentialFieldData = data.FieldsByType[answer.CredentialApplicationFieldId.Value];
                        credentialFieldData.Value = responses.Contains(answer.Id)
                            ? answer.FieldData ?? true.ToString()
                            : false.ToString();

                        credentialFieldData.FieldOptionId = responses.Contains(answer.Id) ? answer.FieldOptionId.GetValueOrDefault() : 0;
                    }
                }
            }
            else
            {
                foreach (var answer in question.AnswerOptions)
                {
                    if (answer.CredentialApplicationFieldId.HasValue)
                    {
                        var credentialFieldData = data.FieldsByType[answer.CredentialApplicationFieldId.Value];
                        credentialFieldData.Value = null;
                        credentialFieldData.FieldOptionId = 0;
                    }
                }
            }
        }

        private void LoadCheckOptionsResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
            var responses = new List<object>();
            foreach (var answerOption in question.AnswerOptions.Where(x => x.CredentialApplicationFieldId.HasValue))
            {
                var field = data.FieldsByType[answerOption.CredentialApplicationFieldId.GetValueOrDefault()];

                if (field.Value?.ToUpper() == answerOption.FieldData?.ToUpper() || (field.FieldOptionId != 0 && answerOption.FieldOptionId.HasValue && field.FieldOptionId == answerOption.FieldOptionId.Value))
                {
                    responses.Add(answerOption.Id);
                }
            }

            question.Responses = responses;
        }

        private void LoadDefaultCredentialResponses(ApplicationFormQuestionContract question, GetFormSectionRequestContract data)
        {
            var responses = new List<object>();

            var answerOptions = new List<AnswerOptionContract>();
            var currentCredentials = mCredentialPointsCalculator.GetDefaultPeriodCredentials(data.NaatiNumber).ToList();
            for (var i = 0; i < currentCredentials.Count; i++)
            {
                var credential = currentCredentials[i];
                var status = $"{Convert.ToInt32(credential.Points)} out of {credential.Requirement} {credential.WorkPracticeUnits}";
                var statusColor = credential.Points >= Convert.ToDecimal(credential.Requirement) ? "success" : "warning";

                var option = new AnswerOptionContract
                {
                    Id = credential.CredentialRequestId,
                    Option = $" <span>{credential.CredentialType} {credential.Skill} </span> <span class='label text-base label-{statusColor}'>{status}</span>",
                    DisplayOrder = i,
                    Description = "<strong>TODO</strong>: Add description if necessary",
                    Documents = Enumerable.Empty<AnswerDocumentContract>()
                };

                answerOptions.Add(option);

            }


            question.Responses = responses;
            question.AnswerOptions = answerOptions;
        }
        private void LoadRecertificationCredentialResponses(ApplicationFormQuestionContract question, LoadFormData data)
        {
            var responses = new List<object>();

            var currentCredentials = mCredentialPointsCalculator.GetDefaultPeriodCredentials(data.ApplicationDetails.ApplicationInfo.NaatiNumber).ToList();

            foreach (var credentialRequest in data.ApplicationDetails.CredentialRequests)
            {
                var currentCredentialRequest =
                    currentCredentials.First(x => x.SkillId == credentialRequest.SkillId &&
                                                  x.CredentialTypeId == credentialRequest.CredentialTypeId);

                responses.Add(currentCredentialRequest.Id);
            }

            if (!responses.Any())
            {
                responses.AddRange(question.AnswerOptions.Select(x => x.Id).Cast<object>());
            }

            question.Responses = responses;
        }

        public IEnumerable<AnswerCredentialOptionContract> GetRecertificationOptions(int naatiNumber)
        {
            var answerOptions = new List<AnswerCredentialOptionContract>();
            var currentCredentials = mCredentialPointsCalculator.GetDefaultPeriodCredentials(naatiNumber).ToList();
            for (var i = 0; i < currentCredentials.Count; i++)
            {
                var credential = currentCredentials[i];
                var status = $"{Convert.ToInt32(credential.Points)} out of {credential.Requirement} {credential.WorkPracticeUnits}";
                var statusColor = credential.Points >= Convert.ToDecimal(credential.Requirement) ? "success" : "danger";

                var option = new AnswerCredentialOptionContract()
                {
                    Id = credential.CredentialRequestId,
                    Option = $" <span>{credential.CredentialType} {credential.Skill} </span> <span class='label text-base label-{statusColor}'>{status}</span>",
                    DisplayOrder = i,
                    Description = string.Empty,
                    Documents = Enumerable.Empty<AnswerDocumentContract>(),
                    CredentialTypeId = credential.CredentialTypeId
                };

                answerOptions.Add(option);

            }

            return answerOptions;
        }

        private void SetInputResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var value = question.Response?.ToString();
            if (!question.CredentialApplicationFieldId.HasValue)
            {
                throw new Exception($"Question id {question.Id} needs to have a value!");
            }
            var credentialFieldData = data.FieldsByType[question.CredentialApplicationFieldId.Value];
            credentialFieldData.Value = value;
        }

        private void LoadInputResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
            if (!question.CredentialApplicationFieldId.HasValue)
            {
                throw new Exception($"Question id {question.Id} needs to have a value!");
            }

            var credentialFieldData = data.FieldsByType[question.CredentialApplicationFieldId.GetValueOrDefault()];
            question.Response = credentialFieldData.Value;
        }

        private void SetDateResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var value = question.Response?.ToString();

            if (!question.CredentialApplicationFieldId.HasValue)
            {
                throw new Exception($"Question id {question.Id} needs to have a value!");
            }
            var credentialFieldData = data.FieldsByType[question.CredentialApplicationFieldId.Value];
            credentialFieldData.Value = value;
        }

        private void LoadDateResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
            if (!question.CredentialApplicationFieldId.HasValue)
            {
                throw new Exception($"Question id {question.Id} needs to have a value!");
            }

            var credentialFieldData = data.FieldsByType[question.CredentialApplicationFieldId.GetValueOrDefault()];
            question.Response = credentialFieldData.Value;
        }

        private void SetProductSelectorResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
        }

        private void LoadProductSelectorResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
        }

        private void SetTestLocationResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var selectedOption = question.Response;
            data.ApplicationUpsertRequest.PreferredTestLocationId = (int)(selectedOption ?? 0);
        }

        private void LoadTestLocationResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
            var preferedTestLocationId = data.ApplicationDetails.ApplicationInfo.PreferredTestLocationId;
            question.Response = preferedTestLocationId == 0 ? null : (object)preferedTestLocationId;
        }

        private void SetLanguageSelectorResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var selectedOption = question.Response;
            if (selectedOption != null)
            {
                var languageId = (int)selectedOption;
                // Only one credentialType should be available
                var credentialType = mApplicationQueryService
                    .GetCredentialTypesForApplicationForm(data.ApplicationFormId)
                    .Results.Single();

                var skill = mApplicationQueryService
                    .GetSkillsDetailsForCredentialType(
                        new GetSkillsDetailsRequest { CredentialTypeIds = new List<int> { credentialType.Id }, Language1Id = languageId })
                    .Results.OrderBy(x => x.DisplayName).Single(y => y.Language1Id == languageId);

                var newcredentialRequest = new CredentialRequestData
                {
                    Credentials = Enumerable.Empty<CredentialDto>(),
                    CredentialTypeId = credentialType.Id,
                    Fields = Enumerable.Empty<CredentialFieldData>(),
                    StatusChangeUserId = data.UserId,
                    StatusChangeDate = DateTime.Now,
                    StatusTypeId = (int)CredentialRequestStatusTypeName.Draft,
                    SkillId = skill.Id,
                    CredentialRequestPathTypeId = 1
                };

                var existingRequest = data.ApplicationUpsertRequest.CredentialRequests.Any(
                    x => x.CredentialTypeId == newcredentialRequest.CredentialTypeId
                         && x.SkillId == newcredentialRequest.SkillId &&
                         x.StatusTypeId == (int)CredentialRequestStatusTypeName.Draft);

                if (!existingRequest)
                {
                    foreach (var existingCredentialRequest in data.ApplicationUpsertRequest.CredentialRequests)
                    {
                        if (existingCredentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.Draft)
                        {
                            existingCredentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.Deleted;
                        }
                    }

                    AddCredentialRequest(newcredentialRequest, data.ApplicationUpsertRequest.ApplicationId, credentialType.CategoryId);
                }
            }

            else
            {
                foreach (var existingCredentialRequest in data.ApplicationUpsertRequest.CredentialRequests)
                {
                    if (existingCredentialRequest.StatusTypeId == (int)CredentialRequestStatusTypeName.Draft)
                    {
                        existingCredentialRequest.StatusTypeId = (int)CredentialRequestStatusTypeName.Deleted;
                    }
                }
            }
        }

        private void LoadLanguageSelectorResponse(ApplicationFormQuestionContract question, LoadFormData data)
        {
            var credentialRequest = data.ApplicationDetails.CredentialRequests.FirstOrDefault();

            question.Response = credentialRequest?.SkillLanguage1Id;
        }

        private void SetPersonDeatilsResponse(ApplicationFormQuestionContract question, SaveFormData data)
        {
            var response = question.Response;
            if (response != null)
            {
                var personDetailsData =
                    JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(response.ToString());

                string phoneNumber = personDetailsData[nameof(PersonDetailsResponse.PhoneNumber)];
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    data.SetPhoneNumber(phoneNumber);
                }

                var address = personDetailsData[nameof(PersonDetailsResponse.Address)];
                string streetDetails = address[nameof(AddressContract.StreetDetails)]?.Value;

                if (!string.IsNullOrEmpty(streetDetails))
                {
                    int? countryId = Convert.ToInt32(address[nameof(AddressContract.CountryId)]?.Value);
                    string countryName = address[nameof(AddressContract.CountryName)]?.Value;
                    string suburbName = address[nameof(AddressContract.SuburbName)]?.Value;
                    int? postCodeId = Convert.ToInt32(address[nameof(AddressContract.PostCodeId)]?.Value);
                    string postCode = address[nameof(AddressContract.Postcode)]?.Value;
                    string state = address[nameof(AddressContract.State)]?.Value;
                    bool validateInExternalTool = address[nameof(AddressContract.ValidateInExternalTool)]?.Value ?? false;

                    data.SetStreetDetails(streetDetails, countryName, countryId, suburbName, validateInExternalTool, postCodeId, postCode, state);
                }
            }
        }


        private class LoadFormData
        {
            public IDictionary<int, CredentialApplicationFieldDataDto> FieldsByType { get; }

            public GetApplicationDetailsResponse ApplicationDetails { get; }

            public LoadFormData(GetApplicationDetailsResponse applicationDetails)
            {
                ApplicationDetails = applicationDetails;
                FieldsByType = new Dictionary<int, CredentialApplicationFieldDataDto>();

                foreach(var field in applicationDetails.Fields)
                {
                    FieldsByType[field.FieldTypeId] = field;
                }
            }
        }
        private class SaveFormData
        {
            public string ApplicationReference { get; }

            public int UserId { get; }

            public int ApplicationFormId { get; }

            public SaveFormData(PersonDetailsResponse personData,
                UpsertCredentialApplicationRequest upsertApplicationRequest, int applicationFormId, int userId, string applicationReference)
            {
                FieldsByType = new Dictionary<int, ApplicationFieldData>();

                foreach(var field in upsertApplicationRequest.Fields)
                {
                    FieldsByType[field.FieldTypeId] = field;
                }

                PersonData = personData;
                ApplicationUpsertRequest = upsertApplicationRequest;
                ApplicationFormId = applicationFormId;
                UserId = userId;
                ApplicationReference = applicationReference;

            }

            public IDictionary<int, ApplicationFieldData> FieldsByType { get; }

            public PersonDetailsResponse PersonData { get; }
            public UpsertCredentialApplicationRequest ApplicationUpsertRequest { get; }

            public PhoneDetailsDto NewPhone { get; private set; }

            public AddressDetailsDto NewAddress { get; private set; }

            public void SetPhoneNumber(string phoneNumber)
            {

                NewPhone = new PhoneDetailsDto
                {
                    Invalid = false,
                    LocalNumber = phoneNumber,
                    Note = string.Empty,
                    EntityId = PersonData.EntityId,
                    PrimaryContact = true
                };
            }

            public void SetStreetDetails(string streetDetails,
                string countryName,
                int? countryId,
                string suburbName,
                bool validateInExternalTool,
                int? postCodeId,
                string postCode,
                string state)
            {
                var australiaCountryId = 13;
                if (countryId.GetValueOrDefault() == 0 && (string.IsNullOrWhiteSpace(countryName) || countryName.ToLower() == "australia"))
                {
                    countryId = australiaCountryId;
                }

                suburbName = countryId.GetValueOrDefault() == australiaCountryId ? suburbName : null;

                NewAddress = new AddressDetailsDto
                {
                    Invalid = false,
                    StreetDetails = streetDetails,
                    CountryName = countryId.GetValueOrDefault() == 0 ? countryName : null,
                    Suburb = suburbName,
                    EntityId = PersonData.EntityId,
                    Note = string.Empty,
                    PrimaryContact = true,
                    ValidateInExternalTool = validateInExternalTool,
                    CountryId = countryId.GetValueOrDefault() != 0 ? countryId : null,
                    PostcodeId = countryId.GetValueOrDefault() == australiaCountryId && postCodeId.GetValueOrDefault() != 0 ? postCodeId : null,
                    Postcode = countryId.GetValueOrDefault() == australiaCountryId ? postCode : null,
                    SuburbName = countryId.GetValueOrDefault() == australiaCountryId ? suburbName : null,
                    StateAbbreviation = state
                };
            }
        }

        public SkillLookupResponse GetLanguagesForCredentialTypes(IEnumerable<int> credentialTypes)
        {
            var languages =
                mApplicationQueryService.GetSkillsDetailsForCredentialType(
                    new GetSkillsDetailsRequest { CredentialTypeIds = credentialTypes });
            return new SkillLookupResponse
            {
                Results = languages.Results.OrderBy(x => x.DisplayName).Select(_autoMapperHelper.Mapper.Map<SkillLookupContract>)
            };
        }

        public LanguageLookupResponse GetLanguagesForApplicationForm(int applicationFormId, int applicationId)
        {
            // Only one credentialtype must be available for the selected application form ID

            var credentialType = mApplicationQueryService.GetCredentialTypesForApplicationForm(applicationFormId)
                .Results.SingleOrDefault();

            if (credentialType == null)
            {
                return new LanguageLookupResponse
                {
                    Results = Enumerable.Empty<LanguageLookupContract>()
                };
            }

            var response = mApplicationQueryService.GetSkillsForCredentialType(new GetSkillsForCredentialTypeRequest { CredentialTypeIds = new[] { credentialType.Id }, ApplicationId = applicationId }).Results;
            return new LanguageLookupResponse
            {
                Results = response.OrderBy(x => x.DisplayName).Select(
                    x => new LanguageLookupContract { Id = x.Language1Id, DisplayName = x.DisplayName, SkillId = x.Id })
            };
        }

        public GetEndorsedQualificationForApplicationFormResponse GetEndorsedQualificationForApplicationForm(int applicationFormId, int applicationId)
        {
            var response = mApplicationQueryService.GetEndorsedQualificationForApplicationForm(applicationFormId, applicationId);
            return new GetEndorsedQualificationForApplicationFormResponse
            {
                Results = response.Results.Select(_autoMapperHelper.Mapper.Map<EndorsedQualificationContract>)
            };
        }

        public PersonDetailsResponse GetPersonDetails(int naatiNumber)
        {
            var getPersonDetailsRequest = new GetPersonDetailsRequest
            {
                NaatiNumber = naatiNumber
            };
            var personDetailsResponse = mPersonQueryService.GetPersonDetailsBasic(getPersonDetailsRequest);
            var contactDetails =
                mPersonQueryService.GetPersonContactDetails(
                    new GetContactDetailsRequest { EntityId = personDetailsResponse.PersonDetails.EntityId });
            var address = contactDetails.Addresses.FirstOrDefault(x => x.PrimaryContact && !x.Invalid) ??
                          contactDetails.Addresses.FirstOrDefault(x => !x.Invalid);

            AddressContract addressContaract = null;
            if (address != null)
            {
                addressContaract = new AddressContract
                {
                    Id = address.AddressId.GetValueOrDefault(),
                    CountryName = address.CountryName,
                    IsFromAustralia = address.CountryName == "Australia",
                    Latitude = null,
                    Longitude = null,
                    Postcode = address.Postcode,
                    StreetDetails = address.StreetDetails,
                    SuburbName = address.Suburb,
                    State = string.Empty,
                    ValidateInExternalTool = address.ValidateInExternalTool,
                    PostCodeId = address.PostcodeId
                };
            }


            return new PersonDetailsResponse
            {
                NaatiNumber = personDetailsResponse.PersonDetails.NaatiNumber,
                EntityId = personDetailsResponse.PersonDetails.EntityId,
                Address = addressContaract,
                PhoneNumber = personDetailsResponse.PersonDetails.PrimaryContactNumber,
                Deceased = personDetailsResponse.PersonDetails.Deceased
            };
        }

        public IEnumerable<DocTypeContract> GetDocumentTypes(GetDocumentTypesRequestContract request)
        {
            var sections = GetSections(new GetFormSectionRequestContract
            {
                ApplicationId = request.ApplicationId,
                ApplicationFormId = request.ApplicationFormId,
                ExternalUrls = request.ExternalUrls,
                NaatiNumber = request.NaatiNumber
            });

            var documentTypesDictionary = new Dictionary<int, DocTypeContract>();

            foreach (var section in sections)
            {
                foreach (var question in section.Questions)
                {
                    var documents = new List<AnswerDocumentContract>();
                    if (question.AnswerTypeId == (int)CredentialApplicationFormAnswerTypeName.RadioOptions)
                    {
                        var answer = question.AnswerOptions.FirstOrDefault(x => x.Id == Convert.ToInt32(question.Response ?? 0));
                        if (answer != null)
                        {
                            documents.AddRange(answer.Documents);
                        }
                    }
                    if (question.AnswerTypeId == (int)CredentialApplicationFormAnswerTypeName.CheckOptions)
                    {
                        foreach (int answerId in question.Responses)
                        {
                            var answer = question.AnswerOptions.First(x => x.Id == answerId);
                            documents.AddRange(answer.Documents);
                        }
                    }

                    documents.ForEach(d => documentTypesDictionary[d.DocumentTypeId] = new DocTypeContract { DisplayName = d.DisplayName, Id = d.DocumentTypeId, Required = true });
                }
            }

            return documentTypesDictionary.Values;
        }

        private bool HasTokens(string content)
        {
            return mTokenReplacementService.TotalTokens(content) > 0;
        }

        private void ReplaceConfigTokens(IEnumerable<ApplicationFormSectionContract> sections, IDictionary<string, string> externalUrls)
        {
            foreach (var section in sections)
            {
                section.Name = ReplaceConfigToken(section.Name, externalUrls);
                section.Description = ReplaceConfigToken(section.Description, externalUrls);
                section.HasTokens = HasTokens(section.Name) || HasTokens(section.Description);
                foreach (var question in section.Questions)
                {
                    ReplaceConfigTokens(question, externalUrls);
                }
            }
        }

        private void ReplaceConfigTokens(ApplicationFormQuestionContract question, IDictionary<string, string> externalUrls)
        {
            question.Text = ReplaceConfigToken(question.Text, externalUrls);
            question.Description = ReplaceConfigToken(question.Description, externalUrls);
            question.HasTokens = HasTokens(question.Text) || HasTokens(question.Description);
            foreach (var answer in question.AnswerOptions)
            {
                answer.Option = ReplaceConfigToken(answer.Option, externalUrls);
                answer.Description = ReplaceConfigToken(answer.Description, externalUrls);

                if (answer.Function != null)
                {
                    answer.Function.Parameter = ReplaceConfigToken(answer.Function.Parameter, externalUrls);
                }

                answer.HasTokens = HasTokens(answer.Option) || HasTokens(answer.Description) ||
                                   HasTokens(answer.Function?.Parameter ?? string.Empty);
            }

        }
        private string ReplaceToken(string content, IDictionary<string, string> extraTokens)
        {
            IEnumerable<string> errors;
            var text = content;
            mTokenReplacementService.ReplaceTemplateFieldValues(content, (token, value) => text = text.Replace(token, value), extraTokens, true, out errors);
            return text;
        }


        private string ReplaceConfigToken(string content, IDictionary<string, string> externalUrls)
        {
            var replacedContent = content ?? string.Empty;

            foreach (var foundToken in GetTokens(content ?? string.Empty))
            {
                string tokenValue;
                if (!externalUrls.TryGetValue(foundToken, out tokenValue))
                {
                    continue;
                }

                replacedContent = replacedContent.Replace($"[[{foundToken}]]", tokenValue);
            }

            return replacedContent;
        }


        private IEnumerable<string> GetTokens(string content)
        {
            var foundTokens =
                Regex.Matches((content ?? string.Empty).Replace(Environment.NewLine, string.Empty), @"\[\[([^]]*)\]\]")
                    .Cast<Match>()
                    .Select(x => x.Groups[1].Value);

            return foundTokens;
        }


        private void LoadResponses(IEnumerable<ApplicationFormSectionContract> sections, int applicationId)
        {
            var applicationDetails = GetApplicationDetails(applicationId);
            var loadFormData = new LoadFormData(applicationDetails);

            var responsesDictioanry =
            new Dictionary<CredentialApplicationFormAnswerTypeName,
                Action<ApplicationFormQuestionContract, LoadFormData>>
            {
                    {CredentialApplicationFormAnswerTypeName.RadioOptions, LoadRadioOptionResponse},
                    {CredentialApplicationFormAnswerTypeName.CheckOptions, LoadCheckOptionsResponse},
                    {CredentialApplicationFormAnswerTypeName.Input, LoadInputResponse},
                    {CredentialApplicationFormAnswerTypeName.Date, LoadDateResponse},
                    {CredentialApplicationFormAnswerTypeName.PersonVerification,(q,a)=> { }},
                    {CredentialApplicationFormAnswerTypeName.PersonDetails, (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.LanguageSelector, LoadLanguageSelectorResponse},
                    {CredentialApplicationFormAnswerTypeName.CredentialSelector,  (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.TestLocation, LoadTestLocationResponse},
                    {CredentialApplicationFormAnswerTypeName.ProductSelector, LoadProductSelectorResponse},
                    {CredentialApplicationFormAnswerTypeName.DocumentUpload,  (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.PersonPhoto,  (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.CountrySelector,  LoadInputResponse},
                    {CredentialApplicationFormAnswerTypeName.Email,  LoadInputResponse},
                    {CredentialApplicationFormAnswerTypeName.TestSessions,  (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.CredentialSelectorUpgradeAndSameLevel,  (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.Fees,  (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.RecertificationCredentialSelector,  LoadRecertificationCredentialResponses},
                    {CredentialApplicationFormAnswerTypeName.EndorsedQualification, (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.EndorsedQualificationInstitution, (q,a) => { }},
                    {CredentialApplicationFormAnswerTypeName.EndorsedQualificationLocation, (q,a) => { }},
            };

            var questions = sections.SelectMany(s => s.Questions.ToList());
            foreach (var question in questions)
            {
                var responseLoader =
                    responsesDictioanry[(CredentialApplicationFormAnswerTypeName)question.AnswerTypeId];
                responseLoader(question, loadFormData);
            }
        }
        //private HttpResponseMessage ExecutePostSamAction(object obj, string url, string actionName)
        //{
        //    var data = JsonConvert.SerializeObject(obj);
        //    var ctn = new StringContent(data, Encoding.UTF8, "application/json");

        //    var credentials = CredentialCache.DefaultNetworkCredentials;

        //    var ncmsUrl = ConfigurationManager.AppSettings["NcmsUrl"];
        //    var ncmsCookieName = ConfigurationManager.AppSettings["NcmsAuthCookieName"];
        //    var identity = mSecretsProvider.Get("MyNaatiDefaultIdentity");

        //    var httpCookie = FormsAuthentication.GetAuthCookie(identity, false);
        //    httpCookie.Expires = DateTime.Now.AddMinutes(1);
        //    var cookieContainer = new CookieContainer();
        //    cookieContainer.Add(new Uri(ncmsUrl), new Cookie(ncmsCookieName, httpCookie.Value));

        //    var httpClientHandler = new HttpClientHandler
        //    {
        //        Credentials = credentials,
        //        CookieContainer = cookieContainer
        //    };

        //    using (var client = new HttpClient(httpClientHandler))
        //    {
        //        client.BaseAddress = new Uri(ncmsUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var response = client.PostAsync($"{url}", ctn).Result;

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var builder = new StringBuilder();
        //            builder.AppendLine("Error executing NCMS request.");
        //            builder.AppendLine($"Error occurred while executing {actionName} action");
        //            builder.AppendLine($"URL: {response.RequestMessage?.RequestUri}");
        //            builder.AppendLine("Response:");
        //            builder.AppendLine(response.ToString());

        //            throw new Exception(builder.ToString());
        //        }

        //        return response;
        //    }
        //}

        //private HttpResponseMessage ExecuteSamAction(int actionId, int applicationId, IEnumerable<object> steps = null)
        //{
        //    if (applicationId == 0)
        //    {
        //        return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        //    }

        //    var request = new { ApplicationId = applicationId, ActionId = actionId, Steps = steps ?? Enumerable.Empty<object>() };
        //    var data = JsonConvert.SerializeObject(request);
        //    var ctn = new StringContent(data, Encoding.UTF8, "application/json");

        //    var credentials = CredentialCache.DefaultNetworkCredentials;

        //    var ncmsUrl = ConfigurationManager.AppSettings["NcmsUrl"];
        //    var ncmsCookieName = ConfigurationManager.AppSettings["NcmsAuthCookieName"];
        //    var identity = mSecretsProvider.Get("MyNaatiDefaultIdentity");

        //    var httpCookie = FormsAuthentication.GetAuthCookie(identity, false);
        //    httpCookie.Expires = DateTime.Now.AddMinutes(1);
        //    var cookieContainer = new CookieContainer();
        //    cookieContainer.Add(new Uri(ncmsUrl), new Cookie(ncmsCookieName, httpCookie.Value));

        //    var httpClientHandler = new HttpClientHandler
        //    {
        //        Credentials = credentials,
        //        CookieContainer = cookieContainer
        //    };

        //    using (HttpClient client = new HttpClient(httpClientHandler))
        //    {
        //        client.BaseAddress = new Uri(ncmsUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = client.PostAsync("api/application/MyNaatiWizard", ctn).Result;

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var actionName = actionId == 3 ? "Submit" : actionId == 9 ? "Delete" : actionId.ToString();
        //            var builder = new StringBuilder();
        //            builder.AppendLine("Error executing NCMS request.");
        //            builder.AppendLine($"Error occurred while executing Application action: '{actionName}'");
        //            builder.AppendLine($"Application ID: '{applicationId}'");
        //            builder.AppendLine($"URL: {response.RequestMessage?.RequestUri}");
        //            builder.AppendLine("Response:");
        //            builder.AppendLine(response.ToString());

        //            throw new Exception(builder.ToString());
        //        }

        //        return response;
        //    }
        //}

        //public void CreateCredentialRequestAction(int applicationId, int skillId, int credentialTypeId, int categoryId)
        //{
            
        //}

        public void AddCredentialRequest(CredentialRequestData requestData, int applicationId, int categoryId)
        {
            if (applicationId == 0)
            {
                return;
            }

            var response = _ncmsIntegrationService.CreateNcmsApplicationCredentialRequest(applicationId, requestData.SkillId, requestData.CredentialTypeId, categoryId);

            if (!response.Success)
            {
                throw new Exception($"Error Creating Credential Request For Application {applicationId}");
            }
        }

        private string GetPdPointsSummaryToken(int naatiNumber)
        {
            var points = mActivityPointSService.CaluculatePointsForDefaultPeriod(naatiNumber);
            var builder = new StringBuilder();
            builder.Append("<br/>");
            builder.Append("<br/>");

            var messages = points.Sections.SelectMany(x => x.PreRequisites).Concat(points.PreRequisites).GroupBy(y => y.Message);

            foreach (var message in messages)
            {
                var classColor = message.Any(x => !x.Completed) ? "text-danger" : "text-success";
                var iconClass = message.Any(x => !x.Completed) ? "fa fa-remove" : "fa fa-check";
                var content = $"<div class='{classColor}'><i class= '{iconClass}'></i><span>{message.Key}</span></div>";
                builder.Append(content);
            }

            return builder.ToString();
        }

        public ApplicationFormQuestionContract ReplaceQuestionFormTokens(ReplaceFormTokenRequest request)
        {
            var tokensDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    mTokenReplacementService.GetTokenNameFor(TokenReplacementField.PdPointsSummary),
                    GetPdPointsSummaryToken(request.NaatiNumber)
                }
            };

            var response = mApplicationQueryService.GetCredentialApplicationFormQuestion(request.QuestionId);
            var question = _autoMapperHelper.Mapper.Map<ApplicationFormQuestionContract>(response);

            var urlTokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach(var externalUrl in request.ExternalUrls)
            {
                urlTokens.Add(externalUrl.Key, externalUrl.Value);
            }

            ReplaceConfigTokens(question, urlTokens);
            question.Text = ReplaceToken(question.Text, tokensDictionary);
            question.Description = ReplaceToken(question.Description, tokensDictionary);

            foreach (var answerOption in question.AnswerOptions)
            {
                answerOption.Option = ReplaceToken(answerOption.Option, tokensDictionary);
                answerOption.Description = ReplaceToken(answerOption.Description, tokensDictionary);
            }

            return question;

        }


        public WorkPracticeStatusResponse GetRecertficationtWorkPracticeStatus(WorkPracticeStatusRequest request)
        {
            var response = new WorkPracticeStatusResponse
            {
                Met = true
            };
            var applicationRequests = mApplicationQueryService.GetCredentialRequests(request.ApplicationId,
                    new[] { CredentialRequestStatusTypeName.Deleted, CredentialRequestStatusTypeName.Withdrawn })
                .Results.ToList();

            if (!applicationRequests.Any())
            {
                response.Met = false;
                return response;
            }

            var currentCredentials = mCredentialPointsCalculator.GetDefaultPeriodCredentials(request.NaatiNumber).ToList();

            foreach (var applicationRequest in applicationRequests)
            {
                var currentCredential =
                    currentCredentials.First(x => x.SkillId == applicationRequest.SkillId &&
                                                  x.CredentialTypeId == applicationRequest.CredentialTypeId);

                if (Convert.ToDecimal(currentCredential.Requirement) > currentCredential.Points)
                {
                    response.Met = false;
                    return response;
                }
            }

            return response;
        }

        public PdPointsStatusResponse GetRecertficationPdPointsStatus(PdPointsStatusRequest request)
        {
            var completed = mActivityPointSService.CaluculatePointsForDefaultPeriod(request.NaatiNumber).Completed;

            return new PdPointsStatusResponse
            {
                Met = completed
            };
        }

    }
}
