//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using F1Solutions.Naati.Common.Contracts.Dal.DTO;
//using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
//using F1Solutions.Naati.Common.Contracts.Dal.Response;
//using Microsoft.AspNet.SignalR;

//namespace F1Solutions.Naati.Common.Bl.Message
//{
//    /// <summary>
//    /// Exposed SignalR Hub: all public methods in this class are called by a Client Javascript
//    /// </summary>
//    public class MessageHub : Hub<IMessengerClient>
//    {
//        private readonly INotificationQueryService _notificationQueryService;
//        public MessageHub(INotificationQueryService notificationQueryService)
//        {
//            _notificationQueryService = notificationQueryService;
//        }

//        public Task<NotificationDto[]> CheckMessages()
//        {
//            var messages = GetMessages();
//            return Task.FromResult(messages);
//        }

//        private NotificationDto[] GetMessages()
//        {
//            var username = Context?.User?.Identity?.Name;
//            var notifications = _notificationQueryService.GetNotifications(new Contracts.Dal.Request.GetNotificationRequest
//            {
//                UserName = username,
//                ShowExpired = false
//            });

//            var sanitizedMessages = notifications.Select(_notificationQueryService.SanitizeNotification).ToArray();

//            return sanitizedMessages;
//        }

//        public Task<NotificationDto[]> MarkAsRead(int notificationId)
//        {
//            var username = Context.User?.Identity?.Name;
//            var notification = _notificationQueryService.GetNotificationById(notificationId);
//            if (username != null && notification?.Data?.ToUserName == username)
//            {
//                notification.Data.ExpiryDate = DateTime.Now;
//                _notificationQueryService.CreateOrUpdateNotification(notification.Data);
//            }

//            var messages = GetMessages();
//            return Task.FromResult(messages);
//        }

//        public Task<int> SimultaneousConnections()
//        {
//            var username = Context.User?.Identity?.Name;
//            var list = new List<string>();
//            list = UserHandler.ConnectedIds.GetOrAdd(username, list);
//            return Task.FromResult(list.Count);
//        }

//        public override Task OnConnected()
//        {
//            var username = Context.User?.Identity?.Name;
//            if (username != null)
//            {
//                var list = new List<string>();
//                list = UserHandler.ConnectedIds.GetOrAdd(username, list);
//                list.Add(Context.ConnectionId);
//                UserHandler.ConnectedIds.AddOrUpdate(username, list, (key, value) => list);
//            }
//            return base.OnConnected();
//        }

//        public override Task OnDisconnected(bool stopCalled)
//        {
//            var username = Context.User?.Identity?.Name;
//            if (username != null)
//            {
//                var list = new List<string>();
//                list = UserHandler.ConnectedIds.GetOrAdd(username, list);
//                list.Remove(Context.ConnectionId);
//                if (!list.Any())
//                {
//                    UserHandler.ConnectedIds.TryRemove(username, out list);
//                }
//                else
//                {
//                    UserHandler.ConnectedIds.AddOrUpdate(username, list, (key, value) => list);
//                }
//            }
//            return base.OnDisconnected(stopCalled);
//        }
//    }

//    public static class UserHandler
//    {
//        public static ConcurrentDictionary<string, List<string>> ConnectedIds = new ConcurrentDictionary<string, List<string>>();
//    }
//}