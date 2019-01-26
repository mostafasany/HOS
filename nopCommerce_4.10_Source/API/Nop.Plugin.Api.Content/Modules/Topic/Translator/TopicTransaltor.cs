using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Api.Content.Modules.Topic.Dto;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Content.Modules.Topic.Translator
{
    public class TopicTransaltor : ITopicTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;

        public TopicTransaltor(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                StringValues lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public TopicDto ConvertToDto(Core.Domain.Topics.Topic topic)
        {
            string seName = _urlRecordService.GetSeName(topic);
            string body = _localizationService.GetLocalized(topic, x => x.Body, _currentLangaugeId);
            string title = _localizationService.GetLocalized(topic, x => x.Title, _currentLangaugeId);
            return new TopicDto {Id = topic.Id, Body = body, Title = title, SeName = seName};
        }
    }
}