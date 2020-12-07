using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Mystique.Core.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class CollectibleAssemblyLoadContextProvider
    {
        public CollectibleAssemblyLoadContext Get(string moduleName, IMvcBuilder mvcBuilder, IServiceScope scope, IDataStore dataStore)
        {
            CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(moduleName);
            IReferenceLoader loader = scope.ServiceProvider.GetService<IReferenceLoader>();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.dll");
            string viewFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.Views.dll");
            string referenceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName);

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                Assembly assembly = context.LoadFromStream(fs);

                context.SetEntryPoint(assembly);

                loader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                AssemblyPart controllerAssemblyPart = new AssemblyPart(assembly);
                mvcBuilder.PartManager.ApplicationParts.Add(controllerAssemblyPart);


                var resources = assembly.GetManifestResourceNames();

                if (resources.Any())
                {
                    foreach (var item in resources)
                    {
                        var stream = new MemoryStream();
                        var source = assembly.GetManifestResourceStream(item);
                        source.CopyTo(stream);

                        context.RegisterResource(item, stream.ToArray());
                    }
                }

                BuildNotificationProvider(assembly, scope);
                RegisterModuleQueries(dataStore, moduleName, assembly, scope);
            }

            using (FileStream fsView = new FileStream(viewFilePath, FileMode.Open))
            {
                Assembly viewAssembly = context.LoadFromStream(fsView);
                loader.LoadStreamsIntoContext(context, referenceFolderPath, viewAssembly);

                MystiqueRazorAssemblyPart moduleView = new MystiqueRazorAssemblyPart(viewAssembly, moduleName);
                mvcBuilder.PartManager.ApplicationParts.Add(moduleView);
            }

            context.Enable();

            return context;
        }

        public CollectibleAssemblyLoadContext Get(string moduleName, ApplicationPartManager apm, IServiceScope scope, IDataStore dataStore)
        {
            CollectibleAssemblyLoadContext context = new CollectibleAssemblyLoadContext(moduleName);
            IReferenceLoader loader = scope.ServiceProvider.GetService<IReferenceLoader>();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.dll");
            string viewFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName, $"{moduleName}.Views.dll");
            string referenceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", moduleName);

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                Assembly assembly = context.LoadFromStream(fs);

                context.SetEntryPoint(assembly);

                loader.LoadStreamsIntoContext(context, referenceFolderPath, assembly);

                AssemblyPart controllerAssemblyPart = new AssemblyPart(assembly);
                apm.ApplicationParts.Add(controllerAssemblyPart);

                BuildNotificationProvider(assembly, scope);
                RegisterModuleQueries(dataStore, moduleName, assembly, scope);
            }

            using (FileStream fsView = new FileStream(viewFilePath, FileMode.Open))
            {
                Assembly viewAssembly = context.LoadFromStream(fsView);
                loader.LoadStreamsIntoContext(context, referenceFolderPath, viewAssembly);

                MystiqueRazorAssemblyPart moduleView = new MystiqueRazorAssemblyPart(viewAssembly, moduleName);
                apm.ApplicationParts.Add(moduleView);
            }

            context.Enable();

            return context;
        }

        private static void BuildNotificationProvider(Assembly assembly, IServiceScope scope)
        {
            IEnumerable<Type> providers = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x == typeof(INotificationProvider)));

            if (providers.Any())
            {
                INotificationRegister register = scope.ServiceProvider.GetService<INotificationRegister>();

                IDbHelper dbHelper = scope.ServiceProvider.GetService<IDbHelper>();

                foreach (Type p in providers)
                {
                    INotificationProvider obj = (INotificationProvider)assembly.CreateInstance(p.FullName);
                    Dictionary<string, List<INotificationHandler>> result = obj.GetNotifications(dbHelper);

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

        private static void RegisterModuleQueries(IDataStore dataStore, string moduleName, Assembly assembly, IServiceScope scope)
        {
            IEnumerable<Type> queries = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x == typeof(IDataStoreQuery)));
            if (queries.Any())
            {
                var dbHelper = scope.ServiceProvider.GetService<IDbHelper>();

                foreach (Type p in queries)
                {
                    var constructor = p.GetConstructors().FirstOrDefault(p => p.GetParameters().Length == 1);

                    if (constructor != null)
                    {
                        IDataStoreQuery obj = (IDataStoreQuery)constructor.Invoke(new object[] { dbHelper });
                        dataStore.RegisterQuery(moduleName, obj.QueryName, obj.Query);
                    }
                    else
                    {
                        IDataStoreQuery obj = (IDataStoreQuery)assembly.CreateInstance(p.FullName);
                        dataStore.RegisterQuery(moduleName, obj.QueryName, obj.Query);
                    }
                }
            }
        }
    }
}
