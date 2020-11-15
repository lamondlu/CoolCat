using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class MystiqueModuleViewCompiledItem : RazorCompiledItem
    {
        public override string Identifier { get; }

        public override string Kind { get; }

        public override IReadOnlyList<object> Metadata { get; }

        public override Type Type { get; }

        public MystiqueModuleViewCompiledItem(RazorCompiledItemAttribute attr, string moduleName)
        {
            Type = attr.Type;
            Kind = attr.Kind;
            Identifier = "/Modules/" + moduleName + attr.Identifier;

            Metadata = Type.GetCustomAttributes(inherit: true).Select(o =>
                o is RazorSourceChecksumAttribute rsca
                    ? new RazorSourceChecksumAttribute(rsca.ChecksumAlgorithm, rsca.Checksum, "/Modules/" + moduleName + rsca.Identifier)
                    : o).ToList();
        }

    }
}
