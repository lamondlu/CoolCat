using Microsoft.AspNetCore.Builder;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public static class CoolCatRouteConfiguration
    {
        public static IApplicationBuilder CoolCatRoute(this IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapAreaControllerRoute(
                      "Admin", "Admin",
                      "Admin/{controller}/{action}/{id?}");

                routes.MapControllerRoute(
                       name: "Customer",
                       pattern: "Modules/{area}/{controller=Home}/{action=Dashboard}/{id?}");

                routes.MapControllerRoute(
                    name: "Customer",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            return app;
        }
    }
}
