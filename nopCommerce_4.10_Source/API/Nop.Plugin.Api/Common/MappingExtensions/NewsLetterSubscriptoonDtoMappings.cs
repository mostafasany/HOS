using Nop.Core.Domain.Messages;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.NewsLetterSubscription.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class NewsLetterSubscriptoonDtoMappings
    {
        public static NewsLetterSubscriptionDto ToDto(this NewsLetterSubscription newsLetterSubscription) => newsLetterSubscription.MapTo<NewsLetterSubscription, NewsLetterSubscriptionDto>();

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionDto newsLetterSubscriptionDto) => newsLetterSubscriptionDto.MapTo<NewsLetterSubscriptionDto, NewsLetterSubscription>();
    }
}