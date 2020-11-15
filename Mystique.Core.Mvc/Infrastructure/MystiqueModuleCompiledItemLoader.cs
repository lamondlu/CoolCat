using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
