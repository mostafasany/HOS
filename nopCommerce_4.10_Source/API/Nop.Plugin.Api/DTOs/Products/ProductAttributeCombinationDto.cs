using AutoMapper;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Products
{
   public class ProductAttributeCombinationDto
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("productId")]
        public int ProductId { get; set; }
      
        
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("productAttributId")]
        public int ProductAttributId { get; set; }


        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        [JsonProperty("attributesXml")]
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        [JsonProperty("stockQuantity")]
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow orders when out of stock
        /// </summary>
        [JsonProperty("allowOutOfStockOrders")]
        public bool AllowOutOfStockOrders { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        [JsonProperty("sku")]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer part number
        /// </summary>
        [JsonProperty("manufacturerPartNumber")]
        public string ManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
        /// </summary>
        [JsonProperty("gtin")]
        public string Gtin { get; set; }

        /// <summary>
        /// Gets or sets the attribute combination price. This way a store owner can override the default product price when this attribute combination is added to the cart. For example, you can give a discount this way.
        /// </summary>
        [JsonProperty("overriddenPrice")]
        public decimal? OverriddenPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity when admin should be notified
        /// </summary>
        [JsonProperty("notifyAdminForQuantityBelow")]
        public int NotifyAdminForQuantityBelow { get; set; }

        /// <summary>
        /// Gets or sets the identifier of picture associated with this combination
        /// </summary>
        [JsonProperty("pictureId")]
        public int PictureId { get; set; }

    }
}
