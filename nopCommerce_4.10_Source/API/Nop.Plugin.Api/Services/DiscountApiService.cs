using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.DataStructures;

namespace Nop.Plugin.Api.Services
{
    public class DiscountApiService : IDiscountApiService
    {
        private readonly IRepository<Discount> _discountRepository;

        public DiscountApiService(IRepository<Discount> discountRepository) => _discountRepository = discountRepository;

        public IList<Discount> GetDiscounts(IList<int> ids = null, int? productId = null)
        {
            IQueryable<Discount> query = GetDiscountQueryQuery(ids);

            return new ApiList<Discount>(query, 0, 100);
        }

        private IQueryable<Discount> GetDiscountQueryQuery(IList<int> ids = null)

        {
            IQueryable<Discount> query = _discountRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            query = query.OrderByDescending(product => product.EndDateUtc);

            return query;
        }
    }
}