using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using System;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/messenger")]
    public class MessengerController : BaseApiController
    {
        private readonly INotificationQueryService _notificationQueryService;

        public MessengerController(INotificationQueryService notificationQueryService)
        {
            _notificationQueryService = notificationQueryService;
        }

        public HttpResponseMessage Get()
        {
            var messages = GetMessages(CurrentPrincipal.Identity.Name);
            return this.CreateResponse(() => messages);
        }

        private NotificationDto[] GetMessages(string userName)
        {
            var notifications = _notificationQueryService.GetNotifications(new GetNotificationRequest
            {
                UserName = userName,
                ShowExpired = false
            });

            var sanitizedMessages = notifications.Select(_notificationQueryService.SanitizeNotification).ToArray();

            return sanitizedMessages;
        }

        [HttpPost]
        [Route("markAsRead/{id}")]
        public HttpResponseMessage MarkAsRead([FromUri] int id)
        {
            var username = CurrentPrincipal.Identity.Name;
            var notification = _notificationQueryService.GetNotificationById(id);
            if (username != null && notification?.Data?.ToUserName == username)
            {
                notification.Data.ExpiryDate = DateTime.Now;
                _notificationQueryService.CreateOrUpdateNotification(notification.Data);
            }

            return Get();
        }
    }
}
