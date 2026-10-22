using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu;

/// <summary>
/// Represents plugin settings
/// </summary>
public class NopwaraMegaMenuSettings : ISettings
{
    /// <summary>
    /// Gets or sets the picture size of featured products in dropdowns
    /// </summary>
    public int ProductPictureSize { get; set; } = 160;

    /// <summary>
    /// Gets or sets the picture size of promo banners
    /// </summary>
    public int PromoPictureSize { get; set; } = 600;
}
