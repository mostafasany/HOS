using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Product.Modules.Product.Dto
{
    public class ProductsReviewRootObjectDto : ISerializableObject
    {
        public ProductsReviewRootObjectDto()
        {
            ProductsReview = new List<ProductReviewDto>();
        }

        [JsonProperty("products_review")] public IList<ProductReviewDto> ProductsReview { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "products_review";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ProductReviewDto);
        }
    }
}