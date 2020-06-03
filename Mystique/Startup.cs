using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Mystique.Core.Mvc.Infrastructure;
using Mystique.Core.Repository.MySql.Migrations;
using System;
using System.Threading;

namespace Mystique
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using (var scope = CreateServices().CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }

            services.MystiqueSetup(Configuration);

            
        }

        private static IServiceProvider CreateServices()
        {
            return new ServiceCollection().AddFluentMigratorCore().ConfigureRunner(rb =>
             rb.AddMySql5()
                 .WithGlobalConnectionString("server=localhost;port=3306;Database=pluginDB;UID=root;PWD=123456")
                 .ScanIn(typeof(InitialDB).Assembly)
                 .For
                 .Migrations())
                 .AddLogging(lb => lb.AddFluentMigratorConsole())
                 .BuildServiceProvider(false);
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

#if DEBUG
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(@"/home/lamond/myApp/Mystique/Mystique/Mystique/wwwroot")
            });
#endif

#if !DEBUG

            app.UseStaticFiles();
#endif

            app.MystiqueRoute();
        }
    }
}


