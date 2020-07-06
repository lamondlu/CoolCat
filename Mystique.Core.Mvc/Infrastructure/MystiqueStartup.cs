using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mystique.Core.BusinessLogic;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using Mystique.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Mystique.Core.Mvc.Infrastructure
{
    public static class MystiqueStartup
    {
        private static readonly IList<string> _presets = new List<string>();

        public static void MystiqueSetup(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddOptions();
            services.Configure<ConnectionStringSetting>(configuration.GetSection("ConnectionStringSetting"));

            services.AddSingleton<IMvcModuleSetup, MvcModuleSetup>();
            services.AddScoped<IPluginManager, PluginManager>();
            services.AddScoped<ISystemManager, SystemManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<INotificationRegister, NotificationRegister>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MystiqueActionDescriptorChangeProvider.Instance);
            services.AddSingleton<IReferenceContainer, DefaultReferenceContainer>();
            services.AddSingleton<IReferenceLoader, DefaultReferenceLoader>();
            services.AddSingleton(MystiqueActionDescriptorChangeProvider.Instance);

            IMvcBuilder mvcBuilder = services.AddMvc();

            ServiceProvider provider = services.BuildServiceProvider();
            using (IServiceScope scope = provider.CreateScope())
            {
                MvcRazorRuntimeCompilationOptions option = scope.ServiceProvider.GetService<MvcRazorRuntimeCompilationOptions>();

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

                        context.Enable();
                    }
                }
            }

            mvcBuilder.AddRazorRuntimeCompilation(o =>
            {
                foreach (string item in _presets)
                {
                    o.AdditionalReferencePaths.Add(item);
                }

                AdditionalReferencePathHolder.AdditionalReferencePaths = o.AdditionalReferencePaths;
            });

            AssemblyLoadContextResoving();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });
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
