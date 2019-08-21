using Nop.Plugin.Api.Content.Modules.Topic.Dto;

namespace Nop.Plugin.Api.Content.Modules.Topic.Translator
{
    public interface ITopicTranslator
    {
        TopicDto ToDto(Core.Domain.Topics.Topic topic);
    }
}