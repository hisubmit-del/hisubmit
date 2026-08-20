using HiSubmit.Application.Services.PaymentService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HiSubmit.Infrastructure.Services;

public class PaypalService(HttpClient httpClient, IConfiguration config) : IPayPalService
{
    public async Task<PayPalOrderResponse> VerifyOrderAsync(string orderId, decimal expectedAmount)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new InvalidOperationException("PayPal order reference is required.");

        var accessToken = await GetAccessTokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync(
            $"{GetApiBaseUrl()}/v2/checkout/orders/{Uri.EscapeDataString(orderId)}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<PayPalOrderResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (order is null || !string.Equals(order.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("PayPal payment is not completed.");

        var purchaseUnit = order.PurchaseUnits?.FirstOrDefault();
        if (purchaseUnit?.Amount is null ||
            !decimal.TryParse(purchaseUnit.Amount.Value, NumberStyles.Number,
                CultureInfo.InvariantCulture, out var amount))
            throw new InvalidOperationException("PayPal payment amount was not returned.");

        var currency = config["PayPal:Currency"] ?? "USD";
        if (!string.Equals(purchaseUnit.Amount.CurrencyCode, currency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("PayPal payment currency is not supported.");

        if (amount != expectedAmount)
            throw new InvalidOperationException(
                $"Expected amount {expectedAmount.ToString(CultureInfo.InvariantCulture)}, but got {amount.ToString(CultureInfo.InvariantCulture)}.");

        return order;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = config["PayPal:ClientId"];
        var clientSecret = config["PayPal:ClientSecret"];

        var byteArray = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var request = new HttpRequestMessage(HttpMethod.Post, $"{GetApiBaseUrl()}/v1/oauth2/token")
        {
            Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<PayPalTokenResponse>(json);

        if (string.IsNullOrWhiteSpace(token?.AccessToken))
            throw new InvalidOperationException("PayPal access token was not returned.");

        return token.AccessToken;
    }

    private string GetApiBaseUrl() =>
        string.Equals(config["PayPal:Environment"], "Live", StringComparison.OrdinalIgnoreCase)
            ? "https://api-m.paypal.com"
            : "https://api-m.sandbox.paypal.com";
}
