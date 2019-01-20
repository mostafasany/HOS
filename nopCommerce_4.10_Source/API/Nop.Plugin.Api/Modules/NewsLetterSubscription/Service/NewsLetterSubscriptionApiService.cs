using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Modules.NewsLetterSubscription.Service
{
    public class NewsLetterSubscriptionApiService : INewsLetterSubscriptionApiService
    {
        private readonly IRepository<Core.Domain.Messages.NewsLetterSubscription> _newsLetterSubscriptionRepository;
        private readonly IStoreContext _storeContext;

        public NewsLetterSubscriptionApiService(IRepository<Core.Domain.Messages.NewsLetterSubscription> newsLetterSubscriptionRepository, IStoreContext storeContext)
        {
            _newsLetterSubscriptionRepository = newsLetterSubscriptionRepository;
            _storeContext = storeContext;
        }

        public List<Core.Domain.Messages.NewsLetterSubscription> GetNewsLetterSubscriptions(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? onlyActive = true)
        {
            IQueryable<Core.Domain.Messages.NewsLetterSubscription> query = GetNewsLetterSubscriptionsQuery(createdAtMin, createdAtMax, onlyActive);

            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Core.Domain.Messages.NewsLetterSubscription>(query, page - 1, limit);
        }

        private IQueryable<Core.Domain.Messages.NewsLetterSubscription> GetNewsLetterSubscriptionsQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, bool? onlyActive = true)
        {
            IQueryable<Core.Domain.Messages.NewsLetterSubscription> query = _newsLetterSubscriptionRepository.Table.Where(nls => nls.StoreId == _storeContext.CurrentStore.Id);

            if (onlyActive != null && onlyActive == true) query = query.Where(nls => nls.Active == onlyActive);

            if (createdAtMin != null) query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null) query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            query = query.OrderBy(nls => nls.Id);

            return query;
        }
    }
}