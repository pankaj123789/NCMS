using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal.QueryHelper
{
    internal class ApplicationQueryHelper : QuerySearchHelper
    {
        private ApplicationSearchDto mApplicationSearchDto => null;
        private CredentialRequestSummarySearchDto mCredentialRequestSummarySearchDto => null;

        public IList<ApplicationSearchDto> SearchApplications(GetApplicationSearchRequest request)
        {
            var applicationFilterDictionary = new Dictionary<ApplicationFilterType, Func<ApplicationSearchCriteria, Junction, Junction>>
            {
                [ApplicationFilterType.NaatiNumberIntList] = (criteria, previousJunction) => GetNaatiNumberFilter(criteria, previousJunction),
                [ApplicationFilterType.ApplicationReferenceString] = (criteria, previousJunction) => GetApplicationReferenceFilter(criteria, previousJunction),
                [ApplicationFilterType.ApplicationOwnerIntList] = (criteria, previousJunction) => GetApplicationOwnerFilter(criteria, previousJunction),
                [ApplicationFilterType.PersonNameString] = (criteria, previousJunction) => GetNameFilter(criteria, previousJunction),
                [ApplicationFilterType.PhoneNumberString] = (criteria, previousJunction) => GetPhoneFilter(criteria, previousJunction),
                [ApplicationFilterType.ActiveApplicationBoolean] = (criteria, previousJunction) => GetActiveApplicationFilter(criteria, previousJunction),
                [ApplicationFilterType.AutoCreatedApplicationBoolean] = (criteria, previousJunction) => GetAutoCreatedApplicationFilter(criteria, previousJunction),
                [ApplicationFilterType.AutoCreatedCredentialRequestBoolean] = (criteria, previousJunction) => GetAutoCreatedCredentialRequestFilter(criteria, previousJunction),
                [ApplicationFilterType.ApplicationTypeIntList] = (criteria, previousJunction) => GetApplicationTypeFilter(criteria, previousJunction),
                [ApplicationFilterType.CredentialRequestTypeIntList] = (criteria, previousJunction) => GetCredentialRequestTypeFilter(criteria, previousJunction),
                [ApplicationFilterType.ApplicationStatusIntList] = (criteria, previousJunction) => GetApplicationStatusFilter(criteria, previousJunction),
                [ApplicationFilterType.CredentialRequestStatusIntList] = (criteria, previousJunction) => GetCredentialRequestStatusFilter(criteria, previousJunction),
                [ApplicationFilterType.EnteredOfficeIntList] = (criteria, previousJunction) => GetEnteredOfficeFilter(criteria, previousJunction),
                [ApplicationFilterType.LanguageIntList] = (criteria, previousJunction) => GetLanguageFilter(criteria, previousJunction),
                [ApplicationFilterType.PreferredTestLocationIntList] = (criteria, previousJunction) => GetPreferredTestLocationFilter(criteria, previousJunction),
                [ApplicationFilterType.SponsorIntList] = (criteria, previousJunction) => GetSponsorFilter(criteria, previousJunction),
                [ApplicationFilterType.EnteredDateFrom] = (criteria, previousJunction) => GetEnteredDateFromFilter(criteria, previousJunction),
                [ApplicationFilterType.EnteredDateTo] = (criteria, previousJunction) => GetEnteredDateToFilter(criteria, previousJunction),
                [ApplicationFilterType.StatusModifiedDateFrom] = (criteria, previousJunction) => GetStatusModifiedFromFilter(criteria, previousJunction),
                [ApplicationFilterType.StatusModifiedDateTo] = (criteria, previousJunction) => GetStatusModifiedToFilter(criteria, previousJunction),
                [ApplicationFilterType.ApplicationTypeCategoryIntList] = (criteria, previousJunction) => GetApplicationTypeCategoryFilter(criteria, previousJunction),
                [ApplicationFilterType.AllowMultipleActiveApplicationsBoolean] = (criteria, previousJunction) => GetAllowMultipleApplicationFilter(criteria, previousJunction),
                [ApplicationFilterType.CredentialRequestIntList] = (criteria, previousJunction) => GetCredentialRequestFilter(criteria, previousJunction),
                [ApplicationFilterType.DisplayBillsBoolean] = (criteria, previousJunction) => GetDisplayBillsFilter(criteria, previousJunction),
                [ApplicationFilterType.CredentialTypeCategoryString] = (criteria, previousJunction) => GetCredentialTypeCategoryFilter(criteria, previousJunction),

                [ApplicationFilterType.EndorsedQualificationIdsIntList] = (criteria, previousJunction) => GetEndorsedQualificationFilter(criteria, previousJunction),
                [ApplicationFilterType.EndorsementQualificationDateFrom] = (criteria, previousJunction) => GetEndorsementQualificationFromFilter(criteria, previousJunction),
                [ApplicationFilterType.EndorsementQualificationDateTo] = (criteria, previousJunction) => GetEndorsementQualificationToFilter(criteria, previousJunction)

            };

            Junction junction = null;

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                junction = Restrictions.Conjunction();
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = applicationFilterDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }

            var queryOver = BuildQuery(validCriterias);

            var subquery = GetSubquery(queryOver, junction, request.Take, request.Skip);

            queryOver = queryOver.Where(Restrictions.In(Projections.Property(() => CredentialApplication.Id),
                subquery.List<int>().ToList()));

            var projections = BuildProjections();
            queryOver = queryOver.Select(projections.ToArray());

            var indices = projections.ToDictionary(k => k.Aliases.First(), v => projections.FindIndex(value => value == v));

            return queryOver.TransformUsing(new ApplicationSearchTransformer(indices)).List<ApplicationSearchDto>();
        }
       
 public IList<CredentialRequestSummarySearchDto> SearchCredentialRequestSummary(GetCredentialRequestSummarySearchRequest request)
        {
            var credentialRequestSummaryFilterDictionary = new Dictionary<CredentialRequestSummaryFilterType, Func<CredentialRequestSummarySearchCriteria, Junction, Junction>>
            {
                [CredentialRequestSummaryFilterType.PreferredTestLocationIntList] = (criteria, previousJunction) => GetPreferredTestLocationFilter(criteria, previousJunction),
                [CredentialRequestSummaryFilterType.CredentialRequestStatusIntList] = (criteria, previousJunction) => GetCredentialRequestStatusFilter(criteria, previousJunction),
                [CredentialRequestSummaryFilterType.ApplicationTypeIntList] = (criteria, previousJunction) => GetApplicationTypeFilter(criteria, previousJunction),
                [CredentialRequestSummaryFilterType.CredentialRequestTypeIntList] = (criteria, previousJunction) => GetCredentialRequestTypeFilter(criteria, previousJunction),
                [CredentialRequestSummaryFilterType.LanguageIntList] = (criteria, previousJunction) => GetLanguageFilter(criteria, previousJunction),
                [CredentialRequestSummaryFilterType.SkillIntList] = (criteria, previousJunction) => GetSkillFilter(criteria, previousJunction),
                [CredentialRequestSummaryFilterType.IntendedTestSessionIntList] = (criteria, previousJunction) => GetIntendedTestSessionFilter(criteria, previousJunction),
            };

            Junction junction = null;

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            if (validCriterias.Any())
            {
                junction = Restrictions.Conjunction();
                foreach (var criteria in validCriterias)
                {
                    var junctionFunc = credentialRequestSummaryFilterDictionary[criteria.Filter];
                    junction = junctionFunc(criteria, junction);
                }
            }

            var queryOver = BuildQueryForCredentialRequstSummary(validCriterias);
            
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
            
            var projections = BuildProjectionsForCredentialRequstSummary();
            queryOver = queryOver.Select(projections.ToArray());


            var searchResult = queryOver.TransformUsing(Transformers.AliasToBean<CredentialRequestSummarySearchDto>());

            var credentialRequestSummaryResult = searchResult.List<CredentialRequestSummarySearchDto>();

            return credentialRequestSummaryResult;
        }

        protected IQueryOver<CredentialRequest, CredentialRequest> GetSubqueryForCredentialRequstSummary(IQueryOver<CredentialRequest, CredentialRequest> queyrOver, Junction junction, int? take, int? skip)
        {
            var subQueryOver = queyrOver.Clone();
            if (junction != null)
            {
                subQueryOver = subQueryOver.Where(junction);
            }

            var credentialRequestId = Projections.Property(() => CredentialRequest.Id);

            subQueryOver = subQueryOver.Select(Projections.Distinct(credentialRequestId))
                .OrderBy(() => CredentialRequest.Id)
                .Asc;

            if (skip.HasValue)
            {
                subQueryOver.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                subQueryOver.Take(take.Value);
            }

            return subQueryOver;
        }

        protected IQueryOver<CredentialApplication, CredentialApplication> GetSubquery(IQueryOver<CredentialApplication, CredentialApplication> queyrOver, Junction junction, int? take, int? skip)
        {
            var subQueryOver = queyrOver.Clone();
            if (junction != null)
            {
                subQueryOver = subQueryOver.Where(junction);
            }

            var applicationId = Projections.Property(() => CredentialApplication.Id);

            subQueryOver = subQueryOver.Select(Projections.Distinct(applicationId))
                .OrderBy(() => CredentialApplication.Id)
                .Asc;

            if (skip.HasValue)
            {
                subQueryOver.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                subQueryOver.Take(take.Value);
            }

            return subQueryOver;
        }

        private IQueryOver<CredentialApplication, CredentialApplication> BuildQuery(IList<ApplicationSearchCriteria> filterCriterias)
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.CredentialApplicationType, () => CredentialApplicationType)
                .Inner.JoinAlias(x => CredentialApplicationType.CredentialApplicationTypeCategory, () => CredentialApplicationTypeCategory)
                .Inner.JoinAlias(x => CredentialApplication.CredentialApplicationStatusType, () => CredentialApplicationStatusType)
                .Left.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Left.JoinAlias(x => Person.Entity, () => Entity)
                .Left.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Left.JoinAlias(x => LatestPersonName.PersonName, () => PersonName)
                .Left.JoinAlias(x => Entity.Phones, () => Phone)
                .Left.JoinAlias(x => CredentialApplication.OwnedByUser, () => OwnedByUser)
                .Left.JoinAlias(x => CredentialApplication.SponsorInstitution, () => SponsorInstitution)
                .Left.JoinAlias(x => CredentialApplication.PreferredTestLocation, () => PreferredTestLocation)
                .Left.JoinAlias(x => SponsorInstitution.Entity, () => SponsorEntity)
                .Left.JoinAlias(x => SponsorInstitution.LatestInstitutionName, () => LatestInstitutionName)
                .Left.JoinAlias(x => LatestInstitutionName.InstitutionName, () => SponsorInstitutionName);

            queryOver =
                queryOver.Where(
                    x => x.CredentialApplicationStatusType.Id != (int) CredentialApplicationStatusTypeName.Deleted);


            var filtersDictionary = filterCriterias.ToDictionary(x => x.Filter, y => y.Values);

            if (filtersDictionary.ContainsKey(ApplicationFilterType.CredentialRequestTypeIntList)
                || filtersDictionary.ContainsKey(ApplicationFilterType.CredentialRequestStatusIntList)
                || filtersDictionary.ContainsKey(ApplicationFilterType.LanguageIntList)
                || filtersDictionary.ContainsKey(ApplicationFilterType.StatusModifiedDateFrom)
                || filtersDictionary.ContainsKey(ApplicationFilterType.StatusModifiedDateTo)
                || filtersDictionary.ContainsKey(ApplicationFilterType.AutoCreatedCredentialRequestBoolean))
            {
                queryOver = queryOver.Inner.JoinAlias(x => CredentialApplication.CredentialRequests,
                    () => CredentialRequest);

                if (filtersDictionary.ContainsKey(ApplicationFilterType.CredentialRequestTypeIntList))
                {
                    queryOver = queryOver.Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType);
                }

                if (filtersDictionary.ContainsKey(ApplicationFilterType.CredentialRequestStatusIntList))
                {
                    queryOver = queryOver.Inner.JoinAlias(x => CredentialRequest.CredentialRequestStatusType,
                        () => CredentialRequestStatusType);
                }

                if (filtersDictionary.ContainsKey(ApplicationFilterType.LanguageIntList))
                {
                    queryOver = queryOver.Left.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                        .Left.JoinAlias(x => Skill.Language1, () => Language1)
                        .Left.JoinAlias(x => Skill.Language2, () => Language2);
                }

                if (filtersDictionary.ContainsKey(ApplicationFilterType.CredentialTypeCategoryString))
                {
                    queryOver = queryOver.Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                        .Inner.JoinAlias(x => CredentialType.CredentialCategory, () => CredentialCategory);
                }
            }

            if (filtersDictionary.ContainsKey(ApplicationFilterType.EnteredOfficeIntList))
            {
                queryOver = queryOver.Left.JoinAlias(() => CredentialApplication.ReceivingOffice, () => ReceivingOffice);
            }

            return queryOver;
        }

        private IQueryOver<CredentialRequest, CredentialRequest> BuildQueryForCredentialRequstSummary(IList<CredentialRequestSummarySearchCriteria> criterias)
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialApplication, () => CredentialApplication)
                .Inner.JoinAlias(x => CredentialApplication.Person, () => Person)
                .Inner.JoinAlias(x => CredentialApplication.CredentialApplicationType, () => CredentialApplicationType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialRequest.Skill, () => Skill)
                .Inner.JoinAlias(x => Skill.DirectionType, () => DirectionType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialRequestStatusType, () => CredentialRequestStatusType)
                .Inner.JoinAlias(x => Skill.Language1, () => Language1)
                .Inner.JoinAlias(x => Skill.Language2, () => Language2)

                .Left.JoinAlias(x => CredentialApplication.PreferredTestLocation, () => PreferredTestLocation)
                .Left.JoinAlias(x => PreferredTestLocation.Office, () => Office)
                .Left.JoinAlias(x => Office.State, () => State)
                .Left.JoinAlias(x => CredentialRequest.TestSittings, () => TestSitting)
                .Left.JoinAlias(x => TestSitting.TestSession, () => TestSession);

                if(criterias.Any(x=> x.Filter == CredentialRequestSummaryFilterType.IntendedTestSessionIntList))
                {
                    queryOver = queryOver.Left.JoinAlias(x => PreferredTestLocation.Venues, () => Venue)
                        .Left.JoinAlias(x => Venue.TestSessions, () => IntendedTestSession)
                        .Left.JoinAlias(x => IntendedTestSession.CredentialType, () => TestSessionCredentialType);

                }

            queryOver = queryOver.Where(x => (x.CredentialRequestStatusType.Id != (int) CredentialRequestStatusTypeName.Deleted &&
                             x.CredentialRequestStatusType.Id != (int) CredentialRequestStatusTypeName.Draft &&
                             x.CredentialRequestStatusType.Id != (int) CredentialRequestStatusTypeName.Cancelled));

            return queryOver;
        }


        private List<IProjection> BuildProjections()
        {
            return new List<IProjection>
            {
                Projections.Property(() => CredentialApplication.Id)
                    .WithAlias(() => mApplicationSearchDto.Id),

                Projections.Property(() => CredentialApplication.Reference)
                    .WithAlias(() => mApplicationSearchDto.ApplicationReference),

                Projections.Property(() => CredentialApplicationType.DisplayName)
                    .WithAlias(() => mApplicationSearchDto.ApplicationType),

                Projections.Property(() => CredentialApplicationType.Id)
                    .WithAlias(() => mApplicationSearchDto.ApplicationTypeId),

                Projections.Property(() => CredentialApplicationStatusType.DisplayName)
                    .WithAlias(() => mApplicationSearchDto.ApplicationStatus),

                Projections.Property(() => Entity.NaatiNumber)
                    .WithAlias(() => mApplicationSearchDto.NaatiNumber),

                Projections.Property(() => CredentialApplication.StatusChangeDate)
                    .WithAlias(() => mApplicationSearchDto.StatusChangeDate),

                Projections.Property(() => CredentialApplication.AutoCreated)
                    .WithAlias(() => mApplicationSearchDto.AutoCreated),

                GetNameProjection().WithAlias(() => mApplicationSearchDto.Name),
                GetPhoneProjection().WithAlias(() => mApplicationSearchDto.PrimaryContactNumber),
                GetApplicationOwnerProjection().WithAlias(()=> mApplicationSearchDto.ApplicationOwner),
                Projections.Property(()=> SponsorInstitutionName.Name).WithAlias(()=> mApplicationSearchDto.SponsorName),
                Projections.Property(()=> SponsorEntity.NaatiNumber).WithAlias(()=> mApplicationSearchDto.SponsorNaatiNumber),
                Projections.Property(()=> CredentialApplication.EnteredDate).WithAlias(()=> mApplicationSearchDto.EnteredDate),
                Projections.Property(()=> PreferredTestLocation.Id).WithAlias(()=> mApplicationSearchDto.PreferredTestLocationId),
                Projections.Property(()=> CredentialApplicationType.DisplayBills).WithAlias(()=> mApplicationSearchDto.DisplayBills)

            };
        }

        private List<IProjection> BuildProjectionsForCredentialRequstSummary()
        {
            return new List<IProjection>
            {
                Projections.Group(() => CredentialApplicationType.Id).WithAlias(() => mCredentialRequestSummarySearchDto.CredentialApplicationTypeId),
                Projections.Group(() => CredentialApplicationType.DisplayName).WithAlias(() => mCredentialRequestSummarySearchDto.ApplicationType),

                Projections.Group(() => CredentialApplication.AutoCreated).WithAlias(() => mCredentialRequestSummarySearchDto.AutoCreated),

                Projections.Group(() => CredentialType.Id).WithAlias(() => mCredentialRequestSummarySearchDto.CredentialTypeId),
                Projections.Group(() => CredentialType.InternalName).WithAlias(() => mCredentialRequestSummarySearchDto.CredentialType),

                Projections.Group(() => Skill.Id).WithAlias(() => mCredentialRequestSummarySearchDto.SkillId),
                Projections.Group(() => DirectionType.DisplayName).WithAlias(() => mCredentialRequestSummarySearchDto.Skill),
                //Projections.GroupProperty(GetDirectionProjection()).WithAlias(() => mCredentialRequestSummarySearchDto.Skill), //if use this then runtime error saying "Index out of range...."
                //Projections.Group(() => GetDirectionProjection()).WithAlias(() => mCredentialRequestSummarySearchDto.Skill), //if use this then runtime error saying "Unrecognized method...."

                Projections.Group(() => PreferredTestLocation.Id).WithAlias(() => mCredentialRequestSummarySearchDto.TestLocationId),
                Projections.Group(() => PreferredTestLocation.Name).WithAlias(() => mCredentialRequestSummarySearchDto.PreferredTestLocation),
                //Projections.GroupProperty(GetTestLocationProjection()).WithAlias(() => mCredentialRequestSummarySearchDto.PreferredTestLocation), //if use this then runtime error saying "Unrecognized method...."
                
                Projections.Group(() => CredentialRequestStatusType.Id).WithAlias(() => mCredentialRequestSummarySearchDto.CredentialRequestStatusTypeId),
                Projections.Group(() => CredentialRequestStatusType.DisplayName).WithAlias(() => mCredentialRequestSummarySearchDto.CredentialRequestStatus),

                Projections.Count(Projections.Distinct(Projections.Property(()=> Person.Id))).WithAlias(() => mCredentialRequestSummarySearchDto.NumberOfApplicants),
                Projections.Min(() => CredentialApplication.EnteredDate).WithAlias(() => mCredentialRequestSummarySearchDto.EnteredDate),
                                
                Projections.Group(() => Language1.Name).WithAlias(() => mCredentialRequestSummarySearchDto.Language1Name),
                Projections.Group(() => Language2.Name).WithAlias(() => mCredentialRequestSummarySearchDto.Language2Name),
                Projections.Group(() => DirectionType.DisplayName).WithAlias(() => mCredentialRequestSummarySearchDto.DirectionDisplayName),
                Projections.Group(() => State.Abbreviation).WithAlias(() => mCredentialRequestSummarySearchDto.StateAbbr),
                Projections.Min(() => CredentialApplication.EnteredDate).WithAlias(() => mCredentialRequestSummarySearchDto.EarliestApplicationEnteredDate),
                Projections.Max(() => CredentialApplication.EnteredDate).WithAlias(() => mCredentialRequestSummarySearchDto.LastApplicationEnteredDate),
            };
        }

        private Junction GetApplicationOwnerFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S,int>();
            if (typeList.Contains(0))
            {
                var restriction = Restrictions.Or(
                      Restrictions.Eq(Projections.Property(() => CredentialApplication.OwnedByApplicant), true),
                      Restrictions.In(Projections.Property(() => OwnedByUser.Id), typeList));

                return junction.Add(restriction);
            }

            return junction.Add<CredentialApplication>(cp => OwnedByUser.Id.IsIn(typeList));
        }

        private IProjection GetApplicationOwnerProjection()
        {
            var ownedByAppliacant = Restrictions.Eq(Projections.Property(() => CredentialApplication.OwnedByApplicant),
                true);

            var ownedByuser = Restrictions.IsNotNull(Projections.Property(() => OwnedByUser.Id));

            var emptyValue = Projections.Constant("", NHibernateUtil.String);
            var applicantValue = Projections.Constant("Applicant", NHibernateUtil.String);
            var ownerName = Projections.Property(() => OwnedByUser.FullName);

            return Projections.Conditional(ownedByAppliacant, applicantValue,
                 Projections.Conditional(ownedByuser, ownerName, emptyValue));
        }

        private Junction GetApplicationReferenceFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterString = criteria.Values.FirstOrDefault() ?? string.Empty;

            if (filterString.StartsWith(CredentialApplication.ApplicationReferencePrefix, StringComparison.OrdinalIgnoreCase))
            {
                filterString = filterString.Replace(CredentialApplication.ApplicationReferencePrefix, string.Empty,
                    StringComparison.OrdinalIgnoreCase);
            }

            int applicationId;

            if (!int.TryParse(filterString, out applicationId))
            {
                applicationId = -1000;
            }

            return junction.Add<CredentialApplication>(e => CredentialApplication.Id == applicationId);
        }

        protected Junction GetCredentialRequestFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialRequest>(at => CredentialRequest.Id.IsIn(typeList));
            return junction;
        }
        
        protected Junction GetEndorsedQualificationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>().ToArray();
            var filter = Subqueries.WhereProperty<CredentialApplication>(e => CredentialApplication.Id).In(GetEndorsedQualificationApplicationId(typeList));
            return junction.Add(filter);
        }

        private QueryOver<CredentialApplicationFieldData, CredentialApplicationFieldData> GetEndorsedQualificationApplicationId(int[] endorsedQualificationIds)
        {
            CredentialApplicationFieldData mCredentialApplicationFieldData = null;
            CredentialApplicationField mCredentialApplicationField = null;
            object[] endorsedQualificationStringIds = endorsedQualificationIds.Select(x => x.ToString()).ToArray();

            var subquery = QueryOver.Of(() => mCredentialApplicationFieldData)
                .Inner.JoinAlias(x => mCredentialApplicationFieldData.CredentialApplicationField, () => mCredentialApplicationField)
                .Where(
                    Restrictions.Conjunction()
                    .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationField.DataType.Id), (int)DataTypeName.EndorsedQualificationIdLookup))
                    .Add(Restrictions.In(Projections.Property(() => mCredentialApplicationFieldData.Value), endorsedQualificationStringIds))
                )
                .Select(Projections.Property(() => mCredentialApplicationFieldData.CredentialApplication.Id));
            return subquery; 
        }

        private Junction GetNameFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var nameProjection = GetNameProjection();

            return junction.Add(Restrictions.Like(nameProjection, criteria.Values.FirstOrDefault()));
        }
        private Junction GetDisplayBillsFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var displayBills = criteria.ToList<S, bool>().First();

            return junction.Add(Restrictions.Eq(Projections.Property(() => CredentialApplicationType.DisplayBills), displayBills));
        }

        private Junction GetCredentialTypeCategoryFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterString = criteria.Values.FirstOrDefault() ?? string.Empty;

            return junction.Add<CredentialCategory>(x => CredentialCategory.Name == filterString);
        }

        private Junction GetCredentialRequestStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialRequestStatusType>(at => CredentialRequestStatusType.Id.IsIn(typeList));
            return junction;
        }
        private Junction GetAllowMultipleApplicationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var filterValue = criteria.ToList<S, bool>().First();
            junction = junction.Add( Restrictions.Eq(Projections.Property(() => CredentialApplicationType.AllowMultiple), filterValue));
            return junction;
        }

        private Junction GetEnteredOfficeFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Office>(at => ReceivingOffice.Id.IsIn(typeList));
            return junction;
        }
        
        private Junction GetSponsorFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();

            junction.Add(Restrictions.IsNotNull(Projections.Property(() => SponsorEntity.Id)));
            junction.Add<Institution>(x => SponsorEntity.NaatiNumber.IsIn(typeList));
            return junction;
        }

        private Junction GetEnteredDateFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = Convert.ToDateTime(criteria.Values.FirstOrDefault());
            junction.Add(Restrictions.IsNotNull(Projections.Property(() => CredentialApplication.EnteredDate)));

            var enteredDate = GetDateProjectionFrom(Projections.Property(() => CredentialApplication.EnteredDate));

            var restriction = Restrictions.Ge(enteredDate, typeList.Date);
            junction.Add(restriction);
            return junction;
        }

        private Junction GetEnteredDateToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = Convert.ToDateTime(criteria.Values.FirstOrDefault());
            junction.Add(Restrictions.IsNotNull(Projections.Property(() => CredentialApplication.EnteredDate)));

            var enteredDate = GetDateProjectionFrom(Projections.Property(() => CredentialApplication.EnteredDate));
            
            var restriction = Restrictions.Le(enteredDate, typeList.Date);
            junction.Add(restriction);
            return junction;
        }
       

        private Junction GetStatusModifiedFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = Convert.ToDateTime(criteria.Values.FirstOrDefault());
            var statusChangeDate = GetDateProjectionFrom(Projections.Property(() => CredentialRequest.StatusChangeDate));
            var restriction = Restrictions.Ge(statusChangeDate, typeList.Date);
            var notNull = Restrictions.IsNotNull(Projections.Property(() => CredentialRequest.StatusChangeDate));
            junction.Add(Restrictions.And(notNull, restriction));
            return junction;
        }

        private Junction GetStatusModifiedToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = Convert.ToDateTime(criteria.Values.FirstOrDefault());
            var statusChangeDate = GetDateProjectionFrom(Projections.Property(() => CredentialRequest.StatusChangeDate));
            var restriction = Restrictions.Le(statusChangeDate, typeList.Date);
            var notNull = Restrictions.IsNotNull(Projections.Property(() => CredentialRequest.StatusChangeDate));
            junction.Add(Restrictions.And(notNull, restriction));
            return junction;
        }

        private Junction GetEndorsementQualificationFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = Convert.ToDateTime(criteria.Values.FirstOrDefault());
            var filter = Subqueries.WhereProperty<CredentialApplication>(e => CredentialApplication.Id).In(GetEndorsedQualificationApplicationIdByFromDate(typeList));
            return junction.Add(filter);
        }

        private QueryOver<CredentialApplicationFieldData, CredentialApplicationFieldData> GetEndorsedQualificationApplicationIdByFromDate(DateTime qualificationFromDate)
        {
            CredentialApplicationFieldData mCredentialApplicationFieldData = null;
            CredentialApplicationField mCredentialApplicationField = null;

            var subquery = QueryOver.Of(() => mCredentialApplicationFieldData)
                .Inner.JoinAlias(x => mCredentialApplicationFieldData.CredentialApplicationField, () => mCredentialApplicationField)
                .Where(
                    Restrictions.Conjunction()
                        .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationField.DataType.Id), (int)DataTypeName.EndorsedQualificationStartDate))
                        .Add(Restrictions.Ge(TryGetDateProjectionFrom(Projections.Property(() => mCredentialApplicationFieldData.Value)), qualificationFromDate.Date))
                        .Add(Restrictions.IsNotNull(Projections.Property(() => mCredentialApplicationFieldData.Value)))
                )
                .Select(Projections.Property(() => mCredentialApplicationFieldData.CredentialApplication.Id));
            return subquery;
        }

        private Junction GetEndorsementQualificationToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = Convert.ToDateTime(criteria.Values.FirstOrDefault());
            var filter = Subqueries.WhereProperty<CredentialApplication>(e => CredentialApplication.Id).In(GetEndorsedQualificationApplicationIdByToDate(typeList));
            return junction.Add(filter);
        }

        private QueryOver<CredentialApplicationFieldData, CredentialApplicationFieldData> GetEndorsedQualificationApplicationIdByToDate(DateTime qualificationToDate)
        {
            CredentialApplicationFieldData mCredentialApplicationFieldData = null;
            CredentialApplicationField mCredentialApplicationField = null;

            var subquery = QueryOver.Of(() => mCredentialApplicationFieldData)
                .Inner.JoinAlias(x => mCredentialApplicationFieldData.CredentialApplicationField, () => mCredentialApplicationField)
                .Where(
                    Restrictions.Conjunction()
                        .Add(Restrictions.Eq(Projections.Property(() => mCredentialApplicationField.DataType.Id), (int)DataTypeName.EndorsedQualificationEndDate))
                        .Add(Restrictions.Le(TryGetDateProjectionFrom(Projections.Property(() => mCredentialApplicationFieldData.Value)), qualificationToDate.Date))
                        .Add(Restrictions.IsNotNull(Projections.Property(() => mCredentialApplicationFieldData.Value)))
                )
                .Select(Projections.Property(() => mCredentialApplicationFieldData.CredentialApplication.Id));
            return subquery;
        }

        private Junction GetPreferredTestLocationFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Office>(at => CredentialApplication.PreferredTestLocation.Id.IsIn(typeList));
            return junction;
        }
        

        private Junction GetApplicationStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<CredentialApplicationStatusType>(at => CredentialApplicationStatusType.Id.IsIn(typeList));
            return junction;
        }


        private Junction GetIntendedTestSessionFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            // TODO : CHECK Performance of this filter
            var selectedTestSessionIds = criteria.ToList<S, int>();

            //Note: The intended test session is joined using test location, therefore filtering by testSessionId is like filtering the testLocation Id
            var credentialRequestHasSameTestLocation = Restrictions.In(Projections.Property(() => IntendedTestSession.Id), selectedTestSessionIds);
          
            var credentialRequestHasSameCredentialType =
                Restrictions.EqProperty(Projections.Property(() => TestSessionCredentialType.Id),
                    Projections.Property(() => CredentialType.Id));

            var testSittingProperty = Projections.Property(() => TestSitting.Id);
            var notAssignedToAnyTestSession = Restrictions.IsNull(testSittingProperty);

            var testSessionProperty = Projections.Property(() => TestSession.Id);
            var hasBeenAssignedToSelectedTestSessions = Restrictions.In(testSessionProperty, selectedTestSessionIds);

            var rejectedProperty = Projections.Property(() => TestSitting.Rejected);

            var hasRejectedOtherTestSession = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(testSittingProperty))
                .Add(Restrictions.Not(hasBeenAssignedToSelectedTestSessions))
                .Add(Restrictions.Eq(rejectedProperty, true));

            var suitableForTestSession = Restrictions.Conjunction()
                .Add(Restrictions.Or(notAssignedToAnyTestSession, hasRejectedOtherTestSession))
                .Add(credentialRequestHasSameTestLocation)
                .Add(credentialRequestHasSameCredentialType);
            
            var belongsToSelectedTestSession = Restrictions.Conjunction()
                .Add(Restrictions.IsNotNull(testSittingProperty))
                .Add(hasBeenAssignedToSelectedTestSessions)
                .Add(Restrictions.Eq(rejectedProperty, false));

            junction.Add(Restrictions.Or(suitableForTestSession, belongsToSelectedTestSession));
            return junction;
        }

        private ICriterion GetSuitableTestSessionRestriction(List<int> testSessionIds)
        {
            //Note: The intended test session is joined using test location, therefore filtering by testSessionId is like filtering the testLocation Id
            var credentialRequestHasSameTestLocation = Restrictions.In(Projections.Property(() => IntendedTestSession.Id), testSessionIds);

            var credentialRequestHasSameCredentialType =
                Restrictions.EqProperty(Projections.Property(() => TestSessionCredentialType.Id),
                    Projections.Property(() => CredentialType.Id));

            var supplementaryCredentialRequestProperty = Projections.Property(() => CredentialRequest.Supplementary);

            var notAllocatedCredentialRequest= Restrictions.And(Restrictions.Eq(supplementaryCredentialRequestProperty, false),
                Restrictions.Not(Subqueries.WhereProperty<CredentialRequest>(e => CredentialRequest.Id).In(GetAllocatedCredentialRequests())));

            var notAllocatedSupplementaryCredentialRequest = Restrictions.And(Restrictions.Eq(supplementaryCredentialRequestProperty, true),
                Restrictions.Not(Subqueries.WhereProperty<CredentialRequest>(e => CredentialRequest.Id).In(GetAllocatedSupplementaryCredentialRequests())));

            var suitableForTestSession = Restrictions.Conjunction()
                .Add(Restrictions.Or(notAllocatedCredentialRequest, notAllocatedSupplementaryCredentialRequest))
                .Add(credentialRequestHasSameTestLocation)
                .Add(credentialRequestHasSameCredentialType);

            return suitableForTestSession;
        }

        private QueryOver<TestSitting, TestSitting> GetAllocatedCredentialRequests()
        {
            TestSitting testSitting = null;
            CredentialRequest credentialRequest = null;
         
            return QueryOver.Of(() => testSitting)
                .Inner.JoinAlias(() => testSitting.CredentialRequest, () => credentialRequest)
                .Where(e => !e.Rejected)
                .Where(e => !e.Supplementary)
                .Select(Projections.GroupProperty(Projections.Property(() => credentialRequest.Id)));
        }

        private QueryOver<TestSitting, TestSitting> GetAllocatedSupplementaryCredentialRequests()
        {
            TestSitting testSitting = null;
            CredentialRequest credentialRequest = null;

            return QueryOver.Of(() => testSitting)
                .Inner.JoinAlias(() => testSitting.CredentialRequest, () => credentialRequest)
                .Where(e => !e.Rejected)
                .Where(e => e.Supplementary)
                .Select(Projections.GroupProperty(Projections.Property(() => credentialRequest.Id)));
        }

        private Junction GetSkillFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<Skill>(at => Skill.Id.IsIn(typeList));
            return junction;
        }

        public IDictionary<int, CredentialStatusTypeName> GetCredentialStatuses(List<int> credentialRequestIds)
        {
            if (credentialRequestIds.Count == 0)
            {
                return new Dictionary<int, CredentialStatusTypeName>();
            }
           
            var result = NHibernateSession.Current.QueryOver(() => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialCredentialRequests, () => CredentialCredentialRequest)
                .Inner.JoinAlias(x => CredentialCredentialRequest.Credential, () => Credential)
                .Left.JoinAlias(x => Credential.CertificationPeriod, () => CertificationPeriod)
                .Where(Restrictions.In(Projections.Property(()=> CredentialRequest.Id), credentialRequestIds))
                .Select(Projections.ProjectionList()
                    .Add(GetCredentialStatusProjection())
                    .Add(Projections.Property(() => Credential.Id)))
                .List<dynamic>();

            // return null;
            var dictionary =  new Dictionary<int, CredentialStatusTypeName>();

           
            result.ForEach(x => dictionary[(int)x[1]] = (CredentialStatusTypeName)x[0]);

            return dictionary;

        }

        public IDictionary<int, CredentialStatusTypeName> GetCredentialStatusesByCredentialIds(List<int> credentialIds)
        {
            if (credentialIds.Count == 0)
            {
                return new Dictionary<int, CredentialStatusTypeName>();
            }

            var result = NHibernateSession.Current.QueryOver(() => CredentialRequest)
                .Inner.JoinAlias(x => CredentialRequest.CredentialType, () => CredentialType)
                .Inner.JoinAlias(x => CredentialRequest.CredentialCredentialRequests, () => CredentialCredentialRequest)
                .Inner.JoinAlias(x => CredentialCredentialRequest.Credential, () => Credential)
                .Left.JoinAlias(x => Credential.CertificationPeriod, () => CertificationPeriod)
                .Where(Restrictions.In(Projections.Property(() => Credential.Id), credentialIds))
                .Select(Projections.ProjectionList()
                    .Add(GetCredentialStatusProjection())
                    .Add(Projections.Property(() => Credential.Id)))
                .List<dynamic>();

            // return null;
            var dictionary = new Dictionary<int, CredentialStatusTypeName>();

            result.ForEach( x => dictionary[(int)x[1]] =(CredentialStatusTypeName)x[0]);

            return dictionary;

        }
    }
}
