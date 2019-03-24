using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Modules.Discount.Service
{
    public class DiscountApiService : IDiscountApiService
    {
        private readonly IRepository<Core.Domain.Discounts.Discount> _discountRepository;

        public DiscountApiService(IRepository<Core.Domain.Discounts.Discount> discountRepository) => _discountRepository = discountRepository;

        public IList<Core.Domain.Discounts.Discount> GetDiscounts(IList<int> ids = null, int? productId = null)
        {
            IQueryable<Core.Domain.Discounts.Discount> query = GetDiscountQueryQuery(ids);

            return new ApiList<Core.Domain.Discounts.Discount>(query, 0, 100);
        }

        private IQueryable<Core.Domain.Discounts.Discount> GetDiscountQueryQuery(IList<int> ids = null)

        {
            IQueryable<Core.Domain.Discounts.Discount> query = _discountRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            query = query.OrderByDescending(product => product.EndDateUtc);

            return query;
        }
    }
}