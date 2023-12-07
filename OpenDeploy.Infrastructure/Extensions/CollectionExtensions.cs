namespace OpenDeploy.Infrastructure;

/// <summary>
/// 集合扩展
/// </summary>
public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this ICollection<T> values)
    {
        return values == null || values.Count == 0;
    }
}
