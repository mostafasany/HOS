using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Customer.Modules.CustomerRoles.Dto;
using Nop.Plugin.Api.Customer.Modules.CustomerRoles.Translator;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerRolesController : BaseApiController
    {
        public CustomerRolesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService)
            : base(jsonFieldsSerializer,
                aclService,
                customerService,
                storeMappingService,
                storeService,
                discountService,
                customerActivityService,
                localizationService,
                pictureService)
        {
        }

        /// <summary>
        ///     Retrieve all customer roles
        /// </summary>
        /// <param name="fields">Fields from the customer role you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customer_roles")]
        [ProducesResponseType(typeof(CustomerRolesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllCustomerRoles(string fields = "")
        {
            IList<CustomerRole> allCustomerRoles = CustomerService.GetAllCustomerRoles();

            IList<CustomerRoleDto> customerRolesAsDto = allCustomerRoles.Select(role => role.ToDto()).ToList();

            var customerRolesRootObject = new CustomerRolesRootObject
            {
                CustomerRoles = customerRolesAsDto
            };

            string json = JsonFieldsSerializer.Serialize(customerRolesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}