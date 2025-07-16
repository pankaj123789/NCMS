using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface INotificationQueryService : IQueryService
    {
        IEnumerable<NotificationDto> GetNotifications(GetNotificationRequest request);
        ServiceResponse<NotificationDto> GetNotificationById(int notificationId);
        ServiceResponse<NotificationDto> CreateOrUpdateNotification(NotificationDto dto);
        NotificationDto SanitizeNotification(NotificationDto notification);

        IEnumerable<NotificationDto> GetSanitizeNotifications(IEnumerable<int> notificationIds);

        IEnumerable<NotificationDto> DeleteExpiredNotifications(DateTime expiredDate);
    }
}
