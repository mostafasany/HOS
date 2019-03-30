using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.SpecificationAttributes.Translator
{
    public class SpecificationAttributesTransaltor : ISpecificationAttributesTransaltor
    {
        public SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute) => specificationAttribute.ToDto();
    }
}