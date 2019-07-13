using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nop.Plugin.Api.Common.Validators
{
    public class FieldsValidator : IFieldsValidator
    {
        public Dictionary<string, bool> GetValidFields(string fields, Type type)
        {
            // This check ensures that the fields won't be null, because it can couse exception.
            fields = fields ?? string.Empty;
            // This is needed in case you pass the fields as you see them in the json representation of the objects.
            // By specification if the property consists of several words, each word should be separetate from the others with underscore.
            fields = fields.Replace("_", string.Empty);

            var fieldsAsList = GetPropertiesIntoList(fields);

            return (from field in fieldsAsList
                let propertyExists =
                    type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) !=
                    null
                where propertyExists
                select field).ToDictionary(field => field, field => true);
        }

        private static IEnumerable<string> GetPropertiesIntoList(string fields)
        {
            var properties = fields.ToLowerInvariant()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }
    }
}