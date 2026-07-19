using FluentAssertions;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Validators;

namespace VehicleServiceBooking.Tests.Application.Validators;

public class CreateOrderRequestValidatorTests
{
    private readonly CreateOrderRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldPass()
    {
        var request = CreateValidRequest();

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDuplicateServiceTypeIds_ShouldFail()
    {
        var serviceTypeId = Guid.NewGuid();
        var request = CreateValidRequest();
        request.ServiceTypeItems =
        [
            new CreateOrderServiceTypeItemRequest { ServiceTypeId = serviceTypeId, Quantity = 1 },
            new CreateOrderServiceTypeItemRequest { ServiceTypeId = serviceTypeId, Quantity = 2 }
        ];

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorCode == "DUPLICATE_SERVICE_TYPE");
    }

    [Fact]
    public void Validate_WithDuplicateAppointmentIds_ShouldFail()
    {
        var appointmentId = Guid.NewGuid();
        var request = CreateValidRequest();
        request.AppointmentIds = [appointmentId, appointmentId];

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorCode == "DUPLICATE_APPOINTMENT_ID");
    }

    [Fact]
    public void Validate_WithEmptyOrderCode_ShouldFail()
    {
        var request = CreateValidRequest();
        request.OrderCode = string.Empty;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorCode == "INVALID_ORDER_CODE");
    }

    [Fact]
    public void Validate_WithEmptyCurrencyId_ShouldFail()
    {
        var request = CreateValidRequest();
        request.CurrencyId = Guid.Empty;

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorCode == "INVALID_CURRENCY_ID");
    }

    private static CreateOrderRequest CreateValidRequest()
    {
        return new CreateOrderRequest
        {
            OrderCode = "ORD-2026-001",
            CurrencyId = Guid.Parse("00000000-0000-0000-0004-000000000001"),
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest
                {
                    ServiceTypeId = Guid.NewGuid(),
                    Quantity = 2
                }
            ],
            AppointmentIds = [Guid.NewGuid()]
        };
    }
}
