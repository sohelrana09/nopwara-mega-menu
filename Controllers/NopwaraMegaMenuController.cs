using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Models;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Services;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Controllers;

/// <summary>
/// Storefront endpoints of the mega menu
/// </summary>
public class NopwaraMegaMenuController : BasePublicController
{
    #region Fields

    private readonly IAclService _aclService;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly MegaMenuService _megaMenuService;
    private readonly NopwaraMegaMenuSettings _settings;

    #endregion

    #region Ctor

    public NopwaraMegaMenuController(IAclService aclService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService,
        MegaMenuService megaMenuService,
        NopwaraMegaMenuSettings settings)
    {
        _aclService = aclService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _megaMenuService = megaMenuService;
        _settings = settings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the featured product cards of a dropdown; the Nopwara theme loads them the first time the dropdown opens
    /// </summary>
    /// <param name="key">Extras key ("i{menuItemId}" / "c{categoryId}")</param>
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> Products(string key)
    {
        var extras = await _megaMenuService.GetByKeyAsync(key);
        var productIds = extras?.GetProductIds() ?? [];
        if (!productIds.Any())
            return Content(string.Empty);

        //same visibility rules as catalog pages, in the chosen order
        var products = (await _productService.GetProductsByIdsAsync(productIds.ToArray()))
            .OrderBy(product => productIds.IndexOf(product.Id))
            .ToList();
        var visible = new List<Product>();
        foreach (var product in products)
        {
            if (product.Published && !product.Deleted && product.VisibleIndividually && _productService.ProductIsAvailable(product)
                && await _aclService.AuthorizeAsync(product) && await _storeMappingService.AuthorizeAsync(product))
            {
                visible.Add(product);
            }
        }

        var model = new MegaMenuProductsModel
        {
            Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(visible.Take(NopwaraMegaMenuDefaults.MAX_PRODUCTS),
                productThumbPictureSize: _settings.ProductPictureSize)).ToList()
        };

        return PartialView($"~/Plugins/{NopwaraMegaMenuDefaults.PLUGIN_FOLDER}/Views/_Products.cshtml", model);
    }

    #endregion
}
