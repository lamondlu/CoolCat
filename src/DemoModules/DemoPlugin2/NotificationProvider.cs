using CoolCat.Core.Contracts;
using System.Collections.Generic;

namespace DemoPlugin2
{
    public class NotificationProvider : INotificationProvider
    {
        public Dictionary<string, List<INotificationHandler>> GetNotifications(IDbHelper dbHelper)
        {
            List<INotificationHandler> handlers = new List<INotificationHandler> { new LoadHelloWorldEventHandler() };
            Dictionary<string, List<INotificationHandler>> result = new Dictionary<string, List<INotificationHandler>>
            {
                { "LoadHelloWorldEvent", handlers }
            };

            return result;
        }
    }
}
