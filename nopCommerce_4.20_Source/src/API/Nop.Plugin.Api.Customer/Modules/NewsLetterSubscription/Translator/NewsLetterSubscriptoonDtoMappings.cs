using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Customer.Modules.NewsLetterSubscription.Dto;

namespace Nop.Plugin.Api.Customer.Modules.NewsLetterSubscription.Translator
{
    public static class NewsLetterSubscriptoonDtoMappings
    {
        public static NewsLetterSubscriptionDto ToDto(
            this Core.Domain.Messages.NewsLetterSubscription newsLetterSubscription)
        {
            return newsLetterSubscription
                .MapTo<Core.Domain.Messages.NewsLetterSubscription, NewsLetterSubscriptionDto>();
        }

        public static Core.Domain.Messages.NewsLetterSubscription ToEntity(
            this NewsLetterSubscriptionDto newsLetterSubscriptionDto)
        {
            return newsLetterSubscriptionDto
                .MapTo<NewsLetterSubscriptionDto, Core.Domain.Messages.NewsLetterSubscription>();
        }
    }
}