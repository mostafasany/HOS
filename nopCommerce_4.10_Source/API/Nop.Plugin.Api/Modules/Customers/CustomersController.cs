﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomersController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerRolesHelper _customerRolesHelper;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFactory<Customer> _factory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMappingHelper _mappingHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        public CustomersController(
            ICustomerApiService customerApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerRolesHelper customerRolesHelper,
            IGenericAttributeService genericAttributeService,
            IEncryptionService encryptionService,
            IFactory<Customer> factory,
            ICountryService countryService,
            IMappingHelper mappingHelper,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService, ILanguageService languageService,
            ICustomerRegistrationService customerRegistrationService,
            IShoppingCartService shoppingCartService,
            IAuthenticationService authenticationService,
            IWorkContext workContext,
            IEventPublisher eventPublisher,
            IWorkflowMessageService workflowMessageService
        ) :
            base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _customerApiService = customerApiService;
            _factory = factory;
            _countryService = countryService;
            _mappingHelper = mappingHelper;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _languageService = languageService;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _customerRolesHelper = customerRolesHelper;
            _customerActivityService = customerActivityService;
            _customerRegistrationService = customerRegistrationService;
            _shoppingCartService = shoppingCartService;
            _authenticationService = authenticationService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _customerSettings = customerSettings;
            _workflowMessageService = workflowMessageService;
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/changepassword")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult ChangePassword([ModelBinder(typeof(JsonModelBinder<ChangePasswordDto>))]
            Delta<ChangePasswordDto> changeDelta)
        {
            //if (!_workContext.CurrentCustomer.IsRegistered())
            //    return Challenge();

            //var customer = _workContext.CurrentCustomer;
            ChangePasswordResult result = null;
            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(changeDelta.Dto.Email,
                    true, _customerSettings.DefaultPasswordFormat, changeDelta.Dto.NewPassword, changeDelta.Dto.OldPassword);
                result = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (result.Success) return Ok(_localizationService.GetResource("Account.ChangePassword.Success"));

                //errors
                foreach (string error in result.Errors)
                    ModelState.AddModelError("", error);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("/api/customers")]
        [ProducesResponseType(typeof(CustomersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult CreateCustomer([ModelBinder(typeof(JsonModelBinder<CustomerDto>))]
            Delta<CustomerDto> customerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            //If the validation has passed the customerDelta object won't be null for sure so we don't need to check for this.

            // Inserting the new customer
            Customer newCustomer = _factory.Initialize();
            customerDelta.Merge(newCustomer);

            foreach (AddressDto address in customerDelta.Dto.Addresses)
            {
                // we need to explicitly set the date as if it is not specified
                // it will default to 01/01/0001 which is not supported by SQL Server and throws and exception
                if (address.CreatedOnUtc == null) address.CreatedOnUtc = DateTime.UtcNow;
                newCustomer.Addresses.Add(address.ToEntity());
            }

            CustomerService.InsertCustomer(newCustomer);

            InsertFirstAndLastNameGenericAttributes(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, newCustomer);

            if (!string.IsNullOrEmpty(customerDelta.Dto.LanguageId) && int.TryParse(customerDelta.Dto.LanguageId, out int languageId)
                                                                    && _languageService.GetLanguageById(languageId) != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.LanguageIdAttribute, languageId);

            //password
            if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password)) AddPassword(customerDelta.Dto.Password, newCustomer);

            // We need to insert the entity first so we can have its id in order to map it to anything.
            // TODO: Localization
            // TODO: move this before inserting the customer.
            if (customerDelta.Dto.RoleIds.Count > 0)
            {
                AddValidRoles(customerDelta, newCustomer);

                CustomerService.UpdateCustomer(newCustomer);
            }

            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            CustomerDto newCustomerDto = newCustomer.ToDto();

            // This is needed because the entity framework won't populate the navigation properties automatically
            // and the country will be left null. So we do it by hand here.
            PopulateAddressCountryNames(newCustomerDto);

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            newCustomerDto.FirstName = customerDelta.Dto.FirstName;
            newCustomerDto.LastName = customerDelta.Dto.LastName;

            newCustomerDto.LanguageId = customerDelta.Dto.LanguageId;

            //activity log
            CustomerActivityService.InsertActivity("AddNewCustomer", LocalizationService.GetResource("ActivityLog.AddNewCustomer"), newCustomer);

            var customersRootObject = new CustomersRootObject();

            customersRootObject.Customers.Add(newCustomerDto);

            string json = JsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/customers/{id}")]
        [GetRequestsErrorInterceptorActionFilter]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult DeleteCustomer(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Customer customer = _customerApiService.GetCustomerEntityById(id);

            if (customer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            CustomerService.DeleteCustomer(customer);

            //remove newsletter subscription (if exists)
            foreach (Store store in StoreService.GetAllStores())
            {
                NewsLetterSubscription subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                if (subscription != null)
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
            }

            //activity log
            CustomerActivityService.InsertActivity("DeleteCustomer", LocalizationService.GetResource("ActivityLog.DeleteCustomer"), customer);

            return new RawJsonActionResult("{}");
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/forget")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult Forget([ModelBinder(typeof(JsonModelBinder<ForgetDto>))]
            Delta<ForgetDto> forgetDelta)
        {
            Customer customer = _customerService.GetCustomerByEmail(forgetDelta.Dto.Email);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                Guid passwordRecoveryToken = Guid.NewGuid();
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                    passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                _genericAttributeService.SaveAttribute(customer,
                    NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                //send email
                _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer,
                    _workContext.WorkingLanguage.Id);

                //return new RawJsonActionResult(_localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent"));
                return Ok(_localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent"));
            }

            return BadRequest(_localizationService.GetResource("Account.PasswordRecovery.EmailNotFound"));
        }

        /// <summary>
        ///     Retrieve customer by spcified id
        /// </summary>
        /// <param name="id">Id of the customer</param>
        /// <param name="fields">Fields from the customer you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customers/{id}")]
        [ProducesResponseType(typeof(CustomersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCustomerById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            CustomerDto customer = _customerApiService.GetCustomerById(id);

            if (customer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            var customersRootObject = new CustomersRootObject();
            customersRootObject.Customers.Add(customer);

            string json = JsonFieldsSerializer.Serialize(customersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve all customers of a shop
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customers")]
        [ProducesResponseType(typeof(CustomersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCustomers(CustomersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            IList<CustomerDto> allCustomers = _customerApiService.GetCustomersDtos(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId);

            var customersRootObject = new CustomersRootObject
            {
                Customers = allCustomers
            };

            string json = JsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customers/count")]
        [ProducesResponseType(typeof(CustomersCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult GetCustomersCount()
        {
            int allCustomersCount = _customerApiService.GetCustomersCount();

            var customersCountRootObject = new CustomersCountRootObject
            {
                Count = allCustomersCount
            };

            return Ok(customersCountRootObject);
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/login")]
        [ProducesResponseType(typeof(CustomersCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult Login([ModelBinder(typeof(JsonModelBinder<LoginDto>))]
            Delta<LoginDto> loginDelta)
        {
            CustomerLoginResults loginResult = _customerRegistrationService.ValidateCustomer(loginDelta.Dto.UserNameOrEmail, loginDelta.Dto.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                {
                    Customer customer = _customerSettings.UsernamesEnabled
                        ? _customerService.GetCustomerByUsername(loginDelta.Dto.UserNameOrEmail)
                        : _customerService.GetCustomerByEmail(loginDelta.Dto.UserNameOrEmail);

                    CustomerDto customerDto = _customerApiService.GetCustomerById(customer.Id);

                    //migrate shopping cart
                    _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                    //sign in new customer
                    _authenticationService.SignIn(customer, true);

                    //raise event       
                    _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                    //activity log
                    _customerActivityService.InsertActivity(customer, "PublicStore.Login",
                        _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                    var customersRootObject = new CustomersRootObject();
                    customersRootObject.Customers.Add(customerDto);

                    string json = JsonFieldsSerializer.Serialize(customersRootObject, "");

                    return new RawJsonActionResult(json);
                    //if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                    //    return RedirectToRoute("HomePage");
                    // return Ok(customer);
                    //return Redirect(returnUrl);
                }
                case CustomerLoginResults.CustomerNotExist:
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                    break;
                case CustomerLoginResults.Deleted:
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                    break;
                case CustomerLoginResults.NotActive:
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                    break;
                case CustomerLoginResults.NotRegistered:
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                    break;
                case CustomerLoginResults.LockedOut:
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                    break;
                case CustomerLoginResults.WrongPassword:
                default:
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                    break;
            }

            return null;
        }

        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/logout")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult Logout()
        {
            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                        _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id),
                    _workContext.CurrentCustomer);

                _customerActivityService.InsertActivity("Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                        _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                    _workContext.OriginalCustomerIfImpersonated);

                //logout impersonated customer
                _genericAttributeService
                    .SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);
            }

            //activity log
            _customerActivityService.InsertActivity(_workContext.CurrentCustomer, "PublicStore.Logout",
                _localizationService.GetResource("ActivityLog.PublicStore.Logout"), _workContext.CurrentCustomer);

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            return Ok();
        }

        /// <summary>
        ///     Search for customers matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customers/search")]
        [ProducesResponseType(typeof(CustomersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult Search(CustomersSearchParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page <= 0) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            IList<CustomerDto> customersDto = _customerApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

            var customersRootObject = new CustomersRootObject
            {
                Customers = customersDto
            };

            string json = JsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/customers/{id}")]
        [ProducesResponseType(typeof(CustomersRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult UpdateCustomer([ModelBinder(typeof(JsonModelBinder<CustomerDto>))]
            Delta<CustomerDto> customerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            // Updateting the customer
            Customer currentCustomer = _customerApiService.GetCustomerEntityById(customerDelta.Dto.Id);

            if (currentCustomer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            customerDelta.Merge(currentCustomer);

            if (customerDelta.Dto.RoleIds.Count > 0)
            {
                // Remove all roles
                while (currentCustomer.CustomerRoles.Count > 0) currentCustomer.CustomerRoles.Remove(currentCustomer.CustomerRoles.First());

                AddValidRoles(customerDelta, currentCustomer);
            }

            if (customerDelta.Dto.Addresses.Count > 0)
            {
                Dictionary<int, Address> currentCustomerAddresses = currentCustomer.Addresses.ToDictionary(address => address.Id, address => address);

                foreach (AddressDto passedAddress in customerDelta.Dto.Addresses)
                {
                    Address addressEntity = passedAddress.ToEntity();

                    if (currentCustomerAddresses.ContainsKey(passedAddress.Id))
                        _mappingHelper.Merge(passedAddress, currentCustomerAddresses[passedAddress.Id]);
                    else
                        currentCustomer.Addresses.Add(addressEntity);
                }
            }

            CustomerService.UpdateCustomer(currentCustomer);

            InsertFirstAndLastNameGenericAttributes(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, currentCustomer);


            if (!string.IsNullOrEmpty(customerDelta.Dto.LanguageId) && int.TryParse(customerDelta.Dto.LanguageId, out int languageId)
                                                                    && _languageService.GetLanguageById(languageId) != null)
                _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.LanguageIdAttribute, languageId);

            //password
            if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password)) AddPassword(customerDelta.Dto.Password, currentCustomer);

            // TODO: Localization

            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            CustomerDto updatedCustomer = currentCustomer.ToDto();

            // This is needed because the entity framework won't populate the navigation properties automatically
            // and the country name will be left empty because the mapping depends on the navigation property
            // so we do it by hand here.
            PopulateAddressCountryNames(updatedCustomer);

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            GenericAttribute firstNameGenericAttribute = _genericAttributeService.GetAttributesForEntity(currentCustomer.Id, typeof(Customer).Name)
                .FirstOrDefault(x => x.Key == "FirstName");

            if (firstNameGenericAttribute != null) updatedCustomer.FirstName = firstNameGenericAttribute.Value;

            GenericAttribute lastNameGenericAttribute = _genericAttributeService.GetAttributesForEntity(currentCustomer.Id, typeof(Customer).Name)
                .FirstOrDefault(x => x.Key == "LastName");

            if (lastNameGenericAttribute != null) updatedCustomer.LastName = lastNameGenericAttribute.Value;

            GenericAttribute languageIdGenericAttribute = _genericAttributeService.GetAttributesForEntity(currentCustomer.Id, typeof(Customer).Name)
                .FirstOrDefault(x => x.Key == "LanguageId");

            if (languageIdGenericAttribute != null) updatedCustomer.LanguageId = languageIdGenericAttribute.Value;

            //activity log
            CustomerActivityService.InsertActivity("UpdateCustomer", LocalizationService.GetResource("ActivityLog.UpdateCustomer"), currentCustomer);

            var customersRootObject = new CustomersRootObject();

            customersRootObject.Customers.Add(updatedCustomer);

            string json = JsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private void AddPassword(string newPassword, Customer customer)
        {
            // TODO: call this method before inserting the customer.
            var customerPassword = new CustomerPassword
            {
                Customer = customer,
                PasswordFormat = _customerSettings.DefaultPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };

            switch (_customerSettings.DefaultPasswordFormat)
            {
                case PasswordFormat.Clear:
                {
                    customerPassword.Password = newPassword;
                }
                    break;
                case PasswordFormat.Encrypted:
                {
                    customerPassword.Password = _encryptionService.EncryptText(newPassword);
                }
                    break;
                case PasswordFormat.Hashed:
                {
                    string saltKey = _encryptionService.CreateSaltKey(5);
                    customerPassword.PasswordSalt = saltKey;
                    customerPassword.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey, _customerSettings.HashedPasswordFormat);
                }
                    break;
            }

            CustomerService.InsertCustomerPassword(customerPassword);

            // TODO: remove this.
            CustomerService.UpdateCustomer(customer);
        }

        private void AddValidRoles(Delta<CustomerDto> customerDelta, Customer currentCustomer)
        {
            IList<CustomerRole> validCustomerRoles =
                _customerRolesHelper.GetValidCustomerRoles(customerDelta.Dto.RoleIds).ToList();

            // Add all newly passed roles
            foreach (CustomerRole role in validCustomerRoles) currentCustomer.CustomerRoles.Add(role);
        }

        private void InsertFirstAndLastNameGenericAttributes(string firstName, string lastName, Customer newCustomer)
        {
            // we assume that if the first name is not sent then it will be null and in this case we don't want to update it
            if (firstName != null) _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.FirstNameAttribute, firstName);

            if (lastName != null) _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.LastNameAttribute, lastName);
        }

        private void PopulateAddressCountryNames(CustomerDto newCustomerDto)
        {
            foreach (AddressDto address in newCustomerDto.Addresses) SetCountryName(address);

            if (newCustomerDto.BillingAddress != null) SetCountryName(newCustomerDto.BillingAddress);

            if (newCustomerDto.ShippingAddress != null) SetCountryName(newCustomerDto.ShippingAddress);
        }

        private void SetCountryName(AddressDto address)
        {
            if (string.IsNullOrEmpty(address.CountryName) && address.CountryId.HasValue)
            {
                Country country = _countryService.GetCountryById(address.CountryId.Value);
                address.CountryName = country.Name;
            }
        }
    }
}