using System.IO.Compression;

namespace OpenDeploy.Infrastructure;

/// <summary>
/// 压缩和解压缩帮助类
/// </summary>
public class ZipHelper
{
    private static string EnsureZipDirCreated()
    {
        var zipDir = Path.Combine(Environment.CurrentDirectory, "ZipDir");
        if (!Directory.Exists(zipDir))
        {
            Directory.CreateDirectory(zipDir);
        }
        return zipDir;
    }

    /// <summary>
    /// 把多个文件添加到压缩包 (保留文件夹层级关系)
    /// </summary>
    public static async Task<ZipFileResult> CreateZipAsync(IEnumerable<ZipFileInfo> zipFileInfo)
    {
        return await Task.Run(() =>
        {
            var zipDir = EnsureZipDirCreated();
            var zipFileName = $"{DateTime.Now:yyyyMMdd_HHmmss_}{Guid.NewGuid()}.zip";
            var zipPath = Path.Combine(zipDir, zipFileName);
            using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update);
            foreach (var item in zipFileInfo)
            {
                archive.CreateEntryFromFile(item.FileAbsolutePath, item.FileRelativePath, CompressionLevel.SmallestSize);
            }
            return new ZipFileResult() { FullFileName = zipPath, FileName = zipFileName };
        });
    }


    /// <summary>
    /// 解压
    /// </summary>
    /// <param name="zipBytes">压缩包字节数组</param>
    /// <param name="zipFileName">压缩包文件名</param>
    public static string UnZip(byte[] zipBytes, string zipFileName)
    {
        string zipDir = EnsureZipDirCreated();

        var uploadFolder = Path.Combine(zipDir, zipFileName);
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        using var ms = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(ms);
        archive.ExtractToDirectory(uploadFolder);

        return uploadFolder;
    }
}

public class ZipFileInfo(string fileAbsolutePath, string fileRelativePath)
{
    public string FileAbsolutePath { get; set; } = fileAbsolutePath;
    public string FileRelativePath { get; set; } = fileRelativePath;
}

public class ZipFileResult
{
    public string FullFileName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
