namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for currencies supported by the booking and payment domain.
/// </summary>
public class CurrencyLookup : BaseEntity
{
    /// <summary>
    /// ISO-like currency code. Example: VND, USD.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the currency.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional display symbol. Example: USD, VND.
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Number of fractional digits used by this currency.
    /// </summary>
    public int DecimalPlaces { get; set; }

    public ICollection<ServiceTypePrice> ServiceTypePrices { get; set; } = new List<ServiceTypePrice>();

    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
