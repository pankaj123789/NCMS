using System;
using System.Threading;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.NotificationScheduler;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Bl.BackgroundOperation
{
    public abstract class BaseBackgroundOperation : BaseBackgroundTask
    {
        private readonly INotificationQueryService _notificationQueryService;
        private readonly INotificationScheduler _notificationScheduler;

        protected BaseBackgroundOperation(ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService, INotificationQueryService notificationQueryService, INotificationScheduler notificationScheduler) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _notificationQueryService = notificationQueryService;
            _notificationScheduler = notificationScheduler;
        }

        protected GenericResponse<NotificationDto<T>> CreateNotification<T>(NotificationTypeName notificationType,
            string toUserName,
            T parameter,
            DateTime expiryDate)
        {
            var notificationDto = new NotificationDto<T>
            {
                CreatedDate = DateTime.Now,
                ExpiryDate = expiryDate,
                FromUserName = Thread.CurrentPrincipal.Identity.Name,
                ToUserName = toUserName,
                NotificationTypeId = (int)notificationType,
                Data = parameter
            };

            _notificationQueryService.CreateOrUpdateNotification(notificationDto);
            return new GenericResponse<NotificationDto<T>>(notificationDto);
        }

        protected void SendNotification(NotificationDto notification)
        {
            _notificationScheduler.EnqueueNotification(notification);
        }
    }
}
