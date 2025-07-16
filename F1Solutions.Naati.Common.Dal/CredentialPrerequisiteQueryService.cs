using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Ncms.Contracts.Models.CredentialPrerequisite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal
{
    public class CredentialPrerequisiteQueryService : ICredentialPrerequisiteQueryService
    {
        public IEnumerable<CredentialPrerequisite> GetCredentialPrerequisites()
        {
            var credentialPrerequisites = NHibernateSession.Current.Query<CredentialPrerequisite>().AsEnumerable();

            return credentialPrerequisites;
        }

        public PrerequisiteSummaryResult GetPrerequisiteSummary(int credentialRequestId)
        {
            //get the details of the credential request
            var credentialRequestResult = from credentialRequest in NHibernateSession.Current.Query<CredentialRequest>()
                                          where credentialRequest.Id == credentialRequestId
                                          select credentialRequest;
            var credentialRequestSingle = credentialRequestResult.First();

            //get the prequisities and their match criteria
            var prerequisiteList = GetPreRequisiteList(new List<PrerequisiteIdAndMatch>() { new PrerequisiteIdAndMatch() { Id = credentialRequestSingle.CredentialType.Id, Match = null } }, credentialRequestSingle);
            //but remove the first one
            prerequisiteList = prerequisiteList.Where(x => x.Match.HasValue).Select(x => x).ToList();

            //now get any prerequisites
            var prerequsiteSummaryDtos = GetMatchingCredentials(credentialRequestSingle, prerequisiteList);

            //combine them into PrerequisiteSummaryModel
            var prerequisiteSummaryModels = ConstructModel(prerequsiteSummaryDtos);

            // get result of whether or not the prerequisite credentials have been satisfied or not
            var result = GetPrerequisiteResult(prerequisiteSummaryModels);

            return result;
        }

        private List<PrerequisiteSummaryModel> ConstructModel(List<PrerequisiteSummaryDto> prerequisiteSummaryDtos)
        {
            var prerequisiteSummaryModels = new List<PrerequisiteSummaryModel>();

            foreach (var prerequisiteSummaryDto in prerequisiteSummaryDtos)
            {
                // If SkillMatch then show skills in credentials
                if (prerequisiteSummaryDto.CredentialPrerequistieSkillMatch)
                {
                    if (prerequisiteSummaryDto.ExistingRequestCredentialTypeId.HasValue)
                    {
                        prerequisiteSummaryModels.Add(
                          new PrerequisiteSummaryModel()
                          {
                              PreRequisiteCredential = $"{NHibernateSession.Current.Get<CredentialType>(prerequisiteSummaryDto.PreRequisiteCredentialId).InternalName} ({prerequisiteSummaryDto.PrerequisiteCredentialLanguage1} and {prerequisiteSummaryDto.PrerequisiteCredentialLanguage2})",
                              MatchingCredential = $"{NHibernateSession.Current.Get<CredentialType>(prerequisiteSummaryDto.ExistingRequestCredentialTypeId).InternalName} {GetMatchingCredentialLanguageString(prerequisiteSummaryDto.MatchingCredentialLanguage1, prerequisiteSummaryDto.MatchingCredentialLanguage2)}",
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
                              PreRequisiteCredential = $"{NHibernateSession.Current.Get<CredentialType>(prerequisiteSummaryDto.PreRequisiteCredentialId).InternalName} ({prerequisiteSummaryDto.PrerequisiteCredentialLanguage1} and {prerequisiteSummaryDto.PrerequisiteCredentialLanguage2})",
                              MatchingCredential = $"-",
                              Match = prerequisiteSummaryDto.Match,
                              StartDate = prerequisiteSummaryDto.MatchingCredentialStartDate,
                              EndDate = prerequisiteSummaryDto.MatchingCredentialEndDate,
                              CertificationPeriodId = prerequisiteSummaryDto.MatchingCredentialCertificationPeriodId
                          });
                }

                // No skillmatch so dont show skills in credentials
                if (prerequisiteSummaryDto.ExistingRequestCredentialTypeId.HasValue)
                {
                    prerequisiteSummaryModels.Add(
                      new PrerequisiteSummaryModel()
                      {
                          PreRequisiteCredential = $"{NHibernateSession.Current.Get<CredentialType>(prerequisiteSummaryDto.PreRequisiteCredentialId).InternalName}",
                          MatchingCredential = $"{NHibernateSession.Current.Get<CredentialType>(prerequisiteSummaryDto.ExistingRequestCredentialTypeId).InternalName} {GetMatchingCredentialLanguageString(prerequisiteSummaryDto.MatchingCredentialLanguage1, prerequisiteSummaryDto.MatchingCredentialLanguage2)}",
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
                          PreRequisiteCredential = $"{NHibernateSession.Current.Get<CredentialType>(prerequisiteSummaryDto.PreRequisiteCredentialId).InternalName}",
                          MatchingCredential = $"-",
                          Match = prerequisiteSummaryDto.Match,
                          StartDate = prerequisiteSummaryDto.MatchingCredentialStartDate,
                          EndDate = prerequisiteSummaryDto.MatchingCredentialEndDate,
                          CertificationPeriodId = prerequisiteSummaryDto.MatchingCredentialCertificationPeriodId
                      });
            }

            return prerequisiteSummaryModels;
        }

        private static List<PrerequisiteSummaryDto> GetMatchingCredentials(CredentialRequest credentialRequestSingle, List<PrerequisiteIdAndMatch> prerequisiteList)
        {
            var personId = credentialRequestSingle.CredentialApplication.Person.Id;

            var credentialTypesToSelect = prerequisiteList.Select(x => x.Id).ToList();

            // Get credential requests that have been issued to the person
            var credentialRequestResult = (from credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() join
                          credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id join
                          person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals person.Id join
                          credentialType in NHibernateSession.Current.Query<CredentialType>() on credentialRequest.CredentialType.Id equals credentialType.Id //join
                          where
                          person.Id == personId &&
                          credentialRequest.CredentialRequestStatusType.Id == 12 && // 12 = Certification Issued
                          credentialTypesToSelect.Contains(credentialRequest.CredentialType.Id)

                          select credentialRequest).ToList();

            var prerequisiteSummaryDtos = new List<PrerequisiteSummaryDto>();
            // Now find the certification records if any and search the CredentialPeriodId to get endDate. If no credentialPeriodId then EndDate retrieved from certification table
            var certificationPeriods = (from credential in NHibernateSession.Current.Query<Credential>() join
                                        certificationPeriod in NHibernateSession.Current.Query<CertificationPeriod>() on credential.CertificationPeriod.Id equals certificationPeriod.Id
                                        where certificationPeriod.Person.Id == personId
                                        select certificationPeriod).ToList();

            // Return list of prerequisite credentials against person credentials that are matched and/ or unmatched
            foreach (var prerequisite in prerequisiteList)
            {
                var currentCredentialRequests = credentialRequestResult.Where(x => x.CredentialType.Id == prerequisite.Id).ToList();

                var credentialPrerequisiteResult = from credentialPrereq in NHibernateSession.Current.Query<CredentialPrerequisite>()
                                                   where credentialPrereq.CredentialTypePrerequisite.Id == prerequisite.Id
                                                   select credentialPrereq;

                var credentialPrerequisiteSkillMatch = credentialPrerequisiteResult.First().SkillMatch;

                if (!currentCredentialRequests.Any())
                {
                    prerequisiteSummaryDtos.Add(
                        new PrerequisiteSummaryDto()
                        {
                            PersonId = 0,
                            PreRequisiteCredentialId = prerequisite.Id,
                            ExistingRequestCredentialTypeId = null,
                            PrerequisiteCredentialLanguage1 = credentialRequestSingle.Skill.Language1.Name,
                            PrerequisiteCredentialLanguage2 = credentialRequestSingle.Skill.Language2.Name,
                            MatchingCredentialLanguage1 = null,
                            MatchingCredentialLanguage2 = null,
                            Match = false,
                            MatchingCredentialStartDate = null,
                            MatchingCredentialEndDate = null,
                            MatchingCredentialCertificationPeriodId = null
                        });
                    continue;
                }

                foreach (var credentialRequest in currentCredentialRequests)
                {
                    var dates = GetDateTime(credentialRequest, certificationPeriods);

                    // if the expiry/ end date has passed then continue then return no matched credential
                    if (dates.Item2.IsNotNull() && dates.Item2 < DateTime.Now)
                    {
                        prerequisiteSummaryDtos.Add(
                        new PrerequisiteSummaryDto()
                        {
                            PersonId = credentialRequest.CredentialApplication.Person.Id,
                            PreRequisiteCredentialId = prerequisite.Id,
                            ExistingRequestCredentialTypeId = null,
                            PrerequisiteCredentialLanguage1 = credentialRequestSingle.Skill.Language1.Name,
                            PrerequisiteCredentialLanguage2 = credentialRequestSingle.Skill.Language2.Name,
                            MatchingCredentialLanguage1 = null,
                            MatchingCredentialLanguage2 = null,
                            Match = false,
                            MatchingCredentialStartDate = null,
                            MatchingCredentialEndDate = null,
                            MatchingCredentialCertificationPeriodId = null
                        });
                        continue;
                    }

                    var match = GetMatch(credentialPrerequisiteSkillMatch, prerequisite.Id, credentialRequestSingle, credentialRequest);
                    prerequisiteSummaryDtos.Add(
                    new PrerequisiteSummaryDto()
                    {
                        PersonId = credentialRequest.CredentialApplication.Person.Id,
                        PreRequisiteCredentialId = prerequisite.Id,
                        ExistingRequestCredentialTypeId = credentialRequest.CredentialType.Id,
                        PrerequisiteCredentialLanguage1 = credentialRequestSingle.Skill.Language1.Name,
                        PrerequisiteCredentialLanguage2 = credentialRequestSingle.Skill.Language2.Name,
                        MatchingCredentialLanguage1 = credentialRequest.Skill.Language1.Name,
                        MatchingCredentialLanguage2 = credentialRequest.Skill.Language2.Name,
                        Match = match,
                        CredentialPrerequistieSkillMatch = credentialPrerequisiteSkillMatch,
                        MatchingCredentialStartDate = dates.Item1,
                        MatchingCredentialEndDate = dates.Item2,
                        MatchingCredentialCertificationPeriodId = certificationPeriods.Count > 0 ? (int?)certificationPeriods.First().Id : null
                    });
                }
            }
            return prerequisiteSummaryDtos;
        }

        private static (DateTime?, DateTime?) GetDateTime(CredentialRequest credentialRequest, List<CertificationPeriod> certificationPeriods)
        {
            if (credentialRequest.Credentials.Any() && certificationPeriods.Any())
            {
                return (certificationPeriods.First().StartDate, certificationPeriods.First().EndDate);
            }

            if (credentialRequest.Credentials.Any() && !certificationPeriods.Any())
            {
                return (credentialRequest.Credentials.First().StartDate, credentialRequest.Credentials.First().ExpiryDate);
            }

            return (null, null);
        }

        private static bool GetMatch(bool skillMatch, int prerequisiteId, CredentialRequest credentialRequest, CredentialRequest credentialRequestResult)
        {
            var match = false;
            var credentialType = NHibernateSession.Current.Get<CredentialType>(prerequisiteId);

            // as per business requirements, Interpreters are required to have skills in the correct order.
            if (credentialType.InternalName.Contains("Interpreter"))
            {
                if (skillMatch)
                {
                    match = prerequisiteId == credentialRequestResult.CredentialType.Id &&
                            (credentialRequest.Skill.Language1.Name == credentialRequestResult.Skill.Language1.Name ||
                            credentialRequest.Skill.Language1.Name == credentialRequestResult.Skill.Language2.Name) &&
                            (credentialRequest.Skill.Language2.Name == credentialRequestResult.Skill.Language1.Name ||
                            credentialRequest.Skill.Language2.Name == credentialRequestResult.Skill.Language2.Name);

                    return match;
                }

                match = prerequisiteId == credentialRequestResult.CredentialType.Id;

                return match;
            }

            if (skillMatch)
            {
                match = prerequisiteId == credentialRequestResult.CredentialType.Id &&
                        credentialRequest.Skill.Language1.Name == credentialRequestResult.Skill.Language1.Name &&
                        credentialRequest.Skill.Language2.Name == credentialRequestResult.Skill.Language2.Name;

                return match;
            }

            match = prerequisiteId == credentialRequestResult.CredentialType.Id;

            return match;
        }

        private List<PrerequisiteIdAndMatch> GetPreRequisiteList(List<PrerequisiteIdAndMatch> prerequisitesAndMatches, CredentialRequest credentialRequest)
        {
            //trying to clone here so foreach doesnt bomb with modified error
            var copiedCredentialTypes = prerequisitesAndMatches.ToList();
            foreach (var credentialTypeIn in copiedCredentialTypes)
            {
                var prerequisitesAndMatchesResult = (from 
                                                     credRequest in NHibernateSession.Current.Query<CredentialRequest>() join
                                                     credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id join
                                                     credentialPrerequisite in NHibernateSession.Current.Query<CredentialPrerequisite>() on credRequest.CredentialType.Id equals credentialPrerequisite.CredentialType.Id join
                                                     skill in NHibernateSession.Current.Query<Skill>() on credRequest.Skill.Id equals skill.Id join
                                                     lang1 in NHibernateSession.Current.Query<Language>() on credRequest.Skill.Language1.Id equals lang1.Id join
                                                     lang2 in NHibernateSession.Current.Query<Language>() on credRequest.Skill.Language2.Id equals lang2.Id join
                                                     credentialType in NHibernateSession.Current.Query<CredentialType>() on credentialPrerequisite.CredentialTypePrerequisite.Id equals credentialType.Id join
                                                     credentialApplicationType in NHibernateSession.Current.Query<CredentialApplicationType>() on credentialApplication.CredentialApplicationType.Id equals credentialApplicationType.Id
                                                     where
                                                     credRequest.Id == credentialRequest.Id &&
                                                     credentialType.Id == credentialPrerequisite.CredentialTypePrerequisite.Id &&
                                                     credentialPrerequisite.CredentialApplicationType.Id == credentialApplicationType.Id
                                                     select 
                                                     new PrerequisiteIdAndMatch() { Id = credentialPrerequisite.CredentialTypePrerequisite.Id, Match = credentialPrerequisite.SkillMatch }).ToList();

                prerequisitesAndMatches.AddRange(prerequisitesAndMatchesResult);

            }
            return prerequisitesAndMatches;
        }

        private static PrerequisiteSummaryResult GetPrerequisiteResult(List<PrerequisiteSummaryModel> prerequisiteSummaryModels)
        {
            var match = false;

            var distinctCredentialTypeNames = prerequisiteSummaryModels
                .Select(x => x.PreRequisiteCredential).Distinct().ToList();

            foreach (var distinctCredentialTypeName in distinctCredentialTypeNames)
            {
                var currentPrerequisiteSummaryModels = prerequisiteSummaryModels.Where(x => x.PreRequisiteCredential == distinctCredentialTypeName).ToList();

                var matchList = currentPrerequisiteSummaryModels.Select(x => x.Match).ToList();

                if (matchList.Contains(true))
                {
                    match = true;
                    continue;
                }

                if (matchList.Contains(false))
                {
                    match = false;
                    continue;
                }
            }

            return new PrerequisiteSummaryResult() { PrerequisiteSummaryModels = prerequisiteSummaryModels};
        }

        public int GetCredentialRequestTypeFromName(string name)
        {
            var result = NHibernateSession.Current.Query<CredentialType>().FirstOrDefault(x => x.InternalName == name);
            return result.Id;
        }

        public GenericResponse<IEnumerable<string>> GetExistingApplicationFieldsForCredentialRequest(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Query<CredentialRequest>().FirstOrDefault(x => x.Id == credentialRequestId);
            var result = credentialRequest.CredentialApplication.CredentialApplicationFieldsData.Where(x => x.Value != null).Select(x => x.CredentialApplicationField.Name).ToList();
            return result;
        }

        public GenericResponse<Dictionary<int, List<string>>> GetCredentialApplicationTypeListWithMandatoryFields(int parentCredentialRequestId)
        {
            var responseData = new Dictionary<int, List<string>>();

            var credentialRequest = NHibernateSession.Current.Query<CredentialRequest>().FirstOrDefault(x => x.Id == parentCredentialRequestId);
            var preRequisites = NHibernateSession.Current.Query<CredentialPrerequisite>().Where(x => x.CredentialTypePrerequisite == credentialRequest.CredentialType).Select(x=>x);

            foreach(var preRequisite in preRequisites)
            {
                var mandatoryFields = GetMandatoryFieldsForApplicationType(preRequisite.CredentialApplicationType.Id);
                responseData.Add(preRequisite.CredentialType.Id, mandatoryFields.Data);
            }

            return responseData;
        }

        public GenericResponse<IEnumerable<string>> GetExistingDocumentTypesForCredentialRequest(int credentialRequestId)
        {
            var credentialRequest = NHibernateSession.Current.Query<CredentialRequest>().FirstOrDefault(x => x.Id == credentialRequestId);
            var documentTypes = NHibernateSession.Current.Query<CredentialApplicationAttachment>().Where(x => x.CredentialApplication == credentialRequest.CredentialApplication).Select(x=>x.StoredFile.DocumentType.DisplayName);

            return documentTypes.ToList();
        }

        public GenericResponse<List<string>> GetMandatoryFieldsForApplicationType(int credentialApplicationTypeId)
        {
            var mandatoryFields = (from credentialApplicationField in NHibernateSession.Current.Query<CredentialApplicationField>() join
                                    credentialApplicationType in NHibernateSession.Current.Query<CredentialApplicationType>() on credentialApplicationField.CredentialApplicationType equals credentialApplicationType
                                   where credentialApplicationType.Id == credentialApplicationTypeId && credentialApplicationField.Mandatory
                                   select credentialApplicationField.Name).ToList();
            return mandatoryFields;
        }

        public GenericResponse<List<string>> GetMandatoryDocumentsForApplicationType(int credentialApplicationTypeId)
        {
            var mandatoryDocuments = (from credentialApplicationTypeDocumentType in NHibernateSession.Current.Query<CredentialApplicationTypeDocumentType>() join
                                    credentialApplicationType in NHibernateSession.Current.Query<CredentialApplicationType>() on credentialApplicationTypeDocumentType.CredentialApplicationType equals credentialApplicationType join
                                    documentType in NHibernateSession.Current.Query<DocumentType>() on credentialApplicationTypeDocumentType.DocumentType equals documentType
                                   where credentialApplicationType.Id == credentialApplicationTypeId && credentialApplicationTypeDocumentType.Mandatory
                                   select documentType.Name).ToList();
            return mandatoryDocuments;
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

        public PrerequisiteSummaryResult GetPreReqsForCredRequest(int credentialRequestId)
        {
            throw new NotImplementedException();
        }
    }
}
