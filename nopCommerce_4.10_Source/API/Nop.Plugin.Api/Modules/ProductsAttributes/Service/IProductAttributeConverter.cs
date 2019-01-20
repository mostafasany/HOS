﻿using System.Collections.Generic;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.ProductsAttributes.Service
{
    public interface IProductAttributeConverter
    {
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
        List<ProductItemAttributeDto> Parse(string attributesXml);
    }
}