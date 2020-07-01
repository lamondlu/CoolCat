using Mystique.Core.ViewModels;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;
using System;

namespace Mystique.Core
{
    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        private Assembly _entryPoint = null;
        private bool _isEnabled = false;
        private string _pluginName = string.Empty;

        public CollectibleAssemblyLoadContext(string pluginName) : base(isCollectible: true)
        {
            _pluginName = pluginName;
        }

        public string PluginName
        {
            get
            {
                return _pluginName;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void SetEntryPoint(Assembly entryPoint)
        {
            _entryPoint = entryPoint;
        }

        public Assembly GetEntryPointAssembly()
        {
            return _entryPoint;
        }

        protected override Assembly Load(AssemblyName name)
        {
            return null;
        }
    }
}
