using Mystique.Core.Contracts;
using System.Collections.Generic;

namespace BookInventory
{
    public class NotificationProvider : INotificationProvider
    {
        public Dictionary<string, List<INotificationHandler>> GetNotifications(IDbHelper dbHelper)
        {
            List<INotificationHandler> inHandlers = new List<INotificationHandler> { new BookInEventHandler(dbHelper) };
            List<INotificationHandler> outHandlers = new List<INotificationHandler> { new BookOutEventHandler(dbHelper) };
            Dictionary<string, List<INotificationHandler>> result = new Dictionary<string, List<INotificationHandler>>
            {
                { "BookInEvent", inHandlers },
                { "BookOutEvent",outHandlers }
            };

            return result;
        }
    }
}
