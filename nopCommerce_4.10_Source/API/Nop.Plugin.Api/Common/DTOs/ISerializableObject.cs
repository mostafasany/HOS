using System;

namespace Nop.Plugin.Api.Common.DTOs
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();
        Type GetPrimaryPropertyType();
    }
}