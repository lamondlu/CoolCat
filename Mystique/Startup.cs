using Mystique.Core;
using Mystique.Core.BusinessLogics;
using Mystique.Core.Contracts;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using Mystique.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mystique
{
    public class MyAssemblyPart : AssemblyPart, ICompilationReferencesProvider
    {
        public MyAssemblyPart(Assembly assembly) : base(assembly) { }

        public IEnumerable<string> GetReferencePaths() => Array.Empty<string>();
    }

    public static class AdditionalReferencePathHolder
    {
        public static IList<string> AdditionalReferencePaths = new List<string>();
    }

    public class Startup
    {
        public IList<string> _presets = new List<string>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ConnectionStringSetting>(Configuration.GetSection("ConnectionStringSetting"));

            services.AddScoped<IPluginManager, PluginManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var mvcBuilders = services.AddMvc()
                .AddRazorRuntimeCompilation(o =>
                {
                    foreach (var item in _presets)
                    {
                        o.AdditionalReferencePaths.Add(item);
                    }

                    AdditionalReferencePathHolder.AdditionalReferencePaths = o.AdditionalReferencePaths;
                });

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
            services.AddSingleton(MyActionDescriptorChangeProvider.Instance);

            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var option = scope.ServiceProvider.GetService<MvcRazorRuntimeCompilationOptions>();


                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var allEnabledPlugins = unitOfWork.PluginRepository.GetAllEnabledPlugins();

                foreach (var plugin in allEnabledPlugins)
                {
                    var context = new CollectibleAssemblyLoadContext();
                    var moduleName = plugin.Name;
                    var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules\\{moduleName}\\{moduleName}.dll";

                    _presets.Add(filePath);
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {

                        var assembly = context.LoadFromStream(fs);

                        var controllerAssemblyPart = new MyAssemblyPart(assembly);

                        mvcBuilders.PartManager.ApplicationParts.Add(controllerAssemblyPart);
                        PluginsLoadContexts.AddPluginContext(plugin.Name, context);
                    }
                }
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "Customer",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                routes.MapControllerRoute(
                    name: "Customer",
                    pattern: "Modules/{area}/{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
