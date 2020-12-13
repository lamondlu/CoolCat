using System;

namespace CoolCat.Core.Attributes
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
