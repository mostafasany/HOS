using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.NewsLetterSubscription.Dto;

namespace Nop.Plugin.Api.Modules.NewsLetterSubscription.Translator
{
    public static class NewsLetterSubscriptoonDtoMappings
    {
        public static NewsLetterSubscriptionDto ToDto(this Core.Domain.Messages.NewsLetterSubscription newsLetterSubscription) => newsLetterSubscription.MapTo<Core.Domain.Messages.NewsLetterSubscription, NewsLetterSubscriptionDto>();

        public static Core.Domain.Messages.NewsLetterSubscription ToEntity(this NewsLetterSubscriptionDto newsLetterSubscriptionDto) => newsLetterSubscriptionDto.MapTo<NewsLetterSubscriptionDto, Core.Domain.Messages.NewsLetterSubscription>();
    }
}