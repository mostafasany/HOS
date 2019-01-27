using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.NewsLetterSubscription.Dto
{
    public class NewsLetterSubscriptionsRootObject : ISerializableObject
    {
        public NewsLetterSubscriptionsRootObject() => NewsLetterSubscriptions = new List<NewsLetterSubscriptionDto>();

        [JsonProperty("news_letter_subscriptions")]
        public IList<NewsLetterSubscriptionDto> NewsLetterSubscriptions { get; set; }

        public string GetPrimaryPropertyName() => "news_letter_subscriptions";

        public Type GetPrimaryPropertyType() => typeof(NewsLetterSubscriptionDto);
    }
}