using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Topic.Service
{
    public class TopicApiService : ITopicApiService
    {
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Core.Domain.Topics.Topic> _topicRepository;

        public TopicApiService(IRepository<Core.Domain.Topics.Topic> topicRepository,
            IStoreMappingService storeMappingService)
        {
            _topicRepository = topicRepository;
            _storeMappingService = storeMappingService;
        }

        public IList<Core.Domain.Topics.Topic> GetTopics(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null)
        {
            IQueryable<Core.Domain.Topics.Topic> query = GetCategoriesQuery(publishedStatus, ids);


            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Core.Domain.Topics.Topic>(query, page - 1, limit);
        }

        public Core.Domain.Topics.Topic GetTopicById(int id)
        {
            if (id <= 0)
                return null;

            Core.Domain.Topics.Topic category = _topicRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public int GetTopicsCount(
            bool? publishedStatus = null)
        {
            IQueryable<Core.Domain.Topics.Topic> query = GetCategoriesQuery(publishedStatus);

            return query.Count(c => _storeMappingService.Authorize(c));
        }

        private IQueryable<Core.Domain.Topics.Topic> GetCategoriesQuery(bool? publishedStatus = null, IList<int> ids = null)
        {
            IQueryable<Core.Domain.Topics.Topic> query = _topicRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.OrderBy(category => category.Id);

            return query;
        }
    }
}