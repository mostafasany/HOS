using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Customer.Modules.NewsLetterSubscription.Service
{
    public interface INewsLetterSubscriptionApiService
    {
        List<Core.Domain.Messages.NewsLetterSubscription> GetNewsLetterSubscriptions(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? onlyActive = true);
    }
}