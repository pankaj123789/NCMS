using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationCloseAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.InProgress };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.Completed;

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Close;

        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateEntryState,
                                                              ValidateUserPermissions,
                                                              ValidateCredentialRequestStates
                                                          };

        private void ValidateCredentialRequestStates()
        {
            if (!ApplicationModel.CredentialRequests.Any() || !ApplicationModel.CredentialRequests.All(x =>
                    ((CredentialRequestStatusTypeName)x.StatusTypeId).IsFinalisedStatus()))
            {
                ValidationErrors.Add(
                    new ValidationResultModel
                    {
                        Message = "To close the application, all credential requests must be finalised."
                    });
            }
        }

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              CreateNote,
                                                              SetExitState
                                                          };
    }
}
