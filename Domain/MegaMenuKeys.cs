using Nop.Core.Domain.Menus;
using Nop.Web.Models.Menus;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

/// <summary>
/// Keys of the extras handed to the theme: "i{menuItemId}" for core menu items, "c{categoryId}" for the category items
/// core generates without a menu item ("Display all categories" and the subcategories of category dropdowns)
/// </summary>
public static class MegaMenuKeys
{
    public static string For(int menuItemId, int categoryId)
    {
        return menuItemId > 0 ? $"i{menuItemId}" : categoryId > 0 ? $"c{categoryId}" : null;
    }

    /// <summary>
    /// Gets the key of a storefront menu item model; null when extras can't apply to it
    /// </summary>
    public static string For(MenuItemModel item)
    {
        if (item.Id > 0)
            return For(item.Id, 0);

        return item.MenuItemType == MenuItemType.Category ? For(0, item.EntityId) : null;
    }
}
