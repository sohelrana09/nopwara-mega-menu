# Nopwara Mega Menu for nopCommerce

Mega menu extras for the **nopCommerce 5.00 main menu**, drawn by the [Nopwara theme](https://github.com/sohelrana09/nopwara-theme).

nopCommerce 5.00 already has a menu builder: Content management → Menus, with category, manufacturer, vendor, topic, product and custom-link items, plus picture-grid or list dropdowns. This plugin doesn't replace it. You keep building the menu there, and add the extras that premium mega menus have:

- **Badges:** "New", "Sale", "Hot"… next to an item, in five colours.
- **Icons:** 20 line icons (Heroicons) shown before an item's title.
- **"View all" links** at the end of a dropdown, with customisable text ("View all {0}").
- **Full-width dropdowns** for any item, not just picture grids.
- **Promo banner column:** a picture with a title, a subtitle and a link.
- **Featured products column:** up to 4 product cards with picture, name and price, including old prices. They're customer- and store-aware, and hidden products are skipped.
- **Content column:** a topic (its ACL and store rules apply) or custom HTML. Useful for a "Help" item showing your shipping and returns info.
- **Slide-in mobile menu** (the Nopwara theme's mobile drawer):
  - each level slides in with a Back button and a "View all" link;
  - the promo banner shows at the bottom of its level.

Everything is translatable (badge, "View all" text, promo texts, titles, HTML).

It works with menus set to **"Display all categories"** too. Those categories have no menu items in core, so the plugin lists them, and their subcategories, on its own page.

## Requirements

- nopCommerce **5.00**
- The [Nopwara theme](https://github.com/sohelrana09/nopwara-theme). Its main menu renders the extras. Without the plugin, the theme's menu works exactly as before.

## Install

1. Copy this folder to `src/Plugins/`, add the project to `NopCommerce.sln` and build.
2. Install **Nopwara mega menu** (Configuration → Local plugins) and restart. The widget is activated on install.
3. Go to **Content management → Mega menu**, open a menu item and add its extras.

Or install the whole Nopwara set in one upload with [nopwara-suite](https://github.com/sohelrana09/nopwara-suite).

## How it works

| Part | What it does |
|---|---|
| `NopwaraMegaMenuItem` table | One row per menu item (`MenuItemId`) or generated category (`CategoryId`) with extras. Deleting a core menu item deletes its row. |
| `MegaMenuModelConsumer` | Listens for core's `ModelPreparedEvent` of the main menu. It puts each item's badge, icon, "View all" and layout into `HttpContext.Items["nopwara.megamenu"]` for the current request. Core caches the menu model and shares it between requests, so it's never modified. |
| `MegaMenuPanelViewComponent` | Renders the promo, products and content columns in the theme's widget zones `nopwara_megamenu_panel` (dropdowns) and `nopwara_megamenu_panel_mobile` (mobile levels, promo only). |
| Admin | Content management → Mega menu: the main menus as trees, plus an edit page per item. Products are picked with core's product picker. Permissions are core's menu permissions. |

Keys used by the theme: `i{menuItemId}` for menu items, `c{categoryId}` for category items core generates.

## Settings

| Setting | Default |
|---|---|
| Product picture size | 160 px |
| Banner picture size | 600 px |

## License

Copyright (C) 2026 Sohel Rana

Licensed under the [GNU Affero General Public License v3.0](LICENSE), like nopCommerce (the [nopCommerce Public License](https://www.nopcommerce.com/license) is AGPL v3 plus its additional terms). A store that uses it must keep nopCommerce's "Powered by nopCommerce" link. The icons are [Heroicons](https://heroicons.com) (MIT).
