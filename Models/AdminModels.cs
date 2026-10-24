using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Models;

/// <summary>
/// Represents the admin list: main menus and their items as indented trees
/// </summary>
public record MegaMenuListModel : BaseNopModel
{
    public IList<MenuTreeModel> Menus { get; set; } = new List<MenuTreeModel>();
}

public record MenuTreeModel : BaseNopModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool Published { get; set; }

    public IList<MenuTreeItemModel> Items { get; set; } = new List<MenuTreeItemModel>();
}

public record MenuTreeItemModel : BaseNopModel
{
    public int MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the category of an item core generates ("Display all categories"); 0 for a menu item
    /// </summary>
    public int CategoryId { get; set; }

    public string Title { get; set; }

    public string Type { get; set; }

    public string Template { get; set; }

    public int Depth { get; set; }

    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a short summary of the extras ("Badge, Promo, 3 products"); empty when the item has none
    /// </summary>
    public string Extras { get; set; }
}

/// <summary>
/// Represents the extras of one menu item (admin edit page)
/// </summary>
public record MegaMenuItemModel : BaseNopEntityModel, ILocalizedModel<MegaMenuItemLocalizedModel>
{
    public int MenuItemId { get; set; }

    public int CategoryId { get; set; }

    public string MenuItemTitle { get; set; }

    public string MenuItemType { get; set; }

    public int MenuId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeText")]
    public string BadgeText { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeColor")]
    public int BadgeColorId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.Icon")]
    public string Icon { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ShowViewAll")]
    public bool ShowViewAll { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ViewAllText")]
    public string ViewAllText { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PanelLayout")]
    public int PanelLayoutId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ColumnOrder")]
    public int ColumnOrderId { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoPicture")]
    public int PromoPictureId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoUrl")]
    public string PromoUrl { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoTitle")]
    public string PromoTitle { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoSubtitle")]
    public string PromoSubtitle { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.Products")]
    public string ProductIds { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ProductsTitle")]
    public string ProductsTitle { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTopic")]
    public int ContentTopicId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ContentHtml")]
    public string ContentHtml { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTitle")]
    public string ContentTitle { get; set; }

    /// <summary>
    /// Gets or sets whether the admin may edit the HTML (it renders unencoded on every page, so it needs the topics permission)
    /// </summary>
    public bool CanEditHtml { get; set; }

    /// <summary>
    /// Gets or sets the chosen products (id + name) for the products list on the edit page
    /// </summary>
    public IList<SelectListItem> SelectedProducts { get; set; } = new List<SelectListItem>();

    public IList<SelectListItem> AvailableBadgeColors { get; set; } = new List<SelectListItem>();

    public IList<SelectListItem> AvailableIcons { get; set; } = new List<SelectListItem>();

    public IList<SelectListItem> AvailablePanelLayouts { get; set; } = new List<SelectListItem>();

    public IList<SelectListItem> AvailableColumnOrders { get; set; } = new List<SelectListItem>();

    public IList<SelectListItem> AvailableTopics { get; set; } = new List<SelectListItem>();

    /// <summary>
    /// Gets or sets icon SVG path data by name, for the preview next to the icon dropdown
    /// </summary>
    public IDictionary<string, string[]> IconPaths { get; set; } = new Dictionary<string, string[]>();

    public IList<MegaMenuItemLocalizedModel> Locales { get; set; } = new List<MegaMenuItemLocalizedModel>();
}

public record MegaMenuItemLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.BadgeText")]
    public string BadgeText { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ViewAllText")]
    public string ViewAllText { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoTitle")]
    public string PromoTitle { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoSubtitle")]
    public string PromoSubtitle { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ProductsTitle")]
    public string ProductsTitle { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ContentHtml")]
    public string ContentHtml { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ContentTitle")]
    public string ContentTitle { get; set; }
}

/// <summary>
/// Represents the plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.ProductPictureSize")]
    public int ProductPictureSize { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoPictureSize")]
    public int PromoPictureSize { get; set; }
}
