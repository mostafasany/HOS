using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Common.Controllers
{
    public class BaseApiController : Controller
    {
        protected readonly IAclService AclService;
        protected readonly ICustomerActivityService CustomerActivityService;
        protected readonly ICustomerService CustomerService;
        protected readonly IDiscountService DiscountService;
        protected readonly IJsonFieldsSerializer JsonFieldsSerializer;
        protected readonly ILocalizationService LocalizationService;
        protected readonly IPictureService PictureService;
        protected readonly IStoreMappingService StoreMappingService;
        protected readonly IStoreService StoreService;

        public BaseApiController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService)
        {
            JsonFieldsSerializer = jsonFieldsSerializer;
            AclService = aclService;
            CustomerService = customerService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            DiscountService = discountService;
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            PictureService = pictureService;
        }

        protected IActionResult Error(HttpStatusCode statusCode = (HttpStatusCode) 422, string propertyKey = "", string errorMessage = "")
        {
            var errors = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(propertyKey))
            {
                var errorsList = new List<string> {errorMessage};
                errors.Add(propertyKey, errorsList);
            }

            foreach (KeyValuePair<string, ModelStateEntry> item in ModelState)
            {
                IEnumerable<string> errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                var validErrorMessages = new List<string>();

                validErrorMessages.AddRange(errorMessages.Where(message => !string.IsNullOrEmpty(message)));

                if (validErrorMessages.Count > 0)
                    if (errors.ContainsKey(item.Key))
                        errors[item.Key].AddRange(validErrorMessages);
                    else
                        errors.Add(item.Key, validErrorMessages.ToList());
            }

            var errorsRootObject = new ErrorsRootObject
            {
                Errors = errors
            };

            string errorsJson = JsonFieldsSerializer.Serialize(errorsRootObject, null);

            return new ErrorActionResult(errorsJson, statusCode);
        }

        protected void UpdateAclRoles<TEntity>(TEntity entity, List<int> passedRoleIds) where TEntity : BaseEntity, IAclSupported
        {
            if (passedRoleIds == null) return;

            entity.SubjectToAcl = passedRoleIds.Any();

            IList<AclRecord> existingAclRecords = AclService.GetAclRecords(entity);
            IList<CustomerRole> allCustomerRoles = CustomerService.GetAllCustomerRoles(true);
            foreach (CustomerRole customerRole in allCustomerRoles)
                if (passedRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        AclService.InsertAclRecord(entity, customerRole.Id);
                }
                else
                {
                    //remove role
                    AclRecord aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        AclService.DeleteAclRecord(aclRecordToDelete);
                }
        }

        protected void UpdateStoreMappings<TEntity>(TEntity entity, List<int> passedStoreIds) where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (passedStoreIds == null)
                return;

            entity.LimitedToStores = passedStoreIds.Any();

            IList<StoreMapping> existingStoreMappings = StoreMappingService.GetStoreMappings(entity);
            IList<Store> allStores = StoreService.GetAllStores();
            foreach (Store store in allStores)
                if (passedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        StoreMappingService.InsertStoreMapping(entity, store.Id);
                }
                else
                {
                    //remove store
                    StoreMapping storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        StoreMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
        }
    }
}