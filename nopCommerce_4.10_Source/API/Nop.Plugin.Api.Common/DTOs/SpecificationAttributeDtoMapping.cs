using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Common.DTOs
{
    public static class SpecificationAttributeDtoMappings
    {
        public static ProductSpecificationAttributeDto ToDto(this ProductSpecificationAttribute productSpecificationAttribute) => productSpecificationAttribute.MapTo<ProductSpecificationAttribute, ProductSpecificationAttributeDto>();

        public static SpecificationAttributeDto ToDto(this SpecificationAttribute specificationAttribute) => specificationAttribute.MapTo<SpecificationAttribute, SpecificationAttributeDto>();

        public static SpecificationAttributeOptionDto ToDto(this SpecificationAttributeOption specificationAttributeOption) => specificationAttributeOption.MapTo<SpecificationAttributeOption, SpecificationAttributeOptionDto>();
    }
}