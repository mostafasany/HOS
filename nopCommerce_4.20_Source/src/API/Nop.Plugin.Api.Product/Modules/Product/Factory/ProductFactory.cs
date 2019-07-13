using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Common.Factories;
using Nop.Services.Directory;

namespace Nop.Plugin.Api.Product.Modules.Product.Factory
{
    public class ProductFactory : IFactory<Core.Domain.Catalog.Product>
    {
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;

        public ProductFactory(IMeasureService measureService, MeasureSettings measureSettings)
        {
            _measureService = measureService;
            _measureSettings = measureSettings;
        }

        public Core.Domain.Catalog.Product Initialize()
        {
            var defaultProduct = new Core.Domain.Catalog.Product
            {
                Weight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Ratio,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                ProductType = ProductType.SimpleProduct,
                MaximumCustomerEnteredPrice = 1000,
                MaxNumberOfDownloads = 10,
                RecurringCycleLength = 100,
                RecurringTotalCycles = 10,
                RentalPriceLength = 1,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                UnlimitedDownloads = true,
                IsShipEnabled = true,
                AllowCustomerReviews = true,
                Published = true,
                VisibleIndividually = true
            };


            return defaultProduct;
        }
    }
}