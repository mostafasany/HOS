using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Modules.Order.Service
{
    public class OrderApiService : IOrderApiService
    {
        private readonly IRepository<Core.Domain.Orders.Order> _orderRepository;

        public OrderApiService(IRepository<Core.Domain.Orders.Order> orderRepository) => _orderRepository = orderRepository;

        public IList<Core.Domain.Orders.Order> GetOrdersByCustomerId(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            OrderStatus? status = null, PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, int? customerId = null,
            int? storeId = null)
        {
            IQueryable<Core.Domain.Orders.Order> query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, ids, customerId, storeId);

            if (sinceId > 0) query = query.Where(order => order.Id > sinceId);

            return new ApiList<Core.Domain.Orders.Order>(query, page - 1, limit);
        }

        public IList<Core.Domain.Orders.Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            OrderStatus? status = null, PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, int? customerId = null,
            int? storeId = null)
        {
            IQueryable<Core.Domain.Orders.Order> query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, ids, customerId, storeId);

            if (sinceId > 0) query = query.Where(order => order.Id > sinceId);

            return new ApiList<Core.Domain.Orders.Order>(query, page - 1, limit);
        }

        public Core.Domain.Orders.Order GetOrderById(int orderId)
        {
            if (orderId <= 0)
                return null;

            return _orderRepository.Table.FirstOrDefault(order => order.Id == orderId && !order.Deleted);
        }

        public int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
            int? customerId = null, int? storeId = null)
        {
            IQueryable<Core.Domain.Orders.Order> query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, customerId: customerId, storeId: storeId);

            return query.Count();
        }

        private IQueryable<Core.Domain.Orders.Order> GetOrdersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, IList<int> ids = null,
            int? customerId = null, int? storeId = null)
        {
            IQueryable<Core.Domain.Orders.Order> query = _orderRepository.Table;

            if (customerId != null) query = query.Where(order => order.CustomerId == customerId);

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (status != null) query = query.Where(order => order.OrderStatusId == (int) status);

            if (paymentStatus != null) query = query.Where(order => order.PaymentStatusId == (int) paymentStatus);

            if (shippingStatus != null) query = query.Where(order => order.ShippingStatusId == (int) shippingStatus);

            query = query.Where(order => !order.Deleted);

            if (createdAtMin != null) query = query.Where(order => order.CreatedOnUtc > createdAtMin.Value.ToUniversalTime());

            if (createdAtMax != null) query = query.Where(order => order.CreatedOnUtc < createdAtMax.Value.ToUniversalTime());

            if (storeId != null) query = query.Where(order => order.StoreId == storeId);

            query = query.OrderByDescending(order => order.CreatedOnUtc);

            //query = query.Include(c => c.Customer);
            //query = query.Include(c => c.BillingAddress);
            //query = query.Include(c => c.ShippingAddress);
            //query = query.Include(c => c.PickupAddress);
            //query = query.Include(c => c.RedeemedRewardPointsEntry);
            //query = query.Include(c => c.DiscountUsageHistory);
            //query = query.Include(c => c.GiftCardUsageHistory);
            //query = query.Include(c => c.OrderNotes);
            //query = query.Include(c => c.OrderItems);
            //query = query.Include(c => c.Shipments);

            return query;
        }
    }
}