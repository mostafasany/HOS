using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Cart.Translator
{
    public class CartTranslator : ICartTranslator
    {
        private readonly IProductAttributeConverter _productAttributeConverter;

        public CartTranslator(
            IProductAttributeConverter productAttributeConverter)
        {
            _productAttributeConverter = productAttributeConverter;
        }

        public ShippingOptionDto PrepareShippingOptionItemDto(ShippingOption shippingOption)
        {
            var options = new ShippingOptionDto
            {
                Description = shippingOption.Description,
                Name = shippingOption.Name,
                Rate = shippingOption.Rate,
                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
            };
            try
            {
                var fromToDays = shippingOption.Description.Split(':');
                if (fromToDays.Any())
                {
                    options.FromDays = int.Parse(fromToDays.First());
                    options.ToDays = int.Parse(fromToDays.Last());
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return options;
        }


        public ShoppingCartItemDto PrepareShoppingCartItemDto(ShoppingCartItem shoppingCartItem)
        {
            var dto = shoppingCartItem.ToDto();
            dto.ProductDto = shoppingCartItem.Product.ToDto();
            dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
            return dto;
        }
    }
}