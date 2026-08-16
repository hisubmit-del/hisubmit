using System.Collections.Generic;
using System.Net;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Extensions
{
    internal static class ResultExtensions
    {
    internal static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
    {
            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseAsString))
                    return new Result<T> { Succeeded = true };

                var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject;
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            return new Result<T>()
            {
                Succeeded = false,
                Messages = new List<string>
                {
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: " +
                    $"{response.RequestMessage?.RequestUri} {errorBody}".Trim()
                }
            };
        }

        internal static async Task<Result<T>> ToResult2<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        internal static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseAsString))
                    return new Result { Succeeded = true };

                var responseObject = JsonSerializer.Deserialize<Result>
                (responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject;
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            return new Result()
            {
                Succeeded = false,
                Messages = new List<string>
                {
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: " +
                    $"{response.RequestMessage?.RequestUri} {errorBody}".Trim()
                }
            };
        }

        internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                return responseObject;
            }
            var errorBody = await response.Content.ReadAsStringAsync();
            return new PaginatedResult<T>(new List<T>())
            {
                Succeeded = false,
                Messages = new List<string>
                {
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: " +
                    $"{response.RequestMessage?.RequestUri} {errorBody}".Trim()
                }
            };
        }
    }
}
