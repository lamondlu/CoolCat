using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Mystique.Core.BusinessLogic;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Repositories;
using Mystique.Mvc.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Mystique.Core.Mvc.Infrastructure
{
   
    

    

    public static class MystiqueStartup
    {
        private static readonly IList<string> _presets = new List<string>();
        private static IServiceCollection _serviceCollection;

        public static IServiceCollection Services => _serviceCollection;

        public static void MystiqueSetup(this IServiceCollection services, IConfiguration configuration)
        {
            _serviceCollection = services;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IMvcModuleSetup, MvcModuleSetup>();
            services.AddScoped<IPluginManager, PluginManager>();
            services.AddScoped<ISystemManager, SystemManager>();
            services.AddScoped<IUnitOfWork, Repository.MySql.UnitOfWork>();
            services.AddSingleton<INotificationRegister, NotificationRegister>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MystiqueActionDescriptorChangeProvider.Instance);
            services.AddSingleton<IReferenceContainer, DefaultReferenceContainer>();
            services.AddSingleton<IReferenceLoader, DefaultReferenceLoader>();
            services.AddSingleton(MystiqueActionDescriptorChangeProvider.Instance);

            IMvcBuilder mvcBuilder = services.AddMvc();

            ServiceProvider provider = services.BuildServiceProvider();
            using (IServiceScope scope = provider.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                if (unitOfWork.CheckDatabase())
                {
                    List<ViewModels.PluginListItemViewModel> allEnabledPlugins = unitOfWork.PluginRepository.GetAllEnabledPlugins();
                    IReferenceLoader loader = scope.ServiceProvider.GetService<IReferenceLoader>();

                    foreach (ViewModels.PluginListItemViewModel plugin in allEnabledPlugins)
                    {
                        CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(plugin.Name);
                        string moduleName = plugin.Name;

                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.dll");
                        string viewFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.Views.dll");
                        string referenceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName);

                        _presets.Add(filePath);
                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {
                            Assembly assembly = context.LoadFromStream(fs);
                            context.SetEntryPoint(assembly);

                            loader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                            MystiqueAssemblyPart controllerAssemblyPart = new MystiqueAssemblyPart(assembly);
                            mvcBuilder.PartManager.ApplicationParts.Add(controllerAssemblyPart);
                            PluginsLoadContexts.Add(plugin.Name, context);

                            BuildNotificationProvider(assembly, scope);
                        }

                        using (FileStream fsView = new FileStream(viewFilePath, FileMode.Open))
                        {
                            Assembly viewAssembly = context.LoadFromStream(fsView);
                            loader.LoadStreamsIntoContext(context, referenceFolderPath, viewAssembly);

                            MystiqueRazorAssemblyPart moduleView = new MystiqueRazorAssemblyPart(viewAssembly, moduleName);
                            mvcBuilder.PartManager.ApplicationParts.Add(moduleView);
                        }

                        context.Enable();
                    }
                }
            }

            AssemblyLoadContextResoving();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.Replace<IViewCompilerProvider, MystiqueViewCompilerProvider>();
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

        private static void BuildNotificationProvider(Assembly assembly, IServiceScope scope)
        {
            IEnumerable<Type> providers = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x.Name == "INotificationProvider"));

            if (providers.Any())
            {
                INotificationRegister register = scope.ServiceProvider.GetService<INotificationRegister>();

                foreach (Type p in providers)
                {
                    INotificationProvider obj = (INotificationProvider)assembly.CreateInstance(p.FullName);
                    Dictionary<string, List<INotificationHandler>> result = obj.GetNotifications();

                    foreach (KeyValuePair<string, List<INotificationHandler>> item in result)
                    {
                        foreach (INotificationHandler i in item.Value)
                        {
                            register.Subscribe(item.Key, i);
                        }
                    }
                }
            }
        }
    }
}
