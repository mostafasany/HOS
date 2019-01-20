﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.HomeCarsoul.Dto
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