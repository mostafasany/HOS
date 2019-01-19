using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Customers
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