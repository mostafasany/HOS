﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.DiscountsParameters
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<DiscountsParametersModel>))]
    public class DiscountsParametersModel
    {
        public DiscountsParametersModel()
        {
            Fields = null;
            Ids = null;
        }

        /// <summary>
        ///     A comma-separated list of order ids
        /// </summary>
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        /// <summary>
        /// comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}