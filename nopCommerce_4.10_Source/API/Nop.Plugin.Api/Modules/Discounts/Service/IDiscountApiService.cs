using System.Collections.Generic;
using Nop.Core.Domain.Discounts;

namespace Nop.Plugin.Api.Modules.Discounts.Service
{
    public interface IDiscountApiService
    {
        IList<Discount> GetDiscounts(IList<int> ids = null, int? productId = null);
    }
}