using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Common.Helpers
{
    public interface IMappingHelper
    {
        void Merge(object source, object destination);
        void SetValues(Dictionary<string, object> propertyNameValuePairs, object objectToBeUpdated, Type objectToBeUpdatedType, Dictionary<object, object> objectPropertyNameValuePairs, bool handleComplexTypeCollections = false);
    }
}