using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.DataAccess;

namespace F1Solutions.Naati.Common.Dal
{
    public class AuditLogQueryService : IAuditLogQueryService
    {
        public FindAuditLogsResponse FindAuditLogs(FindAuditLogsRequest request)
        {
            PagedResult<AuditLog> pagedResult = FindAuditLogs(request.AuditTypeId, request.Username, request.PageSize, request.PageNumber, request.FromDate, request.ToDate, request.RecordName, request.ParentID, request.RecordID, request.ParentName, request.ChangeDetailPart);

            return new FindAuditLogsResponse()
            {
                AuditLogDTOs = pagedResult.Results.Select(auditLog => new AuditLogDto
                {
                    AuditType =
                        auditLog.AuditType.Name,
                    ChangeDetail =
                        auditLog.ChangeDetail,
                    DateTime = auditLog.DateTime,
                    ParentID = auditLog.ParentID ?? 0,
                    RecordID = auditLog.RecordID,
                    RecordName = auditLog.RecordName,
                    Username =
                        auditLog.User.UserName
                }).ToArray(),
                TotalResultsCount = pagedResult.TotalResultsCount,
                LastPage = pagedResult.LastPage
            };
        }

        private PagedResult<AuditLog> FindAuditLogs(int? auditTypeID, string username, int pageSize, int pageNumber, DateTime? fromDate, DateTime? toDate, string recordName, int? parentID, int? recordID, string parentName, string changeDetailPart = null)
        {
           

            var query = from auditLog in NHibernateSession.Current.Query<AuditLog>()
                        join user in NHibernateSession.Current.Query<User>() on auditLog.User.Id equals user.Id
                join auditType in NHibernateSession.Current.Query<AuditType>() on auditLog.AuditType.Id equals
                auditType.Id
                select new
                {
                    AuditLog = auditLog,
                    User = user,
                    AuditType = auditType
                };

            query = query.OrderByDescending(x => x.AuditLog.DateTime).OrderByDescending(p => p.AuditLog.RecordName);

            if (parentID.HasValue && parentID != 0)
            {
                var transformedParentId = parentID != -1 ? parentID : null;
                query = query.Where(x => x.AuditLog.ParentID == transformedParentId);
            }

            if (recordID.HasValue && recordID != 0)
            {
                query = query.Where(x => x.AuditLog.RecordID == recordID);
            }

            if (!string.IsNullOrEmpty(recordName))
            {
                query = query.Where(x => x.AuditLog.RecordName == recordName);
            }
            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(x => x.User.UserName == username);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.AuditLog.DateTime >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.AuditLog.DateTime <= toDate);
            }

            if (auditTypeID.HasValue && auditTypeID != 0)
            {
                query = query.Where(x => x.AuditType.Id == auditTypeID);
            }

            if (!String.IsNullOrEmpty(changeDetailPart))
            {
                query = query.Where(x => x.AuditLog.ChangeDetail.Contains(changeDetailPart));
            }

            if (!String.IsNullOrEmpty(parentName))
            {
                query = query.Where(x => x.AuditLog.ParentName == parentName);
            }

            var pagedResult = new PagedResult<AuditLog>
            {
                Results = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList().Select(x => x.AuditLog),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalResultsCount = query.Count()
            };

            if (pagedResult.TotalResultsCount % pageSize == 0)
            {
                pagedResult.LastPage = (pagedResult.TotalResultsCount / pageSize);
            }
            else
            {
                pagedResult.LastPage = (pagedResult.TotalResultsCount / pageSize) + 1;
            }

            return pagedResult;
        }
    }
}
