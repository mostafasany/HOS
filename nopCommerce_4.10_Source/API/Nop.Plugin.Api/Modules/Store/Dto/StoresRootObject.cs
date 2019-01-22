using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Store.Dto
{
    public class StoresRootObject : ISerializableObject
    {
        public StoresRootObject() => Stores = new List<StoreDto>();

        [JsonProperty("stores")]
        public IList<StoreDto> Stores { get; set; }

        public string GetPrimaryPropertyName() => "stores";

        public Type GetPrimaryPropertyType() => typeof(StoreDto);
    }
}