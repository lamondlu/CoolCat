using System;
using System.IO;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Mystique.Core.Mvc.Infrastructure;
using Mystique.Core.Repository.MySql.Migrations;
using Mystique.Models;

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
            var siteSettings = new SiteSettings();

            Configuration.Bind("SiteSettings", siteSettings);

            using (var scope = CreateServices(siteSettings).CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }

            services.MystiqueSetup(Configuration);
        }

        private static IServiceProvider CreateServices(SiteSettings settings)
        {
            return new ServiceCollection().AddFluentMigratorCore().ConfigureRunner(rb =>
             rb.AddMySql5()
                 .WithGlobalConnectionString(settings.MySQLConnection)
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
                FileProvider = new PhysicalFileProvider(@"F:\D1\Mystique\Mystique\wwwroot")
                //FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });
#endif

#if !DEBUG

            app.UseStaticFiles();
#endif

            app.MystiqueRoute();
        }
    }
}


