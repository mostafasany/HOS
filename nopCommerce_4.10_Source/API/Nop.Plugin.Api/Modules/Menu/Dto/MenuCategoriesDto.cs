using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.DTOs.Categories;

namespace Nop.Plugin.Api.DTOs.Menu
{
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
}