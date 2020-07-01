using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Page : Attribute
    {
        public Page(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
