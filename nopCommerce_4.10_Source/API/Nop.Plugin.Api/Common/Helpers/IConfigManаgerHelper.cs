using Nop.Core.Data;

namespace Nop.Plugin.Api.Common.Helpers
{
    public interface IConfigManagerHelper
    {
        void AddBindingRedirects();
        void AddConnectionString();
        DataSettings DataSettings { get; }
    }
}