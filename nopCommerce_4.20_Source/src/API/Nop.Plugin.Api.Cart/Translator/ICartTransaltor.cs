using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Cart.Translator
{
    public interface ICartTransaltor
    {
        ProductDto PrepareProductDTO(Core.Domain.Catalog.Product product);
        ShippingOptionDto PrepareShippingOptionItemDTO(ShippingOption shoppingCartItem);
        ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem);
    }
}