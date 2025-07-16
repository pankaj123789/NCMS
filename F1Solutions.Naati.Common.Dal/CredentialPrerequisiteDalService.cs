using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace F1Solutions.Naati.Common.Dal
{
    public class CredentialPrerequisiteDalService : ICredentialPrerequisiteDalService
    {
        private readonly IApplicationQueryService _applicationQueryService;
        private readonly IPersonQueryService _personQueryService;
        private readonly ICredentialPrerequisiteQueryService _credentialPrerequisiteQueryService;
        private readonly IFileStorageService _fileSystemStorageService;

        public CredentialPrerequisiteDalService(IApplicationQueryService applicationQueryService, IPersonQueryService personQueryService, ICredentialPrerequisiteQueryService credentialPrerequisiteQueryService, IFileStorageService fileSystemStorageService)
        {
            _applicationQueryService = applicationQueryService;
            _personQueryService = personQueryService;
            _credentialPrerequisiteQueryService = credentialPrerequisiteQueryService;
            _fileSystemStorageService = fileSystemStorageService;
        }

        public IEnumerable<CredentialPrerequisite> GetCredentialPrerequisites()
        {
            var credentialPrerequisites = NHibernateSession.Current.Query<CredentialPrerequisite>().AsEnumerable();

            return credentialPrerequisites;
        }

        public PrerequisiteApplicationDalModel CreateApplicationAndCredentialRequest(CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel)
        {
            var preReqApplicationModel = CreateApplication(createPrerequisiteApplicationsDalModel);
            return preReqApplicationModel;
        }

        public UpsertApplicationResponse UpsertApplicationAndCredentialRequest(UpsertCredentialApplicationRequest upsertCredentialApplicationRequest)
        {
            var upsertedApplication = _applicationQueryService.UpsertApplication(upsertCredentialApplicationRequest);
            if(upsertedApplication.CredentialApplicationId == 0)
            {
                LoggingHelper.LogError($"APP{upsertedApplication.CredentialApplicationId} auto created failed.");
                return upsertedApplication;
            }
            LoggingHelper.LogInfo($"APP{upsertedApplication.CredentialApplicationId} has been automatically created.");
            return upsertedApplication;
        }

        public GenericResponse<PrerequisiteApplicationsResult> GetPrerequisiteApplications(int applicationId)
        {
            var results = NHibernateSession.Current.TransformSqlQueryDataRowResult<PrerequisiteApplicationsDto>($"exec PrerequisiteApplications {applicationId}").ToList();

            var filteredList = new List<PrerequisiteApplicationsDto>();
            //if more than one credential request is in the Application then duplicates can occur. Best to filter them out
            foreach (var dto in results)
            {
                if (filteredList.FirstOrDefault(x => x.RequiredPrerequisiteCredentialType == dto.RequiredPrerequisiteCredentialType) == null)
                {
                    filteredList.Add(dto);
                }
                //else is duplicate
            }

            var prerequisiteApplications = new List<PrerequisiteApplicationsDalModel>();

            foreach (var result in filteredList)
            {
                prerequisiteApplications = AddApplication(result, prerequisiteApplications);
            }

            return new PrerequisiteApplicationsResult() { PrerequisiteApplicationsModels = prerequisiteApplications };
        }

        public GenericResponse<PrerequisiteApplicationsNullableApplicationsResult> GetPrerequisiteApplicationsNullableApplications(int credentialRequestId)
        {
            var prerequistieApplicationsNullableApplicationsDtos = NHibernateSession.Current.TransformSqlQueryDataRowResult<PrerequisiteApplicationsNullableApplicationsDto>($"exec PrerequisiteApplicationsNullableApplications {credentialRequestId}").ToList();

            LoggingHelper.LogInfo($"Records returned from PrerequisiteApplicationsNullableApplications SP: {prerequistieApplicationsNullableApplicationsDtos.Count}");

            return new PrerequisiteApplicationsNullableApplicationsResult() { PrerequisiteApplicationsNullableApplicationsDtos = prerequistieApplicationsNullableApplicationsDtos };
        }

        public GenericResponse<PrerequisiteSummaryResult> GetPrerequisiteSummary(int credentialRequestId)
        {
            var prerequisiteSummaryDtos = NHibernateSession.Current.TransformSqlQueryDataRowResult<PrerequisiteSummaryDto>($"exec PrerequisiteSummary {credentialRequestId}").ToList();

            var prerequisiteSummaryModels = new List<PrerequisiteSummaryModel>();

            foreach (var prerequisiteSummaryDto in prerequisiteSummaryDtos)
            {
                // This is because the stored procedure returns PrerequisiteDirection as the root credentials direction
                // If skill match show lang 1 and lang 2
                var finalPrerequistieSkillDisplay = new StringBuilder("[Language 1]");
                if (prerequisiteSummaryDto.CredentialPrerequistieSkillMatch)
                {
                    // remove [Language 1]
                    finalPrerequistieSkillDisplay.Remove(0, 12);
                    finalPrerequistieSkillDisplay.Append(prerequisiteSummaryDto.PrerequistieDirection);
                    finalPrerequistieSkillDisplay.Replace("[Language 2]", prerequisiteSummaryDto.PrerequisiteCredentialLanguage2);
                }
                // if not then only show lang 1
                finalPrerequistieSkillDisplay.Replace("[Language 1]", prerequisiteSummaryDto.PrerequisiteCredentialLanguage1);

                if (prerequisiteSummaryDto.ExistingRequestCredentialTypeId.HasValue)
                {
                    var finalMatchingSkillDisplay = new StringBuilder(prerequisiteSummaryDto.MatchingCredentialDirection);
                    finalMatchingSkillDisplay.Replace("[Language 1]", prerequisiteSummaryDto.MatchingCredentialLanguage1);
                    finalMatchingSkillDisplay.Replace("[Language 2]", prerequisiteSummaryDto.MatchingCredentialLanguage2);

                    prerequisiteSummaryModels.Add(
                        new PrerequisiteSummaryModel()
                        {
                            PreRequisiteCredential = $"{prerequisiteSummaryDto.PrerequisiteCredentialName} ({finalPrerequistieSkillDisplay})",
                            MatchingCredential = $"{prerequisiteSummaryDto.MatchingCredentialName} ({finalMatchingSkillDisplay})",
                            Match = prerequisiteSummaryDto.Match,
                            StartDate = prerequisiteSummaryDto.MatchingCredentialStartDate,
                            EndDate = prerequisiteSummaryDto.MatchingCredentialEndDate,
                            CertificationPeriodId = prerequisiteSummaryDto.MatchingCredentialCertificationPeriodId
                        });
                    continue;
                }

                prerequisiteSummaryModels.Add(
                      new PrerequisiteSummaryModel()
                      {
                          PreRequisiteCredential = $"{prerequisiteSummaryDto.PrerequisiteCredentialName} ({finalPrerequistieSkillDisplay})",
                          MatchingCredential = $"-",
                          Match = prerequisiteSummaryDto.Match,
                          StartDate = prerequisiteSummaryDto.MatchingCredentialStartDate,
                          EndDate = prerequisiteSummaryDto.MatchingCredentialEndDate,
                          CertificationPeriodId = prerequisiteSummaryDto.MatchingCredentialCertificationPeriodId
                      });
            }

            var result =  new PrerequisiteSummaryResult() { PrerequisiteSummaryModels = prerequisiteSummaryModels};

            return new GenericResponse<PrerequisiteSummaryResult>(result);
        }

        public GenericResponse<CredentialRequestDetails> GetCredentialRequestDetails(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(credentialRequestId);

            var credentialRequestDetails = MapCredentialRequestToDetails(credentialRequest);

            return credentialRequestDetails;
        }

        public GenericResponse<List<CredentialPrerequisiteExemptionDto>> GetActiveCredentialPrerequisiteExemptions(CredentialRequestDetails credentialRequestDetails)
        {
            var exemptions = (from credentialPrerequisiteExemption in NHibernateSession.Current.Query<CredentialPrerequisiteExemption>()
                              where credentialPrerequisiteExemption.Person.Id == credentialRequestDetails.PersonId
                              && !credentialPrerequisiteExemption.EndDate.HasValue
                              select credentialPrerequisiteExemption).ToList();

            var exemptionDtos = new List<CredentialPrerequisiteExemptionDto>();

            foreach(var exemption in exemptions)
            {
                exemptionDtos.Add(MapCredentialPrerequisiteExemptionToDto(exemption));
            }

            return exemptionDtos;
        }

        public GenericResponse<List<CredentialPrerequisiteDetails>> GetPrerequisitesForCredentialRequest(CredentialRequestDetails credentialRequestDetails)
        {
            var allPrePrequisites = new List<CredentialPrerequisite>();

            var prerequisites = (from credentialPrerequisite in NHibernateSession.Current.Query<CredentialPrerequisite>()
                                 where credentialPrerequisite.CredentialType.Id == credentialRequestDetails.CredentialTypeId
                                 && credentialPrerequisite.CredentialApplicationType.Id == credentialRequestDetails.ApplicationTypeId
                                 select credentialPrerequisite).ToList();

            //iterate down one level 

            foreach (var prerequisite in prerequisites)
            {
                var newPrerequisites = (from credentialPrerequisite in NHibernateSession.Current.Query<CredentialPrerequisite>()
                                     where credentialPrerequisite.CredentialType.Id == prerequisite.CredentialTypePrerequisite.Id
                                     && credentialPrerequisite.CredentialApplicationType.Id == prerequisite.ApplicationTypePrerequisite.Id
                                     select credentialPrerequisite).ToList();
                allPrePrequisites.Add(prerequisite);
                allPrePrequisites.AddRange(newPrerequisites);
            }

            var credentialPrerequisiteDetails = new List<CredentialPrerequisiteDetails>();

            foreach(var prerequisite in allPrePrequisites)
            {
                credentialPrerequisiteDetails.Add(MapCredentialPrerequisiteToDetails(prerequisite));
            }

            return credentialPrerequisiteDetails;
        }
        public GenericResponse<List<CredentialPrerequisiteExemptionDto>> GetAllCredentialPrerequisiteExemptions(int naatiNumber)
        {
            var p = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            if (p == null)
            {
                throw new ArgumentException($"Person with NAATI Number {naatiNumber} not found");
            }
            var exemptions = (from credentialPrerequisiteExemption in NHibernateSession.Current.Query<CredentialPrerequisiteExemption>()
                              where credentialPrerequisiteExemption.Person.Id == p.Id
                              select credentialPrerequisiteExemption).ToList();

            var exemptionDtos = new List<CredentialPrerequisiteExemptionDto>();

            foreach (var exemption in exemptions)
            {
                exemptionDtos.Add(MapCredentialPrerequisiteExemptionToDto(exemption));
            }

            return exemptionDtos;
        }

        public GenericResponse<bool> SaveOrUpdateCredentialPrerequisiteExemptions(CredentialPrerequisiteExemptionDto credentialPrerequisiteExemptionDto)
        {
            // get exemption from dto id
            var exemptionToSaveOrUpdate = NHibernateSession.Current.Get<CredentialPrerequisiteExemption>(credentialPrerequisiteExemptionDto.CredentialPrerequisiteExemptionId);

            // if exemption is null then we save a new record
            if (exemptionToSaveOrUpdate.IsNull())
            {
                var person = NHibernateSession.Current.Get<Person>(credentialPrerequisiteExemptionDto.PersonId);
                var credentialType = NHibernateSession.Current.Get<CredentialType>(credentialPrerequisiteExemptionDto.CredentialTypeId);
                var skill = NHibernateSession.Current.Get<Skill>(credentialPrerequisiteExemptionDto.SkillId);
                var user = NHibernateSession.Current.Get<User>(credentialPrerequisiteExemptionDto.ModifiedUserId);
                exemptionToSaveOrUpdate = new CredentialPrerequisiteExemption()
                {
                    Person = person,
                    CredentialType = credentialType,
                    Skill = skill,
                    StartDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifiedUser = user,
                };

                NHibernateSession.Current.SaveOrUpdate(exemptionToSaveOrUpdate);

                return new GenericResponse<bool>(true);
            }

            // otherwise we update the existing record
            exemptionToSaveOrUpdate.ModifiedDate = DateTime.Now;
            exemptionToSaveOrUpdate.ModifiedUser = NHibernateSession.Current.Get<User>(credentialPrerequisiteExemptionDto.ModifiedUserId);
            exemptionToSaveOrUpdate.EndDate = credentialPrerequisiteExemptionDto.EndDate;

            NHibernateSession.Current.SaveOrUpdate(exemptionToSaveOrUpdate);

            return new GenericResponse<bool>(true);
        }

        public GenericResponse<SkillDetails> GetSkillForExemption(int prerequisiteApplicationTypeId, int prerequisiteCredentialTypeId, int parentCredentialRequestId)
        {
            var prerequisiteSkillId = 0; //did not find a match
            var prerequisiteCredentialType = _applicationQueryService.GetCredentialTypeById(prerequisiteCredentialTypeId);
            var parentCredentialRequest = NHibernateSession.Current.Get<CredentialRequest>(parentCredentialRequestId);
            var prerequisiteSkill = new Skill();

            if (prerequisiteCredentialType.InternalName == "Ethics")
            {
                prerequisiteSkillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType(null, "Ethics", "Ethics", prerequisiteApplicationTypeId, prerequisiteCredentialTypeId);
                prerequisiteSkill = NHibernateSession.Current.Get<Skill>(prerequisiteSkillId);

                return new SkillDetails()
                {
                    SkillId = prerequisiteSkillId,
                    SkillName = prerequisiteSkill.DisplayName
                };
            }

            if (prerequisiteCredentialType.InternalName == "Indigenous Ethics")
            {
                prerequisiteSkillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType(null, "Ethics", "Ethics", prerequisiteApplicationTypeId, prerequisiteCredentialTypeId);
                prerequisiteSkill = NHibernateSession.Current.Get<Skill>(prerequisiteSkillId);

                return new SkillDetails()
                {
                    SkillId = prerequisiteSkillId,
                    SkillName = prerequisiteSkill.DisplayName
                };
            }

            if (prerequisiteCredentialType.Certification.HasValue && !prerequisiteCredentialType.Certification.Value)
            {
                //if Cred is not a certification
                var nonEnglishLanguage = GetNonEnglishLanguage(parentCredentialRequest.Skill.Language1.Name, parentCredentialRequest.Skill.Language2.Name, parentCredentialRequest.Skill.Id);
                prerequisiteSkillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType((int)DirectionTypeName.L1,
                    nonEnglishLanguage, nonEnglishLanguage, prerequisiteApplicationTypeId, prerequisiteCredentialTypeId);

                prerequisiteSkill = NHibernateSession.Current.Get<Skill>(prerequisiteSkillId);

                return new SkillDetails()
                {
                    SkillId = prerequisiteSkillId,
                    SkillName = prerequisiteSkill.DisplayName
                };
            }

            prerequisiteSkillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType(parentCredentialRequest.Skill.DirectionType.Id,
                parentCredentialRequest.Skill.Language1.Name, parentCredentialRequest.Skill.Language2.Name, prerequisiteApplicationTypeId, prerequisiteCredentialTypeId);

            prerequisiteSkill = NHibernateSession.Current.Get<Skill>(prerequisiteSkillId);

            return new SkillDetails()
            {
                SkillId = prerequisiteSkillId,
                SkillName = prerequisiteSkill.DisplayName
            };
        }

        public GenericResponse<OnHoldCredentialChildParameters> GetChildParamsForRelatedCredentialsOnHold(int childCredentialRequestId)
        {
            var childParamsQuery = from childCredentialRequest in NHibernateSession.Current.Query<CredentialRequest>()
                              join childCredentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on childCredentialRequest.CredentialApplication.Id equals childCredentialApplication.Id
                              join skillOfChildCredRequest in NHibernateSession.Current.Query<Skill>() on childCredentialRequest.Skill.Id equals skillOfChildCredRequest.Id
                              where childCredentialRequest.Id == childCredentialRequestId
                              select new OnHoldCredentialChildParameters
                              {
                                  CredentialRequestId = childCredentialRequest.Id,
                                  CredentialTypeId = childCredentialRequest.CredentialType.Id,
                                  CredentialTypeName = childCredentialRequest.CredentialType.InternalName,
                                  PersonId = childCredentialApplication.Person.Id,
                                  SkillId = skillOfChildCredRequest.Id,
                                  SkillDisplayName = NHibernateSession.Current.Get<Skill>(skillOfChildCredRequest.Id).DisplayName,
                                  Language1Id = skillOfChildCredRequest.Language1.Id,
                                  Language2Id = skillOfChildCredRequest.Language2.Id,
                                  CredentialApplicationId = childCredentialApplication.Id,
                                  CredentialApplicationStatusTypeId = childCredentialApplication.CredentialApplicationStatusType.Id,
                                  CredentialApplicationStatusTypeDisplayName = childCredentialApplication.CredentialApplicationStatusType.DisplayName,
                                  CredentialApplicationTypeDisplayName = childCredentialApplication.CredentialApplicationType.DisplayName
                              };

            var childParams = childParamsQuery.FirstOrDefault();

            if (childParams.IsNull())
            {
                return new GenericResponse<OnHoldCredentialChildParameters>(null)
                {
                    Errors = new List<string>() { $"Failed to get the child parameters for the child credential request with id {childCredentialRequestId}." }
                };
            }

            return childParams;
        }

        public GenericResponse<List<OnHoldCredentialParentParameters>> GetParentParamsForRelatedCredentialsOnHold(OnHoldCredentialChildParameters childParams)
        {
            var parentParamsQuery = from onHoldCredentialPrereq in NHibernateSession.Current.Query<CredentialPrerequisite>()
                                    join onHoldPrereqCredentialType in NHibernateSession.Current.Query<CredentialType>() on onHoldCredentialPrereq.CredentialType.Id equals onHoldPrereqCredentialType.Id
                                    join onHoldCredRequest in NHibernateSession.Current.Query<CredentialRequest>() on onHoldPrereqCredentialType.Id equals onHoldCredRequest.CredentialType.Id
                                    join onHoldCredApplication in NHibernateSession.Current.Query<CredentialApplication>() on onHoldCredRequest.CredentialApplication.Id equals onHoldCredApplication.Id
                                    join skillOfOnHoldCredRequest in NHibernateSession.Current.Query<Skill>() on onHoldCredRequest.Skill.Id equals skillOfOnHoldCredRequest.Id
                                    join onHoldCredentialType in NHibernateSession.Current.Query<CredentialType>() on onHoldCredRequest.CredentialType.Id equals onHoldCredentialType.Id
                                    where onHoldCredentialPrereq.CredentialTypePrerequisite.Id == childParams.CredentialTypeId
                                    && onHoldCredApplication.Person.Id == childParams.PersonId
                                    && onHoldCredApplication.CredentialApplicationType.Id == onHoldCredentialPrereq.CredentialApplicationType.Id
                                    // If ethics then don't check any skill, if no skillmatch then language 1 will still match, otherwise both skills need to match
                                    && ((onHoldCredentialPrereq.CredentialTypePrerequisite.Id == 16 || (onHoldCredentialPrereq.SkillMatch == false && childParams.Language1Id == skillOfOnHoldCredRequest.Language1.Id)  
                                        || skillOfOnHoldCredRequest.Language1.Id == childParams.Language1Id && skillOfOnHoldCredRequest.Language2.Id == childParams.Language2Id))
                                    && onHoldCredRequest.CredentialRequestStatusType.Id == 39 // on hold
                                    select new OnHoldCredentialParentParameters
                                    {
                                        CredentialRequestId = onHoldCredRequest.Id,
                                        CredentialTypeId = onHoldCredentialType.Id,
                                        CredentialTypeName = onHoldCredentialType.InternalName,
                                        CredentialRequestStatusTypeId = onHoldCredRequest.CredentialRequestStatusType.Id,
                                        CredentialRequestStatusTypeDisplayName = onHoldCredRequest.CredentialRequestStatusType.DisplayName,
                                        CredentialApplicationTypeId = onHoldCredApplication.CredentialApplicationType.Id,
                                        SkillDisplayName = NHibernateSession.Current.Get<Skill>(skillOfOnHoldCredRequest.Id).DisplayName,
                                        ChildParameters = childParams,
                                        CredentialApplicationId = onHoldCredApplication.Id,
                                        CredentialApplicationStatusTypeId = onHoldCredApplication.CredentialApplicationStatusType.Id,
                                        CredentialApplicationStatusTypeDisplayName = onHoldCredApplication.CredentialApplicationStatusType.DisplayName,
                                        CredentialApplicationTypeDisplayName = onHoldCredApplication.CredentialApplicationType.DisplayName
                                    };

            var parentParams = parentParamsQuery.ToList();

            if (parentParams.IsNull())
            {
                return new GenericResponse<List<OnHoldCredentialParentParameters>>(null);
            }

            return parentParams;
        }

        public GenericResponse<bool> CheckParentCredentialsOnHoldElligibleToIssue(OnHoldCredentialParentParameters parentParams)
        {
            var prerequisitesMet = false;

            // required prerequisites for the parent on hold credential
            var prerequisitesOfParentQuery = from prereqOfOnHoldCredential in NHibernateSession.Current.Query<CredentialPrerequisite>()
                                         where prereqOfOnHoldCredential.CredentialType.Id == parentParams.CredentialTypeId
                                         && prereqOfOnHoldCredential.CredentialApplicationType.Id == parentParams.CredentialApplicationTypeId
                                         select prereqOfOnHoldCredential;

            var prerequisitesOfParent = prerequisitesOfParentQuery.ToList();

            // the persons satisfied prerequisites
            var satisfiedPrerequisitesOfParentQuery = from prereqOfOnHoldCredential in NHibernateSession.Current.Query<CredentialPrerequisite>()
                                                      join credRequestOfPrereq in NHibernateSession.Current.Query<CredentialRequest>() on prereqOfOnHoldCredential.CredentialTypePrerequisite.Id equals credRequestOfPrereq.CredentialType.Id
                                                      join skillOfCredRequest in NHibernateSession.Current.Query<Skill>() on credRequestOfPrereq.Skill.Id equals skillOfCredRequest.Id
                                                      join credApplicationOfPrereq in NHibernateSession.Current.Query<CredentialApplication>() on credRequestOfPrereq.CredentialApplication.Id equals credApplicationOfPrereq.Id
                                                      where credApplicationOfPrereq.Person.Id == parentParams.ChildParameters.PersonId
                                                      && prereqOfOnHoldCredential.CredentialType.Id == parentParams.CredentialTypeId
                                                      && (
                                                             credRequestOfPrereq.CredentialRequestStatusType.Id == 12
                                                             || ((credRequestOfPrereq.CredentialRequestStatusType.Id == 22 || credRequestOfPrereq.CredentialRequestStatusType.Id == 39) && credRequestOfPrereq.Id == parentParams.ChildParameters.CredentialRequestId)
                                                         )
                                                      && prereqOfOnHoldCredential.ApplicationTypePrerequisite.Id == credApplicationOfPrereq.CredentialApplicationType.Id
                                                      && prereqOfOnHoldCredential.CredentialApplicationType.Id == parentParams.CredentialApplicationTypeId
                                                      && (
                                                             prereqOfOnHoldCredential.SkillMatch == false
                                                             || (parentParams.ChildParameters.Language1Id == skillOfCredRequest.Language1.Id && parentParams.ChildParameters.Language2Id == skillOfCredRequest.Language2.Id)
                                                         )
                                                      select prereqOfOnHoldCredential;

            var satisfiedPrerequisitesOfParent = satisfiedPrerequisitesOfParentQuery.ToList();

            if (prerequisitesOfParent.Count == satisfiedPrerequisitesOfParent.Count && satisfiedPrerequisitesOfParent.OrderBy(x => x.Id).SequenceEqual(prerequisitesOfParent.OrderBy(x => x.Id)))
            {
                prerequisitesMet = true;
                return prerequisitesMet;
            }

            // the persons exemptions matched to a prerequisite record
            var personExemptionsOfParentQuery = from exemption in NHibernateSession.Current.Query<CredentialPrerequisiteExemption>()
                                                join prerequisite in NHibernateSession.Current.Query<CredentialPrerequisite>() on exemption.CredentialType equals prerequisite.CredentialTypePrerequisite
                                                where exemption.Person.Id == parentParams.ChildParameters.PersonId
                                                && !exemption.EndDate.HasValue
                                                && prerequisite.CredentialApplicationType.Id == parentParams.CredentialApplicationTypeId
                                                && (
                                                            prerequisite.SkillMatch == false
                                                            || (parentParams.ChildParameters.Language1Id == exemption.Skill.Language1.Id && parentParams.ChildParameters.Language2Id == exemption.Skill.Language2.Id)
                                                    )
                                                && prerequisite.CredentialType.Id == parentParams.CredentialTypeId
                                                select prerequisite;

            var personExemptionsOfParent = personExemptionsOfParentQuery.ToList();

            // add exemption to list of satisfied prerequisites
            satisfiedPrerequisitesOfParent.AddRange(personExemptionsOfParent);
            // distinct the list of the users prerequisites
            satisfiedPrerequisitesOfParent = satisfiedPrerequisitesOfParent.Distinct().ToList();
            // compare with required prerequisites

            if(prerequisitesOfParent.Count == satisfiedPrerequisitesOfParent.Count && satisfiedPrerequisitesOfParent.OrderBy(x => x.Id).SequenceEqual(prerequisitesOfParent.OrderBy(x => x.Id)))
            {
                prerequisitesMet = true;
                return prerequisitesMet;
            }

            return prerequisitesMet;
        }

        private CredentialPrerequisiteDetails MapCredentialPrerequisiteToDetails(CredentialPrerequisite credentialPrerequisite)
        {
            return new CredentialPrerequisiteDetails()
            {
                CredentialPrerequisiteId = credentialPrerequisite.Id,
                CredentialPrerequsiteName = credentialPrerequisite.CredentialTypePrerequisite.InternalName,
                CredentialTypePrerequisiteId = credentialPrerequisite.CredentialTypePrerequisite.Id,
                ApplicationTypePrerequisiteId = credentialPrerequisite.ApplicationTypePrerequisite.Id,
            };
        }

        private CredentialPrerequisiteExemptionDto MapCredentialPrerequisiteExemptionToDto(CredentialPrerequisiteExemption exemption)
        {
            return new CredentialPrerequisiteExemptionDto()
            {
                CredentialPrerequisiteExemptionId = exemption.Id,
                CredentialTypeName = exemption.CredentialType.InternalName,
                CredentialTypeId = exemption.CredentialType.Id,
                SkillName = exemption.Skill.DisplayName,
                SkillId = exemption.Skill.Id,
                PersonId = exemption.Person.Id,
                StartDate = exemption.StartDate.Date,
                EndDate = exemption.EndDate,
                ModifiedDate = exemption.ModifiedDate,
                ModifiedUser = exemption.ModifiedUser.UserName,
                ModifiedUserId = exemption.ModifiedUser.Id
            };
        }

        private CredentialRequestDetails MapCredentialRequestToDetails(CredentialRequest credentialRequest)
        {
            return new CredentialRequestDetails()
            {
                PersonId = credentialRequest.CredentialApplication.Person.Id,
                CredentialTypeId = credentialRequest.CredentialType.Id,
                SkillId = credentialRequest.Skill.Id,
                ApplicationTypeId = credentialRequest.CredentialApplication.CredentialApplicationType.Id
            };
        }

        private PrerequisiteApplicationDalModel CreateApplication(CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel)
        {
            var preReqRequest = createPrerequisiteApplicationsDalModel.ChildCredentialRequestType;

            var preReqApplicationModel = new PrerequisiteApplicationDalModel();

            var personResponse = _personQueryService.GetPersonDetailsBasic(new GetPersonDetailsRequest() { PersonId = createPrerequisiteApplicationsDalModel.ApplicationPersonId });

            var credentialRequests = CreateCredentialRequest(createPrerequisiteApplicationsDalModel);

            //if CreRequests not raised then raise applicaiton as Draft (191381)
            var raiseAsDraft = preReqRequest.HasValidationError || credentialRequests.Count() == 0;

            var application = new UpsertCredentialApplicationRequest()
            {
                CredentialRequests = raiseAsDraft ? new List<CredentialRequestData>():credentialRequests,
                EnteredDate = DateTime.Today.Date,
                StatusChangeDate = DateTime.Now,
                OwnedByApplicant = false,
                EnteredUserId = createPrerequisiteApplicationsDalModel.EnteredUserId,
                NaatiNumber = personResponse.PersonDetails.NaatiNumber,
                ApplicationStatusTypeId = raiseAsDraft ? (int)CredentialApplicationStatusTypeName.Draft : (int)CredentialApplicationStatusTypeName.InProgress,
                ApplicationTypeId = preReqRequest.ApplicationTypeId,
                Notes = new List<ApplicationNoteData>(),
                ReceivingOfficeId = 1, //check that this is OK
                StatusChangeUserId = createPrerequisiteApplicationsDalModel.EnteredUserId,
                PreferredTestLocationId = createPrerequisiteApplicationsDalModel.PreferredTestLocationId,
                AutoCreated = true,
            };

            preReqApplicationModel.UpsertCredentialApplicationRequest = AddMandatoryFields(application, createPrerequisiteApplicationsDalModel);
            preReqApplicationModel.Attachments = AddMandatoryDocuments(application, createPrerequisiteApplicationsDalModel);

            return preReqApplicationModel;
        }

        private IEnumerable<CredentialApplicationAttachmentDalModel> AddMandatoryDocuments(UpsertCredentialApplicationRequest application, CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel)
        {
            var attachments = new List<CredentialApplicationAttachmentDalModel>();

            var result = _credentialPrerequisiteQueryService.GetExistingDocumentTypesForCredentialRequest(createPrerequisiteApplicationsDalModel.CredentialRequestId);
            if (!result.Success)
            {
                throw new Exception($"Can't get Application Documents for Credential Request {createPrerequisiteApplicationsDalModel.CredentialRequestId}");
            }

            var mandatoryDocsResult = _credentialPrerequisiteQueryService.GetMandatoryDocumentsForApplicationType(application.ApplicationTypeId);
            if (!mandatoryDocsResult.Success)
            {
                throw new Exception($"failed to get mandatory Documents from credentialApplicationTypeId {application.ApplicationTypeId}");
            }
            var mandatoryDocs = mandatoryDocsResult.Data;

            var parentAttachmentsResponse = _applicationQueryService.GetAttachments(new GetApplicationAttachmentsRequest()
            {
                ApplicationId = createPrerequisiteApplicationsDalModel.ApplicationId
            });

            var parentAttachments = parentAttachmentsResponse.Attachments;

            foreach (var parentAttachment in parentAttachments)
            {
                if (mandatoryDocs.Contains(parentAttachment.DocumentType))
                {

                    var attachment = CloneExistingAttachment(parentAttachment);

                    attachments.Add(attachment);

 
                }
            }

            return attachments;
        }

        private CredentialApplicationAttachmentDalModel CloneExistingAttachment(CredentialApplicationAttachmentDto parentAttachment)
        {
            var type = GetTypeForDocument(parentAttachment.DocumentType);

            //copy to temp storage. Maintly to get a reliable file path
            var fileResponse = _fileSystemStorageService.GetFile(new GetFileRequest()
            { 
                StoredFileId = parentAttachment.StoredFileId, 
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] 
            });
        

            //var createAttachmentRequest = new CreateOrReplaceApplicationAttachmentRequest()
            //{
            //    DocumentType = parentAttachment.DocumentType,
            //    FileName = parentAttachment.FileName,
            //    FilePath = fileResponse.FilePaths.First(),
            //    FileSize = parentAttachment.FileSize,
            //    FileType = parentAttachment.FileType,
            //    StoragePath = parentAttachment.StoragePath,
            //    UploadedByUserId = parentAttachment.UploadedByUserId,

            //};
            //var createResponse = _applicationQueryService.CreateOrReplaceAttachment(createAttachmentRequest);

            var attachment = new CredentialApplicationAttachmentDalModel()
            {
                CredentialApplicationId = 0, //unknown at this stage. Added in UpsertApplication
                DocumentType = parentAttachment.DocumentType,
                FileName = parentAttachment.FileName,
                StoragePath = $"{parentAttachment.DocumentType}\\<applicationid>\\{parentAttachment.FileName}", //sems to be file destination path i.e. ..FileStorage/<documenttype>
                FileSize = parentAttachment.FileSize,
                FilePath = fileResponse.FilePaths.First(),
                UploadedByName = parentAttachment.UploadedByName,
                UploadedByUserId = parentAttachment.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                Type = (StoredFileType)type
            };

            return attachment;
        }

        private UpsertCredentialApplicationRequest AddMandatoryFields(UpsertCredentialApplicationRequest application, CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel)
        {
            //get list of fields that have data in current credential request
            var result = _credentialPrerequisiteQueryService.GetExistingApplicationFieldsForCredentialRequest(createPrerequisiteApplicationsDalModel.CredentialRequestId);
            if (!result.Success)
            {
                throw new Exception($"Can't get ApplicationFields for Credential Request {createPrerequisiteApplicationsDalModel.CredentialRequestId}");
            }

            var mandatoryFieldsResult = _credentialPrerequisiteQueryService.GetMandatoryFieldsForApplicationType(application.ApplicationTypeId);
            if (!mandatoryFieldsResult.Success)
            {
                throw new Exception($"failed to get mandatory fields from credentialApplicationTypeId {application.ApplicationTypeId}");
            }

            var mandatoryFields = mandatoryFieldsResult.Data;
            var applicationFields = new List<ApplicationFieldData>();

            foreach (var mandatoryField in mandatoryFields)
            {
                var foundField = createPrerequisiteApplicationsDalModel.ApplicationFields.FirstOrDefault(x => x.Name == mandatoryField);
                if (foundField != null)
                {
                    applicationFields.Add(new ApplicationFieldData()
                    {
                        FieldDataId = foundField.FieldDataId,
                        FieldOptionId = foundField.FieldOptionId,
                        FieldTypeId = foundField.FieldTypeId,
                        Id = foundField.Id,
                        Value = foundField.Value
                    });
                }
            }

            if (applicationFields.Count > 0)
            {
                application.Fields = applicationFields;
            }

            return application;
        }

        private int GetTypeForDocument(string documentType)
        {
            var type = (StoredFileType)Enum.Parse(typeof(StoredFileType), documentType);

            if (Enum.IsDefined(typeof(StoredFileType), type))
            {
                return (int)type;
            }

            throw new Exception($"No StoredFileType exists for {documentType}");
        }

        private IEnumerable<CredentialRequestData> CreateCredentialRequest(CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel)
        {
            var credentialTypeId = _credentialPrerequisiteQueryService.GetCredentialRequestTypeFromName(createPrerequisiteApplicationsDalModel.ChildCredentialRequestType.CredentialRequestType);

            var skillId = GetSkillForNewCredentialRequest(credentialTypeId, createPrerequisiteApplicationsDalModel);
            if (skillId == 0)
            {
                //dont raise credential request
                return new List<CredentialRequestData>();
            }

            var credentialRequestData = new CredentialRequestData()
            {
                CredentialTypeId = credentialTypeId,
                SkillId = skillId,
                StatusChangeDate = DateTime.Now,
                StatusChangeUserId = createPrerequisiteApplicationsDalModel.EnteredUserId,
                StatusTypeId = (int)CredentialRequestStatusTypeName.EligibleForTesting,
                CredentialRequestPathTypeId = 1, //New
                AutoCreated = true,
            };

            return new List<CredentialRequestData>() { credentialRequestData };
        }

        private int GetSkillForNewCredentialRequest(int credentialTypeId, CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel)
        {
            var preReqRequest = createPrerequisiteApplicationsDalModel.ChildCredentialRequestType;
            var skillId = 0; //did not find a match
            var credentialType = _applicationQueryService.GetCredentialTypeById(credentialTypeId);
            //var skillTypeId = createPrerequisiteApplicationsDalModel.CredentialRequestSkillTypeId;

            if (credentialType.InternalName == "Ethics")
            {
                skillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType(null, "Ethics", "Ethics", preReqRequest.ApplicationTypeId, credentialTypeId);
                return skillId;
            }

            if (credentialType.Certification.HasValue && !credentialType.Certification.Value)
            {
                //if Cred is not a certification
                var nonEnglishLanguage = GetNonEnglishLanguage(createPrerequisiteApplicationsDalModel.CredentialRequestSkillLanguage1, createPrerequisiteApplicationsDalModel.CredentialRequestSkillLanguage2, createPrerequisiteApplicationsDalModel.CredentialRequestSkillId); 
                skillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType((int)DirectionTypeName.L1,
                    nonEnglishLanguage, nonEnglishLanguage, preReqRequest.ApplicationTypeId, credentialTypeId);

                return skillId;
            }

            skillId = _applicationQueryService.GetSkillFromLanguagesAndCredentialApplicationType(createPrerequisiteApplicationsDalModel.CredentialRequestSkillDirectionId,
                createPrerequisiteApplicationsDalModel.CredentialRequestSkillLanguage1, createPrerequisiteApplicationsDalModel.CredentialRequestSkillLanguage2, preReqRequest.ApplicationTypeId, credentialTypeId);

            return skillId;
        }

        private string GetNonEnglishLanguage(string l1, string l2, int credentialRequestSkillId)
        {
            var language = l1;
            if (language != "English")
            {
                return language;
            }
            language = l2;
            if (language != "English")
            {
                return language;
            }
            throw new Exception($"Could not find a non english Langugage for Intercultural Credential Typer. {credentialRequestSkillId}");
        }

        private List<PrerequisiteApplicationsDalModel> AddApplication(PrerequisiteApplicationsDto result, List<PrerequisiteApplicationsDalModel> prerequisiteApplications)
        {
            // format the current credential request skill
            var currentSkillDisplayName = new StringBuilder(result.CurrentCredentialRequestSkillDirection);
            currentSkillDisplayName.Replace("[Language 1]", result.CurrentCredentialRequestLanguage1);
            currentSkillDisplayName.Replace("[Language 2]", result.CurrentCredentialRequestLanguage2);

            // format the existing credential request skill
            var existingSkillDisplayName = new StringBuilder(result.ExistingCredentialRequestSkillDirection);
            existingSkillDisplayName.Replace("[Language 1]", result.ExistingCredentialRequestLanguage1);
            existingSkillDisplayName.Replace("[Language 2]", result.ExistingCredentialRequestLanguage2);

            var requiredPrerequisiteSkillDisplayName = new StringBuilder("[Language 1]");

            if (result.RequiredPrerequisiteSkillMatch)
            {
                // if skill match remove [Language 1] and add the current cred request direction
                requiredPrerequisiteSkillDisplayName.Remove(0, 12);
                requiredPrerequisiteSkillDisplayName.Append(result.CurrentCredentialRequestSkillDirection);
                requiredPrerequisiteSkillDisplayName.Replace("[Language 2]", result.RequiredPrerequisiteLanguage2);
            }

            requiredPrerequisiteSkillDisplayName.Replace("[Language 1]", result.RequiredPrerequisiteLanguage1);

            prerequisiteApplications.Add(
                new PrerequisiteApplicationsDalModel
                {
                    CurrentCredentialRequest = $"{result.CurrentCredentialRequestCredentialTypeName} ({currentSkillDisplayName})",
                    RequiredPrerequisite = $"{result.RequiredPrerequisiteCredentialType} ({requiredPrerequisiteSkillDisplayName})",
                    ExistingApplicationId = result.ExistingApplicationId,
                    ExistingApplication = $"APP{result.ExistingApplicationId}",
                    ExistingApplicationStatusTypeId = result.ExistingApplicationStatusTypeId,
                    ExistingApplicationStatus = result.ExistingApplicationStatusName,
                    ExistingApplicationAutoCreated = result.ExistingApplicationAutoCreated,
                    ExistingCredentialRequest = $"{result.ExistingCredentialRequestCredentialTypeName} ({existingSkillDisplayName})",
                    ExistingCredentialRequestStatusTypeId = result.ExistingCredentialRequestStatusTypeId,
                    ExistingCredentialRequestStatus = result.ExsitingCredentialRequestStatusName,
                    ExistingCredentialRequestAutoCreated = result.ExistingCredentialRequestAutoCreated,
                    CreatedDate = result.ExistingApplicationCreatedDate,
                    ExistingApplicationType = result.ExistingApplicationTypeName
                }
            );
            return prerequisiteApplications;
        }
        private string GetMatchingCredentialLanguageString(string lang1, string lang2)
        {
            // both languages are null return empty string
            if (lang1 == null && lang2 == null)
            {
                return "";
            }

            // if lang1 and lang 2 are the same return just lang1
            if (lang1 == lang2)
            {
                return $"({lang1})";
            }

            // if lang1 and lang2 are not the same and one of them is not null return lang1 and lang2
            if (lang1 != lang2 && (lang1 != null || lang2 != null))
            {
                return $"({lang1} and {lang2})";
            }


            // if none of the above cases return empty string
            return "";

        }
    }
}