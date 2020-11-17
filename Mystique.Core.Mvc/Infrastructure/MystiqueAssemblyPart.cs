using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
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
}
