using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;
using Nop.Plugin.Api.Modules.Manufacturers.Dto;

namespace Nop.Plugin.Api.Modules.Menu.Dto
{
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
}