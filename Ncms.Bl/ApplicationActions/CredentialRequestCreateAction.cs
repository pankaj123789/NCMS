using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestCreateAction : CredentialRequestStateAction
    {
        protected override IList<Action> Preconditions => new List<Action>
                                                          {
                                                              ValidateUserPermissions,
                                                              ValidateApplicationState,
                                                          };

        protected override IList<Action> SystemActions => new List<Action>
                                                          {
                                                              SetOwner,
                                                              CreateCredentialRequest,
                                                              AttachWorkPractices,
                                                              CreateNote,
                                                          };

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Manage;
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Application;
        protected override void ValidateApplicationState()
        {
            if (ApplicationModel.ApplicationStatus.Id != (int)CredentialApplicationStatusTypeName.Draft
                && ApplicationModel.ApplicationStatus.Id != (int)CredentialApplicationStatusTypeName.Entered
                && ApplicationModel.ApplicationStatus.Id != (int)CredentialApplicationStatusTypeName.BeingChecked
                && ApplicationModel.ApplicationStatus.Id != (int)CredentialApplicationStatusTypeName.InProgress)
            {
                var status = CredentialApplicationStatusTypes.Single(x => x.Id == ApplicationModel.ApplicationStatus.Id).DisplayName;
                throw new UserFriendlySamException($"A credential request cannot be entered while the Application is in the {status} state.");
            }
        }

        protected override string GetNote()
        {
            var credentialTypeName = CredentialTypes.Single(x => x.Id == CredentialRequestModel.CredentialTypeId).DisplayName;
            var statusTypeName = CredentialRequestStatusTypes.Single(x => x.Id == CredentialRequestModel.StatusTypeId).DisplayName;
            var skillTypeName = SKillTypes.Single(x => x.Id == CredentialRequestModel.SkillId).DisplayName;

            return String.Format(Naati.Resources.Application.CredentialRequestCreatedNote,
                credentialTypeName,
                skillTypeName,
                statusTypeName);
        }

        private void CreateCredentialRequest()
        {
            if (ApplicationModel.CredentialRequests == null)
            {
                ApplicationModel.CredentialRequests = new List<CredentialRequestModel>();
            }

            var credentialTypeId = WizardModel?.CredentialTypeId;
            if (credentialTypeId == null)
            {
                throw new UserFriendlySamException("You must specify a credential type.");
            }

            var skillId = WizardModel?.SkillId;
            if (skillId == null)
            {
                throw new UserFriendlySamException("You must specify language(s) and direction.");
            }

            var categoryId = WizardModel?.CategoryId;
            if (categoryId == null)
            {
                throw new UserFriendlySamException("Category was not specified");
            }

            var pathType = GetCredentialRequestPathType(skillId.GetValueOrDefault(), categoryId.GetValueOrDefault(), credentialTypeId.GetValueOrDefault());

            CredentialRequestModel = new CredentialRequestModel
            {
                Id = 0,
                CredentialTypeId = credentialTypeId.Value,
                SkillId = skillId.Value,
                StatusChangeUserId = CurrentUser.Id,
                StatusChangeDate = DateTime.Now,
                CredentialRequestPathTypeId = (int)pathType,
                CredentialId = WizardModel.CredentialId,
                WorkPractices = new List<WorkPracticeDataModel>()
            };

            //BR12 - UC5000 - added the logic below
            if (ApplicationModel.ApplicationStatus.Id == (int) CredentialApplicationStatusTypeName.Draft)
            {
                CredentialRequestModel.StatusTypeId = (int) CredentialRequestStatusTypeName.Draft;
            }
            else if (ApplicationModel.ApplicationStatus.Id != (int) CredentialApplicationStatusTypeName.InProgress)
            {
                CredentialRequestModel.StatusTypeId = (int) CredentialRequestStatusTypeName.RequestEntered;
            }
            else
            {
                if (ApplicationType.RequiresAssessment)
                {
                    CredentialRequestModel.StatusTypeId = (int)CredentialRequestStatusTypeName.ReadyForAssessment;
                }
                else
                {
                    var hasTest = ApplicationService.HasTest(ApplicationModel.ApplicationType.Id, credentialTypeId.Value); 
                    var hasTestFee = ApplicationService.HasTestFee(ApplicationModel.ApplicationType.Id, credentialTypeId.Value);

                    if ( hasTest && hasTestFee)
                    {
                        CredentialRequestModel.StatusTypeId = (int)CredentialRequestStatusTypeName.EligibleForTesting;
                    }
                    else if (hasTest && !hasTestFee)
                    {
                        CredentialRequestModel.StatusTypeId = (int)CredentialRequestStatusTypeName.TestAccepted;
                    }
                    else
                    {
                        CredentialRequestModel.StatusTypeId = (int)CredentialRequestStatusTypeName.AssessmentComplete;
                    }
                }
            }

            ApplicationModel.CredentialRequests.Add(CredentialRequestModel);
        }

        private CredentialRequestPathTypeName GetCredentialRequestPathType(int newSkillId, int categoryId, int newCredentialTypeId)
        {
            if (ApplicationModel.ApplicationType.Category == CredentialApplicationTypeCategoryName.Recertification)
            {
                // the only time we create a CredentialRequest for an existing Credential is when it's a Recertification
                return CredentialRequestPathTypeName.Recertify;
            }

            var newSkill = ApplicationService.GetSkillsForCredentialType(new[] { newCredentialTypeId }, null).Data.First(x => x.Id == newSkillId);

            var sameCategoryCredentials = GetAllPersonCredentialRequests()
                .Where(x => x.CategoryId == categoryId && x.Credentials.Any(y => y.StatusId == (int)CredentialStatusTypeName.Active))
                .ToList();

            if (!sameCategoryCredentials.Any())
            {
                return CredentialRequestPathTypeName.NewSkill;
            }


            var validUpgradeCredentialsTypes = ApplicationService.GetValidCredentialTypeUpgradePaths(new CredentialPathRequestModel
            {
                CredentialTypesIdsFrom = sameCategoryCredentials.Select(x => x.CredentialTypeId)
            });


            foreach (var currentCredential in sameCategoryCredentials)
            {
                foreach (var upgradeCredential in validUpgradeCredentialsTypes.Data.Where(x => x.CredentialTypeFromId == currentCredential.CredentialTypeId))
                {
                    // Note: used display name to compare in order to support different DirectionTypes ( L1toL2 and L2TOL1)
                    if (upgradeCredential.CredentialTypeToId == newCredentialTypeId && ((upgradeCredential.MatchDirection && currentCredential.Skill.DisplayName == newSkill.DisplayName)|| !upgradeCredential.MatchDirection))
                    {
                        return CredentialRequestPathTypeName.Upgrade;
                    }
                }
            }

            return CredentialRequestPathTypeName.New;
        }

        private GetApplicationSearchRequest GetSearchRequest()
        {
            var naatiNumber = ApplicationModel.ApplicationInfo.NaatiNumber;
            var filter = new ApplicationSearchCriteria { Filter = ApplicationFilterType.NaatiNumberIntList, Values = new[] { naatiNumber.ToString() } };

            return new GetApplicationSearchRequest
            {
                Filters = new[] { filter }
            };
        }


        private IEnumerable<CredentialRequestModel> GetAllPersonCredentialRequests()
        {
            return ApplicationService.GetAllCredentialRequests(GetSearchRequest()).Data;
        }

        protected override void AttachWorkPractices()
        {
            if (ApplicationModel.ApplicationInfo.ApplicationStatusTypeId == (int)CredentialApplicationStatusTypeName.Draft)
            {
                return;
            }
            AttachWorkPractices(new[] { CredentialRequestModel });
        }
    }
}