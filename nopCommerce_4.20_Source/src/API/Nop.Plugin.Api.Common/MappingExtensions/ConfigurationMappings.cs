using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.Domain;
using Nop.Plugin.Api.Common.Models;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class ConfigurationMappings
    {
        public static ApiSettings ToEntity(this ConfigurationModel apiSettingsModel)
        {
            return apiSettingsModel.MapTo<ConfigurationModel, ApiSettings>();
        }

        public static ConfigurationModel ToModel(this ApiSettings apiSettings)
        {
            return apiSettings.MapTo<ApiSettings, ConfigurationModel>();
        }
    }
}