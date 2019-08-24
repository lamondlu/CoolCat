using Mystique.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mystique
{

    public static class PluginsLoadContexts
    {
        private static Dictionary<string, CollectibleAssemblyLoadContext> _pluginContexts = null;

        static PluginsLoadContexts()
        {
            _pluginContexts = new Dictionary<string, CollectibleAssemblyLoadContext>();
        }

        public static bool Any(string pluginName)
        {
            return _pluginContexts.ContainsKey(pluginName);
        }

        public static void RemovePluginContext(string pluginName)
        {
            if (_pluginContexts.ContainsKey(pluginName))
            {
                _pluginContexts[pluginName].Unload();
                _pluginContexts.Remove(pluginName);
            }
        }

        public static CollectibleAssemblyLoadContext GetContext(string pluginName)
        {
            return _pluginContexts[pluginName];
        }

        public static void AddPluginContext(string pluginName, CollectibleAssemblyLoadContext context)
        {
            _pluginContexts.Add(pluginName, context);
        }
    }
}
