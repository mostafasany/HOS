using Nop.Core.Domain.Common;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class AddressDtoMappings
    {
        public static AddressDto ToDto(this Address address) => address.MapTo<Address, AddressDto>();

        public static Address ToEntity(this AddressDto addressDto) => addressDto.MapTo<AddressDto, Address>();
    }
}