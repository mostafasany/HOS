using System;
using Microsoft.AspNetCore.Http;
using Nop.Core.Http;

namespace Nop.Plugin.Api.IdentityServer
{
    public class CookiesService : ICookiesService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookiesService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            const int regCookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(regCookieExpires);

            //if passed guid is empty set cookie as expired
            if (customerGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            var options = new CookieOptions
            {
                HttpOnly = true, Secure = false, Expires = cookieExpiresDate, SameSite = SameSiteMode.None
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, customerGuid.ToString(), options);
        }

        public void SetCustomerCookieAndHeader(Guid customerGuid)
        {
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            SetCustomerCookie(customerGuid);
            _httpContextAccessor.HttpContext.Response.Headers.Remove(cookieName);
            _httpContextAccessor.HttpContext.Response.Headers.Add(cookieName, customerGuid.ToString());
        }
    }
}