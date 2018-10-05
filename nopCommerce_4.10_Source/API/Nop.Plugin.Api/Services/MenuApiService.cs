using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.DTOs.Menu;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Media;

namespace Nop.Plugin.Api.Services
{
    public class MenuApiService : IMenuApiService
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IPictureService _pictureService;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IProductApiService _productApiService;

        public MenuApiService(IProductApiService productApiService, ICategoryApiService categoryApiService, IDTOHelper dtoHelper,
            IPictureService pictureService,IManufacturerApiService manufacturerApiService)
        {
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
            _dtoHelper = dtoHelper;
            _pictureService = pictureService;
            _manufacturerApiService = manufacturerApiService;
        }

        public MenuDto GetMenu()
        {
            IEnumerable<MenuProductDto> promotionsProducts = _productApiService.GetProducts(categoryId: 17, limit: 5).Select(product => new MenuProductDto
            {
                Description = product.ShortDescription,
                Name = product.Name,
                Price = product.Price,
                //Discount = product.Discount,
                Url = product.Name,
                Image = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()?.Picture),
                Id = product.Id
            });
            IEnumerable<MenuProductDto> top10Products = _productApiService.GetProducts(categoryId: 18, limit: 5).Select(product => new MenuProductDto
            {
                Description = product.ShortDescription,
                Name = product.Name,
                Price = product.Price,
                //Discount = product.Discount,
                Url = product.Name,
                Image = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()?.Picture),
                Id = product.Id
            });
            IEnumerable<MenuProductDto> justArriveProducts = _productApiService.GetProducts(categoryId: 19, limit: 5).Select(product => new MenuProductDto
            {
                Description = product.ShortDescription,
                Name = product.Name,
                Price = product.Price,
                //Discount = product.Discount,
                Url = product.Name,
                Image = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()?.Picture),
                Id = product.Id
            });
            IEnumerable<MenuProductDto> accessoriesProducts = _productApiService.GetProducts(categoryId: 20, limit: 5).Select(product => new MenuProductDto
            {
                Description = product.ShortDescription,
                Name = product.Name,
                Price = product.Price,
                //Discount = product.Discount,
                Url = product.Name,
                Image = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()?.Picture),
                Id = product.Id
            });
            IEnumerable<MenuProductDto> stackProducts = _productApiService.GetProducts(categoryId: 22, limit: 5).Select(product => new MenuProductDto
            {
                Description = product.ShortDescription,
                Name = product.Name,
                Price = product.Price,
                //Discount = product.Discount,
                Url = product.Name,
                Image = _pictureService.GetPictureUrl(product.ProductPictures?.FirstOrDefault()?.Picture),
                Id = product.Id
            });
            IEnumerable<MenuCategoryDto> allCategories = _categoryApiService.GetCategories().Select(a => new MenuCategoryDto
            {
                Description = a.Description,
                Name = a.Name,
                Url = a.Name,
                Id = a.Id,
                ParentCategoryId = a.ParentCategoryId
            });

            IEnumerable<MenuCategoryDto> allManufacturers = _manufacturerApiService.GetManufacturers(limit:5).Select(a => new MenuCategoryDto
            {
                Description = a.Description,
                Name = a.Name,
                Url = a.Name,
                Id = a.Id,
            });

            IEnumerable<MenuCategoryDto> ingerdientCategories = allCategories.Where(a => a.ParentCategoryId == 24);
            IEnumerable<MenuCategoryDto> goalCategories = allCategories.Where(a => a.ParentCategoryId == 26);
            IEnumerable<MenuCategoryDto> categoriesCategories = allCategories.Where(a => a.ParentCategoryId == 27);
            var mainStoreProducts = new List<MenuProductsDto>
            {
                new MenuProductsDto
                {
                    MenuItemName = "Promotions",
                    ProductsFirstRow = promotionsProducts.Skip(0).Take(2),
                    ProductsSecondRow = promotionsProducts.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    MenuItemName = "Top 10",
                    ProductsFirstRow = top10Products.Skip(0).Take(2),
                    ProductsSecondRow = top10Products.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    MenuItemName = "Just Arrive",
                    ProductsFirstRow = justArriveProducts.Skip(0).Take(2),
                    ProductsSecondRow = justArriveProducts.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    MenuItemName = "Stack",
                    ProductsFirstRow = stackProducts.Skip(0).Take(2),
                    ProductsSecondRow = stackProducts.Skip(2).Take(3)
                },
                new MenuProductsDto
                {
                    MenuItemName = "Accessories",
                    ProductsFirstRow = accessoriesProducts.Skip(0).Take(2),
                    ProductsSecondRow = accessoriesProducts.Skip(2).Take(3)
                }
            };

            var mainStoreCategories = new List<MenuCategoriesDto>
            {
                new MenuCategoriesDto
                {
                    MenuItemName = "Goals",
                    CategoriesFirstRow = goalCategories
                },
                new MenuCategoriesDto
                {
                    MenuItemName = "Ingerdients",
                    CategoriesFirstRow = ingerdientCategories
                },
                new MenuCategoriesDto
                {
                    MenuItemName = "Categories",
                    CategoriesFirstRow = categoriesCategories
                }
                ,
                new MenuCategoriesDto
                {
                    MenuItemName = "Brands",
                    CategoriesFirstRow = allManufacturers
                }
            };
            return new MenuDto
            {
                MainStoreProducts = mainStoreProducts,
                MainStoreCategories = mainStoreCategories,
                ArticlesAndVideos = GetArticlesAndVideos(),
                Nutrations = GetArticlesAndVideos(),
                Trainings = GetArticlesAndVideos()
            };
        }

        private List<MenuArticlesDto> GetArticlesAndVideos()
        {
            List<MenuArticlesDto> articles = new List<MenuArticlesDto>
            {
                new MenuArticlesDto
                {
                    MenuItemName = "Workout",
                    ArticlesFirstRow = new List<MenuArticleDto>
                    {
                        new MenuArticleDto {Id = 1, Name = "Workout 1", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Workout 2", Url = ""},
                    },
                    ArticlesSecondRow = new List<MenuArticleDto>
                    {
                        new MenuArticleDto {Id = 1, Name = "Workout 3", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Workout 4", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Workout 5", Url = ""}
                    }
                },
                new MenuArticlesDto
                {
                    MenuItemName = "Motivation",
                    ArticlesFirstRow = new List<MenuArticleDto>
                    {
                        new MenuArticleDto {Id = 1, Name = "Motivation 1", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Motivation 2", Url = ""},
                    },
                    ArticlesSecondRow = new List<MenuArticleDto>
                    {
                        new MenuArticleDto {Id = 1, Name = "Motivation 3", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Motivation 4", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Motivation 5", Url = ""}
                    }
                },
                new MenuArticlesDto
                {
                    MenuItemName = "Supplementation",
                    ArticlesFirstRow = new List<MenuArticleDto>
                    {
                        new MenuArticleDto {Id = 1, Name = "Supplementation 1", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Supplementation 2", Url = ""},
                    },
                    ArticlesSecondRow = new List<MenuArticleDto>
                    {
                        new MenuArticleDto {Id = 1, Name = "Supplementation 3", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Supplementation 4", Url = ""},
                        new MenuArticleDto {Id = 1, Name = "Motivation 5", Url = ""}
                    }
                }

            };

            return articles;
        }
    }
}