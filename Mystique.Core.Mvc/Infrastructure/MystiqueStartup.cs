﻿using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mystique.Core.BusinessLogics;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using Mystique.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mystique.Core.Mvc.Infrastructure
{
    public static class MystiqueStartup
    {
        private static IList<string> _presets = new List<string>();

        public static void MystiqueSetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ConnectionStringSetting>(configuration.GetSection("ConnectionStringSetting"));

            services.AddSingleton<IMvcModuleSetup, MvcModuleSetup>();
            services.AddScoped<IPluginManager, PluginManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<PluginPackage>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MystiqueActionDescriptorChangeProvider.Instance);
            services.AddSingleton<IReferenceContainer, DefaultReferenceContainer>();
            services.AddSingleton<IReferenceLoader, DefaultReferenceLoader>();
            services.AddSingleton(MystiqueActionDescriptorChangeProvider.Instance);

            var mvcBuilder = services.AddMvc();

            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var option = scope.ServiceProvider.GetService<MvcRazorRuntimeCompilationOptions>();

                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var allEnabledPlugins = unitOfWork.PluginRepository.GetAllEnabledPlugins();
                var loader = scope.ServiceProvider.GetService<IReferenceLoader>();

                foreach (var plugin in allEnabledPlugins)
                {
                    var context = new CollectibleAssemblyLoadContext();
                    var moduleName = plugin.Name;
                    var filePath = Path.Combine(Environment.CurrentDirectory, "Mystique_Plugins", moduleName, $"{moduleName}.dll");
                    var referenceFolderPath = Path.GetDirectoryName(filePath);

                    _presets.Add(filePath);
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var assembly = context.LoadFromStream(fs);
                        loader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                        var controllerAssemblyPart = new MystiqueAssemblyPart(assembly);
                        mvcBuilder.PartManager.ApplicationParts.Add(controllerAssemblyPart);
                        PluginsLoadContexts.AddPluginContext(plugin.Name, context);
                    }
                }
            }

            mvcBuilder.AddRazorRuntimeCompilation(o =>
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
        }
    }
}
