using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class CoolCatRazorAssemblyPart : ApplicationPart, IRazorCompiledItemProvider
    {
        public CoolCatRazorAssemblyPart(Assembly assembly, string areaName)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            AreaName = areaName;
        }

        public string AreaName { get; }

        public Assembly Assembly { get; }

        public override string Name => Assembly.GetName().Name;

        IEnumerable<RazorCompiledItem> IRazorCompiledItemProvider.CompiledItems
        {
            get
            {
                var loader = new CoolCatModuleViewCompiledItemLoader(AreaName);
                return loader.LoadItems(Assembly);
            }
        }
    }
}
