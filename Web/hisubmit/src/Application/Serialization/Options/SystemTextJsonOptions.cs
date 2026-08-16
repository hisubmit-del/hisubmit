using HiSubmit.Application.Interfaces.Serialization.Options;
using System.Text.Json;

namespace HiSubmit.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}