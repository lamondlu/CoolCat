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
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            return services.Replace<TService>(typeof(TImplementation));
        }

        public static IServiceCollection Replace<TService>(this IServiceCollection services, Type implementationType)
        {
            return services.Replace(typeof(TService), implementationType);
        }

        public static IServiceCollection Replace(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!services.TryGetDescriptors(serviceType, out var descriptors))
            {
                throw new ArgumentException($"No services found for {serviceType.FullName}.", nameof(serviceType));
            }

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);

                services.Insert(index, descriptor.WithImplementationType(implementationType));

                services.Remove(descriptor);
            }

            return services;
        }

        private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors)
        {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
        }

        private static ServiceDescriptor WithImplementationType(this ServiceDescriptor descriptor, Type implementationType)
        {
            return new ServiceDescriptor(descriptor.ServiceType, implementationType, descriptor.Lifetime);
        }
    }
    public class MyViewCompilerProvider : IViewCompilerProvider
    {
        private MyViewCompiler _compiler;
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly ILoggerFactory _loggerFactory;

        public MyViewCompilerProvider(
            ApplicationPartManager applicationPartManager,
            ILoggerFactory loggerFactory)
        {
            _applicationPartManager = applicationPartManager;
            _loggerFactory = loggerFactory;
            Modify();
        }

        public void Modify()
        {
            var feature = new ViewsFeature();
            _applicationPartManager.PopulateFeature(feature);

            _compiler = new MyViewCompiler(feature.ViewDescriptors, _loggerFactory.CreateLogger<MyViewCompiler>());
        }

        public IViewCompiler GetCompiler() => _compiler;
    }

    public class MyViewCompiler : IViewCompiler
    {
        private readonly Dictionary<string, Task<CompiledViewDescriptor>> _compiledViews;
        private readonly ConcurrentDictionary<string, string> _normalizedPathCache;
        private readonly ILogger _logger;

        public MyViewCompiler(
            IList<CompiledViewDescriptor> compiledViews,
            ILogger logger)
        {
            if (compiledViews == null)
            {
                throw new ArgumentNullException(nameof(compiledViews));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
            _normalizedPathCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

            // We need to validate that the all of the precompiled views are unique by path (case-insensitive).
            // We do this because there's no good way to canonicalize paths on windows, and it will create
            // problems when deploying to linux. Rather than deal with these issues, we just don't support
            // views that differ only by case.
            _compiledViews = new Dictionary<string, Task<CompiledViewDescriptor>>(
                compiledViews.Count,
                StringComparer.OrdinalIgnoreCase);

            foreach (var compiledView in compiledViews)
            {

                if (!_compiledViews.ContainsKey(compiledView.RelativePath))
                {
                    // View ordering has precedence semantics, a view with a higher precedence was not
                    // already added to the list.
                    _compiledViews.Add(compiledView.RelativePath, Task.FromResult(compiledView));
                }
            }

            if (_compiledViews.Count == 0)
            {

            }
        }

        /// <inheritdoc />
        public Task<CompiledViewDescriptor> CompileAsync(string relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            // Attempt to lookup the cache entry using the passed in path. This will succeed if the path is already
            // normalized and a cache entry exists.
            if (_compiledViews.TryGetValue(relativePath, out var cachedResult))
            {

                return cachedResult;
            }

            var normalizedPath = GetNormalizedPath(relativePath);
            if (_compiledViews.TryGetValue(normalizedPath, out cachedResult))
            {

                return cachedResult;
            }

            // Entry does not exist. Attempt to create one.

            return Task.FromResult(new CompiledViewDescriptor
            {
                RelativePath = normalizedPath,
                ExpirationTokens = Array.Empty<IChangeToken>(),
            });
        }

        private string GetNormalizedPath(string relativePath)
        {
            Debug.Assert(relativePath != null);
            if (relativePath.Length == 0)
            {
                return relativePath;
            }

            if (!_normalizedPathCache.TryGetValue(relativePath, out var normalizedPath))
            {
                normalizedPath = NormalizePath(relativePath);
                _normalizedPathCache[relativePath] = normalizedPath;
            }

            return normalizedPath;
        }

        public static string NormalizePath(string path)
        {
            var addLeadingSlash = path[0] != '\\' && path[0] != '/';
            var transformSlashes = path.IndexOf('\\') != -1;

            if (!addLeadingSlash && !transformSlashes)
            {
                return path;
            }

            var length = path.Length;
            if (addLeadingSlash)
            {
                length++;
            }

            return string.Create(length, (path, addLeadingSlash), (span, tuple) =>
            {
                var (pathValue, addLeadingSlashValue) = tuple;
                var spanIndex = 0;

                if (addLeadingSlashValue)
                {
                    span[spanIndex++] = '/';
                }

                foreach (var ch in pathValue)
                {
                    span[spanIndex++] = ch == '\\' ? '/' : ch;
                }
            });
        }
    }

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
                //MvcRazorRuntimeCompilationOptions option = scope.ServiceProvider.GetService<MvcRazorRuntimeCompilationOptions>();

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

                        using (FileStream fs1 = new FileStream(viewFilePath, FileMode.Open))
                        {
                            Assembly view = context.LoadFromStream(fs1);
                            loader.LoadStreamsIntoContext(context, referenceFolderPath, view);


                            MystiqueRazorAssemblyPart ap = new MystiqueRazorAssemblyPart(view, moduleName);
                            mvcBuilder.PartManager.ApplicationParts.Add(ap);
                        }

                        context.Enable();
                    }
                }
            }

            //mvcBuilder.AddRazorRuntimeCompilation(o =>
            //{
            //    foreach (string item in _presets)
            //    {
            //        o.AdditionalReferencePaths.Add(item);
            //    }

            //    AdditionalReferencePathHolder.AdditionalReferencePaths = o.AdditionalReferencePaths;
            //});

            AssemblyLoadContextResoving();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.Replace<IViewCompilerProvider, MyViewCompilerProvider>();
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
