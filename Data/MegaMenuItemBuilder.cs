using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Menus;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Data;

/// <summary>
/// Represents the mega menu item entity builder
/// </summary>
public class MegaMenuItemBuilder : NopEntityBuilder<MegaMenuItem>
{
    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            //deleting a core menu item removes its extras; one row per item/category is enforced by the service
            .WithColumn(nameof(MegaMenuItem.MenuItemId)).AsInt32().Nullable().ForeignKey<MenuItem>()
            .WithColumn(nameof(MegaMenuItem.CategoryId)).AsInt32().NotNullable().Indexed()
            .WithColumn(nameof(MegaMenuItem.BadgeText)).AsString(50).Nullable()
            .WithColumn(nameof(MegaMenuItem.Icon)).AsString(50).Nullable()
            .WithColumn(nameof(MegaMenuItem.ViewAllText)).AsString(200).Nullable()
            .WithColumn(nameof(MegaMenuItem.PromoUrl)).AsString(1000).Nullable()
            .WithColumn(nameof(MegaMenuItem.PromoTitle)).AsString(200).Nullable()
            .WithColumn(nameof(MegaMenuItem.PromoSubtitle)).AsString(400).Nullable()
            .WithColumn(nameof(MegaMenuItem.ProductIds)).AsString(200).Nullable()
            .WithColumn(nameof(MegaMenuItem.ProductsTitle)).AsString(200).Nullable()
            .WithColumn(nameof(MegaMenuItem.ContentHtml)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(MegaMenuItem.ContentTitle)).AsString(200).Nullable();
    }
}
