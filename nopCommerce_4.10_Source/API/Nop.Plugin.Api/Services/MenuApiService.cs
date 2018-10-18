using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Articles;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Menu;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Services
{
    public class MenuApiService : IMenuApiService
    {
        private readonly IArticleApiService _articleApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IPictureService _pictureService;
        private readonly IProductApiService _productApiService;
        private readonly IUrlRecordService _urlRecordService;

        public MenuApiService(IProductApiService productApiService, ICategoryApiService categoryApiService, IUrlRecordService urlRecordService,
            IPictureService pictureService, IManufacturerApiService manufacturerApiService, IArticleApiService articleApiService)
        {
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _manufacturerApiService = manufacturerApiService;
            _articleApiService = articleApiService;
        }

        public MenuDto GetMenu()
        {
            IList<Category> exercisesArticlesCategories = _categoryApiService.GetCategories(parenttId: 37);
            IList<Category> nutrationArticlesCategories = _categoryApiService.GetCategories(parenttId: 38);
            IList<Category> articlesAndVideosCategories = _categoryApiService.GetCategories(parenttId: 39);
            return new MenuDto
            {
                MainStoreProducts = GetMenuProducts(),
                MainStoreCategories = GetMenuCategories(),
                ArticlesAndVideos = GetMeuArticles(articlesAndVideosCategories),
                Nutrations = GetMeuArticles(nutrationArticlesCategories, true),
                Exercises = GetMeuArticles(exercisesArticlesCategories, true)
            };
        }

        private List<MenuCategoriesDto> GetMenuCategories()
        {
            IEnumerable<CategoryDto> allCategories = _categoryApiService.GetCategories().Select(a => new CategoryDto
            {
                Description = a.Description,
                Name = a.Name,
                SeName = _urlRecordService.GetSeName(a),
                Id = a.Id,
                ParentCategoryId = a.ParentCategoryId
            });
            IEnumerable<CategoryDto> allManufacturers = _manufacturerApiService.GetManufacturers(limit: 5).Select(a => new CategoryDto
            {
                Description = a.Description,
                Name = a.Name,
                SeName = _urlRecordService.GetSeName(a),
                Id = a.Id
            });

            List<CategoryDto> ingerdientCategories = allCategories.Where(a => a.ParentCategoryId == 24).ToList();
            ingerdientCategories.Add(new CategoryDto {Name = "All Ingredients", SeName = "all-ingredients", Id = 24});

            List<CategoryDto> goalCategories = allCategories.Where(a => a.ParentCategoryId == 26).ToList();
            goalCategories.Add(new CategoryDto {Name = "All Goals", SeName = "all-goals", Id = 26});

            List<CategoryDto> categoriesCategories = allCategories.Where(a => a.ParentCategoryId == 27).ToList();
            categoriesCategories.Add(new CategoryDto {Name = "All Categories", SeName = "all-categories", Id = 27});

            var mainStoreCategories = new List<MenuCategoriesDto>
            {
                new MenuCategoriesDto
                {
                    Id = 26,
                    MenuItemName = "Goals",
                    CategoriesFirstRow = goalCategories
                },
                new MenuCategoriesDto
                {
                    Id = 24,
                    MenuItemName = "Ingerdients",
                    CategoriesFirstRow = ingerdientCategories
                },
                new MenuCategoriesDto
                {
                    Id = 27,
                    MenuItemName = "Categories",
                    CategoriesFirstRow = categoriesCategories
                },
                new MenuCategoriesDto
                {
                    MenuItemName = "Brands",
                    CategoriesFirstRow = allManufacturers
                }
            };

            return mainStoreCategories;
        }

        private List<MenuProductsDto> GetMenuProducts()
        {
            IEnumerable<ProductDto> promotionsProducts = GetProducts(17);
            IEnumerable<ProductDto> top10Products = GetProducts(18);
            IEnumerable<ProductDto> justArriveProducts = GetProducts(19);
            IEnumerable<ProductDto> accessoriesProducts = GetProducts(20);
            IEnumerable<ProductDto> stackProducts = GetProducts(22);
            var mainStoreProducts = new List<MenuProductsDto>
            {
                new MenuProductsDto
                {
                    Id = 17,
                    MenuItemName = "Promotions",
                    ProductsFirstRow = promotionsProducts.Skip(0).Take(2),
                    ProductsSecondRow = promotionsProducts.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    Id = 18,
                    MenuItemName = "Top 10",
                    ProductsFirstRow = top10Products.Skip(0).Take(2),
                    ProductsSecondRow = top10Products.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    Id = 19,
                    MenuItemName = "Just Arrive",
                    ProductsFirstRow = justArriveProducts.Skip(0).Take(2),
                    ProductsSecondRow = justArriveProducts.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    Id = 22,
                    MenuItemName = "Stack",
                    ProductsFirstRow = stackProducts.Skip(0).Take(2),
                    ProductsSecondRow = stackProducts.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    Id = 20,
                    MenuItemName = "Accessories",
                    ProductsFirstRow = accessoriesProducts.Skip(0).Take(2),
                    ProductsSecondRow = accessoriesProducts.Skip(2).Take(3)
                }
            };
            return mainStoreProducts;
        }

        private List<MenuArticlesDto> GetMeuArticles(IEnumerable<Category> categories, bool oneRow = false)
        {
            var articles = new List<MenuArticlesDto>();
            foreach (Category category in categories)
            {
                IEnumerable<ArticlesDto> articlesDto = _articleApiService.GetArticles(categoryId: category.Id, limit: 5).Select(a => new ArticlesDto
                {
                    SeName = a.Title,
                    Id = a.Id,
                    Title = a.Title,
                    Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                    Body = a.Body
                });
                MenuArticlesDto dto;
                if (oneRow)
                    dto = new MenuArticlesDto
                    {
                        Id = category.Id,
                        MenuItemName = category.Name,
                        ArticlesFirstRow = articlesDto.ToList()
                    };
                else
                    dto = new MenuArticlesDto
                    {
                        Id = category.Id,
                        MenuItemName = category.Name,
                        ArticlesFirstRow = articlesDto.Skip(0).Take(2).ToList(),
                        ArticlesSecondRow = articlesDto.Skip(2).Take(3).ToList()
                    };

                articles.Add(dto);
            }

            return articles;
        }

        private IEnumerable<ProductDto> GetProducts(int categoryId, int limit = 5)
        {
            IEnumerable<ProductDto> products = _productApiService.GetProducts(categoryId: categoryId, limit: limit).Select(product => new ProductDto
            {
                ShortDescription = product.ShortDescription,
                Name = product.Name,
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
            return products;
        }
    }
}