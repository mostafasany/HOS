using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Article.Service;
using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Category.Service;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Service;
using Nop.Plugin.Api.Modules.Menu.Dto;
using Nop.Plugin.Api.Product.Modules.Product.Service;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Modules.Menu.Service
{
    public class MenuApiService : IMenuApiService
    {
        private readonly IArticleApiService _articleApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IPictureService _pictureService;
        private readonly IProductApiService _productApiService;
        private readonly IUrlRecordService _urlRecordService;
        private const int ProductCategoryId = 34;
        private const int ExercisesArticlesCategoryId = 37;
        private const int NutrationArticlesCategoryId = 38;
        private const int ArticlesAndVideosArticlesCategoryId = 39;
        private const int CategoiresCategoryId = 55;

        public MenuApiService(IProductApiService productApiService, ICategoryApiService categoryApiService, IUrlRecordService urlRecordService, ILocalizationService localizationService,
            IPictureService pictureService, IManufacturerApiService manufacturerApiService, IArticleApiService articleApiService, IHttpContextAccessor httpContextAccessor)
        {
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _manufacturerApiService = manufacturerApiService;
            _articleApiService = articleApiService;
            _localizationService = localizationService;
            IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                StringValues lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public MenuDto GetMenu()
        {
            IList<Core.Domain.Catalog.Category> allCategories = _categoryApiService.GetCategories(limit: 1000).Where(cat => cat.IncludeInTopMenu).ToList();
            List<CategoryDto> allCategoriesDto = allCategories.Select(a => new CategoryDto
            {
                Description = _localizationService.GetLocalized(a, x => x.Description, _currentLangaugeId),
                Name = _localizationService.GetLocalized(a, x => x.Name, _currentLangaugeId),
                SeName = _urlRecordService.GetSeName(a),
                Id = a.Id,
                ParentCategoryId = a.ParentCategoryId
            }).ToList();
            List<CategoryDto> exercisesArticlesCategories = allCategoriesDto.Where(cat => cat.ParentCategoryId == ExercisesArticlesCategoryId).ToList();
            List<CategoryDto> nutrationArticlesCategories = allCategoriesDto.Where(cat => cat.ParentCategoryId == NutrationArticlesCategoryId).ToList();
            List<CategoryDto> articlesAndVideosCategories = allCategoriesDto.Where(cat => cat.ParentCategoryId == ArticlesAndVideosArticlesCategoryId).ToList();
            List<CategoryDto> productsCategories = allCategoriesDto.Where(cat => cat.ParentCategoryId == ProductCategoryId).ToList();
            List<CategoryDto> categoriesCategories = allCategoriesDto.Where(cat => cat.ParentCategoryId == CategoiresCategoryId).ToList();

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

            List<CategoryDto> allManufacturers = _manufacturerApiService.GetManufacturers(limit: 5).Select(a => new CategoryDto
            {
                Description = _localizationService.GetLocalized(a, x => x.Description, _currentLangaugeId),
                Name = _localizationService.GetLocalized(a, x => x.Name, _currentLangaugeId),
                SeName = _urlRecordService.GetSeName(a),
                Id = a.Id
            }).ToList();

            if (allManufacturers.Any())
                mainStoreCategories.Add(new MenuCategoriesDto
                {
                    Id = -1,
                    MenuItemName = "Brands",
                    SeName = "brands",
                    CategoriesFirstRow = allManufacturers
                });
            return mainStoreCategories;
        }

        private List<MenuCategoriesDto> GetMenuCategories(List<CategoryDto> allCategoriesDto, List<CategoryDto> allMenuCategoriesDto)
        {
            var mainStoreCategories = new List<MenuCategoriesDto>();

            if (allMenuCategoriesDto == null)
                return mainStoreCategories;

            foreach (CategoryDto categroyDto in allMenuCategoriesDto)
            {
                List<CategoryDto> ingerdientCategories = allCategoriesDto.Where(a => a.ParentCategoryId == categroyDto.Id).ToList();
                ingerdientCategories.Add(new CategoryDto { Name = "All " + categroyDto.Name, SeName = "all-" + categroyDto.SeName, Id = categroyDto.Id });
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

            foreach (CategoryDto categoryDto in allCategoriesDto)
            {
                IEnumerable<ProductDto> products = GetProducts(categoryDto);
                int firstRowItemCount = RandomNumber(2, 4);
                int secondRowItemCount = RandomNumber(2, 4);
                int thirdRowItemCount = RandomNumber(2, 4);
                mainStoreProducts.Add(new MenuProductsDto
                {
                    Id = categoryDto.Id,
                    MenuItemName = categoryDto.Name,
                    SeName = categoryDto.SeName,
                    ProductsFirstRow = products.Skip(0).Take(firstRowItemCount),
                    ProductsSecondRow = products.Skip(firstRowItemCount).Take(secondRowItemCount),
                    ProductsThirdRow = products.Skip(secondRowItemCount).Take(thirdRowItemCount)
                });
            }

            return mainStoreProducts;
        }

        private List<MenuArticlesDto> GetMeuArticles(List<CategoryDto> categoriesDto, bool oneRow = false)
        {
            var articles = new List<MenuArticlesDto>();
            foreach (CategoryDto category in categoriesDto)
            {
                List<ArticlesDto> articlesDto = _articleApiService.GetArticles(categoryId: category.Id, limit: 5).Item1.Select(a => new ArticlesDto
                {
                    SeName = a.Title,
                    Id = a.Id,
                    Title = _localizationService.GetLocalized(a, x => x.Title, _currentLangaugeId),
                    Image = new ImageDto { Src = _pictureService.GetPictureUrl(a.PictureId) },
                    Body = _localizationService.GetLocalized(a, x => x.Body, _currentLangaugeId)
                }).ToList();
                MenuArticlesDto dto;
                int firstRowItemCount = RandomNumber(2, 4);
                int secondRowItemCount = RandomNumber(2, 4);
                int thirdRowItemCount = RandomNumber(2, 4);
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
                        ArticlesThirdRow = articlesDto.Skip(secondRowItemCount).Take(thirdRowItemCount).ToList()
                    };

                articles.Add(dto);
            }

            return articles;
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private List<ProductDto> GetProducts(CategoryDto category, int limit = 9)
        {
            IEnumerable<ProductDto> products = _productApiService.GetProducts(categoryId: category.Id, limit: limit).Item1.Select(product => new ProductDto
            {
                ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, _currentLangaugeId),
                Name = _localizationService.GetLocalized(product, x => x.Name, _currentLangaugeId),
                Price = product.Price,
                SeName = _urlRecordService.GetSeName(product),
                Images = new List<ImageMappingDto>
                {
                    new ImageMappingDto
                    {
                        Src = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()?.Picture)
                    }
                },
                Id = product.Id
            });
            return products.ToList();
        }
    }
}