using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Common.DTOs.Product
{
    [JsonObject(Title = "extended_attribute")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductExtendedAttributeDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }


        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("extended_attribute_values")]
        public List<ProductExtendedAttributeValueDto> Values { get; set; }
    }

    [JsonObject(Title = "extended_attribute_value")]
    //[Validator(typeof(ProductDtoValidator))]
    public class ProductExtendedAttributeValueDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }


        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("stockQty")]
        public int? StockQty { get; set; }


        /// <summary>
        ///     Gets or sets the product attribute name
        /// </summary>
        [JsonProperty("pictureId")]
        public int? PictureId { get; set; }

    }
}