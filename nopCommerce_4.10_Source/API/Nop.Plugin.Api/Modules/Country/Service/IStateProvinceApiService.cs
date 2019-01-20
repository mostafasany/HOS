using System.Collections.Generic;
using Nop.Core.Domain.Directory;

namespace Nop.Plugin.Api.Modules.Countries.Service
{
    public interface IStateProvinceApiService
    {
        /// <summary>
        ///     Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>States</returns>
        IList<StateProvince> GetStateProvincesByCountryId(int countryId);
    }
}