using Microsoft.AspNetCore.Mvc;
using Mystique.Core.Attributes;
using Mystique.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mystique.Core.Mvc.Extensions
{
    public static class CollectibleAssemblyLoadContextExtension
    {
        public static List<PageRouteViewModel> GetPages(this CollectibleAssemblyLoadContext context)
        {
            var entryPointAssembly = context.GetEntryPointAssembly();
            var result = new List<PageRouteViewModel>();

            if (entryPointAssembly == null || !context.IsEnabled)
            {
                return result;
            }

            var areaName = context.PluginName;

            var types = entryPointAssembly.GetExportedTypes().Where(p => p.BaseType == typeof(Controller));

            if (types.Any())
            {
                foreach (var type in types)
                {

                    var controllerName = type.Name.Replace("Controller", "");

                    var actions = type.GetMethods().Where(p => p.GetCustomAttributes(false).Any(x => x.GetType() == typeof(Page))).ToList();

                    foreach (var action in actions)
                    {
                        var actionName = action.Name;

                        var pageAttribute = (Page)action.GetCustomAttributes(false).First(p => p.GetType() == typeof(Page));
                        result.Add(new PageRouteViewModel(pageAttribute.Name, areaName, controllerName, actionName));
                    }
                }

                return result;
            }
            else
            {
                return result;
            }
        }
    }
}
