using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
using F1Solutions.Naati.Common.Dal.Domain.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    //public interface IRegistrationRequestRepository : IRepository<RegistrationRequest>
    //{
    //    bool AddRegistrationRequest(UserRegistrationRequest request);
    //    void DeleteRegistrationRequest(DeleteRegistrationRequest request);
    //    SearchResults<RegistrationRequest> Search(int start, int length, Dictionary<string, SortDirection> sortOrder);
    //    bool IsRegistrationRequestExist(UserRegistrationRequest request);
    //}

    //public class RegistrationRequestRepository : SecuredRepository<RegistrationRequest>, IRegistrationRequestRepository
    //{
    //    private const string NaatiNumber = "NaatiNumber";
    //    private const string Fullname = "FullName";
    //    private const string DateOfBirth = "DateOfBirth";
    //    private const string EmailAddress = "Email";
    //    private const string DateRequested = "DateTimeRequested";

    //    public RegistrationRequestRepository(ICustomSessionManager sessionManager, IDataSecurityProvider dataSecurityProvider) : base(sessionManager, dataSecurityProvider)
    //    {
    //    }

    //    #region IRegistrationRequestRepository Members

    //    public bool AddRegistrationRequest(UserRegistrationRequest request)
    //    {
    //        var registration = new RegistrationRequest
    //        {
    //            NaatiNumber = request.NaatiNumber,
    //            EmailAddress = request.Email,
    //            GivenName = request.GivenName,
    //            FamilyName = request.FamilyName,
    //            DateOfBirth = request.DateOfBirth,
    //            DateRequested = DateTime.Now,
    //            Gender = request.Gender,
    //            Title = request.Title
    //        };
    //        SaveOrUpdate(registration);

    //        return true;
    //    }


    //    public void DeleteRegistrationRequest(DeleteRegistrationRequest request)
    //    {
    //        var registrationRequest = Get(request.Id);

    //        Delete(registrationRequest);
    //    }

    //    public bool IsRegistrationRequestExist(UserRegistrationRequest request)
    //    {
    //        var query = Session.Query<RegistrationRequest>();
    //        var result = false;

    //        if (request.NaatiNumber != 0)
    //        {
    //            var q = from r in query
    //                    where r.NaatiNumber == request.NaatiNumber
    //                    select r;

    //            if (q.Any())
    //            {
    //                result = true;
    //            }
    //        }
    //        else
    //        {
    //            var q = from r in query
    //                    where r.GivenName == request.GivenName &&
    //                    r.FamilyName == request.FamilyName &&
    //                    r.DateOfBirth == request.DateOfBirth
    //                    select r;

    //            if (q.Any())
    //            {
    //                result = true;
    //            }
    //        }

    //        return result;
    //    }

    //    public SearchResults<RegistrationRequest> Search(int start, int length, Dictionary<string, SortDirection> sortOrder)
    //    {
    //        var query = Session.Query<RegistrationRequest>();

    //        var sorter = new ResultsSorter<RegistrationRequest>();
    //        sorter.DefineSortTerm(NaatiNumber, r => r.NaatiNumber);
    //        sorter.DefineSortTerm(Fullname, r => r.FamilyName, s => s.GivenName);
    //        sorter.DefineSortTerm(DateOfBirth, r => r.DateOfBirth);
    //        sorter.DefineSortTerm(EmailAddress, r => r.EmailAddress);
    //        sorter.DefineSortTerm(DateRequested, r => r.DateRequested);

    //        foreach (var sort in sortOrder)
    //        {
    //            query = sorter.IsDefined(sort.Key)
    //            ? sorter.Sort(query, sort.Key, sort.Value)
    //            : query.OrderBy(r => r.DateRequested).ThenBy(s => s.FamilyName).ThenBy(s => s.GivenName);
    //        }

    //        var totalCount = query.Count();

    //        query = query.Skip(start);

    //        if (length != -1)
    //        {
    //            query = query.Take(length);
    //        }

    //        var results = query.ToList();

    //        return new SearchResults<RegistrationRequest>
    //        {
    //            Results = results,
    //            TotalResultsCount = totalCount
    //        };
    //    }

    //    #endregion
    //}
}
