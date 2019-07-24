using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicPluginsDemoSite
{
    public static class ABC
    {
        public static IServiceCollection ServiceCollection { get; set; }
    }
}
