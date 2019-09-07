using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Menu.Dto
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

        [JsonProperty("ArticlesAndVideos")] public List<MenuArticlesDto> ArticlesAndVideos { get; set; }


        [JsonProperty("Exercises")] public List<MenuArticlesDto> Exercises { get; set; }


        [JsonProperty("Nutrations")] public List<MenuArticlesDto> Nutrations { get; set; }

        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("MainStoreBrands")]
        public List<MenuCategoriesDto> MainStoreBrands { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "MenuDto";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(List<MenuProductsDto>);
        }
    }


    public class MenuRootDto : ISerializableObject
    {
        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Menu")]
        public List<MenuDto2> Menu { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "Menu";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(List<MenuDto2>);
        }
    }


    public class MenuDto2 : ISerializableObject
    {
        public MenuDto2()
        {
            MenuTree = new List<MenuDto2>();
        }
        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }


        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }


        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("SeName")]
        public string SeName { get; set; }

        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("MenuTree")]
        public List<MenuDto2> MenuTree { get; set; }


        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("MenuTreeItems")]
        public List<MenuItemDto> MenuItems { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "MenuDto";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(List<MenuDto2>);
        }
    }


    public class MenuItemDto : ISerializableObject
    {
        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }


        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }



        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Image")]
        public string Image { get; set; }

        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("Description")]
        public string Description { get; set; }


        /// <summary>
        ///     Gets or sets the Main Store Products
        /// </summary>
        [JsonProperty("SeName")]
        public string SeName { get; set; }

      

        public string GetPrimaryPropertyName()
        {
            return "MenuItemDto";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(MenuItemDto);
        }
    }
}