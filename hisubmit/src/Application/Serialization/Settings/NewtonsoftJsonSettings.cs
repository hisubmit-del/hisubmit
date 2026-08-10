
using HiSubmit.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace HiSubmit.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}