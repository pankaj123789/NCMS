using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Dal
{
    public class NotificationQueryService : INotificationQueryService
    {
        public IEnumerable<NotificationDto> GetNotifications(GetNotificationRequest request)
        {
            var query = NHibernateSession.Current.Query<Notification>();
            var now = DateTime.Now;
            return query
                .Where(n =>
                    n.ToUser.UserName == request.UserName
                    && (request.ShowExpired || n.ExpiryDate > now)
                )
                .OrderByDescending(n => n.CreatedDate)
                .Select(MapNotification)
                .ToList();
        }

        public NotificationDto SanitizeNotification(NotificationDto notification)
        {
            if (notification == null)
            {
                return notification;
            }

            var json = JsonConvert.SerializeObject(notification);

            switch (notification.NotificationTypeId)
            {
                case (int)NotificationTypeName.DownloadTestMaterial:
                    return JsonConvert.DeserializeObject<NotificationDto<NotificationDownloadTestMaterialParameterSanitizedDto>>(json);
                case (int)NotificationTypeName.ErrorMessage:
                    return JsonConvert.DeserializeObject<NotificationDto<ErrorMessageParameterSanitizedDto>>(json);
                default:
                    throw new NullReferenceException($"SanitizedType for {notification.NotificationTypeId} not found.");
            }
        }

        public IEnumerable<NotificationDto> GetSanitizeNotifications(IEnumerable<int> notificationIds)
        {
            var ids = notificationIds.ToList();
            if (!ids.Any())
            {
                return Enumerable.Empty<NotificationDto>();
            }

            var notifications = NHibernateSession.Current.Query<Notification>().Where(x => ids.Contains(x.Id))
                .Select(MapNotification).ToList();

            var sanitisedNotifications = notifications.Select(SanitizeNotification).ToList();
            return sanitisedNotifications;
        }

        public IEnumerable<NotificationDto> DeleteExpiredNotifications(DateTime expiredDate)
        {
            var notificationsToDelete =
                NHibernateSession.Current.Query<Notification>().Where(x => x.ExpiryDate < expiredDate).ToList();

            foreach (var notification in notificationsToDelete)
            {
                NHibernateSession.Current.Delete(notification);
            }

            return notificationsToDelete.Select(MapNotification).ToList();
        }

        private NotificationDto MapNotification(Notification notification)
        {
            NotificationDto dto;

            switch (notification.NotificationType.Id)
            {
                case (int)NotificationTypeName.DownloadTestMaterial:
                    dto = new NotificationDto<NotificationDownloadTestMaterialParameterDto>
                    {
                        Data = JsonConvert.DeserializeObject<NotificationDownloadTestMaterialParameterDto>(notification.Parameter)
                    };
                    break;
                case (int)NotificationTypeName.ErrorMessage:
                    dto = new NotificationDto<ErrorMessageParameterDto>
                    {
                        Data = JsonConvert.DeserializeObject<ErrorMessageParameterDto>(notification.Parameter)
                    };
                    break;
                default:
                    throw new NotSupportedException($"parameter type {notification.NotificationType.Id} not supported");
            }

            dto.Id = notification.Id;
            dto.CreatedDate = notification.CreatedDate;
            dto.ExpiryDate = notification.ExpiryDate;
            dto.FromUserName = notification.FromUser.UserName;
            dto.NotificationTypeId = notification.NotificationType.Id;
            dto.ToUserName = notification.ToUser.UserName;

            return dto;
        }

        public ServiceResponse<NotificationDto> GetNotificationById(int notificationId)
        {
            var notification = NHibernateSession.Current.Get<Notification>(notificationId);
            var response = new ServiceResponse<NotificationDto>();
            if (notification == null)
            {
                return response;
            }

            var dto = MapNotification(notification);
            response.Data = dto;
            return response;
        }

        public ServiceResponse<NotificationDto> CreateOrUpdateNotification(NotificationDto dto)
        {
            var fromUser = NHibernateSession.Current.Query<User>().First(x => x.UserName == dto.FromUserName);
            var toUser = NHibernateSession.Current.Query<User>().First(x => x.UserName == dto.ToUserName);
            var notification = new Notification();
            if (dto.Id > 0)
            {
                notification = NHibernateSession.Current.Load<Notification>(dto.Id);
            }

            notification.FromUser = fromUser;
            notification.ToUser = toUser;
            notification.NotificationType = NHibernateSession.Current.Load<NotificationType>(dto.NotificationTypeId);
            notification.Parameter = dto.Parameter;
            notification.CreatedDate = dto.CreatedDate;
            notification.ExpiryDate = dto.ExpiryDate;
            NHibernateSession.Current.Save(notification);
            dto.Id = notification.Id;
            return new ServiceResponse<NotificationDto>() { Data = dto };
        }
    }
}
