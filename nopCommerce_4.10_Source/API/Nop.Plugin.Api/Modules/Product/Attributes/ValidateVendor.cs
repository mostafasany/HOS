using System.Collections.Generic;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Services.Vendors;

namespace Nop.Plugin.Api.Modules.Product.Attributes
{
    public class ValidateVendor : BaseValidationAttribute
    {
        private readonly Dictionary<string, string> _errors;

        private IVendorService _vendorService;

        public ValidateVendor() => _errors = new Dictionary<string, string>();

        private IVendorService VendorService
        {
            get
            {
                if (_vendorService == null) _vendorService = EngineContext.Current.Resolve<IVendorService>();

                return _vendorService;
            }
        }

        public override Dictionary<string, string> GetErrors() => _errors;

        public override void Validate(object instance)
        {
            var vendorId = 0;

            if (instance != null && int.TryParse(instance.ToString(), out vendorId))
                if (vendorId > 0)
                {
                    Vendor vendor = VendorService.GetVendorById(vendorId);

                    if (vendor == null) _errors.Add("Invalid vendor id", "Non existing vendor");
                }
        }
    }
}