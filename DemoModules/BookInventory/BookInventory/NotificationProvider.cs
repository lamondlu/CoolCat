using Mystique.Core.Contracts;
using System.Collections.Generic;

namespace BookInventory
{
    public class NotificationProvider : INotificationProvider
    {
        public Dictionary<string, List<INotificationHandler>> GetNotifications()
        {
            List<INotificationHandler> handlers = new List<INotificationHandler> { new BookInEventHandler() };
            Dictionary<string, List<INotificationHandler>> result = new Dictionary<string, List<INotificationHandler>>
            {
                { "BookDeliverredEvent", handlers }
            };

            return result;
        }
    }
}
