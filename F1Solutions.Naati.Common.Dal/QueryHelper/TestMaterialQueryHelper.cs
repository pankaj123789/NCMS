using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    public class TestMaterialQueryHelper : QuerySearchHelper
    {
        private IList<SpecificationSkillDto> GetSkills(IEnumerable<int> testSessionIds, int testSpecificationId)
        {
            SpecificationSkillDto dto = null;
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                .Inner.JoinAlias(x => Skill.Language2, () => Language2);

            var filters = Restrictions.Conjunction();
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSession.Completed), false));
            filters.Add(Restrictions.In(Projections.Property(() => TestSession.Id), testSessionIds.ToArray()));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), testSpecificationId));

            var skillId = Projections.GroupProperty(Projections.Property(() => Skill.Id));
            var skill = Projections.Max(GetDirectionProjection());
            var totalApplicants = Projections.Count(Projections.Distinct(Projections.Property(() => TestSitting.Id)));
            query.Where(filters);

            var properties = Projections.ProjectionList()
                .Add(skillId.WithAlias(() => dto.Id))
                .Add(skill.WithAlias(() => dto.Skill))
                .Add(totalApplicants.WithAlias(() => dto.TotalApplicants));

            var result = query.Select(properties)
                .TransformUsing(Transformers.AliasToBean<SpecificationSkillDto>())
                .List<SpecificationSkillDto>();

            return result;
        }

        public IList<TestSpecificationDetailsDto> GetTestSpecificationDetails(IEnumerable<int> testSessionIds)
        {
            TestSpecificationDetailsDto dto = null;
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.TestSpecification, () => TestSpecification)
                .Left.JoinAlias(x => TestSpecification.TestComponents, () => TestComponent);


            var filters = Restrictions.Conjunction();
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSession.Completed), false));
            filters.Add(Restrictions.In(Projections.Property(() => TestSession.Id), testSessionIds.ToArray()));
            query.Where(filters);

            var testSpecificationId = Projections.GroupProperty(Projections.Property(() => TestSpecification.Id));
            var description = Projections.Max(Projections.Property(() => TestSpecification.Description));
            var numberOfTasks = Projections.Count(Projections.Distinct(Projections.Property(() => TestComponent.Id)));

            var properties = Projections.ProjectionList()
                .Add(testSpecificationId.WithAlias(() => dto.Id))
                .Add(description.WithAlias(() => dto.Description))
                .Add(numberOfTasks.WithAlias(() => dto.NumberOfTasks));

            var result = query.Select(properties)
                .TransformUsing(Transformers.AliasToBean<TestSpecificationDetailsDto>())
                .List<TestSpecificationDetailsDto>();

            return result;
        }

        public IEnumerable<SpecificationSkillDto> GetTestSpecificationSkills(IEnumerable<int> testSessionIds, int testSpecificationId)
        {

            var sessionIds = testSessionIds.ToArray();

            var query = NHibernateSession.Current.QueryOver(() => TestSpecification)
                .Inner.JoinAlias(x => TestSpecification.TestComponents, () => TestComponent)
                .Left.JoinAlias(x => TestComponent.TestSittingTestMaterials, () => TestSittingTestMaterial)
                .Left.JoinAlias(x => TestSittingTestMaterial.TestSitting, () => TestSitting)
                .Left.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Left.JoinAlias(x => CredentialRequest.Skill, () => Skill);

            var filters = Restrictions.Conjunction();

            var tesSittingIds = GetNotSatTestAttendanceIdsInTestSessionsQuery(sessionIds);
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), testSpecificationId));
            filters.Add(Restrictions.Or(Restrictions.IsNull(Projections.Property(() => TestSitting.Id)), Subqueries.WhereProperty<TestSitting>(x => TestSitting.Id).In(tesSittingIds)));

            var grouping1 = Projections.GroupProperty(Projections.Property(() => Skill.Id));
            var grouping2 = Projections.GroupProperty(Projections.Property(() => TestComponent.Id));
            var totalSittingsIdsWithAssignedTask = Projections.Count(Projections.Distinct(Projections.Property(() => TestSitting.Id)));

            var properties = Projections.ProjectionList().Add(grouping1).Add(grouping2).Add(totalSittingsIdsWithAssignedTask);

            var totalSkillMaterials = query.Where(filters)
                .Select(properties)
                .List<IList>();
            var totalTestComponents = NHibernateSession.Current.Query<TestComponent>()
                .Count(x => x.TestSpecification.Id == testSpecificationId);

            var totalSkillMaterialGroup = totalSkillMaterials.GroupBy(x => x[0]).ToList();
            var specificationSkills = GetSkills(sessionIds, testSpecificationId);

            var sessionDatesForSkills = GetTestSessionDatesForSkills(sessionIds, testSpecificationId);

            foreach (var skill in specificationSkills)
            {
                skill.TestDates = sessionDatesForSkills.Where(x => x.SkillId == skill.Id).Select(x => x.Date);
                var group = totalSkillMaterialGroup.FirstOrDefault(x => (int?)x.Key == skill.Id);
                if (group == null || group.Count() < totalTestComponents)
                {
                    skill.ApplicantsWithoutMaterials = skill.TotalApplicants;
                }
                else
                {
                    skill.ApplicantsWithoutMaterials = skill.TotalApplicants - group.Min(x => (int)(x[2] ?? 0));
                }

            }

            return specificationSkills;

        }


        private IList<SessionDateForSkill> GetTestSessionDatesForSkills(IEnumerable<int> testSessionIds, int testSpecificationId)
        {
            SessionDateForSkill dto = null;
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill);

            var filters = Restrictions.Conjunction();
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), false));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), testSpecificationId));
            filters.Add(Restrictions.In(Projections.Property(() => TestSession.Id), testSessionIds.ToArray()));
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false));

            var skillId = Projections.GroupProperty(Projections.Property(() => Skill.Id));
            var date = Projections.GroupProperty(GetDateProjectionFrom(Projections.Property(() => TestSession.TestDateTime)));

            var properties = Projections.ProjectionList()
                .Add(skillId.WithAlias(() => dto.SkillId))
                .Add(date.WithAlias(() => dto.Date));

            return query.Where(filters).Select(properties)
                .TransformUsing(Transformers.AliasToBean<SessionDateForSkill>())
                .List<SessionDateForSkill>();

        }

        public int GetTotalPendingSittingsWithTasksToAssign(int testSpecificationId, IEnumerable<int> testSessionIds)
        {
            var sessionIds = testSessionIds.ToArray();

            var query = NHibernateSession.Current.QueryOver(() => TestSpecification)
                .Inner.JoinAlias(x => TestSpecification.TestComponents, () => TestComponent)
                .Left.JoinAlias(x => TestComponent.TestSittingTestMaterials, () => TestSittingTestMaterial)
                .Left.JoinAlias(x => TestSittingTestMaterial.TestSitting, () => TestSitting);

            var filters = Restrictions.Conjunction();

            var tesSittingIds = GetNotSatTestAttendanceIdsInTestSessionsQuery(sessionIds);
            filters.Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), testSpecificationId));
            filters.Add(Restrictions.Or(Restrictions.IsNull(Projections.Property(() => TestSitting.Id)), Subqueries.WhereProperty<TestSitting>(x => TestSitting.Id).In(tesSittingIds)));

            var totalSittingsIdsWithAssignedTask = Projections.Count(Projections.Distinct(Projections.Property(() => TestSitting.Id)));
            var grouping = Projections.GroupProperty(Projections.Property(() => TestComponent.Id));

            var properties = Projections.ProjectionList().Add(grouping).Add(totalSittingsIdsWithAssignedTask);

            var result = query.Where(filters)
                .Select(properties)
                .List<IList>();

            var totalTestComponents = NHibernateSession.Current.Query<TestComponent>()
                .Count(x => x.TestSpecification.Id == testSpecificationId);

            var totalSittings = GetTotalNotSatTestAttendanceIdsInTestSessions(sessionIds, testSpecificationId);
            if (result.Count < totalTestComponents)
            {
                return totalSittings;
            }

            var pendingSittings = totalSittings - result.Min(x => (int)(x[1] ?? 0));
            return pendingSittings;

        }


        private QueryOver<TestSittingTestMaterial, TestSittingTestMaterial> GetMaterialsSatByMinimumNPeopleInTestSessions(IEnumerable<int> testSessionIds, int minApplicants, int testSpecificationId, int skillId, bool excludeSupplementary)
        {
            var sessionds = testSessionIds.ToArray();
            TestSittingTestMaterial pmTestSittingTestMaterial = null;
            TestMaterial pmTestMaterial = null;
            TestSitting pmTestSitting = null;
            TestSession pmTestSession = null;
            CredentialRequest pmCredentialRequest = null;
            CredentialApplication pmCredentialApplication = null;
            Person pmPerson = null;

            var query = QueryOver.Of(() => pmTestSittingTestMaterial)
                .Inner.JoinAlias(x => pmTestSittingTestMaterial.TestMaterial, () => pmTestMaterial)
                .Inner.JoinAlias(x => pmTestSittingTestMaterial.TestSitting, () => pmTestSitting)
                .Inner.JoinAlias(x => pmTestSitting.TestSession, () => pmTestSession)
                .Inner.JoinAlias(x => pmTestSitting.CredentialRequest, () => pmCredentialRequest)
                .Inner.JoinAlias(x => pmCredentialRequest.CredentialApplication, () => pmCredentialApplication)
                .Inner.JoinAlias(x => pmCredentialApplication.Person, () => pmPerson);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSitting.Sat), true));
            filter.Add(Restrictions.Not(InList(Projections.Property(() => pmTestSession.Id), sessionds))); // TODO REVIEW THIS
            filter.Add(Subqueries.WhereProperty<Person>(e => pmPerson.Id).In(PersonIdsInNotCompletedTestSessions(sessionds, testSpecificationId, skillId, excludeSupplementary)));
            query.Where(filter);
            query.Select(Projections.GroupProperty(Projections.Property(() => pmTestMaterial.Id)));
            //having clause
            query.Where(Restrictions.Ge(Projections.Count(Projections.Distinct(Projections.Property(() => pmPerson.Id))), minApplicants));
            return query;
        }

        private QueryOver<TestSittingTestMaterial, TestSittingTestMaterial> GetMaterialsSatByMinumumNPeopleWhoIsNotInTestSessions(IEnumerable<int> testSessionIds, int minApplicants)
        {
            TestSittingTestMaterial pmTestSittingTestMaterial = null;
            TestMaterial pmTestMaterial = null;
            TestSitting pmTestSitting = null;
            TestSession pmTestSession = null;
            CredentialRequest pmCredentialRequest = null;
            CredentialApplication pmCredentialApplication = null;
            Person pmPerson = null;

            var query = QueryOver.Of(() => pmTestSittingTestMaterial)
                .Inner.JoinAlias(x => pmTestSittingTestMaterial.TestMaterial, () => pmTestMaterial)
                .Inner.JoinAlias(x => pmTestSittingTestMaterial.TestSitting, () => pmTestSitting)
                .Inner.JoinAlias(x => pmTestSitting.TestSession, () => pmTestSession)
                .Inner.JoinAlias(x => pmTestSitting.CredentialRequest, () => pmCredentialRequest)
                .Inner.JoinAlias(x => pmCredentialRequest.CredentialApplication, () => pmCredentialApplication)
                .Inner.JoinAlias(x => pmCredentialApplication.Person, () => pmPerson);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSitting.Sat), true));
            // filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSession.Completed), true));
            //taking this out in Ticket 201036 to speed up where clause
            //filter.Add(Subqueries.WhereProperty<Person>(e => pmPerson.Id).NotIn(PersonIdsInNotCompletedTestSessions(testSessionIds)));
            query.Where(filter);

            query.Select(Projections.GroupProperty(Projections.Property(() => pmTestMaterial.Id)));
            //having clause
            query.Where(Restrictions.Ge(Projections.Count(Projections.Distinct(Projections.Property(() => pmPerson.Id))), minApplicants));
            return query;
        }


        private IProjection GetTestMaterialStatus(IEnumerable<int> testSessionIds, int testSpecificationId, int skillId)
        {
            var sessionIds = testSessionIds.ToArray();

            var generalStatus = GetGeneralTestMaterialStatus(sessionIds);
            var materialsUsedByTestSessionApplicantsCondition = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id).In(GetMaterialsSatByMinimumNPeopleInTestSessions(sessionIds, 1, testSpecificationId, skillId, true));
            var usedByApplicantsStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.UsedByApplicants);
            var status = Projections.Conditional(materialsUsedByTestSessionApplicantsCondition, usedByApplicantsStatus, generalStatus);
            return status;
        }

        private IProjection GetGeneralTestMaterialStatus(IEnumerable<int> testSessionIds)
        {
            var sessionIds = testSessionIds.ToArray();

            var materialsUsedByOtherApplicantsCondition = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id).In(GetMaterialsSatByMinumumNPeopleWhoIsNotInTestSessions(sessionIds, 1));
            var materialsToBeUsed = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id).In(GetMaterialsThatAreGoingTobeUsedByMinimumNApplicants(1));

            var previouslyUsedStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.PreviouslyUsed);
            var toBeUsedStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.ToBeUsed);
            var newStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.New);

            var status = Projections.Conditional(materialsUsedByOtherApplicantsCondition, previouslyUsedStatus,
                    Projections.Conditional(materialsToBeUsed, toBeUsedStatus, newStatus));

            return status;
        }

        private IProjection GetTestMaterialStatusProjection(TestMaterialStatusTypeName status)
        {
            return Projections.Constant((int)status, NHibernateUtil.Int32);
        }

        private IProjection GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName range)
        {
            return Projections.Constant((int)range, NHibernateUtil.Int32);
        }


        private IProjection GetApplicantRangeTypeForUsedByApplicantStatus(int[] testSessionIds, int testSpecificationId, int skillId)
        {
            var usedByOneOrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                    .In(GetMaterialsSatByMinimumNPeopleInTestSessions(testSessionIds, 1, testSpecificationId, skillId, true));

            var usedByTwoOrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                    .In(GetMaterialsSatByMinimumNPeopleInTestSessions(testSessionIds, 2, testSpecificationId, skillId, true));

            var usedBy6OrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                    .In(GetMaterialsSatByMinimumNPeopleInTestSessions(testSessionIds, 6, testSpecificationId, skillId, true));

            var moreThanFive = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.MoreThanFive);
            var towAndFive = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.TwoAndFive);
            var one = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.One);
            var none = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.None);

            return Projections.Conditional(usedBy6OrMoreApplicantInTestSession, moreThanFive,
                 Projections.Conditional(usedByTwoOrMoreApplicantInTestSession, towAndFive,
                     Projections.Conditional(usedByOneOrMoreApplicantInTestSession, one, none)));
        }

        private IProjection GetApplicantRangeTypeForPreviouslyUsedStatus(int[] testSessionIds)
        {
            var usedByOneOrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                .In(GetMaterialsSatByMinumumNPeopleWhoIsNotInTestSessions(testSessionIds, 1));

            var usedByTwoOrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                .In(GetMaterialsSatByMinumumNPeopleWhoIsNotInTestSessions(testSessionIds, 2));

            var usedBy6OrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                .In(GetMaterialsSatByMinumumNPeopleWhoIsNotInTestSessions(testSessionIds, 6));

            var moreThanFive = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.MoreThanFive);
            var towAndFive = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.TwoAndFive);
            var one = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.One);
            var none = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.None);

            return Projections.Conditional(usedBy6OrMoreApplicantInTestSession, moreThanFive,
                Projections.Conditional(usedByTwoOrMoreApplicantInTestSession, towAndFive,
                    Projections.Conditional(usedByOneOrMoreApplicantInTestSession, one, none)));
        }

        private IProjection GetApplicantRangeTypeForToBeUsedStatus()
        {
            var usedByOneOrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                .In(GetMaterialsThatAreGoingTobeUsedByMinimumNApplicants(1));

            var usedByTwoOrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                .In(GetMaterialsThatAreGoingTobeUsedByMinimumNApplicants(2));

            var usedBy6OrMoreApplicantInTestSession = Subqueries.WhereProperty<TestMaterial>(e => TestMaterial.Id)
                .In(GetMaterialsThatAreGoingTobeUsedByMinimumNApplicants(6));

            var moreThanFive = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.MoreThanFive);
            var towAndFive = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.TwoAndFive);
            var one = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.One);
            var none = GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.None);

            return Projections.Conditional(usedBy6OrMoreApplicantInTestSession, moreThanFive,
                Projections.Conditional(usedByTwoOrMoreApplicantInTestSession, towAndFive,
                    Projections.Conditional(usedByOneOrMoreApplicantInTestSession, one, none)));
        }

        public IProjection GetApplicantRangeTypeId(IEnumerable<int> testSessionIds, IProjection materialTestStatus, int testSpecificationId, int skillId)
        {
            var sessionIds = testSessionIds.ToArray();

            var usedByApplicantsStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.UsedByApplicants);
            var previouslyUsedStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.PreviouslyUsed);
            var toBeUsedStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.ToBeUsed);
            var newStatus = GetTestMaterialStatusProjection(TestMaterialStatusTypeName.New);

            var isUsedByApplicantStatus = Restrictions.EqProperty(materialTestStatus, usedByApplicantsStatus);
            var isPrevioulyUsedStatus = Restrictions.EqProperty(materialTestStatus, previouslyUsedStatus);
            var isToBeUsedStatus = Restrictions.EqProperty(materialTestStatus, toBeUsedStatus);
            var isNewStatus = Restrictions.EqProperty(materialTestStatus, newStatus);

            return Projections.Conditional(isUsedByApplicantStatus,
                GetApplicantRangeTypeForUsedByApplicantStatus(sessionIds, testSpecificationId, skillId),
                Projections.Conditional(isPrevioulyUsedStatus, GetApplicantRangeTypeForPreviouslyUsedStatus(sessionIds),
                    Projections.Conditional(isToBeUsedStatus, GetApplicantRangeTypeForToBeUsedStatus(),
                        Projections.Conditional(isNewStatus,
                            GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.None),
                            GetApplicantRangeTypeProjection(TestMaterialApplicantsRangeTypeName.Undefined)))));


        }

        private QueryOver<TestSittingTestMaterial, TestSittingTestMaterial> GetMaterialsThatAreGoingTobeUsedByMinimumNApplicants(int minApplicants)
        {
            TestSittingTestMaterial pmTestSittingTestMaterial = null;
            TestMaterial pmTestMaterial = null;
            TestSitting pmTestSitting = null;
            TestSession pmTestSession = null;
            CredentialRequest pmCredentialRequest = null;
            CredentialApplication pmCredentialApplication = null;
            Person pmPerson = null;

            var query = QueryOver.Of(() => pmTestSittingTestMaterial)
                .Inner.JoinAlias(x => pmTestSittingTestMaterial.TestMaterial, () => pmTestMaterial)
                .Inner.JoinAlias(x => pmTestSittingTestMaterial.TestSitting, () => pmTestSitting)
                .Inner.JoinAlias(x => pmTestSitting.TestSession, () => pmTestSession)
                .Inner.JoinAlias(x => pmTestSitting.CredentialRequest, () => pmCredentialRequest)
                .Inner.JoinAlias(x => pmCredentialRequest.CredentialApplication, () => pmCredentialApplication)
                .Inner.JoinAlias(x => pmCredentialApplication.Person, () => pmPerson);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSitting.Sat), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => pmTestSession.Completed), false));
            query.Where(filter);

            query.Select(Projections.GroupProperty(Projections.Property(() => pmTestMaterial.Id)));
            //having clause
            query.Where(Restrictions.Ge(Projections.Count(Projections.Distinct(Projections.Property(() => pmPerson.Id))), minApplicants));
            return query;
        }


        private QueryOver<TestSitting, TestSitting> PersonIdsInNotCompletedTestSessions(IEnumerable<int> testSessionIds, int? tesSpecificationId = null, int? skillId = null, bool excludeSupplementary = false)
        {
            TestSitting ptsTestSitting = null;
            TestSession ptsTestSession = null;
            CredentialRequest ptsCredentialRequest = null;
            CredentialApplication ptsCredentialApplication = null;
            Skill ptsSkill = null;
            TestSpecification ptsTestSpecification = null;
            Person ptsPerson = null;

            var query = QueryOver.Of(() => ptsTestSitting)
                .Inner.JoinAlias(x => ptsTestSitting.TestSession, () => ptsTestSession)
                .Inner.JoinAlias(x => ptsTestSitting.CredentialRequest, () => ptsCredentialRequest)
                .Inner.JoinAlias(x => ptsCredentialRequest.Skill, () => ptsSkill)
                .Inner.JoinAlias(x => ptsTestSitting.TestSpecification, () => ptsTestSpecification)
                .Inner.JoinAlias(x => ptsCredentialRequest.CredentialApplication, () => ptsCredentialApplication)
                .Inner.JoinAlias(x => ptsCredentialApplication.Person, () => ptsPerson);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Rejected), false));
            if (excludeSupplementary)
            {
                filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Supplementary), false));
            }

            if (skillId.HasValue)
            {
                filter.Add(Restrictions.Eq(Projections.Property(() => ptsSkill.Id), skillId.GetValueOrDefault()));
            }

            if (tesSpecificationId.HasValue)
            {
                filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSpecification.Id), tesSpecificationId.GetValueOrDefault()));
            }
            filter.Add(InList(Projections.Property(() => ptsTestSession.Id), testSessionIds));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSession.Completed), false));
            query.Where(filter);
            query.Select(Projections.Distinct(Projections.Property(() => ptsPerson.Id)));
            return query;

        }

        public IEnumerable<int> GetNotSatTestAttendanceIdsInTestSessions(TestMaterialsSummaryRequest request)
        {
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), false));
            filter.Add(Restrictions.In(Projections.Property(() => TestSession.Id), request.TestSessionIds.ToArray()));
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => Skill.Id), request.SkillId));
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), request.TestSpecificationId));
            query.Where(filter);
            query.Select(Projections.Property(() => TestSitting.Id));
            var result = query.List<int>();
            return result;

        }

        public int GetTotalNotSatTestAttendanceIdsInTestSessions(IEnumerable<int> testSessionIds, int testSpecificationId)
        {
            TestSitting ptsTestSitting = null;
            TestSession ptsTestSession = null;
            TestSpecification ptsTestSpecification = null;

            var query = NHibernateSession.Current.QueryOver(() => ptsTestSitting)
                .Inner.JoinAlias(x => ptsTestSitting.TestSession, () => ptsTestSession)
                .Inner.JoinAlias(x => ptsTestSitting.TestSpecification, () => ptsTestSpecification);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Supplementary), false));
            filter.Add(Restrictions.In(Projections.Property(() => ptsTestSession.Id), testSessionIds.ToArray()));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Sat), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSpecification.Id), testSpecificationId));
            query.Where(filter);
            query.Select(Projections.Count(Projections.Property(() => ptsTestSitting.Id)));
            var result = query.List<int>()[0];
            return result;

        }

        public QueryOver<TestSitting, TestSitting> GetNotSatTestAttendanceIdsInTestSessionsQuery(IEnumerable<int> testSessionIds)
        {
            TestSitting ptsTestSitting = null;
            TestSession ptsTestSession = null;

            var query = QueryOver.Of(() => ptsTestSitting)
                .Inner.JoinAlias(x => ptsTestSitting.TestSession, () => ptsTestSession);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Supplementary), false));
            filter.Add(Restrictions.In(Projections.Property(() => ptsTestSession.Id), testSessionIds.ToArray()));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting.Sat), false));
            query.Where(filter);
            query.Select(Projections.Property(() => ptsTestSitting.Id));

            return query;

        }


        public QueryOver<TestSitting, TestSitting> GetNotSatTestAttendanceIdsInTestSessionsQuery2(IEnumerable<int> testSessionIds)
        {
            TestSitting ptsTestSitting2 = null;
            TestSession ptsTestSession2 = null;

            var query = QueryOver.Of(() => ptsTestSitting2)
                .Inner.JoinAlias(x => ptsTestSitting2.TestSession, () => ptsTestSession2);

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting2.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting2.Supplementary), false));
            filter.Add(Restrictions.In(Projections.Property(() => ptsTestSession2.Id), testSessionIds.ToArray()));
            filter.Add(Restrictions.Eq(Projections.Property(() => ptsTestSitting2.Sat), false));
            query.Where(filter);
            query.Select(Projections.Property(() => ptsTestSitting2.Id));

            return query;

        }



        public IEnumerable<TestMaterialDetailDto> GetTestSpecificationMaterials(TestSpecificationMaterialRequest request)
        {
            TestMaterialDetailDto dto = null;
            var query = NHibernateSession.Current.QueryOver(() => TestMaterial)
                 .Inner.JoinAlias(x => TestMaterial.TestMaterialType, () => TestMaterialType)
                 .Inner.JoinAlias(x => TestMaterial.TestMaterialDomain, () => TestMaterialDomain)
                 .Inner.JoinAlias(x => TestMaterial.TestComponentType, () => TestComponentType)
                 .Inner.JoinAlias(x => TestComponentType.TestComponentBaseType, () => TestComponentBaseType)
                 .Inner.JoinAlias(x => TestComponentType.TestSpecification, () => TestSpecification)
                 .Inner.JoinAlias(x => TestMaterial.TestMaterialLastUsed, () => TestMaterialLastUsed)
                 .Left.JoinAlias(x => TestMaterial.Skill, () => Skill)
                 .Left.JoinAlias(x => TestMaterial.Language, () => Language1);

            var languageBaseTypes = Restrictions.Conjunction();
            languageBaseTypes.Add(Restrictions.Eq(Projections.Property(() => TestComponentBaseType.Id), (int)TestComponentBaseTypeName.Language));
            languageBaseTypes.Add(Subqueries.WhereProperty<Language>(e => Language1.Id).In(GetLanguage1FromSkill(request.SkillId)));

            var skillBaseTypes = Restrictions.Conjunction();
            skillBaseTypes.Add(Restrictions.Eq(Projections.Property(() => TestComponentBaseType.Id), (int)TestComponentBaseTypeName.Skill));
            skillBaseTypes.Add(Restrictions.Eq(Projections.Property(() => Skill.Id), request.SkillId));

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => TestMaterial.Available), true));
            //filter.Add(Restrictions.Eq(Projections.Property(() => TestMaterialType.Id), (int)TestMaterialTypeName.Test));//172638
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), request.TestSpecificationId));
            filter.Add(Restrictions.Or(languageBaseTypes, skillBaseTypes));

            var testMaterialId = Projections.Property(() => TestMaterial.Id);
            var testMaterialTypeId = Projections.Property(() => TestMaterial.TestMaterialType.Id);
            var testcomponentTypId = Projections.Property(() => TestComponentType.Id);
            var testComponentTypeLabel = Projections.Property(() => TestComponentType.Label);
            var testComponentTypeDescription = Projections.Property(() => TestComponentType.Description);
            var testMaterialTitle = Projections.Property(() => TestMaterial.Title);
            var testMaterialDomain = Projections.Property(() => TestMaterialDomain.Id);
            var testMaterialStatus = GetTestMaterialStatus(request.TestSessionIds, request.TestSpecificationId, request.SkillId);
            //var applicantRangeTypeId = GetApplicantRangeTypeId(request.TestSessionIds, testMaterialStatus, request.TestSpecificationId, request.SkillId);
            var applicantRangeTypeId = Projections.Constant(0,NHibernateUtil.Int32);
            var lastDateUsed = Projections.Property(() => TestMaterialLastUsed.LastUsedDate);

            var properties = Projections.ProjectionList()
                .Add(testMaterialId.WithAlias(() => dto.Id))
                .Add(testMaterialTypeId.WithAlias(() => dto.TestMaterialTypeId))
                .Add(testcomponentTypId.WithAlias(() => dto.TypeId))
                .Add(testComponentTypeLabel.WithAlias(() => dto.TypeLabel))
                .Add(testMaterialTitle.WithAlias(() => dto.Title))
                .Add(testMaterialStatus.WithAlias(() => dto.StatusId))
                .Add(testComponentTypeDescription.WithAlias(() => dto.TypeDescription))
                .Add(applicantRangeTypeId.WithAlias(() => dto.ApplicantsRangeTypeId))
                .Add(lastDateUsed.WithAlias(() => dto.LastUsedDate))
                .Add(testMaterialDomain.WithAlias(() => dto.TestMaterialDomainId));

            var result = query.Where(filter).Select(properties)
                .OrderByAlias(() => dto.StatusId).Asc
                .ThenByAlias(() => dto.ApplicantsRangeTypeId).Asc
                .ThenByAlias(() => dto.Id).Asc
                .TransformUsing(Transformers.AliasToBean<TestMaterialDetailDto>()).List<TestMaterialDetailDto>()
                .Skip(request.Skip)
                .Take(request.Take);

            return result;
        }


        private QueryOver<Skill, Skill> GetLanguage1FromSkill(int skillId)
        {
            Skill mSkill = null;
            Language mLanguage1 = null;
            var query = QueryOver.Of(() => mSkill)
                .Inner.JoinAlias(x => mSkill.Language1, () => mLanguage1)
                .Where(x => mSkill.Id == skillId)
                .Select(Projections.Property(() => mLanguage1.Id));

            return query;


        }

        public IEnumerable<SupplementarytTestApplicantDto> GetSupplementaryTestApplicants(SupplementaryTestRequest request)
        {
            SupplementarytTestApplicantDto dto = null;
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity);

            var filters = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false))
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), true))
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false))
                .Add(Restrictions.In(Projections.Property(() => TestSession.Id), request.TestSessionIds.ToArray()));

            var properties = Projections.ProjectionList()
                .Add(Projections.Property(() => TestSitting.Id).WithAlias(() => dto.TestSittingId))
                .Add(Projections.Property(() => Entity.NaatiNumber).WithAlias(() => dto.NaatiNumber))
                .Add(Projections.Property(() => CredentialApplication.Reference).WithAlias(() => dto.ApplicationReference))
                .Add(Projections.Property(() => TestSession.Id).WithAlias(() => dto.TestSessionId))
                .Add(Projections.Property(() => CredentialApplication.Id).WithAlias(() => dto.ApplicationId));

            var result = query.Where(filters)
                .Select(properties)
                .TransformUsing(Transformers.AliasToBean<SupplementarytTestApplicantDto>())
                .List<SupplementarytTestApplicantDto>();

            return result;

        }

        public IEnumerable<TestSittingInfoDto> GetApplicantIdsToOverridMaterials(TestMaterialsSummaryRequest request)
        {
            TestSittingInfoDto dto = null;
            var testTaksIds = request.TestMaterialAssignments.Select(x => x.TestTaskId);

            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => TestSitting.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSitting.TestSittingTestMaterials, () => TestSittingTestMaterial)
                .Inner.JoinAlias(x => TestSittingTestMaterial.TestComponent, () => TestComponent);

            var filter = Restrictions.Conjunction()
            .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false))
            .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), false))
            .Add(Restrictions.Eq(Projections.Property(() => Skill.Id), request.SkillId))
            .Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), request.TestSpecificationId))
            .Add(Restrictions.In(Projections.Property(() => TestSession.Id), request.TestSessionIds.ToArray()))
            .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false))
            .Add(Restrictions.In(Projections.Property(() => TestComponent.Id), testTaksIds.ToArray()));

            var testSittingId = Projections.Property(() => TestSitting.Id);

            var personId = Projections.Property(() => Person.Id);


            var properties = Projections.ProjectionList()
                .Add(Projections.GroupProperty(testSittingId).WithAlias(() => dto.TestSittingId))
                .Add(Projections.Max(personId).WithAlias(() => dto.PersonId));


            var result = query.Where(filter)
                .Select(properties)
                .TransformUsing(Transformers.AliasToBean<TestSittingInfoDto>())
                .List<TestSittingInfoDto>();


            return result;

        }

        private QueryOver<TestSittingTestMaterial, TestSittingTestMaterial> GetTestSittingIdsWithTeskTaskAssigned(TestMaterialsSummaryRequest request)
        {
            var testTaksIds = request.TestMaterialAssignments.Select(x => x.TestTaskId);
            TestSittingTestMaterial mTestSittingTestMaterial = null;
            TestSitting mTestSitting = null;
            TestComponent mTestComponent = null;
            CredentialRequest mCredentialRequest = null;
            TestSpecification mTestSpecification = null;
            TestSession mTestSession = null;
            Skill mSkill = null;
            var query = QueryOver.Of(() => mTestSittingTestMaterial)
                .Inner.JoinAlias(x => mTestSittingTestMaterial.TestSitting, () => mTestSitting)
                .Inner.JoinAlias(x => mTestSitting.CredentialRequest, () => mCredentialRequest)
                .Inner.JoinAlias(x => mCredentialRequest.Skill, () => mSkill)
                .Inner.JoinAlias(x => mTestSitting.TestSpecification, () => mTestSpecification)
                .Inner.JoinAlias(x => mTestSitting.TestSession, () => mTestSession)
                .Inner.JoinAlias(x => mTestSittingTestMaterial.TestComponent, () => mTestComponent);

            var filter = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => mTestSitting.Rejected), false))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSitting.Supplementary), false))
                .Add(Restrictions.Eq(Projections.Property(() => mSkill.Id), request.SkillId))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSpecification.Id), request.TestSpecificationId))
                .Add(Restrictions.In(Projections.Property(() => mTestSession.Id), request.TestSessionIds.ToArray()))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSitting.Sat), false))
                .Add(Restrictions.In(Projections.Property(() => mTestComponent.Id), testTaksIds.ToArray()));

            var testSittingId = Projections.Property(() => mTestSitting.Id);

            return query.Where(filter).Select(testSittingId);
        }

        public IEnumerable<TestSittingInfoDto> GetNewMaterialsApplicantIds(TestMaterialsSummaryRequest request)
        {
            TestSittingInfoDto dto = null;
            var testTaksIds = request.TestMaterialAssignments.Select(x => x.TestTaskId);
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Left.JoinAlias(x => TestSitting.TestSittingTestMaterials, () => TestSittingTestMaterial)
                .Left.JoinAlias(x => TestSittingTestMaterial.TestComponent, () => TestComponent);

            var filter = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false))
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Supplementary), false))
                .Add(Restrictions.Eq(Projections.Property(() => Skill.Id), request.SkillId))
                .Add(Restrictions.Eq(Projections.Property(() => TestSpecification.Id), request.TestSpecificationId))
                .Add(Restrictions.In(Projections.Property(() => TestSession.Id), request.TestSessionIds.ToArray()))
                .Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), false))
                .Add(Subqueries.WhereProperty<TestSitting>(x => TestSitting.Id).NotIn(GetTestSittingIdsWithTeskTaskAssigned(request)));


            var testSittingId = Projections.Property(() => TestSitting.Id);
            var personId = Projections.Property(() => Person.Id);

            var properties = Projections.ProjectionList()
                .Add(Projections.GroupProperty(testSittingId).WithAlias(() => dto.TestSittingId))
                .Add(Projections.Max(personId).WithAlias(() => dto.PersonId));


            var result = query.Where(filter)
                .Select(properties)
                .TransformUsing(Transformers.AliasToBean<TestSittingInfoDto>())
                .List<TestSittingInfoDto>();


            return result;
        }

        public IEnumerable<TestMaterialApplicantDto> GetApplicantWithAlreadySatMaterials(TestMaterialsSummaryRequest request)
        {

            var testMaterialIds = request.TestMaterialAssignments.Select(x => x.TestMaterialId);
            TestMaterialApplicantDto dto = null;
            var query = NHibernateSession.Current.QueryOver(() => TestSittingTestMaterial)
                .Inner.JoinAlias(x => TestSittingTestMaterial.TestMaterial, () => TestMaterial)
                .Inner.JoinAlias(x => TestSittingTestMaterial.TestSitting, () => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName);


            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), true));
            filter.Add(Restrictions.In(Projections.Property(() => TestMaterial.Id), testMaterialIds.ToArray()));
            filter.Add(Restrictions.Not(InList(Projections.Property(() => TestSession.Id), request.TestSessionIds))); // TODO REVIEW THIS
            filter.Add(Subqueries.WhereProperty<Person>(e => Person.Id).In(PersonIdsInNotCompletedTestSessions(request.TestSessionIds, request.TestSpecificationId, request.SkillId, true)));
            query.Where(filter);

            var properties = new List<IProjection>
            {
                Projections.GroupProperty(Projections.Property(() => TestMaterial.Id)).WithAlias(()=> dto.ConflictingTestMaterialsIds),
                Projections.GroupProperty(Projections.Property(() => Person.Id)),
                Projections.Max(Projections.Property(() => TestSitting.Id)).WithAlias(()=> dto.TestSittingId),
                Projections.Max(Projections.Property(() => TestSession.Id)).WithAlias(()=> dto.PreviousTestSessionId),
                Projections.Max(Projections.Property(() => TestSession.Name)).WithAlias(()=> dto.PreviousTestSessionName),
                Projections.Max(Projections.Property(() => TestSession.TestDateTime)).WithAlias(()=> dto.PreviousTestSessionDate),
                Projections.Max(Projections.Property(() => Entity.NaatiNumber)).WithAlias(()=> dto.NaatiNumber),
                Projections.Max(GetNameProjection()).WithAlias(()=> dto.Name),
                Projections.Max(Projections.Property(()=> Person.Id)).WithAlias(()=> dto.PersonId)
            };

            query.Where(filter).Select(properties.ToArray());
            var indices = properties.Where(x => x.Aliases.Length > 0).ToDictionary(k => k.Aliases.First(), v => properties.FindIndex(value => value == v));

            var result = query.TransformUsing(new TestMaterialApplicantDtoTransformer(indices)).List<TestMaterialApplicantDto>();
            return result;
        }

        public IEnumerable<ExaminerTestMaterialDto> GetApplicantExaminersAndRolePlayers(TestMaterialsSummaryRequest request)
        {
            ExaminerTestMaterialDto dto = null;
            PanelMembershipDto panelMembershipDto = null;
            PanelDto panelDto = null;

            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.PanelMemberships, () => PanelMembership)
                .Inner.JoinAlias(x => PanelMembership.Panel, () => Panel)
                .Inner.JoinAlias(x => PanelMembership.PanelRole, () => PanelRole)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName);


            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filter.Add(Restrictions.Le(Projections.Property(() => PanelMembership.StartDate), DateTime.Now));
            filter.Add(Restrictions.In(Projections.Property(() => TestSession.Id), request.TestSessionIds));
            query.Where(filter);

            var properties = new List<IProjection>
            {
                Projections.Property(() => PanelRole.Name).WithAlias(() => panelMembershipDto.RoleName),
                Projections.Property(() => PanelMembership.StartDate).WithAlias(() => panelMembershipDto.From),
                Projections.Property(() => PanelMembership.EndDate).WithAlias(() => panelMembershipDto.To),
                Projections.Property(() => Panel.Id).WithAlias(() => panelDto.PanelId),
                Projections.Property(() => Panel.Name).WithAlias("PanelName"),
                Projections.Property(() => Entity.NaatiNumber).WithAlias(()=> dto.NaatiNumber),
                GetNameProjection().WithAlias(()=> dto.Name),
            };

            query.Where(filter).Select(properties.ToArray());
            var indices = properties.Where(x => x.Aliases.Length > 0).ToDictionary(k => k.Aliases.First(), v => properties.FindIndex(value => value == v));

            var result = query.TransformUsing(new ExaminerTestMaterialDtoTransformer(indices)).List<ExaminerTestMaterialDto>();
            return result;
        }

        public IEnumerable<ExaminerTestMaterialDto> GetExaminersAndRolePlayers(TestMaterialsSummaryRequest request)
        {
            var examinerMemberships = GetExaminerMemberships(request);
            var examinersQuery = GetExaminerAndRolePlayerQuery();
            var result = ExecuteExaminerAndRolePlayersQuery(examinersQuery, examinerMemberships);

            var rolePlayerMemberships = GetRolePlayerMemberships(request);
            var rolePlayerQuery = GetExaminerAndRolePlayerQuery();
            UnionExaminerAndRolePlayers(result, ExecuteExaminerAndRolePlayersQuery(rolePlayerQuery, rolePlayerMemberships));

            return result.OrderBy(s => s.Name);
        }

        private void UnionExaminerAndRolePlayers(IList<ExaminerTestMaterialDto> dest, IEnumerable<ExaminerTestMaterialDto> source)
        {
            foreach (var s in source)
            {
                var membership = dest.FirstOrDefault(d => d.NaatiNumber == s.NaatiNumber);
                if (membership == null)
                {
                    dest.Add(s);
                    continue;
                }

                var membershipsToAdd = s.PanelMemberships.Where(p => membership.PanelMemberships.Count(pm => pm.Panel.PanelId == p.Panel.PanelId && pm.RoleName == p.RoleName && pm.To == p.To && pm.From == p.From) == 0);
                foreach (var m in membershipsToAdd)
                {
                    membership.PanelMemberships.Add(m);
                }
            }
        }

        private int[] GetExaminerMemberships(TestMaterialsSummaryRequest request)
        {
            var panelMembershipQuery = NHibernateSession.Current.QueryOver(() => TestSession)
                             .Inner.JoinAlias(x => TestSession.TestSittings, () => TestSitting)
                             .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                             .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                             .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                             .Inner.JoinAlias(x => TestSitting.TestResults, () => TestResult)
                             .Inner.JoinAlias(x => TestResult.CurrentJob, () => Job)
                             .Inner.JoinAlias(x => Job.JobExaminers, () => JobExaminer)
                             .Inner.JoinAlias(x => JobExaminer.PanelMembership, () => PanelMembership)
                            .Select(new[] {
                    Projections.Property(() => PanelMembership.Id).WithAlias("Key"),
                            });

            panelMembershipQuery.Where(GetExaminersAndRolePlayersWhere(request));
            var panelMemberships =
                panelMembershipQuery
                .TransformUsing(Transformers.AliasToBean<KeyValuePair<int, int>>())
                .List<KeyValuePair<int, int>>()
                .Select(l => l.Key)
                .Distinct()
                .ToArray();
            return panelMemberships;
        }

        private int[] GetRolePlayerMemberships(TestMaterialsSummaryRequest request)
        {
            var panelRoleCategoryRolePlayer = (int)PanelRoleCategoryName.RolePlayer;
            var panelMembershipQuery = NHibernateSession.Current.QueryOver(() => TestSession)
                             .Inner.JoinAlias(x => TestSession.TestSittings, () => TestSitting)
                             .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                             .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                             .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                             .Inner.JoinAlias(x => Skill.Language2, () => Language2)
                             .Inner.JoinAlias(x => TestSession.TestSessionRolePlayers, () => TestSessionRolePlayer)
                             .Inner.JoinAlias(x => TestSessionRolePlayer.RolePlayer, () => RolePlayer)
                             .Inner.JoinAlias(x => RolePlayer.Person, () => Person)
                             .Inner.JoinAlias(x => Person.PanelMemberships, () => PanelMembership)
                             .Inner.JoinAlias(x => PanelMembership.PanelRole, () => PanelRole)
                             .Inner.JoinAlias(x => PanelMembership.Panel, () => Panel)
                             .Inner.JoinAlias(x => Panel.Language, () => Language, () => Language.Id == Language1.Id || Language.Id == Language2.Id)
                             .Inner.JoinAlias(x => PanelRole.PanelRoleCategory, () => PanelRoleCategory, () => PanelRoleCategory.Id == panelRoleCategoryRolePlayer)
                            .Select(new[] {
                    Projections.Property(() => PanelMembership.Id).WithAlias("Key"),
                            });

            panelMembershipQuery.Where(GetExaminersAndRolePlayersWhere(request));
            return
                panelMembershipQuery
                .TransformUsing(Transformers.AliasToBean<KeyValuePair<int, int>>())
                .List<KeyValuePair<int, int>>()
                .Select(l => l.Key)
                .Distinct()
                .ToArray();
        }

        private IQueryOver<Person, Person> GetExaminerAndRolePlayerQuery()
        {
            return NHibernateSession.Current.QueryOver(() => Person)
                             .Inner.JoinAlias(x => Person.PanelMemberships, () => PanelMembership)
                             .Inner.JoinAlias(x => PanelMembership.Panel, () => Panel)
                             .Inner.JoinAlias(x => PanelMembership.PanelRole, () => PanelRole)
                             .Inner.JoinAlias(x => Person.Entity, () => Entity)
                             .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                             .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName);
        }

        private Conjunction GetExaminersAndRolePlayersWhere(TestMaterialsSummaryRequest request)
        {
            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
            filter.Add(Restrictions.In(Projections.Property(() => TestSession.Id), request.TestSessionIds));
            filter.Add(Restrictions.Le(Projections.Property(() => PanelMembership.StartDate), DateTime.Now));
            filter.Add(Restrictions.Ge(Projections.Property(() => PanelMembership.EndDate), DateTime.Now));
            return filter;
        }

        private IList<ExaminerTestMaterialDto> ExecuteExaminerAndRolePlayersQuery(IQueryOver<Person, Person> query, int[] panelMemberships)
        {
            ExaminerTestMaterialDto dto = null;
            PanelMembershipDto panelMembershipDto = null;
            PanelDto panelDto = null;

            var properties = new List<IProjection>
            {
                Projections.Property(() => PanelRole.Name).WithAlias(() => panelMembershipDto.RoleName),
                Projections.Property(() => PanelMembership.StartDate).WithAlias(() => panelMembershipDto.From),
                Projections.Property(() => PanelMembership.EndDate).WithAlias(() => panelMembershipDto.To),
                Projections.Property(() => Panel.Id).WithAlias(() => panelDto.PanelId),
                Projections.Property(() => Panel.Name).WithAlias("PanelName"),
                Projections.Property(() => Entity.NaatiNumber).WithAlias(() => dto.NaatiNumber),
                GetNameProjection().WithAlias(()=> dto.Name)
            };

            query.Select(properties.ToArray());

            var filter = Restrictions.Conjunction();
            filter.Add(Restrictions.In(Projections.Property(() => PanelMembership.Id), panelMemberships));
            query.Where(filter);

            var indices = properties.Where(x => x.Aliases.Length > 0).ToDictionary(k => k.Aliases.First(), v => properties.FindIndex(value => value == v));

            var result = query.TransformUsing(new ExaminerTestMaterialDtoTransformer(indices)).List<ExaminerTestMaterialDto>();
            return result;
        }

        public IEnumerable<TestMaterialApplicantDto> GetApplicantWithAlreadySatMaterialsForTestSession(int testSessionId)
        {
            var applicants = new Dictionary<int, TestMaterialApplicantDto>();

            var testMaterialIds = NHibernateSession.Current.Query<TestSittingTestMaterial>()
                .Where(x => x.TestSitting.TestSession.Id == testSessionId)
                .Where(x => x.TestSitting.Sat == false)
                .Where(x => x.TestSitting.Rejected == false)
                .Select(x => x.TestMaterial.Id).Distinct().ToList();

            foreach (var testMaterialId in testMaterialIds)
            {
                var personIds = NHibernateSession.Current.Query<TestSittingTestMaterial>()
                    .Where(x => x.TestSitting.TestSession.Id == testSessionId)
                    .Where(x => x.TestSitting.Sat == false)
                    .Where(x => x.TestSitting.Rejected == false)
                    .Where(x => x.TestMaterial.Id == testMaterialId)
                    .Select(x => x.TestSitting.CredentialRequest.CredentialApplication.Person.Id).ToList();

                TestMaterialApplicantDto dto = null;
                var query = NHibernateSession.Current.QueryOver(() => TestSittingTestMaterial)
                    .Inner.JoinAlias(x => TestSittingTestMaterial.TestMaterial, () => TestMaterial)
                    .Inner.JoinAlias(x => TestSittingTestMaterial.TestSitting, () => TestSitting)
                    .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                    .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                    .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                    .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                    .Inner.JoinAlias(x => Person.Entity, () => Entity)
                    .Inner.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                    .Inner.JoinAlias(x => LatestPersonName.PersonName, () => PersonName);

                var filter = Restrictions.Conjunction();
                filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false));
                filter.Add(Restrictions.Eq(Projections.Property(() => TestSitting.Sat), true));
                filter.Add(Restrictions.Eq(Projections.Property(() => TestMaterial.Id), testMaterialId));
                filter.Add(Restrictions.Not(Restrictions.Eq(Projections.Property(() => TestSession.Id), testSessionId)));
                filter.Add(Restrictions.In(Projections.Property(() => Person.Id), personIds.ToArray()));

                var properties = new List<IProjection>
                {
                    Projections.GroupProperty(Projections.Property(() => TestMaterial.Id)).WithAlias(()=> dto.ConflictingTestMaterialsIds),
                    Projections.GroupProperty(Projections.Property(() => Person.Id)),
                    Projections.Max(Projections.Property(() => TestSitting.Id)).WithAlias(()=> dto.TestSittingId),
                    Projections.Max(Projections.Property(() => TestSession.Id)).WithAlias(()=> dto.PreviousTestSessionId),
                    Projections.Max(Projections.Property(() => TestSession.Name)).WithAlias(()=> dto.PreviousTestSessionName),
                    Projections.Max(Projections.Property(() => TestSession.TestDateTime)).WithAlias(()=> dto.PreviousTestSessionDate),
                    Projections.Max(Projections.Property(() => Entity.NaatiNumber)).WithAlias(()=> dto.NaatiNumber),
                    Projections.Max(GetNameProjection()).WithAlias(()=> dto.Name),
                    Projections.Max(Projections.Property(()=> Person.Id)).WithAlias(()=> dto.PersonId)
                };

                query.Where(filter).Select(properties.ToArray());
                var indices = properties.Where(x => x.Aliases.Length > 0).ToDictionary(k => k.Aliases.First(), v => properties.FindIndex(value => value == v));

                var result = query.TransformUsing(new TestMaterialApplicantDtoTransformer(indices)).List<TestMaterialApplicantDto>();
                foreach (var item in result)
                {

                    TestMaterialApplicantDto value;

                    if (!applicants.TryGetValue(item.TestSittingId, out value))
                    {
                        value = item;
                        applicants[item.TestSittingId] = value;
                    }

                    value.ConflictingTestMaterialsIds.Add(testMaterialId);
                }

            }

            return applicants.Values;
        }

        public IEnumerable<TestSittingDetailsDto> GetPeopleWithOtherTestSittingAssingnedForTheSameDay(int testSessionId)
        {
            TestSittingDetailsDto dto = null;
            var testSessionDate = NHibernateSession.Current.Get<TestSession>(testSessionId).TestDateTime.Date;
            var query = NHibernateSession.Current.QueryOver(() => TestSitting)
                .Inner.JoinAlias(x => TestSitting.TestSession, () => TestSession)
                .Inner.JoinAlias(x => TestSitting.CredentialRequest, () => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => Person.Entity, () => Entity)
                .Where(Restrictions.Eq(Projections.Property(() => TestSitting.Rejected), false))
                .Where(Subqueries.WhereProperty<Person>(e => Person.Id)
                    .In(PersonIdsInNotCompletedTestSessions(new[] { testSessionId })))
                .Where(Restrictions.Not(Restrictions.Eq(Projections.Property(() => TestSession.Id), testSessionId)))
                .Where(Restrictions.Eq(GetDateProjectionFrom(Projections.Property(() => TestSession.TestDateTime)),
                    testSessionDate));


            var testSittingProperty = Projections.Property(() => TestSitting.Id);
            var testSessionProperty = Projections.Property(() => TestSession.Id);
            var naatiNumberProperty = Projections.Property(() => Entity.NaatiNumber);

            var properties = Projections.ProjectionList()
                .Add(testSittingProperty.WithAlias(() => dto.TestSittingId))
                .Add(testSessionProperty.WithAlias(() => dto.TestSessionId))
                .Add(naatiNumberProperty.WithAlias(() => dto.NaatiNumber));

            var result = query.Select(properties)
                .TransformUsing(Transformers.AliasToBean<TestSittingDetailsDto>())
                .List<TestSittingDetailsDto>();

            return result;
        }

        public IEnumerable<ApplicationBriefDto> FilterOutSupplementaryMaterialsThatPassed(IEnumerable<ApplicationBriefDto> allBriefs)
        {
            var filteredBriefs = allBriefs.ToList();
            foreach (var brief in allBriefs)
            {
                var filteredBrief = brief;

                if (brief.Supplementary)
                {
                    //remove components
                    var filteredComponentResultBriefs = new List<CandidateBriefFileInfoDto>();

                    //call sp to get details of acxtual test and what components passed
                    var initialTestResultForSuplementaryDtos = NHibernateSession.Current.TransformSqlQueryDataRowResult<InitialTestResultForSuplementaryDto>($"exec SupplementaryMaterialsFilterTestComponentsThatPassed {brief.CredentialRequestId}").ToList();

                    foreach(var componentResult in brief.Briefs)
                    {
                        if(initialTestResultForSuplementaryDtos.Any(x=>x.TestComponentId == componentResult.TestComponentId && x.Successful))
                        {
                            //filter it out
                            continue;
                        }
                        filteredComponentResultBriefs.Add(componentResult);
                    }
                    filteredBrief.Briefs = filteredComponentResultBriefs;
                }

                filteredBriefs.RemoveAll(x=>x.CredentialApplicationId == brief.CredentialApplicationId && x.CredentialRequestId == brief.CredentialRequestId);
                filteredBriefs.Add(filteredBrief);

            }

            return filteredBriefs;
        }


        public IEnumerable<ApplicationBriefDto> GetPendingCandidateBriefsToSend(PendingBriefRequest request)
        {
            TestSittingTestMaterial testSittingTestMaterial = null;
            TestMaterial testMaterial = null;
            TestMaterialAttachment testMaterialAttachment = null;
            CandidateBrief candidateBrief = null;
            StoredFile storedFile = null;
            TestComponent testComponent = null;
            TestComponentType testComponentType = null;
            CredentialRequest credentialRequest = null;
            CredentialRequestStatusType credentialRequestStatusType = null;
            TestSession testSession = null;
            TestSitting testSitting = null;


            var testDate = GetDateProjectionFrom(Projections.Property(() => testSession.TestDateTime));
            var availableDays = MultiplyProjections(Projections.Property(() => testComponentType.CandidateBriefAvailabilityDays), Projections.Constant(-1));
            var availabilityDate = AddDays(availableDays, testDate);
            var currentDate = Projections.Constant(request.SendDate.Date, NHibernateUtil.Date);

            var restrictions = Restrictions.Conjunction()
                .Add(Restrictions.Eq(Projections.Property(() => testSitting.Rejected), false))
                .Add(Restrictions.Eq(Projections.Property(() => testSitting.Sat), false))
                .Add(Restrictions.Eq(Projections.Property(() => credentialRequestStatusType.Id), (int)request.CredentialRequestStatus))
                .Add(Restrictions.Eq(Projections.Property(() => testComponentType.CandidateBriefRequired), true))
                .Add(Restrictions.GeProperty(currentDate, availabilityDate))
                .Add(Restrictions.Eq(Projections.Property(() => testMaterialAttachment.Deleted), false))
                .Add(Restrictions.Eq(Projections.Property(() => storedFile.DocumentType.Id), (int)StoredFileType.CandidateBrief))
                .Add(Restrictions.Or(Restrictions.IsNull(Projections.Property(() => candidateBrief.Id)), Restrictions.IsNull(Projections.Property(() => candidateBrief.EmailedDate))));

            if (request.TestSittingId.HasValue)
            {
                restrictions.Add(Restrictions.Eq(Projections.Property(() => testSitting.Id), request.TestSittingId.GetValueOrDefault()));
            }

            var query = NHibernateSession.Current.QueryOver(() => testSittingTestMaterial)
                .Inner.JoinAlias(x => x.TestSitting, () => testSitting)
                .Inner.JoinAlias(x => testSitting.TestSession, () => testSession)
                .Inner.JoinAlias(x => x.TestMaterial, () => testMaterial)
                .Inner.JoinAlias(x => x.TestComponent, () => testComponent)
                .Inner.JoinAlias(x => testSitting.CredentialRequest, () => credentialRequest)
                .Inner.JoinAlias(x => credentialRequest.CredentialRequestStatusType, () => credentialRequestStatusType)
                .Inner.JoinAlias(x => testComponent.Type, () => testComponentType)
                .Inner.JoinAlias(x => testMaterial.TestMaterialAttachments, () => testMaterialAttachment)
                .Inner.JoinAlias(x => testMaterialAttachment.StoredFile, () => storedFile)
                .Left.JoinAlias(x => testMaterialAttachment.CandidateBriefs, () => candidateBrief, () => testSitting.Id == candidateBrief.TestSitting.Id)
                .Where(restrictions);

            query.Select(Projections.ProjectionList()
                .Add(Projections.GroupProperty(Projections.Property(() => credentialRequest.Id)))
                .Add(Projections.GroupProperty(Projections.Property(() => storedFile.Id)))
                .Add(Projections.Max(Projections.Property(() => credentialRequest.CredentialApplication.Id)))
                .Add(Projections.Max(Projections.Property(() => candidateBrief.Id)))
                .Add(Projections.Max(Projections.Property(() => testComponent.Label)))
                .Add(Projections.Max(Projections.Property(() => testComponentType.Label)))
                .Add(Projections.Max(Projections.Property(() => testMaterial.Id)))
                .Add(Projections.Max(Projections.Property(() => testMaterialAttachment.Id)))
                .Add(Projections.GroupProperty(Projections.Property(() => testSitting.Supplementary)))
                .Add(Projections.Max(Projections.Property(() => testComponent.Id)))
                );

            var result = query.List<IList>();

            var data = result.GroupBy(x => x[0])
                .Select(y =>
                {
                    var pendingBrief = new ApplicationBriefDto
                    {
                        CredentialRequestId = (int)y.Key,
                        CredentialApplicationId = (int)y.First()[2],
                        Supplementary = (bool)y.First()[8],
                        Briefs = y.Select(b => new CandidateBriefFileInfoDto
                        {
                            StorageFileId = (int)b[1],
                            CandidateBriefId = (int?)b[3],
                            TaskLabel = b[4] as string,
                            TaskTypeLabel = b[5] as string,
                            TestMaterialId = (int)b[6],
                            TestMaterialAttachmentId = (int)b[7],
                            TestComponentId = (int)b[9],
                        })
                    };
                    return pendingBrief;
                });

            return data;
        }

        public IEnumerable<TestMaterialSearchDto> SearchTestMaterials(TestMaterialSearchRequest request)

        {
            var filterDictionary = new Dictionary<TestMaterialFilterType, Func<TestMaterialSearchCriteria, Junction, Junction>>
            {
                [TestMaterialFilterType.MaterialIdIntList] = (criteria, previousJunction) => GetMaterialIdFilter(criteria, previousJunction),
                [TestMaterialFilterType.LanguageIntList] = (criteria, previousJunction) => GetLanguageFilter(criteria, previousJunction),
                [TestMaterialFilterType.CredentialTypeIntList] = (criteria, previousJunction) => GetCredentialTypeFilter(criteria, previousJunction),
                [TestMaterialFilterType.TaskTypeIntList] = (criteria, previousJunction) => GetTaskTypeFilter(criteria, previousJunction),
                [TestMaterialFilterType.AvailabilityBoolean] = (criteria, previousJunction) => GetAvailabilityFilter(criteria, previousJunction),
                [TestMaterialFilterType.TitleString] = (criteria, previousJunction) => GetTitleFilter(criteria, previousJunction),
                [TestMaterialFilterType.TestMaterialStatusIntList] = (criteria, previousJunction) => GetTestMaterialStatusFilter(criteria, previousJunction),
                [TestMaterialFilterType.TestSpecificationIntList] = (criteria, previousJunction) => GetTestSpecificationFilter(criteria, previousJunction),
                [TestMaterialFilterType.TestMaterialTypeIntList] = (criteria, previousJunction) => GetTestMaterialTypeFilter(criteria, previousJunction),
                [TestMaterialFilterType.TestMaterialDomainIntList] = (criteria, previousJunction) => GetTestMaterialDomainFilter(criteria, previousJunction),
                [TestMaterialFilterType.SourceTestMaterialIdIntList] = (criteria, previousJunction) => GetSourceTestMaterialIdFilter(criteria, previousJunction)
            };

            Junction junction = null;

            var validCriterias = request.Filter.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                junction = Restrictions.Conjunction();
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = filterDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }

            var queryOver = BuildQuery();

            if (request.Skip.HasValue)
            {
                queryOver.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                queryOver.Take(request.Take.Value);
            }

            if (junction != null)
            {
                queryOver = queryOver.Where(junction);
            }

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections.ToArray());
            var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<TestMaterialSearchDto>());
            var resultList = searchResult.List<TestMaterialSearchDto>();
            return resultList;
        }

        protected override Junction GetLanguageFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var langugeIds = criteria.ToList<S, int>();

            var languageBaseTypes = Restrictions.Conjunction();
            languageBaseTypes.Add(Restrictions.Eq(Projections.Property(() => TestComponentBaseType.Id), (int)TestComponentBaseTypeName.Language));
            languageBaseTypes.Add(Restrictions.In(Projections.Property(() => TestMaterial.Language.Id), langugeIds));

            var skillBaseTypes = Restrictions.Conjunction();
            skillBaseTypes.Add(Restrictions.Eq(Projections.Property(() => TestComponentBaseType.Id), (int)TestComponentBaseTypeName.Skill));
            skillBaseTypes.Add(Restrictions.In(Projections.Property(() => Skill.Language1.Id), langugeIds));

            var filter = Restrictions.Disjunction()
                .Add(languageBaseTypes)
                .Add(skillBaseTypes);

            return junction.Add(filter);
        }

        protected Junction GetCredentialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var credentialTypeIds = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => CredentialType.Id), credentialTypeIds);

            return junction.Add(filter);
        }

        protected Junction GetTaskTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var taskTypeIds = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => TestComponentType.Id), taskTypeIds);

            return junction.Add(filter);
        }

        protected Junction GetTestMaterialTypeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterIds = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => TestMaterialType.Id), filterIds);

            return junction.Add(filter);
        }
        protected Junction GetTestMaterialDomainFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterIds = criteria.ToList<S, int>();

            var filter = Restrictions.In(Projections.Property(() => TestMaterialDomain.Id), filterIds);

            return junction.Add(filter);
        }

        protected Junction GetSourceTestMaterialIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterIds = criteria.ToList<S, int>();


            var sourceMaterialNotNull = Restrictions.IsNotNull(Projections.Property(() => SourceMaterial.Id));
            var filter = Restrictions.In(Projections.Property(() => SourceMaterial.Id), filterIds);

            return junction.Add(Restrictions.And(sourceMaterialNotNull, filter));
        }

        protected Junction GetAvailabilityFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValue = criteria.ToList<S, bool>().First();
            var filter = Restrictions.Eq(Projections.Property(() => TestMaterial.Available), filterValue);
            return junction.Add(filter);
        }
        protected Junction GetTestMaterialStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValues = criteria.ToList<S, int>();

            Disjunction filter = Restrictions.Disjunction();

            foreach (var filterValue in filterValues)
            {
                switch (filterValue)
                {
                    case (int)TestMaterialStatusTypeName.New:
                        var newMaterial = Restrictions.EqProperty(GetGeneralTestMaterialStatus(new[] { 0 }),
                            GetTestMaterialStatusProjection(TestMaterialStatusTypeName.New));
                        filter.Add(newMaterial);
                        break;
                    case (int)TestMaterialStatusTypeName.ToBeUsed:
                        filter.Add(Restrictions.EqProperty(GetGeneralTestMaterialStatus(new[] { 0 }), GetTestMaterialStatusProjection(TestMaterialStatusTypeName.ToBeUsed)));
                        break;
                    case (int)TestMaterialStatusTypeName.PreviouslyUsed:
                        filter.Add(Restrictions.EqProperty(GetGeneralTestMaterialStatus(new[] { 0 }), GetTestMaterialStatusProjection(TestMaterialStatusTypeName.PreviouslyUsed)));
                        break;

                    default:
                        throw new Exception($"Test Material Status {filterValue} not supported");

                }
            }
            return junction.Add(filter);
        }
        protected Junction GetTestSpecificationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var testSpecificationIds = criteria.ToList<S, int>();
            var filter = Restrictions.In(Projections.Property(() => TestSpecification.Id), testSpecificationIds);
            return junction.Add(filter);
        }
        protected Junction GetTitleFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var stringValue = criteria.Values.FirstOrDefault()?.Trim();

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return junction;
            }

            var filter = Restrictions.InsensitiveLike(Projections.Property(() => TestMaterial.Title), stringValue, MatchMode.Anywhere);
            return junction.Add(filter);
        }

        protected Junction GetMaterialIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var materialIds = criteria.ToList<S, int>();
            junction.Add<TestMaterial>(c => TestMaterial.Id.IsIn(materialIds));
            return junction;
        }

        protected IProjection GetMaterialDirectionProjection()
        {
            var isSkillBased = Restrictions.Eq(Projections.Property(() => TestComponentBaseType.Id), (int)TestComponentBaseTypeName.Skill);

            return Projections.Conditional(isSkillBased, GetDirectionProjection(),
                 Projections.Property(() => TestMateriaLanguage.Name));
        }

        private IProjection GetHasFileProjection()
        {
            TestMaterialAttachment mAttachment = null;
            var query = QueryOver.Of(() => mAttachment)
                .Where(x => !x.Deleted)
                .Select(Projections.Distinct(Projections.Property(() => mAttachment.TestMaterial.Id)));

            var resstriction = Subqueries.WhereProperty<TestMaterial>(x => TestMaterial.Id).In(query);
            return GetBooleanProjectionFor(resstriction);
        }

        private IQueryOver<TestMaterial, TestMaterial> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => TestMaterial)
                .Inner.JoinAlias(x => TestMaterial.TestComponentType, () => TestComponentType)
                .Inner.JoinAlias(x => TestMaterial.TestMaterialType, () => TestMaterialType)
                .Inner.JoinAlias(x => TestMaterial.TestMaterialDomain, () => TestMaterialDomain)
                .Inner.JoinAlias(x => TestComponentType.TestSpecification, () => TestSpecification)
                .Inner.JoinAlias(x => TestSpecification.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => TestComponentType.TestComponentBaseType, () => TestComponentBaseType)
                .Inner.JoinAlias(x => TestMaterial.TestMaterialLastUsed, () => TestMaterialLastUsed)
                .Left.JoinAlias(x => TestMaterial.MaterialRequests, () => MaterialRequest)
                .Left.JoinAlias(x => MaterialRequest.SourceMaterial, () => SourceMaterial)
                .Left.JoinAlias(x => TestMaterial.Skill, () => Skill)
                .Left.JoinAlias(x => Skill.Language1, () => Language1)
                .Left.JoinAlias(x => Skill.Language2, () => Language2)
                .Left.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Left.JoinAlias(x => TestMaterial.Language, () => TestMateriaLanguage);

            //Ticket 201036 set timeout to 480 seconds
            queryOver.UnderlyingCriteria.SetTimeout(480);

            return queryOver;
        }

        private List<IProjection> BuildProjections()
        {
            TestMaterialSearchDto dto = null;
            return new List<IProjection>
            {
                Projections.Property(() => TestMaterial.Id).WithAlias(() => dto.Id),
                Projections.Property(() => TestMaterial.Title).WithAlias(() => dto.Title),
                GetMaterialDirectionProjection().WithAlias(() => dto.LanguageOrSkill),
                Projections.Property(() => TestMateriaLanguage.Id).WithAlias(() => dto.LanguageId),
                Projections.Property(() => CredentialType.InternalName).WithAlias(() => dto.CredentialType),
                Projections.Property(() => CredentialType.Id).WithAlias(() => dto.CredentialTypeId),
                Projections.Property(() => TestComponentType.Id).WithAlias(() => dto.TestComponentTypeId),
                Projections.Property(() => TestComponentType.Name).WithAlias(() => dto.TaskType),
                GetHasFileProjection().WithAlias(() => dto.HasFile),
                Projections.Property(() => TestMaterial.Available).WithAlias(() => dto.Available),
                Projections.Property(() => TestSpecification.Active).WithAlias(() => dto.TestSpecificationActive),
                Projections.Property(() => TestSpecification.Id).WithAlias(() => dto.TestSpecificationId),
                Projections.Property(() => Skill.Id).WithAlias(() => dto.SkillId),
                GetGeneralTestMaterialStatus(new []{0}).WithAlias(() => dto.StatusId),
                Projections.Property(() => TestMaterialLastUsed.LastUsedDate).WithAlias(() => dto.LastUsedDate),
                Projections.Property(() => TestMaterialDomain.Id).WithAlias(() => dto.TestMaterialDomainId),
                Projections.Property(() => SourceMaterial.Id).WithAlias(() => dto.SourceMaterialId),
                Projections.Property(() => SourceMaterial.Title).WithAlias(() => dto.SourceMaterialTitle)
                
            };
        }
    }

    

    public class SessionDateForSkill
    {
        public int SkillId { get; set; }
        public DateTime Date { get; set; }
    }

    public class TestMaterialStatus
    {
        public int TestMaterialId { get; set; }
        public int StatusId { get; set; }
    }
}
