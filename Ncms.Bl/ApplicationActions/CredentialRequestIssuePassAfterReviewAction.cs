using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestIssuePassAfterReviewAction : CredentialRequestIssuePassAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.UnderPaidTestReview };
    }
}
