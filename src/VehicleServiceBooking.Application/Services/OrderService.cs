using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Service for creating order aggregates.
/// </summary>
public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IServiceTypeRepository _serviceTypeRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICurrencyLookupRepository _currencyLookupRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IServiceTypeRepository serviceTypeRepository,
        IAppointmentRepository appointmentRepository,
        ICurrencyLookupRepository currencyLookupRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _serviceTypeRepository = serviceTypeRepository ?? throw new ArgumentNullException(nameof(serviceTypeRepository));
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _currencyLookupRepository = currencyLookupRepository ?? throw new ArgumentNullException(nameof(currencyLookupRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreateOrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedOrderCode = request.OrderCode.Trim();

        _logger.LogInformation(
            "Creating order: orderCode={OrderCode}, currencyId={CurrencyId}, serviceTypeItemCount={ServiceTypeItemCount}, appointmentCount={AppointmentCount}",
            normalizedOrderCode,
            request.CurrencyId,
            request.ServiceTypeItems.Count,
            request.AppointmentIds.Count);

        var currency = await _currencyLookupRepository
            .GetByIdAsync(request.CurrencyId, cancellationToken)
            .ConfigureAwait(false);
        if (currency == null)
        {
            throw new InvalidOperationException($"Currency '{request.CurrencyId}' was not found or is inactive.");
        }

        var orderCodeExists = await _orderRepository
            .OrderCodeExistsAsync(normalizedOrderCode, cancellationToken)
            .ConfigureAwait(false);
        if (orderCodeExists)
        {
            throw new OrderConflictException($"Order code '{normalizedOrderCode}' already exists.");
        }

        var serviceTypeIds = request.ServiceTypeItems
            .Select(x => x.ServiceTypeId)
            .Distinct()
            .ToList();

        var serviceTypes = (await _serviceTypeRepository
            .GetByIdsWithPricesAsync(serviceTypeIds, cancellationToken)
            .ConfigureAwait(false))
            .ToDictionary(x => x.Id, x => x);

        var missingServiceTypeIds = serviceTypeIds
            .Where(id => !serviceTypes.ContainsKey(id))
            .ToList();
        if (missingServiceTypeIds.Count > 0)
        {
            throw new InvalidOperationException(
                $"Service type(s) not found: {string.Join(", ", missingServiceTypeIds)}");
        }

        var appointmentIds = request.AppointmentIds
            .Distinct()
            .ToList();

        if (appointmentIds.Count > 0)
        {
            var appointments = await _appointmentRepository
                .GetByIdsAsync(appointmentIds, cancellationToken)
                .ConfigureAwait(false);

            var foundAppointmentIds = appointments.Select(x => x.Id).ToHashSet();
            var missingAppointmentIds = appointmentIds
                .Where(id => !foundAppointmentIds.Contains(id))
                .ToList();

            if (missingAppointmentIds.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Appointment(s) not found: {string.Join(", ", missingAppointmentIds)}");
            }

            var anyLinked = await _orderRepository
                .AnyAppointmentAlreadyLinkedAsync(appointmentIds, cancellationToken)
                .ConfigureAwait(false);
            if (anyLinked)
            {
                throw new OrderConflictException("One or more appointments are already linked to an existing order.");
            }
        }

        var utcNow = DateTime.UtcNow;
        var orderId = Guid.NewGuid();

        var pricedLineItems = request.ServiceTypeItems
            .Select(item =>
            {
                var serviceType = serviceTypes[item.ServiceTypeId];
                var priceRow = serviceType.ServiceTypePrices
                    .SingleOrDefault(x => x.IsActive && x.CurrencyId == request.CurrencyId);

                if (priceRow == null)
                {
                    throw new InvalidOperationException(
                        $"No active ServiceTypePrice is configured for service type '{item.ServiceTypeId}' and currency '{request.CurrencyId}'.");
                }

                var rawLineTotal = priceRow.Price * item.Quantity;

                return new
                {
                    item.ServiceTypeId,
                    item.Quantity,
                    UnitPrice = priceRow.Price,
                    RawLineTotal = rawLineTotal
                };
            })
            .ToList();

        var totalAmount = decimal.Round(
            pricedLineItems.Sum(x => x.RawLineTotal),
            currency.DecimalPlaces,
            MidpointRounding.ToEven);

        var pendingOrderPaymentStatus = await _orderRepository
            .GetOrderPaymentStatusAsync(OrderPaymentStatus.Pending, cancellationToken)
            .ConfigureAwait(false);

        if (pendingOrderPaymentStatus == null)
        {
            throw new InvalidOperationException("Order payment status 'Pending' is not configured or inactive.");
        }

        var order = new Order
        {
            Id = orderId,
            OrderCode = normalizedOrderCode,
            CurrencyId = request.CurrencyId,
            TotalAmount = totalAmount,
            PaymentStatusId = pendingOrderPaymentStatus.Id,
            AmountPaid = 0m,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            IsActive = true
        };

        var serviceTypeOrders = pricedLineItems
            .Select(item => new ServiceTypeOrder
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ServiceTypeId = item.ServiceTypeId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.RawLineTotal,
                CreatedAt = utcNow,
                UpdatedAt = utcNow,
                IsActive = true
            })
            .ToList();

        var orderAppointments = appointmentIds
            .Select(appointmentId => new OrderAppointment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                AppointmentId = appointmentId,
                CreatedAt = utcNow,
                UpdatedAt = utcNow,
                IsActive = true
            })
            .ToList();

        await _orderRepository
            .CreateAggregateAsync(order, serviceTypeOrders, orderAppointments, cancellationToken)
            .ConfigureAwait(false);

        var lineItems = pricedLineItems
            .Select(item =>
            {
                return new CreateOrderServiceTypeItemResponse
                {
                    ServiceTypeId = item.ServiceTypeId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.RawLineTotal
                };
            })
            .ToList();

        _logger.LogInformation(
            "Order created successfully: orderId={OrderId}, orderCode={OrderCode}, totalAmount={TotalAmount}",
            orderId,
            normalizedOrderCode,
            totalAmount);

        return new CreateOrderResponse
        {
            OrderId = orderId,
            OrderCode = normalizedOrderCode,
            CurrencyId = request.CurrencyId,
            TotalAmount = totalAmount,
            CreatedAt = order.CreatedAt,
            ServiceTypeItems = lineItems,
            AppointmentIds = appointmentIds
        };
    }

    public async Task<CreateOrderResponse?> GetOrderByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetAggregateByIdAsync(orderId, cancellationToken)
            .ConfigureAwait(false);

        if (order == null)
        {
            return null;
        }

        return new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            CurrencyId = order.CurrencyId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            ServiceTypeItems = order.ServiceTypeOrders
                .Select(item => new CreateOrderServiceTypeItemResponse
                {
                    ServiceTypeId = item.ServiceTypeId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                })
                .ToList(),
            AppointmentIds = order.OrderAppointments
                .Select(item => item.AppointmentId)
                .ToList()
        };
    }
}