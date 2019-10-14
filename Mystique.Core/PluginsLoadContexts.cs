using System.Collections.Generic;

namespace Mystique.Core
{
    public static class PluginsLoadContexts
    {
        private static readonly Dictionary<string, CollectibleAssemblyLoadContext> pluginContexts = new Dictionary<string, CollectibleAssemblyLoadContext>();

        public static bool Any(string pluginName) => pluginContexts.ContainsKey(pluginName);

        public static void RemovePluginContext(string pluginName)
        {
            if (pluginContexts.ContainsKey(pluginName))
            {
                pluginContexts[pluginName].Unload();
                pluginContexts.Remove(pluginName);
            }
        }

        public static CollectibleAssemblyLoadContext GetContext(string pluginName) => pluginContexts[pluginName];

        public static void AddPluginContext(string pluginName, CollectibleAssemblyLoadContext context) => pluginContexts.Add(pluginName, context);
    }
}
