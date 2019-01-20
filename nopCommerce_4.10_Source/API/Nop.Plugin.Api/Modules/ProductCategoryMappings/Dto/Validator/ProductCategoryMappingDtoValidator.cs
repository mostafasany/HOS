using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.Validators;

namespace Nop.Plugin.Api.Modules.ProductCategoryMappings.Dto.Validator
{
    public class ProductCategoryMappingDtoValidator : BaseDtoValidator<ProductCategoryMappingDto>
    {
        #region Constructors

        public ProductCategoryMappingDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetCategoryIdRule();
            SetProductIdRule();
        }

        #endregion

        #region Private Methods

        private void SetCategoryIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.CategoryId, "invalid category_id", "category_id");
        }

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.ProductId, "invalid product_id", "product_id");
        }

        #endregion
    }
}