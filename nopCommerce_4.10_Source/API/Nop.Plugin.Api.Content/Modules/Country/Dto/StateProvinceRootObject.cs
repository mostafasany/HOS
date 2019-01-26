using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Content.Modules.Country.Dto
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