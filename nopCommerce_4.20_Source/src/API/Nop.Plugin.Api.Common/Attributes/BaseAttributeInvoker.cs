using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Common.Attributes
{
    public abstract class BaseValidationAttribute : Attribute
    {
        public abstract Dictionary<string, string> GetErrors();
        public abstract void Validate(object instance);
    }
}