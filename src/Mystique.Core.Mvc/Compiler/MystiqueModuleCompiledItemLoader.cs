using Microsoft.AspNetCore.Razor.Hosting;
using System;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class MystiqueModuleViewCompiledItemLoader : RazorCompiledItemLoader
    {
        public string ModuleName { get; }

        public MystiqueModuleViewCompiledItemLoader(string moduleName)
        {
            ModuleName = moduleName;
        }

        protected override RazorCompiledItem CreateItem(RazorCompiledItemAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            return new MystiqueModuleViewCompiledItem(attribute, ModuleName);
        }

    }
}
