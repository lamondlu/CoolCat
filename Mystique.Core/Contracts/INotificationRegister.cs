using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Contracts
{
    public interface INotificationRegister
    {
        void Subscribe(string eventName, INotification handler);

        void Publish(string eventName, string data);
    }
}
