using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    public class AuthorizeController : BaseApiController
    {
//#if DEBUG
//        private readonly string _clientId = "de2c9a4c-c982-4c3b-bc0f-cb9b6a6463e9";
//        private readonly string _clientSecret = "45a80076-62b0-455f-90e9-8bb481d4df02";
//        private readonly string _redirectUrl = "http://localhost:15536/api/authorize";
//        private readonly string _serverUrl = "http://localhost:15536";
//#elif !DEBUG
        private readonly string _clientId = "1d21bd69-4c79-4036-9ee0-cf7318887807";
        private readonly string _clientSecret = "56a67572-d9e5-45e4-990e-74a947297199";
        private readonly string _redirectUrl = "http://houseofsupplements.azurewebsites.net/api/authorize";
        private readonly string _serverUrl = "http://houseofsupplements.azurewebsites.net";
//#endif
        public AuthorizeController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService, ILanguageService languageService) :
            base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService,
                customerActivityService, localizationService, pictureService)
        {
        }

        public string AuthorizeClient(string code, string grantType, string redirectUrl)
        {
            string requestUriString = string.Format("{0}/api/token", _serverUrl);

            string queryParameters = $"client_id={_clientId}&client_secret={_clientSecret}&code={code}&grant_type={grantType}&redirect_uri={redirectUrl}";

            var httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUriString);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            using (new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(queryParameters);
                    streamWriter.Close();
                }
            }

            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            string json = string.Empty;

            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    json = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }

            return json;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("/api/authorize")]
        public ActionResult GetAccessToken(string code, string state)
        {
            string json = AuthorizeClient(code, "authorization_code", _redirectUrl);
            return Json(json);
        }

        public string GetAuthorizationUrl(string redirectUrl, string[] scope, string state = null)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0}/oauth/authorize", _serverUrl);
            stringBuilder.AppendFormat("?client_id={0}", HttpUtility.UrlEncode(_clientId));
            stringBuilder.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(redirectUrl));
            stringBuilder.Append($"&response_type={OidcConstants.ResponseTypes.Code}");

            if (!string.IsNullOrEmpty(state)) stringBuilder.AppendFormat("&state={0}", state);

            if (scope != null && scope.Length > 0)
            {
                string scopeJoined = string.Join(",", scope);
                stringBuilder.AppendFormat("&scope={0}", HttpUtility.UrlEncode(scopeJoined));
            }

            return stringBuilder.ToString();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/api/authorize/token")]
        public ActionResult GetAuthorizeUrl()
        {
            Guid state = Guid.NewGuid();
            string authUrl = GetAuthorizationUrl(_redirectUrl, new string[] { }, state.ToString());
            RedirectResult result = Redirect(authUrl);
            return result;
        }
    }
}