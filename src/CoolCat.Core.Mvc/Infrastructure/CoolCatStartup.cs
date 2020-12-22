using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoolCat.Core.BusinessLogic;
using CoolCat.Core.Contracts;
using CoolCat.Core.Helpers;
using CoolCat.Core.Repositories;
using CoolCat.Core.Repository.MySql;
using CoolCat.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public static class CoolCatStartup
    {
        private static IServiceCollection _serviceCollection;

        public static IServiceCollection Services => _serviceCollection;

        public static void CoolCatSetup(this IServiceCollection services, IConfiguration configuration)
        {
            _serviceCollection = services;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IMvcModuleSetup, MvcModuleSetup>();
            services.AddScoped<IDbHelper, DbHelper>();
            services.AddScoped<IPluginManager, PluginManager>();
            services.AddScoped<ISystemManager, SystemManager>();
            services.AddScoped<IUnitOfWork, Repository.MySql.UnitOfWork>();
            services.AddSingleton<INotificationRegister, NotificationRegister>();
            services.AddSingleton<IActionDescriptorChangeProvider>(CoolCatActionDescriptorChangeProvider.Instance);
            services.AddSingleton<IReferenceContainer, DefaultReferenceContainer>();
            services.AddSingleton<IReferenceLoader, DefaultReferenceLoader>();

            var documentation = new CoolCatModuleDocumentation();

            services.AddSingleton<IQueryDocumentation>(documentation);
            services.AddSingleton(CoolCatActionDescriptorChangeProvider.Instance);

            IMvcBuilder mvcBuilder = services.AddMvc();

            ServiceProvider provider = services.BuildServiceProvider();
            using (IServiceScope scope = provider.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var dataStore = new DefaultDataStore(scope.ServiceProvider.GetService<ILogger<DefaultDataStore>>());
                services.AddSingleton<IDataStore>(dataStore);
                var contextProvider = new CollectibleAssemblyLoadContextProvider();

                if (unitOfWork.CheckDatabase())
                {
                    List<ViewModels.PluginListItemViewModel> allEnabledPlugins = unitOfWork.PluginRepository.GetAllEnabledPlugins();
                    IReferenceLoader loader = scope.ServiceProvider.GetService<IReferenceLoader>();

                    foreach (ViewModels.PluginListItemViewModel plugin in allEnabledPlugins)
                    {
                        var context = contextProvider.Get(plugin.Name, mvcBuilder, scope, dataStore, documentation);
                        PluginsLoadContexts.Add(plugin.Name, context);
                    }
                }
            }

            AssemblyLoadContextResoving();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.Replace<IViewCompilerProvider, CoolCatViewCompilerProvider>();
        }

        private static void AssemblyLoadContextResoving()
        {
            AssemblyLoadContext.Default.Resolving += (context, assembly) =>
            {
                Func<CollectibleAssemblyLoadContext, bool> filter = p => p.Assemblies.Any(p => p.GetName().Name == assembly.Name
                                                        && p.GetName().Version == assembly.Version);

                if (PluginsLoadContexts.All().Any(filter))
                {
                    Assembly ass = PluginsLoadContexts.All().First(filter)
                        .Assemblies.First(p => p.GetName().Name == assembly.Name
                        && p.GetName().Version == assembly.Version);
                    return ass;
                }

                return null;
            };
        }


    }

}
