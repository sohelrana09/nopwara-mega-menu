using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Services;

/// <summary>
/// Clears the cached mega menu items when one is inserted, updated or deleted (the base class removes the "all" keys)
/// </summary>
public class MegaMenuItemCacheEventConsumer : CacheEventConsumer<MegaMenuItem>
{
}
