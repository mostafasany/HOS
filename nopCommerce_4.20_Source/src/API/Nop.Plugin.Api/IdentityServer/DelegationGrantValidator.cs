using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Customer.Modules.Customer.Dto;
using Nop.Plugin.Api.Customer.Modules.Customer.Service;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Api.IdentityServer
{
    public class DelegationGrantValidator : IExtensionGrantValidator
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICookiesService _cookiesService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITokenValidator _validator;
        private readonly IWorkContext _workContext;

        public DelegationGrantValidator(ICustomerApiService customerApiService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            IAuthenticationService authenticationService,
            ICookiesService cookiesService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            ITokenValidator validator,
            IEventPublisher eventPublisher)
        {
            _customerApiService = customerApiService;
            _customerActivityService = customerActivityService;
            _shoppingCartService = shoppingCartService;
            _authenticationService = authenticationService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _cookiesService = cookiesService;
            _validator = validator;
        }

        public string GrantType => "external";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userToken = context.Request.Raw.Get("external_token");
            var userEmail = context.Request.Raw.Get("email");
            var provider = context.Request.Raw.Get("provider");
            var fullName = context.Request.Raw.Get("full_name");
            var firstName = context.Request.Raw.Get("first_name");
            var lastName = context.Request.Raw.Get("last_name");
            var picture = context.Request.Raw.Get("picture");
            var phone = context.Request.Raw.Get("phone");
            var dob = context.Request.Raw.Get("dob");
            var gender = context.Request.Raw.Get("gender");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
                return;
            }

            //var result = await _validator.ValidateAccessTokenAsync(userToken);
            //if (result.IsError)
            //{
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            //    return;
            //}

            var customer = _customerService.GetCustomerByEmail(userEmail);
            if (customer == null)
            {
                customer = new Core.Domain.Customers.Customer
                {
                    Email = userEmail, Username = userEmail, Active = true, CustomerGuid = Guid.NewGuid()
                };
                _customerService.InsertCustomer(customer);
            }

            InsertGenericAttributes(firstName, lastName, gender, dob, phone, picture, customer);

            var customerDto = _customerApiService.GetCustomerById(customer.Id);

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

            AddValidRoles(customer, 3);

            _customerService.UpdateCustomer(customer);

            _cookiesService.SetCustomerCookieAndHeader(customer.CustomerGuid);

            var dict = new Dictionary<string, object>
            {
                {"grant_type", GrantType},
                {"email", customerDto.Email},
                {"user_name", customerDto.Username},
                {"id", customerDto.Id},
                {"full_name", firstName + " " + lastName},
                {"provider", provider}
            };
            context.Result = new GrantValidationResult(dict);
            await Task.FromResult(context.Result);
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

        private void AddValidRoles(Core.Domain.Customers.Customer currentCustomer, int roleId)
        {
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var customerRole = allCustomerRoles.FirstOrDefault(a => a.Id == roleId);
            if (currentCustomer.CustomerCustomerRoleMappings.Count(mapping =>
                    customerRole != null && mapping.CustomerRoleId == customerRole.Id) == 0)
                currentCustomer.CustomerCustomerRoleMappings.Add(
                    new CustomerCustomerRoleMapping {CustomerRole = customerRole});
        }
    }
}