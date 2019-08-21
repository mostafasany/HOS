using System;

namespace Nop.Plugin.Api.IdentityServer
{
    public interface ICookiesService
    {
        void SetCustomerCookie(Guid customerGuid);
        void SetCustomerCookieAndHeader(Guid customerGuid);
    }
}