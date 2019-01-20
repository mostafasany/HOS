using System.Collections.Generic;
using Nop.Plugin.Api.DTOs;

namespace Nop.Plugin.Api.Services
{
    public interface IProductAttributeConverter
    {
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
        List<ProductItemAttributeDto> Parse(string attributesXml);
    }
}