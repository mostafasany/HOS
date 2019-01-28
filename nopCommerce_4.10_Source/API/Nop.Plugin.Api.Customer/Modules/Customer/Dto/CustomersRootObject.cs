using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Dto
{
    public class CustomersRootObject : ISerializableObject
    {
        public CustomersRootObject() => Customers = new List<CustomerDto>();

        [JsonProperty("customers")]
        public IList<CustomerDto> Customers { get; set; }

        public string GetPrimaryPropertyName() => "customers";

        public Type GetPrimaryPropertyType() => typeof(CustomerDto);
    }
}