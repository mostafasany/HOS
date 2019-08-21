﻿using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Service
{
    public interface IManufacturerApiService
    {
        Core.Domain.Catalog.Manufacturer GetManufacturerById(int manufacturerId);

        IEnumerable<Core.Domain.Catalog.Manufacturer> GetManufacturers(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null);

        int GetManufacturersCount(
            bool? publishedStatus = null);
    }
}