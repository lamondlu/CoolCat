using Mystique.Core.Contracts;
using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class NotificationRegister : INotificationRegister
    {
        private static Dictionary<string, List<INotification>> _containers = new Dictionary<string, List<INotification>>();

        public void Publish(string eventName, string data)
        {
            if (_containers.ContainsKey(eventName))
            {
                foreach (var item in _containers[eventName])
                {
                    item.Handle(data);
                }
            }
        }

        public void Subscribe(string eventName, INotification handler)
        {
            if (_containers.ContainsKey(eventName))
            {
                _containers[eventName].Add(handler);
            }
            else
            {
                _containers[eventName] = new List<INotification>() { handler };
            }
        }
    }


}
