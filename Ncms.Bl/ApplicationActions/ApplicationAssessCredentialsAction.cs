using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationAssessCredentialsAction : ApplicationStateAction    
    {
        protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates => new[] { CredentialApplicationStatusTypeName.InProgress };
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.InProgress;
        private bool? _prerequisitesMet;
        private bool? _canProcessCredential;

        protected virtual CredentialRequestStatusTypeName RequiredCredentialRequestStatus => CredentialRequestStatusTypeName.ProcessingRequest;
        protected virtual bool PrerequisitesMet => (_prerequisitesMet = FulFillPrerequisites()).GetValueOrDefault();
        protected virtual bool AutoIssueCredential => (_canProcessCredential = CanProcessCredential()).GetValueOrDefault();
       
        protected IList<string> Messages = new List<string>();

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
        };
        
        protected override IList<Action> SystemActions => new List<Action>
        {
            ProcesssCredentials,
            ClearOwner,
            CreateNote,
            SetExitState
        };
        protected override string GetNote()
        {
            if (ApplicationExitState != CurrentEntryState)
            {
                Messages.Add(base.GetNote());
            }

            return string.Join(". ", Messages.Select(x =>
            {
                var value = x ?? string.Empty;
                if (value.EndsWith("."))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                return value;
            }).ToList());

        }

        protected override void CreateNote()
        {
            if (ApplicationExitState != CurrentEntryState || Messages.Any())
            {
                base.CreateNote();
            }
        }

        protected virtual bool FulFillPrerequisites()
        {
            return ValidateApplication() && ValidateCredentialRequests();
        }


        protected virtual bool ValidateApplication()
        {
            if (IsSponsored && !ApplicationModel.ApplicationInfo.TrustedInstitutionPayer.GetValueOrDefault())
            {
                Messages.Add(Naati.Resources.Application.ActionNotSupportedForSponsoredApplications);
                return false;
            }

            return true;
        }

        private bool CanProcessCredential()
        {
            var issuedCredentials = Convert.ToInt32(SystemService.GetSystemValue("PercentageOfCredentialIssued"));
            var maxPercentageToIssue = Math.Abs((int)Convert.ToDouble(SystemService.GetSystemValue("AutoReceritificationMaxApplicationPercentage")));

            var canIssue = issuedCredentials < maxPercentageToIssue;
     
            issuedCredentials+=10;

            if (issuedCredentials >= 100)
            {
                issuedCredentials = 0;
            }

            if (!canIssue)
            {
                Messages.Add(Naati.Resources.Application.CredentialMustBeAssessManually);
            }

            SystemService.SetSystemValue("PercentageOfCredentialIssued", issuedCredentials.ToString());
            return canIssue;
        }
        
        protected virtual bool ValidateCredentialRequests()
        {
            var credentialRequests = ApplicationModel.CredentialRequests;
            var currentCertificationPeriodId = ApplicationModel.ApplicationInfo.CertificationPeriodId;
            var credentials = PersonService.GetPersonCredentials(ApplicationModel.ApplicationInfo.NaatiNumber);
            var periodCredentials = credentials.Data.Where(x => x.CertificationPeriod?.Id == currentCertificationPeriodId).ToList();
            var naatiNumber = ApplicationModel.ApplicationInfo.NaatiNumber;
      
            foreach (var credentialRequest in credentialRequests)
            {
                var selectedCredential = periodCredentials.FirstOrDefault(x => x.SkillId == credentialRequest.SkillId && x.CredentialTypeId == credentialRequest.CredentialTypeId);
                if (selectedCredential == null)
                {
                    Messages.Add(String.Format(Naati.Resources.Application.CredentialNotFound, credentialRequest.CredentialType.InternalName, credentialRequest.Skill.DisplayName, currentCertificationPeriodId));
                    return false;
                }
                if (credentialRequest.StatusTypeId != (int)RequiredCredentialRequestStatus)
                {
                    Messages.Add(String.Format(Naati.Resources.Application.InvalidCredentialRequestStatus, RequiredCredentialRequestStatus.ToString().ToSentence()));
                    return false;
                }

                var currentPeriod = CredentialPointsCalculatorService.GetCertificationPeriodDetails(naatiNumber, selectedCredential.Id).First(x => x.Id == currentCertificationPeriodId);

                if (currentPeriod.IsCredentialSubmitted)
                {
                    var credential = CredentialPointsCalculatorService.GetCertificationPeriodCredential(naatiNumber, currentCertificationPeriodId, selectedCredential.Id);

                    if (Convert.ToDecimal(credential.Requirement) > credential.Points)
                    {
                        Messages.Add(string.Format(Naati.Resources.Application.CredentialRequirementNotMet, credential.CredentialType, credential.Skill));
                        return false;
                    }

                    if (Convert.ToDecimal(credential.Requirement) == 0)
                    {
                        Messages.Add(string.Format(Naati.Resources.Application.NotRequiredPoints, credential.CredentialType, credential.Skill));
                        return false;
                    }

                    if (!ValidateCredentialAction(credentialRequest.Id, credential))
                    {
                        return false;
                    }
                }
            }

            var pdPoints = ActivityPointsCalculatorService.CaluculatePointsFor(ApplicationModel.ApplicationInfo.NaatiNumber, currentCertificationPeriodId);
            if (!pdPoints.Completed)
            {
                Messages.Add(Naati.Resources.Application.ActivtyPointsNotFulfilled);
                return false;
            }

            if (pdPoints.MinPoints == 0)
            {
                Messages.Add(Naati.Resources.Application.ProfessionalDevelopmentDoesntHaveRequiredPoints);
                return false;
            }
            return true;
        }
        
        protected virtual void ProcesssCredentials()
        {
           
            if (ApplicationModel.ApplicantDetails.AllowAutoRecertification && PrerequisitesMet && ApplicationModel.CredentialRequests.All(x => x.StatusTypeId == (int)RequiredCredentialRequestStatus))
            {
                if (AutoIssueCredential)
                {
                    UpdateCredentialRequestStatus(CredentialRequestStatusTypeName.ToBeIssued);
                }
                else
                {
                    UpdateCredentialRequestStatus(CredentialRequestStatusTypeName.ReadyForAssessment);
                }
              
            }
            else
            {
                UpdateCredentialRequestStatus(CredentialRequestStatusTypeName.ReadyForAssessment);
            }
        }

        protected virtual bool ValidateCredentialAction(int credentialRequestId, WorkPracticeCredentialDto credential)
        {
            return true;
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            Output.UpsertResults.Messages.AddRange(Messages);

        }

        protected virtual void UpdateCredentialRequestStatus(CredentialRequestStatusTypeName status)
        {
            foreach (var credentialRequest in ApplicationModel.CredentialRequests.Where(x=> x.StatusTypeId == (int)RequiredCredentialRequestStatus))
            {
                
                Messages.Add(string.Format(Naati.Resources.Application.CredentialRequestStateChangeNote,
                    credentialRequest.CredentialType.InternalName,
                    credentialRequest.Direction,
                    credentialRequest.Status,
                    CredentialRequestStatusTypes.Single(x => x.Id == (int)status).DisplayName));

                credentialRequest.StatusTypeId = (int)status;
                credentialRequest.StatusChangeUserId = CurrentUser.Id;
                credentialRequest.StatusChangeDate = DateTime.Now;
            }
        }
    }
}
