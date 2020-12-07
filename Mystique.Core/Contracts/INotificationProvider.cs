using System.Collections.Generic;

namespace Mystique.Core.Contracts
{
    public interface INotificationProvider
    {
        Dictionary<string, List<INotificationHandler>> GetNotifications(IDbHelper dbHelper);
    }
}
