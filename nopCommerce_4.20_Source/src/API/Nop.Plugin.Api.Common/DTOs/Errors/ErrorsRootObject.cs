﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Common.DTOs.Errors
{
    public class ErrorsRootObject : ISerializableObject
    {
        //[JsonProperty("errors")]
        //public Dictionary<string, List<string>> Errors { get; set; }

        [JsonProperty("errors")] public List<ErrorObject> Errors { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "errors";
        }

        public Type GetPrimaryPropertyType()
        {
            return Errors.GetType();
        }
    }
}