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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject;
            }

            return new Result<T>()
            {
                Succeeded = false,
                Messages = new List<string>() { "Http Response Error" + response.RequestMessage?.RequestUri +response.Content}
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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Result>
                (responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject;
            }

            return new Result()
            {
                Succeeded = false,
                Messages = new List<string>() { "Http Response Error" + response.RequestMessage?.RequestUri +response.Content }
            };
        }

        internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                return responseObject;
            }
            return new PaginatedResult<T>(new List<T>())
            {
                Succeeded = false,
                Messages = new List<string>() { "Http Response Error" + response.RequestMessage?.RequestUri +response.Content }
            };
        }
    }
}