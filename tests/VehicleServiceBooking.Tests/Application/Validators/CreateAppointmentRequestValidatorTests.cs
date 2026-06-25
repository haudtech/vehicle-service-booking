using FluentAssertions;
using FluentValidation;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Validators;
using VehicleServiceBooking.Tests.Common;
using Xunit;

namespace VehicleServiceBooking.Tests.Application.Validators;

public class CreateAppointmentRequestValidatorTests
{
    private readonly CreateAppointmentRequestValidator _validator;

    public CreateAppointmentRequestValidatorTests()
    {
        _validator = new CreateAppointmentRequestValidator();
    }

    #region DealershipId Validation Tests

    [Fact]
    public void Validate_WithEmptyDealershipId_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyDealershipId()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "DealershipId");
        result.Errors.First(e => e.PropertyName == "DealershipId").ErrorCode.Should().Be("INVALID_DEALERSHIP_ID");
    }

    [Fact]
    public void Validate_WithValidDealershipId_ShouldPass()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "DealershipId");
    }

    #endregion

    #region CustomerId Validation Tests

    [Fact]
    public void Validate_WithEmptyCustomerId_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyCustomerId()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "CustomerId");
        result.Errors.First(e => e.PropertyName == "CustomerId").ErrorCode.Should().Be("INVALID_CUSTOMER_ID");
    }

    #endregion

    #region VehicleId Validation Tests

    [Fact]
    public void Validate_WithEmptyVehicleId_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyVehicleId()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "VehicleId");
        result.Errors.First(e => e.PropertyName == "VehicleId").ErrorCode.Should().Be("INVALID_VEHICLE_ID");
    }

    #endregion

    #region ServiceTypeId Validation Tests

    [Fact]
    public void Validate_WithEmptyServiceTypeId_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyServiceTypeId()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ServiceTypeId");
        result.Errors.First(e => e.PropertyName == "ServiceTypeId").ErrorCode.Should().Be("INVALID_SERVICE_TYPE_ID");
    }

    #endregion

    #region TechnicianId Validation Tests

    [Fact]
    public void Validate_WithEmptyTechnicianId_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyTechnicianId()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TechnicianId");
        result.Errors.First(e => e.PropertyName == "TechnicianId").ErrorCode.Should().Be("INVALID_TECHNICIAN_ID");
    }

    #endregion

    #region ServiceBayId Validation Tests

    [Fact]
    public void Validate_WithEmptyServiceBayId_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyServiceBayId()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ServiceBayId");
        result.Errors.First(e => e.PropertyName == "ServiceBayId").ErrorCode.Should().Be("INVALID_SERVICE_BAY_ID");
    }

    #endregion

    #region SlotStart Time Validation Tests

    [Fact]
    public void Validate_WithPastSlotStart_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithPastSlotTimes()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "SlotStart");
        result.Errors.First(e => e.PropertyName == "SlotStart").ErrorCode.Should().Be("INVALID_SLOT_START");
    }

    [Fact]
    public void Validate_WithFutureSlotStart_ShouldPass()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "SlotStart");
    }

    #endregion

    #region SlotEnd Time Validation Tests

    [Fact]
    public void Validate_WithSlotEndBeforeStart_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEndTimeBeforeStartTime()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "SlotEnd");
        result.Errors.First(e => e.PropertyName == "SlotEnd").ErrorCode.Should().Be("INVALID_SLOT_END");
    }

    [Fact]
    public void Validate_WithSlotEndAfterStart_ShouldPass()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "SlotEnd");
    }

    [Fact]
    public void Validate_WithSlotEndEqualToStart_ShouldFail()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEndTimeEqualToStartTime()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "SlotEnd");
    }

    #endregion

    #region Multiple Validation Errors Tests

    [Fact]
    public void Validate_WithMultipleInvalidFields_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .WithEmptyDealershipId()
            .WithEmptyCustomerId()
            .WithPastSlotTimes()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(3); // At least 3 errors
        result.Errors.Should().ContainSingle(e => e.PropertyName == "DealershipId");
        result.Errors.Should().ContainSingle(e => e.PropertyName == "CustomerId");
        result.Errors.Should().ContainSingle(e => e.PropertyName == "SlotStart");
    }

    [Fact]
    public void Validate_WithAllValidFields_ShouldPass()
    {
        // Arrange
        var request = new CreateAppointmentRequestBuilder()
            .Build();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion
}
