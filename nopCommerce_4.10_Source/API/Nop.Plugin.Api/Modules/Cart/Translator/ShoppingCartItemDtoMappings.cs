using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Cart.Dto;

namespace Nop.Plugin.Api.Modules.Cart.Translator
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem) => shoppingCartItem.MapTo<ShoppingCartItem, ShoppingCartItemDto>();
    }
}