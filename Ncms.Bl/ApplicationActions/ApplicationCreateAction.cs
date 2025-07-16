using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationCreateAction : ApplicationStateAction
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.None };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.Draft;

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Create;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                              {
                                  ValidateEntryState,
                                  ValidateUserPermissions,
                                  ValidateBackOfficeApplication
                              };

                if (ApplicationModel.ApplicationType.Category == CredentialApplicationTypeCategoryName.Recertification)
                {
                    actions.Add(ValidateEligibleForRecertification);
                }

                return actions;
            }
        }

        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>
                    {
                        SetOwner,
                        SetReceivingOffice,
                        SetEnteredDetails,
                        SetApplicantOwner,
                        CreateNote,
                        SetExitState
                    };

                if (ApplicationModel.ApplicationType.Category == CredentialApplicationTypeCategoryName.Recertification)
                {
                    actions.Add(CreateRecertificationAndCredentialRequests);
                }

                return actions;
            }
        }

        private void ValidateBackOfficeApplication()
        {
            if (!ApplicationType.BackOffice)
            {
                throw new UserFriendlySamException("This action is only applicable to Back Office applications.");
            }
        }

        /// <summary>
        /// UC-BackOffice-5000 BR37
        /// </summary>
        private void ValidateEligibleForRecertification()
        {
            var credentials = PersonService.GetPersonCredentials(ApplicationModel.ApplicationInfo.NaatiNumber).Data;

            var hasEligibleCredential = credentials?.Any(x => x.RecertificationStatus == RecertificationStatus.EligibleForNew) == true;
            var hasExpiredCredential = credentials?.Any(x => x.RecertificationStatus == RecertificationStatus.None &&
                                                             x.Status.ParseEnum<CredentialStatusTypeName>() == CredentialStatusTypeName.Expired) == true;
            var valid = hasEligibleCredential ||
                        WizardModel.Source == SystemActionSource.Ncms && hasExpiredCredential;

            if (!valid)
            {
                ValidationErrors.Add(new ValidationResultModel { Message = "No credentials are eligible for recertification." });
            }
        }

        private void SetEnteredDetails()
        {
            ApplicationModel.ApplicationInfo.EnteredUserId = CurrentUser.Id;
            ApplicationModel.ApplicationInfo.EnteredDate = DateTime.Now;
        }

        protected override string GetNote()
        {
            return Naati.Resources.Application.ApplicationCreatedNote;
        }

        protected virtual void SetApplicantOwner()
        {
            ApplicationModel.ApplicationInfo.OwnedByApplicant =
                WizardModel.Source == SystemActionSource.MyNaati;
        }

        /// <summary>
        /// UC-BackOffice-5000 BR38
        /// </summary>
        private void CreateRecertificationAndCredentialRequests()
        {
            var credentials = PersonService.GetPersonCredentials(ApplicationModel.ApplicationInfo.NaatiNumber).Data;
            IGrouping<DateTime, CredentialModel> eligibleCredentials = credentials.Where(x => x.RecertificationStatus == RecertificationStatus.EligibleForNew)
                .GroupBy(x => x.CertificationPeriod.StartDate)
                .OrderBy(x => x.Key)
                .FirstOrDefault();

            if (eligibleCredentials == null || !eligibleCredentials.Any())
            {
                throw new UserFriendlySamException("This Applicant has no certifications eligible for recertification.");
            }

            // for each  Credential that is eligible for Recertifcation, ensure a Credential Request is created in this application.
                Output.PendingCredentialRequests = eligibleCredentials
                .Select(x => new CredentialRequestModel
                             {
                                 CredentialTypeId = x.CredentialTypeId,
                                 CategoryId = x.CategoryId,
                                 SkillId = x.SkillId,
                                 CredentialId = x.Id
                             })
                .ToList();

            // ensure a Recertification record is created. this links the Recertification CredentialApplication to the Certification Period
            ApplicationModel.Recertification =
                new RecertificationModel
                {
                    CertificationPeriodId = eligibleCredentials.First().CertificationPeriod.Id,
                };
        }

        protected override void LogAction()
        {
            LoggingHelper.LogInfo("Performing workflow action {Action} for NAATI #{NaatiNumber}",
                (SystemActionTypeName)WizardModel.ActionType, ApplicationModel.ApplicationInfo.NaatiNumber);
        }

    }
}