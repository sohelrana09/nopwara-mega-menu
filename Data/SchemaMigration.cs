using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Data;

[NopMigration("2026-10-22 18:00:00", "Nop.Plugin.Widgets.NopwaraMegaMenu schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<MegaMenuItem>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<MegaMenuItem>();
    }
}
