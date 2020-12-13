using System.Collections.Generic;
using System.Linq;

namespace CoolCat.Core
{
    public static class PluginsLoadContexts
    {
        private static readonly Dictionary<string, CollectibleAssemblyLoadContext> _pluginContexts = null;

        static PluginsLoadContexts()
        {
            _pluginContexts = new Dictionary<string, CollectibleAssemblyLoadContext>();
        }

        public static List<CollectibleAssemblyLoadContext> All()
        {
            return _pluginContexts.Select(p => p.Value).ToList();
        }

        public static bool Any(string pluginName)
        {
            return _pluginContexts.ContainsKey(pluginName);
        }

        public static void Remove(string pluginName)
        {
            if (_pluginContexts.ContainsKey(pluginName))
            {
                _pluginContexts[pluginName].Unload();
                _pluginContexts.Remove(pluginName);
            }
        }

        public static CollectibleAssemblyLoadContext Get(string pluginName)
        {
            return _pluginContexts[pluginName];
        }

        public static void Add(string pluginName, CollectibleAssemblyLoadContext context)
        {
            _pluginContexts.Add(pluginName, context);
        }
    }
}
