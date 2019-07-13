using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Dto
{
    [JsonObject(Title = "customer")]
    //TODO:4.2 Migration
    //[Validator(typeof(CustomerDtoValidator))]
    public class CustomerDto : BaseCustomerDto
    {
        private ICollection<AddressDto> _addresses;
       
        [JsonIgnore]
        [JsonProperty("password")]
        public string Password { get; set; }

        #region Navigation properties

     
        /// <summary>
        ///     Default billing address
        /// </summary>
        [JsonProperty("billing_address")]
        public AddressDto BillingAddress { get; set; }

        /// <summary>
        ///     Default shipping address
        /// </summary>
        [JsonProperty("shipping_address")]
        public AddressDto ShippingAddress { get; set; }

        /// <summary>
        ///     Default Picture
        /// </summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }


        /// <summary>
        ///     Default Phone
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Gets or sets customer addresses
        /// </summary>
        [JsonProperty("addresses")]
        public ICollection<AddressDto> Addresses
        {
            get => _addresses ?? (_addresses = new List<AddressDto>());
            set => _addresses = value;
        }

        #endregion
    }
}