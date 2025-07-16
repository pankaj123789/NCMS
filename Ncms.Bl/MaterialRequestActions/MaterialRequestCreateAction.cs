using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestCreateAction : MaterialRequestRoundCreateAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.None };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => MaterialRequestStatusTypeName.InProgress;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    CreateRound,
                    ValidateEntryState,
                };


                return actions;
            }
        }

        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>
                {
                    PopulateRoundDetails,
                    ValidateCoordinator,
                    ValidateOutputTestMaterial,
                    ValidateSourceTestMaterial,
                    CreateTestMaterial,
                    CreateSourceTestMaterial,
                    AddNewRound,
                    AddMembers,
                    AddDocuments,
                    AddRoundLinks,
                    SetCreatedDetails,
                    SetRequestedDate,
                    CreateNote,
                    SetOwner,
                    CreatePublicNote,
                    SetExitState
                };

                return actions;
            }
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {

            var coordinator = PanelService.GetPanelMembershipInfo(new[] { WizardModel.Coordinator.PanelMembershipId }).Data.Single();

            TryInsertToken(tokenDictionary, TokenReplacementField.NaatiNo, () => coordinator.NaatiNumber.ToString());
            TryInsertToken(tokenDictionary, TokenReplacementField.GivenName, () => coordinator.GivenName);
            TryInsertToken(tokenDictionary, TokenReplacementField.CoordinatorEmail, () => coordinator.PrimaryEmail);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestMaterialTitle, () => WizardModel.OutputTestMaterial.Title);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestMaterialId, () => (ActionModel.MaterialRequestInfo.OutputTestMaterial?.Id ?? 0) <= 0 ? "{{To be created}}" : ActionModel.MaterialRequestInfo.OutputTestMaterial?.Id.ToString());

            var testComponentType = MaterialRequestService
                .GetTestComponentType(WizardModel.OutputTestMaterial.TaskTypeId)
                .Data;

            string language;

            if (WizardModel.OutputTestMaterial.SkillId > 0)
            {
                language = ApplicationService
                    .GetSkillsForCredentialType(new[] { testComponentType.CredentialTypeId }, new int[] { })
                    .Data.First(x => x.Id == WizardModel.OutputTestMaterial.SkillId)
                    .DisplayName;
            }
            else
            {
                language = ApplicationService.GetLookupType(LookupType.Languages.ToString()).Data.First(x => x.Id == WizardModel.OutputTestMaterial.LanguageId).DisplayName;
            }
            TryInsertToken(tokenDictionary, TokenReplacementField.CredentialType, () => testComponentType.CredentialType);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestTaskDescription, () => testComponentType.Description);
            TryInsertToken(tokenDictionary, TokenReplacementField.Language, () => language);
            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialCoordinator, () => coordinator.GivenName);

            var cost = ApplicationService.GetLookupType(LookupType.MaterialSpecificationCost.ToString()).Data.First(x => x.Id == WizardModel.OutputTestMaterial.ProductSpecificationId);

            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestCost, () => cost.DisplayName);
            TryInsertToken(tokenDictionary, TokenReplacementField.MaxBillableHours, () => WizardModel.OutputTestMaterial.MaxBillableHours.ToString(CultureInfo.InvariantCulture));
            TryInsertToken(tokenDictionary, TokenReplacementField.ActionPublicNote, () => WizardModel.PublicNotes);

            var userId = UserService.Get().Id;

            var user = UserService.GetUserDetailsById(userId).Data;
          
            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestOwner, () => user.FullName);

            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestOwnerEmail, () => user.Email);
            base.GetEmailTokens(tokenDictionary);
        }

        protected virtual void ValidateCoordinator()
        {
            if (WizardModel.Coordinator == null || WizardModel.Coordinator.PanelMembershipId <= 0)
            {
                throw new ArgumentException(nameof(WizardModel.Coordinator));
            }
        }
        private void CreateTestMaterial()
        {
            ActionModel.MaterialRequestInfo.OutputTestMaterial = new TestMaterialModel()
            {
                TestMaterialTypeId = WizardModel.OutputTestMaterial.TestMaterialTypeId,
                Title = WizardModel.OutputTestMaterial.Title,
                TypeId = WizardModel.OutputTestMaterial.TaskTypeId,
                Available = false,
                LanguageId = WizardModel.OutputTestMaterial.LanguageId,
                SkillId = WizardModel.OutputTestMaterial.SkillId,
                TestMaterialDomainId = WizardModel.OutputTestMaterial.TestMaterialDomainId
            };

            ActionModel.MaterialRequestInfo.PanelId = WizardModel.OutputTestMaterial.PanelId;
            ActionModel.MaterialRequestInfo.MaxBillableHours = WizardModel.OutputTestMaterial.MaxBillableHours;
            ActionModel.MaterialRequestInfo.ProductSpecificationId = WizardModel.OutputTestMaterial.ProductSpecificationId;
            ActionModel.MaterialRequestInfo.Members = new List<MaterialRequestPanelMembershipModel>();

        }

        private void CreateSourceTestMaterial()
        {
            if (WizardModel.TestMaterialSource != null && WizardModel.TestMaterialSource.TestMaterialId > 0)
            {
                ActionModel.MaterialRequestInfo.SourceTestMaterial = new TestMaterialModel()
                {
                    Id = WizardModel.TestMaterialSource.TestMaterialId
                };
            }

        }

        protected override void SetExitState()
        {
            ActionModel.MaterialRequestInfo.StatusTypeId = (int)MaterialRequestExitState;
            ActionModel.MaterialRequestInfo.StatusChangeUserId = CurrentUser.Id;
            ActionModel.MaterialRequestInfo.StatusChangeDate = DateTime.Now;
            base.SetExitState();
        }

        protected override void ValidateEntryState()
        {
            if (!MaterialRequestEntryStates.Contains((MaterialRequestStatusTypeName)ActionModel.MaterialRequestInfo.StatusTypeId))
            {
                var entryStateNames = MaterialRequestEntryStates.Select(x => MaterialRequestStatusTypes.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(String.Format(Naati.Resources.MaterialRequest.WrongMaterialRequestStatusErrorMessage,
                    string.Join(", ", entryStateNames)));
            }

            base.ValidateEntryState();
        }

        private void SetCreatedDetails()
        {
            ActionModel.MaterialRequestInfo.CreatedByUserId = CurrentUser.Id;
            ActionModel.MaterialRequestInfo.CreatedDate = DateTime.Now;

        }
        protected void ValidateSourceTestMaterial()
        {
            var sourceTestMaterial = WizardModel.TestMaterialSource;
            if (WizardModel.OutputTestMaterial == null ||
                WizardModel.OutputTestMaterial.TestMaterialTypeId == 0)
            {
                throw new ArgumentNullException(nameof(WizardModel.TestMaterialSource));
            }
            if (sourceTestMaterial?.TestMaterialId > 0)
            {
                var testMaterial = TestMaterialService.GetTestMaterials(sourceTestMaterial.TestMaterialId);
                if (testMaterial == null)
                {
                    throw new ArgumentNullException(nameof(WizardModel.TestMaterialSource));
                }
            }

        }

        protected override IEnumerable<MaterialRequestEmailMessageModel> GetEmails(EmailTemplateModel template, MaterialRequestEmailMessageModel baseEmail)
        {
            if (Coordinator == null)
            {
                CreateTestMaterial();
                AddMembers();
                Coordinator = ActionModel.MaterialRequestInfo.Members.First(x => x.MemberTypeId == (int)MaterialRequestPanelMembershipTypeName.Coordinator);
            }
            return base.GetEmails(template, baseEmail);
        }


        protected virtual void ValidateOutputTestMaterial()
        {
            var outputMaterial = WizardModel.OutputTestMaterial;
            if (outputMaterial == null)
            {
                throw new ArgumentNullException(nameof(WizardModel.OutputTestMaterial));
            }

            var panelId = outputMaterial.PanelId;

            if (panelId <= 0)
            {
                throw new ArgumentException(nameof(WizardModel.OutputTestMaterial.PanelId));
            }

            var panel = PanelService.GetPanel(panelId).Data;

            if (panel == null)
            {
                throw new ArgumentNullException(nameof(WizardModel.OutputTestMaterial.PanelId));
            }

            if (outputMaterial.TestMaterialTypeId == 0)
            {
                throw new ArgumentException(nameof(outputMaterial.TestMaterialTypeId));
            }

            if (outputMaterial.SkillId == 0 && outputMaterial.LanguageId == 0)
            {
                throw new ArgumentException(nameof(outputMaterial.SkillId));
            }
            if (outputMaterial.TaskTypeId == 0)
            {
                throw new ArgumentException(nameof(outputMaterial.TaskTypeId));
            }

            if (outputMaterial.ProductSpecificationId == 0)
            {
                throw new ArgumentException(nameof(outputMaterial.ProductSpecificationId));
            }

            if (string.IsNullOrWhiteSpace(outputMaterial.Title))
            {
                throw new ArgumentException(nameof(outputMaterial.Title));
            }

            if (outputMaterial.MaxBillableHours < 0)
            {
                throw new ArgumentException(nameof(WizardModel.OutputTestMaterial.MaxBillableHours));
            }
        }

        protected override string GetNote()
        {
            return string.Join(". ", new string[] { Naati.Resources.MaterialRequest.MaterialRequestCreatedNote, base.GetNote() });
        }

        protected override GenericResponse<UpsertMaterialRequestResultModel> SaveActionData()
        {
            var result = base.SaveActionData();

            WizardModel.MaterialRequestId = result.Data.MaterialRequestIds.First();
            ActionModel.MaterialRequestInfo.MaterialRequestId = result.Data.MaterialRequestIds.First();
            ActionModel.MaterialRequestInfo.OutputTestMaterial.Id = result.Data.TestMaterialIds.OrderByDescending(x => x).First();
            return result;
        }
    }
}
