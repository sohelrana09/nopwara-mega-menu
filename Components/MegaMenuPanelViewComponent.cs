using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Models;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Menus;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Components;

/// <summary>
/// Renders the extra columns (promo banner, featured products, HTML/topic content) inside a main menu dropdown
/// (widget zone "nopwara_megamenu_panel"; the Nopwara theme renders it for the desktop and the mobile menu)
/// </summary>
public class MegaMenuPanelViewComponent : NopViewComponent
{
    #region Fields

    private readonly IAclService _aclService;
    private readonly ILocalizationService _localizationService;
    private readonly IPictureService _pictureService;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly ITopicService _topicService;
    private readonly IWorkContext _workContext;
    private readonly MegaMenuService _megaMenuService;
    private readonly NopwaraMegaMenuSettings _settings;

    #endregion

    #region Ctor

    public MegaMenuPanelViewComponent(IAclService aclService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService,
        ITopicService topicService,
        IWorkContext workContext,
        MegaMenuService megaMenuService,
        NopwaraMegaMenuSettings settings)
    {
        _aclService = aclService;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _topicService = topicService;
        _workContext = workContext;
        _megaMenuService = megaMenuService;
        _settings = settings;
    }

    #endregion

    #region Utilities

    protected virtual async Task<MegaMenuPanelModel> PreparePanelModelAsync(MegaMenuItem extras)
    {
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var model = new MegaMenuPanelModel
        {
            Key = extras.Key,
            ExtrasLeft = extras.ColumnOrder == ColumnOrder.ExtrasLeft
        };

        //promo banner
        if (extras.PromoPictureId > 0)
        {
            model.PromoImageUrl = await _pictureService.GetPictureUrlAsync(extras.PromoPictureId, _settings.PromoPictureSize, showDefaultPicture: false);
            model.PromoUrl = extras.PromoUrl;
            model.PromoTitle = await _localizationService.GetLocalizedAsync(extras, x => x.PromoTitle, languageId);
            model.PromoSubtitle = await _localizationService.GetLocalizedAsync(extras, x => x.PromoSubtitle, languageId);
        }

        //featured products: same visibility rules as catalog pages, in the chosen order
        var productIds = extras.GetProductIds();
        if (productIds.Any())
        {
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

            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(visible.Take(NopwaraMegaMenuDefaults.MAX_PRODUCTS),
                productThumbPictureSize: _settings.ProductPictureSize)).ToList();
            model.ProductsTitle = await _localizationService.GetLocalizedAsync(extras, x => x.ProductsTitle, languageId);
        }

        //content: a topic (with its ACL/store rules) or HTML
        model.ContentTitle = await _localizationService.GetLocalizedAsync(extras, x => x.ContentTitle, languageId);
        if (extras.ContentTopicId > 0)
        {
            var topic = await _topicService.GetTopicByIdAsync(extras.ContentTopicId);
            if (topic is { Published: true } && await _aclService.AuthorizeAsync(topic) && await _storeMappingService.AuthorizeAsync(topic))
            {
                model.ContentHtml = await _localizationService.GetLocalizedAsync(topic, x => x.Body, languageId);
                if (string.IsNullOrWhiteSpace(model.ContentTitle))
                    model.ContentTitle = await _localizationService.GetLocalizedAsync(topic, x => x.Title, languageId);
            }
        }
        else
            model.ContentHtml = await _localizationService.GetLocalizedAsync(extras, x => x.ContentHtml, languageId);

        return model;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (additionalData is not MenuItemModel item)
            return Content(string.Empty);

        var key = MegaMenuKeys.For(item);
        var extras = await _megaMenuService.GetByKeyAsync(key);
        if (extras is not { HasPanelColumns: true })
            return Content(string.Empty);

        //the theme renders the zone twice per item (desktop and mobile menu): prepare the model once per request
        var cacheKey = $"{NopwaraMegaMenuDefaults.HTTP_CONTEXT_ITEMS_KEY}.panel.{key}";
        if (HttpContext.Items[cacheKey] is not MegaMenuPanelModel model)
        {
            model = await PreparePanelModelAsync(extras);
            HttpContext.Items[cacheKey] = model;
        }

        if (model.IsEmpty)
            return Content(string.Empty);

        //the mobile menu asks for the promo only
        ViewData["Mobile"] = widgetZone == NopwaraMegaMenuDefaults.PANEL_WIDGET_ZONE_MOBILE;

        return View($"~/Plugins/{NopwaraMegaMenuDefaults.PLUGIN_FOLDER}/Views/Panel.cshtml", model);
    }

    #endregion
}
