using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Modules.Topic.Service
{
    public interface ITopicApiService
    {
        Core.Domain.Topics.Topic GetTopicById(int topicId);

        IList<Core.Domain.Topics.Topic> GetTopics(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null);

        int GetTopicsCount(
            bool? publishedStatus = null);
    }
}