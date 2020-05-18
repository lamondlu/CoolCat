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
        public Dictionary<string, List<INotification>> GetNotifications()
        {
            var handlers = new List<INotification> { new LoadHelloWorldEventHandler() };
            var result = new Dictionary<string, List<INotification>>();

            result.Add("LoadHelloWorldEvent", handlers);

            return result;
        }
    }
}
