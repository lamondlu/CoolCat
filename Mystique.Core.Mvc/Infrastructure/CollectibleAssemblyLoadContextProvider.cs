using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mystique.Core.Contracts;
using Mystique.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        private static void BuildNotificationProvider(Assembly assembly, IServiceScope scope)
        {
            IEnumerable<Type> providers = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x == typeof(INotificationProvider)));

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

        private static void RegisterModuleQueries(IDataStore dataStore, string moduleName, Assembly assembly, IServiceScope scope)
        {
            IEnumerable<Type> queries = assembly.GetExportedTypes().Where(p => p.GetInterfaces().Any(x => x == typeof(IDataStoreQuery)));
            if (queries.Any())
            {
                var connString = scope.ServiceProvider.GetService<IOptions<ConnectionStringSetting>>();

                foreach (Type p in queries)
                {
                    var constructor = p.GetConstructors().FirstOrDefault(p => p.GetParameters().Length == 1);

                    if (constructor != null)
                    {
                        IDataStoreQuery obj = (IDataStoreQuery)constructor.Invoke(new object[] { connString.Value.ConnectionString });
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
