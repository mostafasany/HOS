using System;
using System.Collections.Generic;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Api.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface INewsLetterSubscriptionApiService
    {
        List<NewsLetterSubscription> GetNewsLetterSubscriptions(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? onlyActive = true);
    }
}