using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Contracts
{
    public interface INotificationRegister
    {
        void Subscribe<T>(T e, INotification<T> handler) where T : EventBase;

        void Publish<T>(T e) where T : EventBase;
    }
}
