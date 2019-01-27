using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Modules.SpecificationAttributes.Translator
{
    public interface ISpecificationAttributesTransaltor
    {
        SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute);
    }
}