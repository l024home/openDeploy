namespace OpenDeploy.Domain.Convention;

/// <summary> 发布文件类型 </summary>
public class DeployFileType(string fileType, List<string> extensions)
{
    public string FileType { get; init; } = fileType;
    public List<string> Extensions { get; init; } = extensions;

    public static DeployFileType Html { get; } = new(nameof(Html), [".html", ".htm"]);
    public static DeployFileType CSS { get; } = new(nameof(CSS), [".css"]);
    public static DeployFileType JavaScript { get; } = new(nameof(JavaScript), [".js"]);
    public static DeployFileType Image { get; } = new(nameof(Image), [".png", ".jpg", ".jpeg", ".bmp"]);
    public static DeployFileType Aspx { get; } = new(nameof(Aspx), [".aspx"]);
    public static DeployFileType CS { get; } = new(nameof(CS), [".cs"]);
    public static DeployFileType Master { get; } = new(nameof(Master), [".master"]);
    public static DeployFileType UnKnown { get; } = new(nameof(UnKnown), []);

    /// <summary> 支持的文件类型 </summary>
    public static readonly List<DeployFileType> SupportedFileTypes =
    [
        CS,
        Html,
        Master,
        JavaScript,
        Aspx,
        CSS,
        Image
    ];

    /// <summary> 获取文件类型 </summary>
    public static DeployFileType Parse(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return UnKnown;
        }
        var ext = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(ext))
        {
            return UnKnown;
        }
        ext = ext.ToLower();
        foreach (var supportedFileType in SupportedFileTypes)
        {
            if (supportedFileType.Extensions.Contains(ext))
            {
                return supportedFileType;
            }
        }
        return UnKnown;
    }



    public static bool operator ==(DeployFileType one, DeployFileType other)
    {
        return one.FileType == other.FileType;
    }

    public static bool operator !=(DeployFileType one, DeployFileType other)
    {
        return !(one == other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (obj is DeployFileType fileType && fileType == this)
        {
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return FileType.GetHashCode();
    }

    public override string ToString()
    {
        return FileType;
    }
}
