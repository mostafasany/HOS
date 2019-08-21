using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.Validators;

namespace Nop.Plugin.Api.Product.Modules.ProductAttributes.Dto.Validator
{
    public class ProductAttributeDtoValidator : BaseDtoValidator<ProductAttributeDto>
    {
        #region Constructors

        public ProductAttributeDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper,
            requestJsonDictionary)
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