using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestAssignTestMaterialAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestMaterial;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Assign;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[]
        {
            CredentialRequestStatusTypeName.TestSessionAccepted,
        };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => GetExitState();

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateTestMaterials
        };

        private CredentialRequestStatusTypeName GetExitState()
        {
            return (CredentialRequestStatusTypeName) CredentialRequestModel.StatusTypeId;
        }

        protected override IList<Action> SystemActions => new List<Action>
        {

            AssignTestMaterials,
            CreateNote,
            SetExitState,
        };

        protected virtual void ValidateTestMaterials()
        {
            if (!WizardModel.TestMaterialAssignments.Any())
            {
                throw  new Exception("No Test materials has been assigned");
            }
        }

        protected override void CreateNote()
        {
            
            foreach (var material in TestSessionModel.Materials)
            {
                var noteModel = new ApplicationNoteModel
                {
                    ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                    CreatedDate = DateTime.Now,
                    Note = string.Empty,
                    UserId = CurrentUser.Id,
                    ReadOnly = true
                };
                if (material.ObjectStatusId == (int)ObjectStatusTypeName.Created)
                {
                    var componentName = TestSpecificationComponents.First(x => x.Id == material.TestTaskId).TaskName;
                    noteModel.Note = string.Format(Naati.Resources.TestMaterial.TestMaterialAddedMessage,
                        material.TestMaterialId, componentName);

                    ApplicationModel.Notes.Add(noteModel);
                }
                else if(material.ObjectStatusId == (int)ObjectStatusTypeName.Deleted)
                {
                    var componentName = TestSpecificationComponents.First(x => x.Id == material.TestTaskId).TaskName;
                    noteModel.Note = string.Format(Naati.Resources.TestMaterial.TestMaterialDeletedMessage,
                        material.TestMaterialId, componentName);
                    ApplicationModel.Notes.Add(noteModel);
                }

            }
        }

        protected  virtual void AssignTestMaterials()
        {
            var testSessionModel = TestSessionModel;
            var materiasToOverride =
                testSessionModel.Materials.Where(
                    x => WizardModel.TestMaterialAssignments.Any(a => a.TestTaskId == x.TestTaskId));
        
            materiasToOverride.ForEach(x=> x.ObjectStatusId = (int)ObjectStatusTypeName.Deleted);
            foreach (var assignment in WizardModel.TestMaterialAssignments)
            {
                testSessionModel.Materials.Add(new TestSittingMaterialModel
                {
                    TestTaskId = assignment.TestTaskId,
                    TestMaterialId = assignment.TestMaterialId,
                    ObjectStatusId = (int)ObjectStatusTypeName.Created
                });
            }
        }
    }
}
    