using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using NHibernate.Criterion;
using Credential = F1Solutions.Naati.Common.Dal.Domain.Credential;

namespace F1Solutions.Naati.Common.Dal
{
 
    public class LogbookQueryService : ILogbookQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public LogbookQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public CredentialPointsResponse GetCredentialPoints(CredentialPointsRequest request)
        {
            var result = NHibernateSession.Current.Query<WorkPractice>()
                .Where(x => x.Credential.Id == request.CredentialId && x.Date >= request.StartDate &&
                                              x.Date <= request.EndDate).Select(x => x.Points).ToList().Sum();

            return new CredentialPointsResponse
            {
                CredentialId = request.CredentialId,
                Points = result
            };
        }

        public IEnumerable<CredentialsDetailsDto> GetCertificationPeriodCredentials(int certificationPeriodId)
        {
            var credentials = NHibernateSession.Current.Get<CertificationPeriod>(certificationPeriodId)
                .Credentials.ToList();


            var statuses =
                new ApplicationQueryHelper().GetCredentialStatusesByCredentialIds(
                    credentials.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList());


            var result = new List<CredentialsDetailsDto>();

            foreach (var x in credentials)
            {
                var credentialRequest = x.CredentialCredentialRequests.OrderByDescending(
                            c => c.CredentialRequest.StatusChangeDate)
                        .FirstOrDefault();
                
                if (credentialRequest == null)
                {
                    continue;
                }

                result.Add(MapCredential(x, credentialRequest.CredentialRequest, statuses[x.Id]));
            }

            return result.Distinct();
        }

        public IEnumerable<CredentialsDetailsDto> GetSubmittedRecertificationApplicationCredentials(int credentialApplicationId)
        {

            var personId = NHibernateSession.Current.Get<CredentialApplication>(credentialApplicationId).Person.Id;

            var credentialIds = NHibernateSession.Current.Query<Credential>()
                .Where(x => x.CertificationPeriod != null && x.CertificationPeriod.Person.Id == personId)
                .Select(x => x.Id).ToList();
            var invalidStatuses = BusinessLogicHelper.GetInvalidRecertificationCredentialRequestStatuses();
            var credentialRequests = NHibernateSession.Current.Query<CredentialRequest>()
                .Where(x => x.CredentialApplication.Id == credentialApplicationId && !invalidStatuses.Contains(x.CredentialRequestStatusType.Id));

            var statuses = new ApplicationQueryHelper().GetCredentialStatusesByCredentialIds(credentialIds);
            var foundCredentials = new List<CredentialsDetailsDto>();
            foreach (var credentialRequest in credentialRequests)
            {
                var credentialCredentialRequest = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                    .Where(x => credentialIds.Contains(x.Credential.Id) &&
                                x.CredentialRequest.Skill.Id == credentialRequest.Skill.Id &&
                                x.CredentialRequest.CredentialType.Id == credentialRequest.CredentialType.Id).OrderByDescending(y => y.Id).FirstOrDefault();

                if (credentialCredentialRequest == null)
                {
                    continue;
                }

                foundCredentials.Add(MapCredential(credentialCredentialRequest.Credential, credentialCredentialRequest.CredentialRequest, statuses[credentialCredentialRequest.Credential.Id]));
            }


            return foundCredentials;
        }

        public RecertificationDto GetSubmittedRecertificationApplicationForPeriod(int certificationPeriodId)
        {
            var invalidapplicationStatuses = BusinessLogicHelper.GetInvalidRecertificationApplicationStatuses();

            var recertificationApplication = NHibernateSession.Current.Query<Recertification>()
                .FirstOrDefault(r =>
                    r.CertificationPeriod.Id == certificationPeriodId &&
                    !invalidapplicationStatuses.Contains(r.CredentialApplication.CredentialApplicationStatusType.Id));

            if (recertificationApplication != null)
            {
                return new RecertificationDto
                {
                    ApplicationId = recertificationApplication.CredentialApplication.Id,
                    SubmittedDate = recertificationApplication.CredentialApplication.EnteredDate,
                    CredentialApplicationStatus = (CredentialApplicationStatusTypeName)recertificationApplication.CredentialApplication.CredentialApplicationStatusType.Id,
                    Id = recertificationApplication.Id
                };
            }

            return null;
        }

        public bool IsValidateWorkPracticeAttachment(int workPracticeAttachmentId, int naatiNumber)
        {
            var query = from workPracticeAttachment in NHibernateSession.Current.Query<WorkPracticeAttachment>()
                        join workPractice in NHibernateSession.Current.Query<WorkPractice>() on workPracticeAttachment.WorkPractice.Id equals workPractice.Id
                        join credentialCredentialRequest in NHibernateSession.Current.Query<CredentialCredentialRequest>() on workPractice.Credential.Id equals credentialCredentialRequest.Credential.Id
                        join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on credentialCredentialRequest.CredentialRequest.Id equals credentialRequest.Id
                        join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id
                        join person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals person.Id
                        join naatiEntity in NHibernateSession.Current.Query<NaatiEntity>() on person.Entity.Id equals naatiEntity.Id
                        where workPracticeAttachment.Id == workPracticeAttachmentId && naatiEntity.NaatiNumber == naatiNumber
                        select workPracticeAttachment.StoredFile.Id;

            var result = query.Any();

            return result;
        }

        public bool IsValidateWorkPractice(int workPracticeId, int naatiNumber)
        {
            var query = from workPractice in NHibernateSession.Current.Query<WorkPractice>()
                        join credentialCredentialRequest in NHibernateSession.Current.Query<CredentialCredentialRequest>() on workPractice.Credential.Id equals credentialCredentialRequest.Credential.Id
                        join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on credentialCredentialRequest.CredentialRequest.Id equals credentialRequest.Id
                        join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id
                        join person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals person.Id
                        join naatiEntity in NHibernateSession.Current.Query<NaatiEntity>() on person.Entity.Id equals naatiEntity.Id
                        where workPractice.Id == workPracticeId && naatiEntity.NaatiNumber == naatiNumber
                        select workPractice.Id;

            var result = query.Any();

            return result;
        }

        public bool IsValidActivityAttachment(int activityAttachmentId, int naatiNumber)
        {
            var query = from activityAttachment in NHibernateSession.Current.Query<ProfessionalDevelopmentActivityAttachment>()
                        join activity in NHibernateSession.Current.Query<ProfessionalDevelopmentActivity>() on activityAttachment.ProfessionalDevelopmentActivity.Id equals activity.Id
                        join person in NHibernateSession.Current.Query<Person>() on activity.Person.Id equals person.Id
                        join naatiEntity in NHibernateSession.Current.Query<NaatiEntity>() on person.Entity.Id equals naatiEntity.Id
                        where activityAttachment.Id == activityAttachmentId && naatiEntity.NaatiNumber == naatiNumber
                        select activityAttachment.StoredFile.Id;

            var result = query.Any();

            return result;
        }

        public bool IsValidActivity(int activityId, int naatiNumber)
        {
            var query = from activity in NHibernateSession.Current.Query<ProfessionalDevelopmentActivity>()
                        join person in NHibernateSession.Current.Query<Person>() on activity.Person.Id equals person.Id
                        join naatiEntity in NHibernateSession.Current.Query<NaatiEntity>() on person.Entity.Id equals naatiEntity.Id
                        where activity.Id == activityId && naatiEntity.NaatiNumber == naatiNumber
                        select activity.Id;

            var result = query.Any();

            return result;
        }

        public bool IsValidWorkPracticeStoredFieldId(int storedFieldId, int naatiNumber)
        {
            var query = from workPracticeAttachment in NHibernateSession.Current.Query<WorkPracticeAttachment>()
                        join workPractice in NHibernateSession.Current.Query<WorkPractice>() on workPracticeAttachment.WorkPractice.Id equals workPractice.Id
                        join credentialCredentialRequest in NHibernateSession.Current.Query<CredentialCredentialRequest>() on workPractice.Credential.Id equals credentialCredentialRequest.Credential.Id
                        join credentialRequest in NHibernateSession.Current.Query<CredentialRequest>() on credentialCredentialRequest.CredentialRequest.Id equals credentialRequest.Id
                        join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on credentialRequest.CredentialApplication.Id equals credentialApplication.Id
                        join person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals person.Id
                        join naatiEntity in NHibernateSession.Current.Query<NaatiEntity>() on person.Entity.Id equals naatiEntity.Id
                        where workPracticeAttachment.StoredFile.Id == storedFieldId && naatiEntity.NaatiNumber == naatiNumber
                        select workPracticeAttachment.StoredFile.Id;

            var result = query.Any();

            return result;
        }

        public bool IsValidActivityStoredFieldId(int storedFieldId, int naatiNumber)
        {
            var query = from activityAttachment in NHibernateSession.Current.Query<ProfessionalDevelopmentActivityAttachment>()
                        join activity in NHibernateSession.Current.Query<ProfessionalDevelopmentActivity>() on activityAttachment.ProfessionalDevelopmentActivity.Id equals activity.Id
                        join person in NHibernateSession.Current.Query<Person>() on activity.Person.Id equals person.Id
                        join naatiEntity in NHibernateSession.Current.Query<NaatiEntity>() on person.Entity.Id equals naatiEntity.Id
                        where activityAttachment.StoredFile.Id == storedFieldId && naatiEntity.NaatiNumber == naatiNumber
                        select activityAttachment.StoredFile.Id;

            var result = query.Any();

            return result;
        }

        public IEnumerable<CertificationPeriodRequestDto> GetSubmittedRecertificationCredentialRequests(GetCerfiationPeriodRequest request)
        {
            var dtos = new List<CertificationPeriodRequestDto>();
            var result = NHibernateSession.Current.Query<Recertification>()
                .Where(x => request.ApplicationIds.Union(new[] { 0 }).ToArray().Contains(x.CredentialApplication.Id))
                .ToList();

            var invalidStatuses = BusinessLogicHelper.GetInvalidRecertificationCredentialRequestStatuses();

            foreach (var recertification in result)
            {
                dtos.AddRange(recertification.CredentialApplication.CredentialRequests
                    .Where(c => !invalidStatuses.Contains(c.CredentialRequestStatusType.Id))
                    .Select(x => MapCertificationPeriodRequestDto(x, recertification.CertificationPeriod)));
            }

            return dtos;
        }

        private CertificationPeriodRequestDto MapCertificationPeriodRequestDto(CredentialRequest credentialRequest, CertificationPeriod certificationPeriod)
        {
            return new CertificationPeriodRequestDto
            {
                CredentialRequestId = credentialRequest.Id,
                ExternalName = credentialRequest.CredentialType.ExternalName,
                Skill = credentialRequest.Skill.DisplayName,
                SkillId = credentialRequest.Skill.Id,
                CertificationPeriodId = certificationPeriod.Id
            };
        }

        public GetCredentialResponse GetCredentials(int naatiNumber)
        {
            var credentials =
                NHibernateSession.Current.Query<Person>()
                    .FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber)
                    ?
                    .CredentialApplications.SelectMany(
                        x => x.CredentialRequests.Where(cr => cr.CredentialType.Certification)
                            .SelectMany(y => y.Credentials))
                    .GroupBy(c => c.Id)
                    .Select(g => g.First()).ToList();

            var response = new GetCredentialResponse { List = new List<CredentialsDetailsDto>() };

            if (credentials?.Any() == true)
            {
                var statuses = new ApplicationQueryHelper().GetCredentialStatusesByCredentialIds(
                        credentials.Select(x => x.Id).ToList().Union(new[] { 0 }).ToList());

                response.List.AddRange(
                    credentials.Select(x =>
                        {
                            var credentialRequest = x.CredentialCredentialRequests.OrderByDescending(
                                    c => c.CredentialRequest.StatusChangeDate)
                                .First().CredentialRequest;

                            return MapCredential(x, credentialRequest, statuses[x.Id]);
                        }).ToList());
            }

            return response;
        }

        private CredentialsDetailsDto MapCredential(Credential credential, CredentialRequest credentialRequest,
            CredentialStatusTypeName status)
        {
            var credentialEndDate =
                credential.CertificationPeriod.EndDate > credential.CertificationPeriod.OriginalEndDate
                    ? credential.CertificationPeriod.EndDate
                    : credential.CertificationPeriod.OriginalEndDate;

            credentialEndDate =
                credential.TerminationDate.HasValue &&
                (credential.TerminationDate.GetValueOrDefault() < credentialEndDate)
                    ? credential.TerminationDate.GetValueOrDefault()
                    : credentialEndDate;
            return new CredentialsDetailsDto
            {
                Id = credential.Id,
                CredentialType = credentialRequest.CredentialType.ExternalName,
                Skill = credentialRequest.Skill.DisplayName,
                WorkPracticeUnits = credentialRequest.CredentialType.CredentialCategory
                    .WorkPracticeUnits,
                WorkPracticePoints = credentialRequest.CredentialType.CredentialCategory
                    .WorkPracticePoints,
                CredentialRequestId = credentialRequest.Id,
                CredentialApplicationTypeCategoryId = credentialRequest.CredentialApplication
                    .CredentialApplicationType.CredentialApplicationTypeCategory.Id,
                CredentialApplicationEnteredDate =
                    credentialRequest.CredentialApplication.EnteredDate,
                StartDate = credential.StartDate,
                EndDate = credentialEndDate,
                SkillId = credentialRequest.Skill.Id,
                CredentialTypeId = credentialRequest.CredentialType.Id,
                CategoryId = credentialRequest.CredentialType.CredentialCategory.Id,
                CredentialStatus = status.ToString()
            };
        }

        public IEnumerable<ProfessionalDevelopmentActivityDto> GetRecertificationAtivitiesForApplication(int applicationId)
        {
            var activities = NHibernateSession.Current.Query<ProfessionalDevelopmentCredentialApplication>()
                .Where(x => x.CredentialApplication.Id == applicationId).Select(a => a.ProfessionalDevelopmentActivity).ToList()
                .Select(a => new ProfessionalDevelopmentActivityDto
                {
                    Description = a.Description,
                    DateCompleted = a.DateCompleted,
                    Notes = a.Notes,
                    ProfessionalDevelopmentRequirementId = a.ProfessionalDevelopmentRequirement.Id,
                    ProfessionalDevelopmentCategoryId = a.ProfessionalDevelopmentCategory.Id,
                    ProfessionalDevelopmentCategoryGroupId = a.ProfessionalDevelopmentCategory.ProfessionalDevelopmentCategoryGroup?.Id,
                    ProfessionalDevelopmentCategoryName = a.ProfessionalDevelopmentCategory.Name,
                    ProfessionalDevelopmentCategoryGroupName = a.ProfessionalDevelopmentCategory.ProfessionalDevelopmentCategoryGroup?.Name,
                    Points = a.ProfessionalDevelopmentCategory.CategoryRequirements.FirstOrDefault(x => x.ProfessionalDevelopmentRequirement.Id == a.ProfessionalDevelopmentRequirement.Id)?.Points ?? 0,
                    SectionIds = a.ProfessionalDevelopmentCategory.ProfessionalDevelopmentSectionCategories.Select(x => x.ProfessionalDevelopmentSection.Id).ToList(),
                    Id = a.Id
                }).ToList();

            return activities;

        }

        public void DeleteWorkPractice(int id)
        {
            var domain = NHibernateSession.Current.Get<WorkPractice>(id);
            var credentialRequest = NHibernateSession.Current.Query<WorkPracticeCredentialRequest>()
                .FirstOrDefault(x => x.WorkPractice.Id == id);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                if (credentialRequest != null)
                    NHibernateSession.Current.Delete(credentialRequest);
                NHibernateSession.Current.Delete(domain);
                transaction.Commit();
            }
        }

        public void DetachWorkPractice(int id)
        {
            var credentialRequest = NHibernateSession.Current.Query<WorkPracticeCredentialRequest>()
                .FirstOrDefault(x => x.WorkPractice.Id == id);

            if (credentialRequest == null)
            {
                throw new ArgumentOutOfRangeException(nameof(credentialRequest));
            }

            NHibernateSession.Current.Delete(credentialRequest);
            NHibernateSession.Current.Flush();
        }

        public void DeleteProfessionalDevelopmentCategoryRequirement(int id)
        {
            var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentCategoryRequirement>(id);
            NHibernateSession.Current.Delete(domain);
            NHibernateSession.Current.Flush();
        }

        public void DeleteProfessionalDevelopmentActivity(int id)
        {
            var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentActivity>(id);

            var attachments = NHibernateSession.Current.Query<ProfessionalDevelopmentCredentialApplication>()
                .Where(x => x.ProfessionalDevelopmentActivity.Id == id).ToList();

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                foreach (var attachment in attachments)
                {
                    NHibernateSession.Current.Delete(attachment);
                }
                NHibernateSession.Current.Delete(domain);

                transaction.Commit();
            }
        }

        public void DeleteProfessionalDevelopmentCategory(int id)
        {
            var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentCategory>(id);
            NHibernateSession.Current.Delete(domain);
            NHibernateSession.Current.Flush();
        }

        public void DeleteProfessionalDevelopmentRequirement(int id)
        {
            var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentRequirement>(id);
            NHibernateSession.Current.Delete(domain);
            NHibernateSession.Current.Flush();
        }

        public void DeleteProfessionalDevelopmentSection(int id)
        {
            var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentSection>(id);
            NHibernateSession.Current.Delete(domain);
            NHibernateSession.Current.Flush();
        }

        public void DeleteWorkPracticeAttachment(int id)
        {
            var domain = NHibernateSession.Current.Get<WorkPracticeAttachment>(id);
            NHibernateSession.Current.Delete(domain);
            NHibernateSession.Current.Flush();
        }

        public void DeleteProfessionalDevelopmentActivityAttachment(int id)
        {
            var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentActivityAttachment>(id);
            NHibernateSession.Current.Delete(domain);
            NHibernateSession.Current.Flush();
        }

        public void DetachProfessionalDevelopmentActivity(int activityId, int credentialApplicationId)
        {
            var link = NHibernateSession.Current.Query<ProfessionalDevelopmentCredentialApplication>()
                .First(x => x.ProfessionalDevelopmentActivity.Id == activityId && x.CredentialApplication.Id == credentialApplicationId);

            NHibernateSession.Current.Delete(link);
            NHibernateSession.Current.Flush();
        }

        public void AttachProfessionalDevelopmentActivity(int activityId, int credentialApplicationId)
        {
            var activity = NHibernateSession.Current.Load<ProfessionalDevelopmentActivity>(activityId);
            var application = NHibernateSession.Current.Load<CredentialApplication>(credentialApplicationId);

            var link = new ProfessionalDevelopmentCredentialApplication
            {
                ProfessionalDevelopmentActivity = activity,
                CredentialApplication = application

            };

            NHibernateSession.Current.SaveOrUpdate(link);
            NHibernateSession.Current.Flush();
        }

        public void AttachWorkPractice(int workPracticeId, int credentialApplicationId, int credentialId)
        {
            var workPractice = NHibernateSession.Current.Load<WorkPractice>(workPracticeId);

            var credentialRequest = GetCredentialRequest(credentialApplicationId, credentialId);

            var link = new WorkPracticeCredentialRequest
            {
                WorkPractice = workPractice,
                CredentialRequest = credentialRequest
            };

            NHibernateSession.Current.SaveOrUpdate(link);
            NHibernateSession.Current.Flush();
        }

        private CredentialRequest GetCredentialRequest(int credentialApplicationId, int credentialId)
        {
            var invalidStatuses = BusinessLogicHelper.GetInvalidRecertificationCredentialRequestStatuses();

            var previousRequest = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                .Where(x => x.Credential.Id == credentialId)
                .OrderByDescending(x => x.Id)
                .First().CredentialRequest;

            var credentialRequest = NHibernateSession.Current.Query<CredentialRequest>().First(x =>
                x.CredentialApplication.Id == credentialApplicationId && x.Skill.Id == previousRequest.Skill.Id &&
                x.CredentialType.Id == previousRequest.CredentialType.Id &&
                !invalidStatuses.Contains(x.CredentialRequestStatusType.Id));
            return credentialRequest;
        }

        public IEnumerable<WorkPracticeDetails> GetWorkPracticesForRecertificationApplication(int credentialId, int credentialApplicationId)
        {

            var previousRequest = NHibernateSession.Current.Query<CredentialCredentialRequest>()
                .Where(x => x.Credential.Id == credentialId)
                .OrderByDescending(x => x.Id)
                .First().CredentialRequest;
            var invalidStatuses = BusinessLogicHelper.GetInvalidRecertificationCredentialRequestStatuses();

            var currentRecertificationRequestId = NHibernateSession.Current.Query<CredentialRequest>()
                .First(x => x.CredentialApplication.Id == credentialApplicationId && x.Skill.Id == previousRequest.Skill.Id &&
                            x.CredentialType.Id == previousRequest.CredentialType.Id && !invalidStatuses.Contains(x.CredentialRequestStatusType.Id)).Id;

            var workPractices = NHibernateSession.Current.Query<WorkPracticeCredentialRequest>()
                .Where(x => x.CredentialRequest.Id == currentRecertificationRequestId)
                .Select(y => y.WorkPractice);
            var list = new List<WorkPracticeDetails>();
            foreach (var workPractice in workPractices)
            {
                list.Add(MapWorkPractice(workPractice));
            }

            return list;
        }

        private WorkPracticeDetails MapWorkPractice(WorkPractice workPractice)
        {
            return new WorkPracticeDetails
            {
                Date = workPractice.Date,
                Description = workPractice.Description,
                Points = workPractice.Points,
                HasAttachments = workPractice.WorkPracticeAttachments.Any(),
                Id = workPractice.Id
            };
        }


        public IEnumerable<WorkPracticeDetails> GetWorkPractices(int credentialId)
        {
            var list = new List<WorkPracticeDetails>();

            CredentialRequest credentialRequest = null;
            WorkPracticeCredentialRequest workPracticeCredentialRequest = null;
            CredentialRequestStatusType credentialRequestStatusType = null;
            WorkPractice workPractice = null;
            Credential credential = null;
            var allocatedActivities = QueryOver.Of(() => workPracticeCredentialRequest)
                .Inner.JoinAlias(x => x.CredentialRequest, () => credentialRequest)
                .Inner.JoinAlias(x => credentialRequest.CredentialRequestStatusType, () => credentialRequestStatusType)
                .Inner.JoinAlias(x => workPracticeCredentialRequest.WorkPractice, () => workPractice)
                .Inner.JoinAlias(x => workPractice.Credential, () => credential)
                .Where(x => credential.Id == credentialId)
                .Where(x => credentialRequestStatusType.Id != (int)CredentialRequestStatusTypeName.Deleted)
                .Where(x => credentialRequestStatusType.Id != (int)CredentialRequestStatusTypeName.Rejected)
                .Where(x => credentialRequestStatusType.Id != (int)CredentialRequestStatusTypeName.Withdrawn)
                .Where(x => credentialRequestStatusType.Id != (int)CredentialRequestStatusTypeName.Cancelled)
                .Select(x => workPractice.Id);

            var practices = NHibernateSession.Current.QueryOver<WorkPractice>().Where(wp => wp.Credential.Id == credentialId)
                .WithSubquery.
                WhereProperty(x => x.Id).NotIn(allocatedActivities).List<WorkPractice>();

            foreach (var practice in practices)
            {
                list.Add(MapWorkPractice(practice));
            }

            return list;
        }

        public ProfessionalDevelopmentCategoryRequirementResponse GetProfessionalDevelopmentCategoryRequirement(int id)
        {
            var categoryRequirement = NHibernateSession.Current.Get<ProfessionalDevelopmentCategoryRequirement>(id);
            return new ProfessionalDevelopmentCategoryRequirementResponse
            {
                ProfessionalDevelopmentCategory = new ProfessionalDevelopmentCategoryResponse
                {
                    Description = categoryRequirement.ProfessionalDevelopmentCategory.Description,
                    Name = categoryRequirement.ProfessionalDevelopmentCategory.Name,
                },
                Points = categoryRequirement.Points,
                ProfessionalDevelopmentRequirement = new ProfessionalDevelopmentRequirementResponse
                {
                    Name = categoryRequirement.ProfessionalDevelopmentRequirement.Name,
                    DisplayName = categoryRequirement.ProfessionalDevelopmentRequirement.DisplayName
                },
                Id = categoryRequirement.Id
            };
        }

        public ProfessionalDevelopmentActivityResponse GetProfessionalDevelopmentActivities(GetActivitiesRequest request)
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate;
            var personId = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == request.NaatiNumber)?.Id;

            ProfessionalDevelopmentCredentialApplication pdApplication = null;
            ProfessionalDevelopmentActivity pdActivity = null;
            Person person = null;
            CredentialApplication application = null;
            CredentialApplicationStatusType applicationStatus = null;
            var allocatedActivitiesIds = QueryOver.Of(() => pdApplication)
                .Inner.JoinAlias(x => pdApplication.ProfessionalDevelopmentActivity, () => pdActivity)
                .Inner.JoinAlias(x => pdActivity.Person, () => person)
                .Inner.JoinAlias(x => pdApplication.CredentialApplication, () => application)
                .Inner.JoinAlias(x => application.CredentialApplicationStatusType, () => applicationStatus)

                .Where(x => person.Id == personId)
                .Where(x => applicationStatus.Id != (int)CredentialApplicationStatusTypeName.Deleted)
                .Where(x => applicationStatus.Id != (int)CredentialApplicationStatusTypeName.Rejected)
                .Where(x => applicationStatus.Id != (int)CredentialApplicationStatusTypeName.Draft)
                .Select(x => pdActivity.Id);


            var activities = NHibernateSession.Current.QueryOver<ProfessionalDevelopmentActivity>()
                .Where(a => a.Person.Id == personId);

            if (startDate.HasValue)
            {
                activities = activities.Where(a => a.DateCompleted >= startDate);
            }
            if (endDate.HasValue)
            {
                activities = activities.Where(a => a.DateCompleted <= endDate);
            }
            activities = activities.WithSubquery.WhereProperty(a => a.Id)
                .NotIn(allocatedActivitiesIds);
            var resuList = activities.List<ProfessionalDevelopmentActivity>();
            var list = resuList.Select(a => new ProfessionalDevelopmentActivityDto
            {
                Description = a.Description,
                DateCompleted = a.DateCompleted,
                Notes = a.Notes,
                ProfessionalDevelopmentRequirementId = a.ProfessionalDevelopmentRequirement.Id,
                ProfessionalDevelopmentCategoryId = a.ProfessionalDevelopmentCategory.Id,
                ProfessionalDevelopmentCategoryGroupId = a.ProfessionalDevelopmentCategory
                    .ProfessionalDevelopmentCategoryGroup?.Id,
                ProfessionalDevelopmentCategoryName = a.ProfessionalDevelopmentCategory.Name,
                ProfessionalDevelopmentCategoryGroupName =
                    a.ProfessionalDevelopmentCategory.ProfessionalDevelopmentCategoryGroup?.Name,
                Points = a.ProfessionalDevelopmentCategory.CategoryRequirements.FirstOrDefault(x =>
                                 x.ProfessionalDevelopmentRequirement.Id == a.ProfessionalDevelopmentRequirement.Id)
                             ?.Points ?? 0,
                SectionIds = a.ProfessionalDevelopmentCategory.ProfessionalDevelopmentSectionCategories
                    .Select(x => x.ProfessionalDevelopmentSection.Id).ToList(),
                Id = a.Id
            }).ToList();
            return new ProfessionalDevelopmentActivityResponse { List = list };
        }

        public ProfessionalDevelopmentActivityDto GetProfessionalDevelopmentActivity(int id)
        {
            return (from a in NHibernateSession.Current.Query<ProfessionalDevelopmentActivity>()
                    join cr in NHibernateSession.Current.Query<ProfessionalDevelopmentCategoryRequirement>() on
                    new
                    {
                        x = a.ProfessionalDevelopmentCategory.Id,
                        y = a.ProfessionalDevelopmentRequirement.Id
                    }
                    equals new
                    {
                        x = cr.ProfessionalDevelopmentCategory.Id,
                        y = cr.ProfessionalDevelopmentRequirement.Id
                    }
                    where a.Id == id
                    select new ProfessionalDevelopmentActivityDto
                    {
                        Description = a.Description,
                        DateCompleted = a.DateCompleted,
                        Notes = a.Notes,
                        ProfessionalDevelopmentRequirementId = a.ProfessionalDevelopmentRequirement.Id,
                        ProfessionalDevelopmentCategoryId = a.ProfessionalDevelopmentCategory.Id,
                        ProfessionalDevelopmentCategoryName = a.ProfessionalDevelopmentCategory.Name,
                        Points = cr.Points,
                        Id = a.Id
                    }).ToList().FirstOrDefault();
        }

        public IEnumerable<ProfessionalDevelopmentCategoryResponse> GetProfessionalDevelopmentCategories()
        {
            return NHibernateSession.Current.Query<ProfessionalDevelopmentCategory>().Select(c =>
                new ProfessionalDevelopmentCategoryResponse
                {
                    Description = c.Description,
                    Name = c.Name,
                    Id = c.Id,
                    ProfessionalDevelopmentCategoryGroup = new ProfessionalDevelopmentCategoryGroupResponse
                    {
                        Description = c.ProfessionalDevelopmentCategoryGroup != null
                                ? c.ProfessionalDevelopmentCategoryGroup.Description
                                : "",
                        Name = c.ProfessionalDevelopmentCategoryGroup != null
                                ? c.ProfessionalDevelopmentCategoryGroup.Name
                                : "",
                        Id =
                            c.ProfessionalDevelopmentCategoryGroup != null
                                ? c.ProfessionalDevelopmentCategoryGroup.Id
                                : 0
                    },
                }).ToList();
        }

        public IEnumerable<ProfessionalDevelopmentRequirementResponse> GetProfessionalDevelopmentRequirements(int categoryId)
        {

            return (from c in NHibernateSession.Current.Query<ProfessionalDevelopmentCategory>()
                    join cr in NHibernateSession.Current.Query<ProfessionalDevelopmentCategoryRequirement>() on c.Id equals
                    cr.ProfessionalDevelopmentCategory.Id
                    join r in NHibernateSession.Current.Query<ProfessionalDevelopmentRequirement>() on
                    cr.ProfessionalDevelopmentRequirement.Id equals r.Id
                    where c.Id == categoryId
                    select new ProfessionalDevelopmentRequirementResponse
                    {
                        DisplayName = r.DisplayName,
                        Name = r.Name,
                        Id = r.Id,
                        Points = cr.Points
                    }).ToList();
        }

        public ProfessionalDevelopmentCategoryRequirementListResponse GetProfessionalDevelopmentCategoriesRequirements(
            int sectionId)
        {
            var requirementList = (from c in NHibernateSession.Current.Query<ProfessionalDevelopmentCategory>()
                                   join cr in NHibernateSession.Current.Query<ProfessionalDevelopmentCategoryRequirement>() on c.Id equals
                                   cr.ProfessionalDevelopmentCategory.Id
                                   join r in NHibernateSession.Current.Query<ProfessionalDevelopmentRequirement>() on
                                   cr.ProfessionalDevelopmentRequirement.Id equals r.Id
                                   join sc in NHibernateSession.Current.Query<ProfessionalDevelopmentSectionCategory>() on
                                   c.Id equals sc.ProfessionalDevelopmentCategory.Id
                                   join s in NHibernateSession.Current.Query<ProfessionalDevelopmentSection>() on
                                   sc.ProfessionalDevelopmentSection.Id equals s.Id
                                   where sc.ProfessionalDevelopmentSection.Id == sectionId
                                   select new ProfessionalDevelopmentRequirementResponse
                                   {

                                       DisplayName = r.DisplayName,
                                       Name = r.Name,
                                       Id = r.Id,

                                   }).ToList();

            var categoryList = (from c in NHibernateSession.Current.Query<ProfessionalDevelopmentCategory>()
                                join cr in NHibernateSession.Current.Query<ProfessionalDevelopmentCategoryRequirement>() on c.Id equals
                                cr.ProfessionalDevelopmentCategory.Id
                                join r in NHibernateSession.Current.Query<ProfessionalDevelopmentRequirement>() on
                                cr.ProfessionalDevelopmentRequirement.Id equals r.Id
                                join sc in NHibernateSession.Current.Query<ProfessionalDevelopmentSectionCategory>() on
                                c.Id equals sc.ProfessionalDevelopmentCategory.Id
                                join s in NHibernateSession.Current.Query<ProfessionalDevelopmentSection>() on
                                sc.ProfessionalDevelopmentSection.Id equals s.Id
                                where sc.ProfessionalDevelopmentSection.Id == sectionId
                                select new ProfessionalDevelopmentCategoryResponse
                                {

                                    Description = c.Description,
                                    Name = c.Name,
                                    Id = c.Id,
                                    ProfessionalDevelopmentCategoryGroup = new ProfessionalDevelopmentCategoryGroupResponse
                                    {
                                        Description = c.ProfessionalDevelopmentCategoryGroup != null
                                            ? c.ProfessionalDevelopmentCategoryGroup.Description
                                            : "",
                                        Name = c.ProfessionalDevelopmentCategoryGroup != null
                                            ? c.ProfessionalDevelopmentCategoryGroup.Name
                                            : "",
                                        Id =
                                            c.ProfessionalDevelopmentCategoryGroup != null
                                                ? c.ProfessionalDevelopmentCategoryGroup.Id
                                                : 0
                                    }

                                }).ToList();

            return new ProfessionalDevelopmentCategoryRequirementListResponse
            {
                ProfessionalDevelopmentRequirement = requirementList,
                ProfessionalDevelopmentCategory = categoryList
            };
        }


        public ProfessionalDevelopmentCategoryResponse GetProfessionalDevelopmentCategory(int id)
        {
            var category = NHibernateSession.Current.Get<ProfessionalDevelopmentCategory>(id);
            return new ProfessionalDevelopmentCategoryResponse
            {
                Description = category.Description,
                Name = category.Name,
                Id = category.Id
            };
        }

        public ProfessionalDevelopmentRequirementResponse GetProfessionalDevelopmentRequirement(int id)
        {
            var requirement = NHibernateSession.Current.Get<ProfessionalDevelopmentRequirement>(id);
            var model = _autoMapperHelper.Mapper.Map<ProfessionalDevelopmentRequirement, ProfessionalDevelopmentRequirementResponse>(requirement);
            return model;
        }

        public ProfessionalDevelopmentSectionResponse GetProfessionalDevelopmentSection(int id)
        {
            var section = NHibernateSession.Current.Get<ProfessionalDevelopmentSection>(id);
            var model = _autoMapperHelper.Mapper.Map<ProfessionalDevelopmentSection, ProfessionalDevelopmentSectionResponse>(section);
            return model;
        }

        public IEnumerable<WorkPracticeAttachmentResponse> GetWorkPracticeAttachments(int workPracticeId)
        {
            return
                NHibernateSession.Current.Query<WorkPracticeAttachment>()
                    .Where(x => x.WorkPractice.Id == workPracticeId)
                    .Select(workAttachment =>
                        new WorkPracticeAttachmentResponse()
                        {
                            Description = workAttachment.Description,
                            StoredFile = new StoredFileDto
                            {
                                DocumentType = new DocumentTypeDto
                                {
                                    Name = workAttachment.StoredFile.DocumentType.Name ?? "",
                                    DisplayName = workAttachment.StoredFile.DocumentType.DisplayName ?? "",
                                },
                                ExternalFileId = workAttachment.StoredFile.ExternalFileId,
                                FileName = workAttachment.StoredFile.FileName,
                                FileSize = workAttachment.StoredFile.FileSize,
                                Id = workAttachment.StoredFile.Id,
                                UploadedByName = workAttachment.StoredFile.UploadedByUser.FullName,
                                UploadedDateTime = workAttachment.StoredFile.UploadedDateTime,
                                StoredFileStatusType = workAttachment.StoredFile.StoredFileStatusType.Id,
                                StoredFileStatusChangedDate = workAttachment.StoredFile.StoredFileStatusChangeDate
                            },
                            Id = workAttachment.Id
                        }).ToList();
        }

        public ProfessionalDevelopmentActivityAttachmentResponse GetProfessionalDevelopmentActivityAttachments(int activityId)
        {
            var list = NHibernateSession.Current.Query<ProfessionalDevelopmentActivityAttachment>()
                    .Where(x => x.ProfessionalDevelopmentActivity.Id == activityId).Select(activityAttachment => new ProfessionalDevelopmentActivityAttachmentDto
                    {
                        Description = activityAttachment.Description,
                        StoredFile = new StoredFileDto
                        {
                            Id = activityAttachment.StoredFile.Id,
                            DocumentType = new DocumentTypeDto
                            {
                                Name = activityAttachment.StoredFile.DocumentType.Name ?? "",
                                DisplayName = activityAttachment.StoredFile.DocumentType.DisplayName ?? "",                                
                            },
                            ExternalFileId = activityAttachment.StoredFile.ExternalFileId,
                            FileName = activityAttachment.StoredFile.FileName,
                            FileSize = activityAttachment.StoredFile.FileSize,
                            StoredFileStatusChangedDate = activityAttachment.StoredFile.StoredFileStatusChangeDate,
                            StoredFileStatusType = activityAttachment.StoredFile.StoredFileStatusType.Id
                        },
                        ProfessionalDevelopmentActivity = new ProfessionalDevelopmentActivityDto
                        {
                            Description = activityAttachment.ProfessionalDevelopmentActivity.Description,
                            DateCompleted = activityAttachment.ProfessionalDevelopmentActivity.DateCompleted,
                            Notes = activityAttachment.ProfessionalDevelopmentActivity.Notes,
                            ProfessionalDevelopmentRequirement = new ProfessionalDevelopmentRequirementResponse
                            {
                                Name = activityAttachment.ProfessionalDevelopmentActivity.ProfessionalDevelopmentRequirement.Name,
                                DisplayName = activityAttachment.ProfessionalDevelopmentActivity.ProfessionalDevelopmentRequirement.DisplayName
                            },
                            ProfessionalDevelopmentCategory = new ProfessionalDevelopmentCategoryResponse
                            {
                                Description = activityAttachment.ProfessionalDevelopmentActivity.ProfessionalDevelopmentCategory.Description,
                                Name = activityAttachment.ProfessionalDevelopmentActivity.ProfessionalDevelopmentCategory.Name,
                            }
                        },
                        Id = activityAttachment.Id
                    }).ToList();

            return new ProfessionalDevelopmentActivityAttachmentResponse { List = list };
        }

        public CreateOrUpdateResponse CreateOrUpdateWorkPractice(WorkPracticeRequest model)
        {
            var credential = NHibernateSession.Current.Get<Credential>(model.CredentialId);
            if (credential == null)
            {
                throw new WebServiceException($"Credential is not found (Credential ID {model.CredentialId})");
            }

            WorkPractice domain;
            if (model.Id != null)
            {
                domain = NHibernateSession.Current.Get<WorkPractice>(model.Id);
                domain.Date = model.Date;
                domain.Credential = credential;
                domain.Description = model.Description;
                domain.Points = model.Points;
            }
            else
            {
                domain = new WorkPractice()
                {
                    Date = model.Date,
                    Credential = credential,
                    Description = model.Description,
                    Points = model.Points
                };
            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.SaveOrUpdate(domain);
                if (model.CredentialApplicationId > 0 && model.Id == null)
                {
                    var credentialRequest = GetCredentialRequest(model.CredentialApplicationId, model.CredentialId);
                    var workPracticeCredentialRequest = new WorkPracticeCredentialRequest()
                    {
                        WorkPractice = domain,
                        CredentialRequest = credentialRequest
                    };
                    NHibernateSession.Current.SaveOrUpdate(workPracticeCredentialRequest);
                }
                transaction.Commit();
            }


            NHibernateSession.Current.Flush();

            return new CreateOrUpdateResponse
            {
                Id = domain.Id
            };
        }

        public void CreateOrUpdateProfessionalDevelopmentCategoryRequirement(
            ProfessionalDevelopmentCategoryRequirementRequest model)
        {
            var category =
                NHibernateSession.Current.Get<ProfessionalDevelopmentCategory>(model.ProfessionalDevelopmentCategoryId);
            var requirement =
                NHibernateSession.Current.Get<ProfessionalDevelopmentRequirement>(
                    model.ProfessionalDevelopmentRequirementId);

            if (category == null)
            {
                throw new WebServiceException(
                    $"Professional Development Category is not found (Professional Development Category ID {model.ProfessionalDevelopmentCategoryId})");
            }
            if (requirement == null)
            {
                throw new WebServiceException(
                    $"Professional Development Requirement is not found (Professional Development Requirement ID {model.ProfessionalDevelopmentRequirementId})");
            }
            ProfessionalDevelopmentCategoryRequirement domain;
            if (model.Id != null)
            {
                domain = NHibernateSession.Current.Get<ProfessionalDevelopmentCategoryRequirement>(model.Id);
                domain.ProfessionalDevelopmentCategory = category;
                domain.ProfessionalDevelopmentRequirement = requirement;
                domain.Points = model.Points;
            }
            else
            {
                domain = new ProfessionalDevelopmentCategoryRequirement()
                {
                    Points = model.Points,
                    ProfessionalDevelopmentCategory = category,
                    ProfessionalDevelopmentRequirement = requirement
                };
            }
            NHibernateSession.Current.SaveOrUpdate(domain);
            NHibernateSession.Current.Flush();
        }

        public CreateOrUpdateResponse CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest model, int naatiNumber)
        {
            var category =
                NHibernateSession.Current.Get<ProfessionalDevelopmentCategory>(model.ProfessionalDevelopmentCategoryId);
            var requirement =
                NHibernateSession.Current.Get<ProfessionalDevelopmentRequirement>(
                    model.ProfessionalDevelopmentRequirementId);
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            if (person == null)
            {
                throw new WebServiceException(
                    $"Person is not found (Naati Number {naatiNumber})");
            }
            if (category == null)
            {
                throw new WebServiceException(
                    $"Professional Development Category is not found (Professional Development Category ID {model.ProfessionalDevelopmentCategoryId})");
            }
            if (requirement == null)
            {
                throw new WebServiceException(
                    $"Professional Development Requirement is not found (Professional Development Requirement ID {model.ProfessionalDevelopmentRequirementId})");
            }

            var items = new List<object>();
            if (model.Id != null)
            {
                var domain = NHibernateSession.Current.Get<ProfessionalDevelopmentActivity>(model.Id);
                domain.ProfessionalDevelopmentCategory = category;
                domain.ProfessionalDevelopmentRequirement = requirement;
                domain.DateCompleted = model.DateCompleted;
                domain.Description = model.Description;
                domain.Notes = model.Notes;
                domain.Person = person;

                items.Add(domain);
            }
            else
            {
                var domain = new ProfessionalDevelopmentActivity()
                {
                    ProfessionalDevelopmentCategory = category,
                    ProfessionalDevelopmentRequirement = requirement,
                    DateCompleted = model.DateCompleted,
                    Description = model.Description,
                    Notes = model.Notes,
                    Person = person
                };
                items.Add(domain);
                if (model.CredentialApplicationId.HasValue)
                {
                    var application = NHibernateSession.Current.Get<CredentialApplication>(model.CredentialApplicationId);

                    var applicationLink = new ProfessionalDevelopmentCredentialApplication
                    {
                        ProfessionalDevelopmentActivity = domain,
                        CredentialApplication = application
                    };

                    items.Add(applicationLink);
                }

            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                foreach (var item in items)
                {
                    NHibernateSession.Current.SaveOrUpdate(item);
                }

                transaction.Commit();
            }

            return new CreateOrUpdateResponse
            {
                Id = items.OfType<ProfessionalDevelopmentActivity>().First().Id
            };
        }

        public void CreateOrUpdateProfessionalDevelopmentCategory(ProfessionalDevelopmentCategoryRequest model)
        {
            var section =
                NHibernateSession.Current.Get<ProfessionalDevelopmentSection>(model.ProfessionalDevelopmentSectionId);
            if (section == null)
            {
                throw new WebServiceException(
                    $"Professional Development Section is not found (Professional Development Section ID {model.ProfessionalDevelopmentSectionId})");
            }

            ProfessionalDevelopmentCategory domain;
            if (model.Id != null)
            {
                domain = NHibernateSession.Current.Get<ProfessionalDevelopmentCategory>(model.Id);
                domain.Name = model.Name;
                domain.Description = model.Description;

            }
            else
            {
                domain = new ProfessionalDevelopmentCategory()
                {
                    Description = model.Description,
                    Name = model.Name
                };
            }

            var sectionCategory =
                domain.ProfessionalDevelopmentSectionCategories?.FirstOrDefault(
                    x => x.ProfessionalDevelopmentSection.Id == section.Id) ??
                new ProfessionalDevelopmentSectionCategory
                {
                    ProfessionalDevelopmentCategory = domain,
                    ProfessionalDevelopmentSection = section
                };

            NHibernateSession.Current.SaveOrUpdate(domain);
            NHibernateSession.Current.SaveOrUpdate(sectionCategory);
            NHibernateSession.Current.Flush();
        }

        public void CreateOrUpdateProfessionalDevelopmentRequirement(ProfessionalDevelopmentRequirementRequest model)
        {
            ProfessionalDevelopmentRequirement domain;
            if (model.Id != null)
            {
                domain = NHibernateSession.Current.Get<ProfessionalDevelopmentRequirement>(model.Id);
                domain.Name = model.Name;
                domain.DisplayName = model.DisplayName;
            }
            else
            {
                domain = new ProfessionalDevelopmentRequirement()
                {
                    Name = model.Name,
                    DisplayName = model.DisplayName
                };
            }

            NHibernateSession.Current.SaveOrUpdate(domain);
            NHibernateSession.Current.Flush();
        }

        public void CreateOrUpdateProfessionalDevelopmentSection(ProfessionalDevelopmentSectionRequest model)
        {
            ProfessionalDevelopmentSection domain;
            if (model.Id != null)
            {
                domain = NHibernateSession.Current.Get<ProfessionalDevelopmentSection>(model.Id);
                domain.Name = model.Name;
                domain.Description = model.Description;
            }
            else
            {
                domain = new ProfessionalDevelopmentSection()
                {
                    Name = model.Name,
                    Description = model.Description
                };
            }

            NHibernateSession.Current.SaveOrUpdate(domain);
            NHibernateSession.Current.Flush();
        }

        public AttachmentResponse CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest model)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                Type = StoredFileType.WorkPractice,
                StoragePath = model.StoragePath,
                UploadedByUserId = model.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = model.FilePath,
                TokenToRemoveFromFilename = model.TokenToRemoveFromFilename,
                UpdateStoredFileId = model.StoredFileId != 0 ? (int?)model.StoredFileId : null,
                UpdateFileName = model.StoredFileId != 0 ? model.Description : ""
            });

            var storedFile = NHibernateSession.Current.Get<StoredFile>(response.StoredFileId);
            var workPractice = NHibernateSession.Current.Get<WorkPractice>(model.WorkPracticeId);
            if (storedFile == null)
            {
                throw new WebServiceException($"Stored File is not found (Stored File ID {response.StoredFileId})");
            }
            if (workPractice == null)
            {
                throw new WebServiceException($"Work Practice is not found (Work Practice ID {model.WorkPracticeId})");
            }

            WorkPracticeAttachment domain;
            if (model.Id > 0)
            {
                domain = NHibernateSession.Current.Get<WorkPracticeAttachment>(model.Id);
                domain.StoredFile = storedFile;
                domain.Description = model.Description;
                domain.WorkPractice = workPractice;
            }
            else
            {
                domain = new WorkPracticeAttachment()
                {
                    StoredFile = storedFile,
                    WorkPractice = workPractice,
                    Description = model.Description
                };
            }
            NHibernateSession.Current.SaveOrUpdate(domain);
            NHibernateSession.Current.Flush();

            return new AttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public AttachmentResponse CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest model)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                Type = StoredFileType.ProfessionalDevelopmentActivity,
                StoragePath = model.StoragePath,
                UploadedByUserId = model.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = model.FilePath,
                TokenToRemoveFromFilename = model.TokenToRemoveFromFilename
            });

            var storedFile = NHibernateSession.Current.Get<StoredFile>(response.StoredFileId);
            var activity = NHibernateSession.Current.Get<ProfessionalDevelopmentActivity>(model.ProfessionalDevelopmentActivityId);
            if (storedFile == null)
            {
                throw new WebServiceException($"Stored File is not found (Stored File ID {response.StoredFileId})");
            }
            if (activity == null)
            {
                throw new WebServiceException($"Professional Development Activity is not found (Professional Development Activity ID {model.ProfessionalDevelopmentActivityId})");
            }

            ProfessionalDevelopmentActivityAttachment domain;
            if (model.Id > 0)
            {
                domain = NHibernateSession.Current.Get<ProfessionalDevelopmentActivityAttachment>(model.Id);
                domain.StoredFile = storedFile;
                domain.Description = model.Description;
                domain.ProfessionalDevelopmentActivity = activity;
            }
            else
            {
                domain = new ProfessionalDevelopmentActivityAttachment()
                {
                    StoredFile = storedFile,
                    Description = model.Description,
                    ProfessionalDevelopmentActivity = activity
                };

            }

            NHibernateSession.Current.SaveOrUpdate(domain);
            NHibernateSession.Current.Flush();

            return new AttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public ActivityPointsConfigurationResponse GetActivityPointsConfiguration()
        {
            var sections = NHibernateSession.Current.Query<ProfessionalDevelopmentSection>()
                .Select(MapPdSectionConfiguration).ToList();
            var requiredPointsPerYear = NHibernateSession.Current.Query<SystemValue>().First(x => x.ValueKey == "RecertificationTotalPointsPerYear");
            return new ActivityPointsConfigurationResponse { Sections = sections, RequiredPointsPerYear = Convert.ToDouble(requiredPointsPerYear.Value) };
        }

        private CategoryConfiguration MapPdCategoryConfiguration(ProfessionalDevelopmentSectionCategory sectionCategory)
        {
            var category = sectionCategory.ProfessionalDevelopmentCategory;

            return new CategoryConfiguration
            {
                CategoryId = category.Id,
                CategoryGroupId = category.ProfessionalDevelopmentCategoryGroup?.Id,
                PointsLimit = sectionCategory.PointsLimit,
                CategoryGroupRequiredPointsPerYear = category.ProfessionalDevelopmentCategoryGroup
                    ?.RequiredPointsPerYear,
                PointsLismitTypeId = sectionCategory.PdPointsLimitType?.Id,
                CategoryGroupName = category.ProfessionalDevelopmentCategoryGroup?.Name,
                CategoryName = category.Name,
                Requirements = category.CategoryRequirements.Select(MapPdCategoryRequiremment).ToList()
            };
        }
        private SectionPointsConfiguration MapPdSectionConfiguration(ProfessionalDevelopmentSection section)
        {
            return new SectionPointsConfiguration
            {
                SectionId = section.Id,
                SectionName = section.Name,
                Categories = section.SectionCategories.Select(MapPdCategoryConfiguration).ToList(),
                RequiredPointsPerYear = section.RequiredPointsPerYear
            };
        }

        private RequirementConfiguration MapPdCategoryRequiremment(ProfessionalDevelopmentCategoryRequirement categoryRequirement)
        {
            return new RequirementConfiguration
            {
                CategoryRequirementId = categoryRequirement.Id,
                RequirementId = categoryRequirement.ProfessionalDevelopmentRequirement.Id,
                Points = categoryRequirement.Points

            };
        }

    }
}
