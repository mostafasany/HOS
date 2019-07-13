using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Plugin.Api.Common.Converters
{
    public class ApiTypeConverter : IApiTypeConverter
    {
        /// <summary>
        ///     Converts the value, which should be in ISO 8601 format to UTC time or null if not valid
        /// </summary>
        /// <param name="value">The time format in ISO 8601. If no timezone or offset specified we assume it is in UTC</param>
        /// <returns>The time in UTC or null if the time is not valid</returns>
        public DateTime? ToUtcDateTimeNullable(string value)
        {
            var formats = new[]
            {
                "yyyy", "yyyy-MM", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"
            };

            if (!DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,
                out var result)) return null;
            // only if parsed in Local time then we need to convert it to UTC
            return result.Kind == DateTimeKind.Local ? result.ToUniversalTime() : result;
        }

        public int ToInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }

        public int? ToIntNullable(string value)
        {
            if (int.TryParse(value, out var result)) return result;

            return null;
        }

        public IList<int> ToListOfInts(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var stringIds = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var intIds = new List<int>();

            foreach (var id in stringIds)
            {
                if (int.TryParse(id, out var intId)) intIds.Add(intId);
            }

            intIds = intIds.Distinct().ToList();
            return intIds.Count > 0 ? intIds : null;

        }

        public bool? ToStatus(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (value.Equals("published", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (value.Equals("unpublished", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return null;
        }

        public object ToEnumNullable(string value, Type type)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var enumType = Nullable.GetUnderlyingType(type);

            var enumNames = enumType.GetEnumNames();

            return enumNames.Any(x => x.ToLowerInvariant().Equals(value.ToLowerInvariant())) ? Enum.Parse(enumType, value, true) : null;
        }
    }
}