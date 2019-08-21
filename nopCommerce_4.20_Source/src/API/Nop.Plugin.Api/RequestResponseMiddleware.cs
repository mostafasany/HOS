using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Http;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Api
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
//            var headers = context.Request.Headers;
//            if (headers.ContainsKey("Accept-Language"))
//            {
//                var lan = headers["Accept-Language"];
//                if (lan.ToString() == "en")
//                {
//                    
//                }
//            }
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            context.Response.OnStarting(() =>
            {
                try
                {
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();

                    if (!context.Response.Headers.ContainsKey(cookieName))
                        context.Response.Headers.Add(cookieName, workContext.CurrentCustomer.CustomerGuid.ToString());
                    //context.Response.Cookies.Append(cookieName, workContext.CurrentCustomer.CustomerGuid.ToString());
                    return Task.FromResult(0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });


            await _next(context);
        }
    }
}