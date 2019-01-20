﻿using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Modules.Stores.Dto;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Stores
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StoreController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly IStoreContext _storeContext;

        public StoreController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreContext storeContext,
            IDTOHelper dtoHelper)
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
            _storeContext = storeContext;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Retrieve all stores
        /// </summary>
        /// <param name="fields">Fields from the store you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/stores")]
        [ProducesResponseType(typeof(StoresRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllStores(string fields = "")
        {
            IList<Store> allStores = StoreService.GetAllStores();

            IList<StoreDto> storesAsDto = new List<StoreDto>();

            foreach (Store store in allStores)
            {
                StoreDto storeDto = _dtoHelper.PrepareStoreDTO(store);

                storesAsDto.Add(storeDto);
            }

            var storesRootObject = new StoresRootObject
            {
                Stores = storesAsDto
            };

            string json = JsonFieldsSerializer.Serialize(storesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve category by spcified id
        /// </summary>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/current_store")]
        [ProducesResponseType(typeof(StoresRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCurrentStore(string fields = "")
        {
            Store store = _storeContext.CurrentStore;

            if (store == null) return Error(HttpStatusCode.NotFound, "store", "store not found");

            StoreDto storeDto = _dtoHelper.PrepareStoreDTO(store);

            var storesRootObject = new StoresRootObject();

            storesRootObject.Stores.Add(storeDto);

            string json = JsonFieldsSerializer.Serialize(storesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}