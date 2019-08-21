using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Dto
{
    public class ManufacturersRootObject : ISerializableObject
    {
        public ManufacturersRootObject()
        {
            Manufacturers = new List<ManufacturerDto>();
        }

        [JsonProperty("manufacturers")] public IList<ManufacturerDto> Manufacturers { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "manufacturers";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ManufacturerDto);
        }
    }
}