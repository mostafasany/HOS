using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Countries
{
    public class StateProvinceRootObject : ISerializableObject
    {
        public StateProvinceRootObject() => States = new List<StateProvinceDto>();

        [JsonProperty("states")]
        public IList<StateProvinceDto> States { get; set; }

        public string GetPrimaryPropertyName() => "States";

        public Type GetPrimaryPropertyType() => typeof(StateProvinceDto);
    }
}