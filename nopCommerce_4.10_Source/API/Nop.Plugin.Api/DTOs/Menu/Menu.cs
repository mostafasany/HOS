using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

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

        [JsonProperty("ArticlesAndVideo")]
        public List<MenuArticlesDto> ArticlesAndVideos { get; set; }


        [JsonProperty("Nutrations")]
        public List<MenuArticlesDto> Nutrations { get; set; }


        [JsonProperty("Trainings")]
        public List<MenuArticlesDto> Trainings { get; set; }

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
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public IEnumerable<MenuCategoryDto> CategoriesFirstRow { get; set; }
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
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public List<MenuArticleDto> ArticlesFirstRow { get; set; }

        [JsonProperty("MenuItemsSecondRow")]
        public List<MenuArticleDto> ArticlesSecondRow { get; set; }
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
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsFirstRow")]
        public IEnumerable<MenuProductDto> ProductsFirstRow { get; set; }

        /// <summary>
        ///     Gets or sets the menuItems First Row
        /// </summary>
        [JsonProperty("MenuItemsSecondRow")]
        public IEnumerable<MenuProductDto> ProductsSecondRow { get; set; }
    }


    [JsonObject(Title = "menuCategory")]
    public class MenuCategoryDto : BaseDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("ParentCategoryId")]
        public int ParentCategoryId { get; set; }
    }

    [JsonObject(Title = "menuArticle")]
    public class MenuArticleDto : BaseDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }
    }


    [JsonObject(Title = "menuProduct")]
    public class MenuProductDto : BaseDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("Discount")]
        public string Discount { get; set; }

        [JsonProperty("Image")]
        public string Image { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }
    }
}