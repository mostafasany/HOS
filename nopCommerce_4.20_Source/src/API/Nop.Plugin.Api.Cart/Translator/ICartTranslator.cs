using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Cart.Translator
{
    public interface ICartTranslator
    {
        ProductDto PrepareProductDto(Product product);
        ShippingOptionDto PrepareShippingOptionItemDto(ShippingOption shoppingCartItem);
        ShoppingCartItemDto PrepareShoppingCartItemDto(ShoppingCartItem shoppingCartItem);
    }
}