using FluentValidation;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Domain;
using Nop.Plugin.Widgets.NopwaraMegaMenu.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.NopwaraMegaMenu.Validators;

/// <summary>
/// Represents the mega menu item model validator (lengths match the NopwaraMegaMenuItem columns)
/// </summary>
public class MegaMenuItemModelValidator : BaseNopValidator<MegaMenuItemModel>
{
    #region Ctor

    public MegaMenuItemModelValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.BadgeText).MaximumLength(50);
        RuleFor(model => model.ViewAllText).MaximumLength(200);
        RuleFor(model => model.PromoTitle).MaximumLength(200);
        RuleFor(model => model.PromoSubtitle).MaximumLength(400);
        RuleFor(model => model.ProductsTitle).MaximumLength(200);
        RuleFor(model => model.ContentTitle).MaximumLength(200);
        RuleFor(model => model.PromoUrl)
            .MaximumLength(1000)
            .Must(MegaMenuUrls.IsSafe)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.NopwaraMegaMenu.Fields.PromoUrl.Invalid"));
    }

    #endregion
}
