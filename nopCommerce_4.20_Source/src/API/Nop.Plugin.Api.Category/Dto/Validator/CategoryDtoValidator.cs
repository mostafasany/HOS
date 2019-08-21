using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.Validators;

namespace Nop.Plugin.Api.Category.Dto.Validator
{
    public class CategoryDtoValidator : BaseDtoValidator<CategoryDto>
    {
        #region Constructors

        public CategoryDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper,
            requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Name, "invalid name", "name");
        }

        #endregion
    }
}