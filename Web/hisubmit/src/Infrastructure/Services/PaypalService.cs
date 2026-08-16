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
        var accessToken = await GetAccessTokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync($"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<PayPalOrderResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (order.Status != "COMPLETED")
            throw new Exception("Payment not completed");

        var amount = decimal.Parse(order.PurchaseUnits[0].Amount.Value, CultureInfo.InvariantCulture);

        if (amount != expectedAmount)
            throw new Exception($"Expected amount {expectedAmount}, but got {amount}");

        return order;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = config["PayPal:ClientId"];
        var clientSecret = config["PayPal:ClientSecret"];

        var byteArray = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token")
        {
            Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<PayPalTokenResponse>(json);

        return token.AccessToken;
    }
}
