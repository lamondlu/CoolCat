using Microsoft.AspNetCore.Builder;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public static class CoolCatRouteConfiguration
    {
        public static IApplicationBuilder CoolCatRoute(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                       name: "Customer",
                       pattern: "Modules/{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapControllerRoute(
                    name: "Customer",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                
            });

            return app;
        }
    }
}
