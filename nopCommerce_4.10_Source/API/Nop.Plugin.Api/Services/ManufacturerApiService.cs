using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Services
{
    public class ManufacturerApiService : IManufacturerApiService
    {
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IStoreMappingService _storeMappingService;

        public ManufacturerApiService(IRepository<Manufacturer> topicRepository,
            IStoreMappingService storeMappingService)
        {
            _manufacturerRepository = topicRepository;
            _storeMappingService = storeMappingService;
        }

        public IList<Manufacturer> GetManufacturers(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null)
        {
            IQueryable<Manufacturer> query = GetManufacturersQuery(publishedStatus, ids);


            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Manufacturer>(query, page - 1, limit);
        }

        public Manufacturer GetManufacturerById(int id)
        {
            if (id <= 0)
                return null;

            Manufacturer category = _manufacturerRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public int GetManufacturersCount(
            bool? publishedStatus = null)
        {
            IQueryable<Manufacturer> query = GetManufacturersQuery(publishedStatus);

            return query.Count(c => _storeMappingService.Authorize(c));
        }

        private IQueryable<Manufacturer> GetManufacturersQuery(bool? publishedStatus = null, IList<int> ids = null)
        {
            IQueryable<Manufacturer> query = _manufacturerRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.OrderByDescending(category => category.DisplayOrder);

            return query;
        }
    }
}