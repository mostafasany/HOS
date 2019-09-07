using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Data;
using Nop.Core.Domain.Custom;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Article.Service;
using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Category.Service;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Service;
using Nop.Plugin.Api.Menu.Dto;
using Nop.Plugin.Api.Product.Modules.Product.Service;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Menu.Service
{
    public class MenuApiService : IMenuApiService
    {
        private const int ProductCategoryId = 34;
        private const int ExercisesArticlesCategoryId = 37;
        private const int NutrationArticlesCategoryId = 38;
        private const int ArticlesAndVideosArticlesCategoryId = 39;
        private const int CategoiresCategoryId = 55;
        private readonly IArticleApiService _articleApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IPictureService _pictureService;
        private readonly IProductApiService _productApiService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IRepository<MenuTree> _menutreeRepository;
        private readonly IRepository<MenuTreeItem> _menutreeItemRepository;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _brandService;

        public MenuApiService(IProductApiService productApiService, ICategoryApiService categoryApiService,
            IUrlRecordService urlRecordService, ILocalizationService localizationService,
            IPictureService pictureService, IManufacturerApiService manufacturerApiService,
            IArticleApiService articleApiService, IHttpContextAccessor httpContextAccessor,
            IRepository<MenuTree> menutreeRepository,
            ICategoryService categoryService,
            IProductService productService,
            IManufacturerService brandService,
            IRepository<MenuTreeItem> menutreeItemRepository)
        {
            _menutreeRepository = menutreeRepository;
            _menutreeItemRepository = menutreeItemRepository;
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _manufacturerApiService = manufacturerApiService;
            _articleApiService = articleApiService;
            _localizationService = localizationService;

            this._categoryService = categoryService;
            this._productService = productService;
            this._brandService = brandService;

            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                var lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }


        private MenuItemDto ToMenuItem(string entityName,MenuTreeItem menuTreeItem)
        {
            if(entityName=="Product")
            {
               var product= _productService.GetProductById(menuTreeItem.ItemId);
                return new MenuItemDto
                {
                    Id = product.Id,
                    Name = _localizationService.GetLocalized(product, x => x.Name, _currentLangaugeId),
                    Description=product.Price.ToString(),
                    SeName= _urlRecordService.GetSeName(product),
                    Image= _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()
                                ?.Picture),
                };
            }
            else if (entityName == "Brand")
            {
                var brand = _brandService.GetManufacturerById(menuTreeItem.ItemId);
                return new MenuItemDto
                {
                    Id = brand.Id,
                    Name = _localizationService.GetLocalized(brand, x => x.Name, _currentLangaugeId),
                    Description = _localizationService.GetLocalized(brand, x => x.Description, _currentLangaugeId),
                    SeName = _urlRecordService.GetSeName(brand),
                    Image = _pictureService.GetPictureUrl(brand.PictureId),

                };
            }
            else if (entityName == "Category")
            {
                var category = _categoryService.GetCategoryById(menuTreeItem.ItemId);
                return new MenuItemDto
                {
                    Id = category.Id,
                    Name = _localizationService.GetLocalized(category, x => x.Name, _currentLangaugeId),
                    Description = _localizationService.GetLocalized(category, x => x.Description, _currentLangaugeId),
                    SeName = _urlRecordService.GetSeName(category),
                    Image = _pictureService.GetPictureUrl(category.PictureId),

                };
            }
            else if (entityName == "Article")
            {
                var article = _articleApiService.GetArticleById(menuTreeItem.ItemId);
                return new MenuItemDto
                {
                    Id = article.Id,
                    Name = _localizationService.GetLocalized(article, x => x.Title, _currentLangaugeId),
                    Description = _localizationService.GetLocalized(article, x => x.Body, _currentLangaugeId),
                    SeName = article.Title,
                    Image = _pictureService.GetPictureUrl(article.PictureId),

                };
            }

            return null;

        }

        public List<MenuDto2> GetNewMenu()
        {
            var menutreeQuery = from mt in _menutreeRepository.Table
                                select mt;

            var menutreeItemQuery = from mt in _menutreeItemRepository.Table
                                    select mt;

            var menuTreeQuery = menutreeQuery.Where(mt => mt.Deleted == false && mt.Published == true && (mt.DisplayIn == "Both" || mt.DisplayIn == "Web"))
                .OrderBy(mt => mt.ParentId);

            var menuGroups = menuTreeQuery.GroupBy(mt => mt.ParentId);

            List<MenuDto2> list = new List<MenuDto2>();
            foreach (var group in menuGroups)
            {
                var parentItems = group.ToList().OrderBy(a => a.DisplayOrder);
                if (group.Key == 0)
                {
                    foreach (var item in parentItems)
                    {
                        list.Add(new MenuDto2
                        {
                            Id = item.Id,
                            Name = item.Name,
                            SeName = _urlRecordService.GetSeName(item),
                        });
                    }
                }
                else
                {
                    var parent = list.FirstOrDefault(a => a.Id == group.Key);
                    var parentIdex = list.IndexOf(parent);
                    foreach (var item in parentItems)
                    {
                        var menuItems = menutreeItemQuery.Where(a => a.MenuTreeId == item.Id && a.Deleted == false && a.Published == true)
                           .OrderBy(a => a.DisplayOrder);
                        List<MenuItemDto> items = new List<MenuItemDto>();
                        foreach (var menuItem in menuItems)
                        {
                            items.Add(ToMenuItem(item.EntityName, menuItem));
                        }
                        list[parentIdex].MenuTree.Add(new MenuDto2
                        {
                            Id = item.Id,
                            Name = item.Name,
                            SeName = item.Name,
                            MenuItems = items//menuItems.Select(a=>ToMenuItem(item.EntityName,a)).ToList()
                        });
                    }


                }
            }

            return list;
        }

        public MenuDto GetMenu()
        {
         
            IList<Core.Domain.Catalog.Category> allCategories = _categoryApiService.GetCategories(limit: 1000)
                .Where(cat => cat.IncludeInTopMenu).ToList();
            var allCategoriesDto = allCategories.Select(a => new CategoryDto
            {
                Description = _localizationService.GetLocalized(a, x => x.Description, _currentLangaugeId),
                Name = _localizationService.GetLocalized(a, x => x.Name, _currentLangaugeId),
                SeName = _urlRecordService.GetSeName(a),
                Id = a.Id,
                ParentCategoryId = a.ParentCategoryId
            }).ToList();
            var exercisesArticlesCategories =
                allCategoriesDto.Where(cat => cat.ParentCategoryId == ExercisesArticlesCategoryId).ToList();
            var nutrationArticlesCategories =
                allCategoriesDto.Where(cat => cat.ParentCategoryId == NutrationArticlesCategoryId).ToList();
            var articlesAndVideosCategories = allCategoriesDto
                .Where(cat => cat.ParentCategoryId == ArticlesAndVideosArticlesCategoryId).ToList();
            var productsCategories = allCategoriesDto.Where(cat => cat.ParentCategoryId == ProductCategoryId).ToList();
            var categoriesCategories =
                allCategoriesDto.Where(cat => cat.ParentCategoryId == CategoiresCategoryId).ToList();

            return new MenuDto
            {
                MainStoreProducts = GetMenuProducts(productsCategories),
                MainStoreCategories = GetMenuCategories(allCategoriesDto, categoriesCategories),
                MainStoreBrands = GetMenuBrands(),
                ArticlesAndVideos = GetMeuArticles(articlesAndVideosCategories),
                Nutrations = GetMeuArticles(nutrationArticlesCategories, true),
                Exercises = GetMeuArticles(exercisesArticlesCategories, true)
            };
        }

        private List<MenuCategoriesDto> GetMenuBrands()
        {
            var mainStoreCategories = new List<MenuCategoriesDto>();

            var allManufacturers = _manufacturerApiService.GetManufacturers(limit: 5).Select(a => new CategoryDto
            {
                Description = _localizationService.GetLocalized(a, x => x.Description, _currentLangaugeId),
                Name = _localizationService.GetLocalized(a, x => x.Name, _currentLangaugeId),
                SeName = _urlRecordService.GetSeName(a),
                Id = a.Id
            }).ToList();

            if (allManufacturers.Any())
                mainStoreCategories.Add(new MenuCategoriesDto
                {
                    Id = -1, MenuItemName = "Brands", SeName = "brands", CategoriesFirstRow = allManufacturers
                });
            return mainStoreCategories;
        }

        private List<MenuCategoriesDto> GetMenuCategories(List<CategoryDto> allCategoriesDto,
            List<CategoryDto> allMenuCategoriesDto)
        {
            var mainStoreCategories = new List<MenuCategoriesDto>();

            if (allMenuCategoriesDto == null)
                return mainStoreCategories;

            foreach (var categroyDto in allMenuCategoriesDto)
            {
                var ingerdientCategories = allCategoriesDto.Where(a => a.ParentCategoryId == categroyDto.Id).ToList();
                ingerdientCategories.Add(new CategoryDto
                {
                    Name = "All " + categroyDto.Name, SeName = "all-" + categroyDto.SeName, Id = categroyDto.Id
                });
                mainStoreCategories.Add(new MenuCategoriesDto
                {
                    Id = categroyDto.Id,
                    MenuItemName = categroyDto.Name,
                    SeName = categroyDto.SeName,
                    CategoriesFirstRow = ingerdientCategories
                });
            }

            return mainStoreCategories;
        }

        private List<MenuProductsDto> GetMenuProducts(List<CategoryDto> allCategoriesDto)
        {
            var mainStoreProducts = new List<MenuProductsDto>();
            if (allCategoriesDto == null)
                return mainStoreProducts;

            foreach (var categoryDto in allCategoriesDto)
            {
                IEnumerable<ProductDto> products = GetProducts(categoryDto);
                var firstRowItemCount = RandomNumber(2, 4);
                var secondRowItemCount = RandomNumber(2, 4);
                var thirdRowItemCount = RandomNumber(2, 4);
                mainStoreProducts.Add(new MenuProductsDto
                {
                    Id = categoryDto.Id,
                    MenuItemName = categoryDto.Name,
                    SeName = categoryDto.SeName,
                    ProductsFirstRow = products.Skip(0).Take(firstRowItemCount),
                    ProductsSecondRow = products.Skip(firstRowItemCount).Take(secondRowItemCount),
                    ProductsThirdRow = products.Skip(firstRowItemCount + secondRowItemCount).Take(thirdRowItemCount)
                });
            }

            return mainStoreProducts;
        }

        private List<MenuArticlesDto> GetMeuArticles(List<CategoryDto> categoriesDto, bool oneRow = false)
        {
            var articles = new List<MenuArticlesDto>();
            foreach (var category in categoriesDto)
            {
                var articlesDto = _articleApiService.GetArticles(categoryId: category.Id, limit: 5).Item1.Select(a =>
                    new ArticlesDto
                    {
                        SeName = a.Title,
                        Id = a.Id,
                        Title = _localizationService.GetLocalized(a, x => x.Title, _currentLangaugeId),
                        Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                        Body = _localizationService.GetLocalized(a, x => x.Body, _currentLangaugeId)
                    }).ToList();
                MenuArticlesDto dto;
                var firstRowItemCount = RandomNumber(2, 4);
                var secondRowItemCount = RandomNumber(2, 4);
                var thirdRowItemCount = RandomNumber(2, 4);
                if (oneRow)
                    dto = new MenuArticlesDto
                    {
                        Id = category.Id,
                        MenuItemName = category.Name,
                        SeName = category.SeName,
                        ArticlesFirstRow = articlesDto.ToList()
                    };
                else
                    dto = new MenuArticlesDto
                    {
                        Id = category.Id,
                        MenuItemName = category.Name,
                        SeName = category.SeName,
                        ArticlesFirstRow = articlesDto.Skip(0).Take(firstRowItemCount).ToList(),
                        ArticlesSecondRow = articlesDto.Skip(firstRowItemCount).Take(secondRowItemCount).ToList(),
                        ArticlesThirdRow = articlesDto.Skip(firstRowItemCount + secondRowItemCount)
                            .Take(thirdRowItemCount).ToList()
                    };

                articles.Add(dto);
            }

            return articles;
        }

        private int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        private List<ProductDto> GetProducts(CategoryDto category, int limit = 9)
        {
            var products = _productApiService.GetProducts(categoryId: category.Id, limit: limit).Item1.Select(product =>
                new ProductDto
                {
                    ShortDescription =
                        _localizationService.GetLocalized(product, x => x.ShortDescription, _currentLangaugeId),
                    Name = _localizationService.GetLocalized(product, x => x.Name, _currentLangaugeId),
                    Price = product.Price,
                    SeName = _urlRecordService.GetSeName(product),
                    Images = new List<ImageMappingDto>
                    {
                        new ImageMappingDto
                        {
                            Src = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()
                                ?.Picture)
                        }
                    },
                    Id = product.Id
                });
            return products.ToList();
        }
    }
}
