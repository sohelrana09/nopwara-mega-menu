using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu;

/// <summary>
/// Represents the Nopwara mega menu plugin
/// </summary>
public class NopwaraMegaMenuPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public NopwaraMegaMenuPlugin(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Text strings added in version 1.01 (installs get them too; updates add them in UpdateAsync)
    /// </summary>
    protected virtual async Task AddResourcesAddedIn101Async()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoUrl.Invalid"] = "Enter a link that starts with / (e.g. /summer-sale) or with http:// or https://.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentHtml.NoPermission"] = "Editing the HTML needs the \"Admin area. Topics. Create, edit, delete\" permission, because it's shown as-is on every page. The current HTML is kept."
        });
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        //rendered by the Nopwara theme's main menu: inside each dropdown, and at the bottom of each mobile menu level
        return Task.FromResult<IList<string>>(new List<string>
        {
            NopwaraMegaMenuDefaults.PANEL_WIDGET_ZONE,
            NopwaraMegaMenuDefaults.PANEL_WIDGET_ZONE_MOBILE
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(MegaMenuPanelViewComponent);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(NopwaraMegaMenuDefaults.CONFIGURATION_ROUTE_NAME);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new NopwaraMegaMenuSettings());

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(NopwaraMegaMenuDefaults.SYSTEM_NAME))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(NopwaraMegaMenuDefaults.SYSTEM_NAME);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //admin
            ["Plugins.Widgets.NopwaraMegaMenu.Title"] = "Mega menu",
            ["Plugins.Widgets.NopwaraMegaMenu.Instructions"] = "Build the menu in Content management > Menus as usual. Here you add the mega menu extras to its items: a badge and an icon, a \"View all\" link, a full-width dropdown, a promo banner, featured products and an HTML or topic column. The Nopwara theme draws them. Menus set to \"Display all categories\" list their categories here too.",
            ["Plugins.Widgets.NopwaraMegaMenu.NoMenus"] = "There is no main menu yet. Create one in Content management > Menus.",
            ["Plugins.Widgets.NopwaraMegaMenu.Menu"] = "Menu",
            ["Plugins.Widgets.NopwaraMegaMenu.Unpublished"] = "Unpublished",
            ["Plugins.Widgets.NopwaraMegaMenu.Item"] = "Menu item",
            ["Plugins.Widgets.NopwaraMegaMenu.Type"] = "Type",
            ["Plugins.Widgets.NopwaraMegaMenu.Template"] = "Dropdown",
            ["Plugins.Widgets.NopwaraMegaMenu.Extras"] = "Mega menu extras",
            ["Plugins.Widgets.NopwaraMegaMenu.None"] = "None",
            ["Plugins.Widgets.NopwaraMegaMenu.EditExtras"] = "Edit extras",
            ["Plugins.Widgets.NopwaraMegaMenu.EditMenuItem"] = "Edit the menu item",
            ["Plugins.Widgets.NopwaraMegaMenu.EditCategory"] = "Edit the category",
            ["Plugins.Widgets.NopwaraMegaMenu.EditTitle"] = "Mega menu extras: {0}",
            ["Plugins.Widgets.NopwaraMegaMenu.BackToList"] = "back to the mega menu",
            ["Plugins.Widgets.NopwaraMegaMenu.Remove"] = "Remove extras",
            ["Plugins.Widgets.NopwaraMegaMenu.Saved"] = "The mega menu extras have been saved.",
            ["Plugins.Widgets.NopwaraMegaMenu.Removed"] = "The mega menu extras have been removed.",
            ["Plugins.Widgets.NopwaraMegaMenu.Tab.Badge"] = "Badge and icon",
            ["Plugins.Widgets.NopwaraMegaMenu.Tab.Layout"] = "Layout and \"View all\"",
            ["Plugins.Widgets.NopwaraMegaMenu.Tab.Promo"] = "Promo banner",
            ["Plugins.Widgets.NopwaraMegaMenu.Tab.Products"] = "Featured products",
            ["Plugins.Widgets.NopwaraMegaMenu.Tab.Content"] = "Content",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.Badge"] = "Badge",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.Icon"] = "Icon",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.ViewAll"] = "View all",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.Promo"] = "Promo",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.Products"] = "{0} product(s)",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.Content"] = "Content",
            ["Plugins.Widgets.NopwaraMegaMenu.Summary.FullWidth"] = "Full width",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeText"] = "Badge text",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeText.Hint"] = "A short label next to the item, e.g. \"New\", \"Sale\" or \"Hot\". Leave empty for no badge.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeColor"] = "Badge color",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeColor.Hint"] = "The color of the badge.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.Icon"] = "Icon",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.Icon.Hint"] = "An icon shown before the item title.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ShowViewAll"] = "Show \"View all\" link",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ShowViewAll.Hint"] = "End the dropdown with a link to the item's own page (useful when the dropdown shows only part of a category's subcategories).",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ViewAllText"] = "\"View all\" text",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ViewAllText.Hint"] = "The link text; {0} is replaced with the item title. Leave empty for \"View all {0}\".",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PanelLayout"] = "Dropdown width",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PanelLayout.Hint"] = "Automatic: grid dropdowns and dropdowns with extra columns are full width, lists are compact. Full width: always a full-width panel.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ColumnOrder"] = "Extra columns position",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ColumnOrder.Hint"] = "Show the promo, products and content columns after (right) or before (left) the menu links.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoPicture"] = "Banner picture",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoPicture.Hint"] = "A picture shown as a banner column in the dropdown. Portrait pictures (4:5) fit best; on mobile it's shown at the bottom of the item's menu level.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoUrl"] = "Banner link",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoUrl.Hint"] = "Where the banner links to, e.g. /summer-sale.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoTitle"] = "Banner title",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoTitle.Hint"] = "Shown on the banner picture.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoSubtitle"] = "Banner subtitle",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoSubtitle.Hint"] = "A second line under the banner title.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.Products"] = "Products",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.Products.Hint"] = "Up to 4 products shown as cards (picture, name, price) in the dropdown, in this order. Unpublished or hidden products are skipped.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ProductsTitle"] = "Products title",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ProductsTitle.Hint"] = "A heading above the products, e.g. \"Trending now\".",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTopic"] = "Topic",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTopic.Hint"] = "Show the body of a topic (e.g. \"Shipping & returns\") as a column. Its ACL and store settings apply.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentHtml"] = "HTML",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentHtml.Hint"] = "Used when no topic is chosen.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTitle"] = "Content title",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTitle.Hint"] = "A heading above the content; the topic title when empty.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ProductPictureSize"] = "Product picture size",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.ProductPictureSize.Hint"] = "The size (in pixels) of featured product pictures in dropdowns.",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoPictureSize"] = "Banner picture size",
            ["Plugins.Widgets.NopwaraMegaMenu.Fields.PromoPictureSize.Hint"] = "The size (in pixels) promo banner pictures are resized to.",
            ["Plugins.Widgets.NopwaraMegaMenu.AddProduct"] = "Add product",
            ["Plugins.Widgets.NopwaraMegaMenu.MoveUp"] = "Move up",
            ["Plugins.Widgets.NopwaraMegaMenu.MaxProducts"] = "Up to 4 products.",
            ["Plugins.Widgets.NopwaraMegaMenu.Icon.None"] = "No icon",
            ["Plugins.Widgets.NopwaraMegaMenu.ContentTopic.None"] = "No topic (use the HTML below)",
            ["Plugins.Widgets.NopwaraMegaMenu.BadgeColor.Primary"] = "Brand (blue)",
            ["Plugins.Widgets.NopwaraMegaMenu.BadgeColor.Red"] = "Red",
            ["Plugins.Widgets.NopwaraMegaMenu.BadgeColor.Green"] = "Green",
            ["Plugins.Widgets.NopwaraMegaMenu.BadgeColor.Amber"] = "Amber",
            ["Plugins.Widgets.NopwaraMegaMenu.BadgeColor.Slate"] = "Dark",
            ["Plugins.Widgets.NopwaraMegaMenu.PanelLayout.Auto"] = "Automatic",
            ["Plugins.Widgets.NopwaraMegaMenu.PanelLayout.FullWidth"] = "Full width",
            ["Plugins.Widgets.NopwaraMegaMenu.ColumnOrder.ExtrasRight"] = "Right of the links",
            ["Plugins.Widgets.NopwaraMegaMenu.ColumnOrder.ExtrasLeft"] = "Left of the links",
            ["Plugins.Widgets.NopwaraMegaMenu.ManageItems"] = "Manage mega menu extras",

            //storefront
            ["Plugins.Widgets.NopwaraMegaMenu.ViewAll"] = "View all {0}"
        });
        await AddResourcesAddedIn101Async();

        await base.InstallAsync();
    }

    /// <summary>
    /// Update the plugin
    /// </summary>
    public override async Task UpdateAsync(string currentVersion, string targetVersion)
    {
        await AddResourcesAddedIn101Async();

        await base.UpdateAsync(currentVersion, targetVersion);
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(NopwaraMegaMenuDefaults.SYSTEM_NAME))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(NopwaraMegaMenuDefaults.SYSTEM_NAME);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _settingService.DeleteSettingAsync<NopwaraMegaMenuSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.NopwaraMegaMenu");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}
