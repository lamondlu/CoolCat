using CoolCat.Core.Contracts;
using System.Collections.Generic;

namespace BookInventory
{
    public class NotificationProvider : INotificationProvider
    {
        public Dictionary<string, List<INotificationHandler>> GetNotifications(IDbConnectionFactory dbConnectionFactory)
        {
            List<INotificationHandler> inHandlers = new List<INotificationHandler> { new BookInEventHandler(dbConnectionFactory) };
            List<INotificationHandler> outHandlers = new List<INotificationHandler> { new BookOutEventHandler(dbConnectionFactory) };
            Dictionary<string, List<INotificationHandler>> result = new Dictionary<string, List<INotificationHandler>>
            {
                { "BookInEvent", inHandlers },
                { "BookOutEvent",outHandlers }
            };

            return result;
        }
    }
}
