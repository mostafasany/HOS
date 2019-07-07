using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Product.Modules.ProductAttributes.Dto
{
    [JsonObject(Title = "product_attribute")]
    //TODO:4.2 Migration
    // [Validator(typeof(ProductAttributeDtoValidator))]
    public class ProductAttributeDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        ///// <summary>
        ///// Gets or sets the localized names
        ///// </summary>
        //[JsonProperty("localized_names")]
        //public List<LocalizedNameDto> LocalizedNames
        //{
        //    get
        //    {
        //        return _localizedNames;
        //    }
        //    set
        //    {
        //        _localizedNames = value;
        //    }
        //}

        /// <summary>
        ///     Gets or sets the description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}