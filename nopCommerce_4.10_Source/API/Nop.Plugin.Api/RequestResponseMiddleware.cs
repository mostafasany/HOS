using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Http;

namespace Nop.Plugin.Api
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestResponseMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Todo: Our logic that we need to put in when the request is coming in
            //var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            //if (context.Request.Headers.ContainsKey(cookieName))
            //{
            //    context.Response.Cookies.Delete(cookieName);
            //    var cookieExpires = 24 * 365; //TODO make configurable
            //    var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);
            //    var options = new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Expires = cookieExpiresDate
            //    };
            //    string nopCustomer = context.Request.Headers[cookieName];
            //    context.Request.Cookies.Append(cookieName, nopCustomer, options);
            //}

            //if (context.Request.Method == "OPTIONS")
            //{
            //    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            //    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            //    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            //    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //    context.Response.Headers.Add("Access-Control-Expose-Headers", cookieName);
            //}
            await _next(context);
            // Todo: Our logic that we need to put in when the response is going back
        }
    }
}