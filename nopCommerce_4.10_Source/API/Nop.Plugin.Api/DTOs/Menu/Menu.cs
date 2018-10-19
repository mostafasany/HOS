using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Articles;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Manufacturers;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.DTOs.Menu
{
    public class MenuDto : ISerializableObject
    {
        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("MainStoreProducts")]
        public List<MenuProductsDto> MainStoreProducts { get; set; }

        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("MainStoreCategories")]
        public List<MenuCategoriesDto> MainStoreCategories { get; set; }

        [JsonProperty("ArticlesAndVideos")]
        public List<MenuArticlesDto> ArticlesAndVideos { get; set; }


        [JsonProperty("Exercises")]
        public List<MenuArticlesDto> Exercises { get; set; }


        [JsonProperty("Nutrations")]
        public List<MenuArticlesDto> Nutrations { get; set; }

        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("MainStoreBrands")]
        public List<MenuCategoriesDto> MainStoreBrands { get; set; }

        public string GetPrimaryPropertyName() => "MenuDto";

        public Type GetPrimaryPropertyType() => typeof(List<MenuProductsDto>);
    }

    [JsonObject(Title = "menuCategories")]
    public class MenuCategoriesDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the menu Item  name
        /// </summary>
        [JsonProperty("MenuItemName")]
        public string MenuItemName { get; set; }

        /// <summary>
        ///     Gets or sets the menu Item  se name
        /// </summary>
        [JsonProperty("se_name")]
        public string SeName { get; set; }

        /// <summary>
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public IEnumerable<CategoryDto> CategoriesFirstRow { get; set; }
    }

    [JsonObject(Title = "menuBrands")]
    public class MenuBrandDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the menu Item  name
        /// </summary>
        [JsonProperty("MenuItemName")]
        public string MenuItemName { get; set; }

        /// <summary>
        ///     Gets or sets the menu Item  se name
        /// </summary>
        [JsonProperty("se_name")]
        public string SeName { get; set; }

        /// <summary>
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public IEnumerable<ManufacturerDto> CategoriesFirstRow { get; set; }
    }

    [JsonObject(Title = "menuArticles")]
    public class MenuArticlesDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the menu Item  name
        /// </summary>
        [JsonProperty("MenuItemName")]
        public string MenuItemName { get; set; }

        /// <summary>
        ///     Gets or sets the menu Item  se name
        /// </summary>
        [JsonProperty("se_name")]
        public string SeName { get; set; }

        /// <summary>
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public List<ArticlesDto> ArticlesFirstRow { get; set; }

        [JsonProperty("MenuItemsSecondRow")]
        public List<ArticlesDto> ArticlesSecondRow { get; set; }
    }

    [JsonObject(Title = "menuProducts")]
    public class MenuProductsDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the menu Item  name
        /// </summary>
        [JsonProperty("MenuItemName")]
        public string MenuItemName { get; set; }

        /// <summary>
        ///     Gets or sets the menu Item  se name
        /// </summary>
        [JsonProperty("se_name")]
        public string SeName { get; set; }


        /// <summary>
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public IEnumerable<ProductDto> ProductsFirstRow { get; set; }

        /// <summary>
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsSecondRow")]
        public IEnumerable<ProductDto> ProductsSecondRow { get; set; }
    }
}