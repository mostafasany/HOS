
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.MenuTree.Models;
using Nop.Plugin.MenuTree.Services;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.MenuTree.Domain;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Nop.Web.Framework.Models;
using System.Collections.Generic;
using Nop.Services.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Web.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class MenuTreeController : BasePluginController
    {

        private readonly IMenuTreeService _menutreeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _brandService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;

        public MenuTreeController(
              IMenuTreeService menutreeService
            , IUrlRecordService urlRecordService
            , ILocalizationService localizationService
            , INotificationService notificationService
            , ILanguageService languageService
            , ILocalizedEntityService localizedEntityService
            , ICategoryService categoryService
            , IProductService productService
            , IManufacturerService brandService
            , IBaseAdminModelFactory baseAdminModelFactory
            )
        {
            this._menutreeService = menutreeService;
            this._urlRecordService = urlRecordService;
            this._localizationService = localizationService;
            this._notificationService = notificationService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._brandService = brandService;
            this. _baseAdminModelFactory= baseAdminModelFactory;

    }

        protected virtual void UpdateLocales(MenuTree menutree, MenuTreeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(menutree,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                var seName = _urlRecordService.ValidateSeName(menutree, localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(menutree, seName, localized.LanguageId);
            }
        }

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            var model = new MenuTreeSearchModel();
            return View("/Plugins/MenuTree/Views/MenuTree/List.cshtml",model);
        }

        [HttpPost]
        public virtual IActionResult List(MenuTreeSearchModel searchModel)
        {
            var menutrees = _menutreeService.GetAllMenutree(searchModel.SearchMenuTreeName).ToPagedList(searchModel);

            var gridModel = new MenuTreeListModel().PrepareToGrid(searchModel, menutrees, () =>
            {
                return menutrees.Select(menutree => new MenuTreeModel
                {
                    Id = menutree.Id,
                    Name = menutree.Name,
                    Published=menutree.Published,
                    DisplayOrder=menutree.DisplayOrder,
                    ShowOnHomePage=menutree.ShowOnHomePage
                });
            });

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            var model = PrepareMenutreeModel(new MenuTreeModel());

            return View("/Plugins/MenuTree/Views/MenuTree/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(MenuTreeModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
               
                var menutree = new MenuTree();
                menutree.CreatedOnUtc = DateTime.UtcNow;
                menutree.Deleted = false;
                menutree.DisplayIn = model.DisplayIn;
                menutree.DisplayOrder = model.DisplayOrder;
                menutree.EntityName = model.EntityName;
                menutree.Name = model.Name;
                menutree.ParentId = model.ParentId;
                menutree.Published = model.Published;
                menutree.ShowOnHomePage = model.ShowOnHomePage;
                menutree.UpdatedOnUtc = DateTime.UtcNow;

                _menutreeService.InsertMenuTree(menutree);

                model.SeName = _urlRecordService.ValidateSeName(menutree, model.SeName, menutree.Name, true);
                _urlRecordService.SaveSlug(menutree, model.SeName, 0);

               UpdateLocales(menutree, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("New Menu Item Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = menutree.Id });
            }

               return View("/Plugins/MenuTree/Views/MenuTree/Create.cshtml" , model);
           }

        public virtual IActionResult Edit(int id)
        {

            var menutree = _menutreeService.GetMenutreeById(id);
            if (menutree == null || menutree.Deleted)
                return RedirectToAction("List");

            var model = PrepareMenutreeeditModel(menutree);

            return View("/Plugins/MenuTree/Views/MenuTree/Edit.cshtml" , model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(MenuTreeModel model, bool continueEditing)
        {
            var menutree = _menutreeService.GetMenutreeById(model.Id);
            if (menutree == null || menutree.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                menutree.DisplayIn = model.DisplayIn;
                menutree.DisplayOrder = model.DisplayOrder;
                menutree.EntityName = model.EntityName;
                menutree.Name = model.Name;
                menutree.ParentId = model.ParentId;
                menutree.Published = model.Published;
                menutree.ShowOnHomePage = model.ShowOnHomePage;
                menutree.UpdatedOnUtc = DateTime.UtcNow;

                _menutreeService.UpdateMenuTree(menutree);

               model.SeName = _urlRecordService.ValidateSeName(menutree, model.SeName, menutree.Name, true);
                _urlRecordService.SaveSlug(menutree, model.SeName, 0);

                UpdateLocales(menutree, model);

                _notificationService.SuccessNotification("Menu Tree Item Updated");

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = menutree.Id });
            }

            return View("/Plugins/MenuTree/Views/MenuTree/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {

            var menutree = _menutreeService.GetMenutreeById(id);
            if (menutree == null)
                return RedirectToAction("List");

            menutree.Deleted = true;
            _menutreeService.DeleteMenuTree(menutree);
    
            _notificationService.SuccessNotification("MenuTree Item is deleted");

            return RedirectToAction("List");
        }


        [HttpPost]
        public virtual IActionResult ItemList(MenuTreeItemSearchModel searchModel)
        {
            var menutree = _menutreeService.GetMenutreeById(searchModel.menutreeId)
                ?? throw new ArgumentException("No menutree found with the specified id");

            var model = PrepareMenuTreeItemListModel(searchModel, menutree);

            return Json(model);
        }

        public virtual IActionResult ItemUpdate(MenuTreeItemModel model)
        {
            var MenuTreeItem = _menutreeService.GetMenuTreeItemById(model.Id)
                ?? throw new ArgumentException("No MenuTreeItem mapping found with the specified id");

            MenuTreeItem.DisplayOrder = model.DisplayOrder;
            MenuTreeItem.Published = model.Published;
            MenuTreeItem.UpdatedOnUtc = DateTime.UtcNow;
            _menutreeService.UpdateMenuTreeItem(MenuTreeItem);

            return new NullJsonResult();
        }

        public virtual IActionResult ItemDelete(int id)
        {
            var MenuTreeItem = _menutreeService.GetMenuTreeItemById(id)
                ?? throw new ArgumentException("No Menu Tree Item mapping found with the specified id", nameof(id));

            MenuTreeItem.Deleted = true;

            _menutreeService.UpdateMenuTreeItem(MenuTreeItem);

            return new NullJsonResult();
        }

        public virtual IActionResult ItemAddPopup(int MenutreeId)
        {
            var model = new AddItemToMenuTreeSearchModel();
            var menutree = _menutreeService.GetMenutreeById(MenutreeId);
            if (menutree != null)
            model.EntityName = menutree.EntityName;

            _baseAdminModelFactory.PrepareCategories(model.AvailableCategories);

            _baseAdminModelFactory.PrepareManufacturers(model.AvailableManufacturers);

            _baseAdminModelFactory.PrepareStores(model.AvailableStores);

            _baseAdminModelFactory.PrepareVendors(model.AvailableVendors);

            _baseAdminModelFactory.PrepareProductTypes(model.AvailableProductTypes);

            model.SetPopupGridPageSize();

            return View("/Plugins/MenuTree/Views/MenuTree/ItemAddPopup.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult ItemAddPopupList(AddItemToMenuTreeSearchModel searchModel)
        {
            var model = PrepareAddItemToMenuTreeListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ItemAddPopup(AddItemToMenuTreeModel model)
        {
            var menutree = _menutreeService.GetMenutreeById(model.MenuTreeId);
            if(menutree!=null && menutree.EntityName=="Product")
            {
                var selectedItems = _productService.GetProductsByIds(model.SelectedItemIds.ToArray());
                if (selectedItems.Any())
                {
                    var existingMenuTreeItems = _menutreeService.GetItemMenutreesByMenutreeId(model.MenuTreeId);
                    foreach (var Item in selectedItems)
                    {
                        if (_menutreeService.FindMenuTreeItem(existingMenuTreeItems, Item.Id, model.MenuTreeId) != null)
                            continue;
                        _menutreeService.InsertMenuTreeItem(new MenuTreeItem
                        {
                            MenuTreeId = model.MenuTreeId,
                            ItemId = Item.Id,
                            Deleted = false,
                            Published = false ,
                            DisplayOrder= 1 ,
                            CreatedOnUtc= DateTime.UtcNow ,
                            UpdatedOnUtc = DateTime.UtcNow
                        });
                    }
                }

            }
            else if (menutree != null && menutree.EntityName == "Brand")
            {
                var selectedItems = new List<Manufacturer>();
                foreach (var id  in model.SelectedItemIds.ToArray())
                {
                  selectedItems.Add(_brandService.GetManufacturerById(id));
                }
                if (selectedItems.Any())
                {
                    var existingMenuTreeItems = _menutreeService.GetItemMenutreesByMenutreeId(model.MenuTreeId);
                    foreach (var Item in selectedItems)
                    {
                        if (_menutreeService.FindMenuTreeItem(existingMenuTreeItems, Item.Id, model.MenuTreeId) != null)
                            continue;

                        _menutreeService.InsertMenuTreeItem(new MenuTreeItem
                        {
                            MenuTreeId = model.MenuTreeId,
                            ItemId = Item.Id,
                            Deleted = false,
                            Published = false,
                            DisplayOrder = 1,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        });
                    }
                }

            }
            else if (menutree != null && menutree.EntityName == "Category")
            {

                  var  selectedItems=_categoryService.GetCategoriesByIds(model.SelectedItemIds.ToArray());

                if (selectedItems.Any())
                {
                    var existingMenuTreeItems = _menutreeService.GetItemMenutreesByMenutreeId(model.MenuTreeId);
                    foreach (var Item in selectedItems)
                    {
                        if (_menutreeService.FindMenuTreeItem(existingMenuTreeItems, Item.Id, model.MenuTreeId) != null)
                            continue;
                        _menutreeService.InsertMenuTreeItem(new MenuTreeItem
                        {
                            MenuTreeId = model.MenuTreeId,
                            ItemId = Item.Id,
                            Deleted = false,
                            Published = false,
                            DisplayOrder = 1,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        });
                    }
                }

            }
            else if (menutree != null && menutree.EntityName == "Article")
            {

                var selectedItems = _menutreeService.GetArticlesByIds(model.SelectedItemIds.ToArray());

                if (selectedItems.Any())
                {
                    var existingMenuTreeItems = _menutreeService.GetItemMenutreesByMenutreeId(model.MenuTreeId);
                    foreach (var Item in selectedItems)
                    {
                        if (_menutreeService.FindMenuTreeItem(existingMenuTreeItems, Item.Id, model.MenuTreeId) != null)
                            continue;
                        _menutreeService.InsertMenuTreeItem(new MenuTreeItem
                        {
                            MenuTreeId = model.MenuTreeId,
                            ItemId = Item.Id,
                            Deleted = false,
                            Published = false,
                            DisplayOrder = 1,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        });
                    }
                }

            }


            ViewBag.RefreshPage = true;

            return View("/Plugins/MenuTree/Views/MenuTree/ItemAddPopup.cshtml", new AddItemToMenuTreeSearchModel());
        }



        public MenuTreeModel PrepareMenutreeModel(MenuTreeModel MenuTreeModel)
        {
            Action<MenuTreeLocalizedModel, int> localizedModelConfiguration = null;

            var mtmodel = new MenuTreeModel();

            mtmodel.AvailableEntities.Add(new SelectListItem { Value = "0", Text = "Select Type of Items" });
            mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Product", Text = "Product" });
            mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Brand", Text = "Brand" });
            mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Category", Text = "Category" });
            mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Article", Text = "Article" });

            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "NoChoice", Text = "Select Display" });
            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "Web", Text = "Web" });
            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "Mobile", Text = "Mobile" });
            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "Both", Text = "Both" });

            var menutrees = _menutreeService.Menutree();
            mtmodel.AvailableMenuTree.Add(new SelectListItem { Value = 0.ToString(), Text = "No Parent" });

            if (menutrees != null)
            foreach(var item in menutrees)
            {
                    mtmodel.AvailableMenuTree.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
            }
            mtmodel.Locales = PrepareLocalizedModels(localizedModelConfiguration);

            return mtmodel;
        }

        public MenuTreeModel PrepareMenutreeeditModel(MenuTree menutree)
        {
            Action<MenuTreeLocalizedModel, int> localizedModelConfiguration = null;
            var mtmodel = new MenuTreeModel();
            mtmodel.CreatedOnUtc = menutree.CreatedOnUtc;
            mtmodel.UpdatedOnUtc = menutree.UpdatedOnUtc;
            mtmodel.Deleted = menutree.Deleted;
            mtmodel.DisplayOrder = menutree.DisplayOrder;
            mtmodel.EntityName = menutree.EntityName;
            mtmodel.DisplayIn = menutree.DisplayIn;
            mtmodel.Id = menutree.Id;
            mtmodel.Name = menutree.Name;
            mtmodel.ParentId = menutree.ParentId;
            mtmodel.Published = menutree.Published;
            mtmodel.ShowOnHomePage = menutree.ShowOnHomePage;
            mtmodel.SeName = _urlRecordService.GetActiveSlug(menutree.Id, "MenuTree", 0);

            mtmodel.MenuTreeItemSearchModel.menutreeId = menutree.Id;

            var menutrees = _menutreeService.Menutree();
            mtmodel.AvailableMenuTree.Add(new SelectListItem { Value = 0.ToString(), Text = "No Parent" });
            if (menutrees != null)
                foreach (var item in menutrees)
                {
                    if(item.Id != mtmodel.Id)
                    if(item.Id== mtmodel.ParentId)
                    mtmodel.AvailableMenuTree.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name , Selected=true });
                    else
                    mtmodel.AvailableMenuTree.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name, Selected = false });

                }
            if(menutree.EntityName!=0.ToString())
            {
                mtmodel.AvailableEntities.Add(new SelectListItem { Value = menutree.EntityName, Text = menutree.EntityName });
            }
            else
            {
                mtmodel.AvailableEntities.Add(new SelectListItem { Value = "0", Text = "Select Type of Items" });
                mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Product", Text = "Product", Selected = mtmodel.EntityName == "Product" ? true : false });
                mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Brand", Text = "Brand", Selected = mtmodel.EntityName == "Brand" ? true : false });
                mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Category", Text = "Category", Selected = mtmodel.EntityName == "Category" ? true : false });
                mtmodel.AvailableEntities.Add(new SelectListItem { Value = "Article", Text = "Article", Selected = mtmodel.EntityName == "Article" ? true : false });
            }


            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "NoChoice", Text = "Select Display" });
            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "Web", Text = "Web", Selected = mtmodel.DisplayIn == "Web" ? true : false });
            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "Mobile", Text = "Mobile", Selected = mtmodel.DisplayIn == "Mobile" ? true : false });
            mtmodel.AvailableDisplayOptions.Add(new SelectListItem { Value = "Both", Text = "Both", Selected = mtmodel.DisplayIn == "Both" ? true : false  });

            localizedModelConfiguration = (locale, languageId) =>
            {
                locale.Name = _localizationService.GetLocalized(menutree, entity => entity.Name, languageId, false, false);
                locale.SeName = _urlRecordService.GetSeName(menutree, languageId, false, false);
            };

            mtmodel.Locales = PrepareLocalizedModels(localizedModelConfiguration);

            return mtmodel;
        }

        public virtual IList<T> PrepareLocalizedModels<T>(Action<T, int> configure = null) where T : ILocalizedLocaleModel
        {
            var availableLanguages = _languageService.GetAllLanguages(showHidden: true);

            var localizedModels = availableLanguages.Select(language =>
            {
                var localizedModel = Activator.CreateInstance<T>();

                localizedModel.LanguageId = language.Id;

                if (configure != null)
                    configure.Invoke(localizedModel, localizedModel.LanguageId);

                return localizedModel;
            }).ToList();

            return localizedModels;
        }

        public virtual MenuTreeItemListModel PrepareMenuTreeItemListModel(MenuTreeItemSearchModel searchModel, MenuTree menuTree)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (menuTree == null)
                throw new ArgumentNullException(nameof(menuTree));

            var menutreeitems = _menutreeService.GetItemMenutreesByMenutreeId(menuTree.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new MenuTreeItemListModel().PrepareToGrid(searchModel, menutreeitems, () =>
            {
                return menutreeitems.Select(menutreeitem =>
                {
                    var MenuTreeItemModel = new MenuTreeItemModel();
                    MenuTreeItemModel.Id = menutreeitem.Id;
                    MenuTreeItemModel.ItemId = menutreeitem.ItemId;
                    MenuTreeItemModel.MenuTreeId = menutreeitem.MenuTreeId;
                    MenuTreeItemModel.DisplayOrder = menutreeitem.DisplayOrder;
                    MenuTreeItemModel.Published = menutreeitem.Published;

                    if (menuTree.EntityName== "Product")
                    {
                        MenuTreeItemModel.Name = _productService.GetProductById(menutreeitem.ItemId)?.Name;
                    }
                    else if(menuTree.EntityName == "Brand")
                    {
                        MenuTreeItemModel.Name = _brandService.GetManufacturerById(menutreeitem.ItemId)?.Name;
                    }
                    else if(menuTree.EntityName == "Category")
                    {
                        MenuTreeItemModel.Name = _categoryService.GetCategoryById(menutreeitem.ItemId)?.Name;

                    }
                    else if (menuTree.EntityName == "Article")
                    {
                        MenuTreeItemModel.Name = _menutreeService.GetArticleById(menutreeitem.ItemId)?.Title;

                    }
                    return MenuTreeItemModel;
                });
            });

            return model;
        }




        public virtual AddItemToMenuTreeListModel PrepareAddItemToMenuTreeListModel(AddItemToMenuTreeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if(searchModel.EntityName=="Product")
            {
                var products = _productService.SearchProducts(showHidden: true,
                    categoryIds: new List<int> { searchModel.SearchCategoryId },
                    manufacturerId: searchModel.SearchManufacturerId,
                    storeId: searchModel.SearchStoreId,
                    vendorId: searchModel.SearchVendorId,
                    productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                    keywords: searchModel.SearchProductName,
                    pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
                var model = new AddItemToMenuTreeListModel().PrepareToGrid(searchModel, products, () =>
                {
                    return products.Select(product =>
                    {
                        var productModel = new ItemModel();
                        productModel.Id = product.Id;
                        productModel.Name = product.Name;

                        return productModel;
                    });
                });

                return model;

            }
            else if(searchModel.EntityName == "Category")
            {
            var categories = _categoryService.GetAllCategories(categoryName: searchModel.SearchCategoryName,
               showHidden: true,
               storeId: searchModel.SearchStoreId,
               pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new AddItemToMenuTreeListModel().PrepareToGrid(searchModel, categories, () =>
                {
                    return categories.Select(category =>
                    {
                        var categoryModel = new ItemModel();
                        categoryModel.Id = category.Id;
                        categoryModel.Name = category.Name;

                        return categoryModel;
                    });
                });

                return model;
            }
            else if (searchModel.EntityName == "Brand")
            {
                var brands = _brandService.GetAllManufacturers(showHidden: true,
                    storeId: searchModel.SearchStoreId,
                    manufacturerName: searchModel.SearchBrandName,
                    pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
                var model = new AddItemToMenuTreeListModel().PrepareToGrid(searchModel, brands, () =>
                {
                    return brands.Select(brand =>
                    {
                        var brandModel = new ItemModel();
                        brandModel.Id = brand.Id;
                        brandModel.Name = brand.Name;

                        return brandModel;
                    });
                });

                return model;
            }
            else if (searchModel.EntityName == "Article")
            {
                var articles = _menutreeService.GetAllArticles(
                    articlename: searchModel.SearchArticleName,
                    pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
                var model = new AddItemToMenuTreeListModel().PrepareToGrid(searchModel, articles, () =>
                {
                    return articles.Select(article =>
                    {
                        var articlemodel = new ItemModel();
                        articlemodel.Id = article.Id;
                        articlemodel.Name = article.Title;

                        return articlemodel;
                    });
                });

                return model;
            }

            return null;
              
        }
    }
}