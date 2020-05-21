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
        public Dictionary<string, List<INotificationHandler>> GetNotifications()
        {
            var handlers = new List<INotificationHandler> { new LoadHelloWorldEventHandler() };
            var result = new Dictionary<string, List<INotificationHandler>>();

            result.Add("LoadHelloWorldEvent", handlers);

            return result;
        }
    }
}
