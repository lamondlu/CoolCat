using CoolCat.Core.Contracts;
using System.Collections.Generic;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class NotificationRegister : INotificationRegister
    {
        private static readonly Dictionary<string, List<INotificationHandler>>
            _containers = new Dictionary<string, List<INotificationHandler>>();

        public void Publish(string eventName, string data)
        {
            if (_containers.ContainsKey(eventName))
            {
                foreach (INotificationHandler item in _containers[eventName])
                {
                    item.Handle(data);
                }
            }
        }

        public void Subscribe(string eventName, INotificationHandler handler)
        {
            if (_containers.ContainsKey(eventName))
            {
                _containers[eventName].Add(handler);
            }
            else
            {
                _containers[eventName] = new List<INotificationHandler>() { handler };
            }
        }
    }
}
