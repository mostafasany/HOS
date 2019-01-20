using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Modules.Countries.Service
{
    public class StateProvinceApiService : IStateProvinceApiService
    {
        private readonly IRepository<StateProvince> _stateProvinceRepository;

        public StateProvinceApiService(IRepository<StateProvince> stateProvinceRepository) => _stateProvinceRepository = stateProvinceRepository;

        public IList<StateProvince> GetStateProvincesByCountryId(int countryId)
        {
            IQueryable<StateProvince> query = GetStatesQuery(countryId);
            return new ApiList<StateProvince>(query, 0, 10000);
        }

        private IQueryable<StateProvince> GetStatesQuery(int countryId)
        {
            IQueryable<StateProvince> query = from productCategoryMapping in _stateProvinceRepository.Table
                where productCategoryMapping.CountryId == countryId
                select productCategoryMapping;
            return query;
        }
    }
}