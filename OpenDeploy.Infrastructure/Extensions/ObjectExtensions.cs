using System.Text.Json;

namespace OpenDeploy.Infrastructure.Extensions;

/// <summary> Object类型扩展 </summary>
public static class ObjectExtensions
{
    public static string ToJsonString(this object? obj)
    {
        if (obj is null)
        {
            return string.Empty;
        }
        return JsonSerializer.Serialize(obj);
    }
}
