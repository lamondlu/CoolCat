using System.Collections.Generic;

namespace CoolCat.Core.Contracts
{
    public interface INotificationProvider
    {
        Dictionary<string, List<INotificationHandler>> GetNotifications(IDbConnectionFactory dbConnectionFactory);
    }
}
