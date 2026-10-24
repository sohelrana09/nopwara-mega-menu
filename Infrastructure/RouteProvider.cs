using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: NopwaraMegaMenuDefaults.CONFIGURATION_ROUTE_NAME,
            pattern: "Admin/NopwaraMegaMenuAdmin/Configure",
            defaults: new { controller = "NopwaraMegaMenuAdmin", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NopwaraMegaMenuDefaults.PRODUCTS_ROUTE_NAME,
            pattern: "nopwara-megamenu/products/{key}",
            defaults: new { controller = "NopwaraMegaMenu", action = "Products" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}
