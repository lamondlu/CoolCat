using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class CoolCatAssemblyPart : AssemblyPart
    {
        public CoolCatAssemblyPart(Assembly assembly) : base(assembly) { }
    }
}
