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

    public IList<ProductOverviewModel> Products { get; set; } = new List<ProductOverviewModel>();

    public string ContentTitle { get; set; }

    public string ContentHtml { get; set; }

    public bool HasPromo => !string.IsNullOrEmpty(PromoImageUrl);

    public bool HasContent => !string.IsNullOrWhiteSpace(ContentHtml);

    public bool IsEmpty => !HasPromo && !Products.Any() && !HasContent;
}
