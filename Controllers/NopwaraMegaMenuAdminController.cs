using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Menus;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Models;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Menus;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class NopwaraMegaMenuAdminController : BasePluginController
{
    #region Fields

    private const string VIEWS = $"~/Plugins/{NopwaraMegaMenuDefaults.PLUGIN_FOLDER}/Views/Admin/";

    private readonly ICategoryService _categoryService;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly ILocalizedEntityService _localizedEntityService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IMenuService _menuService;
    private readonly INotificationService _notificationService;
    private readonly IProductService _productService;
    private readonly ISettingService _settingService;
    private readonly ITopicService _topicService;
    private readonly IVendorService _vendorService;
    private readonly MegaMenuService _megaMenuService;
    private readonly NopwaraMegaMenuSettings _settings;

    #endregion

    #region Ctor

    public NopwaraMegaMenuAdminController(ICategoryService categoryService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IManufacturerService manufacturerService,
        IMenuService menuService,
        INotificationService notificationService,
        IProductService productService,
        ISettingService settingService,
        ITopicService topicService,
        IVendorService vendorService,
        MegaMenuService megaMenuService,
        NopwaraMegaMenuSettings settings)
    {
        _categoryService = categoryService;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _manufacturerService = manufacturerService;
        _menuService = menuService;
        _notificationService = notificationService;
        _productService = productService;
        _settingService = settingService;
        _topicService = topicService;
        _vendorService = vendorService;
        _megaMenuService = megaMenuService;
        _settings = settings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a readable title: the item's own title, or the name of the entity it links to
    /// </summary>
    protected virtual async Task<string> GetMenuItemTitleAsync(MenuItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Title))
            return item.Title;

        var entityId = item.EntityId ?? 0;
        var name = item.MenuItemType switch
        {
            MenuItemType.Category => (await _categoryService.GetCategoryByIdAsync(entityId))?.Name,
            MenuItemType.Manufacturer => (await _manufacturerService.GetManufacturerByIdAsync(entityId))?.Name,
            MenuItemType.Vendor => (await _vendorService.GetVendorByIdAsync(entityId))?.Name,
            MenuItemType.Product => (await _productService.GetProductByIdAsync(entityId))?.Name,
            MenuItemType.TopicPage => (await _topicService.GetTopicByIdAsync(entityId)) is { } topic ? topic.Title ?? topic.SystemName : null,
            _ => null
        };

        return name ?? $"#{item.Id}";
    }

    /// <summary>
    /// Gets the title and type of what the extras are for: a core menu item, or a category item core generates; null when not found
    /// </summary>
    protected virtual async Task<(string title, string type)?> GetTargetAsync(int menuItemId, int categoryId)
    {
        if (menuItemId > 0)
        {
            var menuItem = await _menuService.GetMenuItemByIdAsync(menuItemId);
            if (menuItem == null)
                return null;

            return (await GetMenuItemTitleAsync(menuItem), await _localizationService.GetLocalizedEnumAsync(menuItem.MenuItemType));
        }

        var category = categoryId > 0 ? await _categoryService.GetCategoryByIdAsync(categoryId) : null;
        if (category == null)
            return null;

        return (category.Name, await _localizationService.GetLocalizedEnumAsync(MenuItemType.Category));
    }

    protected virtual async Task<string> GetExtrasSummaryAsync(MegaMenuItem extras)
    {
        if (extras == null)
            return string.Empty;

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(extras.BadgeText))
            parts.Add($"{await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.Badge")} \"{extras.BadgeText}\"");
        if (!string.IsNullOrEmpty(extras.Icon))
            parts.Add(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.Icon"));
        if (extras.ShowViewAll)
            parts.Add(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.ViewAll"));
        if (extras.PromoPictureId > 0)
            parts.Add(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.Promo"));
        if (extras.GetProductIds().Count is var count and > 0)
            parts.Add(string.Format(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.Products"), count));
        if (extras.ContentTopicId > 0 || !string.IsNullOrWhiteSpace(extras.ContentHtml))
            parts.Add(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.Content"));
        if (extras.PanelLayout == PanelLayout.FullWidth)
            parts.Add(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Summary.FullWidth"));

        return string.Join(", ", parts);
    }

    protected virtual async Task PrepareListsAsync(MegaMenuItemModel model)
    {
        foreach (var color in Enum.GetValues<BadgeColor>())
            model.AvailableBadgeColors.Add(new SelectListItem(await _localizationService.GetResourceAsync($"Plugins.Widgets.NopwaraMegaMenu.BadgeColor.{color}"), ((int)color).ToString()));

        model.AvailableIcons.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Icon.None"), string.Empty));
        foreach (var name in MegaMenuIcons.Paths.Keys)
            model.AvailableIcons.Add(new SelectListItem(name, name));
        model.IconPaths = MegaMenuIcons.Paths.ToDictionary(icon => icon.Key, icon => icon.Value);

        foreach (var layout in Enum.GetValues<PanelLayout>())
            model.AvailablePanelLayouts.Add(new SelectListItem(await _localizationService.GetResourceAsync($"Plugins.Widgets.NopwaraMegaMenu.PanelLayout.{layout}"), ((int)layout).ToString()));

        foreach (var order in Enum.GetValues<ColumnOrder>())
            model.AvailableColumnOrders.Add(new SelectListItem(await _localizationService.GetResourceAsync($"Plugins.Widgets.NopwaraMegaMenu.ColumnOrder.{order}"), ((int)order).ToString()));

        model.AvailableTopics.Add(new SelectListItem(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.ContentTopic.None"), "0"));
        foreach (var topic in await _topicService.GetAllTopicsAsync(0, ignoreAcl: true, showHidden: true))
            model.AvailableTopics.Add(new SelectListItem(string.IsNullOrEmpty(topic.Title) ? topic.SystemName : $"{topic.Title} ({topic.SystemName})", topic.Id.ToString()));

        var productIds = new MegaMenuItem { ProductIds = model.ProductIds }.GetProductIds();
        var products = (await _productService.GetProductsByIdsAsync(productIds.ToArray())).ToDictionary(product => product.Id);
        foreach (var id in productIds.Where(products.ContainsKey))
            model.SelectedProducts.Add(new SelectListItem(products[id].Name, id.ToString()));
    }

    protected virtual bool IsEmpty(MegaMenuItemModel model)
    {
        return string.IsNullOrWhiteSpace(model.BadgeText) && string.IsNullOrEmpty(model.Icon) && !model.ShowViewAll
            && model.PanelLayoutId == (int)PanelLayout.Auto && model.ColumnOrderId == (int)ColumnOrder.ExtrasRight
            && model.PromoPictureId == 0 && string.IsNullOrWhiteSpace(model.ProductIds)
            && model.ContentTopicId == 0 && string.IsNullOrWhiteSpace(model.ContentHtml)
            && model.Locales.All(locale => string.IsNullOrWhiteSpace(locale.BadgeText) && string.IsNullOrWhiteSpace(locale.ContentHtml));
    }

    #endregion

    #region List

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        var model = new MegaMenuListModel();
        var extras = (await _megaMenuService.GetAllAsync())
            .Where(item => item.Key != null)
            .GroupBy(item => item.Key)
            .ToDictionary(group => group.Key, group => group.First());
        var categoryType = await _localizationService.GetLocalizedEnumAsync(MenuItemType.Category);
        var listTemplate = await _localizationService.GetLocalizedEnumAsync(MenuItemTemplate.List);

        foreach (var menu in await _menuService.GetAllMenusAsync(menuType: MenuType.Main, showHidden: true))
        {
            var tree = new MenuTreeModel { Id = menu.Id, Name = menu.Name, Published = menu.Published };

            //"Display all categories": core shows the root categories (and their subcategories) without menu items
            if (menu.DisplayAllCategories)
            {
                foreach (var category in await _categoryService.GetAllCategoriesByParentCategoryIdAsync(0, showHidden: true))
                {
                    tree.Items.Add(new MenuTreeItemModel
                    {
                        CategoryId = category.Id,
                        Title = category.Name,
                        Type = categoryType,
                        Template = listTemplate,
                        Published = category.Published,
                        Extras = await GetExtrasSummaryAsync(extras.GetValueOrDefault(MegaMenuKeys.For(0, category.Id)))
                    });

                    foreach (var subcategory in await _categoryService.GetAllCategoriesByParentCategoryIdAsync(category.Id, showHidden: true))
                    {
                        tree.Items.Add(new MenuTreeItemModel
                        {
                            CategoryId = subcategory.Id,
                            Title = subcategory.Name,
                            Type = categoryType,
                            Depth = 1,
                            Published = subcategory.Published,
                            Extras = await GetExtrasSummaryAsync(extras.GetValueOrDefault(MegaMenuKeys.For(0, subcategory.Id)))
                        });
                    }
                }
            }

            var items = await _menuService.GetAllMenuItemsAsync(menuId: menu.Id, treeSorting: true, showHidden: true);
            var depthById = new Dictionary<int, int>();
            foreach (var item in items)
            {
                var depth = item.ParentId > 0 && depthById.TryGetValue(item.ParentId, out var parentDepth) ? parentDepth + 1 : 0;
                depthById[item.Id] = depth;

                tree.Items.Add(new MenuTreeItemModel
                {
                    MenuItemId = item.Id,
                    Title = await GetMenuItemTitleAsync(item),
                    Type = await _localizationService.GetLocalizedEnumAsync(item.MenuItemType),
                    Template = await _localizationService.GetLocalizedEnumAsync(item.Template),
                    Depth = depth,
                    Published = item.Published,
                    Extras = await GetExtrasSummaryAsync(extras.GetValueOrDefault(MegaMenuKeys.For(item.Id, 0)))
                });
            }

            model.Menus.Add(tree);
        }

        return View(VIEWS + "List.cshtml", model);
    }

    #endregion

    #region Edit

    [CheckPermission(StandardPermission.ContentManagement.MENU_VIEW)]
    public virtual async Task<IActionResult> Edit(int menuItemId, int categoryId)
    {
        var target = await GetTargetAsync(menuItemId, categoryId);
        if (target == null)
            return RedirectToAction("List");

        var extras = await _megaMenuService.GetAsync(menuItemId, categoryId)
            ?? new MegaMenuItem { MenuItemId = menuItemId > 0 ? menuItemId : null, CategoryId = menuItemId > 0 ? 0 : categoryId };
        var model = new MegaMenuItemModel
        {
            Id = extras.Id,
            MenuItemId = menuItemId > 0 ? menuItemId : 0,
            CategoryId = menuItemId > 0 ? 0 : categoryId,
            MenuItemTitle = target.Value.title,
            MenuItemType = target.Value.type,
            BadgeText = extras.BadgeText,
            BadgeColorId = extras.BadgeColorId,
            Icon = extras.Icon,
            ShowViewAll = extras.ShowViewAll,
            ViewAllText = extras.ViewAllText,
            PanelLayoutId = extras.PanelLayoutId,
            ColumnOrderId = extras.ColumnOrderId,
            PromoPictureId = extras.PromoPictureId,
            PromoUrl = extras.PromoUrl,
            PromoTitle = extras.PromoTitle,
            PromoSubtitle = extras.PromoSubtitle,
            ProductIds = extras.ProductIds,
            ProductsTitle = extras.ProductsTitle,
            ContentTopicId = extras.ContentTopicId,
            ContentHtml = extras.ContentHtml,
            ContentTitle = extras.ContentTitle
        };

        await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
        {
            if (extras.Id == 0)
                return;

            locale.BadgeText = await _localizationService.GetLocalizedAsync(extras, x => x.BadgeText, languageId, false, false);
            locale.ViewAllText = await _localizationService.GetLocalizedAsync(extras, x => x.ViewAllText, languageId, false, false);
            locale.PromoTitle = await _localizationService.GetLocalizedAsync(extras, x => x.PromoTitle, languageId, false, false);
            locale.PromoSubtitle = await _localizationService.GetLocalizedAsync(extras, x => x.PromoSubtitle, languageId, false, false);
            locale.ProductsTitle = await _localizationService.GetLocalizedAsync(extras, x => x.ProductsTitle, languageId, false, false);
            locale.ContentHtml = await _localizationService.GetLocalizedAsync(extras, x => x.ContentHtml, languageId, false, false);
            locale.ContentTitle = await _localizationService.GetLocalizedAsync(extras, x => x.ContentTitle, languageId, false, false);
        });

        await PrepareListsAsync(model);

        return View(VIEWS + "Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(MegaMenuItemModel model, bool continueEditing)
    {
        if (await GetTargetAsync(model.MenuItemId, model.CategoryId) == null)
            return RedirectToAction("List");

        var extras = await _megaMenuService.GetAsync(model.MenuItemId, model.CategoryId);

        //nothing set: no row (the item renders as plain core)
        if (IsEmpty(model))
        {
            if (extras != null)
                await _megaMenuService.DeleteAsync(extras);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Saved"));
            return continueEditing ? RedirectToAction("Edit", new { menuItemId = model.MenuItemId, categoryId = model.CategoryId }) : RedirectToAction("List");
        }

        extras ??= new MegaMenuItem
        {
            MenuItemId = model.MenuItemId > 0 ? model.MenuItemId : null,
            CategoryId = model.MenuItemId > 0 ? 0 : model.CategoryId
        };
        extras.BadgeText = model.BadgeText?.Trim();
        extras.BadgeColorId = model.BadgeColorId;
        extras.Icon = MegaMenuIcons.Paths.ContainsKey(model.Icon ?? string.Empty) ? model.Icon : null;
        extras.ShowViewAll = model.ShowViewAll;
        extras.ViewAllText = model.ViewAllText?.Trim();
        extras.PanelLayoutId = model.PanelLayoutId;
        extras.ColumnOrderId = model.ColumnOrderId;
        extras.PromoPictureId = model.PromoPictureId;
        extras.PromoUrl = model.PromoUrl?.Trim();
        extras.PromoTitle = model.PromoTitle?.Trim();
        extras.PromoSubtitle = model.PromoSubtitle?.Trim();
        //keep the chosen order, drop unknown ids, at most MAX_PRODUCTS
        extras.ProductIds = string.Join(",", new MegaMenuItem { ProductIds = model.ProductIds }.GetProductIds().Take(NopwaraMegaMenuDefaults.MAX_PRODUCTS));
        extras.ProductsTitle = model.ProductsTitle?.Trim();
        extras.ContentTopicId = model.ContentTopicId;
        extras.ContentHtml = model.ContentHtml;
        extras.ContentTitle = model.ContentTitle?.Trim();

        if (extras.Id == 0)
            await _megaMenuService.InsertAsync(extras);
        else
            await _megaMenuService.UpdateAsync(extras);

        foreach (var locale in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.BadgeText, locale.BadgeText, locale.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.ViewAllText, locale.ViewAllText, locale.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.PromoTitle, locale.PromoTitle, locale.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.PromoSubtitle, locale.PromoSubtitle, locale.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.ProductsTitle, locale.ProductsTitle, locale.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.ContentHtml, locale.ContentHtml, locale.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(extras, x => x.ContentTitle, locale.ContentTitle, locale.LanguageId);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Saved"));

        return continueEditing ? RedirectToAction("Edit", new { menuItemId = model.MenuItemId, categoryId = model.CategoryId }) : RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.ContentManagement.MENU_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int menuItemId, int categoryId)
    {
        var extras = await _megaMenuService.GetAsync(menuItemId, categoryId);
        if (extras != null)
            await _megaMenuService.DeleteAsync(extras);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Removed"));

        return RedirectToAction("List");
    }

    #endregion

    #region Configuration

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            ProductPictureSize = _settings.ProductPictureSize,
            PromoPictureSize = _settings.PromoPictureSize
        };

        return View(VIEWS + "Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return Configure();

        _settings.ProductPictureSize = Math.Clamp(model.ProductPictureSize, 40, 1000);
        _settings.PromoPictureSize = Math.Clamp(model.PromoPictureSize, 100, 2000);
        await _settingService.SaveSettingAsync(_settings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}
