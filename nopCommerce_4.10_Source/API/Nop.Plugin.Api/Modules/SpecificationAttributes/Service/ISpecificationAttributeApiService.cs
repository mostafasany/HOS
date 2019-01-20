﻿using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Modules.SpecificationAttributes.Service
{
    public interface ISpecificationAttributeApiService
    {
        IList<ProductSpecificationAttribute> GetProductSpecificationAttributes(int? productId = null, int? specificationAttributeOptionId = null, bool? allowFiltering = null, bool? showOnProductPage = null, int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);
        IList<SpecificationAttribute> GetSpecificationAttributes(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);
    }
}