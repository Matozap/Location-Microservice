using System.Text.Json;

namespace LocationService.Infrastructure.Extensions;

public static class JsonExtension
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T Deserialize<T>(this string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    public static string Serialize<T>(this T obj) => JsonSerializer.Serialize(obj, JsonSerializerOptions);
}