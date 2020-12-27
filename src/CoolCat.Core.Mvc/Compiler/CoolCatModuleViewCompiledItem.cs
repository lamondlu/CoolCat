using CoolCat.Core.Consts;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class CoolCatModuleViewCompiledItem : RazorCompiledItem
    {
        public override string Identifier { get; }

        public override string Kind { get; }

        public override IReadOnlyList<object> Metadata { get; }

        public override Type Type { get; }

        public CoolCatModuleViewCompiledItem(RazorCompiledItemAttribute attr, string moduleName)
        {
            Type = attr.Type;
            Kind = attr.Kind;
            Identifier = $"/{GlobalConst.ModulePrefix}/{moduleName}{attr.Identifier}";

            Metadata = Type.GetCustomAttributes(inherit: true).Select(o =>
                o is RazorSourceChecksumAttribute rsca
                    ? new RazorSourceChecksumAttribute(rsca.ChecksumAlgorithm, rsca.Checksum, $"/{GlobalConst.ModulePrefix}/{moduleName}{rsca.Identifier}")
                    : o).ToList();
        }

    }
}
