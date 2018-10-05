using System.Collections.Generic;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.Constants;

namespace Nop.Plugin.Api.Services
{
    public interface ITopicApiService
    {
        Topic GetTopicById(int categoryId);

        IList<Topic> GetTopics(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null);

        int GetTopicsCount(
            bool? publishedStatus = null);
    }
}