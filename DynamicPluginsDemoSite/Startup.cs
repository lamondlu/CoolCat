using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DynamicPlugins.Core.BusinessLogics;
using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.Models;
using DynamicPlugins.Core.Repositories;
using DynamicPluginsDemoSite.Controllers;
using DynamicPluginsDemoSite.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicPluginsDemoSite
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddOptions();


            services.Configure<ConnectionStringSetting>(Configuration.GetSection("ConnectionStringSetting"));

            services.AddScoped<IPluginRepository, PluginRepository>();
            services.AddScoped<IPluginManager, PluginManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var mvcBuilders = services.AddMvc();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.AreaViewLocationFormats.Add("/Modules/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
            services.AddSingleton(MyActionDescriptorChangeProvider.Instance);

            mvcBuilders.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "Modules/{area}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
