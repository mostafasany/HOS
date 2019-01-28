using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Common.AutoMapper;

namespace Nop.Plugin.Api.Cart.Translator
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem) => shoppingCartItem.MapTo<ShoppingCartItem, ShoppingCartItemDto>();
    }
}