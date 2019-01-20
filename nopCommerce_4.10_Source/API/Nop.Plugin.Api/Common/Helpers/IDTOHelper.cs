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
        CategoryDto PrepareCategoryDTO(Category category);

        ExtendedShoppingCartDto PrepareExtendedShoppingCartItemDto(IEnumerable<ShoppingCartItem> shoppingCartItem);
        OrderDto PrepareOrderDTO(Order order);

        OrderItemDto PrepareOrderItemDTO(OrderItem orderItem);
        ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
        ProductDto PrepareProductDTO(Product product);
        ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
        ShippingOptionDto PrepareShippingOptionItemDTO(ShippingOption shoppingCartItem);
        ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem);
        SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute);
        StoreDto PrepareStoreDTO(Store store);

        ArticlesDto PrepateArticleDto(Article article);

        ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup article);

        DiscountDto PrepateDiscountDto(Discount article);
        LanguageDto PrepateLanguageDto(Language language);

        ManufacturerDto PrepateManufacturerDto(Manufacturer manufacturer);
        StateProvinceDto PrepateProvinceStateDto(StateProvince state);

        TopicDto PrepateTopicDto(Topic topic);
    }
}