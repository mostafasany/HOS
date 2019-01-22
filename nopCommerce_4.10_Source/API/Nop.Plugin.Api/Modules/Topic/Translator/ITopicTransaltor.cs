using Nop.Plugin.Api.Modules.Topic.Dto;

namespace Nop.Plugin.Api.Modules.Topic.Translator
{
    public interface ITopicTransaltor
    {
        TopicDto ConvertToDto(Core.Domain.Topics.Topic topic);
    }
}