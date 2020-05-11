using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Models
{
    public abstract class EventBase
    {
        public string Name { get; set; }

        public EventBase(string name)
        {
            Name = name;
        }
    }
}
