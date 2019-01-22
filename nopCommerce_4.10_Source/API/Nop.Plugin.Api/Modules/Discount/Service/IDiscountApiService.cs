using System.Collections.Generic;

namespace Nop.Plugin.Api.Modules.Discount.Service
{
    public interface IDiscountApiService
    {
        IList<Core.Domain.Discounts.Discount> GetDiscounts(IList<int> ids = null, int? productId = null);
    }
}