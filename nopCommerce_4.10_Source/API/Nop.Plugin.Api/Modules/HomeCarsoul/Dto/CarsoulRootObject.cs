using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Carsoul
{
    public class CarsoulRootObject : ISerializableObject
    {
        public CarsoulRootObject() => Carsoul = new List<CarsoulDto>();

        [JsonProperty("carsoul")]
        public IList<CarsoulDto> Carsoul { get; set; }

        public string GetPrimaryPropertyName() => "Carsoul";

        public Type GetPrimaryPropertyType() => typeof(CarsoulDto);
    }
}