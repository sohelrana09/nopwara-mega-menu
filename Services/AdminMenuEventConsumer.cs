using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Services;

/// <summary>
/// Adds "Mega menu" to the admin Content management menu, after core "Menus"
/// </summary>
public class AdminMenuEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    private readonly ILocalizationService _localizationService;
    private readonly IPluginManager<IPlugin> _pluginManager;

    public AdminMenuEventConsumer(ILocalizationService localizationService,
        IPluginManager<IPlugin> pluginManager)
    {
        _localizationService = localizationService;
        _pluginManager = pluginManager;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        //LoadPluginBySystemNameAsync returns only fully installed plugins (the event may run before the installation is complete)
        if (await _pluginManager.LoadPluginBySystemNameAsync(NopwaraMegaMenuDefaults.SYSTEM_NAME) == null)
            return;

        eventMessage.RootMenuItem.GetItemBySystemName("Content Management")?.InsertAfter("Menus", new AdminMenuItem
        {
            Visible = true,
            SystemName = NopwaraMegaMenuDefaults.ADMIN_MENU_SYSTEM_NAME,
            Title = await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Title"),
            Url = eventMessage.GetMenuItemUrl("NopwaraMegaMenuAdmin", "List"),
            IconClass = "far fa-dot-circle",
            PermissionNames = new List<string> { StandardPermission.ContentManagement.MENU_VIEW }
        });
    }
}
