using System.Linq;
using System.Net;
using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Modules.ProductAttributes.Dto;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Dto;

namespace Nop.Plugin.Api.Product.Modules
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<ProductCategory, ProductCategoryMappingDto>();

            CreateProductMap();

            CreateMap<ProductAttributeValue, ProductAttributeValueDto>();

            CreateMap<ProductAttribute, ProductAttributeDto>();

            CreateMap<ProductSpecificationAttribute, ProductSpecificationAttributeDto>();

            CreateMap<SpecificationAttribute, SpecificationAttributeDto>();

            CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionDto>();
        }

        public int Order => 0;


        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }


        private void CreateProductMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Core.Domain.Catalog.Product, ProductDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.ProductAttributeMappings, y => y.Ignore())
                .ForMember(x => x.FullDescription, y => y.MapFrom(src => WebUtility.HtmlEncode(src.FullDescription)))
                .ForMember(x => x.Tags,
                    y => y.MapFrom(src => src.ProductProductTagMappings.Select(x => x.ProductTag.Name)));
        }
    }
}