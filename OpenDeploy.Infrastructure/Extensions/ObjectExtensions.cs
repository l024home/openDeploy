using System.Text.Json;

namespace OpenDeploy.Infrastructure.Extensions;

/// <summary> Object类型扩展 </summary>
public static class ObjectExtensions
{
    private static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
    };

    public static string ToJsonString(this object? obj, bool indent = false)
    {
        if (obj is null)
        {
            return string.Empty;
        }
        if (indent)
        {
            return JsonSerializer.Serialize(obj, options);
        }
        else
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
