using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Modules.Cart.Dto;

namespace Nop.Plugin.Api.Modules.Cart.Translator
{
    public interface ICartTransaltor
    {
        ProductDto PrepareProductDTO(Core.Domain.Catalog.Product product);
        ShippingOptionDto PrepareShippingOptionItemDTO(ShippingOption shoppingCartItem);
        ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem);
    }
}