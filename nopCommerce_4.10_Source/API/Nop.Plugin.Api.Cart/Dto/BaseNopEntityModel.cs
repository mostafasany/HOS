namespace Nop.Plugin.Api.Modules.Cart.Dto
{
    /// <summary>
    ///     Represents base nopCommerce entity model
    /// </summary>
    public class BaseNopEntityModel : BaseNopModel
    {
        /// <summary>
        ///     Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}