using System.Collections.Generic;
using Nop.Core.Domain.Articles;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.Modules.Articles.Dto;
using Nop.Plugin.Api.Modules.Cart.Dto;
using Nop.Plugin.Api.Modules.Categories.Dto;
using Nop.Plugin.Api.Modules.Countries.Dto;
using Nop.Plugin.Api.Modules.Discounts.Dto;
using Nop.Plugin.Api.Modules.Languages.Dto;
using Nop.Plugin.Api.Modules.Manufacturers.Dto;
using Nop.Plugin.Api.Modules.Orders.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Orders.Dto.Orders;
using Nop.Plugin.Api.Modules.Products.Dto;
using Nop.Plugin.Api.Modules.ProductsAttributes.Dto;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Dto;
using Nop.Plugin.Api.Modules.Stores.Dto;
using Nop.Plugin.Api.Modules.Topics.Dto;

namespace Nop.Plugin.Api.Common.Helpers
{
    public interface IDTOHelper
    {
        ProductDto PrepareProductDTO(Product product);
        CategoryDto PrepareCategoryDTO(Category category);
        OrderDto PrepareOrderDTO(Order order);
        ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem);

        ExtendedShoppingCartDto PrepareExtendedShoppingCartItemDto(IEnumerable<ShoppingCartItem> shoppingCartItem);

        OrderItemDto PrepareOrderItemDTO(OrderItem orderItem);
        StoreDto PrepareStoreDTO(Store store);
        LanguageDto PrepateLanguageDto(Language language);
        ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
        ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
        SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute);

        TopicDto PrepateTopicDto(Topic topic);

        ManufacturerDto PrepateManufacturerDto(Manufacturer manufacturer);

        ArticlesDto PrepateArticleDto(Article article);

        ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup article);

        DiscountDto PrepateDiscountDto(Discount article);
        StateProvinceDto PrepateProvinceStateDto(StateProvince state);
        ShippingOptionDto PrepareShippingOptionItemDTO(ShippingOption shoppingCartItem);
    }
}