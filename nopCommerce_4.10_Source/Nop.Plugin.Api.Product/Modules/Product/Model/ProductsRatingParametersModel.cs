using Newtonsoft.Json;

namespace Nop.Plugin.Api.Product.Modules.Product.Model
{
    [JsonObject(Title = "rating")]
    //[Validator(typeof(ProductDtoValidator))]
    //[ModelBinder(typeof(ParametersModelBinder<ProductsRatingParametersModel>))]
    public class ProductsRatingParametersModel
    {
      
        [JsonProperty("store_id")]
        public int  StoreId { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("review_text")]
        public string ReviewText { get; set; }
    }


}