//using System;
//using System.Collections.Generic;
//using System.Linq;
//using F1Solutions.Naati.Common.Contracts.Bl.Message;
//using F1Solutions.Naati.Common.Contracts.Dal.DTO;
//using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
//using Microsoft.AspNet.SignalR;

//namespace F1Solutions.Naati.Common.Bl.Message
//{
//    public class Messenger : IMessenger
//    {
//        private readonly INotificationQueryService _notificationQueryService;

//        public Messenger(INotificationQueryService notificationQueryService)
//        {
//            _notificationQueryService = notificationQueryService;
//        }
//        public static IHubContext<IMessengerClient> GetHubContext()
//        {
//            return GlobalHost.ConnectionManager.GetHubContext<MessageHub, IMessengerClient>();
//        }
//        public void Notify(IEnumerable<int> notificationIds)
//        {
//            var notifications = _notificationQueryService.GetSanitizeNotifications(notificationIds);

//            foreach (var notification in notifications)
//            {
//                GetHubContext().Clients.User(notification.ToUserName).notify(notification);
//            }
//        }
//        public void Refresh(IEnumerable<string> usernames)
//        {
//            foreach (var userName in usernames.Distinct())
//            {
//                var messages = GetMessages(userName);
//                GetHubContext().Clients.User(userName).refresh(messages);
//            }
            
//        }

//        private NotificationDto[] GetMessages(string userName)
//        {            
//            var notifications = _notificationQueryService.GetNotifications(new Contracts.Dal.Request.GetNotificationRequest
//            {
//                UserName = userName,
//                ShowExpired = false
//            });

//            var sanitizedMessages = notifications.Select(_notificationQueryService.SanitizeNotification).ToArray();

//            return sanitizedMessages;
//        }
//    }
//}