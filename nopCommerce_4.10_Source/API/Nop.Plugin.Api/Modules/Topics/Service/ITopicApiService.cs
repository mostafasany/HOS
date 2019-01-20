using System.Collections.Generic;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Modules.Topics.Service
{
    public interface ITopicApiService
    {
        Topic GetTopicById(int topicId);

        IList<Topic> GetTopics(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null);

        int GetTopicsCount(
            bool? publishedStatus = null);
    }
}