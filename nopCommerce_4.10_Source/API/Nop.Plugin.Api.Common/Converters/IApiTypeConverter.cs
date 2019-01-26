using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Common.Converters
{
    public interface IApiTypeConverter
    {
        object ToEnumNullable(string value, Type type);
        int ToInt(string value);
        int? ToIntNullable(string value);
        IList<int> ToListOfInts(string value);
        bool? ToStatus(string value);
        DateTime? ToUtcDateTimeNullable(string value);
    }
}