using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDeploy.Domain.Models;

/// <summary> 解决方案领域模型 </summary>
[Table("Solution")]
public class Solution
{
    [Key]
    public Guid Id { get; set; }

    /// <summary> 解决方案名称 </summary>
    public string SolutionName { get; set; } = string.Empty;

    /// <summary> 解决方案所在文件夹 (git路径可能与sln路径不一致) </summary>
    public string SolutionDir { get; set; } = string.Empty;

    /// <summary> 解决方案Git仓储路径 </summary>
    public string GitRepositoryPath { get; set; } = string.Empty;

    /// <summary> 项目列表 </summary>
    public List<Project> Projects { get; set; } = [];
}
