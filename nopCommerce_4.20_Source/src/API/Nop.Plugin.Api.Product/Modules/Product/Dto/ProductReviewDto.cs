using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.Product.Dto
{
    public class ProductReviewDto : ISerializableObject
    {
        public ProductReviewDto()
        {
            Product = new ProductDto();
        }

        [JsonProperty("product")] public ProductDto Product { get; set; }

        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the review text
        /// </summary>
        [JsonProperty("review_text")]
        public string ReviewText { get; set; }

        /// <summary>
        ///     Gets or sets the reply text
        /// </summary>
        [JsonProperty("reply_text")]
        public string ReplyText { get; set; }


        /// <summary>
        ///     Review rating
        /// </summary>
        [JsonProperty("rating")]
        public int Rating { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        [JsonProperty("created_on")]
        public DateTime CreatedOnUtc { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ProductDto);
        }
    }
}