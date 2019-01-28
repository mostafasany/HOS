using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Admin.Model;
using Nop.Plugin.Api.Common.AutoMapper;


namespace Nop.Plugin.Api
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateClientToClientApiModelMap();
        }

        public int Order => 0;

        private static void CreateClientToClientApiModelMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Client, ClientApiModel>()
                .ForMember(x => x.ClientSecret, y => y.MapFrom(src => src.ClientSecrets.FirstOrDefault().Description))
                .ForMember(x => x.RedirectUrl, y => y.MapFrom(src => src.RedirectUris.FirstOrDefault().RedirectUri))
                .ForMember(x => x.AccessTokenLifetime, y => y.MapFrom(src => src.AccessTokenLifetime))
                .ForMember(x => x.RefreshTokenLifetime, y => y.MapFrom(src => src.AbsoluteRefreshTokenLifetime));
        }
    }
}