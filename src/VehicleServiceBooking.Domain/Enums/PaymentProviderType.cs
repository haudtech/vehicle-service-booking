namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Supported payment providers for customer checkout and payout operations.
/// </summary>
public enum PaymentProviderType
{
    ZaloPay = 1,
    Momo = 2,
    ApplePay = 3,
    Visa = 4,
    MasterCard = 5,
    VnPay = 6,
    AtmTransfer = 7,
    InternalWallet = 8
}
