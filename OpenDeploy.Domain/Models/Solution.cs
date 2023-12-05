using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenDeploy.Domain.Models;

/// <summary> 解决方案领域模型 </summary>
[Table("Solution")]
public class Solution
{
    [Key]
    public int Id { get; set; }

    /// <summary> 解决方案名称 </summary>
    public string SolutionName { get; set; } = string.Empty;

    /// <summary> 解决方案Git仓储路径 </summary>
    public string GitRepositoryPath { get; set; } = string.Empty;
}
