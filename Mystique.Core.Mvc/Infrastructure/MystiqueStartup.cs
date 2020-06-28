using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Mystique.Core.BusinessLogic;
using Mystique.Core.Contracts;
using Mystique.Core.Helpers;
using Mystique.Core.Models;
using Mystique.Core.Repositories;
using Mystique.Mvc.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

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
            services.AddSingleton<IRazorPageFactoryProvider, RazorPageFactory>();

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

            AssemblyLoadContext.Default.Resolving += (context, assembly) =>
            {
                Func<CollectibleAssemblyLoadContext, bool> filter = p => p.Assemblies.Any(p => p.GetName().Name == assembly.Name
                                                        && p.GetName().Version == assembly.Version);

                if (PluginsLoadContexts.All().Any(filter))
                {
                    var ass = PluginsLoadContexts.All().First(filter)
                        .Assemblies.First(p => p.GetName().Name == assembly.Name
                        && p.GetName().Version == assembly.Version);
                    return ass;
                }

                return null;
            };

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });
        }


        public class RazorPageFactory : IRazorPageFactoryProvider
        {
            private readonly IViewCompilerProvider _viewCompilerProvider;

            /// <summary>
            /// Initializes a new instance of <see cref="DefaultRazorPageFactoryProvider"/>.
            /// </summary>
            /// <param name="viewCompilerProvider">The <see cref="IViewCompilerProvider"/>.</param>
            public RazorPageFactory(IViewCompilerProvider viewCompilerProvider)
            {
                _viewCompilerProvider = viewCompilerProvider;
            }

            private IViewCompiler Compiler => _viewCompilerProvider.GetCompiler();

            /// <inheritdoc />
            public RazorPageFactoryResult CreateFactory(string relativePath)
            {
                if (relativePath == null)
                {
                    throw new ArgumentNullException(nameof(relativePath));
                }

                if (relativePath.StartsWith("~/", StringComparison.Ordinal))
                {
                    // For tilde slash paths, drop the leading ~ to make it work with the underlying IFileProvider.
                    relativePath = relativePath.Substring(1);
                }


                var compileTask = Compiler.CompileAsync(relativePath);



                var viewDescriptor = compileTask.GetAwaiter().GetResult();

                var viewType = viewDescriptor.Type;
                if (viewType != null)
                {
                    var newExpression = Expression.New(viewType);
                    var pathProperty = viewType.GetTypeInfo().GetProperty(nameof(IRazorPage.Path));

                    // Generate: page.Path = relativePath;
                    // Use the normalized path specified from the result.
                    var propertyBindExpression = Expression.Bind(pathProperty, Expression.Constant(viewDescriptor.RelativePath));
                    var objectInitializeExpression = Expression.MemberInit(newExpression, propertyBindExpression);
                    var pageFactory = Expression
                        .Lambda<Func<IRazorPage>>(objectInitializeExpression)
                        .Compile();
                    return new RazorPageFactoryResult(viewDescriptor, pageFactory);
                }
                else
                {
                    return new RazorPageFactoryResult(viewDescriptor, razorPageFactory: null);
                }
            }
        }
    }
}
