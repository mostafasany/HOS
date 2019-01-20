using Nop.Core.Data;

namespace Nop.Plugin.Api.Common.Helpers
{
    public interface IConfigManagerHelper
    {
        DataSettings DataSettings { get; }
        void AddBindingRedirects();
        void AddConnectionString();
    }
}