using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.CredentialPrerequisite;
using Ncms.Contracts.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl
{
    public class CredentialPrerequisiteService : ICredentialPrerequisiteService
    {
        private readonly ICredentialPrerequisiteQueryService _credentialPrerequisiteQueryService;
        private readonly ICredentialPrerequisiteDalService _credentialPrerequisiteDalService;
        private readonly IApplicationQueryService _applicationQueryService;

        public CredentialPrerequisiteService(ICredentialPrerequisiteQueryService credentialPrerequsitieQueryService, ICredentialPrerequisiteDalService prerequisiteApplicationDalService, IApplicationQueryService applicationQueryService)
        {
            _credentialPrerequisiteQueryService = credentialPrerequsitieQueryService;
            _credentialPrerequisiteDalService = prerequisiteApplicationDalService;
            _applicationQueryService = applicationQueryService;
        }

        public GenericResponse<ValidatePrerequisiteResponse> ValidateMandatoryFields(CreatePrerequisiteRequest createPrerequisiteRequest)
        {

            var response = new ValidatePrerequisiteResponse()
            {
                CreatePrerequisiteRequest = new CreatePrerequisiteRequest()
                {
                    CredentialRequestTypes = createPrerequisiteRequest.CredentialRequestTypes,
                    EnteredUserId = createPrerequisiteRequest.EnteredUserId,
                    ParentCredentialRequestId = createPrerequisiteRequest.ParentCredentialRequestId
                },
                MissingMandatoryFields = new List<MissingMandatoryField>()
            };

            //get list of fields that have data in current credential request
            var result = _credentialPrerequisiteQueryService.GetExistingApplicationFieldsForCredentialRequest(createPrerequisiteRequest.ParentCredentialRequestId);
            if(!result.Success)
            {
                return new GenericResponse<ValidatePrerequisiteResponse>() { Errors = new List<string>() { "Failed to get current ApplicationFields for Credential Request" } };
            }

            var existingFieldsInParent = result.Data.ToList();

            //for each credential type, match what fields are already in the parent. Those that arent in the parent go back as errors to the UI
            foreach(var credentialRequestType in createPrerequisiteRequest.CredentialRequestTypes)
            {
                var credentialApplicationTypeId = credentialRequestType.ApplicationTypeId;
                var mandatoryFieldsResult = _credentialPrerequisiteQueryService.GetMandatoryFieldsForApplicationType(credentialApplicationTypeId);
                if(!mandatoryFieldsResult.Success)
                {
                    throw new Exception($"failed to get mandatory fields from credentialApplicationTypeId {credentialApplicationTypeId}");
                }
                var mandatoryFields = mandatoryFieldsResult.Data;
                foreach(var mandatoryField in mandatoryFields)
                {
                    if(!existingFieldsInParent.Contains(mandatoryField))
                    {
                        var credRequestToMarkAsError = response.CreatePrerequisiteRequest.CredentialRequestTypes.First(x => x.CredentialRequestType == credentialRequestType.CredentialRequestType);
                        credRequestToMarkAsError.HasValidationError = true;

                        response.MissingMandatoryFields.Add(new MissingMandatoryField()
                        {
                            ApplicationType = credentialRequestType.CredentialRequestType,
                            PrerequisiteCredential = credentialRequestType.CredentialRequestType,
                            MandatoryInformation = mandatoryField
                        });
                    }
                }
            }

            return response;
        }

        public GenericResponse<ValidatePrerequisiteResponse> ValidateMandatoryDocuments(CreatePrerequisiteRequest createPrerequisiteRequest)
        {
            var response = new ValidatePrerequisiteResponse()
            {
                CreatePrerequisiteRequest = new CreatePrerequisiteRequest()
                {
                    CredentialRequestTypes = createPrerequisiteRequest.CredentialRequestTypes,
                    EnteredUserId = createPrerequisiteRequest.EnteredUserId,
                    ParentCredentialRequestId = createPrerequisiteRequest.ParentCredentialRequestId
                },
                MissingMandatoryDocuments = new List<MissingMandatoryDocument>()
            };

            //get list of fields that have data in current credential request
            var result = _credentialPrerequisiteQueryService.GetExistingDocumentTypesForCredentialRequest(createPrerequisiteRequest.ParentCredentialRequestId);
            if (!result.Success)
            {
                return new GenericResponse<ValidatePrerequisiteResponse>() { Errors = new List<string>() { "Failed to get current ApplicationFields for Credential Request" } };
            }

            var existingDocumentsInParent = result.Data.ToList();

            //for each credential type, match what fields are already in the parent. Those that arent in the parent go back as errors to the UI
            foreach (var credentialRequestType in createPrerequisiteRequest.CredentialRequestTypes)
            {
                var credentialApplicationTypeId = credentialRequestType.ApplicationTypeId;
                var mandatoryDocumentsResult = _credentialPrerequisiteQueryService.GetMandatoryDocumentsForApplicationType(credentialApplicationTypeId);
                if (!mandatoryDocumentsResult.Success)
                {
                    throw new Exception($"failed to get mandatory documents from credentialApplicationTypeId {credentialApplicationTypeId}");
                }
                var mandatoryDocuments = mandatoryDocumentsResult.Data;
                foreach (var mandatoryDocument in mandatoryDocuments)
                {
                    if (!existingDocumentsInParent.Contains(mandatoryDocument))
                    {
                        var credRequestToMarkAsError = response.CreatePrerequisiteRequest.CredentialRequestTypes.First(x => x.CredentialRequestType == credentialRequestType.CredentialRequestType);
                        credRequestToMarkAsError.HasValidationError = true;

                        response.MissingMandatoryDocuments.Add(new MissingMandatoryDocument()
                        {
                            ApplicationType = credentialRequestType.CredentialRequestType,
                            PrerequisiteCredential = credentialRequestType.CredentialRequestType,
                            MandatoryDocumentType = mandatoryDocument
                        });
                    }
                }
            }
            return response;
        }

        public IEnumerable<CredentialPrerequisite> GetCredentialPrerequisites()
        {
            var credentialPrerequisites = _credentialPrerequisiteDalService.GetCredentialPrerequisites();

            return credentialPrerequisites;
        }

        public IEnumerable<int> GetCredentialPrerequisiteIds(IEnumerable<CredentialPrerequisite> credentialPrerequisites)
        {
            var credentialPrerequisiteIds = new List<int>();

            foreach (var credentialPrerequisite in credentialPrerequisites)
            {
                var credentialPrerequisiteId = credentialPrerequisite.CredentialType.Id;
                credentialPrerequisiteIds.Add(credentialPrerequisiteId);
            }

            return credentialPrerequisiteIds;
        }

        public GenericResponse<PrerequisiteSummaryResult> GetPrerequisiteSummary(int credentialRequestId)
        {
            var prerequisitesForCredentialRequest = _credentialPrerequisiteDalService.GetPrerequisiteSummary(credentialRequestId);

            return prerequisitesForCredentialRequest;
        }

        public PrerequisiteSummaryResult GetPreReqsForCredRequest(int credentialRequestId)
        {
            var prerequisitesForCredentialRequest = _credentialPrerequisiteQueryService.GetPreReqsForCredRequest(credentialRequestId);

            return prerequisitesForCredentialRequest;
        }
        public GenericResponse<List<CredentialPrerequisiteExemptionModel>> GetCredentialPrerequisiteExemptions(int credentialRequestId)
        {
            // Call DAL to get details of the credential request
            var credentialRequestDetailsResponse = _credentialPrerequisiteDalService.GetCredentialRequestDetails(credentialRequestId);

            if (!credentialRequestDetailsResponse.Success)
            {
                return new GenericResponse<List<CredentialPrerequisiteExemptionModel>>(null)
                {
                    Errors = new List<string>() { $"Could not get credential request details for credential request {credentialRequestId}" }
                };
            }

            // Call DAL to get the exemptions of the person based on the details of the credential request (credential type id, skill id, person id)
            var credentialRequestDetails = credentialRequestDetailsResponse.Data;
            var credentialPrerequisiteExemptionsResponse = _credentialPrerequisiteDalService.GetActiveCredentialPrerequisiteExemptions(credentialRequestDetails);

            if (!credentialPrerequisiteExemptionsResponse.Success)
            {
                return new GenericResponse<List<CredentialPrerequisiteExemptionModel>>(null)
                {
                    Errors = new List<string>() { $"Could not get active credential prerequisite exemptions for credential request {credentialRequestId}" }
                };
            }

            // Call DAL to get the required prerequisites of the current credential request
            var credentialPrerequisitesResponse = _credentialPrerequisiteDalService.GetPrerequisitesForCredentialRequest(credentialRequestDetails);

            if (!credentialPrerequisitesResponse.Success)
            {
                return new GenericResponse<List<CredentialPrerequisiteExemptionModel>>(null)
                {
                    Errors = new List<string>() { $"Could not get credential prerequisites for credential request {credentialRequestId}" }
                };
            }

            var credentialPrerequisiteExemptionModels = new List<CredentialPrerequisiteExemptionModel>();

            var credentialPrerequisites = credentialPrerequisitesResponse.Data;

            // foreach prerequisite, match exemption if it exists with the prerequisite and pack into UI model
            foreach(var prerequisite in credentialPrerequisites)
            {
                // need to figure out skills. e.g. skill match, ethics is ethics, intercultural uses [language 1] etc
                var skillDetailsResponse = GetSkillForPrerequisite(prerequisite.ApplicationTypePrerequisiteId, prerequisite.CredentialTypePrerequisiteId, credentialRequestId);

                if (!skillDetailsResponse.Success)
                {
                    return new GenericResponse<List<CredentialPrerequisiteExemptionModel>>(null)
                    {
                        Errors = new List<string>() { $"Could not get skill for credential prerequisite {prerequisite.CredentialPrerequisiteId} for credential request {credentialRequestId}" }
                    };
                }

                var prerequisiteSkillId = skillDetailsResponse.Data.SkillId;

                if (credentialPrerequisiteExemptionsResponse.Data.Any(x => x.CredentialTypeName == prerequisite.CredentialPrerequsiteName && x.SkillId == prerequisiteSkillId))
                {
                    var exemptionDto = credentialPrerequisiteExemptionsResponse.Data.Select(x => x).Where(x => x.CredentialTypeName == prerequisite.CredentialPrerequsiteName).FirstOrDefault();

                    credentialPrerequisiteExemptionModels.Add(
                        new CredentialPrerequisiteExemptionModel()
                        {
                            PrerequisiteExemptionId = exemptionDto.CredentialPrerequisiteExemptionId,
                            PrerequisiteCredentialName = prerequisite.CredentialPrerequsiteName,
                            PrerequisiteSkill = skillDetailsResponse.Data.SkillName,
                            ExemptedCredentialName = exemptionDto.CredentialTypeName,
                            ExemptedCredentialTypeId = exemptionDto.CredentialTypeId,
                            ExemptedCredentialSkill = skillDetailsResponse.Data.SkillName,
                            ExemptedCredentialSkillId = skillDetailsResponse.Data.SkillId,
                            ExemptionStartDate = exemptionDto.StartDate.Date,
                            ExemptionGrantedByUser = exemptionDto.ModifiedUser,
                            ExemptionGrantedByUserId = exemptionDto.ModifiedUserId,
                            PersonId = exemptionDto.PersonId
                        }
                    );

                    credentialPrerequisiteExemptionsResponse.Data.Remove(exemptionDto);

                    continue;
                }

                credentialPrerequisiteExemptionModels.Add(
                    new CredentialPrerequisiteExemptionModel()
                    {
                        PrerequisiteExemptionId = 0,
                        PrerequisiteCredentialName = prerequisite.CredentialPrerequsiteName,
                        ExemptedCredentialTypeId = prerequisite.CredentialTypePrerequisiteId,
                        PrerequisiteSkill = skillDetailsResponse.Data.SkillName,
                        ExemptedCredentialSkill = skillDetailsResponse.Data.SkillName,
                        ExemptedCredentialSkillId = skillDetailsResponse.Data.SkillId,
                        PersonId = credentialRequestDetailsResponse.Data.PersonId
                    }
                );
            }

            // return the list of UI models
            return credentialPrerequisiteExemptionModels;
        }

        public GenericResponse<List<CredentialPrerequisiteExemptionModel>> GetExemptions(int naatiNumber)
        {
            var results =  _credentialPrerequisiteDalService.GetAllCredentialPrerequisiteExemptions(naatiNumber);

            if (!results.Success)
            {
                return new GenericResponse<List<CredentialPrerequisiteExemptionModel>>() { Errors = new List<string>() { "Failed to get exemptions for Person" } };
            }

            var response = new GenericResponse<List<CredentialPrerequisiteExemptionModel>>()
            {
                Data = new List<CredentialPrerequisiteExemptionModel>()
            };

            foreach (var result in results.Data)
            {
                response.Data.Add(new CredentialPrerequisiteExemptionModel()
                {
                    ExemptedCredentialName = result.CredentialTypeName,
                    ExemptedCredentialSkill = result.SkillName,
                    ExemptionStartDate = result.StartDate,
                    ExemptionEndDate = result.EndDate,
                    ExemptionGrantedByUser = result.ModifiedUser
                });
            }

            return response;
        }

        public GenericResponse<bool> HandlePrerequisiteExemptions(PrerequisiteExemptionRequest prerequisiteExemptionRequests, UserModel currentUser)
        {
            var response = new GenericResponse<bool>();
            var exemptionsToSaveOrUpdate = new List<CredentialPrerequisiteExemptionDto>();

            foreach(var exemption in prerequisiteExemptionRequests.PrerequisiteExemptions)
            {
                // if exemption is checked and does not have date then is new exemption
                if (exemption.Checked && !exemption.ExemptionStartDate.HasValue)
                {
                    exemption.ExemptedCredentialName = exemption.PrerequisiteCredentialName;
                    exemption.ExemptedCredentialSkill = exemption.PrerequisiteSkill;
                    exemption.ExemptionStartDate = DateTime.Now;
                    exemption.ExemptionGrantedByUser = currentUser.Name;
                    exemption.ExemptionGrantedByUserId = currentUser.Id;

                    exemptionsToSaveOrUpdate.Add(MapCredentialPrerequisiteExemptionToDto(exemption));

                    continue;
                }

                // if exemption is unchecked and has a date then update to expired
                if(!exemption.Checked && exemption.ExemptionStartDate.HasValue)
                {
                    exemption.ExemptionEndDate = DateTime.Now;
                    exemptionsToSaveOrUpdate.Add(MapCredentialPrerequisiteExemptionToDto(exemption));

                    continue;
                }

            }

            foreach(var exemptionToSaveOrUpdate in exemptionsToSaveOrUpdate)
            {
                var saveOrUpdateResponse = _credentialPrerequisiteDalService.SaveOrUpdateCredentialPrerequisiteExemptions(exemptionToSaveOrUpdate);

                if (!saveOrUpdateResponse.Success)
                {
                    response.Errors.Add($"Could not Save Or Update the credential prerequisite exemption with id {exemptionToSaveOrUpdate.CredentialPrerequisiteExemptionId}");
                }
            }

            return response;
        }

        public GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>> GetRelatedCredentialIdsOnHold(int childCredentialRequestId, List<OnHoldCredentialsToBeIssuedModel> parentsOnHoldToBeIssued = null)
        {
            if (parentsOnHoldToBeIssued.IsNull())
            {
                parentsOnHoldToBeIssued = new List<OnHoldCredentialsToBeIssuedModel>();
            }

            if (childCredentialRequestId == 0)
            {
                return new GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>>(null);
            }

            // get child parameters to find the parent on hold credentials
            var childParamsResponse = _credentialPrerequisiteDalService.GetChildParamsForRelatedCredentialsOnHold(childCredentialRequestId);

            if (!childParamsResponse.Success)
            {
                return new GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>>(null)
                {
                    Errors = childParamsResponse.Errors
                };
            }

            var childParams = childParamsResponse.Data;

            if(childParams.IsNull())
            {
                return new GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>>(null)
                {
                    Errors = new List<string>() { $"Could not get the child parameters for credential request {childCredentialRequestId}." }
                };
            }

            // get the parents on hold credentials using the child params
            var parentsParamsResponse = _credentialPrerequisiteDalService.GetParentParamsForRelatedCredentialsOnHold(childParamsResponse.Data);

            if (!parentsParamsResponse.Success)
            {
                return new GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>>(null)
                {
                    Errors = childParamsResponse.Errors
                };
            }

            var parentsParams = parentsParamsResponse.Data;

            // if there are no parents then return the list as it is
            if (parentsParams.Count == 0)
            {
                return parentsOnHoldToBeIssued;
            }

            // check if parent on hold meets the prerequisites or has exempted prerequisites
            foreach(var parentParams in parentsParams)
            {
                var parentElligibleToBeIssuedResponse = _credentialPrerequisiteDalService.CheckParentCredentialsOnHoldElligibleToIssue(parentParams);

                if (!parentElligibleToBeIssuedResponse.Success)
                {
                    return new GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>>(null)
                    {
                        Errors = childParamsResponse.Errors
                    };
                }

                var parentElligible = parentElligibleToBeIssuedResponse.Data;

                // if the parent is elligible to be issued add to the list for issuing and repeat the above steps for the parent using recursion
                if (parentElligible)
                {
                    parentsOnHoldToBeIssued.Add(
                        new OnHoldCredentialsToBeIssuedModel()
                        {
                            CredentialRequestId = parentParams.CredentialRequestId,
                            CredentialTypeName = parentParams.CredentialTypeName,
                            SkillDisplayName = parentParams.SkillDisplayName,
                            CredentialRequestStatusTypeId = parentParams.CredentialRequestStatusTypeId,
                            CredentialRequestStatusDisplayName = parentParams.CredentialRequestStatusTypeDisplayName,
                            CredentialApplicationId = parentParams.CredentialApplicationId,
                            CredentialApplicationStatusTypeId = parentParams.CredentialApplicationStatusTypeId,
                            CredentialApplicationStatusDisplayName = parentParams.CredentialApplicationStatusTypeDisplayName,
                            CredentialApplicationTypeDisplayName = parentParams.CredentialApplicationTypeDisplayName
                        }
                    );

                    var parentsOnHoldToBeIssuedResponse = GetRelatedCredentialIdsOnHold(parentsOnHoldToBeIssued.Last().CredentialRequestId, parentsOnHoldToBeIssued);

                    if (!parentsOnHoldToBeIssuedResponse.Success)
                    {
                        return new GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>>(null)
                        {
                            Errors = childParamsResponse.Errors
                        };
                    }

                    parentsOnHoldToBeIssued = parentsOnHoldToBeIssuedResponse.Data.ToList();
                }
            }

            // other wise return the list as it is
            return parentsOnHoldToBeIssued;
        }

        private CredentialPrerequisiteExemptionDto MapCredentialPrerequisiteExemptionToDto(PrerequisiteExemption prerequisiteExemption)
        {
            return new CredentialPrerequisiteExemptionDto()
            {
                CredentialPrerequisiteExemptionId = prerequisiteExemption.PrerequisiteExemptionId,
                CredentialTypeName = prerequisiteExemption.ExemptedCredentialName,
                CredentialTypeId = prerequisiteExemption.ExemptedCredentialTypeId,
                SkillName = prerequisiteExemption.ExemptedCredentialSkill,
                SkillId = prerequisiteExemption.ExemptedCredentialSkillId,
                PersonId = prerequisiteExemption.PersonId,
                StartDate = prerequisiteExemption.ExemptionStartDate.Value,
                EndDate = prerequisiteExemption.ExemptionEndDate ?? null,
                ModifiedDate = DateTime.Now,
                ModifiedUser = prerequisiteExemption.ExemptionGrantedByUser,
                ModifiedUserId = prerequisiteExemption.ExemptionGrantedByUserId
            };
        }

        private GenericResponse<SkillDetails> GetSkillForPrerequisite(int prerequisiteApplicationTypeId, int prerequisiteCredentialTypeId, int parentCredentialRequestId)
        {
            var response = _credentialPrerequisiteDalService.GetSkillForExemption(prerequisiteApplicationTypeId, prerequisiteCredentialTypeId, parentCredentialRequestId);

            if (!response.Success)
            {
                return new GenericResponse<SkillDetails>(null)
                {
                    Errors = { $"Failed to get prerequisite skills for the parent credential request {parentCredentialRequestId}" }
                };
            }

            return response;
        }
    }
}
