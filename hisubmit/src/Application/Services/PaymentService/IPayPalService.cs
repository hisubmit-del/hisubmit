using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HiSubmit.Application.Services.PaymentService;

public interface IPayPalService
{
    Task<PayPalOrderResponse> VerifyOrderAsync(string orderId, decimal expectedAmount);
}


public class PayPalOrderResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("purchase_units")]
    public List<PurchaseUnit> PurchaseUnits { get; set; }

    [JsonPropertyName("payer")]
    public Payer Payer { get; set; }
}

public class PurchaseUnit
{
    [JsonPropertyName("amount")]
    public Amount Amount { get; set; }

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }
}

public class Amount
{
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class Payer
{
    [JsonPropertyName("email_address")]
    public string EmailAddress { get; set; }
}


public class PayPalTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}
