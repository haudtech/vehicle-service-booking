namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Supported customer-facing payment methods/channels.
/// </summary>
public enum PaymentMethodType
{
    AtmCardDomestic = 1,
    CreditCard = 2,
    EWallet = 3,
    ApplePay = 4,
    InternalWallet = 5
}
