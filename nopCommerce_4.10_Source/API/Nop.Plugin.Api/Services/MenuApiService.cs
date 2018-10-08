using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.DTOs.Menu;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Media;

namespace Nop.Plugin.Api.Services
{
    public class MenuApiService : IMenuApiService
    {
        private readonly IArticleApiService _articleApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IPictureService _pictureService;
        private readonly IProductApiService _productApiService;

        public MenuApiService(IProductApiService productApiService, ICategoryApiService categoryApiService, IDTOHelper dtoHelper,
            IPictureService pictureService, IManufacturerApiService manufacturerApiService, IArticleApiService articleApiService)
        {
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
            _dtoHelper = dtoHelper;
            _pictureService = pictureService;
            _manufacturerApiService = manufacturerApiService;
            _articleApiService = articleApiService;
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
            IEnumerable<MenuCategoryDto> allManufacturers = _manufacturerApiService.GetManufacturers(limit: 5).Select(a => new MenuCategoryDto
            {
                Description = a.Description,
                Name = a.Name,
                Url = a.Name,
                Id = a.Id
            });

            List<MenuCategoryDto> ingerdientCategories = allCategories.Where(a => a.ParentCategoryId == 24).ToList();
            ingerdientCategories.Add(new MenuCategoryDto { Name = "All Ingredients", Url = "All" });

            List<MenuCategoryDto> goalCategories = allCategories.Where(a => a.ParentCategoryId == 26).ToList();
            goalCategories.Add(new MenuCategoryDto { Name = "All Goals", Url = "All" });

            List<MenuCategoryDto> categoriesCategories = allCategories.Where(a => a.ParentCategoryId == 27).ToList();
            categoriesCategories.Add(new MenuCategoryDto { Name = "All Categories", Url = "All" });

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
                },
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
                Nutrations = GetNutrationsArticles(),
                Trainings = GetExercisesArticles()
            };
        }
        private List<MenuArticlesDto> GetExercisesArticles()
        {
            IEnumerable<MenuArticleDto> upperBodyExercises = _articleApiService.GetArticles(groupId: 4, limit: 5).Select(a => new MenuArticleDto
            {
                Url = "",
                Id = a.Id,
                Name = a.Title,
                Image = _pictureService.GetPictureUrl(a.PictureId),
                Description = a.MetaDescription
            });
            IEnumerable<MenuArticleDto> lowerBodyExercises = _articleApiService.GetArticles(groupId: 5, limit: 5).Select(a => new MenuArticleDto
            {
                Url = "",
                Id = a.Id,
                Name = a.Title,
                Image = _pictureService.GetPictureUrl(a.PictureId),
                Description = a.MetaDescription
            });
            var articles = new List<MenuArticlesDto>
            {
                new MenuArticlesDto
                {
                    MenuItemName = "Upper Body Exercises",
                    ArticlesFirstRow = upperBodyExercises.Skip(0).Take(2).ToList(),
                    ArticlesSecondRow = upperBodyExercises.Skip(2).Take(3).ToList()
                },
                new MenuArticlesDto
                {
                    MenuItemName = "Lower Body Exercises",
                    ArticlesFirstRow = lowerBodyExercises.Skip(0).Take(2).ToList(),
                    ArticlesSecondRow = lowerBodyExercises.Skip(2).Take(3).ToList()
                }
            };

            return articles;
        }

        private List<MenuArticlesDto> GetNutrationsArticles()
        {
            IEnumerable<MenuArticleDto> keto = _articleApiService.GetArticles(groupId: 6, limit: 5).Select(a => new MenuArticleDto
            {
                Url = "",
                Id = a.Id,
                Name = a.Title,
                Image = _pictureService.GetPictureUrl(a.PictureId),
                Description = a.MetaDescription
            });
            IEnumerable<MenuArticleDto> chemical = _articleApiService.GetArticles(groupId: 7, limit: 5).Select(a => new MenuArticleDto
            {
                Url = "",
                Id = a.Id,
                Name = a.Title,
                Image = _pictureService.GetPictureUrl(a.PictureId),
                Description = a.MetaDescription
            });
            var articles = new List<MenuArticlesDto>
            {
                new MenuArticlesDto
                {
                    MenuItemName = "Keto",
                    ArticlesFirstRow = keto.Skip(0).Take(2).ToList(),
                    ArticlesSecondRow = keto.Skip(2).Take(3).ToList()
                },
                new MenuArticlesDto
                {
                    MenuItemName = "Chemical",
                    ArticlesFirstRow = chemical.Skip(0).Take(2).ToList(),
                    ArticlesSecondRow = chemical.Skip(2).Take(3).ToList()
                }
            };

            return articles;
        }

        private List<MenuArticlesDto> GetArticlesAndVideos()
        {
            IEnumerable<MenuArticleDto> health = _articleApiService.GetArticles(groupId: 8, limit: 5).Select(a => new MenuArticleDto
            {
                Url = "",
                Id = a.Id,
                Name = a.Title,
                Image = _pictureService.GetPictureUrl(a.PictureId),
                Description = a.MetaDescription
            });
            IEnumerable<MenuArticleDto> strength = _articleApiService.GetArticles(groupId: 9, limit: 5).Select(a => new MenuArticleDto
            {
                Url = "",
                Id = a.Id,
                Name = a.Title,
                Image = _pictureService.GetPictureUrl(a.PictureId),
                Description = a.MetaDescription
            });
            var articles = new List<MenuArticlesDto>
            {
                new MenuArticlesDto
                {
                    MenuItemName = "Health",
                    ArticlesFirstRow = health.Skip(0).Take(2).ToList(),
                    ArticlesSecondRow = health.Skip(2).Take(3).ToList()
                },
                new MenuArticlesDto
                {
                    MenuItemName = "Strength",
                    ArticlesFirstRow = strength.Skip(0).Take(2).ToList(),
                    ArticlesSecondRow = strength.Skip(2).Take(3).ToList()
                }
            };

            return articles;
        }
    }
}