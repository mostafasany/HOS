using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Modules.Menu.Dto
{
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

        [JsonProperty("MenuItemsThirdRow")]
        public List<ArticlesDto> ArticlesThirdRow { get; set; }

        
    }
}