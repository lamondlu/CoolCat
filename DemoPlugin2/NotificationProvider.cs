using Mystique.Core.Contracts;
using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoPlugin2
{
    public class NotificationProvider : INotificationProvider
    {
        public Dictionary<string, List<INotification<EventBase>>> GetNotifications()
        {
            throw new NotImplementedException();
            //return new Dictionary<string, List<INotification<EventBase>>> { "", };
        }
    }
}
