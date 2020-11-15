using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class MystiqueAssemblyPart : AssemblyPart, ICompilationReferencesProvider
    {
        public MystiqueAssemblyPart(Assembly assembly) : base(assembly) { }

        public IEnumerable<string> GetReferencePaths()
        {
            return Array.Empty<string>();
        }
    }

    public class MystiqueRazorAssemblyPart : ApplicationPart, IRazorCompiledItemProvider
    {
        public MystiqueRazorAssemblyPart(Assembly assembly, string areaName)
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
                var loader = new AreaCompiledItemLoader(AreaName);
                return loader.LoadItems(Assembly);
            }
        }
    }
    public class AreaCompiledItemLoader : RazorCompiledItemLoader
    {
        public string AreaName { get; }

        public AreaCompiledItemLoader(string areaName)
        {
            AreaName = areaName;
        }

        protected override RazorCompiledItem CreateItem(RazorCompiledItemAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            return new AreaRazorCompiledItem(attribute, AreaName);
        }
    }

    public class AreaRazorCompiledItem : RazorCompiledItem
    {
        public override string Identifier { get; }

        public override string Kind { get; }

        public override IReadOnlyList<object> Metadata { get; }

        public override Type Type { get; }

        public AreaRazorCompiledItem(RazorCompiledItemAttribute attr, string areaName)
        {
            Type = attr.Type;
            Kind = attr.Kind;
            Identifier = "/Modules/" + areaName + attr.Identifier;

            Metadata = Type.GetCustomAttributes(inherit: true).Select(o =>
                o is RazorSourceChecksumAttribute rsca
                    ? new RazorSourceChecksumAttribute(rsca.ChecksumAlgorithm, rsca.Checksum, "/Modules/" + areaName + rsca.Identifier)
                    : o).ToList();
        }
    }
}
