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
        private Dictionary<Type, List<INotification<EventBase>>> _containers = new Dictionary<Type, List<INotification<EventBase>>>();

        public void Publish<T>(T e) where T : EventBase
        {
            if (_containers[e.GetType()] != null)
            {
                foreach (var item in _containers[e.GetType()])
                {
                    item.Handle(e);
                }
            }
        }

        public void Subscribe<T>(T e, INotification<T> handler) where T : EventBase
        {
            var type = e.GetType();

            if (_containers.ContainsKey(type))
            {
                _containers[type].Add(handler);
            }
            else
            {
                _containers[type] = new List<INotification<EventBase>>() { handler };
            }
        }
    }


}
