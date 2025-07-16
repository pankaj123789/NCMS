using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Bl.NotificationScheduler
{
    public interface INotificationScheduler
    {
        void EnqueueNotification(NotificationDto notification);
    }
}
