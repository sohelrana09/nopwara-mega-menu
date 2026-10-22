using Nop.Data.Mapping;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Data;

/// <summary>
/// Plugin table naming compatibility
/// </summary>
public class NopwaraMegaMenuNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        [typeof(MegaMenuItem)] = "NopwaraMegaMenuItem"
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}
