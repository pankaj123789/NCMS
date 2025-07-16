using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.Extensions;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestSessionQueryService : ITestSessionQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestSessionQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public bool CheckCapacityTestSession(
            int credentialTypeId,
            int skillId,
            int testSessionId,
            int credentialStatusTypeId,
            int preferredTestLocationId)
        {
            var availabilityHelper = new TestSessionAvailabiltyQueryHelper();
            return availabilityHelper.IsAvailableTestSession(
                credentialTypeId,
                skillId,
                testSessionId,
                credentialStatusTypeId,
                preferredTestLocationId);
        }

        public GetTestSessionDetailsResponse GetTestSessionDetailsById(int testSessionId)
        {
            var totalAttendees = NHibernateSession.Current.Query<TestSitting>()
                .Count(s => s.TestSession.Id == testSessionId && s.Rejected == false);
            var testSessionDetails = NHibernateSession.Current.Get<TestSession>(testSessionId);
            var sessionQueryHelper = new TestSessionQueryHelper();
            var isRolePlayerAvailable = IsRolePlayerAvailable();
            var rolePlayersRequired = sessionQueryHelper.SearchTestSessions(
                new GetTestSessionSearchRequest()
                {
                    Filters = new[]
                    {
                        new TestSessionSearchCriteria
                        {
                            Filter = TestSessionFilterType.RolePlayersRequiredBoolean,
                            Values = new[] { true.ToString() }
                        },
                        new TestSessionSearchCriteria
                        {
                            Filter = TestSessionFilterType.CredentialIntList,
                            Values = new[] { testSessionDetails.CredentialType.Id.ToString() }
                        }
                    },
                    Take = 1
                }).Any();
            var response = new GetTestSessionDetailsResponse
            {
                Result = MapTestSessionDetailsDto(testSessionDetails, totalAttendees, rolePlayersRequired, isRolePlayerAvailable)
            };

            return response;
        }

        //public GetTestSessionsForTelevicDownloadResponse GetTestSessionsForTelevicDownload(GetTestSessionsForTelevicDownloadRequest request)
        //{
        //    TestSitting mTestSitting = null;
        //    TestSession mTestSession = null;
        //    TestLocation mTestLocation = null;
        //    Venue mVenue = null;
        //    CredentialRequest mCredentialRequest = null;
        //    CredentialApplication mCredentialApplication = null;
        //    Person mPerson = null;
        //    NaatiEntity mEntity = null;
        //    GetTestSessionsForTelevicDownloadDto dto = null;

        //    var validTestSitting = Restrictions.Eq(Projections.Property(() => mTestSitting.Rejected), false);
        //    var validTestSession = Restrictions.Between(Projections.Property(() => mTestSession.TestDateTime), request.StartDate, request.EndDate);
        //    var validTestLocation = Restrictions.Eq(Projections.Property(() => mTestLocation.IsAutomated), true);
        //    var validCredentialrequest = Restrictions.Eq(Projections.Property(() => mCredentialRequest.CredentialRequestStatusType.Id), (int)CredentialRequestStatusTypeName.TestSessionAccepted);

        //    var testSessionsForTelevicDownloadQuery = NHibernateSession.Current.QueryOver(() => mTestSitting)
        //        .Inner.JoinAlias(x => mTestSitting.TestSession, () => mTestSession, validTestSession)
        //        .Inner.JoinAlias(x => mTestSession.Venue, () => mVenue)
        //        .Inner.JoinAlias(x => mVenue.TestLocation, () => mTestLocation, validTestLocation)
        //        .Inner.JoinAlias(x => mTestSitting.CredentialRequest, () => mCredentialRequest, validCredentialrequest)
        //        .Inner.JoinAlias(x => mCredentialRequest.CredentialApplication, () => mCredentialApplication)
        //        .Inner.JoinAlias(x => mCredentialApplication.Person, () => mPerson)
        //        .Inner.JoinAlias(x => mPerson.Entity, () => mEntity)
        //        .Where(validTestSitting)
        //        .Select(Projections.ProjectionList()
        //        .Add(Projections.Property(() => mCredentialRequest.Id).WithAlias(() => dto.CredentialRequestId))
        //        .Add(Projections.Property(() => mCredentialApplication.Id).WithAlias(() => dto.CredentialApplicationId))
        //        .Add(Projections.Property(() => mTestSitting.Id).WithAlias(() => dto.TestSittingId))
        //        .Add(Projections.Property(() => mTestSession.Id).WithAlias(() => dto.TestSessionId))
        //        .Add(Projections.Property(() => mEntity.NaatiNumber).WithAlias(() => dto.NaatiNumber)))
        //        .TransformUsing(Transformers.AliasToBean<GetTestSessionsForTelevicDownloadDto>())
        //        .Skip(request.Page * request.PageSize)
        //        .Take(request.PageSize);

        //    var testSessionsForTelevicDownload = testSessionsForTelevicDownloadQuery.List<GetTestSessionsForTelevicDownloadDto>();

        //    var response = new GetTestSessionsForTelevicDownloadResponse
        //    {
        //        Result = testSessionsForTelevicDownload
        //    };

        //    return response;
        //}

        public GetTestSessionSkillsResponse GetSkillAttendeesCount(int testSessionId)
        {
            TestSittingCountDto dto = null;
            TestSitting mTestSitting = null;
            TestSession mTestSession = null;
            CredentialRequest mCredentialRequest = null;
            Skill mSkill = null;
            CredentialRequestStatusType mCredentialRequestStatusType = null;

            var validTestSitting = Restrictions.Eq(Projections.Property(() => mTestSitting.Rejected), false);
            var validTestSession = Restrictions.Eq(Projections.Property(() => mTestSession.Id), testSessionId);

            var projections = new[]
            {
                Projections.Group(() => mSkill.Id).WithAlias(() => dto.SkillId),
                Projections.Group(() => mCredentialRequestStatusType.Id)
                    .WithAlias(() => dto.CredentialRequestStatusTypeId),
                Projections.Count(() => mTestSitting.Id).WithAlias(() => dto.TotalSittings),
            };

            var testSittingsCountGroups = NHibernateSession.Current.QueryOver(() => mTestSitting)
                .Inner.JoinAlias(x => mTestSitting.TestSession, () => mTestSession, validTestSession)
                .Inner.JoinAlias(x => mTestSitting.CredentialRequest, () => mCredentialRequest, validTestSitting)
                .Inner.JoinAlias(x => mCredentialRequest.Skill, () => mSkill)
                .Inner.JoinAlias(
                    x => mCredentialRequest.CredentialRequestStatusType,
                    () => mCredentialRequestStatusType)
                .Select(projections).TransformUsing(Transformers.AliasToBean<TestSittingCountDto>())
                .List<TestSittingCountDto>();

            var sessionSkills = NHibernateSession.Current.Query<TestSessionSkill>()
                .Where(s => s.TestSession.Id == testSessionId && s.Skill.Id > 0).ToList()
                .Select(y => new { SkillId = y.Skill.Id, Capacity = y.Capacity, y.Skill.DisplayName }).ToList();

            var testSession = NHibernateSession.Current.Get<TestSession>(testSessionId);
            var sessionCapacity = (testSession.OverrideVenueCapacity
                ? testSession.Capacity.GetValueOrDefault()
                : testSession.Venue.Capacity);
            var list = new List<TestSessionSkillCountModel>();
            var testSessionsFreeSeats = sessionCapacity - testSittingsCountGroups.Sum(x => x.TotalSittings);

            foreach (var sessionSkill in sessionSkills)
            {
                var skillGroups = testSittingsCountGroups.Where(x => x.SkillId == sessionSkill.SkillId).ToList();
                var skillAttendees = skillGroups.Sum(x => x.TotalSittings);
                var capacity = sessionSkill.Capacity ?? sessionCapacity;
                var skillFreeSeats = (capacity - skillAttendees) < 0 ? 0 : (capacity - skillAttendees);

                list.Add(
                    new TestSessionSkillCountModel
                    {
                        SkillId = sessionSkill.SkillId,
                        DisplayName = sessionSkill.DisplayName,
                        Attendees = skillAttendees,
                        FreeSeats = testSessionsFreeSeats < 0 ? 0 : Math.Min(skillFreeSeats, testSessionsFreeSeats),
                        AwaitingPaymentCount = skillGroups.FirstOrDefault(
                                                       x => x.CredentialRequestStatusTypeId ==
                                                            (int)CredentialRequestStatusTypeName.AwaitingTestPayment)
                                                   ?.TotalSittings ?? 0,
                        ConfirmedCount = skillGroups.FirstOrDefault(
                                                 x => x.CredentialRequestStatusTypeId ==
                                                      (int)CredentialRequestStatusTypeName.TestSessionAccepted)
                                             ?.TotalSittings ?? 0,
                        CheckedInCount = skillGroups.FirstOrDefault(
                                             x => x.CredentialRequestStatusTypeId ==
                                                  (int)CredentialRequestStatusTypeName.CheckedIn)?.TotalSittings ?? 0,
                        SatCount = skillGroups.FirstOrDefault(
                                       x => x.CredentialRequestStatusTypeId ==
                                            (int)CredentialRequestStatusTypeName.TestSat)?.TotalSittings ?? 0,
                        ProcessingInvoiceCount = skillGroups.FirstOrDefault(
                                                         x => x.CredentialRequestStatusTypeId ==
                                                              (int)CredentialRequestStatusTypeName
                                                                  .ProcessingTestInvoice)
                                                     ?.TotalSittings ?? 0
                    });
            }

            var skillIds = sessionSkills.Select(x => x.SkillId).ToList();
            var otherSkillGroups = testSittingsCountGroups.Where(x => skillIds.All(s => s != x.SkillId))
                .GroupBy(g => g.SkillId).ToList();

            var otherSkillIds = otherSkillGroups.Select(x => x.Key).ToList();

            var skillDisplayName = NHibernateSession.Current.Query<Skill>().Where(x => otherSkillIds.Contains(x.Id))
                .ToList()
                .Select(
                    r => new
                    {
                        r.Id,
                        r.DisplayName
                    }).ToList().ToDictionary(x => x.Id, y => y.DisplayName);

            foreach (var otherSkillGroup in otherSkillGroups)
            {
                list.Add(
                    new TestSessionSkillCountModel
                    {
                        DisplayName = skillDisplayName[otherSkillGroup.Key],
                        Attendees = otherSkillGroup.Sum(x => x.TotalSittings),
                        FreeSeats = 0,
                        AwaitingPaymentCount = otherSkillGroup.FirstOrDefault(
                                                       x => x.CredentialRequestStatusTypeId ==
                                                            (int)CredentialRequestStatusTypeName.AwaitingTestPayment)
                                                   ?.TotalSittings ?? 0,
                        ConfirmedCount = otherSkillGroup.FirstOrDefault(
                                                 x => x.CredentialRequestStatusTypeId ==
                                                      (int)CredentialRequestStatusTypeName.TestSessionAccepted)
                                             ?.TotalSittings ?? 0,
                        CheckedInCount = otherSkillGroup.FirstOrDefault(
                                             x => x.CredentialRequestStatusTypeId ==
                                                  (int)CredentialRequestStatusTypeName.CheckedIn)?.TotalSittings ?? 0,
                        SatCount = otherSkillGroup.FirstOrDefault(
                                       x => x.CredentialRequestStatusTypeId ==
                                            (int)CredentialRequestStatusTypeName.TestSat)?.TotalSittings ?? 0,
                        ProcessingInvoiceCount = otherSkillGroup.FirstOrDefault(
                                                         x => x.CredentialRequestStatusTypeId ==
                                                              (int)CredentialRequestStatusTypeName
                                                                  .ProcessingTestInvoice)
                                                     ?.TotalSittings ?? 0
                    });
            }

            return new GetTestSessionSkillsResponse { Result = list };
        }

        // todo: move to business layer, pass dto to update method
        public void MarkAsCompleteTestSession(int testSessionId)
        {
            var testSession = NHibernateSession.Current.Get<TestSession>(testSessionId);

            testSession.Completed = true;

            //  ensure we keep a record of the capacity for reporting
            if (testSession.Capacity == null)
            {
                testSession.Capacity = testSession.Venue.Capacity;
            }

            NHibernateSession.Current.SaveOrUpdate(testSession);
            NHibernateSession.Current.Flush();
        }

        // todo: move to business layer, pass dto to update method
        public void ReopenTestSession(int testSessionId)
        {
            var testSession = NHibernateSession.Current.Get<TestSession>(testSessionId);
            testSession.Completed = false;
            NHibernateSession.Current.SaveOrUpdate(testSession);
            NHibernateSession.Current.Flush();
        }

        public GetTestSessionResponse GetTestSessionById(int testSessionId)
        {
            var testSession = NHibernateSession.Current.Get<TestSession>(testSessionId);

            var response = new GetTestSessionResponse
            {
                Result = MapTestSessionDto(testSession)
            };
            return response;
        }

        public GetTestSessionApplicantsResponse GetApplicantsById(int testSessionId, bool includeRejected)
        {
            var query = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => x.TestSession.Id == testSessionId);

            if (!includeRejected)
            {
                query = query.Where(x => !x.Rejected);
            }

            var result = query.ToList();

            var testSessionApplicants = result.Select(
                x => new TestSessionApplicantsDto
                {
                    TestSittingId = x.Id,
                    ApplicationId = x.CredentialRequest.CredentialApplication.Id,
                    ApplicationReference = "APP" + x.CredentialRequest.CredentialApplication.Id,
                    Name = x.CredentialRequest.CredentialApplication.Person.FullName,
                    CustomerNo = x.CredentialRequest.CredentialApplication.Person.NaatiNumberDisplay,
                    Gender = x.CredentialRequest.CredentialApplication.Person.Gender,
                    DateOfBirth = x.CredentialRequest.CredentialApplication.Person.BirthDate,
                    Email = x.CredentialRequest.CredentialApplication.Person.PrimaryEmailAddress ?? string.Empty,
                    Phone = x.CredentialRequest.CredentialApplication.Person.PrimaryContactNumber ?? string.Empty,
                    Skill = x.CredentialRequest.Skill.DisplayName,
                    SkillId = x.CredentialRequest.Skill.Id,
                    Status = x.CredentialRequest.CredentialRequestStatusType.Id,
                    StatusModifiedDate = x.CredentialRequest.StatusChangeDate,
                    CredentialRequestId = x.CredentialRequest.Id,
                    ApplicationStatusId = x.CredentialRequest.CredentialApplication.CredentialApplicationType.Id,
                    CredentialTypeId = x.CredentialRequest.CredentialType.Id,
                    ApplicationTypeId = x.CredentialRequest.CredentialApplication.CredentialApplicationType.Id,
                    Rejected = x.Rejected,
                    AttendanceId = !x.Rejected ? x.Id.ToString() : "",
                    TestTasks = GetTestTasksByAttendanceId(x),
                    Supplementary = x.Supplementary,
                    Sat = x.Sat,
                    StatusDisplayName = x.CredentialRequest.CredentialRequestStatusType.DisplayName,
                    SupplementaryCredentialRequest = x.CredentialRequest.Supplementary,
                    HasDefaultSpecification = x.HasDefaultSpecification(),
                    HasTestMaterials = x.TestSittingTestMaterials.Any(),
                    LanguageCharacterType = GetLanguageType(x)
                });

            return new GetTestSessionApplicantsResponse { Results = testSessionApplicants };
        }

        public bool AllTestSittingsAreSat(int testSessionId)
        {
            var notSit = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => x.TestSession.Id == testSessionId).Any(x => !x.Sat && !x.Rejected);

            return !notSit;
        }

        public string AllTestSittingsMarkCompleteRequirements(int testSessionId)
        {
            var returnValues = string.Empty;

            var notSit = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => x.TestSession.Id == testSessionId).Any(x => !x.Sat && !x.Rejected);

            if (notSit)
            {
                // Todo REFACTOR THIS!!!
                returnValues = "NotSit";
                return returnValues;
            }

            var testSittingIds = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => x.TestSession.Id == testSessionId && x.Sat && !x.Rejected);
            foreach (var testSitting in testSittingIds)
            {
                var hasTasksWithNoAssingedMaterials =
                    GetTestTasksByAttendanceId(testSitting).Any(x => !x.TestMaterialId.HasValue);
                if (hasTasksWithNoAssingedMaterials)
                {
                    // Todo REFACTOR THIS!!!
                    returnValues = "TestMaterialNotAssigned";
                    return returnValues;
                }

            }

            return returnValues;
        }

        internal IEnumerable<TestTaskDto> GetTestTasksByAttendanceId(TestSitting testSitting)
        {
            var testTasks = new List<TestTaskDto>();
            IEnumerable<RubricTestComponentResult> rubricTestComponentResults = null;

            if (testSitting.Supplementary)
            {
                var originalTestSitting = testSitting.CredentialRequest.TestSittings.Where(x => !x.Rejected)
                    .OrderBy(x => x.Id).FirstOrDefault();
                rubricTestComponentResults = originalTestSitting?.TestResults.OrderByDescending(x => x.Id)
                    .FirstOrDefault()?.RubricTestComponentResults;
            }

            foreach (var c in testSitting.TestSpecification.TestComponents)
            {
                if (testSitting.Supplementary)
                {
                    var component = rubricTestComponentResults.FirstOrDefault(tc => tc.TestComponent.Id == c.Id);
                    if ((component?.MarkingResultType.Id ?? 0) == (int)MarkingResultTypeName.EligableForSupplementary)
                    {
                        continue;
                    }
                }

                var testMaterials = testSitting.TestSittingTestMaterials.Where(t => t.TestComponent.Id == c.Id);

                if (!testMaterials.Any())
                {
                    testTasks.Add(
                        new TestTaskDto
                        {
                            TestComponentId = c.Id,
                            TestComponentName = c.ComponentNumber.ToString(),
                            Label = c.Label,
                            TestComponentTypeLabel = c.Type.Label,
                            TaskComponentTypeId = c.Type.Id
                        });
                }
                else
                {
                    foreach (var m in testMaterials)
                    {
                        testTasks.Add(
                            new TestTaskDto
                            {
                                TestComponentId = c.Id,
                                TestComponentName = c.ComponentNumber.ToString(),
                                Label = c.Label,
                                TestComponentTypeLabel = c.Type.Label,
                                TestMaterialId = m.TestMaterial.Id,
                                TestMaterialDomainId = m.TestMaterial.TestMaterialDomain.Id,
                                HasTestMaterialAttachment = m.TestMaterial.TestMaterialAttachments.Any(),
                                MaterialComponentTypeId = m.TestMaterial.TestComponentType.Id,
                                TaskComponentTypeId = c.Type.Id
                            });
                    }
                }
            }

            return testTasks.OrderBy(c => c.TestMaterialId).OrderBy(c => c.TestComponentTypeLabel)
                .OrderBy(c => c.TestComponentName);
        }

        public void UpdateTestSession(UpdateTestSessionRequest request)
        {
            var testSession = NHibernateSession.Current.Get<TestSession>(request.TestSessionId);

            var venue = NHibernateSession.Current.Get<Venue>(request.VenueId);

            if (testSession == null)
            {
                throw new WebServiceException($"Test session not found (Test Session ID {request.TestSessionId})");
            }

            _autoMapperHelper.Mapper.Map(request, testSession);

            testSession.Venue = venue;

            NHibernateSession.Current.Save(testSession);
            NHibernateSession.Current.Flush();
        }

        public void UpdateTestSitting(UpdateTestSessionRequest request)
        {
            var testSession = NHibernateSession.Current.Get<TestSession>(request.TestSessionId);

            var venue = NHibernateSession.Current.Get<Venue>(request.VenueId);

            if (testSession == null)
            {
                throw new WebServiceException($"Test session not found (Test Session ID {request.TestSessionId})");
            }

            _autoMapperHelper.Mapper.Map(request, testSession);

            testSession.Venue = venue;

            NHibernateSession.Current.Save(testSession);
            NHibernateSession.Current.Flush();
        }

        public void UpdateTestSitting(UpdateTestSittingRequest request)
        {
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(request.CredentialRequestId);
            var existingTestSitting = credentialRequest.TestSittings.Where(x => x.TestSession.Id == request.TestSessionId).FirstOrDefault();

            if (existingTestSitting != null)
            {
                existingTestSitting.Rejected = request.IsRejected;
                credentialRequest.CredentialRequestStatusType = NHibernateSession.Current.Get<CredentialRequestStatusType>(request.CredentialRequestStatusTypeId);
            }
            else
            {
                var testSession = NHibernateSession.Current.Get<TestSession>(request.TestSessionId);
                //add new test sitting to the session
                TestSitting testSitting = new TestSitting
                {
                    CredentialRequest = credentialRequest, 
                    TestSession = testSession, 
                    Rejected = request.IsRejected, 
                    AllocatedDate = request.AllocatedDate,
                    Supplementary = request.Supplementary, 
                    TestSpecification = NHibernateSession.Current.Get<TestSpecification>(request.TestSpecificationId)
                };

                credentialRequest.CredentialRequestStatusType = NHibernateSession.Current.Get<CredentialRequestStatusType>(request.CredentialRequestStatusTypeId);

                NHibernateSession.Current.Save(testSitting);
            }

            NHibernateSession.Current.Save(credentialRequest);
            NHibernateSession.Current.Flush();
        }

        public GetTestSessionsResponse GetActiveTestSessions(GetActiveTestSessionRequest request)
        {
            var result =
                NHibernateSession.Current.Query<TestSession>()
                    .Where(
                        ts => !ts.Completed && ts.CredentialType.Id == request.CredentialTypeId
                                            && ts.Venue.TestLocation.Id == request.TestLocationId
                                            && (ts.TestDateTime > DateTime.Now || ts.CredentialType.AllowBackdating))
                    .ToList().Select(
                        ts =>

                            new TestSessionBasicDto
                            {
                                TestSessionId = ts.Id,
                                Name = ts.Name,
                                TestDate = ts.TestDateTime,
                                ArrivalTime = ts.ArrivalTime,
                                Duration = ts.Duration,
                                VenueId = ts.Venue.Id,
                                VenueName = ts.Venue.Name,
                                VenueAddress = ts.Venue.Address,
                                VenueCapacity = ts.Venue.Capacity,
                                CredentialTypeId = ts.CredentialType.Id,
                                PublicNotes = ts.PublicNote,
                                Completed = ts.Completed,
                                RehearsalDateTime = ts.RehearsalDateTime,
                                RehearsalNotes = ts.RehearsalNotes,
                                AllowSelfAssign = ts.AllowSelfAssign,
                                OverrideCapacity = ts.OverrideVenueCapacity,
                                OverridenCapacity = ts.Capacity,
                                NewCandidatesOnly = ts.NewCandidatesOnly.HasValue ? ts.NewCandidatesOnly.Value : false,
                                Skills = NHibernateSession.Current.Query<TestSessionSkill>()
                                    .Where(x => x.TestSession.Id == ts.Id)
                                    .Select(
                                        x => new SkillDto()
                                        {
                                            Language1Name = x.Skill.Language1.Name,
                                            Language2Name = x.Skill.Language2.Name,
                                            DirectionDisplayName = x.Skill.DirectionType.DisplayName
                                        }).ToList(),
                                DefaultTestSpecificationId = ts.DefaultTestSpecification.Id,
                                MarkingSchemaTypeId = (int)ts.DefaultTestSpecification.MarkingSchemaType(),
                                TotalApplicants = NHibernateSession.Current.Query<TestSitting>()
                                    .Count(x => x.TestSession.Id == ts.Id && !x.Rejected)

                            }).ToList();

            return new GetTestSessionsResponse { Result = result };
        }

        public SetAllowAvailabilityNoticeResponse DisableAllowAvailabilityNotice(HashSet<int> ids)
        {
            var sessions = NHibernateSession.Current.Query<TestSession>().Where(x => ids.Contains(x.Id));

            foreach (var session in sessions)
            {
                session.AllowAvailabilityNotice = false;

                NHibernateSession.Current.Update(session);
            }

            return new SetAllowAvailabilityNoticeResponse();
        }

        //public void MarkAsSynced(int testSessionId)
        //{
        //    var testSession = NHibernateSession.Current.Get<TestSession>(testSessionId);

        //    testSession.Synced = true;

        //    NHibernateSession.Current.Update(testSession);
        //    NHibernateSession.Current.Flush();
        //}

        public CreateOrUpdateTestSessionResponse CreateOrUpdateTestSession(CreateOrUpdateTestSessionRequest request)
        {
            var testSession = NHibernateSession.Current.Get<TestSession>(request.Id) ?? new TestSession();
            _autoMapperHelper.Mapper.Map(request, testSession);

            testSession.CredentialType = NHibernateSession.Current.Load<CredentialType>(request.CredentialTypeId);
            testSession.Venue = NHibernateSession.Current.Load<Venue>(request.VenueId);
            testSession.Capacity = request.OverrideVenueCapacity ? (int?)request.Capacity : null;
            testSession.NewCandidatesOnly = request.NewCandidatesOnly;
            testSession.IsActive = request.IsActive;
            testSession.DefaultTestSpecification =
                NHibernateSession.Current.Load<TestSpecification>(request.DefaultTestSpecificationId);

            var testSessionSkills = new List<TestSessionSkill>();
            foreach (var sessionSkill in request.Skills)
            {
                var item = NHibernateSession.Current.Get<TestSessionSkill>(sessionSkill.Id) ?? new TestSessionSkill();
                item.Skill = NHibernateSession.Current.Get<Skill>(sessionSkill.SkillId);
                item.TestSession = testSession;
                _autoMapperHelper.Mapper.Map(sessionSkill, item);
                testSessionSkills.Add(item);
            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.SaveOrUpdate(testSession);

                NHibernateSession.Current.DeleteList(testSession.TestSessionSkills?.Where(ts => !testSessionSkills.Any(t => t.Skill.Id == ts.Skill.Id)).ToList());

                foreach(var testSessionSkill in testSessionSkills)
                {
                    NHibernateSession.Current.SaveOrUpdate(testSessionSkill);
                }

                transaction.Commit();
            }

            return new CreateOrUpdateTestSessionResponse { TestSessionId = testSession.Id };
        }

        public TestSessionSearchResponse Search(GetTestSessionSearchRequest request)
        {
            var queryHelper = new TestSessionQueryHelper();
            var result = queryHelper.SearchTestSessions(request);

            return new TestSessionSearchResponse
            {
                TestSessions = result
            };
        }

        public TestSessionSkillResponse GetTestSessionSkillsForTestSession(int testSessionId)
        {
            var response = NHibernateSession.Current.Query<TestSessionSkill>()
                .Where(x => x.TestSession.Id == testSessionId);

            var results = response.Select(_autoMapperHelper.Mapper.Map<TestSessionSkillDto>);

            return new TestSessionSkillResponse
            {
                Results = results
            };
        }

        public AvailableTestSessionsResponse ApiGetTestSessionAvailability(ApiSessionAvailabilityRequest request)
        {
            var queryHelper = new TestSessionAvailabiltyQueryHelper();

            var availableTestSessions = queryHelper.GetAllAvailableTestSessions(
                request.SkillId,
                request.CredentialTypeId,
                request.PreferredTestLocationId,
                false,
                0,
                request.FromTestDate,
                request.ToTestDate);
            var testSittingBacklog = request.IncludeBacklog
                ? queryHelper.GetTestSittingBacklog(request.SkillId, 0)
                : new List<TestSittingBacklog>();

            return GetAvailableTestSessionAfterBacklogAssignment(availableTestSessions, testSittingBacklog);
        }

        private AvailableTestSessionsResponse GetAvailableTestSessionAfterBacklogAssignment(
            IList<AvailableTestSessionDto> availableTestSessions,
            IList<TestSittingBacklog> testSittingBacklog)
        {
            var backlogByTestLocation = testSittingBacklog.ToDictionary(x => x.TestLocationId, y => y.Backlog);

            var availableSessions = new List<AvailableTestSessionDto>();
            foreach (var availableTestSession in availableTestSessions.OrderBy(x => x.TestDateTime))
            {
                int backLog;
                if (backlogByTestLocation.TryGetValue(availableTestSession.TestLocationId, out backLog))
                {
                    availableTestSession.AvailableSeats = availableTestSession.AvailableSeats - backLog;
                    if (availableTestSession.AvailableSeats < 0)
                    {
                        backLog = Math.Abs(availableTestSession.AvailableSeats);
                    }
                    else
                    {
                        backLog = 0;
                    }

                    backlogByTestLocation[availableTestSession.TestLocationId] = backLog;
                }

                if (availableTestSession.AvailableSeats > 0)
                {
                    availableSessions.Add(availableTestSession);
                }
            }

            return new AvailableTestSessionsResponse() { AvailableTestSessions = availableSessions };
        }

        public AvailableTestSessionsResponse GetAvailableTestSessionAfterBacklogAssignment(int credentialRequestId)
        {
            var queryHelper = new TestSessionAvailabiltyQueryHelper();

            var availableTestSessions = queryHelper.GetAllAvailableTestSessions(credentialRequestId);

            return new AvailableTestSessionsResponse() { AvailableTestSessions = availableTestSessions };
        }

        //See TestSpecImporter for a debuggable version of this.
        private static string GetLanguageType(TestSitting testSitting)
        {
            var languageTypes = new[] { "Traditional", "Simplified" };

            var result = testSitting.CredentialRequest.CredentialApplication.CredentialApplicationFieldsData
                .Select(x => x.CredentialApplication.CredentialApplicationFieldsData).Distinct()
                .SelectMany(x => x.Where(y => y.CredentialApplicationFieldOptionOption != null && languageTypes.Contains(y.CredentialApplicationFieldOptionOption.CredentialApplicationFieldOption.Name)))
                .FirstOrDefault();

            return result == null ? "" : result.CredentialApplicationFieldOptionOption.CredentialApplicationFieldOption.DisplayName;
        }

        private Func<TestSessionSkill, bool> GetSkillFilter(IEnumerable<TestSessionSkillFilter> skillFilters)
        {
            return testSessionSkill =>
            {
                foreach (var skillFilter in skillFilters)
                {
                    if (testSessionSkill.Skill.Id == skillFilter.SkillId &&
                        testSessionSkill.TestSession.CredentialType.Id == skillFilter.CredentialTypeId)
                    {
                        return true;
                    }
                }

                return false;
            };
        }

        private TestSessionDto MapTestSessionDto(TestSession session)
        {
            var hasRolePlayers = session.TestSessionRolePlayers.Any();
            var dto = new TestSessionDto
            {
                TestSessionId = session.Id,
                Name = session.Name,
                TestDate = session.TestDateTime,
                ArrivalTime = session.ArrivalTime,
                Duration = session.Duration,
                VenueId = session.Venue.Id,
                VenueName = session.Venue.Name,
                VenueAddress = session.Venue.Address,
                VenueCapacity = session.Venue.Capacity,
                OverrideCapacity = session.OverrideVenueCapacity,
                OverridenCapacity = session.Capacity,
                NewCandidatesOnly = session.NewCandidatesOnly.HasValue ? session.NewCandidatesOnly.Value : false,
                CredentialTypeId = session.CredentialType.Id,
                PublicNotes = session.PublicNote,
                Completed = session.Completed,
                AllowSelfAssign = session.AllowSelfAssign,
                RehearsalDateTime = session.RehearsalDateTime,
                RehearsalNotes = session.RehearsalNotes,
                HasRolePlayers = hasRolePlayers,
                Skills = session.TestSessionSkills.Select(
                    x => new SkillDto()
                    {
                        Id = x.Skill.Id,
                        Language1Name = x.Skill.Language1.Name,
                        Language2Name = x.Skill.Language2.Name,
                        DirectionDisplayName = x.Skill.DirectionType.DisplayName,
                        DisplayName = x.Skill.DisplayName
                    }).ToList(),
                DefaultTestSpecificationId = session.DefaultTestSpecification.Id,
                MarkingSchemaTypeId = (int)session.DefaultTestSpecification.MarkingSchemaType()
            };

            dto.TestSessionApplicants = GetTestSessionApplicants(dto.TestSessionId);
            return dto;
        }

        private TestSessionDetailsDto MapTestSessionDetailsDto(
            TestSession testSessionDetails,
            int totalAttendees,
            bool rolePlayersRequired,
            bool isRolePlayerAvailable)
        {
            return new TestSessionDetailsDto
            {
                Id = testSessionDetails.Id,
                Name = testSessionDetails.Name,
                TestDate = testSessionDetails.TestDateTime,
                CredentialType = testSessionDetails.CredentialType.ExternalName,
                CredentialTypeInternalName = testSessionDetails.CredentialType.InternalName,
                ArrivalTime = testSessionDetails.ArrivalTime,
                Duration = testSessionDetails.Duration,
                VenueName = testSessionDetails.Venue.Name,
                Capacity = testSessionDetails.Capacity ?? testSessionDetails.Venue.Capacity,
                Attendees = totalAttendees,
                Completed = testSessionDetails.Completed,
                VenueAddress = testSessionDetails.Venue.Address,
                TestLocationId = testSessionDetails.Venue.TestLocation.Id,
                VenueId = testSessionDetails.Venue.Id,
                CredentialTypeId = testSessionDetails.CredentialType.Id,
                AllowSelfAssign = testSessionDetails.AllowSelfAssign,
                PublicNote = testSessionDetails.PublicNote,
                TestSessionApplicants = GetTestSessionApplicants(testSessionDetails.Id),
                OverrideVenueCapacity = testSessionDetails.OverrideVenueCapacity,
                NewCandidatesOnly = testSessionDetails.NewCandidatesOnly.HasValue ? testSessionDetails.NewCandidatesOnly.Value : false,
                RehearsalDateTime = testSessionDetails.RehearsalDateTime,
                RehearsalNotes = testSessionDetails.RehearsalNotes,
                HasRolePLayers = testSessionDetails.TestSessionRolePlayers.Any() && isRolePlayerAvailable,
                RolePlayersRequired = rolePlayersRequired && isRolePlayerAvailable,
                DefaultTestSpecificationId = testSessionDetails.DefaultTestSpecification.Id,
                TestSpecificationDescription = testSessionDetails.DefaultTestSpecification.Description,
                IsActiveTestSpecification = testSessionDetails.DefaultTestSpecification.Active,
                IsActive = testSessionDetails.IsActive,
            };
        }

        private IList<TestSessionApplicantDto> GetTestSessionApplicants(int testSessionId)
        {
            var query = from credentialRequest in NHibernateSession.Current.Query<CredentialRequest>()
                        join testSitting in NHibernateSession.Current.Query<TestSitting>() on credentialRequest.Id equals
                            testSitting.CredentialRequest.Id
                        join testSession in NHibernateSession.Current.Query<TestSession>() on testSitting.TestSession.Id equals
                            testSession.Id
                        join credentialApplication in NHibernateSession.Current.Query<CredentialApplication>() on
                            credentialRequest.CredentialApplication.Id equals credentialApplication.Id
                        join person in NHibernateSession.Current.Query<Person>() on credentialApplication.Person.Id equals
                            person.Id
                        where testSession.Id == testSessionId
                        select new TestSessionApplicantDto
                        {
                            CredentialRequestId = credentialRequest.Id,
                            ApplicationId = credentialRequest.CredentialApplication.Id,
                            CustomerNo = person.Entity.NaatiNumber,
                            PersonId = person.Id,
                            Name = $"{person.GivenName} {person.Surname}",
                            ApplicationReference = credentialApplication.Reference,
                            Status = credentialRequest.CredentialRequestStatusType.DisplayName,
                            StatusId = credentialRequest.CredentialRequestStatusType.Id,
                            ApplicationSubmittedDate = credentialApplication.EnteredDate,
                            Rejected = testSitting.Rejected,
                            StatusModifiedDate = credentialRequest.StatusChangeDate,
                            Sat = testSitting.Sat,
                            Language1 = credentialRequest.Skill.Language1.Name,
                            Language2 = credentialRequest.Skill.Language2.Name,
                            DirectionDisplayName = credentialRequest.Skill.DirectionType.DisplayName
                        };

            return query.ToList();
        }


        public GetTestSessionRolePlayerResponse GetTestSessionRolePlayers(GetTestSessionRolePlayerRequest request)
        {
            var queryHelper = new TestSessionRolePlayerQueryHelper();
            return new GetTestSessionRolePlayerResponse
            {
                RolePlayers = queryHelper.GetTestSessionRolePlayers(request)
            };

        }

        public GetRolePlayerResponse GetAvailableRolePlayers(GetRolePlayerRequest request)
        {

            var queryHelper = new TestSessionRolePlayerQueryHelper();

            var rolePlayers = queryHelper.GetAvailableRolePlayers(request);

            return new GetRolePlayerResponse
            {
                RolePlayers = rolePlayers
            };
        }

        public IEnumerable<TestSessionSpecificationDetailsDto> GetTestSpecificationDetails(
            TestSpecificationDetailsRequest request)
        {
            var queryHelper = new TestSessionRolePlayerQueryHelper();
            return queryHelper.GetTestSpecificationWithPendingRolePlayers(request.TestSessionIds.FirstOrDefault());
        }

        public IEnumerable<TestSessionSkillDetailsDto> GetTestSessionLanguageDetails(
            TestSessionLanguageDetailsRequest request)
        {
            var queryHelper = new TestSessionRolePlayerQueryHelper();
            return queryHelper.GetTestSkillsWithPendingRolePlayers(
                request.TestSessionIds.FirstOrDefault(),
                request.TestSpecificationId);
        }

        public SkillDetailsDto GetSkillDetails(int skillId)
        {
            var result = NHibernateSession.Current.Get<Skill>(skillId);

            return new SkillDetailsDto()
            {
                SkillId = result.Id,
                Language1Id = result.Language1.Id,
                Language2Id = result.Language2.Id,
                Language1DisplayName = result.Language1.Name,
                Language2DisplayName = result.Language2.Name,
                SkillDisplayName = result.DisplayName
            };
        }

        public UpsertRolePlayerResponse UpsertTestSessionRolePlayer(UpsertSessionRolePlayerRequest request)
        {
            var dataToUpsert = new List<object>();
            var dataToDelete = new List<object>();

            var rolePlayerData = GetTestSessionRolePlayerToUpsert(request);
            dataToUpsert.AddRange(rolePlayerData.dataToUpsert);
            dataToDelete.AddRange(rolePlayerData.dataToDelete);

            var personNoteData = GetPersonNoteDataToUsert(request);
            dataToUpsert.AddRange(personNoteData.dataToUpsert);
            dataToDelete.AddRange(personNoteData.dataToDelete);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                dataToDelete.Reverse();
                dataToDelete.ForEach(x => NHibernateSession.Current.Delete(x));
                dataToUpsert.ForEach(x => NHibernateSession.Current.SaveOrUpdate(x));

                transaction.Commit();
            }

            return new UpsertRolePlayerResponse
            {
                TestSessionRolePlayerId = dataToUpsert.OfType<TestSessionRolePlayer>().FirstOrDefault()?.Id ?? 0
            };
        }

        private TestSessionRolePlayerDto MapTestSessionRolePlayer(TestSessionRolePlayer result)
        {
            var details = new List<TestSessionRolePlayerDetailDto>();
            var testSessionRolePlayer = new TestSessionRolePlayerDto
            {
                Attended = result.Attended,
                Rejected = result.Rejected,
                Rehearsed = result.Rehearsed,
                StatusChangeUserId = result.StatusChangeUser.Id,
                TestSessionId = result.TestSession.Id,
                TestSessionRolePlayerId = result.Id,
                RolePlayerStatusId = result.RolePlayerStatusType.Id,
                RolePlayerId = result.RolePlayer.Id,
                StatusChangeDate = result.StatusChangeDate,
                NaatiNumber = result.RolePlayer.Person.Entity.NaatiNumber,
                Details = details,
                RehearsalDate = result.TestSession.RehearsalDateTime,
                TestSessionDate = result.TestSession.TestDateTime,
                TestSessionName = result.TestSession.Name,
                TestSessionLocation = result.TestSession.Venue.TestLocation.Name,
                VenueName = result.TestSession.Venue.Name,
                RolePlayerStatus = result.RolePlayerStatusType.Name,
                RolePlayerStatusDisplayName = result.RolePlayerStatusType.DisplayName,
                CredentialTypeExternalName = result.TestSession.CredentialType.ExternalName,
                RehearsalNotes = result.TestSession.RehearsalNotes,
                PublicNote = result.TestSession.PublicNote,
                VenueAddress = result.TestSession.Venue.Address,
                VenueCoordinates = result.TestSession.Venue.Coordinates
            };

            foreach (var detail in result.Details)
            {
                var sessionRolePlayerDetail = new TestSessionRolePlayerDetailDto
                {
                    TestSessionRolePlayerDetailId = detail.Id,
                    SkillId = detail.Skill.Id,
                    LanguageId = detail.Language.Id,
                    RolePlayerRoleTypeId = detail.RolePlayerRoleType.Id,
                    TestComponentId = detail.TestComponent.Id,
                    RolePlayerRoleTypeName = detail.RolePlayerRoleType.DisplayName,
                    SkillName = detail.Language.Name,
                    TestComponentName = detail.TestComponent.Name,
                    TaskLabel = detail.TestComponent.Label,
                    TaskTypeLabel = detail.TestComponent.Type.Label

                };

                details.Add(sessionRolePlayerDetail);
            }

            return testSessionRolePlayer;
        }

        public ServiceResponse<IEnumerable<TestSessionRolePlayerDto>> GetTestSessionRolePlayerBySessionId(
            int testSessionId)
        {
            var dtos = new List<TestSessionRolePlayerDto>();
            var rolePlayers = NHibernateSession.Current.Query<TestSessionRolePlayer>()
                .Where(x => x.TestSession.Id == testSessionId).ToList();

            foreach (var rolePlayer in rolePlayers)
            {
                var dto = MapTestSessionRolePlayer(rolePlayer);
                dtos.Add(dto);
            }

            return new ServiceResponse<IEnumerable<TestSessionRolePlayerDto>> { Data = dtos };
        }

        public ServiceResponse<TestSessionRolePlayerDto> GetTestSessionRolePlayer(int testSessionRolePlayerId)
        {
            var result = NHibernateSession.Current.Load<TestSessionRolePlayer>(testSessionRolePlayerId);

            var dto = MapTestSessionRolePlayer(result);

            return new ServiceResponse<TestSessionRolePlayerDto>() { Data = dto };
        }

        public ServiceResponse<IEnumerable<TestSessionRolePlayerDto>> GetSessionRolePlayers(
            GetRolePlaySessionRequest request)
        {
            var query = NHibernateSession.Current.Query<TestSessionRolePlayer>()
                .Where(x => x.RolePlayer.Person.Entity.NaatiNumber == request.NaatiNumber);
            if (request.TestSessionRolePlayerId.HasValue)
            {
                query = query.Where(x => x.Id == request.TestSessionRolePlayerId.GetValueOrDefault());
            }

            if (!request.IncludeRejected)
            {
                query = query.Where(x => !x.Rejected);
            }

            var result = query.ToList();
            var dtos = result.Select(MapTestSessionRolePlayer).ToList();

            return new ServiceResponse<IEnumerable<TestSessionRolePlayerDto>>() { Data = dtos };
        }

        public PersonTestSessionRolePlaysResponse GetPersonRolePlays(int naatiNumber)
        {
            var dtos = new List<TestSessionRolePlayerDto>();
            var rolePlays = NHibernateSession.Current.Query<TestSessionRolePlayer>()
                .Where(x => x.RolePlayer.Person.Entity.NaatiNumber == naatiNumber).ToList();

            foreach (var rolePlay in rolePlays)
            {
                var dto = MapTestSessionRolePlayer(rolePlay);
                dtos.Add(dto);
            }

            return new PersonTestSessionRolePlaysResponse { Data = dtos };
        }

        public IEnumerable<TestSessionAvailabilityObject> GetTestSessionAvailabilityObjects()
        {

            TestSession mTestSession = null;
            TestSessionSkill mTestSessionSkill = null;
            Skill mSkill = null;
            Venue mVenue = null;
            CredentialType mCrednetialType = null;
            CredentialRequest mCredentialRequest = null;
            CredentialApplication mCredentialApplication = null;
            CredentialRequestStatusType mCredentialRequestStatusType = null;
            var validStatuses = new[] { (int)CredentialRequestStatusTypeName.EligibleForTesting, (int)CredentialRequestStatusTypeName.TestAccepted };
            TestSessionAvailabilityObject dto = null;

            var projectionsList = Projections.ProjectionList()
                .Add(Projections.Property(() => mCredentialRequest.Id).WithAlias(() => dto.CredentialRequestId))
                .Add(Projections.Property(() => mTestSession.Id).WithAlias(() => dto.TestSessionId))
                .Add(Projections.Property(() => mCredentialApplication.Id).WithAlias(() => dto.ApplicationId));

            var whereFilter = Restrictions.Conjunction()
                .Add(Restrictions.In(Projections.Property(() => mCredentialRequestStatusType.Id), validStatuses))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSession.AllowAvailabilityNotice), true))
                .Add(Restrictions.Eq(Projections.Property(() => mCrednetialType.AllowAvailabilityNotice), true))
                .Add(Restrictions.EqProperty(Projections.Property(() => mVenue.TestLocation), Projections.Property(() => mCredentialApplication.PreferredTestLocation)))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSession.IsActive), true));

            var results = NHibernateSession.Current.QueryOver(() => mTestSession)
                .Inner.JoinAlias(x => mTestSession.Venue, () => mVenue)
                .Inner.JoinAlias(x => mTestSession.TestSessionSkills, () => mTestSessionSkill)
                .Inner.JoinAlias(x => mTestSessionSkill.Skill, () => mSkill)
                .Inner.JoinAlias(x => mSkill.CredentialRequests, () => mCredentialRequest)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialType, () => mCrednetialType)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialApplication, () => mCredentialApplication)
                .Inner.JoinAlias(x => mCredentialRequest.CredentialRequestStatusType, () => mCredentialRequestStatusType)
                .Where(whereFilter)
                .Select(projectionsList).TransformUsing(Transformers.AliasToBean<TestSessionAvailabilityObject>())
                .List<TestSessionAvailabilityObject>();

            return results;
        }

        private (IList<object> dataToDelete, IList<object> dataToUpsert) GetTestSessionRolePlayerToUpsert(
            UpsertSessionRolePlayerRequest request)
        {
            var dataToUpsert = new List<object>();
            var dataToDelete = new List<object>();

            var tesSessionRolePlayer =
                NHibernateSession.Current.Get<TestSessionRolePlayer>(
                    request.TestSessionRolePlayer
                        .TestSessionRolePlayerId) ??
                new TestSessionRolePlayer()
                {
                    RolePlayer = NHibernateSession.Current.Load<RolePlayer>(request.TestSessionRolePlayer.RolePlayerId),
                    TestSession =
                        NHibernateSession.Current.Load<TestSession>(request.TestSessionRolePlayer.TestSessionId),
                };

            tesSessionRolePlayer.Attended = request.TestSessionRolePlayer.Attended;
            tesSessionRolePlayer.Rehearsed = request.TestSessionRolePlayer.Rehearsed;
            tesSessionRolePlayer.Rejected = request.TestSessionRolePlayer.Rejected;
            tesSessionRolePlayer.RolePlayerStatusType =
                NHibernateSession.Current.Load<RolePlayerStatusType>(request.TestSessionRolePlayer.RolePlayerStatusId);
            tesSessionRolePlayer.StatusChangeUser =
                NHibernateSession.Current.Load<User>(request.TestSessionRolePlayer.StatusChangeUserId);
            tesSessionRolePlayer.StatusChangeDate = request.TestSessionRolePlayer.StatusChangeDate;

            (request.TestSessionRolePlayer.ObjectStatus == ObjectStatusTypeName.Deleted ? dataToDelete : dataToUpsert)
                .Add(tesSessionRolePlayer);

            var detailsById = NHibernateSession.Current.Query<TestSessionRolePlayerDetail>()
                .Where(x => x.TestSessionRolePlayer.Id == tesSessionRolePlayer.Id)
                .ToDictionary(n => n.Id, v => v);

            var deletedDetails = detailsById.Values
                .Where(v => request.TestSessionRolePlayer.Details.All(d => d.TestSessionRolePlayerDetailId != v.Id));

            dataToDelete.AddRange(deletedDetails);
            foreach (var detail in request.TestSessionRolePlayer.Details.OrderBy(x => x.SkillId)
                .ThenBy(y => y.LanguageId))
            {
                TestSessionRolePlayerDetail rolePlayerDetail;
                if (!detailsById.TryGetValue(detail.TestSessionRolePlayerDetailId, out rolePlayerDetail))
                {
                    rolePlayerDetail = new TestSessionRolePlayerDetail()
                    {
                        TestSessionRolePlayer = tesSessionRolePlayer,
                    };
                }

                rolePlayerDetail.TestComponent = NHibernateSession.Current.Load<TestComponent>(detail.TestComponentId);
                rolePlayerDetail.Skill = NHibernateSession.Current.Load<Skill>(detail.SkillId);
                rolePlayerDetail.Language = NHibernateSession.Current.Load<Language>(detail.LanguageId);
                rolePlayerDetail.RolePlayerRoleType =
                    NHibernateSession.Current.Load<RolePlayerRoleType>(detail.RolePlayerRoleTypeId);
                (detail.ObjectStatus == ObjectStatusTypeName.Deleted ? dataToDelete : dataToUpsert).Add(
                    rolePlayerDetail);
            }

            return (dataToDelete, dataToUpsert);
        }

        private (IList<object> dataToDelete, IList<object> dataToUpsert) GetPersonNoteDataToUsert(
            UpsertSessionRolePlayerRequest request)
        {
            var entity = NHibernateSession.Current.Load<RolePlayer>(request.TestSessionRolePlayer.RolePlayerId)
                .Person.Entity;
            var existingNotesIds = request.PersonNotes.Select(y => y.NoteId).ToList().Concat(new List<int>() { 0 });
            var existingNotesById = NHibernateSession.Current.Query<NaatiEntityNote>()
                .Where(x => existingNotesIds.Contains(x.Note.Id)).ToDictionary(n => n.Id, v => v);

            var dataToUpsert = new List<object>();
            var dataToDelete = new List<object>();

            foreach (var pn in request.PersonNotes)
            {
                NaatiEntityNote personNote;
                if (!existingNotesById.TryGetValue(pn.NoteId, out personNote))
                {
                    var note = new Note();

                    dataToUpsert.Add(note);
                    personNote = new NaatiEntityNote
                    {
                        Entity = entity,
                        Note = note
                    };
                }

                personNote.Note.ReadOnly = pn.ReadOnly;
                personNote.Note.CreatedDate = pn.CreatedDate;
                personNote.Note.Description = pn.Description;
                personNote.Note.User = NHibernateSession.Current.Load<User>(pn.UserId);
                personNote.Note.Highlight = pn.Highlight;
                personNote.Note.ModifiedDate = DateTime.Now;
                dataToUpsert.Add(personNote);
            }

            return (dataToDelete, dataToUpsert);
        }

        public ServiceResponse<IEnumerable<TestSittingHistoryItemDto>> GetTestSittings(int credentialRequestId)
        {
            var result = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => x.CredentialRequest.Id == credentialRequestId)
                .OrderByDescending(y => y.Id);

            var response = new ServiceResponse<IEnumerable<TestSittingHistoryItemDto>>();
            if (result != null)
            {
                response.Data = result.Select(testSitting => new TestSittingHistoryItemDto
                {
                    Rejected = testSitting.Rejected,
                    CredentialRequestId = testSitting.CredentialRequest.Id,
                    CredentialRequestStatusTypeId = testSitting.CredentialRequest.CredentialRequestStatusType.Id,
                    TestDateTime = testSitting.TestSession.TestDateTime,
                    TestSessionId = testSitting.TestSession.Id,
                    TestSittingId = testSitting.Id,
                    Sat = testSitting.Sat,
                    AllocatedDate = testSitting.AllocatedDate,
                    RejectedDate = testSitting.RejectedDate
                });
            }

            return response;
        }

        public ServiceResponse<List<string>> TestSittingsWithoutMaterialReminderEmailAddresses()
        {
            var emailAddresses = NHibernateSession.Current.Query<SystemValue>().First(x => x.ValueKey == "TestSittingsWithoutMaterialReminderEmailAddresses");

            var result = emailAddresses.Value.Split(';').ToList();

            return new ServiceResponse<List<string>>()
            { 
                Data = result
            };
        }

        //For SendTestSessionSittingMaterialReminderAction, get entityid from email address
        public ServiceResponse<int> TestSittingsWithoutMaterialReminderEmailAddresses(string emailAddress)
        {
            var response = NHibernateSession.Current.Query<Email>()
                .FirstOrDefault(x => x.EmailAddress == emailAddress);

            if(response == null)
            {
                return new ServiceResponse<int>
                {
                    Data = 0
                };
            }

            return new ServiceResponse<int>
            {
                Data = response.Entity.Id
            };
        }

        public bool IsRolePlayerAvailable()
        {
            var result = NHibernateSession.Current.Query<SystemValue>().First(x => x.ValueKey == "RolePlayerAvailable");
            if(result == null)
            {
                return true;
            }
            return result.Value == "true" ? true : false;
        }


        //public IEnumerable<TestSessionSummaryDto> GetTestSittingsWithUsersProj(int hours)
        //{
        //    TestSitting mTestSitting = null;
        //    TestSession mTestSession = null;
        //    CredentialRequest mCredentialRequest = null;
        //    CredentialApplication mCredentialApplication = null;
        //    Person mPerson = null;
        //    NaatiEntity mEntity = null;
        //    Venue mVenue = null;
        //    TestLocation mTestLocation = null;
        //    LatestPersonName mLatestPersonName = null;
        //    PersonName mPersonName = null;
        //    Skill mSkill = null;
        //    Language mLanguage1 = null;
        //    Language mLanguage2 = null;
        //    DirectionType mDirectionType = null;
        //    CredentialType mCredentialType = null;
        //    Email mEmail = null; 
        //    TestSessionSummaryDto dto = null;


        //    var language1Replace = Projections.SqlFunction("REPLACE", NHibernateUtil.String,
        //       Projections.Property(() => mDirectionType.Name),
        //       Projections.Constant("L1", NHibernateUtil.String),
        //       Projections.SqlFunction("CONCAT", NHibernateUtil.String, Projections.Constant(" "), Projections.Property(() => mLanguage1.Name), Projections.Constant(" ")));

        //    var skillDisplayNameProjection = Projections.SqlFunction("REPLACE", NHibernateUtil.String,
        //        language1Replace,
        //        Projections.Constant("L2", NHibernateUtil.String),
        //        Projections.SqlFunction("CONCAT", NHibernateUtil.String, Projections.Constant(" "), Projections.Property(() => mLanguage2.Name), Projections.Constant(" ")));

        //    var validTestSitting = Restrictions.Eq(Projections.Property(() => mTestSitting.Rejected), false);
        //    var validTestSessionDate = Restrictions.Between(Projections.Property(() => mTestSession.TestDateTime), DateTime.Now, DateTime.Now.AddHours(hours));
        //    var validTestSession = Restrictions.Eq(Projections.Property(() => mTestSession.Synced), false);
        //    var validTestLocation = Restrictions.Eq(Projections.Property(() => mTestLocation.IsAutomated), true);
        //    var validEmail = Restrictions.Eq(Projections.Property(() => mEmail.Invalid), false);
        //    var validEmailPreferred = Restrictions.Eq(Projections.Property(() => mEmail.IsPreferredEmail), true);

        //    var testSessionsForTelevicDownloadQuery = NHibernateSession.Current.QueryOver(() => mTestSitting)
        //        .Inner.JoinAlias(x => mTestSitting.TestSession, () => mTestSession)
        //        .Inner.JoinAlias(x => mTestSitting.CredentialRequest, () => mCredentialRequest)
        //        .Inner.JoinAlias(x => mTestSession.Venue, () => mVenue)
        //        .Inner.JoinAlias(x => mVenue.TestLocation, () => mTestLocation)
        //        .Inner.JoinAlias(x => mCredentialRequest.CredentialApplication, () => mCredentialApplication)
        //        .Inner.JoinAlias(x => mCredentialRequest.Skill, () => mSkill)
        //        .Inner.JoinAlias(x => mCredentialRequest.CredentialType, () => mCredentialType)
        //        .Inner.JoinAlias(x => mSkill.Language1, () => mLanguage1)
        //        .Inner.JoinAlias(x => mSkill.Language2, () => mLanguage2)
        //        .Inner.JoinAlias(x => mSkill.DirectionType, () => mDirectionType)
        //        .Inner.JoinAlias(x => mCredentialApplication.Person, () => mPerson)
        //        .Inner.JoinAlias(x => mPerson.Entity, () => mEntity)
        //        .Inner.JoinAlias(x => mPerson.LatestPersonName, () => mLatestPersonName)
        //        .Inner.JoinAlias(x => mLatestPersonName.PersonName, () => mPersonName)
        //        .Inner.JoinAlias(x => mEntity.Emails, () => mEmail, Restrictions.And(validEmail, validEmailPreferred))
        //        .Where(Restrictions.Conjunction()
        //        .Add(validTestSitting)
        //        .Add(validTestSessionDate)
        //        .Add(validTestSession)
        //        .Add(validTestLocation))
        //        .Select(Projections.ProjectionList()
        //        .Add(Projections.Property(() => mTestSession.Id).WithAlias(() => dto.TestSessionId))
        //        .Add(Projections.Property(() => mSkill.Id).WithAlias(() => dto.SkillId))
        //        .Add(skillDisplayNameProjection.WithAlias(() => dto.SkillName))
        //        .Add(Projections.Property(() => mTestSession.Name).WithAlias(() => dto.TestSessionName))
        //        .Add(Projections.Property(() => mTestSession.TestDateTime).WithAlias(() => dto.TestSessionDate))
        //        .Add(Projections.Property(() => mPersonName.OtherNames).WithAlias(() => dto.OtherNames))
        //        .Add(Projections.Property(() => mPersonName.GivenName).WithAlias(() => dto.GivenName))
        //        .Add(Projections.Property(() => mPerson.Surname).WithAlias(() => dto.Surname))
        //        .Add(Projections.Property(() => mPerson.BirthDate).WithAlias(() => dto.BirthDate))
        //        .Add(Projections.Property(() => mCredentialType.Id).WithAlias(() => dto.CredentialTypeId))
        //        .Add(Projections.Property(() => mCredentialType.InternalName).WithAlias(() => dto.CredentialTypeName))
        //        .Add(Projections.Property(() => mPerson.BirthDate).WithAlias(() => dto.BirthDate))
        //        .Add(Projections.Property(() => mEmail.EmailAddress).WithAlias(() => dto.Email))
        //        .Add(Projections.Property(() => mEntity.NaatiNumber).WithAlias(() => dto.NaatiNumber))
        //        )
        //        .TransformUsing(Transformers.AliasToBean<TestSessionSummaryDto>());

        //    var results = testSessionsForTelevicDownloadQuery.List<TestSessionSummaryDto>();

        //    return results; 
        //}

        //    public IEnumerable<TestSessionSummaryDto> GetTestSittingsWithUsers(int hours)
        //    {
        //        var result = new List<TestSessionSummaryDto>();

        //        var testSittingGroups = NHibernateSession.Current.Query<TestSitting>()
        //           .Where(x => !x.Rejected
        //                        && x.TestSession.TestDateTime >= DateTime.Now
        //                        && x.TestSession.TestDateTime <= DateTime.Now.AddHours(hours)
        //                        && !x.TestSession.Synced
        //                        && x.TestSession.Venue.TestLocation.IsAutomated)
        //           .GroupBy(x => new { TestSessionId = x.TestSession.Id, SkillId = x.CredentialRequest.Skill.Id });

        //        foreach (var testSittingGroup in testSittingGroups)
        //        {
        //            var testSession = new TestSessionSummaryDto()
        //            {
        //                TestSessionId = testSittingGroup.Key.TestSessionId,
        //                TestSessionName = testSittingGroup.FirstOrDefault().TestSession.Name,
        //                //Applicants = testSittingGroup.Select(x => new PersonEntityDto
        //                //{
        //                //    NaatiNumber = x.CredentialRequest.CredentialApplication.Person.Entity.NaatiNumber,
        //                //    Email = x.CredentialRequest.CredentialApplication.Person.PrimaryEmailAddress,
        //                //    GivenName = x.CredentialRequest.CredentialApplication.Person.GivenName,
        //                //    BirthDate = x.CredentialRequest.CredentialApplication.Person.BirthDate,
        //                //    Surname = x.CredentialRequest.CredentialApplication.Person.Surname
        //                //})
        //            };

        //            result.Add(testSession);
        //        }

        //        return result; 
        //    }
    }
}
