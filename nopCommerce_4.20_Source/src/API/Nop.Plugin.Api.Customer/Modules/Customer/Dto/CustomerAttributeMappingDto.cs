using Nop.Core.Domain.Common;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Dto
{
    public class CustomerAttributeMappingDto
    {
        public Core.Domain.Customers.Customer Customer { get; set; }
        public GenericAttribute Attribute { get; set; }
    }
}