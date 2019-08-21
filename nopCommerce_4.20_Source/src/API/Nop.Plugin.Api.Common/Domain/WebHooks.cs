using Nop.Core;

namespace Nop.Plugin.Api.Common.Domain
{
    public class WebHooks : BaseEntity
    {
        public string User { get; set; }

        public string ProtectedData { get; set; }

        public byte[] RowVer { get; set; }
    }
}