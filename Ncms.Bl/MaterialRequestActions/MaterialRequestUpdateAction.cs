using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestUpdateAction : MaterialRequestAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.InProgress, MaterialRequestStatusTypeName.AwaitingFinalisation };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => CurrentEntryState;

        private readonly IList<string> _notes = new List<string>();

        private bool _coordinatorChanged = false;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
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
                    UpdateMaterialRequestDetails,
                    AddMembers,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }

        protected override IList<ActionEventLevel> GetActionEvents()
        {
            var events = base.GetActionEvents();

            if (_coordinatorChanged)
            {
                var maxLevel = events.Max(x => x.Level);
                var coordinatorChangedEvent = new ActionEventLevel { Event = SystemActionEventTypeName.CoordinatorChanged, Level = maxLevel + 1 };
                events.Add(coordinatorChangedEvent);
            }

            return events;
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            var coordinator = PanelService.GetPanelMembershipInfo(new[] { WizardModel.Coordinator.PanelMembershipId }).Data.Single();

            TryInsertToken(tokenDictionary, TokenReplacementField.NaatiNo, () => coordinator.NaatiNumber.ToString());
            TryInsertToken(tokenDictionary, TokenReplacementField.GivenName, () => coordinator.GivenName);
            TryInsertToken(tokenDictionary, TokenReplacementField.TestMaterialTitle, () => WizardModel.OutputTestMaterial.Title);
            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialCoordinator, () => coordinator.GivenName);

            var cost = ApplicationService.GetLookupType(LookupType.MaterialSpecificationCost.ToString()).Data.First(x => x.Id == WizardModel.OutputTestMaterial.ProductSpecificationId);

            TryInsertToken(tokenDictionary, TokenReplacementField.MaterialRequestCost, () => cost.DisplayName);
            TryInsertToken(tokenDictionary, TokenReplacementField.MaxBillableHours, () => WizardModel.OutputTestMaterial.MaxBillableHours.ToString(CultureInfo.InvariantCulture));
            TryInsertToken(tokenDictionary, TokenReplacementField.ActionPublicNote, () => WizardModel.PublicNotes);
            TryInsertToken(tokenDictionary, TokenReplacementField.RoundDueDate, () => ActionModel.Rounds.OrderByDescending(x => x.RoundNumber).First().DueDate.ToShortDateString());
            base.GetEmailTokens(tokenDictionary);
        }


        protected override void AddMembers()
        {
            if ((WizardModel.Coordinator == null) || (WizardModel.Coordinator.PanelMembershipId == Coordinator.PanelMemberShipId))
            {
                return;
            }

            _coordinatorChanged = true;

            var currentCoordinator = Coordinator;
            var member = PanelService.GetPanelMembershipInfo(new[] { WizardModel.Coordinator.PanelMembershipId }).Data.Single();
            CheckPropertyChange(() => member.GivenName, () => Coordinator.GivenName, Naati.Resources.MaterialRequest.Coordinator);


            ActionModel.MaterialRequestInfo.Members.Remove(currentCoordinator);

            var existingMember = ActionModel.MaterialRequestInfo.Members.FirstOrDefault(x => x.PanelMemberShipId == member.PanelMemberShipId);
            if (existingMember != null)
            {
                existingMember.MemberTypeId = (int)MaterialRequestPanelMembershipTypeName.Coordinator;
                existingMember.Tasks = currentCoordinator.Tasks;
                Coordinator = existingMember;
            }
            else
            {
                var newCoordinator = new MaterialRequestPanelMembershipModel()
                {
                    EntityId = member.EntityId,
                    GivenName = member.GivenName,
                    Tasks = currentCoordinator.Tasks,
                    MemberTypeId = (int)MaterialRequestPanelMembershipTypeName.Coordinator,
                    NaatiNumber = member.NaatiNumber,
                    PanelMemberShipId = member.PanelMemberShipId,
                    PrimaryEmail = member.PrimaryEmail,
                    Id = currentCoordinator.Id
                };

                ActionModel.MaterialRequestInfo.Members.Add(newCoordinator);
                Coordinator = newCoordinator;
            }

        }
        protected virtual void UpdateMaterialRequestDetails()
        {
            var tittle = WizardModel.OutputTestMaterial.Title;

            if (CheckPropertyChange(() => tittle, () => ActionModel.MaterialRequestInfo.OutputTestMaterial.Title,
                nameof(ActionModel.MaterialRequestInfo.OutputTestMaterial.Title)))
            {
                ActionModel.MaterialRequestInfo.OutputTestMaterial.Title = tittle;  
            }

            var specifications = ApplicationService.GetLookupType(LookupType.MaterialRequestSpecification.ToString()).Data;
            var wizardSpecification = specifications.First(ps => ps.Id == WizardModel.OutputTestMaterial.ProductSpecificationId).DisplayName;
            var modelSpecification = specifications.First(ps => ps.Id == ActionModel.MaterialRequestInfo.ProductSpecificationId).DisplayName;

            if (CheckPropertyChange(() => wizardSpecification, () => modelSpecification,
                Naati.Resources.MaterialRequest.ProductSpecification))
            {
                ActionModel.MaterialRequestInfo.ProductSpecificationId = WizardModel.OutputTestMaterial.ProductSpecificationId;
            }

            var maxBillableHours = WizardModel.OutputTestMaterial.MaxBillableHours;
            if (CheckPropertyChange(() => maxBillableHours, () => ActionModel.MaterialRequestInfo.MaxBillableHours, Naati.Resources.MaterialRequest.MaxBillableHours))
            {
                ActionModel.MaterialRequestInfo.MaxBillableHours = maxBillableHours;
            }

            var domains = MaterialRequestService.GetTestMaterialDomains(ActionModel.MaterialRequestInfo.OutputTestMaterial.CredentialTypeId).Data.Concat(new[]{ new LookupTypeModel()
            {
                Id = (int)TestMaterialDomainTypeName.Undefined,
                DisplayName = TestMaterialDomainTypeName.Undefined.ToString()
            }}).ToList();
            
            var modelDomain = domains.First(ps => ps.Id == ActionModel.MaterialRequestInfo.OutputTestMaterial.TestMaterialDomainId).DisplayName;
            var wizardDomain = domains.First(ps => ps.Id == WizardModel.OutputTestMaterial.TestMaterialDomainId).DisplayName;

            if (CheckPropertyChange(() => wizardDomain, () => modelDomain, Naati.Resources.TestMaterial.TestMaterialDomain))
            {
                ActionModel.MaterialRequestInfo.OutputTestMaterial.TestMaterialDomainId = WizardModel.OutputTestMaterial.TestMaterialDomainId;
            }
        }

        public override IList<EmailTemplateModel> GetEmailTemplates()
        {
            AddMembers();
            return base.GetEmailTemplates();
        }

        private bool CheckPropertyChange<T>(Func<T> wizardProperty, Func<T> modelProperty, string propertyName)
        {
            if (!(wizardProperty()?.Equals(modelProperty()) ?? false))
            {
                var note = string.Format(Naati.Resources.MaterialRequest.MaterialRequestUpdated, propertyName,
                    modelProperty(), wizardProperty());
                _notes.Add(note);
                return true;
            }

            return false;
        }

        protected override string GetNote()
        {
            var entryStateName = MaterialRequestStatusTypes.Single(x => x.Id == ActionModel.MaterialRequestInfo.StatusTypeId).DisplayName;
            var exitStateName = MaterialRequestStatusTypes.Single(x => x.Id == (int)MaterialRequestExitState).DisplayName;
            if (entryStateName != exitStateName)
            {
                _notes.Add(String.Format(Naati.Resources.MaterialRequest.MaterialRquestStatusChangeNote, entryStateName, exitStateName));
            }
            return string.Join(". ", _notes);
        }
    }
}
