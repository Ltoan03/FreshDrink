using Microsoft.AspNetCore.Http;

namespace FreshDrink.Web
{
    public static class SessionExt
    {
        public static void SetObject(this ISession session, string key, object value)
            => session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));

        public static T? GetObject<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return data == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(data);
        }
    }
}
