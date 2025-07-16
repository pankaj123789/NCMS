using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using NHibernate;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    //public interface IDownloadRepository : IRepository<Download>
    //{
    //    IList<Download> GetDownloadsForReferenceNumber(string referenceNumber);
    //    void DeleteExistingDownloadsForOrder(string referenceNumber);
    //}

    //public class DownloadRepository : Repository<Download>, IDownloadRepository
    //{
    //    public DownloadRepository(ICustomSessionManager sessionManager)
    //        : base(sessionManager)
    //    {

    //    }

    //    public IList<Download> GetDownloadsForReferenceNumber(string referenceNumber)
    //    {
    //        return Session.Query<Download>().Where(d => d.ReferenceNumber == referenceNumber).ToList();
    //    }

    //    public void DeleteExistingDownloadsForOrder(string referenceNumber)
    //    {
    //        Session.Delete("select D from Download D where D.ReferenceNumber = :ref", referenceNumber,
    //                       NHibernateUtil.String);            
    //    }

    //}
}
