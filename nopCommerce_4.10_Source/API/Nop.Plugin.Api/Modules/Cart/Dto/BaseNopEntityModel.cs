namespace Nop.Plugin.Api.DTOs.ShoppingCarts
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