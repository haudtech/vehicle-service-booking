namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Supported payment providers for customer checkout and payout operations.
/// </summary>
public enum PaymentProviderType
{
    ZaloPay = 1,
    Momo = 2,
    VnPay = 3,
    ShopeePay = 4,
    OnePay = 5
}
