using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Customer.Modules.Customer.Dto;
using Nop.Plugin.Api.Customer.Modules.Customer.Service;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Api.IdentityServer
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICookiesService _cookiesService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;

        public PasswordValidator(ICustomerApiService customerApiService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerRegistrationService customerRegistrationService,
            IShoppingCartService shoppingCartService,
            IAuthenticationService authenticationService,
            ICookiesService cookiesService,
            IWorkContext workContext,
            IEventPublisher eventPublisher)
        {
            _customerApiService = customerApiService;
            _customerActivityService = customerActivityService;
            _customerRegistrationService = customerRegistrationService;
            _shoppingCartService = shoppingCartService;
            _authenticationService = authenticationService;
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _customerSettings = customerSettings;
            _cookiesService = cookiesService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userName = context.UserName;
            var password = context.Password;

            var loginResult = _customerRegistrationService.ValidateCustomer(userName, password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                {
                    var customer = _customerSettings.UsernamesEnabled
                        ? _customerService.GetCustomerByUsername(userName)
                        : _customerService.GetCustomerByEmail(userName);

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

                    _cookiesService.SetCustomerCookieAndHeader(customer.CustomerGuid);

                    var dict = new Dictionary<string, object>
                    {
                        {"grant_type", "password"},
                        {"email", customerDto.Email},
                        {"user_name", customerDto.Username},
                        {"id", customerDto.Id},
                        {"full_name", customerDto.FirstName + " " + customerDto.LastName},
                        {"provider", ""}
                    };
                    context.Result = new GrantValidationResult(
                        userName,
                        "Authenticated",
                        DateTime.Now,
                        CreateClaim(customerDto), "local", dict);
                }
                    break;
            }

            await Task.FromResult(context.Result);
        }

        private IEnumerable<Claim> CreateClaim(BaseCustomerDto userInfo)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Email, userInfo.Email), new Claim(JwtClaimTypes.Id, userInfo.Id.ToString())
            };
        }
    }
}