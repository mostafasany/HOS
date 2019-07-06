using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Product.Modules.ProductCategoryMappings.Dto
{
    [JsonObject(Title = "product_category_mapping")]
    //TODO:4.2 Migration
    //[Validator(typeof(ProductCategoryMappingDtoValidator))]
    public class ProductCategoryMappingDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        ///     Gets or sets the category identifier
        /// </summary>
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the product is featured
        /// </summary>
        [JsonProperty("is_featured_product")]
        public bool? IsFeaturedProduct { get; set; }

        /// <summary>
        ///     Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }
    }
}