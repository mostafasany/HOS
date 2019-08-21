using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Attributes;

namespace Nop.Plugin.Api.Common.DTOs.Product.Validator
{
    public class ProductTypeValidationAttribute : BaseValidationAttribute
    {
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

        public override Dictionary<string, string> GetErrors()
        {
            return _errors;
        }

        public override void Validate(object instance)
        {
            // Product Type is not required so it could be null
            // and there is nothing to validate in this case
            if (instance == null)
                return;

            var isDefined = Enum.IsDefined(typeof(ProductType), instance);

            if (!isDefined) _errors.Add("ProductType", "Invalid product type");
        }
    }
}