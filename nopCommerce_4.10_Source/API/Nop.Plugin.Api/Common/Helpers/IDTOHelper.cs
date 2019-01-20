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
using Nop.Plugin.Api.Modules.Article.Dto;
using Nop.Plugin.Api.Modules.Cart.Dto;
using Nop.Plugin.Api.Modules.Category.Dto;
using Nop.Plugin.Api.Modules.Country.Dto;
using Nop.Plugin.Api.Modules.Discount.Dto;
using Nop.Plugin.Api.Modules.Language.Dto;
using Nop.Plugin.Api.Modules.Manufacturer.Dto;
using Nop.Plugin.Api.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Order.Dto.Orders;
using Nop.Plugin.Api.Modules.Product.Dto;
using Nop.Plugin.Api.Modules.ProductAttributes.Dto;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Dto;
using Nop.Plugin.Api.Modules.Store.Dto;
using Nop.Plugin.Api.Modules.Topic.Dto;

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