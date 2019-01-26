using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Service
{
    public class ManufacturerApiService : IManufacturerApiService
    {
        private readonly IRepository<Core.Domain.Catalog.Manufacturer> _manufacturerRepository;
        private readonly IStoreMappingService _storeMappingService;

        public ManufacturerApiService(IRepository<Core.Domain.Catalog.Manufacturer> topicRepository,
            IStoreMappingService storeMappingService)
        {
            _manufacturerRepository = topicRepository;
            _storeMappingService = storeMappingService;
        }

        public IList<Core.Domain.Catalog.Manufacturer> GetManufacturers(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null)
        {
            IQueryable<Core.Domain.Catalog.Manufacturer> query = GetManufacturersQuery(publishedStatus, ids);


            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Core.Domain.Catalog.Manufacturer>(query, page - 1, limit);
        }

        public Core.Domain.Catalog.Manufacturer GetManufacturerById(int id)
        {
            if (id <= 0)
                return null;

            Core.Domain.Catalog.Manufacturer category = _manufacturerRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public int GetManufacturersCount(
            bool? publishedStatus = null)
        {
            IQueryable<Core.Domain.Catalog.Manufacturer> query = GetManufacturersQuery(publishedStatus);

            return query.Count(c => _storeMappingService.Authorize(c));
        }

        private IQueryable<Core.Domain.Catalog.Manufacturer> GetManufacturersQuery(bool? publishedStatus = null, IList<int> ids = null)
        {
            IQueryable<Core.Domain.Catalog.Manufacturer> query = _manufacturerRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.OrderByDescending(category => category.DisplayOrder);

            return query;
        }
    }
}