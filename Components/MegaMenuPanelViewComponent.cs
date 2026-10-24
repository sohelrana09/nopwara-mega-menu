using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Models;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
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
        IStoreMappingService storeMappingService,
        ITopicService topicService,
        IWorkContext workContext,
        MegaMenuService megaMenuService,
        NopwaraMegaMenuSettings settings)
    {
        _aclService = aclService;
        _localizationService = localizationService;
        _pictureService = pictureService;
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
            //rows saved before the URL check existed may still hold an unsafe link
            model.PromoUrl = MegaMenuUrls.IsSafe(extras.PromoUrl) ? extras.PromoUrl : null;
            model.PromoTitle = await _localizationService.GetLocalizedAsync(extras, x => x.PromoTitle, languageId);
            model.PromoSubtitle = await _localizationService.GetLocalizedAsync(extras, x => x.PromoSubtitle, languageId);
        }

        //featured products: only counted here; NopwaraMegaMenuController.Products prepares them when the dropdown first opens
        model.ProductCount = Math.Min(extras.GetProductIds().Count, NopwaraMegaMenuDefaults.MAX_PRODUCTS);
        if (model.HasProducts)
        {
            model.ProductsUrl = Url.RouteUrl(NopwaraMegaMenuDefaults.PRODUCTS_ROUTE_NAME, new { key = extras.Key });
            model.ProductsTitle = await _localizationService.GetLocalizedAsync(extras, x => x.ProductsTitle, languageId);
        }

        //content: a topic (with its ACL/store rules) or HTML
        model.ContentTitle = await _localizationService.GetLocalizedAsync(extras, x => x.ContentTitle, languageId);
        if (extras.ContentTopicId > 0)
        {
            var topic = await _topicService.GetTopicByIdAsync(extras.ContentTopicId);
            if (topic is { Published: true, IsPasswordProtected: false } && _topicService.TopicIsAvailable(topic)
                && await _aclService.AuthorizeAsync(topic) && await _storeMappingService.AuthorizeAsync(topic))
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
