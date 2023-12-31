﻿using System.Text.Json.Serialization;
using System.Text;
using OpenDeploy.Infrastructure.Extensions;
using System.IO;

namespace OpenDeploy.Domain.Convention;

/// <summary> 发布文件信息 </summary>
public class DeployFileInfo
{
    /// <summary> 文件类型  </summary>
    [JsonIgnore]
    public DeployFileType FileType { get; set; } = default!;

    /// <summary> 拆分后的文件路径 </summary>
    [JsonIgnore]
    public List<string> SplitedFilePath { get; set; } = default!;

    /// <summary> 是否未知的文件类型 </summary>
    [JsonIgnore]
    public bool IsUnKnown { get; set; }


    /// <summary> Git更改文件相对路径 </summary>
    public string ChangedFileRelativePath { get; set; } = string.Empty;

    /// <summary> Git更改文件绝对路径 </summary>
    public string ChangedFileAbsolutePath { get; set; } = string.Empty;


    /// <summary> 待发布文件相对路径 </summary>
    public string PublishFileRelativePath { get; set; } = string.Empty;

    /// <summary> 待发布文件绝对路径 </summary>
    public string PublishFileAbsolutePath { get; set; } = string.Empty;


    /// <summary> 是否程序集文件类型 </summary>
    public bool IsDLL { get; set; }

    /// <summary> 文件名称 </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary> 项目名称 </summary>
    public string ProjectName { get; set; } = string.Empty;


    /// <summary>
    /// 根据Git更改路径,创建待发布文件信息
    /// </summary>
    public static DeployFileInfo Create(string changedFilePath)
    {
        //解析文件类型
        var fileType = DeployFileType.Parse(changedFilePath);

        var fileInfo = new DeployFileInfo
        {
            ChangedFileRelativePath = changedFilePath,
            FileType = fileType,
            SplitedFilePath = [.. changedFilePath.Split('/', StringSplitOptions.RemoveEmptyEntries)],
        };

        if (fileType == DeployFileType.CS)
        {
            fileInfo.IsDLL = true;
        }
        else if (fileType == DeployFileType.UnKnown)
        {
            fileInfo.IsUnKnown = true;
        }
        else
        {
            fileInfo.FileName = Path.GetFileName(changedFilePath);
        }
        return fileInfo;
    }

    public static DeployFileInfo Create(string projectName, string projectDLLPath, string dllName)
    {
        var fi = new DeployFileInfo
        {
            ProjectName = projectName,
            FileName = dllName,
            IsDLL = true,
            FileType = DeployFileType.CS,
            PublishFileAbsolutePath = Path.Combine(projectDLLPath, dllName)
        };
        return fi;
    }

    private static void GetRelativeFilePath(DeployFileInfo fi, int index)
    {
        var splitedFilePath = fi.SplitedFilePath;
        var len = splitedFilePath.Count;
        if (index < 0 || index > len)
        {
            return;
        }
        StringBuilder sb = new StringBuilder();
        for (; index < len; index++)
        {
            sb.Append(splitedFilePath[index]);
            if (index < len - 1)
            {
                sb.Append("\\");
            }
        }
        string relativePath = sb.ToString();
        fi.PublishFileRelativePath = relativePath;
        if (fi.IsDLL)
        {
            fi.PublishFileRelativePath = fi.FileName;
        }
    }

    public override string ToString()
    {
        return this.ToJsonString();
    }
}

public class DeployFileInfoComparer : IEqualityComparer<DeployFileInfo>
{
    public bool Equals(DeployFileInfo? x, DeployFileInfo? y)
    {
        if (x is null || y is null)
        {
            return false;
        }
        return x.PublishFileAbsolutePath == y.PublishFileAbsolutePath;
    }

    public int GetHashCode(DeployFileInfo obj)
    {
        return 0;
    }
}
