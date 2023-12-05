using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDeploy.Domain.Models;

/// <summary> 项目领域模型 (一个解决方案可能包含多个项目) </summary>
[Table("Project")]
public class Project
{
    [Key]
    public int Id { get; set; }

    /// <summary> 解决方案ID </summary>
    public int SolutionId { get; set; }

    /// <summary> 解决方案名称 </summary>
    public string SolutionName { get; set; } = string.Empty;

    /// <summary> 项目名称 </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary> 项目路径 </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary> 项目编译输出路径 </summary>
    public string PublishPath { get; set; } = string.Empty;
}
