using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace FreshDrink.Web.Infrastructure
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            session.SetString(key, json);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json!, _jsonOptions);
        }
    }
}
