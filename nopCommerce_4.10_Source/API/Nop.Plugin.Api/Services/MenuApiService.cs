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
            IEnumerable<ProductDto> promotionsProducts = _productApiService.GetProducts(categoryId: 17, limit: 5).Select(product => new ProductDto
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
            IEnumerable<ProductDto> top10Products = _productApiService.GetProducts(categoryId: 18, limit: 5).Select(product => new ProductDto
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
            IEnumerable<ProductDto> justArriveProducts = _productApiService.GetProducts(categoryId: 19, limit: 5).Select(product => new ProductDto
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
            IEnumerable<ProductDto> accessoriesProducts = _productApiService.GetProducts(categoryId: 20, limit: 5).Select(product => new ProductDto
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
            IEnumerable<ProductDto> stackProducts = _productApiService.GetProducts(categoryId: 22, limit: 5).Select(product => new ProductDto
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

            IList<Category> exercisesArticlesCategories = _categoryApiService.GetCategories(parenttId: 37);
            IList<Category> nutrationArticlesCategories = _categoryApiService.GetCategories(parenttId: 38);
            IList<Category> articlesAndVideosCategories = _categoryApiService.GetCategories(parenttId: 39);
            return new MenuDto
            {
                MainStoreProducts = mainStoreProducts,
                MainStoreCategories = mainStoreCategories,
                ArticlesAndVideos = GetArticles(articlesAndVideosCategories),
                Nutrations = GetArticles(nutrationArticlesCategories, true),
                Exercises = GetArticles(exercisesArticlesCategories, true)
            };
        }

        private List<MenuArticlesDto> GetArticles(IEnumerable<Category> categories, bool oneRow = false)
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
                        MenuItemName = category.Name,
                        ArticlesFirstRow = articlesDto.ToList()
                    };
                else
                    dto = new MenuArticlesDto
                    {
                        MenuItemName = category.Name,
                        ArticlesFirstRow = articlesDto.Skip(0).Take(2).ToList(),
                        ArticlesSecondRow = articlesDto.Skip(2).Take(3).ToList()
                    };

                articles.Add(dto);
            }

            return articles;
        }

        private List<MenuArticlesDto> GetArticlesAndVideos(IEnumerable<Category> category)
        {
            IEnumerable<ArticlesDto> health = _articleApiService.GetArticles(categoryId: 46, limit: 5).Select(a => new ArticlesDto
            {
                SeName = a.Title,
                Id = a.Id,
                Title = a.Title,
                Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                Body = a.Body
            });
            IEnumerable<ArticlesDto> strength = _articleApiService.GetArticles(categoryId: 47, limit: 5).Select(a => new ArticlesDto
            {
                SeName = a.Title,
                Id = a.Id,
                Title = a.Title,
                Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                Body = a.MetaDescription
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

        private List<MenuArticlesDto> GetExercisesArticles(IEnumerable<Category> category)
        {
            IEnumerable<ArticlesDto> upperBodyExercises = _articleApiService.GetArticles(categoryId: 42, limit: 5).Select(a => new ArticlesDto
            {
                SeName = a.Title,
                Id = a.Id,
                Title = a.Title,
                Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                Body = a.MetaDescription
            });
            IEnumerable<ArticlesDto> lowerBodyExercises = _articleApiService.GetArticles(categoryId: 43, limit: 5).Select(a => new ArticlesDto
            {
                SeName = a.Title,
                Id = a.Id,
                Title = a.Title,
                Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                Body = a.MetaDescription
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

        private List<MenuArticlesDto> GetNutrationsArticles(IEnumerable<Category> category)
        {
            IEnumerable<ArticlesDto> keto = _articleApiService.GetArticles(categoryId: 44, limit: 5).Select(a => new ArticlesDto
            {
                SeName = a.Title,
                Id = a.Id,
                Title = a.Title,
                Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                Body = a.MetaDescription
            });
            IEnumerable<ArticlesDto> chemical = _articleApiService.GetArticles(categoryId: 45, limit: 5).Select(a => new ArticlesDto
            {
                SeName = a.Title,
                Id = a.Id,
                Title = a.Title,
                Image = new ImageDto {Src = _pictureService.GetPictureUrl(a.PictureId)},
                Body = a.MetaDescription
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
    }
}