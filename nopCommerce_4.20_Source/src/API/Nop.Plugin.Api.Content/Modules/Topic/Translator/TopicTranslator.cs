using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Content.Modules.Topic.Dto;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Content.Modules.Topic.Translator
{
    public class TopicTranslator : ITopicTranslator
    {
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;

        public TopicTranslator(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }

        public TopicDto ToDto(Core.Domain.Topics.Topic topic)
        {
            var seName = _urlRecordService.GetSeName(topic);
            var body = _localizationService.GetLocalized(topic, x => x.Body, _currentLanguageId);
            var title = _localizationService.GetLocalized(topic, x => x.Title, _currentLanguageId);
            return new TopicDto {Id = topic.Id, Body = body, Title = title, SeName = seName};
        }
    }
}