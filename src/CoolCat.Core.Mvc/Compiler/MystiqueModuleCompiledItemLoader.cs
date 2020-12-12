using Microsoft.AspNetCore.Razor.Hosting;
using System;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class CoolCatModuleViewCompiledItemLoader : RazorCompiledItemLoader
    {
        public string ModuleName { get; }

        public CoolCatModuleViewCompiledItemLoader(string moduleName)
        {
            ModuleName = moduleName;
        }

        protected override RazorCompiledItem CreateItem(RazorCompiledItemAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            return new CoolCatModuleViewCompiledItem(attribute, ModuleName);
        }

    }
}
