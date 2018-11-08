﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ShoppingCartsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsForCrossSellsParametersModel>))]
    public class ShoppingCartItemsForCrossSellsParametersModel : BaseShoppingCartItemsParametersModel
    {
        /// <summary>
        ///     Show shopping cart items created after date (format: 2008-12-31 03:00)
        /// </summary>

        [JsonProperty("product_ids")]
        public List<int> ProductIds { get; set; }
    }
}