using Nop.Data;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Services;

/// <summary>
/// Mega menu item data access. The table holds one small row per main menu item, so every read goes through
/// the cached "all" list (cleared by MegaMenuItemCacheEventConsumer on any change).
/// </summary>
public class MegaMenuService
{
    #region Fields

    private readonly IRepository<MegaMenuItem> _repository;

    #endregion

    #region Ctor

    public MegaMenuService(IRepository<MegaMenuItem> repository)
    {
        _repository = repository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets all mega menu items (cached)
    /// </summary>
    public virtual async Task<IList<MegaMenuItem>> GetAllAsync()
    {
        //returning null from the key function uses the default "all" key of the entity
        return await _repository.GetAllAsync(query => query, _ => null);
    }

    /// <summary>
    /// Gets the extras by key ("i{menuItemId}" / "c{categoryId}", see MegaMenuKeys); null when there are none
    /// </summary>
    public virtual async Task<MegaMenuItem> GetByKeyAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        return (await GetAllAsync()).FirstOrDefault(item => item.Key == key);
    }

    /// <summary>
    /// Gets the extras of a core menu item or of a generated category item; null when there are none
    /// </summary>
    public virtual Task<MegaMenuItem> GetAsync(int menuItemId, int categoryId)
    {
        return GetByKeyAsync(MegaMenuKeys.For(menuItemId, categoryId));
    }

    /// <summary>
    /// Gets the extras by id straight from the database (not the cached instance), for editing
    /// </summary>
    public virtual Task<MegaMenuItem> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public virtual Task InsertAsync(MegaMenuItem item) => _repository.InsertAsync(item);

    public virtual Task UpdateAsync(MegaMenuItem item) => _repository.UpdateAsync(item);

    public virtual Task DeleteAsync(MegaMenuItem item) => _repository.DeleteAsync(item);

    #endregion
}
