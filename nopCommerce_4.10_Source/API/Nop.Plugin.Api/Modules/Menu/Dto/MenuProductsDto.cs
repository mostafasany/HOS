using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.DTOs.Menu
{
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