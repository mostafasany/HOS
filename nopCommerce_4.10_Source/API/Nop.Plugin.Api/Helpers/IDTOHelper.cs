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
using Nop.Plugin.Api.DTOs.Articles;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Countries;
using Nop.Plugin.Api.DTOs.Discounts;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.DTOs.Manufacturers;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.ProductAttributes;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.DTOs.SpecificationAttributes;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.DTOs.Topics;

namespace Nop.Plugin.Api.Helpers
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

        ArticlesDto PrepateArticleDto(FNS_Article article);

        ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup article);

        DiscountDto PrepateDiscountDto(Discount article);
        StateProvinceDto PrepateProvinceStateDto(StateProvince state);
        ShippingOptionDto PrepareShippingOptionItemDTO(ShippingOption shoppingCartItem);
    }
}