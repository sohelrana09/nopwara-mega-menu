using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Menus;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;
using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Menus;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Services;

/// <summary>
/// Hands the main menu's extras (badge, icon, "View all", layout) to the theme for the current request.
/// </summary>
/// <remarks>
/// Raised by MainMenuViewComponent through ModelPreparedEvent. Core caches the menu model and every request gets the
/// same instance, so the extras are not written into it (that would mean concurrent writes to a shared dictionary):
/// they go to HttpContext.Items as Dictionary&lt;string, Dictionary&lt;string, string&gt;&gt; keyed by MegaMenuKeys
/// ("i{menuItemId}", or "c{categoryId}" for category items core generates).
/// Keys: badge, badgeColor, icon (SVG path data, "|"-separated), viewAllText, viewAllUrl, layout ("full"), columns ("1"
/// when the plugin renders dropdown columns), columnOrder ("left").
/// </remarks>
public class MegaMenuModelConsumer : IConsumer<ModelPreparedEvent<BaseNopModel>>
{
    #region Fields

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreContext _storeContext;
    private readonly IWidgetPluginManager _widgetPluginManager;
    private readonly IWorkContext _workContext;
    private readonly MegaMenuService _megaMenuService;

    #endregion

    #region Ctor

    public MegaMenuModelConsumer(IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext,
        MegaMenuService megaMenuService)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _storeContext = storeContext;
        _widgetPluginManager = widgetPluginManager;
        _workContext = workContext;
        _megaMenuService = megaMenuService;
    }

    #endregion

    #region Utilities

    protected virtual async Task<Dictionary<string, string>> PrepareExtrasAsync(MegaMenuItem extras, MenuItemModel item, Language language)
    {
        var values = new Dictionary<string, string>();

        var badge = await _localizationService.GetLocalizedAsync(extras, x => x.BadgeText, language.Id);
        if (!string.IsNullOrWhiteSpace(badge))
        {
            values["badge"] = badge.Trim();
            values["badgeColor"] = extras.BadgeColor.ToString().ToLowerInvariant();
        }

        if (!string.IsNullOrEmpty(extras.Icon) && MegaMenuIcons.Paths.TryGetValue(extras.Icon, out var paths))
            values["icon"] = string.Join("|", paths);

        if (extras.ShowViewAll && !string.IsNullOrEmpty(item.Url))
        {
            var text = await _localizationService.GetLocalizedAsync(extras, x => x.ViewAllText, language.Id);
            if (string.IsNullOrWhiteSpace(text))
                text = await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.ViewAll", language.Id);
            //admin-entered text: Replace can't throw on stray braces the way string.Format does
            values["viewAllText"] = text.Replace("{0}", item.Title);
            values["viewAllUrl"] = item.Url;
        }

        if (extras.PanelLayout == PanelLayout.FullWidth)
            values["layout"] = "full";

        if (extras.HasPanelColumns)
            values["columns"] = "1";

        if (extras.ColumnOrder == ColumnOrder.ExtrasLeft)
            values["columnOrder"] = "left";

        return values;
    }

    protected virtual async Task CollectAsync(IEnumerable<MenuItemModel> items, IDictionary<string, MegaMenuItem> extrasByKey,
        Dictionary<string, Dictionary<string, string>> result, Language language)
    {
        foreach (var item in items)
        {
            var key = MegaMenuKeys.For(item);
            if (key != null && !result.ContainsKey(key) && extrasByKey.TryGetValue(key, out var extras))
            {
                var values = await PrepareExtrasAsync(extras, item, language);
                if (values.Any())
                    result[key] = values;
            }

            await CollectAsync(item.ChildrenItems, extrasByKey, result, language);
        }
    }

    #endregion

    #region Methods

    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not MenuModel { MenuType: MenuType.Main } menu || _httpContextAccessor.HttpContext is not { } httpContext)
            return;

        var store = await _storeContext.GetCurrentStoreAsync();
        if (!await _widgetPluginManager.IsPluginActiveAsync(NopwaraMegaMenuDefaults.SYSTEM_NAME, await _workContext.GetCurrentCustomerAsync(), store.Id))
            return;

        var extrasByKey = (await _megaMenuService.GetAllAsync())
            .Where(item => item.Key != null)
            .GroupBy(item => item.Key)
            .ToDictionary(group => group.Key, group => group.First());
        if (!extrasByKey.Any())
            return;

        var result = new Dictionary<string, Dictionary<string, string>>();
        await CollectAsync(menu.Items, extrasByKey, result, await _workContext.GetWorkingLanguageAsync());

        httpContext.Items[NopwaraMegaMenuDefaults.HTTP_CONTEXT_ITEMS_KEY] = result;
    }

    #endregion
}
