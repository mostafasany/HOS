using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Common.Maps
{
    public class JsonPropertyMapper : IJsonPropertyMapper
    {
        const string JsonTypeMapsPattern = "json.maps";
        private IStaticCacheManager _cacheManager;

        private IStaticCacheManager StaticCacheManager => _cacheManager ?? (_cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>());

        public Dictionary<string, Tuple<string, Type>> GetMap(Type type)
        {
            var isJsonTypeMapsPatternSet = StaticCacheManager.IsSet(JsonTypeMapsPattern);
            if (!isJsonTypeMapsPatternSet)
            {
                var emptyDic = new Dictionary<string, Dictionary<string, Tuple<string, Type>>>();
                StaticCacheManager.Set(JsonTypeMapsPattern, emptyDic, int.MaxValue);
            }

            var typeMaps = StaticCacheManager.Get<Dictionary<string, Dictionary<string, Tuple<string, Type>>>>
                (JsonTypeMapsPattern, () => null, null);

            if (!typeMaps.ContainsKey(type.Name))
            {
                Build(type);
            }

            return typeMaps[type.Name];
        }

        private void Build(Type type)
        {
            var typeMaps =
                StaticCacheManager.Get<Dictionary<string, Dictionary<string, Tuple<string, Type>>>>(JsonTypeMapsPattern, () => null, null);

            var mapForCurrentType = new Dictionary<string, Tuple<string, Type>>();

            var typeProps = type.GetProperties();
            
            foreach (var property in typeProps)
            {
                var jsonAttribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;
                var doNotMapAttribute = property.GetCustomAttribute(typeof(DoNotMapAttribute)) as DoNotMapAttribute;

                // If it has json attribute set and is not marked as doNotMap
                if (jsonAttribute != null && doNotMapAttribute == null)
                {
                    if (!mapForCurrentType.ContainsKey(jsonAttribute.PropertyName))
                    {
                        var value = new Tuple<string, Type>(property.Name, property.PropertyType);
                        mapForCurrentType.Add(jsonAttribute.PropertyName, value);
                    }
                }
            }
            
            if (!typeMaps.ContainsKey(type.Name))
            {
                typeMaps.Add(type.Name, mapForCurrentType);
            }
        }
    }
}