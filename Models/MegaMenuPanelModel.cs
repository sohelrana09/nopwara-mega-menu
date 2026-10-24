using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Models;

/// <summary>
/// Represents the extra columns of one main menu dropdown
/// </summary>
public record MegaMenuPanelModel : BaseNopModel
{
    public string Key { get; set; }

    public bool ExtrasLeft { get; set; }

    public string PromoImageUrl { get; set; }

    public string PromoUrl { get; set; }

    public string PromoTitle { get; set; }

    public string PromoSubtitle { get; set; }

    public string ProductsTitle { get; set; }

    /// <summary>
    /// Gets or sets the number of featured products set in admin; the products themselves are loaded from ProductsUrl
    /// when the dropdown first opens, so closed dropdowns cost no price calculation
    /// </summary>
    public int ProductCount { get; set; }

    public string ProductsUrl { get; set; }

    public string ContentTitle { get; set; }

    public string ContentHtml { get; set; }

    public bool HasPromo => !string.IsNullOrEmpty(PromoImageUrl);

    public bool HasContent => !string.IsNullOrWhiteSpace(ContentHtml);

    public bool HasProducts => ProductCount > 0;

    public bool IsEmpty => !HasPromo && !HasProducts && !HasContent;
}

/// <summary>
/// Represents the featured products of a dropdown (loaded on demand)
/// </summary>
public record MegaMenuProductsModel : BaseNopModel
{
    public IList<ProductOverviewModel> Products { get; set; } = new List<ProductOverviewModel>();
}
