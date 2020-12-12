using CoolCat.Core.Contracts;

namespace CoolCat.Core.DomainModel
{
    public class ModuleBase : IModule
    {
        public ModuleBase(string name)
        {
            Name = name;
            Version = "1.0.0";
        }

        public ModuleBase(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public ModuleBase(string name, Version version)
        {
            Name = name;
            Version = version;
        }

        public string Name
        {
            get;
            private set;
        }

        public Version Version
        {
            get;
            private set;
        }
    }
}
