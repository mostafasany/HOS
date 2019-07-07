using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.Validators;

namespace Nop.Plugin.Api.Common.DTOs.Product.Validator
{
    public class ProductDtoValidator : BaseDtoValidator<ProductDto>
    {
        #region Constructors

        public ProductDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.Name, "invalid name", "name");
        }

        #endregion
    }
}