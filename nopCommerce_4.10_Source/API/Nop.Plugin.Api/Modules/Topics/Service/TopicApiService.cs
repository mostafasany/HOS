using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Topics.Service
{
    public class TopicApiService : ITopicApiService
    {
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Topic> _topicRepository;

        public TopicApiService(IRepository<Topic> topicRepository,
            IStoreMappingService storeMappingService)
        {
            _topicRepository = topicRepository;
            _storeMappingService = storeMappingService;
        }

        public IList<Topic> GetTopics(IList<int> ids = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            bool? publishedStatus = null)
        {
            IQueryable<Topic> query = GetCategoriesQuery(publishedStatus, ids);


            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Topic>(query, page - 1, limit);
        }

        public Topic GetTopicById(int id)
        {
            if (id <= 0)
                return null;

            Topic category = _topicRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public int GetTopicsCount(
            bool? publishedStatus = null)
        {
            IQueryable<Topic> query = GetCategoriesQuery(publishedStatus);

            return query.Count(c => _storeMappingService.Authorize(c));
        }

        private IQueryable<Topic> GetCategoriesQuery(bool? publishedStatus = null, IList<int> ids = null)
        {
            IQueryable<Topic> query = _topicRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.OrderBy(category => category.Id);

            return query;
        }
    }
}