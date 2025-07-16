using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using System;
using System.Linq;
using System.Collections.Generic;
using Ncms.Contracts.Models.Application;
using Newtonsoft.Json;
using F1Solutions.Naati.Common.Dal;
using static F1Solutions.Naati.Common.Contracts.Dal.CreatePrerequisiteApplicationsDalModel;
using F1Solutions.Naati.Common.Contracts.Dal;
using Ncms.Contracts;
using System.Text;
using F1Solutions.Naati.Common.Contracts.Dal.Request;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCreatePreRequisiteApplicationsAction : CredentialRequestStateAction
    {
        protected ICredentialPrerequisiteQueryService CredentialPrerequisiteQueryService => ServiceLocatorInstance.Resolve<ICredentialPrerequisiteQueryService>();
        protected ICredentialPrerequisiteService CredentialPrerequisiteService => ServiceLocatorInstance.Resolve<ICredentialPrerequisiteService>();
        protected IPersonQueryService PersonQueryService => ServiceLocatorInstance.Resolve<IPersonQueryService>();
        protected IApplicationQueryService ApplicationQueryService => ServiceLocatorInstance.Resolve<IApplicationQueryService>();
        protected IFileStorageService FileSystemFileStorageService => ServiceLocatorInstance.Resolve<IFileStorageService>();


        ICredentialPrerequisiteDalService PrerequisiteDalService => new CredentialPrerequisiteDalService(ApplicationQueryService, PersonQueryService, CredentialPrerequisiteQueryService, FileSystemFileStorageService);

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.Pending, CredentialRequestStatusTypeName.BeingAssessed, CredentialRequestStatusTypeName.EligibleForTesting,
                                                                                                     CredentialRequestStatusTypeName.TestSessionAccepted, CredentialRequestStatusTypeName.TestAccepted };

        protected List<UpsertCredentialApplicationRequest> UpsertCredentialApplicationRequests;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState
        };

        protected CreatePrerequisiteRequest CreatePrerequisiteRequest;
        protected CredentialRequestModel CurrentCredentialRequestModel;

        protected override IList<Action> SystemActions => new List<Action>
        {
            ConstructAndSavePrerequisites,
            HandlePrerequisiteExemptions,
            CreatePendingEmailIfApplicable,
            SetExitState
        };

        public override void SaveChanges()
        {
            //need to overrider to rpevent other actions occurring
        }

        /// <summary>
        /// this is not ideal buit the whole email generation logic relies on the current cred req as being the 
        /// one you want to produce emails for. WHich is not the case here.
        /// So the code substitutes the correct tokens for each created cred req.
        /// </summary>
        protected override void CreatePendingEmailIfApplicable()
        {
            foreach (var UpsertCredentialApplicationRequest in UpsertCredentialApplicationRequests)
            {
                WizardModel.ApplicationId = UpsertCredentialApplicationRequest.ApplicationId;
                ApplicationModel.ApplicationType.Id = UpsertCredentialApplicationRequest.ApplicationTypeId;
                var hasCredentialRequest = UpsertCredentialApplicationRequest.CredentialRequests.FirstOrDefault();
                if (hasCredentialRequest != null)
                {
                    var createdCredRequest = UpsertCredentialApplicationRequest.CredentialRequests.First();
                    var credentialType = ApplicationQueryService.GetCredentialTypeById(createdCredRequest.CredentialTypeId);

                    var wizardCredRequest = ApplicationModel.CredentialRequests.First();
                    wizardCredRequest.CredentialTypeId = createdCredRequest.CredentialTypeId;
                    wizardCredRequest.CredentialType.Id = credentialType.CredentialTypeId;
                    wizardCredRequest.CredentialType.ExternalName = credentialType.DisplayName;

                    wizardCredRequest.ExternalCredentialName = credentialType.DisplayName;
                    // wizardCredRequest.Direction = createdCredRequest.  //??????
                    WizardModel.CredentialRequestId = createdCredRequest.CredentialId.Value;
                    base.CreatePendingEmailIfApplicable();

                    if (Output.PendingEmails != null)
                    {
                        foreach (var pendingEmail in Output.PendingEmails)
                        {
                            var emailCreateResponse = SaveEmailMessage(pendingEmail);

                            if (!emailCreateResponse.Success)
                            {
                                throw new Exception(emailCreateResponse.Errors.FirstOrDefault());
                            }
                        }
                    }
                }
            }
        }

        private void ConstructAndSavePrerequisites()
        {
            UpsertCredentialApplicationRequests = new List<UpsertCredentialApplicationRequest>();

            var prerequisiteDataIndex = -1;
            var count = 0;

            foreach (var data in WizardModel.Data)
            {
                // Step Id for Create Prerequisite Credentials is 21
                if (data.Id == 21)
                {
                    prerequisiteDataIndex = count;
                    break;
                }

                count++;
            }

            if (prerequisiteDataIndex == -1)
            {
                return;
            }

            var prerequisiteData = JsonConvert.DeserializeObject<CreatePrerequisiteRequest>(WizardModel.Data[count].Data.ToString());

            CreatePrerequisiteRequest = prerequisiteData;
            // if create applications is false then do not create applications
            if (!CreatePrerequisiteRequest.CreateApplications)
            {
                return;
            }

            //for testing Entered User Id is already supplied. If it isnt populate it
            if (CreatePrerequisiteRequest.EnteredUserId == 0)
            {
                CreatePrerequisiteRequest.EnteredUserId = CurrentUser.Id;
            }

            var childPrerequisiteData = CreatePrerequisiteRequest.CredentialRequestTypes;
            if (!(childPrerequisiteData.Count != 0))
            {
                return;
            }

            CredentialRequestModel = this.ApplicationModel.CredentialRequests.First(x => x.Id == CreatePrerequisiteRequest.ParentCredentialRequestId);

            //create Application and CredRequest data structure
            var fieldDataList = new List<ApplicationField>();

            foreach (var field in ApplicationModel.Fields)
            {
                fieldDataList.Add(new ApplicationField()
                {
                    Name = field.Name,
                    Id = field.Id,
                    FieldDataId = field.FieldDataId,
                    FieldTypeId = field.FieldTypeId,
                    FieldOptionId = field.FieldOptionId,
                    Value = field.Value
                });
            }

            foreach (var preReqRequest in CreatePrerequisiteRequest.CredentialRequestTypes)
            {
                CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel = new CreatePrerequisiteApplicationsDalModel()
                {
                    CredentialRequestId = CredentialRequestModel.Id,
                    CredentialRequestSkillDirectionId = CredentialRequestModel.DirectionTypeId,
                    CredentialRequestSkillId = CredentialRequestModel.Skill.Id,
                    CredentialRequestSkillLanguage1 = CredentialRequestModel.Skill.Language1Name,
                    CredentialRequestSkillLanguage2 = CredentialRequestModel.Skill.Language2Name,
                    CredentialRequestSkillTypeId = CredentialRequestModel.CredentialType.SkillType,
                    ApplicationPersonId = ApplicationModel.ApplicantDetails.PersonId,
                    ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                    ApplicationFields = fieldDataList,
                    ParentApplicationId = CreatePrerequisiteRequest.ParentApplicationId,
                    ParentCredentialRequestId = CreatePrerequisiteRequest.ParentCredentialRequestId,
                    ChildCredentialRequestType = preReqRequest,
                    EnteredUserId = CreatePrerequisiteRequest.EnteredUserId,
                    PreferredTestLocationId = ApplicationModel.ApplicationInfo.PreferredTestLocationId,
                };

                var prereqApplicationModel = PrerequisiteDalService.CreateApplicationAndCredentialRequest(createPrerequisiteApplicationsDalModel);

                foreach (var dalAttachment in prereqApplicationModel.Attachments)
                {
                    var createOrReplaceApplicationAttachmentRequest = AutoMapperHelper.Mapper.Map<CreateOrReplaceApplicationAttachmentRequest>(dalAttachment);
                    createOrReplaceApplicationAttachmentRequest.PrerequisiteApplicationDocument = true;
                    prereqApplicationModel.UpsertCredentialApplicationRequest.Attachments.Add(createOrReplaceApplicationAttachmentRequest);
                }

                var response = PrerequisiteDalService.UpsertApplicationAndCredentialRequest(prereqApplicationModel.UpsertCredentialApplicationRequest);

                prereqApplicationModel.UpsertCredentialApplicationRequest.ApplicationId = response.CredentialApplicationId;

                if(response.CredentialRequestIds.Count() > 0)
                {
                    prereqApplicationModel.UpsertCredentialApplicationRequest.CredentialRequests.First().CredentialId = response.CredentialRequestIds.First();
                }
                UpsertCredentialApplicationRequests.Add(prereqApplicationModel.UpsertCredentialApplicationRequest);
            }
        }
        private void HandlePrerequisiteExemptions()
        {
            var exemptionDataIndex = -1;
            var count = 0;

            foreach (var data in WizardModel.Data)
            {
                // Step Id for Prerequisite Exemptions is 21
                if (data.Id == 23)
                {
                    exemptionDataIndex = count;
                    break;
                }

                count++;
            }

            if (exemptionDataIndex == -1)
            {
                return;
            }

            var exemptionData = JsonConvert.DeserializeObject<Contracts.Models.CredentialPrerequisite.PrerequisiteExemptionRequest>(WizardModel.Data[exemptionDataIndex].Data.ToString());

            var response = CredentialPrerequisiteService.HandlePrerequisiteExemptions(exemptionData, CurrentUser);

            if (!response.Success)
            {
                var errors = new StringBuilder();

                foreach(var error in response.Errors)
                {
                    errors.AppendLine(error);
                }

                throw new Exception($"Could not save or update the credential prerequisite exemptions for person {exemptionData.PersonId}. Cause: {errors}");  
            }
        }
    }
}