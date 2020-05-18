using Mystique.Core.Contracts;
using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoPlugin2
{
    public class LoadHelloWorldEventHandler : INotification
    {
        public void Handle(string data)
        {
            Console.WriteLine("Plugin2 handled hello world events." + data);
        }
    }

    public class LoadHelloWorldEvent : EventBase
    {
        public string Str { get; set; }

        public LoadHelloWorldEvent() : base("LoadHelloWorldEvent")
        {

        }
    }
}
