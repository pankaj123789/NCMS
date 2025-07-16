using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using F1Solutions.Global.Common.Logging;
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
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{

    public class PanelQueryService : IPanelQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;
        private const int MaxPanelNameLength = 100;
        private const int MaxPanelNoteLength = 500;

        public PanelQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public GetPanelsResponse GetPanels(GetPanelsRequest request)
        {
            var query = NHibernateSession.Current.Query<Panel>();
            var panelIds = FilterPanelsByMembership(request);
            var panelIdsForMaterialRequests = GetPanelIdsForMaterialRequesFilters(request).Concat(new[] { 0 }).ToList();

            query = FilterQuery(query, request.LanguageId, panel => request.LanguageId.Contains(panel.Language.Id));
            query = FilterQuery(query, request.IsVisibleInEportal, panel => request.IsVisibleInEportal == panel.VisibleInEportal);
            query = FilterQuery(query, request.PanelId, panel => request.PanelId == panel.Id);
            query = FilterQuery(query.ToList().AsQueryable(), panelIds, panel => panelIds.Contains(panel.Id));
            query = FilterQuery(query, panelIdsForMaterialRequests, panel => panelIdsForMaterialRequests.Contains(panel.Id));


            return new GetPanelsResponse
            {
                Panels = query.OrderByDescending(x => x.Id).Select(MapPanel).ToArray()
            };
        }

        private IList<int> GetPanelIdsForMaterialRequesFilters(GetPanelsRequest request)
        {
            Panel mPanel = null;
            Domain.MaterialRequest mMaterialRequest = null;
            TestMaterial mOuputTestMaterial = null;
            TestComponentType mtestComponentType = null;
            TestSpecification mTestSpecification = null;

            var query = NHibernateSession.Current.QueryOver(() => mMaterialRequest)
                .Right.JoinAlias(x => mMaterialRequest.Panel, () => mPanel)
                .Left.JoinAlias(x => mMaterialRequest.OutputMaterial, () => mOuputTestMaterial)
                .Left.JoinAlias(x => mOuputTestMaterial.TestComponentType, () => mtestComponentType)
                .Left.JoinAlias(x => mtestComponentType.TestSpecification, () => mTestSpecification);

            query = query.Where(Restrictions.IsNotNull(Projections.Property(() => mPanel.Id)));
            query = TryAddFilter(query, Projections.Property(() => mTestSpecification.CredentialType.Id), request.MaterialRequestCredentialTypeId);
            query = TryAddFilter(query, Projections.Property(() => mtestComponentType.Id), request.MaterialRequestTaskTypeId);
            query = TryAddFilter(query, Projections.Property(() => mMaterialRequest.MaterialRequestStatusType.Id), request.MaterialRequestStatusId);

            if (!string.IsNullOrWhiteSpace(request.MaterialRequestTitle))
            {
                query = query.Where(Restrictions.IsNotNull(Projections.Property(() => mOuputTestMaterial.Title)));
                query = query.Where(Restrictions.Like(Projections.Property(() => mOuputTestMaterial.Title), request.MaterialRequestTitle, MatchMode.Anywhere));
            }

            return query.Select(Projections.Distinct(Projections.Property(() => mPanel.Id))).List<int>();

        }


        private IQueryOver<Domain.MaterialRequest, Domain.MaterialRequest> TryAddFilter(IQueryOver<Domain.MaterialRequest, Domain.MaterialRequest> query, IProjection projection, int[] filters)
        {
            if (filters?.Any() ?? false)
            {
                var filter = Restrictions.Conjunction()
                    .Add(Restrictions.IsNotNull(projection))
                    .Add(Restrictions.In(projection, filters));
                query = query.Where(filter);
            }

            return query;

        }

        private static IList<int> FilterPanelsByMembership(GetPanelsRequest request)
        {
            var query = NHibernateSession.Current.Query<PanelMembership>();
            bool appliedQuery;

            query = FilterQuery(query, request.NAATINumber, panelMembership => request.NAATINumber.Contains(panelMembership.Person.Entity.NaatiNumber), out appliedQuery);
            var applyQuery = appliedQuery;

            query = FilterQuery(query, request.StartDate, panelMembership => panelMembership.StartDate <= request.StartDate, out appliedQuery);
            applyQuery = applyQuery || appliedQuery;

            query = FilterQuery(query, request.EndDate, panelMembership => panelMembership.EndDate >= request.EndDate, out appliedQuery);
            applyQuery = applyQuery || appliedQuery;

            query = FilterQuery(query, request.RoleCategoryIds, panelMembership => request.RoleCategoryIds.Contains(panelMembership.PanelRole.PanelRoleCategory.Id), out appliedQuery);
            applyQuery = applyQuery || appliedQuery;

            query = FilterQuery(query, request.Chair, panelMembership => request.Chair == panelMembership.PanelRole.Chair, out appliedQuery);
            applyQuery = applyQuery || appliedQuery;

            var ids = new List<int>();

            if (applyQuery)
            {
                ids = query.Select(pm => pm.Panel.Id).ToList();
                ids.Add(-1);
            }

            return ids;
        }

        public ServiceResponse<int> CreateOrUpdatePanel(CreateOrUpdatePanelRequest request)
        {
            var response = new ServiceResponse<int>();
            if (request.Name.Length > MaxPanelNameLength)
            {
                response.ErrorMessage = $"Panel name exceeds max character length of {MaxPanelNameLength}";
                return response;
            }
            if (request.Note.Length > MaxPanelNoteLength)
            {
                response.ErrorMessage = $"Panel note exceeds max character length of {MaxPanelNoteLength}";
                return response;
            }

            var panel = NHibernateSession.Current.Get<Panel>(request.PanelId.GetValueOrDefault()) ?? new Panel();
            var existingPanelName = NHibernateSession.Current.Query<Panel>().Count(x => x.Name == request.Name && x.Id != request.PanelId.GetValueOrDefault()) > 0;
            if (existingPanelName)
            {
                response.ErrorMessage = $"A panel with the name {request.Name} already exists";
                return response;
            }
            panel.Name = request.Name;

            panel.PanelType = NHibernateSession.Current.Load<PanelType>(request.PanelTypeId);
            panel.Language = NHibernateSession.Current.Get<Language>(request.LanguageId.GetValueOrDefault());

            if (panel.PanelType.AllowCredentialTypeLink && panel.Language == null)
            {
                response.ErrorMessage = "A language panel must be assigned a language";
                LoggingHelper.LogError(response.ErrorMessage);
                response.Error = true;
                return response;
            }
            panel.Note = request.Note;
            panel.CommissionedDate = request.ComissionedDate;
            if (panel.CommissionedDate < (DateTime)SqlDateTime.MinValue)
            {
                panel.CommissionedDate = (DateTime)SqlDateTime.MinValue;
            }
            if (request.ComissionedDate > (DateTime)SqlDateTime.MaxValue)
            {
                panel.CommissionedDate = (DateTime)SqlDateTime.MaxValue;
            }
            panel.VisibleInEportal = request.VisibleInEportal;

            NHibernateSession.Current.Save(panel);
            NHibernateSession.Current.Flush();
            return new ServiceResponse<int> { Data = panel.Id };
        }

        public DeletePanelResponse DeletePanel(DeletePanelRequest request)
        {
            var panelQuery = NHibernateSession.Current.Query<Panel>();
            var panel = panelQuery.SingleOrDefault(x => x.Id == request.PanelId);

            if (panel == null)
            {
                throw new WebServiceException("Referenced panel does not exist");
            }

            if (!panel.PanelMemberships.All(ValidateMembershipDelete))
            {
                throw new WebServiceException("Referenced panel has an examiner panel membership, with a job assigned");
            }

            foreach (var membership in panel.PanelMemberships)
            {
                NHibernateSession.Current.Delete(membership);
            }

            var response = new DeletePanelResponse
            {
                Panel = MapPanel(panel)
            };

            NHibernateSession.Current.Delete(panel);
            NHibernateSession.Current.Flush();

            return response;
        }

        private static PanelDto MapPanel(Panel panel)
        {
            return new PanelDto
            {
                PanelId = panel.Id,
                LanguageId = panel.Language?.Id,
                Language = panel.Language?.Name,
                Name = panel.Name,
                Note = panel.Note,
                PanelTypeId = panel.PanelType.Id,
                PanelType = panel.PanelType.Name,
                ComissionedDate = panel.CommissionedDate,
                HasCurrentMembers = panel.PanelMemberships.Any(x => x.EndDate >= DateTime.Now),
                HasExaminersAllocated = !panel.PanelMemberships.All(ValidateMembershipDelete),
                VisibleInEportal = panel.VisibleInEportal,
            };
        }

        public ValidateExaminerSecurityCodeResponse ValidateExaminerSecurityCode(ValidateExaminerSecurityCodeRequest request)
        {
            var response = new ValidateExaminerSecurityCodeResponse();
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == request.NAATINumber);

            if (person == null)
            {
                response.Valid = false;
                return response;
            }

            response.Valid = person.ExaminerSecurityCode == request.SecurityCode;
            return response;
        }

        public GetMembershipsResponse GetMemberships(GetMembershipsRequest request)
        {
            var panelMembershipQuery = NHibernateSession.Current.Query<PanelMembership>();
            var testQuery = NHibernateSession.Current.Query<TestSitting>();
            var jobQuery = NHibernateSession.Current.Query<Job>();
            var examinerUnavailableQuery = NHibernateSession.Current.Query<ExaminerUnavailable>();

            panelMembershipQuery = panelMembershipQuery.Where(p => p.Panel.Id == request.PanelId);
            panelMembershipQuery = FilterQuery(panelMembershipQuery, request.StartDate, panelMembership => panelMembership.StartDate <= request.StartDate);
            panelMembershipQuery = FilterQuery(panelMembershipQuery, request.EndDate, panelMembership => panelMembership.EndDate >= request.EndDate);
            panelMembershipQuery = FilterQuery(panelMembershipQuery, request.MembershipId, panelMembership => panelMembership.Id == request.MembershipId);

            var queryResults =
                (from pm in panelMembershipQuery
                 select new
                 {
                     pm.Person,
                     pm.Person.Entity,
                     pm.Panel,
                     PanelRole = pm.PanelRole,
                     Membership = pm,
                     HasUnavailability = examinerUnavailableQuery.Any(eu => eu.Person.Id == pm.Person.Id && eu.EndDate >= DateTime.Now),
                     HasMarkingRequests =
                         (from ta in testQuery
                          from tr in ta.TestResults
                          join j in jobQuery on tr.CurrentJob.Id equals j.Id
                          from je in j.JobExaminers
                          where je.PanelMembership.Id == pm.Id
                          select je).Any(),
                     HasMaterialRequests =
                         (from j in jobQuery
                          from je in j.JobExaminers
                          where pm.StartDate <= DateTime.Now.Date
                               && DateTime.Now.Date <= pm.EndDate
                               && pm.PanelRole.PanelRoleCategory.Id == (int)PanelRoleCategoryName.Examiner
                               && j.JobCategory == 0 && je.PanelMembership.Id == pm.Id && je.Status != JobExaminerStatus.Submitted && j.SettingMaterial != null
                          select je).Any()
                 }).ToArray();

            var panelMembershipSummary = GetPanelMembershipSummary(request.PanelId);

            var panelMembers = queryResults.Select(x =>
            {
                var contactEmail =
                    x.Entity.Emails.FirstOrDefault(y => y.ExaminerCorrespondence && !y.Invalid)?.EmailAddress ?? x.Entity.PrimaryEmail.EmailAddress;
                var contactPhone =
                    x.Entity.Phones.FirstOrDefault(y => y.ExaminerCorrespondence && !y.Invalid)?.Number ??
                    x.Entity.Phones.FirstOrDefault(y => y.PrimaryContact && !y.Invalid)?.Number ?? "";

                var credentialTypeIds = x.Membership.PanelMembershipCredentialTypes.Select(c => c.CredentialType.Id).ToList();
                var materialCredentialTypes = x.Membership.PanelMembershipMaterialCredentialTypes.Select(c => c.CredentialType.Id).ToList();
                var coordinatorCredentialTypes = x.Membership.PanelMembershipCoordinatorCredentialTypes.Select(c => c.CredentialType.Id).ToList();

                return new MembershipDto
                {
                    PanelMembershipId = x.Membership.Id,
                    PersonId = x.Person.Id,
                    NAATINumber = x.Person.Entity.NaatiNumber,
                    Name = x.Person.FullName,
                    PanelId = x.Panel.Id,
                    PanelName = x.Panel.Name,
                    PanelRoleId = x.PanelRole.Id,
                    PanelRole = x.PanelRole.Name,
                    StartDate = x.Membership.StartDate,
                    EndDate = x.Membership.EndDate,
                    Phone = contactPhone,
                    Email = contactEmail,
                    HasUnavailability = x.HasUnavailability,
                    HasMarkingRequests = x.HasMarkingRequests,
                    HasMaterialRequests = x.HasMaterialRequests,
                    CredentialTypeIds = credentialTypeIds,
                    MaterialCredentialTypeIds = materialCredentialTypes,
                    CoordinatorCredentialTypeIds = coordinatorCredentialTypes,
                    PanelRoleCategory = (PanelRoleCategoryName)x.Membership.PanelRole.PanelRoleCategory.Id,

                    Overdue = panelMembershipSummary[x.Membership.Person.Id].Overidue,
                    InProgress = panelMembershipSummary[x.Membership.Person.Id].InProgress,
                    UnAvailableExaminers = x.Person.ExaminerUnavailable.Select(y => new ExaminerUnavailableContract
                    {
                        Id = y.Id,
                        StartDate = y.StartDate,
                        EndDate = y.EndDate
                    }).ToList()

                };
            }).ToArray();

            foreach (var panelMember in panelMembers)
            {
                panelMember.IsExaminerRole = panelMember.PanelRoleCategory == PanelRoleCategoryName.Examiner;
            }

            return new GetMembershipsResponse
            {
                People = panelMembers
            };
        }

        public ServiceResponse<IEnumerable<PanelMembershipInfoDto>> GetPanelMembershipInfo(PanelMembershipInfoRequest request)
        {
            var results = NHibernateSession.Current.Query<PanelMembership>().Where(x => request.PanelMembershipIds.Contains(x.Id)).ToList()
                .Select(y =>
                {
                    var email = y.Person.Entity.Emails.FirstOrDefault(x => x.IsPreferredEmail && !x.Invalid)
                        ?.EmailAddress;
                    var dto = new PanelMembershipInfoDto()
                    {
                        PanelMemberShipId = y.Id,
                        NaatiNumber = y.Person.Entity.NaatiNumber,
                        GivenName = y.Person.LatestPersonName.PersonName.GivenName + ' ' + y.Person.LatestPersonName.PersonName.Surname,
                        PrimaryEmail = email,
                        EntityId = y.Person.Entity.Id
                    };
                    return dto;
                }).ToList();

            return new ServiceResponse<IEnumerable<PanelMembershipInfoDto>>() { Data = results };
        }


        private Dictionary<int, PanelMembershipSummaryDto> GetPanelMembershipSummary(int panelId)
        {
            PanelMembershipSummaryDto dto = null;
            JobExaminer jobExaminer = null;
            Job job = null;
            PanelMembership panelMembership = null;
            Panel panel = null;
            PanelRole panelRole = null;
            PanelRoleCategory panelRoleCategory = null;
            ExaminerMarking examinerMarking = null;
            Person person = null;
            TestResult testResult = null;


            var currentDate = Projections.Constant(DateTime.Now.Date, NHibernateUtil.Date);

            var overdueCondition = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(Projections.Property(() => jobExaminer.Id)))
                .Add(Restrictions.Eq(Projections.Property(() => panelRoleCategory.Id), (int)PanelRoleCategoryName.Examiner))
                .Add(Restrictions.IsNull(Projections.Property(() => jobExaminer.ExaminerReceivedDate)))
                .Add(Restrictions.GtProperty(currentDate, Projections.Property(() => job.DueDate)))
                .Add(Restrictions.Eq(Projections.Property(() => testResult.ResultChecked), false));

            var overdueProjection = Projections.Sum(Projections.Conditional(overdueCondition, Projections.Constant(1, NHibernateUtil.Int32), Projections.Constant(0, NHibernateUtil.Int32)));


            var inProgressCondition = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(Projections.Property(() => jobExaminer.Id)))
                .Add(Restrictions.Eq(Projections.Property(() => panelRoleCategory.Id), (int)PanelRoleCategoryName.Examiner))
                .Add(Restrictions.IsNull(Projections.Property(() => jobExaminer.ExaminerReceivedDate)))
                .Add(Restrictions.LeProperty(currentDate, Projections.Property(() => job.DueDate)));

            var inProgressProjection = Projections.Sum(Projections.Conditional(inProgressCondition, Projections.Constant(1, NHibernateUtil.Int32), Projections.Constant(0, NHibernateUtil.Int32)));

            var personIds = QueryOver.Of<PanelMembership>().Where(x => x.Panel.Id == panelId).Select(y => y.Person.Id);
            var personGroupFilter = Subqueries.WhereProperty<PanelMembership>(e => person.Id).In(personIds);

            var panelMemberShipId = Projections.Max(
                Projections.Conditional(
                Restrictions.Eq(Projections.Property(() => panel.Id), panelId), Projections.Property(() => panelMembership.Id), Projections.Constant(0, NHibernateUtil.Int32)));

            var query = NHibernateSession.Current.QueryOver(() => panelMembership)
                .Inner.JoinAlias(x => panelMembership.Person, () => person)
                .Left.JoinAlias(x => panelMembership.JobExaminers, () => jobExaminer)
                .Left.JoinAlias(x => jobExaminer.Job, () => job)
                .Left.JoinAlias(x => panelMembership.Panel, () => panel)
                .Left.JoinAlias(x => panelMembership.PanelRole, () => panelRole)
                .Left.JoinAlias(x => panelRole.PanelRoleCategory, () => panelRoleCategory)
                .Left.JoinAlias(x => jobExaminer.Markings, () => examinerMarking)
                .Left.JoinAlias(x => job.TestResults, () => testResult)
                .Select(Projections.ProjectionList()
                    .Add(Projections.GroupProperty(Projections.Property(() => person.Id)).WithAlias(() => dto.PersonId))
                    .Add(panelMemberShipId.WithAlias(() => dto.PanelMembershipId))
                    .Add(inProgressProjection.WithAlias(() => dto.InProgress))
                    .Add(overdueProjection.WithAlias(() => dto.Overidue))
                )
                .Where(personGroupFilter);


            var result = query.TransformUsing(Transformers.AliasToBean<PanelMembershipSummaryDto>()).List<PanelMembershipSummaryDto>();

            var panelMembershipSummaryDictionary = new Dictionary<int, PanelMembershipSummaryDto>();

            foreach (var panelMembershipSummary in result)
            {
                panelMembershipSummaryDictionary.Add(panelMembershipSummary.PersonId, new PanelMembershipSummaryDto
                {
                    Overidue = panelMembershipSummary.Overidue,
                    InProgress = panelMembershipSummary.InProgress,
                });
            }

            return panelMembershipSummaryDictionary;
        }

        public MembershipDto GetPanelMembership(int memberShipId)
        {
            throw new NotImplementedException();
        }

        public PanelMembershipLookupTypeResponse GetPanelMembershipLookUp(GetPanelMemberLookupRequest request)
        {
            var panelMembers = NHibernateSession.Current.Query<PanelMembership>().Where(x => request.PanelIds.Contains(x.Panel.Id));

            if (request.ActiveMembersOnly)
            {
                panelMembers = panelMembers.Where(x => DateTime.Now.Date >= x.StartDate && x.EndDate >= DateTime.Now.Date);
            }

            if (request.CredentialTypeId.HasValue)
            {
                panelMembers = panelMembers.Where(x => x.PanelMembershipMaterialCredentialTypes.Count(y => y.CredentialType.Id == request.CredentialTypeId.Value) > 0 
                || x.PanelMembershipCoordinatorCredentialTypes.Count(y => y.CredentialType.Id == request.CredentialTypeId.Value) > 0);
            }

            var result = panelMembers.ToList().Select(x => new PanelMembershipLookupDto()
            {
                Id = x.Id,
                DisplayName = $"{x.Person.Surname} {x.Person.GivenName} - {x.Panel.Name} - ({x.Person.Entity.NaatiNumber}) ",
                Name = $"{x.Person.GivenName} {x.Person.Surname}",
                NaatiNumber = x.Person.Entity.NaatiNumber,
                Email = x.Person.Entity.Emails.FirstOrDefault(y => !y.Invalid && y.IsPreferredEmail)?.EmailAddress,
                IsCoordinatorCredentialType = x.PanelMembershipCoordinatorCredentialTypes.Where(x => x.CredentialType.Id == request.CredentialTypeId).FirstOrDefault() != null
            }).ToList();

            return new PanelMembershipLookupTypeResponse() { Results = result };

        }

        public AddOrUpdateMembershipResponse AddOrUpdateMembership(AddOrUpdateMembershipRequest request)
        {
            var membershipQuery = NHibernateSession.Current.Query<PanelMembership>();
            var isUpdate = request.PanelMembershipId.HasValue;
            var isCreate = !isUpdate;

            var membership = isUpdate
                ? membershipQuery.SingleOrDefault(x => x.Id == request.PanelMembershipId.Value)
                : new PanelMembership();

            if (membership == null)
            {
                throw new WebServiceException("Referenced panel membership does not exist");
            }

            membership.Person = GetUpdatedEntity(membership.Person, request.PersonId, isCreate);
            membership.Panel = GetUpdatedEntity(membership.Panel, request.PanelId, isCreate);
            membership.PanelRole = GetUpdatedEntity(membership.PanelRole, request.PanelRoleId, isCreate);

            membership.StartDate = request.StartDate;
            membership.EndDate = request.EndDate;

            var ids = request.CredentialTypeIds.Concat(new[] { 0 }).ToList();
            ids.AddRange(request.MaterailCredentialTypesIds);
            ids.AddRange(request.CoordinatorCredentialTypesIds);

            var credentialTypes = NHibernateSession.Current.Query<CredentialType>()
                .Where(x => ids.Contains(x.Id))
                .ToList()
                .ToDictionary(a => a.Id, b => b);

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.Save(membership);

                foreach (var credentialType in membership.PanelMembershipCredentialTypes ?? Enumerable.Empty<PanelMembershipCredentialType>())
                {
                    credentialType.PanelMembership = null;
                    NHibernateSession.Current.Delete(credentialType);
                }
                foreach (var materialCredentialType in membership.PanelMembershipMaterialCredentialTypes ?? Enumerable.Empty<PanelMembershipMaterialCredentialType>())
                {
                    materialCredentialType.PanelMembership = null;
                    NHibernateSession.Current.Delete(materialCredentialType);
                }
                foreach (var coordinatorCredentialType in membership.PanelMembershipCoordinatorCredentialTypes ?? Enumerable.Empty<PanelMembershipCoordinatorCredentialType>())
                {
                    coordinatorCredentialType.PanelMembership = null;
                    NHibernateSession.Current.Delete(coordinatorCredentialType);
                }
                var newCredentialypes =
                    request.CredentialTypeIds.Select(
                        x => new PanelMembershipCredentialType()
                        {
                            PanelMembership = membership,
                            CredentialType = credentialTypes[x]
                        });

                var newMaterialCredentialTypes =
                    request.MaterailCredentialTypesIds.Select(x => new PanelMembershipMaterialCredentialType
                    {
                        PanelMembership = membership,
                        CredentialType = credentialTypes[x]
                    });

                var newCoordinatorCredentialTypes =
                    request.CoordinatorCredentialTypesIds.Select(x => new PanelMembershipCoordinatorCredentialType
                    {
                        PanelMembership = membership,
                        CredentialType = credentialTypes[x]
                    });

                foreach (var newCredentialype in newCredentialypes)
                {
                    NHibernateSession.Current.Save(newCredentialype);
                }
                foreach (var newMaterialCredentialType in newMaterialCredentialTypes)
                {
                    NHibernateSession.Current.Save(newMaterialCredentialType);
                }
                foreach (var newCoordinatorCredentialType in newCoordinatorCredentialTypes)
                {
                    NHibernateSession.Current.Save(newCoordinatorCredentialType);
                }

                if (isCreate && membership.PanelRole.PanelRoleCategory.Id == (int)PanelRoleCategoryName.RolePlayer)
                {
                    var rolePlayer = NHibernateSession.Current.Query<RolePlayer>()
                        .FirstOrDefault(x => x.Person.Id == membership.Person.Id) ?? new RolePlayer();

                    rolePlayer.Person = membership.Person;
                    rolePlayer.SessionLimit = request.RolePlayerSessionLimit;
                    rolePlayer.Rating = request.RolePlayerRating;
                    rolePlayer.Senior = request.RolePlayerSenior;
                    NHibernateSession.Current.Save(rolePlayer);
                }

                NHibernateSession.Current.Flush();
                transaction.Commit();
            }


            return new AddOrUpdateMembershipResponse
            {
                PanelMembershipId = membership.Id
            };
        }

        public bool HasPersonEmailAddress(int personId)
        {
            var entityId = NHibernateSession.Current.Get<Person>(personId).Entity.Id;
            var personEmailCount = NHibernateSession.Current.Query<Email>().Count(x => x.Entity.Id == entityId);

            return personEmailCount > 0;
        }

        public bool HasOverlappingMembership(OverlappingMembershipRequestItem[] request)
        {
            var overlappingMembershipsFound = false;

            foreach (var item in request)
            {
                var overlappingMemberships = NHibernateSession.Current.Query<PanelMembership>().Count(x => x.Id != item.PanelMembershipId
                                                                                                                       && x.Person.Id == item.PersonId
                                                                                                                       && x.Panel.Id == item.PanelId
                                                                                                                       && x.PanelRole.Id == item.PanelRoleId
                                                                                                                       && x.StartDate < item.EndDate
                                                                                                                       && item.StartDate < x.EndDate);
                if (overlappingMemberships > 0)
                {
                    overlappingMembershipsFound = true;
                }
            }

            return overlappingMembershipsFound;
        }

        public bool HasRolePlayerRatingLocation(int personId)
        {
            bool hasRolePlayerRatingLocation = false;
            var rolePlayer = NHibernateSession.Current.Query<RolePlayer>().FirstOrDefault(x => x.Person.Id == personId);

            if (rolePlayer != null)
            {
                var rolePlayerTestLocation = NHibernateSession.Current.Query<RolePlayerTestLocation>().Where(x => x.RolePlayer.Id == rolePlayer.Id).ToList();
                if (rolePlayer.Rating == null || rolePlayerTestLocation.Count == 0)
                    hasRolePlayerRatingLocation = true;
            }

            return hasRolePlayerRatingLocation;
        }

        public GetPanelTypesResponse GetPanelTypes()
        {
            var result = NHibernateSession.Current.Query<PanelType>().Select(x => new PanelTypeDto
            {
                Id = x.Id,
                DisplayName = x.Name,
                AllowCredentialTypeLink = x.AllowCredentialTypeLink
            }).ToList();

            return new GetPanelTypesResponse { Results = result };
        }

        public GetPanelResponse GetPanel(int panelId)
        {
            var panel = NHibernateSession.Current.Load<Panel>(panelId);
            return new GetPanelResponse() { Panel = MapPanel(panel) };
        }

        public DeleteMembershipResponse DeleteMembership(DeleteMembershipRequest request)
        {
            var membershipQuery = NHibernateSession.Current.Query<PanelMembership>();
            var membership = membershipQuery.SingleOrDefault(x => x.Id == request.PanelMembershipId);

            if (membership == null)
            {
                throw new WebServiceException("Referenced panel membership does not exist");
            }

            if (!ValidateMembershipDelete(membership))
            {
                throw new WebServiceException("The panel membership cannot be deleted because there are tests tied to it");
            }

            var credentialTypes = membership.PanelMembershipCredentialTypes;

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                NHibernateSession.Current.DeleteList(credentialTypes.ToList());
                NHibernateSession.Current.Delete(membership);
                NHibernateSession.Current.Flush();
                transaction.Commit();
            }

            return new DeleteMembershipResponse
            {
                Member = new MembershipDto
                {
                    PanelMembershipId = membership.Id,
                    PersonId = membership.Person.Id,
                    NAATINumber = membership.Person.Entity.NaatiNumber,
                    Name = membership.Person.FullName,
                    PanelId = membership.Panel.Id,
                    PanelName = membership.Panel.Name,
                    PanelRoleId = membership.PanelRole.Id,
                    PanelRole = membership.PanelRole.Name,
                    IsExaminerRole = membership.PanelRole.PanelRoleCategory.Id == (int)PanelRoleCategoryName.Examiner,
                    StartDate = membership.StartDate,
                    EndDate = membership.EndDate,
                    CredentialTypeIds = new int[0],
                    MaterialCredentialTypeIds = new int[0],
                    CoordinatorCredentialTypeIds = new int[0],
                    PanelRoleCategory = (PanelRoleCategoryName)membership.PanelRole.PanelRoleCategory.Id
                }
            };
        }

        public ReappointMembersResponse ReappointMembers(ReappointMembersRequest request)
        {
            var panelQuery = NHibernateSession.Current.Query<Panel>();

            var panel = panelQuery.SingleOrDefault(x => x.Id == request.PanelId);

            if (panel == null)
            {
                throw new WebServiceException("Referenced panel does not exist");
            }

            if (request.StartDate > request.EndDate)
            {
                throw new WebServiceException("End date must be greater than start date");
            }

            var memberships = panel.PanelMemberships.Where(x => request.PanelMembershipNumbers.Contains(x.Id)).ToArray();

            var reappointments = memberships.Select(x => new
            {
                Reappoinment = new PanelMembership
                {
                    Panel = x.Panel,
                    Person = x.Person,
                    PanelRole = x.PanelRole,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                },
                OldMembership = x
            });

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {

                foreach (var reappointmentData in reappointments)
                {
                    NHibernateSession.Current.Save(reappointmentData.Reappoinment);
                    foreach (var panelCredentialType in reappointmentData.OldMembership.PanelMembershipCredentialTypes)
                    {
                        var newPanelCredentialType = new PanelMembershipCredentialType
                        {
                            CredentialType = panelCredentialType.CredentialType,
                            PanelMembership = reappointmentData.Reappoinment
                        };

                        NHibernateSession.Current.Save(newPanelCredentialType);
                    }
                }

                NHibernateSession.Current.Flush();
                transaction.Commit();
            }




            return new ReappointMembersResponse
            {
                PanelMembershipIds = reappointments.Select(x => x.Reappoinment.Id).ToArray()
            };
        }

        private static IQueryable<T> FilterQuery<T, TU>(IQueryable<T> query, IEnumerable<TU> list, Expression<Func<T, bool>> predicate)
        {
            bool appliedQuery;
            return FilterQuery(query, list, predicate, out appliedQuery);
        }

        private static IQueryable<T> FilterQuery<T, TU>(IQueryable<T> query, IEnumerable<TU> list, Expression<Func<T, bool>> predicate, out bool appliedQuery)
        {
            if (list != null && list.Any())
            {
                appliedQuery = true;
                return query.Where(predicate);
            }

            appliedQuery = false;
            return query;
        }

        private static IQueryable<T> FilterQuery<T, TU>(IQueryable<T> query, TU? filter, Expression<Func<T, bool>> predicate) where TU : struct
        {
            bool appliedQuery;
            return FilterQuery(query, filter, predicate, out appliedQuery);
        }

        private static IQueryable<T> FilterQuery<T, TU>(IQueryable<T> query, TU? filter, Expression<Func<T, bool>> predicate, out bool appliedQuery) where TU : struct
        {
            if (filter.HasValue)
            {
                appliedQuery = true;
                return query.Where(predicate);
            }

            appliedQuery = false;
            return query;
        }

        private static T GetUpdatedEntity<T>(T currentEntity, int requestId, bool isCreate) where T : EntityBase
        {
            if (!isCreate && currentEntity.Id == requestId)
            {
                return currentEntity;
            }

            var query = NHibernateSession.Current.Query<T>();
            var entity = query.SingleOrDefault(x => x.Id == requestId);

            if (entity == null)
            {
                throw new WebServiceException($"Referenced {typeof(T)} does not exist");
            }

            return entity;
        }

        private static bool ValidateMembershipDelete(PanelMembership membership)
        {
            var jobExaminerQuery = NHibernateSession.Current.Query<JobExaminer>();

            return !jobExaminerQuery.Any(x => x.PanelMembership.Id == membership.Id);
        }

        public GetUnavailabilityResponse GetUnavailability(GetUnavailabilityRequest getUnavailabilityRequest)
        {
            var examinerUnavailableQuery = NHibernateSession.Current.Query<ExaminerUnavailable>();
            var panelMember = NHibernateSession.Current.Load<PanelMembership>(getUnavailabilityRequest.PanelMembershipId);

            return new GetUnavailabilityResponse
            {
                Unavailability = examinerUnavailableQuery.Where(x => x.Person.Id == panelMember.Person.Id && x.EndDate >= DateTime.Now).Select(x => new ExaminerUnavailabilityDto
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToArray()
            };
        }

        public GetMarkingRequestsResponse GetMarkingRequests(GetMarkingRequestsRequest getMarkingRequest)
        {
            var examinerService = new ExaminerToolsService(_autoMapperHelper);
            var panelMember = NHibernateSession.Current.Load<PanelMembership>(getMarkingRequest.PanelMembershipId);

            return new GetMarkingRequestsResponse
            {
                MarkingRequests = examinerService.GetTests(new GetTestsRequest
                {
                    PanelId = new[] { panelMember.Panel.Id },
                    RoleCategories = new[] { PanelRoleCategoryName.Examiner, },
                    UserId = panelMember.Person.Entity.NaatiNumber
                }).Tests.Select(x => new MarkingRequestDto
                {
                    AttendanceId = x.TestSittingId,
                    NaatiNumber = x.NaatiNumber,
                    ApplicantName = x.Applicant,
                    Language = x.SkillDisplayName,
                    Status = x.Status,
                    DueDate = x.DueDate,
                    TestDate = x.TestDate,
                    Type = x.CredentialTypeInternalName,
                }).ToArray()
            };
        }

        public GetMaterialRequestsResponse GetMaterialRequests(GetMaterialRequestsRequest getMaterialRequestsRequest)
        {
            var examinerService = new ExaminerToolsService(_autoMapperHelper);
            var panelMember = NHibernateSession.Current.Load<PanelMembership>(getMaterialRequestsRequest.PanelMembershipId);

            return new GetMaterialRequestsResponse
            {
                MaterialRequests = examinerService.GetTestsMaterial(new GetTestsMaterialRequest
                {
                    RoleCategories = new[] { PanelRoleCategoryName.Examiner, },
                    UserId = panelMember.Person.Entity.NaatiNumber,
                    ListAllStatuses = true
                }).Tests.Select(x => new MaterialRequestDto
                {
                    TestMaterialID = x.TestMaterialID,
                    JobExaminerID = x.JobExaminerID,
                    JobID = x.JobID,
                    Language = x.Language,
                    Category = x.Category,
                    Direction = x.Direction,
                    Level = x.Level,
                    DueDate = x.DueDate,
                    DateReceived = x.DateReceived,
                    Cost = x.Cost,
                    Approved = x.Approved
                }).ToArray()
            };
        }
    }
}
