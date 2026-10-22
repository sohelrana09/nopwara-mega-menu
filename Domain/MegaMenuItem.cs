using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

/// <summary>
/// Represents the mega menu extras of a main menu item: a core menu item (MenuItemId), or a category item core
/// generates itself (CategoryId: "Display all categories" menus and the subcategories of category dropdowns)
/// </summary>
public class MegaMenuItem : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the identifier of the core menu item (Nop.Core.Domain.Menus.MenuItem); null for a category item
    /// </summary>
    public int? MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the category of a generated category item; 0 for a menu item
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets the key the theme looks the extras up by ("i{menuItemId}" or "c{categoryId}")
    /// </summary>
    public string Key => MegaMenuKeys.For(MenuItemId ?? 0, CategoryId);

    /// <summary>
    /// Gets or sets the badge text ("New", "Sale"...); empty for no badge (localizable)
    /// </summary>
    public string BadgeText { get; set; }

    /// <summary>
    /// Gets or sets the badge color identifier
    /// </summary>
    public int BadgeColorId { get; set; }

    /// <summary>
    /// Gets or sets the icon name (one of MegaMenuIcons.Names); empty for no icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a "View all" link to the item URL ends the dropdown
    /// </summary>
    public bool ShowViewAll { get; set; }

    /// <summary>
    /// Gets or sets the "View all" text; {0} is the item title (localizable)
    /// </summary>
    public string ViewAllText { get; set; }

    /// <summary>
    /// Gets or sets the dropdown layout identifier
    /// </summary>
    public int PanelLayoutId { get; set; }

    /// <summary>
    /// Gets or sets the side the extra columns are shown on
    /// </summary>
    public int ColumnOrderId { get; set; }

    /// <summary>
    /// Gets or sets the promo banner picture identifier; 0 for no promo
    /// </summary>
    public int PromoPictureId { get; set; }

    /// <summary>
    /// Gets or sets the promo banner link
    /// </summary>
    public string PromoUrl { get; set; }

    /// <summary>
    /// Gets or sets the promo banner title (localizable)
    /// </summary>
    public string PromoTitle { get; set; }

    /// <summary>
    /// Gets or sets the promo banner subtitle (localizable)
    /// </summary>
    public string PromoSubtitle { get; set; }

    /// <summary>
    /// Gets or sets the featured product identifiers, in display order (comma-separated)
    /// </summary>
    public string ProductIds { get; set; }

    /// <summary>
    /// Gets or sets the title above the featured products (localizable)
    /// </summary>
    public string ProductsTitle { get; set; }

    /// <summary>
    /// Gets or sets the topic shown as a content column; 0 to use ContentHtml
    /// </summary>
    public int ContentTopicId { get; set; }

    /// <summary>
    /// Gets or sets the HTML of the content column when no topic is chosen (localizable)
    /// </summary>
    public string ContentHtml { get; set; }

    /// <summary>
    /// Gets or sets the title above the content column (localizable)
    /// </summary>
    public string ContentTitle { get; set; }

    public BadgeColor BadgeColor
    {
        get => (BadgeColor)BadgeColorId;
        set => BadgeColorId = (int)value;
    }

    public PanelLayout PanelLayout
    {
        get => (PanelLayout)PanelLayoutId;
        set => PanelLayoutId = (int)value;
    }

    public ColumnOrder ColumnOrder
    {
        get => (ColumnOrder)ColumnOrderId;
        set => ColumnOrderId = (int)value;
    }

    /// <summary>
    /// Gets a value indicating whether the item has dropdown columns of its own (promo, products or content)
    /// </summary>
    public bool HasPanelColumns => PromoPictureId > 0 || !string.IsNullOrWhiteSpace(ProductIds) || ContentTopicId > 0 || !string.IsNullOrWhiteSpace(ContentHtml);

    /// <summary>
    /// Gets the featured product identifiers in display order
    /// </summary>
    public IList<int> GetProductIds()
    {
        return (ProductIds ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(id => int.TryParse(id, out var value) ? value : 0)
            .Where(id => id > 0)
            .Distinct()
            .ToList();
    }
}

/// <summary>
/// Represents a badge color (mapped to fixed classes by the theme)
/// </summary>
public enum BadgeColor
{
    Primary = 0,
    Red = 10,
    Green = 20,
    Amber = 30,
    Slate = 40
}

/// <summary>
/// Represents a dropdown layout
/// </summary>
public enum PanelLayout
{
    /// <summary>
    /// Core layout (grid dropdowns are full width, list dropdowns are compact); full width when there are extra columns
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Always a full-width panel
    /// </summary>
    FullWidth = 10
}

/// <summary>
/// Represents the side of the extra columns in a dropdown
/// </summary>
public enum ColumnOrder
{
    ExtrasRight = 0,
    ExtrasLeft = 10
}
