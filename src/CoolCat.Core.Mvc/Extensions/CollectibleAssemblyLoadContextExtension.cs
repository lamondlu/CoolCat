using Microsoft.AspNetCore.Mvc;
using CoolCat.Core.Attributes;
using CoolCat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoolCat.Core.Mvc.Extensions
{
    public static class CollectibleAssemblyLoadContextExtension
    {
        public static List<PageRouteViewModel> GetPages(this CollectibleAssemblyLoadContext context)
        {
            System.Reflection.Assembly entryPointAssembly = context.GetEntryPointAssembly();
            List<PageRouteViewModel> result = new List<PageRouteViewModel>();

            if (entryPointAssembly == null || !context.IsEnabled)
            {
                return result;
            }

            string areaName = context.PluginName;

            IEnumerable<Type> types = entryPointAssembly.GetExportedTypes().Where(p => p.BaseType == typeof(Controller));

            if (types.Any())
            {
                foreach (Type type in types)
                {

                    string controllerName = type.Name.Replace("Controller", "");

                    List<System.Reflection.MethodInfo> actions = type.GetMethods().Where(p => p.GetCustomAttributes(false).Any(x => x.GetType() == typeof(Page))).ToList();

                    foreach (System.Reflection.MethodInfo action in actions)
                    {
                        string actionName = action.Name;

                        Page pageAttribute = (Page)action.GetCustomAttributes(false).First(p => p.GetType() == typeof(Page));
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
