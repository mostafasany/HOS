﻿using System.Collections.Generic;

namespace Nop.Plugin.Api.Cart.Dto
{
    /// <summary>
    ///     Represents base nopCommerce model
    /// </summary>
    public class BaseNopModel
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        public BaseNopModel()
        {
            CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        #endregion

        #region Properties

        //MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add it to the model
        //public IFormCollection Form { get; set; }

        /// <summary>
        ///     Gets or sets property to store any custom values for models
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Perform additional actions for binding the model
        /// </summary>
        /// <param name="bindingContext">Model binding context</param>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom model binding</remarks>
        /// <summary>
        ///     Perform additional actions for the model initialization
        /// </summary>
        /// <remarks>
        ///     Developers can override this method in custom partial classes in order to add some custom initialization code
        ///     to constructors
        /// </remarks>
        protected virtual void PostInitialize()
        {
        }

        #endregion
    }
}