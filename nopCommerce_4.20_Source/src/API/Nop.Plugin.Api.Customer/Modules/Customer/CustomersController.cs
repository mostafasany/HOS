using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.Delta;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Customer.Modules.Customer.Dto;
using Nop.Plugin.Api.Customer.Modules.Customer.Model;
using Nop.Plugin.Api.Customer.Modules.Customer.Service;
using Nop.Plugin.Api.Customer.Modules.Customer.Translator;
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
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Customer.Modules.Customer
{
    // [Authorize(Policy = "Registered", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomersController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFactory<Core.Domain.Customers.Customer> _factory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMappingHelper _mappingHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
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
            IGenericAttributeService genericAttributeService,
            IEncryptionService encryptionService,
            IFactory<Core.Domain.Customers.Customer> factory,
            ICountryService countryService,
            IMappingHelper mappingHelper,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService, ILanguageService languageService,
            ICustomerRegistrationService customerRegistrationService,
            IAuthenticationService authenticationService,
            IWorkContext workContext,
            IEventPublisher eventPublisher,
            IWorkflowMessageService workflowMessageService,
            IHttpContextAccessor httpContextAccessor
        ) :
            base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _customerApiService = customerApiService;
            _factory = factory;
            _countryService = countryService;
            _mappingHelper = mappingHelper;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _languageService = languageService;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _customerActivityService = customerActivityService;
            _customerRegistrationService = customerRegistrationService;
            _authenticationService = authenticationService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _customerSettings = customerSettings;
            _workflowMessageService = workflowMessageService;
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/changepassword")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult ChangePassword([ModelBinder(typeof(JsonModelBinder<ChangePasswordDto>))]
            Delta<ChangePasswordDto> changeDelta)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            if (!ModelState.IsValid) return Error();


            var changePasswordRequest = new ChangePasswordRequest(_workContext.CurrentCustomer.Email,
                true, _customerSettings.DefaultPasswordFormat, changeDelta.Dto.NewPassword,
                changeDelta.Dto.OldPassword);
            var result = _customerRegistrationService.ChangePassword(changePasswordRequest);

            var dynamicVariable = new InformationDto
            {
                Message = _localizationService.GetResource("Account.ChangePassword.Success")
            };
            var json = JsonFieldsSerializer.Serialize(dynamicVariable, string.Empty);
            return result.Success ? new RawJsonActionResult(json) : Error(HttpStatusCode.BadRequest, "Password In Correct", result.Errors.ToList());
        }

        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/newslettersubscription")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult SubscribeNewsletter(string email, int storeId)
        {
            if (string.IsNullOrEmpty(email))
                return Error(HttpStatusCode.BadRequest, "Email", "Email is empty");

            var subscription =
                _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, storeId);
            if (subscription == null)
                _newsLetterSubscriptionService.InsertNewsLetterSubscription(
                    new Core.Domain.Messages.NewsLetterSubscription {Email = email, StoreId = storeId, Active = true});

            return Ok();
        }

        [HttpPost]
        [Route("/api/customers")]
        [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public IActionResult CreateCustomer([ModelBinder(typeof(JsonModelBinder<CustomerDto>))]
            Delta<CustomerDto> customerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            //If the validation has passed the customerDelta object won't be null for sure so we don't need to check for this.

            var existingCustomer = _customerService.GetCustomerByEmail(customerDelta.Dto.Email) ?? _customerService.GetCustomerByUsername(customerDelta.Dto.Username);

            if (existingCustomer != null)
                return Error(HttpStatusCode.BadRequest, "Username/Email",
                    _localizationService.GetResource("Account.CheckUsernameAvailability.Available"));


            // Inserting the new customer
            var newCustomer = _factory.Initialize();
            customerDelta.Merge(newCustomer);

            foreach (var address in customerDelta.Dto.Addresses)
            {
                // we need to explicitly set the date as if it is not specified
                // it will default to 01/01/0001 which is not supported by SQL Server and throws and exception
                if (address.CreatedOnUtc == null) address.CreatedOnUtc = DateTime.UtcNow;
                newCustomer.Addresses.Add(address.ToEntity());
            }

            newCustomer.Email = newCustomer.Email.ToLower();
            CustomerService.InsertCustomer(newCustomer);

            InsertGenericAttributes(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, customerDelta.Dto.Gender,
                customerDelta.Dto.DateOfBirth, customerDelta.Dto.Phone, customerDelta.Dto.Picture, newCustomer);

            if (!string.IsNullOrEmpty(customerDelta.Dto.LanguageId) && int.TryParse(customerDelta.Dto.LanguageId,
                                                                        out var languageId)
                                                                    && _languageService.GetLanguageById(languageId) !=
                                                                    null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.LanguageIdAttribute,
                    languageId);

            //password
            if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password))
                AddPassword(customerDelta.Dto.Password, newCustomer);

            // We need to insert the entity first so we can have its id in order to map it to anything.
            // TODO: Localization

            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            var newCustomerDto = newCustomer.ToDto();

            // This is needed because the entity framework won't populate the navigation properties automatically
            // and the country will be left null. So we do it by hand here.
            PopulateAddressCountryNames(newCustomerDto);

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            newCustomerDto.FirstName = customerDelta.Dto.FirstName;
            newCustomerDto.LastName = customerDelta.Dto.LastName;

            newCustomerDto.LanguageId = customerDelta.Dto.LanguageId;

            //activity log
            CustomerActivityService.InsertActivity("AddNewCustomer",
                LocalizationService.GetResource("ActivityLog.AddNewCustomer"), newCustomer);

            var customersRootObject = new CustomersRootObject();

            customersRootObject.Customers.Add(newCustomerDto);

            if (customerDelta.Dto.RoleIds.Count > 0)
            {
                AddValidRoles(customerDelta, newCustomer);

                CustomerService.UpdateCustomer(newCustomer);
            }

            var json = JsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/customers/{id}")]
        [GetRequestsErrorInterceptorActionFilter]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public IActionResult DeleteCustomer(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var customer = _customerApiService.GetCustomerEntityById(id);

            if (customer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            CustomerService.DeleteCustomer(customer);

            //remove newsletter subscription (if exists)
            foreach (var store in StoreService.GetAllStores())
            {
                var subscription =
                    _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                if (subscription != null)
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
            }

            //activity log
            CustomerActivityService.InsertActivity("DeleteCustomer",
                LocalizationService.GetResource("ActivityLog.DeleteCustomer"), customer);

            return new RawJsonActionResult("{}");
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/forget")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult Forget([ModelBinder(typeof(JsonModelBinder<ForgetDto>))]
            Delta<ForgetDto> forgetDelta)
        {
            if (!ModelState.IsValid) return Error();

            var customer = _customerService.GetCustomerByEmail(forgetDelta.Dto.Email);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                    passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                _genericAttributeService.SaveAttribute(customer,
                    NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                //send email
                _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer,
                    _workContext.WorkingLanguage.Id);

                var emailSent = new InformationDto
                {
                    Message = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent")
                };
                var json = JsonFieldsSerializer.Serialize(emailSent, string.Empty);
                return new RawJsonActionResult(json);
            }

            var notFoundEmail = new InformationDto
            {
                Message = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound")
            };
            var notFoundEmailjson = JsonFieldsSerializer.Serialize(notFoundEmail, string.Empty);
            return new RawJsonActionResult(notFoundEmailjson);
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
        [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCustomerById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var customer = _customerApiService.GetCustomerById(id);

            if (customer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            var customersRootObject = new CustomersRootObject();
            customersRootObject.Customers.Add(customer);

            var json = JsonFieldsSerializer.Serialize(customersRootObject, fields);

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
        [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCustomers(CustomersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var allCustomers = _customerApiService.GetCustomersDtos(parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId);

            var customersRootObject = new CustomersRootObject {Customers = allCustomers};

            var json = JsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customers/count")]
        [ProducesResponseType(typeof(CustomersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult GetCustomersCount()
        {
            var allCustomersCount = _customerApiService.GetCustomersCount();

            var customersCountRootObject = new CustomersCountRootObject {Count = allCustomersCount};

            return Ok(customersCountRootObject);
        }


        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/customers/logout")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult Logout()
        {
            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated,
                    "Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                        _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id),
                    _workContext.CurrentCustomer);

                _customerActivityService.InsertActivity("Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                        _workContext.OriginalCustomerIfImpersonated.Email,
                        _workContext.OriginalCustomerIfImpersonated.Id),
                    _workContext.OriginalCustomerIfImpersonated);

                //logout impersonated customer
                _genericAttributeService
                    .SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                        NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);
            }

            //activity log
            _customerActivityService.InsertActivity(_workContext.CurrentCustomer, "PublicStore.Logout",
                _localizationService.GetResource("ActivityLog.PublicStore.Logout"), _workContext.CurrentCustomer);

            //standard logout 
            _authenticationService.SignOut();

            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();

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
        [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public IActionResult Search(CustomersSearchParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page <= 0) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            var customersDto =
                _customerApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

            var customersRootObject = new CustomersRootObject {Customers = customersDto};

            var json = JsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/customers/{id}")]
        [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public IActionResult UpdateCustomer([ModelBinder(typeof(JsonModelBinder<CustomerDto>))]
            Delta<CustomerDto> customerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            // Updateting the customer
            var currentCustomer = _customerApiService.GetCustomerEntityById(customerDelta.Dto.Id);

            if (currentCustomer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            customerDelta.Merge(currentCustomer);

            if (customerDelta.Dto.RoleIds.Count > 0) AddValidRoles(customerDelta, currentCustomer);

            if (customerDelta.Dto.Addresses.Count > 0)
            {
                var currentCustomerAddresses =
                    currentCustomer.Addresses.ToDictionary(address => address.Id, address => address);

                foreach (var passedAddress in customerDelta.Dto.Addresses)
                {
                    var addressEntity = passedAddress.ToEntity();

                    if (currentCustomerAddresses.ContainsKey(passedAddress.Id))
                        _mappingHelper.Merge(passedAddress, currentCustomerAddresses[passedAddress.Id]);
                    else
                        currentCustomer.Addresses.Add(addressEntity);
                }
            }

            CustomerService.UpdateCustomer(currentCustomer);

            InsertGenericAttributes(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, customerDelta.Dto.Gender,
                customerDelta.Dto.DateOfBirth,
                customerDelta.Dto.Phone, customerDelta.Dto.Picture, currentCustomer);


            if (!string.IsNullOrEmpty(customerDelta.Dto.LanguageId) && int.TryParse(customerDelta.Dto.LanguageId,
                                                                        out var languageId)
                                                                    && _languageService.GetLanguageById(languageId) !=
                                                                    null)
                _genericAttributeService.SaveAttribute(currentCustomer, NopCustomerDefaults.LanguageIdAttribute,
                    languageId);

            //password
            if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password))
                AddPassword(customerDelta.Dto.Password, currentCustomer);

            // TODO: Localization

            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            var updatedCustomer = currentCustomer.ToDto();

            // This is needed because the entity framework won't populate the navigation properties automatically
            // and the country name will be left empty because the mapping depends on the navigation property
            // so we do it by hand here.
            PopulateAddressCountryNames(updatedCustomer);

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            var firstNameGenericAttribute = _genericAttributeService
                .GetAttributesForEntity(currentCustomer.Id, typeof(Core.Domain.Customers.Customer).Name)
                .FirstOrDefault(x => x.Key == "FirstName");

            if (firstNameGenericAttribute != null) updatedCustomer.FirstName = firstNameGenericAttribute.Value;

            var lastNameGenericAttribute = _genericAttributeService
                .GetAttributesForEntity(currentCustomer.Id, typeof(Core.Domain.Customers.Customer).Name)
                .FirstOrDefault(x => x.Key == "LastName");

            if (lastNameGenericAttribute != null) updatedCustomer.LastName = lastNameGenericAttribute.Value;

            var languageIdGenericAttribute = _genericAttributeService
                .GetAttributesForEntity(currentCustomer.Id, typeof(Core.Domain.Customers.Customer).Name)
                .FirstOrDefault(x => x.Key == "LanguageId");

            if (languageIdGenericAttribute != null) updatedCustomer.LanguageId = languageIdGenericAttribute.Value;

            //activity log
            CustomerActivityService.InsertActivity("UpdateCustomer",
                LocalizationService.GetResource("ActivityLog.UpdateCustomer"), currentCustomer);

            var customersRootObject = new CustomersRootObject();

            customersRootObject.Customers.Add(updatedCustomer);

            var json = JsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut]
        [Route("/api/customers/langauge/{id}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult SetLanguage(int id)
        {
            if (id <= 0)
                return BadRequest("Language not found");

            var language = _languageService.GetLanguageById(id);
            if (language?.Published ?? false)
                _workContext.WorkingLanguage = language;
            else
                return BadRequest("Language not found");

            return Ok();
        }

        private void AddPassword(string newPassword, Core.Domain.Customers.Customer customer)
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
                    var saltKey = _encryptionService.CreateSaltKey(5);
                    customerPassword.PasswordSalt = saltKey;
                    customerPassword.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey,
                        _customerSettings.HashedPasswordFormat);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CustomerService.InsertCustomerPassword(customerPassword);

            // TODO: remove this.
            CustomerService.UpdateCustomer(customer);
        }

        private void AddValidRoles(Delta<CustomerDto> customerDelta, Core.Domain.Customers.Customer currentCustomer)
        {
            var allCustomerRoles = CustomerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
                if (customerDelta.Dto.RoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (currentCustomer.CustomerCustomerRoleMappings.Count(mapping =>
                            mapping.CustomerRoleId == customerRole.Id) == 0)
                        currentCustomer.CustomerCustomerRoleMappings.Add(
                            new CustomerCustomerRoleMapping {CustomerRole = customerRole});
                }
                else
                {
                    if (currentCustomer.CustomerCustomerRoleMappings.Count(mapping =>
                            mapping.CustomerRoleId == customerRole.Id) > 0)
                        currentCustomer.CustomerCustomerRoleMappings
                            .Remove(currentCustomer.CustomerCustomerRoleMappings.FirstOrDefault(mapping =>
                                mapping.CustomerRoleId == customerRole.Id));
                }
        }

        private void InsertGenericAttributes(string firstName, string lastName, string gender,
            string dob, string phone, string picture, Core.Domain.Customers.Customer newCustomer)
        {
            // we assume that if the first name is not sent then it will be null and in this case we don't want to update it
            if (firstName != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.FirstNameAttribute, firstName);

            if (lastName != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.LastNameAttribute, lastName);

            if (gender != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.GenderAttribute, gender);

            if (phone != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.PhoneAttribute, phone);

            if (dob != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.DateOfBirthAttribute, dob);

            if (picture != null)
                _genericAttributeService.SaveAttribute(newCustomer, NopCustomerDefaults.AvatarPictureIdAttribute,
                    picture);
        }

        private void PopulateAddressCountryNames(CustomerDto newCustomerDto)
        {
            foreach (var address in newCustomerDto.Addresses) SetCountryName(address);

            if (newCustomerDto.BillingAddress != null) SetCountryName(newCustomerDto.BillingAddress);

            if (newCustomerDto.ShippingAddress != null) SetCountryName(newCustomerDto.ShippingAddress);
        }

        private void SetCountryName(AddressDto address)
        {
            if (!string.IsNullOrEmpty(address.CountryName) || !address.CountryId.HasValue) return;
            var country = _countryService.GetCountryById(address.CountryId.Value);
            address.CountryName = country.Name;
        }
    }
}