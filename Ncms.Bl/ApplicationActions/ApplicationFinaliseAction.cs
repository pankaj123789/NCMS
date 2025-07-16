using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationFinaliseAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.InProgress };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => ApplicationModel.ApplicationStatus.Id == (int)CredentialApplicationStatusTypeName.Draft ?
                                                                     CredentialApplicationStatusTypeName.Deleted : CredentialApplicationStatusTypeName.Completed;

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              ClearOwner,
                                                              CreateNote,
                                                              SetExitState,
                                                              LockApplication,
                                                          };

        private void LockApplication()
        {
            // todo 
        }
    }
}