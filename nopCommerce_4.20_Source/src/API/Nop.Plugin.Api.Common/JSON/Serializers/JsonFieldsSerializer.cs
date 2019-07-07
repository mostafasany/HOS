using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.Helpers;

namespace Nop.Plugin.Api.Common.JSON.Serializers
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        public string Serialize(ISerializableObject objectToSerialize, string jsonFields)
        {
            if (objectToSerialize == null) throw new ArgumentNullException(nameof(objectToSerialize));

            IList<string> fieldsList = null;

            if (!string.IsNullOrEmpty(jsonFields))
            {
                string primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();

                fieldsList = GetPropertiesIntoList(jsonFields);

                // Always add the root manually
                fieldsList.Add(primaryPropertyName);
            }

            string json = Serialize(objectToSerialize, fieldsList);

            return json;
        }

        private IList<string> GetPropertiesIntoList(string fields)
        {
            IList<string> properties = fields.ToLowerInvariant()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }

        private string Serialize(object objectToSerialize, IList<string> jsonFields = null)
        {
            JToken jToken = JToken.FromObject(objectToSerialize);

            if (jsonFields != null) jToken = jToken.RemoveEmptyChildrenAndFilterByFields(jsonFields);

            string jTokenResult = jToken.ToString();

            return jTokenResult;
        }
    }
}