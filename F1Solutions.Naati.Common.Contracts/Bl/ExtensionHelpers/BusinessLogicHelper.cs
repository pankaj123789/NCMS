using System;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers
{
    public static class BusinessLogicHelper
    {
        public static bool IsActiveStatus(this CredentialApplicationStatusTypeName applicationStatus)
        {
            return
                applicationStatus != CredentialApplicationStatusTypeName.None &&
                applicationStatus != CredentialApplicationStatusTypeName.Draft &&
                applicationStatus != CredentialApplicationStatusTypeName.Deleted &&
                applicationStatus != CredentialApplicationStatusTypeName.Completed &&
                applicationStatus != CredentialApplicationStatusTypeName.Rejected;
        }

        public static bool IsFinalisedStatus(this CredentialRequestStatusTypeName credentialRequestStatus)
        {
            return
                credentialRequestStatus == CredentialRequestStatusTypeName.AssessmentFailed ||
                credentialRequestStatus == CredentialRequestStatusTypeName.TestFailed ||
                credentialRequestStatus == CredentialRequestStatusTypeName.Cancelled ||
                credentialRequestStatus == CredentialRequestStatusTypeName.CertificationIssued ||
                credentialRequestStatus == CredentialRequestStatusTypeName.Deleted ||
                credentialRequestStatus == CredentialRequestStatusTypeName.Withdrawn;
        }

        public static int[] GetInvalidRecertificationCredentialRequestStatuses()
        {
            var invalidStatuses = new[]
            {
                (int) CredentialRequestStatusTypeName.Deleted, (int) CredentialRequestStatusTypeName.Rejected,
                (int) CredentialRequestStatusTypeName.Withdrawn, (int) CredentialRequestStatusTypeName.Cancelled,
                (int) CredentialRequestStatusTypeName.Draft,
            };

            return invalidStatuses;
        }

        public static int[] GetNonSubmittedApplicationStatuses()
        {
            return new[]
            {
                (int)CredentialApplicationStatusTypeName.Deleted,
                (int)CredentialApplicationStatusTypeName.Draft
            };
        }

        public static int[] GetNonActiveApplicationStatuses()
        {
            return new[]
            {
                (int)CredentialApplicationStatusTypeName.Draft,
                (int)CredentialApplicationStatusTypeName.Rejected,    
                (int)CredentialApplicationStatusTypeName.Completed,    
                (int)CredentialApplicationStatusTypeName.Deleted,    
            };
        }

        public static int[] GetInvalidRecertificationApplicationStatuses()
        {
            var invalidapplicationStatuses = new[]
            {
                (int)CredentialApplicationStatusTypeName.Deleted, (int)CredentialApplicationStatusTypeName.Rejected,
                (int)CredentialApplicationStatusTypeName.Draft
            };

            return invalidapplicationStatuses;
        }

        public static CertificationPeriodStatus GetCertificationPeriodStatus(DateTime startDate, DateTime endDate)
        {
            if (endDate.Date < DateTime.Now.Date)
            {
                return CertificationPeriodStatus.Expired;
            }

            if (startDate.Date <= DateTime.Now.Date && DateTime.Now.Date <= endDate.Date)
            {
                return CertificationPeriodStatus.Current;
            }

            if (startDate.Date > DateTime.Now.Date)
            {
                return CertificationPeriodStatus.Future;
            }

            return CertificationPeriodStatus.None;

        }
    }
}