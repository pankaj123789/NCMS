using System;
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
    internal class EmailMessageQueryHelper : QuerySearchHelper
    {
        private EmailMessageResultDto mEmailMessageDto => null;
        public IList<EmailMessageResultDto> SearchEmails(GetEmailSearchRequest request)
        {
            var emailFilterDictionary = new Dictionary<EmailFilterType, Func<EmailSearchCriteria, Junction, Junction>>
            {
                [EmailFilterType.NaatiNumberIntList] = (criteria, previousJunction) => GetNaatiNumberFilter(criteria, previousJunction),
                [EmailFilterType.DateRequestedFromString] = (criteria, previousJunction) => GetDateRequestedFromFilter(criteria, previousJunction),
                [EmailFilterType.DateRequestedToString] = (criteria, previousJunction) => GetDateRequestedToFilter(criteria, previousJunction),
                [EmailFilterType.EmailStatusIntList] = (criteria, previousJunction) => GetEmailStatusFilter(criteria, previousJunction),
                [EmailFilterType.RecipientEmailString] = (criteria, previousJunction) => GetRecipientEmailFilter(criteria, previousJunction),
                [EmailFilterType.RecipientString] = (criteria, previousJunction) => GetRecipientFilter(criteria, previousJunction),
                [EmailFilterType.CustomerNumberIntList] = (criteria, previousJunction) => GetCustomerNumberFilter(criteria, previousJunction),
                [EmailFilterType.EmailMessageIdIntList] = (criteria, previousJunction) => GetEmailMessageIdFilter(criteria, previousJunction),
                [EmailFilterType.RecipientUserIntList] = (criteria, previousJunction) => GetRecipientUserFilter(criteria, previousJunction)
            };

            Junction junction = Restrictions.Conjunction();

            var validCriterias = request.Filters.Where(x => x.Values.Any(v => v != null)).ToList();
            foreach (var criteria in validCriterias)
            {
                var junctionFunc = emailFilterDictionary[criteria.Filter];
                junction = junctionFunc(criteria, junction);
            }

            var queryOver = BuildQuery();
         
            queryOver = queryOver.Where(junction);

            if (request.Skip.HasValue)
            {
                queryOver.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                queryOver.Take(request.Take.Value);
            }

            var projections = BuildProjections();

            foreach (var sortOption in request.SortingOptions)
            {
                queryOver.UnderlyingCriteria.AddOrder(GetOrdering(sortOption));
            }

            queryOver = queryOver.Select(projections);

            
            var resultList = queryOver.TransformUsing(Transformers.AliasToBean<EmailMessageResultDto>()).List<EmailMessageResultDto>();

            return resultList;
        }

        private Order GetOrdering(EmailMessageSortingOption sorting)
        {
            IProjection projection;
            Order order;

            switch (sorting.SortType)
            {
                case EmailSortType.LastSendAttemptDate:
                    projection = Projections.Property(() => EmailMessage.LastSendAttemptDate);
                    break;
                case EmailSortType.CreatedDate:
                    projection = Projections.Property(() => EmailMessage.CreatedDate);
                    break;
                case EmailSortType.SendStatus:
                    projection = Projections.Property(() => EmailMessage.EmailSendStatusType.Id);
                    break;

                default:
                    throw new Exception($"Ordering {sorting.SortType} not supported.");

            }

            switch (sorting.SortDirection)
            {
                case SortDirection.Ascending:
                    order = Order.Asc(projection);
                    break;
                case SortDirection.Descending:
                    order = Order.Desc(projection);
                    break;
                default:
                    order = Order.Asc(projection);
                    break;
            }

            return order;
        }

        private IQueryOver<EmailMessage, EmailMessage> BuildQuery()
        {
            var queryOver = NHibernateSession.Current.QueryOver(() => EmailMessage)
                .Inner.JoinAlias(x => EmailMessage.CreatedUser, () => CreatedUser)
                .Inner.JoinAlias(x => EmailMessage.EmailSendStatusType, () => EmailSendStatusType)
                .Left.JoinAlias(x => EmailMessage.RecipientEntity, () => RecipientEntity)
                .Left.JoinAlias(x => RecipientEntity.People, () => Person)
                .Left.JoinAlias(x => Person.LatestPersonName, () => LatestPersonName)
                .Left.JoinAlias(x => LatestPersonName.PersonName, () => PersonName)
                .Left.JoinAlias(x => EmailMessage.RecipientUser, () => RecipientUser);
              
            return queryOver;
        }


        
        private ProjectionList BuildProjections()
        {
            return Projections.ProjectionList()
                .Add(Projections.Property(() => EmailMessage.Id).WithAlias(() => mEmailMessageDto.EmailMessageId))
                .Add(Projections.Property(() => EmailMessage.CreatedDate).WithAlias(() => mEmailMessageDto.CreatedDate))
                .Add(Projections.Property(() => CreatedUser.FullName).WithAlias(() => mEmailMessageDto.CreatedUserName))
                .Add(Projections.Property(() => EmailMessage.RecipientEmail).WithAlias(() => mEmailMessageDto.RecipientEmail))
                .Add(Projections.Property(() => EmailMessage.LastSendAttemptDate).WithAlias(() => mEmailMessageDto.LastSendAttemptDate))
                .Add(Projections.Property(() => EmailMessage.LastSendResult).WithAlias(() => mEmailMessageDto.LastSendResult))
                .Add(Projections.Property(() => EmailMessage.Subject).WithAlias(() => mEmailMessageDto.Subject))
                .Add(Projections.Property(() => EmailSendStatusType.Id).WithAlias(() => mEmailMessageDto.EmailSendStatusTypeId))
                .Add(Projections.Property(() => EmailSendStatusType.DisplayName).WithAlias(() => mEmailMessageDto.EmailSendStatus));
        }

        private new Junction GetNaatiNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var naatiNumberList = criteria.ToList<S, int>();

            junction.Add(Restrictions.IsNotNull(Projections.Property(() => RecipientEntity.Id)));
            junction.Add<Entity>(e => RecipientEntity.NaatiNumber.IsIn(naatiNumberList));
            return junction;
        }

        private  Junction GetRecipientUserFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var naatiNumberList = criteria.ToList<S, int>();

            junction.Add(Restrictions.IsNotNull(Projections.Property(() => RecipientUser.Id)));
            junction.Add<Entity>(e => RecipientUser.Id.IsIn(naatiNumberList));
            return junction;
        }

        private Junction GetDateRequestedFromFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;

            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var createdDate = GetDateProjectionFrom(Projections.Property(() => EmailMessage.CreatedDate));
            junction.Add(Restrictions.Ge(createdDate, dateTime.Date));

            return junction;
        }

        private Junction GetDateRequestedToFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            if (criteria.Values.FirstOrDefault() == string.Empty)
            {
                return junction;
            }
            DateTime dateTime;
            if (!DateTime.TryParse(criteria.Values.FirstOrDefault(), out dateTime))
            {
                dateTime = DateTime.Now;
            }

            var createdDate = GetDateProjectionFrom(Projections.Property(() => EmailMessage.CreatedDate));
            junction.Add(Restrictions.Le(createdDate, dateTime.Date));

            return junction;
        }

        private Junction GetRecipientEmailFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var toAddress = Projections.Property(() => EmailMessage.RecipientEmail);
            return junction.Add(Restrictions.Like(toAddress, criteria.Values.FirstOrDefault()));
        }

        private Junction GetEmailStatusFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add<EmailMessage>(at => EmailMessage.EmailSendStatusType.Id.IsIn(typeList));
            return junction;
        }

        protected Junction GetRecipientFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var givenName = Projections.Property(() => PersonName.GivenName);
            var space = Projections.Constant(" ");
            var surName = Projections.Property(() => PersonName.Surname);
            var fullName = Concatenate(givenName, space, surName);

            return junction.Add(Restrictions.Like(fullName, criteria.Values.FirstOrDefault(), MatchMode.Anywhere));
        }

        protected Junction GetCustomerNumberFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var typeList = criteria.ToList<S, int>();
            junction.Add(Restrictions.IsNotNull(Projections.Property(() => RecipientEntity.Id)));
            junction.Add<EmailMessage>(at => RecipientEntity.NaatiNumber.IsIn(typeList));
            return junction;
        }

        protected Junction GetEmailMessageIdFilter<S>(ISearchCriteria<S> criteria, Junction junction)
        {
            var emailMessageIds = criteria.ToList<S, int>();
            junction.Add<EmailMessage>(c => EmailMessage.Id.IsIn(emailMessageIds));
            return junction;
        }
    }
}

