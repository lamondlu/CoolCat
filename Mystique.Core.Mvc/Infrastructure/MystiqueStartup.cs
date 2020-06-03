using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mystique.Core.BusinessLogic;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Mystique.Core.Repository.MySql;
using Mystique.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                List<ViewModels.PluginListItemViewModel> allEnabledPlugins = unitOfWork.PluginRepository.GetAllEnabledPlugins();
                IReferenceLoader loader = scope.ServiceProvider.GetService<IReferenceLoader>();

                foreach (ViewModels.PluginListItemViewModel plugin in allEnabledPlugins)
                {
                    CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(plugin.Name);
                    string moduleName = plugin.Name;
                    string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules/{moduleName}/{moduleName}.dll";
                    string referenceFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules/{moduleName}";

                    _presets.Add(filePath);
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        System.Reflection.Assembly assembly = context.LoadFromStream(fs);
                        context.SetEntryPoint(assembly);
                        loader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                        MystiqueAssemblyPart controllerAssemblyPart = new MystiqueAssemblyPart(assembly);
                        mvcBuilder.PartManager.ApplicationParts.Add(controllerAssemblyPart);
                        PluginsLoadContexts.Add(plugin.Name, context);

                        var providers = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x.Name == "INotificationProvider"));

                        if (providers.Any())
                        {
                            var register = scope.ServiceProvider.GetService<INotificationRegister>();

                            foreach (var p in providers)
                            {
                                var obj = (INotificationProvider)assembly.CreateInstance(p.FullName);
                                var result = obj.GetNotifications();

                                foreach (var item in result)
                                {
                                    foreach (var i in item.Value)
                                    {
                                        register.Subscribe(item.Key, i);
                                    }
                                }
                            }
                        }
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

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });
        }
    }
}
