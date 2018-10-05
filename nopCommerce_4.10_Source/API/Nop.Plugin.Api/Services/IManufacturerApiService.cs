﻿using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface IManufacturerApiService
    {
        Manufacturer GetManufacturerById(int manufacturerId);

        IList<Manufacturer> GetManufacturers(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null);

        int GetManufacturersCount(
            bool? publishedStatus = null);
    }
}