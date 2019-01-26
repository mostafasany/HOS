using Nop.Plugin.Api.Content.Modules.Topic.Dto;

namespace Nop.Plugin.Api.Content.Modules.Topic.Translator
{
    public interface ITopicTransaltor
    {
        TopicDto ConvertToDto(Core.Domain.Topics.Topic topic);
    }
}