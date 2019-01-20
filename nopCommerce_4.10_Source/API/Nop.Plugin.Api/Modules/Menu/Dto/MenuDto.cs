using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Menu.Dto
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
}