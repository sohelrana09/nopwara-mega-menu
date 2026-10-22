namespace Nop.Plugin.Widgets.NopwaraMegaMenu;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class NopwaraMegaMenuDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public const string SYSTEM_NAME = "Widgets.NopwaraMegaMenu";

    /// <summary>
    /// Gets the folder (under Nop.Web/Plugins) the plugin is deployed to
    /// </summary>
    public const string PLUGIN_FOLDER = "Widgets.NopwaraMegaMenu";

    /// <summary>
    /// Gets the widget zone the Nopwara theme renders inside each main menu dropdown (additionalData = the MenuItemModel)
    /// </summary>
    public const string PANEL_WIDGET_ZONE = "nopwara_megamenu_panel";

    /// <summary>
    /// Gets the widget zone the Nopwara theme renders at the bottom of each mobile menu level (promo banner only)
    /// </summary>
    public const string PANEL_WIDGET_ZONE_MOBILE = "nopwara_megamenu_panel_mobile";

    /// <summary>
    /// Gets the HttpContext.Items key with the extras of the current request's main menu
    /// (Dictionary&lt;string, Dictionary&lt;string, string&gt;&gt; by MegaMenuKeys; read by the Nopwara theme)
    /// </summary>
    public const string HTTP_CONTEXT_ITEMS_KEY = "nopwara.megamenu";

    /// <summary>
    /// Gets the name of the admin configuration route
    /// </summary>
    public const string CONFIGURATION_ROUTE_NAME = "Plugin.Widgets.NopwaraMegaMenu.Configure";

    /// <summary>
    /// Gets the system name of the admin menu item (Content management > Mega menu)
    /// </summary>
    public const string ADMIN_MENU_SYSTEM_NAME = "Nopwara mega menu";

    /// <summary>
    /// Gets the maximum number of featured products in one dropdown
    /// </summary>
    public const int MAX_PRODUCTS = 4;
}
